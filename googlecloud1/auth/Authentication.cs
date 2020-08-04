﻿/*
 * Copyright 2014 Daimto.com
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Services;
using googlecloud1.login;
using MicrosoftAccount.WindowsForms;
using OneDrive;
using System.Threading.Tasks;
namespace Daimto.Drive.api
{
    public class Authentication
    {
        /// <summary>
        /// 구글의 Oauth 2.0을 사용하여 유저 정보를 가져온다.
        /// </summary>
        /// <param name="clientId">Developer console에서 발급받은 userid</param>
        /// <param name="clientSecret">Developer console에서 발급받은 보안 번호</param>
        /// <param name="userName">사용자를 구별하기 위한 유저 이름 (닉네임)</param>
        /// <returns></returns>
        public static DriveService AuthenticateOauth(string clientId, string clientSecret, string userName)
        {

            //Google Drive scopes Documentation:   https://developers.google.com/drive/web/scopes
            string[] scopes = new string[] { DriveService.Scope.Drive,  //Google 드라이브에서 파일 보기 및 관리
                                             DriveService.Scope.DriveAppdata,  //Google 드라이브에서 설정 데이터 조회 및 관리
                                             DriveService.Scope.DriveAppsReadonly,   // Google 드라이브 앱 조회
                                             DriveService.Scope.DriveFile,   // 이 앱으로 열거나 만든 Google 드라이브 파일과 폴더 조회 및 관리
                                             DriveService.Scope.DriveMetadataReadonly,   // Google 드라이브에서 파일의 메타데이터 보기
                                             DriveService.Scope.DriveReadonly,   // Google 드라이브에서 파일 보기
                                             DriveService.Scope.DriveScripts };  // Google Apps Script 스크립트의 행동 변경


            try
            {
                // 신규 유저 접근 권한을 받아오거나 혹은 저장되어 있는 (기본 위치 C:\Users\bit-user\AppData\Roaming\)에 토큰을 가지고 유저정보를 가져온다.
                UserCredential credential = GoogleWebAuthorization.LoginAuthorizationCodeFlowAsync(new ClientSecrets { ClientId = clientId, ClientSecret = clientSecret }
                                                                                             , scopes
                                                                                             , userName
                                                                                             , CancellationToken.None
                                                                                             , new FileDataStore("Daimto.Drive.Auth.Store")).Result;
                // 받아온 유저의 정보를 이용하여 google drive 에 연결한다.
                DriveService service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "GoogleCloude Sample",
                });
                return service;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        /// <summary>
        /// Authenticating to Google using a Service account
        /// Documentation: https://developers.google.com/accounts/docs/OAuth2#serviceaccount
        /// </summary>
        /// <param name="serviceAccountEmail">From Google Developer console https://console.developers.google.com</param>
        /// <param name="keyFilePath">Location of the Service account key file downloaded from Google Developer console https://console.developers.google.com</param>
        /// <returns></returns>
        public static DriveService AuthenticateServiceAccount(string serviceAccountEmail, string keyFilePath)
        {

            // check the file exists
            if (!File.Exists(keyFilePath))
            {
                Console.WriteLine("An Error occurred - Key file does not exist");
                return null;
            }

            //Google Drive scopes Documentation:   https://developers.google.com/drive/web/scopes
            string[] scopes = new string[] { DriveService.Scope.Drive,  // view and manage your files and documents
                                             DriveService.Scope.DriveAppdata,  // view and manage its own configuration data
                                             DriveService.Scope.DriveAppsReadonly,   // view your drive apps
                                             DriveService.Scope.DriveFile,   // view and manage files created by this app
                                             DriveService.Scope.DriveMetadataReadonly,   // view metadata for files
                                             DriveService.Scope.DriveReadonly,   // view files and documents on your drive
                                             DriveService.Scope.DriveScripts };  // modify your app scripts     

            var certificate = new X509Certificate2(keyFilePath, "notasecret", X509KeyStorageFlags.Exportable);
            try
            {
                ServiceAccountCredential credential = new ServiceAccountCredential(
                    new ServiceAccountCredential.Initializer(serviceAccountEmail)
                   {
                       Scopes = scopes
                   }.FromCertificate(certificate));

                // Create the service.
                DriveService service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Daimto Drive API Sample",
                });
                return service;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                return null;

            }
        }
    }
    public class OneDriveLogin
    {
        private const string msa_client_id = "0000000044128B55";
        private const string msa_client_secret = "amw-eMF4Ps-jzDVv6qwL4scqp2iFI29l";
        public static async Task<ODConnection> SignInToMicrosoftAccount(string userid, string path)
            {
                FileDataStore datastore = new FileDataStore(path);
                AppTokenResult oldRefreshToken = await LoadToken(datastore, userid, CancellationToken.None).ConfigureAwait(false);
                AppTokenResult appToken = null;
                if (oldRefreshToken != null)
                {
                    appToken = await MicrosoftAccountOAuth.RedeemRefreshTokenAsync(msa_client_id, msa_client_secret, oldRefreshToken.RefreshToken);
                }

                if (null == appToken)
                {
                    string code = await loginform.GetAuthenticationToken(msa_client_id, new[] { "wl.offline_access", "wl.basic", "wl.signin", "onedrive.readwrite" }, userid, "https://login.live.com/oauth20_authorize.srf", "https://login.live.com/oauth20_desktop.srf", LoginOption.OneDrive);

                    appToken = await OneDriveWebAuthorization.RedeemAuthorizationCodeAsync(msa_client_id, "https://login.live.com/oauth20_desktop.srf", msa_client_secret, code);
                }                       

                if (null != appToken)
                {
                    SaveRefreshToken(appToken, datastore, userid);

                    return new ODConnection("https://api.onedrive.com/v1.0", new OAuthTicket(appToken));
                }
             
                return null;
            }

        private static async void SaveRefreshToken(AppTokenResult AppToken, FileDataStore datastore, string userid)
        {
            if (AppToken != null)
            {
                AppTokenResult settings = AppToken;
                await SaveToken(datastore, userid, settings, CancellationToken.None);
            }
        }
        public static async Task<AppTokenResult> LoadToken(FileDataStore datastore, string userid, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            if (datastore != null)
            {
                return await datastore.GetAsync<AppTokenResult>(userid).ConfigureAwait(false);
            }
            return null;
        }
        public static async Task SaveToken(FileDataStore datastore, string userid, AppTokenResult token, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            if (datastore != null)
            {
                await datastore.StoreAsync<AppTokenResult>(userid, token);
            }
        }
        public static async Task DeleteToken(FileDataStore datastore, string userid, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            if (datastore != null)
            {
                await datastore.DeleteAsync<AppTokenResult>(userid);
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
        public static async Task<AppTokenResult> RenewAccessTokenAsync(OAuthTicket ticket, FileDataStore datastore, string userid)
        {
            string oldRefreshToken = ticket.RefreshToken;
            AppTokenResult appToken = null;

            if (!string.IsNullOrEmpty(oldRefreshToken))
            {
                appToken = await MicrosoftAccountOAuth.RedeemRefreshTokenAsync(msa_client_id, msa_client_secret, oldRefreshToken);
                await SaveToken(datastore, userid, appToken, CancellationToken.None);
            }
            return appToken;
        }
    }
    public class OAuthTicket : OneDrive.IAuthenticationInfo
    {
        public OAuthTicket(string accessToken, DateTimeOffset expirationTime, string refreshToken = null)
        {
            AccessToken = accessToken;
            TokenExpiration = expirationTime;
            TokenType = "Bearer";
            RefreshToken = refreshToken;
        }

        public OAuthTicket(AppTokenResult ticket)
        {
            PopulateOAuthTicket(ticket);
        }

        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public DateTimeOffset TokenExpiration { get; set; }
        public string RefreshToken { get; set; }

        public async Task<bool> RefreshAccessTokenAsync(string userid, FileDataStore datastore)
        {
            var newTicket = await OneDriveLogin.RenewAccessTokenAsync(this, datastore, userid);
            PopulateOAuthTicket(newTicket);

            return (newTicket != null);
        }

        private void PopulateOAuthTicket(AppTokenResult newTicket)
        {
            if (null != newTicket)
            {
                AccessToken = newTicket.AccessToken;
                TokenExpiration = DateTimeOffset.Now.AddSeconds(newTicket.AccessTokenExpirationDuration);
                TokenType = newTicket.TokenType;
                RefreshToken = newTicket.RefreshToken;
            }
        }

        public string AuthorizationHeaderValue
        {
            get { return string.Concat(TokenType, " ", AccessToken); }
        }


        public Task<bool> RefreshAccessTokenAsync()
        {
            throw new NotImplementedException();
        }
    } 
}