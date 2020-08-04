using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Apis.Drive.v2;
using Daimto.Drive.api;
using Google.Apis.Services;
using Google.Apis.Util;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Requests;
using Google.Apis.Download;
using Google.Apis.Http;
using System.Net;
using googlecloud1.FileUpDown;
using OneDrive;
using googlecloud1.Files;
namespace googlecloud1
{
    public partial class main : Form
    {
        // delegate 대리자 사용 Cross Thread 문제를 방지하기 위해 설정
        private delegate void CSafeSetValue(object sender, long maxsize, long Downloaded, ProgressBar progress);
        private delegate void CSafeComplete(object sender, ProgressBar progress);
        private delegate void CSafeControl(object sender, ProgressBar progress, Label label);
        private delegate void USafeSetValue(object sender, long maxsize, long Downloaded, ProgressBar progress);
        private delegate void USafeComplet(object sender, ProgressBar progress);
        private delegate void USafeControl(object sender, ProgressBar progress, Label label);
        Download down;
        FileUpload up;
        ODConnection connection;
        WebClient webclient;
        DriveService service;
        File file { get; set; }
        private CSafeSetValue cssv;
        private CSafeComplete cscp;
        private CSafeControl csct;
        private USafeSetValue ussv;
        private USafeControl usct;
        private USafeComplet uscp;
        string parent;
        IList<CloudFiles> files;
        CloudFiles item;
        private File SelectedItem { get; set; }
        public main()
        {
            InitializeComponent();
            webclient = new WebClient();
        }

        private async void main_Load(object sender, EventArgs e)
        {
            cssv = new CSafeSetValue(webclient_UploadProgressChanged);
            cscp = new CSafeComplete(webclient_DownloadFileCompleted);
            csct = new CSafeControl(down_StreamControlCallBack);
            uscp = new USafeComplet(up_StreamCompleteCallback);
            ussv = new USafeSetValue(up_StreamProgressCallback);
            usct = new USafeControl(up_StreamControlCallBack);
            await Signin();
        }

        private void webclient_DownloadFileCompleted(object sender, ProgressBar progress)
        {
            if(progress.InvokeRequired)
            {
                progress.Invoke(cscp, new object[] { sender, progress });
            }
            else
            {
                MessageBox.Show("다운 완료");
                int index = downflowPanel.Controls.IndexOf(progress);
                downflowPanel.Controls.RemoveAt(index);
                down.Stop();
            }
        }

        private void webclient_UploadProgressChanged(object sender, long maxsize, long Downloaded, ProgressBar progress)
        {
            if(progress.InvokeRequired)
            {
                progress.Invoke(cssv, new object[] { sender, maxsize, Downloaded, progress });
            }
            else
            {
                float per = ((float)Downloaded / maxsize) * 100;
                progress.Value = (int)per;
                progress.Controls[0].Text = string.Format("{0}%", (int)per);
            }
        }
        /// <summary>
        /// 드라이브 정보를 받아옴
        /// </summary>
        /// <returns></returns>
        public async Task Signin()
        {
            service = Authentication.AuthenticateOauth("892886432316-smcv78utjgpp1iec18v67amr2gigv24m.apps.googleusercontent.com", "eyOFpG-LFIfp8ad3usTL81LG", "bit12");
            if(service != null)
            {
                item = new GoogleFile(service);
                await LoadFolderFromId("root");
            }
        }
        private void ShowWork(bool working)
        {
            this.UseWaitCursor = working;
            this.progressBar1.Visible = working;
        }
        public async Task LoadFolderFromId(string id)
        {
            if (null == service) return;

            //프로그래스바를 진행시킨다.
            ShowWork(true);
            //LoadChildren(null);  // 폴더 뷰를 비운다.
            try
            {
                if(id == "root")
                {
                    FixBreadCrumbForCurrentFolder(id, "내 드라이브");
                }
                parent = id;
                
                ProcessFolder(id);
            }
            catch
            {
                
            }

            ShowWork(false);
        }
        private async void ProcessFolder(string id, bool clearExistingItems = true)
        {
            //폴더가 널이 아니면
            if (null != id)
            {
                flowLayoutPanel_filecontent.SuspendLayout();

                if (clearExistingItems && id != "root")
                    flowLayoutPanel_filecontent.Controls.Clear();
                List<Control> control = await item.GetFileTile(id);
                item.streamNavigateToItemWithChildren += navigatechild;
                item.streamdownload += DownloadAndSaveItem;
                flowLayoutPanel_filecontent.Controls.AddRange(control.ToArray());
            }
            flowLayoutPanel_filecontent.ResumeLayout();
        }

        private async void navigatechild(string id, string name)
        {
            FixBreadCrumbForCurrentFolder(id, name);
            await this.LoadFolderFromId(id);
        }

        private void LoadProperties(File item)
        {
            SelectedItem = item;
            dob.SelectedItem = item;
        }
        private void DownloadAndSaveItem(System.IO.Stream stream, string name, long maxsize)
        {
            if(stream != null)
            {
                var dialog = new SaveFileDialog();
                dialog.FileName = name;          
                dialog.Filter = "All Files (*.*)|*.*";
                var result = dialog.ShowDialog();
                if (result != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }
                down = new FileDownload(dialog.FileName, stream, maxsize);
                down.StreamCompleteCallback += webclient_DownloadFileCompleted;
                down.StreamProgressCallback += webclient_UploadProgressChanged;
                down.StreamControlCallBack += down_StreamControlCallBack;
                down.Start();
            }
        }

        void down_StreamControlCallBack(object sender, ProgressBar progress, Label label)
        {
            if(downflowPanel.InvokeRequired)
            {
                downflowPanel.Invoke(csct, new object[] { sender, progress, label });
            }
            else
            {
                downflowPanel.Controls.Add(progress);
            }
        }

        private void FixBreadCrumbForCurrentFolder(string id, string name)
        {
            var breadcrumbs = flowLayoutPanel1.Controls;
            bool existingCrumb = false;
            foreach (LinkLabel crumb in breadcrumbs)
            {
                if (crumb.Tag == id)
                {
                    RemoveDeeperBreadcrumbs(crumb);
                    existingCrumb = true;
                    break;
                }
            }

            if (!existingCrumb)
            {
                LinkLabel label = new LinkLabel();
                label.Text = "->" + name;
                label.LinkArea = new LinkArea(2, name.Length);
                label.LinkClicked += linkLabelBreadcrumb_LinkClicked;
                label.AutoSize = true;
                label.Tag = id;
                flowLayoutPanel1.Controls.Add(label);
            }
        }

        private void RemoveDeeperBreadcrumbs(LinkLabel crumb)
        {
            var breadcrumbs = flowLayoutPanel1.Controls;
            int indexOfControl = breadcrumbs.IndexOf(crumb);
            for (int i = breadcrumbs.Count - 1; i > indexOfControl; i--)
            {
                breadcrumbs.RemoveAt(i);
            }
        }

        private void linkLabelBreadcrumb_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabel link = (LinkLabel)sender;

            RemoveDeeperBreadcrumbs(link);

            string id = (string)link.Tag;
            if (null == id)
            {

                Task t = LoadFolderFromId("root");
            }
            else
            {
                Task t = LoadFolderFromId(id);
            }
        }
        private void ChildObject_Click(object sender, EventArgs e)
        {
            
        }

        private void 파일업로드ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "All Files (*.*)|*.*";
            var result = dialog.ShowDialog();
            if (result != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            System.IO.FileStream filestr = new System.IO.FileStream(dialog.FileName, System.IO.FileMode.Open);
            up = new FileUpload(service, dialog.FileName, parent, filestr, filestr.Length);
            up.StreamProgressCallback += up_StreamProgressCallback;
            up.StreamCompleteCallback += up_StreamCompleteCallback;
            up.StreamControlCallBack += up_StreamControlCallBack;
            up.Start();
        }

        void up_StreamControlCallBack(object sender, ProgressBar progress, Label label)
        {
            if(uploadflowpanel.InvokeRequired)
            {
               uploadflowpanel.Invoke(usct, new object[] { sender, progress, label });
            }
            else
            {
                uploadflowpanel.Controls.Add(progress);
            }
        }

        void up_StreamCompleteCallback(object sender, ProgressBar progress)
        {
            if(progress.InvokeRequired)
            {
                progress.Invoke(uscp, new object[] { sender, progress });
            }
            else
            {
                MessageBox.Show("업로드 완료");
                int index = uploadflowpanel.Controls.IndexOf(progress);
                uploadflowpanel.Controls.RemoveAt(index);
                up.Stop();
            }
        }

        void up_StreamProgressCallback(object sender, long maxsize, long Downloaded, ProgressBar progress)
        {
            if(progress.InvokeRequired)
            {
                progress.Invoke(ussv, new object[] { sender, maxsize, Downloaded, progress });
            }
            else
            {
                float per = ((float)Downloaded / maxsize) * 100;
                progress.Value = (int)per;
                progress.Controls[0].Text = string.Format("{0}%", (int)per);
            }
        }

        private void 원드라이브로그인ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sign();
        }
        private async void Sign()
        {
            connection = await OneDriveLogin.SignInToMicrosoftAccount(this, "baba", @"C:\Save");
            if (null != connection)
            {
                item = new OneDriveFile(connection);
                await LoadFolderFromId("root");
            }
            //UpdateConnectedStateUx();
        }
    }
}
