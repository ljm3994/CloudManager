using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nemiro.OAuth;
using googlecloud1.login;
namespace googlecloud1.Files
{
    class DropBoxFile : CloudFiles
    {
        public FileInfo Item { get; set; }
        public DropBoxFile(FileInfo value)
        {
            this.Item = value;
            int index = Item.FileName.LastIndexOf("/");
            Item.FileName = Item.FileName.Substring(index+1, (Item.FileName.Length-index)-1);
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
            string topath = Item.FileID;
            topath = topath.Replace(Item.FileName, filename);
            string uri = "https://api.dropbox.com/1/fileops/move?root=auto&from_path=" + Item.FileID + "&to_path=" + topath;
            string method = "POST";
            try
            {
                System.Net.HttpWebResponse rp = await HttpHelper.RequstHttp(method, uri, null, Item.driveinfo.token.access_token, null, null, 0, 0, 0);
                Item.Path = Item.Path.Replace(Item.FileName, filename);
                Item.FileID = Item.FileID.Replace(Item.FileName, filename);
                Item.FileName = filename;
            }
            catch(Exception e)
            {
                throw e;
            }
        }


        public async Task<bool> FileDelete()
        {
            try
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("root", "auto");
                parameter.Add("path", Item.FileID);
                await HttpHelper.RequstHttp("POST", "https://api.dropbox.com/1/fileops/delete", parameter, Item.driveinfo.token.access_token);
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
            string path = parentfile.FileID;
            string uri = string.Format("https://api.dropbox.com/1/fileops/copy");
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            string topath = path + "/" + Item.FileName;
            parameter.Add("root", "auto");
            parameter.Add("from_path", Item.FileID);
            parameter.Add("to_path", topath);
            try
            {
                System.Net.HttpWebResponse rp = await HttpHelper.RequstHttp(method, uri, parameter, Item.driveinfo.token.access_token);
                Dictionary<string, object> result = HttpHelper.DerealizeJson(rp.GetResponseStream());

            }
            catch(System.Net.WebException e)
            {
                Dictionary<string, object> result = HttpHelper.DerealizeJson(e.Response.GetResponseStream());
                throw e;
            }
            return;
        }
    }
}
