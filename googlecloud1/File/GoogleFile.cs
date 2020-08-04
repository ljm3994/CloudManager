using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using googlecloud1;
using System.Web;
using System.IO;
using Nemiro.OAuth;
namespace googlecloud1.Files
{
    class GoogleFile : CloudFiles
    {
        FileInfo item;

        public FileInfo Item
        {
            get { return item; }
            set { Item = value; }
        }
        public GoogleFile(FileInfo file)
        {
            this.item = file;
        }
        public string GetId()
        {
            return item.FileID;
        }
        public bool DoubleClick()
        {
            if(item.Extention == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public long GetFileSize()
        {
            return (long)item.FileSize;
        }
        public async Task ChageFileName(string filename)
        {
            string uri = "https://www.googleapis.com/drive/v1/files/" + Item.FileID + "?updateViewedDate=true&updateModifiedDate=true";
            UniValue properties = UniValue.Create(new { title = filename });
            string method = "PATCH";
            try
            {
                byte[] buf = Encoding.UTF8.GetBytes(properties.ToString());
                System.Net.HttpWebResponse rp = await HttpHelper.RequstHttp(method, uri, null, Item.driveinfo.token.access_token, null, buf, buf.Length, 0, buf.Length);
                item.Path = item.Path.Replace(item.FileName, filename);
                Item.FileName = filename;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<bool> FileDelete()
        {
            try
            {
                await HttpHelper.RequstHttp("DELETE", string.Format("https://www.googleapis.com/drive/v2/files/{0}", item.FileID), null, item.driveinfo.token.access_token);
                return true;
            }
            catch ( Exception e)
            {
                throw e;
            }
        }
        public async Task CopyFile(FileInfo parentfile)
        {
            string method = "PATCH";
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("addParents", parentfile.FileID);
            string uri = string.Format("https://www.googleapis.com/drive/v2/files/{0}", item.FileID);
            try
            {
                System.Net.HttpWebResponse rp = await HttpHelper.RequstHttp(method, uri, parameter, item.driveinfo.token.access_token);
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}
