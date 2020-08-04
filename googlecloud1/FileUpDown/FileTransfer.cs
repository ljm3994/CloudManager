using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using googlecloud1.Folder;
using googlecloud1.Files;
namespace googlecloud1.FileUpDown
{
    public enum FileOption { Empty = 0, GoogleDrive, OneDrive, DropBox };
    // delegate로 이벤트 연결
    public delegate void StreamComplete(object sender, Control control, CloudFiles item = null, string fileid = null, ListViewGroup group = null);
    public delegate void StreamProgress(object sender, long maxsize, long Downloaded, ProgressBar progress);
    public delegate void StreamControl(object sender, Control control);
    //파일 전송 클래스
    public abstract class FileTransfer
    {
        // 파일전송시 파일을 담을 stream
        public Stream stream;
        // 쓰레드 변수 선언
        public Thread thread;
        // 프로그래스 바 이벤트시 보일 프로그래스바 선언
        public ProgressBar progress;
        // 라벨 선언
        public Label label;
        // 이벤트 선언
        public event StreamComplete StreamCompleteCallback;
        public event StreamProgress StreamProgressCallback;
        public event StreamControl StreamControlCallBack;
        protected int State = 0;
        // 최대 전송할 byte의 size선언
        protected static int StreamBlockSize = 50000;
        protected long maxsize = 0;
        protected string filename;
        protected string fileid = null;
        protected long ContentLength = 0;
        protected string accesstoken;
        protected bool Stopped = false;
        protected FileOption option;
        public FileTransfer()
        {
            thread = new Thread(new ThreadStart(this.StreamThread));
        }
        protected void OnControl(object sender, Control control)
        {
            if (this.StreamControlCallBack != null)
                this.StreamControlCallBack(sender, control);
        }
        protected void OnComplete(object sender, Control control, CloudFiles item = null, string fileid = null, ListViewGroup group = null)
        {
            if (this.StreamCompleteCallback != null)
                this.StreamCompleteCallback(sender, control, item, fileid, group);
        }
        protected void OnProgressChange(object sender, long maxsize, long Downloaded, ProgressBar progress)
        {
            if (this.StreamProgressCallback != null)
                this.StreamProgressCallback(sender, maxsize, Downloaded, progress);
        }
        public abstract void Start();
        public abstract void Stop();
        public abstract void StreamThread();
    }
}