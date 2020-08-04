using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Xml;
using System.Drawing;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Threading.Tasks;
using googlecloud1.login;
using Nemiro.OAuth;
using Nemiro.OAuth.Extensions;
using System.Collections.Specialized;
using googlecloud1.Folder;
using googlecloud1.Files;
namespace googlecloud1.FileUpDown
{
    public class FileUpload : FileTransfer
    {
        System.IO.Stream filestream { get; set; }
        int totalsize = 0;
        byte[] byteArray;
        string path;
        string fildid;
        long presize = 0;
        long endsize = 0;
        ListViewGroup group;
        DriveInfo driveinfo;
        public FileUpload(System.IO.Stream filestream, string filename, long size,string fildeid, DriveInfo driveinfo, ListViewGroup group)
        {
            this.path = filename;
            this.filestream = filestream;
            this.maxsize = size;
            this.fildid = fildeid;
            this.accesstoken = driveinfo.token.access_token;
            this.driveinfo = driveinfo;
            this.group = group;
            label = new Label();
            progress = new ProgressBar();
            thread = new Thread(new ThreadStart(this.StreamThread));
        }
        public override void Start()
        {
            if ((this.thread != null) & (this.thread.ThreadState == ThreadState.Stopped))
                this.Stop();
            thread = new Thread(new ThreadStart(this.StreamThread));
            this.Stopped = false;
            thread.Start();
        }
        public async override void StreamThread()
        {
            int ReadSize = 0;
            label.Text = "0%";
            label.Name = "label";
            label.AutoSize = true;
            progress.Value = 0;
            progress.Maximum = 100;
            progress.Controls.Add(label);
            progress.Size = new System.Drawing.Size(181, 23);
            this.OnControl(this, progress);
            byteArray = new byte[FileTransfer.StreamBlockSize];
            ReadSize = 0;         
            System.IO.MemoryStream mstream = new System.IO.MemoryStream();
            string uploadurl = null;
                string method = "PUT";
                presize = 0;
                endsize = 0;
                UpLoadUrl Url = new UpLoadUrl(path, fildid, maxsize, accesstoken);
                if(driveinfo.token.Drive == "Google")
                {
                    uploadurl = await Url.GetGoogleUploadUrl();
                }
                else if (driveinfo.token.Drive == "OneDrive")
                {
                    uploadurl = await Url.GetOneDriveUrl();
                }
                else if (driveinfo.token.Drive == "DropBox")
                {
                    uploadurl = string.Format("https://api-content.dropbox.com/1/chunked_upload?overwrite=true&autorename=true");
                }
                try
                {
                    HttpWebResponse respone = null;
                    respone = await RequstHttp(method, uploadurl, null, accesstoken, filestream, maxsize, StreamBlockSize);
                    if(driveinfo.token.Drive == "DropBox")
                    {
                        Dictionary<string, object> text = HttpHelper.DerealizeJson(respone.GetResponseStream());
                        string uploadid = text["upload_id"].ToString();
                        Dictionary<string, string> parameter = new Dictionary<string, string>();
                        parameter.Add("upload_id", uploadid);
                        respone = await HttpHelper.RequstHttp("POST", string.Format("https://api-content.dropbox.com/1/commit_chunked_upload/auto/{0}", System.IO.Path.Combine(fildid, path).Replace("\\", "/")), parameter, accesstoken);
                    }
                    Dictionary<string, object> file = HttpHelper.DerealizeJson(respone.GetResponseStream());
                    CloudFiles item = CreateFile(file);
                    this.OnComplete(this, progress, item, fildid, group);
                    filestream.Close();
                    mstream.Close();
                }
                catch (WebException e)
                {
                    MessageBox.Show("전송오류: " + e.Message);
                    return;
                }
                catch (Exception e)
                {
                    MessageBox.Show("전송오류: " + e.Message);
                    return;
                }
            }
        public CloudFiles CreateFile(Dictionary<string, object> fileinfo)
        {
            CloudFiles files = null;
            if(driveinfo.token.Drive == "Google")
            {
                GoogleFolder folder = new GoogleFolder(driveinfo);
                files = folder.AddFiles(fileinfo);
                
            }
            else if (driveinfo.token.Drive == "OneDrive")
            {
                OneDriveFolder folder = new OneDriveFolder(driveinfo);
                files = folder.AddFiles(fileinfo);
                
            }
            else if (driveinfo.token.Drive == "DropBox")
            {
                DropBoxFolder folder = new DropBoxFolder(driveinfo);
                files = folder.AddFiles(fileinfo);
            }
            return files;
        }
        public override void Stop()
        {
            this.Stopped = true;
            if (thread != null)
                thread.Abort();
        }
        public async Task<HttpWebResponse> RequstHttp(string method, string uri, Dictionary<string, string> parameter, string Token, System.IO.Stream filestream, long maxsize = 0, int blocksize = 0)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            string requsturi = uri;
            if (parameter != null)
            {
                requsturi = HttpHelper.HttpParameter(uri, parameter);
            }
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requsturi);
            request.Method = method;
            request.UserAgent = "AllCloude";
            request.Accept = "application/json";
            request.Headers["Authorization"] = "Bearer " + Token;
            request.Timeout = Timeout.Infinite;
            request.ReadWriteTimeout = Timeout.Infinite;
            request.SendChunked = true;
            request.Headers["Content-Range"] = string.Format("bytes 0-{0}/{1}", maxsize - 1, maxsize);
            request.ContentLength = maxsize;
            request.ContentType = "application/json";
            int readsize = 0;
            byte[] buf = new byte[blocksize];
            long presize = 0;
            long totalsize = 0;
            System.IO.Stream stream = await request.GetRequestStreamAsync();
            while ((readsize = await filestream.ReadAsync(buf, 0, blocksize, CancellationToken.None)) > 0)
            {
                await stream.WriteAsync(buf, 0, readsize, CancellationToken.None);
                presize += readsize;
                totalsize += readsize;
                this.OnProgressChange(this, maxsize, totalsize, this.progress);
            }
            stream.Close();
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
    }
}
