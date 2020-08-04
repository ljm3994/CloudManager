using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using googlecloud1.Files;
using googlecloud1.Folder;
using System.IO;
using System.Net;
namespace googlecloud1.FileUpDown
{
    class FolderDownload : FileTransfer
    {
        CloudFiles folder;
        System.IO.FileStream filestream;
        long startsize;
        public FolderDownload(CloudFiles file, string path)
        {
            folder = file;
            this.filename = path;
            if (!Directory.Exists(filename))
            {
                System.Security.AccessControl.DirectorySecurity security = new System.Security.AccessControl.DirectorySecurity();
                security.AddAccessRule(new System.Security.AccessControl.FileSystemAccessRule(System.Environment.UserName, System.Security.AccessControl.FileSystemRights.FullControl, System.Security.AccessControl.AccessControlType.Allow));
                Directory.CreateDirectory(filename, security);
            }
            this.maxsize = 0;
            this.startsize = 0;
            label = new System.Windows.Forms.Label();
            progress = new System.Windows.Forms.ProgressBar();
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
        // 다운로드 스트림
        public async override void StreamThread()
        {
            // 다운로드시 보일 라벨과 프로그래스 바를 설정해준다.
            label.Text = "폴더 압축중.....";
            label.Name = "label";
            label.AutoSize = true;
            progress.Value = 0;
            progress.Maximum = 100;
            progress.Controls.Add(label);
            progress.Size = new System.Drawing.Size(181, 23);
            this.OnControl(this, progress);
            // 폴더안에 파일들을 가져오는 함수 시작
            FolderInFile();
            // 다운로드를 시작한다.
            await DownloadFolder(folder, this.filename);
            // 다운로드 완료시 이벤트 발생
            this.OnComplete(this, progress, folder, folder.Item.FileID);
        }
        /// <summary>
        /// 폴더 다운로드를 시작하는 함수
        /// </summary>
        /// <param name="file"> 다운로드할 파일</param>
        /// <param name="filepath">현재 파일의 경로</param>
        /// <param name="startsize"> 시작 size</param>
        /// <returns></returns>
        private async Task DownloadFolder(CloudFiles file, string filepath)
        {
            //폴더가 아니라 파일이라면
            if (file.Item.ChildFile == null)
            {
                // 함수를 빠져나간다.
                return;
            }
            byte[] buf = new byte[FileTransfer.StreamBlockSize];
            int ReadSize = 0;
            try
            {
                foreach (var item in file.Item.ChildFile)
                {
                    if(item.Item.IsFile == true)
                    {
                        // 파일을 만들경로를 설정한다.
                        string path = null;
                        path = string.Format("{0}\\{1}", filepath, item.Item.FileName);
                        // 파일이 이미 존재하면
                        if (System.IO.File.Exists(filepath))
                        {
                            // 존재하는 파일을 지워준다
                            System.IO.File.Delete(filepath);
                        }
                        // 새로운 파일을 만든다
                        this.filestream = System.IO.File.Create(path);
                        // http에 접속하여 반환값을 얻어온다.
                        HttpWebResponse rp = await HttpHelper.RequstHttp("GET", item.Item.DownUrl, null, item.Item.driveinfo.token.access_token);
                        // 얻어온 내용을 stream으로 쓴다.
                        this.stream = rp.GetResponseStream();
                        // 파일을 전부 쓸때까지 반복
                        while ((ReadSize = await stream.ReadAsync(buf, 0, StreamBlockSize)) > 0)
                        {
                            // 현재 완료된 다운로드 사이즈
                            this.startsize += ReadSize;
                            // 파일 스트림에 얻어온 스트림의 byte를 쓴다.
                            await filestream.WriteAsync(buf, 0, ReadSize, CancellationToken.None);
                            // 프로그래스 이벤트를 발생시켜서 프로그래스 바 값을 바꿔준다.
                            this.OnProgressChange(this, maxsize, startsize, this.progress);
                        }
                        // 스트림을 닫아준다.
                        this.stream.Close();
                        // 파일 스트림을 닫아준다.
                        filestream.Close();
                    }
                        //파일이 아닌 폴더라면
                    else
                    {
                        // 폴더의 경로를 만들어준다.
                        string folderpath = string.Format("{0}\\{1}", filepath, item.Item.FileName);
                        // 폴더가 존재하지 않으면 폴더를 새롭게 생성해준다.
                        if (!Directory.Exists(folderpath))
                        {
                            System.Security.AccessControl.DirectorySecurity security = new System.Security.AccessControl.DirectorySecurity();
                            security.AddAccessRule(new System.Security.AccessControl.FileSystemAccessRule(System.Environment.UserName, System.Security.AccessControl.FileSystemRights.FullControl, System.Security.AccessControl.AccessControlType.Allow));
                            Directory.CreateDirectory(folderpath, security);  
                        }
                        // 재귀적으로 이 함수를 새로운 경로와 새로운 파일들로 다시 실행해준다.
                        await DownloadFolder(item, folderpath);
                    }
                }
            }
            // 예외 처리를 위한 구간
            catch (WebException e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }
        }
        private void FolderInFile()
        {
            AllFolder allfolder = null;
            if(folder.Item.driveinfo.token.Drive == "Google")
            {
                allfolder = new GoogleFolder(folder.Item.driveinfo);
            }
            else if (folder.Item.driveinfo.token.Drive == "OneDrive")
            {
                allfolder = new OneDriveFolder(folder.Item.driveinfo);
            }
            else if(folder.Item.driveinfo.token.Drive == "DropBox")
            {
                allfolder = new DropBoxFolder(folder.Item.driveinfo);
            }
            AddFile(folder, allfolder);
        }
        /// <summary>
        /// 파일을 가져오는 함수
        /// </summary>
        /// <param name="file">부모 파일</param>
        /// <param name="folders"></param>
        private void AddFile(CloudFiles file, AllFolder folders)
        {
            // 부모 파일이 폴더가 아니라 파일이면 함수를 빠져나간다. 재귀함수의 탈출 조건
            if (file.Item.IsFile == true)
            {
                this.maxsize += file.Item.FileSize;
                return;
            }
            folders.AddFiles(file.Item.FileID);
            file.Item.ChildFile = folders.GetFiles();
            foreach (var item in file.Item.ChildFile)
            {
                //재귀함수로 파일리스트에 파일들을 집어넣어준다.
                AddFile(item, folders);
            }
        }
    }
}
