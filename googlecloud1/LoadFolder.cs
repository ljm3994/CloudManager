using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using googlecloud1.Folder;
using googlecloud1.login;
using System.Threading;
using Nemiro.OAuth;
namespace googlecloud1
{
    class LoadFolder
    {
        public delegate void Complete(object sender);
        List<AllFolder> all = new List<AllFolder>();
        DriveInfo driveinfo;
        List<DriveInfo> currentdrive;
        internal List<AllFolder> All
        {
            get { return all; }
        }
        Thread thread;
        string id;
        public event Complete complete;
        public bool Stopped = true;
        public LoadFolder(string id, DriveInfo drive= null, List<DriveInfo> driveinfo = null)
        {
            this.id = id;
            this.currentdrive = driveinfo;
            this.driveinfo = drive;
            thread = new Thread(new ThreadStart(this.LoadFile));
        }
        public async void LoadFile()
        {
            if (currentdrive != null)
                {
                    foreach (var item in currentdrive)
                    {
                        if(item.token.Drive == "Google")
                        {
                            await LoadGoogleFile(id, item);
                        }
                        else if (item.token.Drive == "OneDrive")
                        {
                            await LoadOneDriveFile(id, item);
                        }
                        else if (item.token.Drive == "DropBox")
                        {
                            string path = id;
                            if(id == "root")
                            {
                                path = "/";
                            }
                            await LoadDropBoxFile(path, item);
                        }
                    }
                }
            this.OnComplete(this);
        }

        private void OnComplete(object sender)
        {
            if (this.complete != null)
                this.complete(sender);
        }
        public void Start()
        {
            if ((this.thread != null) & (this.thread.ThreadState == ThreadState.Stopped))
                this.Stop();
            thread = new Thread(new ThreadStart(this.LoadFile));
            this.Stopped = false;
            thread.IsBackground = true;
            thread.Start();
        }

        public void Stop()
        {
            this.Stopped = true;
            if (thread != null)
                thread.Abort();
        }
        private async Task LoadOneDriveFile(string p, DriveInfo driveinfo)
        {
            OneDriveFolder Ones = new OneDriveFolder(driveinfo);
            await Ones.AddFiles(p);
            Ones.ToString();
            all.Add(Ones);
        }
        private async Task LoadGoogleFile(string id, DriveInfo driveinfo)
        {
            GoogleFolder google = new GoogleFolder(driveinfo);
            await google.AddFiles(id);
            all.Add(google);
        }
        private async Task LoadDropBoxFile(string id, DriveInfo driveinfo)
        {
            DropBoxFolder drop = new DropBoxFolder(driveinfo);
            await drop.AddFiles(id);
            all.Add(drop);
        } 
    }
}
