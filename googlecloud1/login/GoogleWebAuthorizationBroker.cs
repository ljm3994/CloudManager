using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Web;
using System.Collections.Specialized;
using googlecloud1.login;
namespace googlecloud1.login
{
    class WebAuthorization
    {
        public enum TokenOption { CODE, REFRESHTOKEN };
        public static async Task<Authentication.TokenResult> RedeemAuthorizationCodeAsync(string clientId, string redirectUrl, string clientSecret, string authCode, string starturi, string granttype, TokenOption option)
        {
            QueryStringBuilder queryBuilder = new QueryStringBuilder();
            queryBuilder.Add("client_id", clientId);
            queryBuilder.Add("client_secret", clientSecret);
            if (!string.IsNullOrEmpty(redirectUrl))
            {
                queryBuilder.Add("redirect_uri", redirectUrl);
            }
            if(option == TokenOption.CODE)
            {
                queryBuilder.Add("code", authCode);
            }
            else
            {
                queryBuilder.Add("refresh_token", authCode);
            }
            queryBuilder.Add("grant_type", granttype);

            return await PostToTokenEndPoint(queryBuilder, starturi);
        }
        private static async Task<Authentication.TokenResult> PostToTokenEndPoint(QueryStringBuilder queryBuilder, string starturi)
        {
            HttpWebRequest request = WebRequest.CreateHttp(starturi);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
         
            using (StreamWriter requestWriter = new StreamWriter(await request.GetRequestStreamAsync()))
            {
                await requestWriter.WriteAsync(queryBuilder.ToString());
                await requestWriter.FlushAsync();
            }

            HttpWebResponse httpResponse;
            try
            {
                var response = await request.GetResponseAsync();
                httpResponse = response as HttpWebResponse;
            }
            catch (WebException webex)
            {
                httpResponse = webex.Response as HttpWebResponse;
            }
            catch (Exception)
            {
                return null;
            }

            // TODO: better error handling

            if (httpResponse == null)
                return null;

            if (httpResponse.StatusCode != HttpStatusCode.OK)
            {
                httpResponse.Dispose();
                return null;
            }
            using (var responseBodyStreamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var responseBody = await responseBodyStreamReader.ReadToEndAsync();
                responseBody = responseBody.Replace("user_id", "uid");
                var tokenResult = Newtonsoft.Json.JsonConvert.DeserializeObject<Authentication.TokenResult>(responseBody);
                httpResponse.Dispose();
                return tokenResult;
            }
        }
    }
    public class QueryStringBuilder
    {
        private readonly Dictionary<string, string> parameters = new Dictionary<string, string>();
        public QueryStringBuilder()
        {
            StartCharacter = null;
            SeperatorCharacter = '&';
            KeyValueJoinCharacter = '=';
        }

        public QueryStringBuilder(string key, string value)
        {
            this[key] = value;
        }

        public void Clear()
        {
            parameters.Clear();
        }

        public bool HasKeys
        {
            get { return parameters.Count > 0; }
        }

        public char? StartCharacter { get; set; }

        public char SeperatorCharacter { get; set; }

        public char KeyValueJoinCharacter { get; set; }

        public string this[string key]
        {
            get
            {
                if (parameters.ContainsKey(key))
                    return parameters[key];
                else
                    return null;
            }
            set
            {
                parameters[key] = value;
            }
        }

        public bool ContainsKey(string key)
        {
            return parameters.ContainsKey(key);
        }

        public string[] Keys
        {
            get { return parameters.Keys.ToArray(); }
        }

        public void Add(string key, string value)
        {
            parameters[key] = value;
        }

        public void Remove(string key)
        {
            parameters.Remove(key);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var param in parameters)
            {
                if ((sb.Length == 0) && (null != StartCharacter))
                    sb.Append(StartCharacter);
                if ((sb.Length > 0) && (sb[sb.Length - 1] != StartCharacter))
                    sb.Append(SeperatorCharacter);

                sb.Append(param.Key);
                sb.Append('=');
                sb.Append(Uri.EscapeDataString(param.Value));
            }
            return sb.ToString();
        }
    }
}
