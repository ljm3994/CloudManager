
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Nemiro.OAuth;
using Nemiro.OAuth.Extensions;
using System.Collections.Specialized;
using googlecloud1.login;
using googlecloud1.UriCollect;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using googlecloud1;
namespace googlecloud1.login
{
    public class Authentication
    {
        public delegate void SafeComplete(object sender);
        Thread thread;
        private bool Stopped;
        string clientId;
        string clientSecret;
        public event SafeComplete complete;
        string userName;
        string path;
        string[] scopes;
        LoginOption option;
        public DriveInfo driveinfo { get; set; }
        public TokenResult token { get; set; }
        public string code { get; set; }
        public Authentication(string clientid, string clientsectet, string username, string path, string[] scopes, LoginOption option, string code = null)
        {
            this.clientId = clientid;
            this.clientSecret = clientsectet;
            this.userName = username;
            this.path = path;
            this.scopes = scopes;
            this.option = option;
            this.code = code;
            this.thread = new Thread(new ThreadStart(this.CloudLogin));
        }
        public void start()
        {
            if ((this.thread != null) & (this.thread.ThreadState == ThreadState.Stopped))
                this.Stop();
            thread = new Thread(new ThreadStart(this.CloudLogin));
            this.Stopped = false;
            thread.Start();
        }
        protected void OnComplete(object sender)
        {
            if (this.complete != null)
                this.complete(sender);
        }
        private void Stop()
        {
            this.Stopped = true;
            if (thread != null)
                thread.Abort();
        }
        /// <summary>
        /// Oauth 2.0을 사용하여 유저 정보를 가져온다.
        /// </summary>
        /// <param name="clientId">Developer console에서 발급받은 userid</param>
        /// <param name="clientSecret">Developer console에서 발급받은 보안 번호</param>
        /// <param name="userName">사용자를 구별하기 위한 유저 이름 (닉네임)</param>
        /// <returns></returns>
        public async void CloudLogin()
        {
            string starturi = null;
            string enduri = null;
            string tokenuri = null;
            string granttype = null;
            string drive = null;
            if(option == LoginOption.GoogleDrive)
            {
                starturi = UriCollection.GoogleAuthorizationUrl;
                enduri = UriCollection.GoogleinstalledAppRedirectUri;
                tokenuri = UriCollection.GoogleTokenUri;
                userName += "Google";
                granttype = "refresh_token";
                drive = "Google";
            }
            else if(option == LoginOption.OneDrive)
            {
                starturi = UriCollection.OneDriveAuthorizationUri;
                enduri = UriCollection.OneDriveinstalledAppRedirectUri;
                tokenuri = UriCollection.OneDriveTokenUri;
                userName += "OneDrive";
                granttype = "refresh_token";
                drive = "OneDrive";
            }
            else
            {
                starturi = UriCollection.DropBoxAuthorizationUri;
                tokenuri = UriCollection.DropBoxTokenUri;
                userName += "DropBox";
                granttype = "authorization_code";
                drive = "DropBox";
            }
            try
            {
                FileDataStore datastore = new FileDataStore(path);
                UserNameToken oldRefreshToken = await LoadToken(datastore, userName, CancellationToken.None).ConfigureAwait(false);
                UserNameToken unametoken = new UserNameToken();
                TokenResult appToken = null;
                if (oldRefreshToken != null)
                {
                    if(oldRefreshToken.Token.refresh_token != null)
                    {
                        appToken = await WebAuthorization.RedeemAuthorizationCodeAsync(clientId, enduri, clientSecret, oldRefreshToken.Token.refresh_token, tokenuri, granttype, WebAuthorization.TokenOption.REFRESHTOKEN);
                        if (option == LoginOption.GoogleDrive)
                        {
                            appToken.refresh_token = oldRefreshToken.Token.refresh_token;
                        }
                        appToken.Drive = drive;
                    }
                    else
                    {
                        appToken = oldRefreshToken.Token;
                    }
                }
                // 신규 유저 접근 권한을 받아오거나 혹은 저장되어 있는 (기본 위치 C:\Users\bit-user\AppData\Roaming\)에 토큰을 가지고 유저정보를 가져온다.
                if(appToken == null)
                {
                    if(code != null)
                    {
                        appToken = await WebAuthorization.RedeemAuthorizationCodeAsync(clientId, enduri, clientSecret, code, tokenuri, "authorization_code", WebAuthorization.TokenOption.CODE);
                        appToken.Drive = drive;
                    }
                }
                if (appToken != null)
                {
                    if (appToken.Drive == "Google")
                    {
                        var result = await HttpHelper.RequstHttp("GET", "https://www.googleapis.com/drive/v2/about", null, appToken.access_token);
                        Dictionary<string, object> drivetext = HttpHelper.DerealizeJson(result.GetResponseStream());
                        var map = new ApiDataMapping();
                        map.Add("name", "DisplayName");
                        map.Add("quotaBytesTotal", "TotalSize", typeof(long));
                        map.Add("quotaBytesUsed", "UseSize", typeof(long));
                        map.Add("quotaBytesUsedAggregate", "EmptySize", typeof(long));
                        map.Add("quotaBytesUsedInTrash", "DeleteSize", typeof(long));
                        map.Add("quotaType", "DriveType");
                        map.Add("rootFolderId", "DriveID");
                        Dictionary<string, object> di = (Dictionary<string, object>)drivetext["user"];
                        driveinfo = new DriveInfo(drivetext, map);
                        driveinfo.UserID = di["emailAddress"].ToString();
                        driveinfo.Status = "normal";
                        driveinfo.token = appToken;
                    }
                    else if (appToken.Drive == "OneDrive")
                    {
                        var result = await HttpHelper.RequstHttp("GET", "https://api.onedrive.com/v1.0/drive", null, appToken.access_token);
                        Dictionary<string, object> drivetext = HttpHelper.DerealizeJson(result.GetResponseStream());
                        Dictionary<string, object> owner = (Dictionary<string, object>)drivetext["owner"];
                        Dictionary<string, object> user = (Dictionary<string, object>)owner["user"];
                        Dictionary<string, object> quoat = (Dictionary<string, object>)drivetext["quota"];
                        var map = new ApiDataMapping();
                        map.Add("id", "DriveID", typeof(string));
                        map.Add("driveType", "DriveType");
                        driveinfo = new DriveInfo(drivetext, map);
                        driveinfo.DisplayName = user["displayName"].ToString();
                        driveinfo.UserID = user["id"].ToString();
                        driveinfo.TotalSize = long.Parse(quoat["total"].ToString());
                        driveinfo.EmptySize = long.Parse(quoat["remaining"].ToString());
                        driveinfo.UseSize = long.Parse(quoat["used"].ToString());
                        driveinfo.DeleteSize = long.Parse(quoat["deleted"].ToString());
                        driveinfo.Status = quoat["state"].ToString();
                        driveinfo.token = appToken;
                    }
                    else if (appToken.Drive == "DropBox")
                    {
                        var result = await HttpHelper.RequstHttp("GET", "https://api.dropbox.com/1/account/info",null, appToken.access_token);
                        Dictionary<string, object> drivetext = HttpHelper.DerealizeJson(result.GetResponseStream());
                        Dictionary<string, object> quato = (Dictionary<string, object>)drivetext["quota_info"];
                        var map = new ApiDataMapping();
                        map.Add("uid", "UserId", typeof(string));
                        map.Add("display_name", "DisplayName");
                        driveinfo = new DriveInfo(drivetext, map);
                        driveinfo.TotalSize = long.Parse(quato["quota"].ToString());
                        driveinfo.UseSize = long.Parse(quato["normal"].ToString());
                        driveinfo.EmptySize = driveinfo.TotalSize - driveinfo.UseSize;
                        driveinfo.token = appToken;
                        driveinfo.Status = "normal";
                    }
                    appToken.uid = driveinfo.UserID;
                    unametoken.UserName = this.userName;
                    unametoken.Token = appToken;
                    SaveRefreshToken(unametoken, datastore, userName);
                    token = appToken;
                }
            }
            catch (Exception ex)
            {
            }
            this.OnComplete(this);
            this.Stop();
        }
        private static async void SaveRefreshToken(UserNameToken AppToken, FileDataStore datastore, string userid)
        {
            if (AppToken != null)
            {
                UserNameToken settings = AppToken;
                await SaveToken(datastore, userid, settings, CancellationToken.None);
            }
        }
        public static async Task<UserNameToken> LoadToken(FileDataStore datastore, string userid, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            if (datastore != null)
            {
                return await datastore.GetAsync<UserNameToken>(userid).ConfigureAwait(false);
            }
            return null;
        }
        public static async Task<List<UserNameToken>> AllLoadToke(FileDataStore datastore, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            if(datastore != null)
            {
                List<UserNameToken> token = await datastore.AllLoad<UserNameToken>();
                return token;
            }
            return null;
        }
        public static async Task SaveToken(FileDataStore datastore, string userid, UserNameToken token, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            if (datastore != null)
            {
                await datastore.StoreAsync<UserNameToken>(userid, token);
            }
        }
        public static async Task DeleteToken(FileDataStore datastore, string userid, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            if (datastore != null)
            {
                await datastore.DeleteAsync<UserNameToken>(userid);
            }
        }
        public static async Task ClearToken(FileDataStore datastore, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            if (datastore != null)
            {
                await datastore.ClearAsync();
            }
        }
        [System.ComponentModel.ComplexBindingProperties("access_token")]
        public class TokenResult
        {
            public string access_token { get; set; }
            public string uid { get; set; }
            public string token_type { get; set; }
            public int expires_in { get; set; }
            public string authentication_token { get; set; }
            public string refresh_token { get; set; }
            public string scope { get; set; }
            public string Drive { get; set; }
        }
        public class UserNameToken
        {
            public string UserName { get; set; }
            public TokenResult Token { get; set; }
        }
    }
}