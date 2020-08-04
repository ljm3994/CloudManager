using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using googlecloud1.Files;
using googlecloud1.Folder;
using System.Net;
using System.Windows.Forms;
namespace googlecloud1.FileUpDown
{
    class FolderInfo
    {
        public FolderInfo()
        {
            file = new List<System.IO.FileInfo>();
            folder = new List<FolderInfo>();
            size = 0;
        }
        public string filename { get; set; }
        public long size { get; set; }
        public List<System.IO.FileInfo> file { get; set; }
        public bool isfile { get; set; }
        public List<FolderInfo> folder {get; set;}
    }
    class FolderUpload : FileTransfer
    {
        System.IO.DirectoryInfo localfolder;
        AllFolder folder;
        DriveInfo driveinfo;
        ListViewGroup group;
        public long startsize { get; set; }
        public FolderUpload(System.IO.DirectoryInfo localfolder, AllFolder folder, string parentid, DriveInfo driveinfo, ListViewGroup group)
        {
            this.localfolder = localfolder;
            this.fileid = parentid;
            this.folder = folder;
            this.driveinfo = driveinfo;
            this.accesstoken = driveinfo.token.access_token;
            this.maxsize = 0;
            startsize = 0;
            this.group = group;
            this.label = new System.Windows.Forms.Label();
            this.progress = new System.Windows.Forms.ProgressBar();
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
        public async override void StreamThread()
        {
            // 다운로드시 보일 라벨과 프로그래스 바를 설정해준다.
            label.Text = "0%";
            label.Name = "label";
            label.AutoSize = true;
            progress.Value = 0;
            progress.Maximum = 100;
            progress.Controls.Add(label);
            progress.Size = new System.Drawing.Size(181, 23);
            this.OnControl(this, progress);
            FolderInfo folders = new FolderInfo();
            folders.isfile = false;
            //folder.directory = new List<System.IO.DirectoryInfo>(); 
            this.maxsize += await CreateDirectory(folders, localfolder, localfolder.FullName);
            // 폴더안에 파일들을 가져오는 함수 시작
            // 업로드를 시작한다.
            CloudFiles file = await folder.CreateFolder(System.IO.Path.GetFileName(localfolder.FullName), this.fileid);
            await UpFolder(folders, file.Item.FileID);
            this.OnProgressChange(this, this.maxsize, this.maxsize, this.progress);
            // 업로드 완료시 이벤트 발생
            this.OnComplete(this, progress, file, this.fileid, group);
        }
        private async Task UpFolder(FolderInfo folders, string ID)
        {
            foreach (var item in folders.file)
            {
                await UpLoadFile(folder, ID, startsize, item.FullName, item.Length);
            }
            if(folders.folder.Count <= 0)
            {
                return;
            }
            else
            {
                foreach (var item in folders.folder)
                {
                    string folderid;
                    CloudFiles file1 = await folder.CreateFolder(System.IO.Path.GetFileName(item.filename), ID);
                    folderid = file1.Item.FileID;
                    await UpFolder(item, folderid);
                }
            }
        }
        private async Task<long> UpLoadFile(AllFolder folder, string fileid, long startsize, string filename, long filemaxsize)
        {
            string uploadurl = null;
            string method = "PUT";
            string naem = System.IO.Path.GetFileName(filename);
            UpLoadUrl Url = new UpLoadUrl(naem, fileid, maxsize, accesstoken);
            if (driveinfo.token.Drive == "Google")
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
                System.IO.Stream filestream = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite);
                HttpWebResponse respone = null;
                respone = await RequstHttp(method, uploadurl, null, accesstoken, filestream, filemaxsize, StreamBlockSize, startsize);
                if (driveinfo.token.Drive == "DropBox")
                {
                    Dictionary<string, object> text = HttpHelper.DerealizeJson(respone.GetResponseStream());
                    string uploadid = text["upload_id"].ToString();
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("upload_id", uploadid);
                    respone = await HttpHelper.RequstHttp("POST", string.Format("https://api-content.dropbox.com/1/commit_chunked_upload/auto/{0}", System.IO.Path.Combine(fileid, filename).Replace("\\", "/")), parameter, accesstoken);
                }
                Dictionary<string, object> file = HttpHelper.DerealizeJson(respone.GetResponseStream());
                filestream.Close();
                return startsize;
                //mstream.Close();
            }
            catch (WebException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<HttpWebResponse> RequstHttp(string method, string uri, Dictionary<string, string> parameter, string Token, System.IO.Stream filestream, long filemaxsize = 0, int blocksize = 0, long startsize = 0)
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
            request.Headers["Content-Range"] = string.Format("bytes 0-{0}/{1}", filemaxsize - 1, filemaxsize);
            request.ContentLength = filemaxsize;
            request.ContentType = "application/json";
            int readsize = 0;
            byte[] buf = new byte[blocksize];
            System.IO.Stream stream = await request.GetRequestStreamAsync();
            while ((readsize = await filestream.ReadAsync(buf, 0, blocksize, CancellationToken.None)) > 0)
            {
                await stream.WriteAsync(buf, 0, readsize, CancellationToken.None);
                this.startsize += readsize;
                this.OnProgressChange(this, this.maxsize, startsize, this.progress);
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
        /// <summary>
        /// 지정된 디렉토리의 모든 파일을 재귀적으로 가져온다.
        /// </summary>
        /// <param name="directory"> 폴더의 정보를 저장할 클래스</param>
        /// <param name="dire"> 폴더의 정보를 저장하고 있는 클래스</param>
        /// <param name="naem"> 폴더의 경로</param>
        public async Task<long> CreateDirectory(FolderInfo directory, System.IO.DirectoryInfo dire, string naem)
        {
            //만약 파일이라면 함수를 빠져나간다.
            // 폴더의 경로를 저장한다.
            directory.filename = naem;
            // 폴더안에 파일들을 전부 저장한다.
            directory.file = dire.GetFiles().ToList();
            long size = 0;
            foreach (var item in directory.file)
            {
                size += item.Length;
            }
            // 폴더안에 하위 폴더들의 목록을 얻어온다.
            System.IO.DirectoryInfo[] direc = dire.GetDirectories();
            //얻어온 폴더목록 만큼 반복한다.
            foreach (var item in direc)
            {
                FolderInfo info = new FolderInfo();
                info.isfile = false;
                // 재귀적으로 이 함수를 다시 호출하여 폴더안에 파일들을 저장해서 온다.
                size += await CreateDirectory(info, item, item.FullName);
                // 가져온 폴더 정보들을 리스트에 담는다.
                directory.folder.Add(info);
            }
            return size;
        }
    }
}
