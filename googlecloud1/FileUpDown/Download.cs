using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Xml;
using System.IO;
using System.Drawing;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Threading.Tasks;
namespace googlecloud1.FileUpDown
{
    class FileDownload : FileTransfer
    {
        private FileStream file;
        string DownUrl;
        public FileDownload(string path, long? maxsize, string DownUrl, string accesstoken, string fileid)
        {
            try
            {
                string realpath = FilePath.GetPath(path);
                if (!Directory.Exists(realpath))
                {
                    Directory.CreateDirectory(realpath);
                }
            }
            catch
            {

            }
            finally
            {
                this.filename = path;
                this.maxsize = (long)maxsize;
                this.DownUrl = DownUrl;
                this.accesstoken = accesstoken;
                this.fileid = fileid;
                thread = new Thread(new ThreadStart(this.StreamThread));
                label = new Label();
                progress = new ProgressBar();
            }

        }
        public override void Start()
        {
            if ((this.thread != null) & (this.thread.ThreadState == ThreadState.Stopped))
                this.Stop();
            thread = new Thread(new ThreadStart(this.StreamThread));
            this.Stopped = false;
            thread.Start();
        }
        public override void Stop()
        {
            this.Stopped = true;
            if (thread != null)
                thread.Abort();
        }
        public override async void StreamThread()
        {
            if (System.IO.File.Exists(this.filename))
                System.IO.File.Delete(this.filename);
            int ReadSize = 0;
            label.Text = "0%";
            label.Name = "label";
            label.AutoSize = true;
            progress.Value = 0;
            progress.Maximum = 100;
            progress.Controls.Add(label);
            progress.Size = new System.Drawing.Size(181, 23);
            this.OnControl(this, progress);
            this.file = System.IO.File.Create(this.filename);
            byte[] buf = new byte[FileTransfer.StreamBlockSize];
            long startsize = 0;
            try
            {
                HttpWebResponse rp = await HttpHelper.RequstHttp("GET", DownUrl, null, accesstoken);
                this.stream = rp.GetResponseStream();
                while ((ReadSize = await stream.ReadAsync(buf, 0, StreamBlockSize)) > 0)
                {
                    startsize += ReadSize;
                    await file.WriteAsync(buf, 0, ReadSize, CancellationToken.None);
                    this.OnProgressChange(this, maxsize, startsize, this.progress);
                }
            }
            catch (Exception e)
            {
                Exception pa;
                pa = e.InnerException;
                MessageBox.Show(string.Format("다운로드 오류 : {0}", e.Message));
            }
            this.stream.Close();
            file.Close();
            this.OnComplete(this, progress);
        }
    }
    class FilePath
    {
        public static string GetPath(string filename)
        {
            if(filename != null)
            {
                int index = filename.LastIndexOf("\\");
                return filename.Substring(0, index);
            }
            return "";
        }
    }
}
