using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using googlecloud1.UriCollect;
using Nemiro.OAuth;
namespace googlecloud1.login
{
    public class AllLogin
    {
        public delegate void SafeComplete(object sender);
        Thread thread;
        public event SafeComplete SC;
        private bool Stopped;
        List<Authentication.UserNameToken> token;
        List<Authentication.TokenResult> apptoken;
        public List<DriveInfo> drive {get;set;}
        string path;
        public AllLogin(List<Authentication.UserNameToken> token, string path)
        {
            this.thread = new Thread(new ThreadStart(this.StreamThread));
            this.token = token;
            this.path = path;
            apptoken = new List<Authentication.TokenResult>();
            drive = new List<DriveInfo>();
        }
        public void Start()
        {
            if ((this.thread != null) & (this.thread.ThreadState == ThreadState.Stopped))
                this.Stop();
            thread = new Thread(new ThreadStart(this.StreamThread));
            this.Stopped = false;
            thread.Start();
        }
        public void Stop()
        {
            this.Stopped = true;
            if (thread != null)
                thread.Abort();
        }
        private void oncomplet(object sender)
        {
            if (this.SC != null)
                this.SC(sender);
        }
        private async void StreamThread()
        {
            foreach (var item in token)
            {
                Authentication.TokenResult appToken = null;
                if(item.Token.Drive == "Google")
                {
                    appToken = await WebAuthorization.RedeemAuthorizationCodeAsync(Client.GoogleClientID, UriCollection.GoogleinstalledAppRedirectUri, Client.GoogleClientSecret,
                    item.Token.refresh_token, UriCollection.GoogleTokenUri, "refresh_token", WebAuthorization.TokenOption.REFRESHTOKEN);
                    appToken.refresh_token = item.Token.refresh_token;
                    appToken.Drive = item.Token.Drive;
                }
                else if(item.Token.Drive == "OneDrive")
                {
                    appToken = await WebAuthorization.RedeemAuthorizationCodeAsync(Client.OneDriveClientID, UriCollection.OneDriveinstalledAppRedirectUri, Client.OneDriveClientSecret,
                    item.Token.refresh_token, UriCollection.OneDriveTokenUri, "refresh_token", WebAuthorization.TokenOption.REFRESHTOKEN);
                    appToken.refresh_token = item.Token.refresh_token;
                    appToken.Drive = item.Token.Drive;
                }
                else if (item.Token.Drive == "DropBox")
                {
                    appToken = item.Token;
                }
                if(appToken != null)
                {
                    this.apptoken.Add(appToken);
                    Authentication.UserNameToken unametoken = new Authentication.UserNameToken();
                    DriveInfo driveinfo = null;
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
                        var result = await HttpHelper.RequstHttp("GET", "https://api.dropbox.com/1/account/info", null, appToken.access_token);
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
                    unametoken.UserName = item.UserName;
                    unametoken.Token = appToken;
                    drive.Add(driveinfo);
                    await Authentication.SaveToken(new FileDataStore(path), item.UserName, unametoken, CancellationToken.None);
                }
            }
            this.oncomplet(this);
            this.Stop();
        }
    }
}
