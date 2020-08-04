using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Nemiro.OAuth;
namespace googlecloud1.Files
{
    class OneDriveFile : CloudFiles
    {
        public FileInfo Item { get; set; }
        
        public OneDriveFile(FileInfo file)
        {
            this.Item = file;
        }
        public string GetId()
        {
            return Item.FileID;
        }
        public bool DoubleClick()
        {
            if (Item.IsFile)
            {
                return true;
            }
            else
            {
                return false;
            }
        }        
        public long GetFileSize()
        {
            return Item.FileSize;
        }
        public async Task ChageFileName(string filename)
        {
            string uri = "https://api.onedrive.com/v1.0/drive/items/" + Item.FileID;
            string method = "PATCH";
            UniValue properties = UniValue.Create(new { name = filename });
            try
            {
                byte[] buf = Encoding.UTF8.GetBytes(properties.ToString());
                System.Net.HttpWebResponse rp = await HttpHelper.RequstHttp(method, uri, null, Item.driveinfo.token.access_token, null, buf, buf.Length, 0, buf.Length);
                Item.Path = Item.Path.Replace(Item.FileName, filename);
                Item.FileName = filename;
            }
            catch ( Exception e)
            {
                throw e;
            }
        }


        public async Task<bool> FileDelete()
        {
            try
            {
                await HttpHelper.RequstHttp("DELETE", string.Format("https://api.onedrive.com/v1.0/drive/items/{0}", Item.FileID), null, Item.driveinfo.token.access_token);
                return true;
            }
            catch ( Exception e)
            {
                throw e;
            }
        }
        public async Task CopyFile(FileInfo parentfile)
        {
            string method = "POST";
            UniValue properte = UniValue.Create(new { name = Item.FileName, parentReference = new { id = parentfile.FileID }});
            System.Collections.Specialized.NameValueCollection header = new System.Collections.Specialized.NameValueCollection();
            header.Add("Prefer", "respond-async");
            byte[] buf = Encoding.UTF8.GetBytes(properte.ToString());
            string uri = string.Format("https://api.onedrive.com/v1.0/drive/items/{0}/action.copy", Item.FileID);
            try
            {
                System.Net.HttpWebResponse rp = await HttpHelper.RequstHttp(method, uri, null, Item.driveinfo.token.access_token, header, buf, buf.Length, 0, buf.Length);
            }
            catch(System.Net.WebException e)
            {
                Dictionary<string, object> ErrorBlinkStyle = HttpHelper.DerealizeJson(e.Response.GetResponseStream());
                throw e;
            }
            return;
        }
    }
}
