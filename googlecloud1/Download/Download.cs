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
using Daimto.Drive.api;
using Google.Apis.Drive.v2;
using System.Threading.Tasks;
namespace googlecloud1
{
    public delegate void StreamComplete(object sender);
    public delegate void StreamProgress(object sender, long maxsize, long Downloaded);
    abstract class Download
    {
        public Stream stream;
        public Thread thread;
        public event StreamComplete StreamCompleteCallback;
        public event StreamProgress StreamProgressCallback;
        public DriveService service;
        public string uri = "";
        public int State = 0;
        public int StreamBlockSize = 64000;
        public long maxsize = 0;
        public string filename;
        public long ContentLength = 0;
        protected bool Stopped = false;

        public Download()
        {
            thread = new Thread(new ThreadStart(this.StreamThread));
        }

        protected void OnComplete(object sender)
        {
            if(this.StreamCompleteCallback != null)
                this.StreamCompleteCallback(sender);
        }

        protected void OnProgressChange(object sender, long maxsize, long Downloaded)
        {
            if(this.StreamProgressCallback != null)
                this.StreamProgressCallback(sender,maxsize, Downloaded);
        }

        protected abstract void StreamThread();
        public abstract void Start();
        public abstract void Stop();
    }
    class FileDownload : Download
    {
        private FileStream file;
        public FileDownload(string path, string uri, DriveService service, long? maxsize)
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
                this.uri = uri;
                this.service = service;
                this.maxsize = (long)maxsize;
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
        protected override async void StreamThread()
        {
            stream = await service.HttpClient.GetStreamAsync(uri);

            int ReadSize = 0;
            long Contentlong = 0;
            if(File.Exists(this.filename))
                File.Delete(this.filename);
            this.file = File.Create(this.filename);
            MemoryStream mstream = new MemoryStream();
            byte[] buf = new byte[this.StreamBlockSize];
                try
                {
                    while ((ReadSize = await stream.ReadAsync(buf, 0, this.StreamBlockSize)) > 0)
                    {
                        Contentlong += ReadSize;
                        this.SaveToFile(this.filename, buf, ReadSize);
                        this.OnProgressChange(this, maxsize, Contentlong);
                    }
                }
                catch(Exception e)
                {
                    System.Exception pa;
                    pa = e.InnerException;    
                }
            this.stream.Close();
            this.OnComplete(this);
        }
        private async void SaveToFile(string filename, byte[] Data, int Size)
        {
            try
            {
                await file.WriteAsync(Data, 0, Size);
            }
            catch
            {

            }
        }
        public override void Stop()
        {
            this.Stopped = true;
            if (thread != null)
                thread.Abort();
        }
    }
    class FilePath
    {
        string path = "";
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
