using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Logging;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Requests;
namespace googlecloud1.login
{
    class googleauth : IAuthorizationCodeInstalledApp
    {
        private readonly GoogleAuthorizationCodeFlow flow;
        private readonly ICodeReceiver codeReceiver;
        public googleauth(GoogleAuthorizationCodeFlow flow, ICodeReceiver codeReceiver)
        { 
            this.flow = flow;
            this.codeReceiver = codeReceiver;
        }
        /// <summary>
        /// 개발자 id와 비밀번호 또 접근 권한 설정등이 담겨있는 Flow클래스
        /// </summary>
        public IAuthorizationCodeFlow Flow
        {
            get { return flow; }
        }

        /// <summary>Gets the code receiver which is responsible for receiving the authorization code.</summary>
        public ICodeReceiver CodeReceiver
        {
            get { return codeReceiver; }
        }
        /// <summary>
        /// 로그인 처리 코드를 가져와 토큰으로 교환
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="taskCancellationToken"></param>
        /// <returns></returns>
        public async Task<UserCredential> AuthorizeAsync(string userId, CancellationToken taskCancellationToken)
        {
            // 토큰이 로컬에 저장되어 있으면 저장되어있는 토큰을 불러옴
            var token = await flow.LoadTokenAsync(userId, taskCancellationToken).ConfigureAwait(false);
            // 로컬에 저장되어 있는 토큰이 없으면 실행
            if (token == null || (token.RefreshToken == null && token.IsExpired(flow.Clock)))
            {
                var authorizationCode = await loginform.GetAuthenticationToken(flow.ClientSecrets.ClientId, flow. Scopes, userId, GoogleAuthConsts.AuthorizationUrl, GoogleAuthConsts.InstalledAppRedirectUri, LoginOption.GoogleDrive);
                if (string.IsNullOrEmpty(authorizationCode))
                    return null;
                //Logger.Debug("Received \"{0}\" code", response.Code);

                // 코드를 기반으로 토큰을 얻어옴
                token = await flow.ExchangeCodeForTokenAsync(userId, authorizationCode, GoogleAuthConsts.InstalledAppRedirectUri,
                    taskCancellationToken).ConfigureAwait(false);
            }

            return new UserCredential(flow, userId, token);
        }
    }
}
