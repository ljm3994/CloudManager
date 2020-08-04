using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using googlecloud1.Files;
using System.Threading;
using Nemiro.OAuth;
namespace googlecloud1
{
    class SerchFile
    {
        public delegate void SafeComp(object sender);
        SerchMode mode;
        string serchname;
        public event SafeComp OnComp;
        List<CloudFiles> file;
        Thread thread;
        bool Stopped = true;
        public SerchFile(SerchMode mode, string serchname)
        {
            this.mode = mode;
            this.serchname = serchname;
            thread = new Thread(new ThreadStart(this.SerchThread));
        }
        public void OnComplite(object sender)
        {
            if (OnComp != null)
                this.OnComp(sender);
        }
        public void Start()
        {
            if ((this.thread != null) & (this.thread.ThreadState == ThreadState.Stopped))
                this.Stop();
            thread = new Thread(new ThreadStart(this.SerchThread));
            this.Stopped = false;
            thread.Start();
        }
        public void Stop()
        {
            this.Stopped = true;
            if (thread != null)
                thread.Abort();
        }
        public void SerchThread()
        {
            if(SerchMode.ALL == mode)
            {
                AllSerach();  
            }
            OnComplite(this);
        }
        private async void AllSerach()
        {
            foreach (var item in Setting.driveinfo)
            {
                CloudFiles files = null;
                if (item.token.Drive == "Google")
                {
                    string query = "title = " + "'" + serchname + "'";
                    var parameter = new Dictionary<string, string>
                        {
                            {"q", query}
                        };

                    var result = await HttpHelper.RequstHttp("GET", "https://www.googleapis.com/drive/v2/files", parameter, item.token.access_token);
                    Dictionary<string, object> fileinfo = HttpHelper.DerealizeJson(result.GetResponseStream());
                    object[] items = (object[])fileinfo["items"];
                    googlecloud1.Folder.GoogleFolder folder = new Folder.GoogleFolder(item);
                    foreach (var fi in items)
                    {
                        files = folder.AddFiles((Dictionary<string, object>)fi);
                        file.Add(files);
                    }
                }
                else if (item.token.Drive == "OneDrive")
                {
                    var parameter = new Dictionary<string, string>
                        {
                            {"q", serchname}
                        };
                    var result = await HttpHelper.RequstHttp("GET", "https://api.onedrive.com/v1.0/drive/root/view.search", parameter, item.token.access_token);
                    Dictionary<string, object> fileinfo = HttpHelper.DerealizeJson(result.GetResponseStream());
                    object[] items = (object[])fileinfo["value"];
                    googlecloud1.Folder.OneDriveFolder folder = new Folder.OneDriveFolder(item);
                    foreach (var fi in items)
                    {
                        files = folder.AddFiles((Dictionary<string, object>)fi);
                        file.Add(files);
                    }
                }
                else if (item.token.Drive == "DropBox")
                {
                    var parameter = new HttpParameterCollection()
                        {
                            {"query", serchname},
                            {"access_token", item.token.access_token}
                        };
                    var result = OAuthUtility.Get("https://api.dropboxapi.com/1/search/auto//", parameter);
                    googlecloud1.Folder.DropBoxFolder folder = new Folder.DropBoxFolder(item);
                    foreach (RequestResult re in result)
                    {
                        files = folder.AddFiles(re.ToDictionary());
                        file.Add(files);
                    }
                }
            }
        }
    }
}
