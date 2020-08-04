using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nemiro.OAuth;
using System.Web;
using System.Net;
using System.Collections.Specialized;
using Nemiro.OAuth.Extensions;
using Newtonsoft.Json.Utilities;
using System.Web.Script.Serialization;
namespace googlecloud1.FileUpDown
{
    class UpLoadUrl
    {
        string path;
        string currentfileid;
        string accesstoken;
        long maxsize;
        JavaScriptSerializer json;
        Dictionary<string, object> upload;
        HttpHelper httphelper;
        NameValueCollection header = null;
        public UpLoadUrl(string path, string currentfileid, long maxsize, string accesstoken)
        {
            this.path = path;
            this.currentfileid = currentfileid;
            this.maxsize = maxsize;
            this.accesstoken = accesstoken;
            Dictionary<string, object> upload = new Dictionary<string, object>();
            httphelper = new HttpHelper();
            header = new NameValueCollection();
        }
        public async Task<string> GetGoogleUploadUrl()
        {
            object parent = null;
            parent = new object[] { new { id = currentfileid } };
            UniValue properties = UniValue.Create(new { title = path, parents = parent, description = "AllCloudeByUpLoad", fileSize = maxsize });
            byte[] date = Encoding.UTF8.GetBytes(properties.ToString());
            HttpWebResponse respone;
            try
            {
                respone = await HttpHelper.RequstHttp("POST", "https://www.googleapis.com/upload/drive/v2/files?uploadType=resumable", null, accesstoken, null, date, date.Length, 0, date.Length);
            }
            catch(Exception e)
            {
                throw e;
            }
            return respone.Headers["Location"].ToString();
        }
        public async Task<string> GetOneDriveUrl()
        {
            HttpWebResponse respone;
            header.Add("Content-Range", string.Format("0-{0}/{1}", maxsize - 1, maxsize));
            try
            {
                respone = await HttpHelper.RequstHttp("POST", string.Format("https://api.onedrive.com/v1.0/drive/items/{0}:/{1}:/upload.createSession", currentfileid, path), null, accesstoken, header, null, maxsize, 0, 0);
                upload = HttpHelper.DerealizeJson(respone.GetResponseStream());
            }
            catch(Exception e)
            {
                throw e;
            }
            return upload["uploadUrl"].ToString();
        }
    }
}
