using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Web.Script.Serialization;
using System.IO;
using System.Threading;
using System.Collections.Specialized;
namespace googlecloud1
{
    class HttpHelper
    {
        public static string HttpParameter(string baseuri, Dictionary<string, string> parameter)
        {
            StringBuilder uri = new StringBuilder();
            uri.Append(baseuri);
            uri.Append("?");
            foreach (var item in parameter)
            {
                if(uri[uri.Length - 1] != '?')
                {
                    uri.Append("&");
                }
                uri.Append(item.Key);
                uri.Append("=");
                uri.Append(Uri.EscapeDataString(item.Value));
            }
            return uri.ToString();
        }
        public async static Task<HttpWebResponse> RequstHttp(string method, string uri, Dictionary<string, string> parameter, string Token, NameValueCollection header = null, byte[] date = null, long maxsize = 0, long startsize = 0, long endsize = 0)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            string requsturi = uri;
            if (parameter != null)
            {
                requsturi = HttpParameter(uri, parameter);
            }
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requsturi);
            
            request.Method = method;
            request.UserAgent = "AllCloude";
            request.Accept = "application/json";
            if (header != null)
            {
                request.Headers.Add(header);
            }
            request.Headers["Authorization"] = "Bearer " + Token;
            if (date != null)
            {
                request.Timeout = Timeout.Infinite;
                request.ReadWriteTimeout = Timeout.Infinite;
                request.SendChunked = true;
                request.ContentLength = maxsize;
                request.ContentType = "application/json";
                Stream stream = await request.GetRequestStreamAsync();
                await stream.WriteAsync(date, 0, (int)((endsize - startsize)), CancellationToken.None);
                stream.Close();
            }
            else
            {
                request.ContentLength = 0;
            }
            HttpWebResponse respone = null;
            try
            {
                respone = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
                if (e.Response != null)
                {
                    Dictionary<string, object> di = HttpHelper.DerealizeJson(e.Response.GetResponseStream());
                }
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
            return respone;
        }
        /// <summary>
        /// json 문자열을 key/value를 쌍으로 가지는 딕셔너리 형태로 변환해줍니다
        /// </summary>
        /// <param name="text">원본 Json 텍스트</param>
        /// <returns></returns>
        public static Dictionary<string, object> DerealizeJson(string text)
        {
            JavaScriptSerializer json = new JavaScriptSerializer();
            return (Dictionary<string, object>)json.DeserializeObject(text);
        }
        /// <summary>
        /// stream에 담긴 json 문자열을 읽어와서 key/value를 쌍으로 가지는 딕셔너리 형태로 변환해줍니다
        /// </summary>
        /// <param name="stream">원본 stream</param>
        /// <returns></returns>
        public static Dictionary<string, object> DerealizeJson(Stream stream)
        {
            StreamReader sr = new StreamReader(stream);
            string text = sr.ReadToEnd();
            JavaScriptSerializer json = new JavaScriptSerializer();
            return (Dictionary<string, object>)json.DeserializeObject(text);
        }
        /// <summary>
        /// stream에 담긴 json 문자열을 읽어와서 지정해준 사용자 클래스 형태로 변환해줍니다
        /// </summary>
        /// <typeparam name="T">사용자 정의 컨트롤 이름</typeparam>
        /// <param name="stream">원본 stream</param>
        /// <returns></returns>
        public static T ClassDerealizeJson<T>(Stream stream)
        {
            StreamReader sr = new StreamReader(stream);
            string text = sr.ReadToEnd();
            JavaScriptSerializer json = new JavaScriptSerializer();
            return json.Deserialize<T>(text);
        }
        /// <summary>
        /// json 문자열을 사용자 정의 클래스 형태로 변환해줍니다
        /// </summary>
        /// <typeparam name="T">사용자 정의 클래스 이름</typeparam>
        /// <param name="text">json문자열</param>
        /// <returns></returns>
        public static T ClassDerealizeJson<T>(string text)
        {
            JavaScriptSerializer json = new JavaScriptSerializer();
            return json.Deserialize<T>(text);
        }
    }
}
