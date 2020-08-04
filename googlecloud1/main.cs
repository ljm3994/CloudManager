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
using googlecloud1.login;
using googlecloud1.Files;
using googlecloud1.Folder;
using googlecloud1.FileUpDown;
using Nemiro.OAuth;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows;
using System.Net;
namespace googlecloud1
{
    public struct Move
    {
        public string currentid{get; set;}
        public AllFolder folder { get; set; }
        public List<AllFolder> cl { get; set; }
        public string path { get; set; }
        public bool allmain { get; set; }
    }
    public partial class main : Form
    {
        // 다운로드와 업로드 이벤트 처리를 위한 delegate 처리
        delegate void SafeComplite(object sender);
        delegate void SafeLoadComplete(object sender);
        delegate void SafeDownloadComplete(object sender, Control control, CloudFiles file ,string fileid = null, ListViewGroup group = null);
        delegate void SafeDownloadProgresbar(object sender, long maxsize, long Downloaded, ProgressBar progress);
        delegate void SafeDownloadControl(object sender, Control control);
        delegate void SafeUploadControl(object sender, Control control);
        delegate void SafeTreeNodeComplete(object sender);
        delegate void SafeUploadComplete(object sender, Control control,CloudFiles file, string fileid = null, ListViewGroup group = null);
        delegate void SafeFolderLoadComplite(object sender);
        delegate void SafeAllLogin(object sender);
        //--------------------------------------------------------------------------------------------------------------------------
        Setting setting;
        List<AllFolder> folder;
        SafeAllLogin SAL;
        SafeComplite SC;
        SafeTreeNodeComplete STNC;
        SafeLoadComplete SLC;
        SafeDownloadComplete SDC;
        SafeDownloadProgresbar SDP;
        SafeDownloadControl SDCT;
        SafeUploadControl SUC;
        SafeUploadComplete SUCP;
        SafeFolderLoadComplite SFLC;
        FileTransfer Transfer;
        ListViewGroup group;
        // 현재 선택한 파일의 아이디
        string currentid = "root";
        Stack<Move> move;
        Stack<Move> premove;
        AllFolder currentfolder = null;
        DriveInfo currentdrive = null;
        CloudFiles copyfile = null;
        bool allmain = true;
        // 더블 클릭과 클릭을 구분하기 위한 불형 변수
        bool isfirstclick = true;
        bool doubleclick = false;
        int milliseconds = 0;
        string path = null;  
        //현재 클릭한 파일
        CloudFiles ClickTile = null;
        // 로딩 이미지 변수
        public main()
        {
            InitializeComponent();
            SC = new SafeComplite(serchfile_OnComp);
            SAL = new SafeAllLogin(login_SC);
            SDC = new SafeDownloadComplete(down_StreamCompleteCallback);
            SDP = new SafeDownloadProgresbar(down_StreamProgressCallback);
            SLC = new SafeLoadComplete(load_complete);
            SDCT = new SafeDownloadControl(down_StreamControlCallBack);
            SUC = new SafeUploadControl(upload_StreamControlCallBack);
            SUCP = new SafeUploadComplete(upload_StreamCompleteCallback);
            STNC = new SafeTreeNodeComplete(loadcomplete);
            SFLC = new SafeFolderLoadComplite(FileLoadCompliet);
            FileDataStore datastore = new FileDataStore(@"C:\Test");
            //CloudeLogIn(datastore, @"C:\Test");
        }

        private void main_Load(object sender, EventArgs e)
        {
            folder = new List<AllFolder>();
            LoadImageBox.SizeMode = PictureBoxSizeMode.CenterImage;
            move = new Stack<googlecloud1.Move>();
            premove = new Stack<Move>();
            TrayIcon.Visible = false;
            TreeNode node = new TreeNode();
            node.Name = "allroot";
            node.Text = "통합클라우드";
            this.FolderTree.Nodes.Add(node);     
        }

        private async void CloudeLogIn(FileDataStore datastore, string path)
        {
            List<Authentication.UserNameToken> token = await datastore.AllLoad<Authentication.UserNameToken>();
            if(token != null)
            {
                AllLogin login = new AllLogin(token, path);
                ShowWork();
                login.Start();
                login.SC += login_SC;
            }
        }
        void login_SC(object sender)
        {
            if (this.InvokeRequired)
                this.Invoke(SAL, new object[] { sender });
            AllLogin login = (AllLogin)sender;
            Setting.driveinfo = login.drive;
            HideWork();
            login.Stop();
        }
        // 로그인 메뉴함수
        private void 로그인ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 세팅 화면을 다이알로그로 뛰운다
           setting = new Setting();
            // 세팅 화면에서 확인을 눌렀을 경우
           if(setting.ShowDialog() == DialogResult.OK)
           {
               if(setting.logout)
               {
                   FolderTree.Nodes[0].Nodes.Clear();
                   setting.currentdriveinfo = Setting.driveinfo;
               }
               setting.logout = false;
               //현재 화면을 지운다.
               FileContentView.Items.Clear();
               //파일을 로드한다.
              FileLoad("root", setting.currentdriveinfo);
              path = "root://";
           }
        }
        /// <summary>
        /// 파일 로드 함수
        /// </summary>
        /// <param name="id">파일의 id</param>
        private void FileLoad(string id, List<DriveInfo> drive)
        {
            //로딩화면을 visible로 두고 나머지를 안보이게 숨긴다.
            ShowWork();
            // 로드 폴더 클래스를 선언한다.
            TreeNode tnode = new TreeNode();
            if(drive != null)
            {
                foreach (var item in drive)
                {
                    tnode = new TreeNode();
                    tnode.Name = item.DriveID;
                    tnode.Text = item.token.Drive;
                    tnode.Tag = item;
                    FolderTree.Nodes[0].Nodes.Add(tnode);
                    List<DriveInfo> sellectdrive = new List<DriveInfo>();
                    sellectdrive.Add(item);
                    LoadTreeNode loadtreeenode = new LoadTreeNode(id, null, sellectdrive, tnode);
                    loadtreeenode.complete += loadcomplete;
                    loadtreeenode.Start();
                }
            }
            LoadFolder load = new LoadFolder(id, null, drive);
            // 로드 폴더 클래스의 이벤트를 연결한다.
            load.complete += load_complete;
            // 쓰레드 시작
            load.Start();
        }
        private void FileLoadCompliet(object sender)
        {
            if (this.InvokeRequired)
                this.Invoke(SFLC, new object[] { sender });
            LoadFolder load = (LoadFolder)sender;
            folder = load.All;
            if(folder.Count > 1)
            {
                allmain = true;
            }
            else
            {
                allmain = false;
            }
            // 아이콘 이미지 리스트를 생성하고 불러온다.
            ImageList list = FileIcon.GetImageList(folder);
            list.ImageSize = new System.Drawing.Size(32, 32);
            FileContentView.LargeImageList = list;
            int index = 0;
            setting.currentdriveinfo.Clear();
            foreach (var item in folder)
            {
                currentfolder = item;
                // 현재 리스트 뷰의 타일들을 생성한다.
                index = LoadTile(item, index);
            }
            setting.currentdriveinfo.Clear();
            Path_NaviGation_TB.Text = path;
            // 쓰레드 정지
            HideWork();
            load.Stop();
        }
        // 파일 로드가 완료되면
        void load_complete(object sender)
        {
            // Cross Thread 문제 : 메인 쓰레드에서 그려진 UI를 다른 쓰레드가 직접 접근하게 되면 발생하는 문제 이를 해결하기 위해 invoke를 필요한 상태인지 체크하여  UI를 델리게이트로 넘겨서 처리
            //(즉 Main Thread가 본인이 하던일을 다 마치고 난 후에 처리하라는 의미이다)
            if (this.InvokeRequired)
                this.Invoke(SLC, new object[] { sender });
            // 로그인한 계정들을 불러온다.
            LoadFolder load = (LoadFolder)sender;
            folder = load.All;
            if (folder.Count > 1)
            {
                allmain = true;
            }
            else
            {
                allmain = false;
            }
            // 아이콘 이미지 리스트를 생성하고 불러온다.
            ImageList list = FileIcon.GetImageList(folder);
            list.ImageSize = new System.Drawing.Size(32, 32);
            FileContentView.LargeImageList = list;
            int index = 0;
            foreach (var item in folder)
            {
                currentfolder = item;
                // 현재 리스트 뷰의 타일들을 생성한다.
                index = LoadTile(item, index);
            }
            setting.currentdriveinfo.Clear();
            Path_NaviGation_TB.Text = path;
            // 쓰레드 정지
            HideWork();
            load.Stop();
        }

        private TreeNode CreateNode(AllFolder folder, TreeNode node)
        {
            List<CloudFiles> file = folder.GetFiles();
            foreach (var item in file)
            {
                if(item.Item.IsFile == false)
                {
                    TreeNode tnode = new TreeNode();
                    tnode.Name = item.Item.FileID;
                    tnode.Text = item.Item.FileName;
                    tnode.Tag = item;
                    node.Nodes.Add(tnode);
                }
            }
            return node;
        }
        /// <summary>
        /// 로딩 화면 시작
        /// </summary>
        protected internal void ShowWork()
        {
            LoadImageBox.Image = global::googlecloud1.Properties.Resources.ajax_loader;
            this.toolStripStatusLabel1.Text = "로딩중";
            this.FileContentView.Visible = false;
            this.FolderTree.Visible = false;
            this.LoadImageBox.Visible = true;
        }
        /// <summary>
        /// 로딩 화면 중지
        /// </summary>
        protected internal void HideWork()
        {
            this.FileContentView.Visible = true;
            this.toolStripStatusLabel1.Text = "로딩완료";
            this.LoadImageBox.Visible = false;
            this.FolderTree.Visible = true;
            this.LoadImageBox.Image = null;
        }
        /// <summary>
        /// ListView의 타일들을 생성해준다.
        /// </summary>
        /// <param name="folder"> 현재 로그인한 folder들</param>
        /// <param name="index"> 이미지리스트의 index</param>
        /// <returns></returns>
        private int LoadTile(AllFolder folder, int index)
        {
           if(folder != null)
           {
               //listview의 업데이트가 끝나기 전까지는 컨트롤이 다시 그려지지 않게한다.
               FileContentView.BeginUpdate();
               //로그인한 게정의 파일들을 불러온다,
               List<CloudFiles> cl = folder.GetFiles();
               // listview의 보기 모드를 설정한다.
               FileContentView.View = View.LargeIcon;
               // listview의 그룹보기를 true로 설정한다.
               FileContentView.ShowGroups = true;
               // 그룹의 이름을 현재 드라이브의 이름과 아이디로 설정한다.
               ListViewGroup group = new ListViewGroup(folder.DriveName());
               // 그룹의 태그로 현재 드라이브를 넣는다.
               group.Tag = folder;
               foreach (var item in cl)
                {
                   // 리스트뷰의 아이템 설정
                   ListViewItem listitem = new ListViewItem(item.Item.FileName, index);
                   listitem.Tag = item;
                   listitem.SubItems.Add(item.Item.FileSize.ToString());
                   listitem.SubItems.Add(item.Item.modifiedDate);
                   group.Items.Add(listitem);
                   FileContentView.Items.Add(listitem);
                   index++;
                }
               FileContentView.Groups.Add(group);
               //FileContentView.Columns.Add("파일명", 200, HorizontalAlignment.Left);
               //FileContentView.Columns.Add("사이즈", 70, HorizontalAlignment.Left);
               //FileContentView.Columns.Add("날짜", 100, HorizontalAlignment.Left);
               // 리스트 뷰의 업데이트가 끝났음을 알리고 이제 컨트롤이 그려지기를 명령한다.
               FileContentView.EndUpdate();
               // 현재 이미지의 index 번호를 반환한다.
               return index;
           }
           return 0;
        }
        void item_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }
        void item_DragDrop(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                
            }
        }
        /// <summary>
        /// 다운로드 프로그래스바의 진행을 업데이트 해주는 이벤트
        /// </summary>
        /// <param name="sender">download클래스 자기 자신</param>
        /// <param name="maxsize">down받는 파일의 최대 크기</param>
        /// <param name="Downloaded">현재까지 다운로드된 크기</param>
        /// <param name="progress">업데이트를 해줄 프로그래스 바</param>
        void down_StreamProgressCallback(object sender, long maxsize, long Downloaded, ProgressBar progress)
        {
            if (progress.InvokeRequired)
                progress.Invoke(SDP, new object[] { sender, maxsize, Downloaded, progress });
            else
            {
                if(maxsize > 0)
                {
                    // 현재까지 다운로드된 크기를 백분율로 계산한다.
                    float per = ((float)Downloaded / maxsize) * 100;
                    progress.Value = (int)per;
                    progress.Controls[0].Text = string.Format("{0}%", (int)per);
                }
            }
        }
        // 다운로드가 시작되면 컨트롤을 그려줄 함수
        void down_StreamControlCallBack(object sender, Control control)
        {
            if(downflowPanel.InvokeRequired)
                downflowPanel.Invoke(SDCT, new object[] { sender, control });
            else
            {
                downflowPanel.Controls.Add(control);
            }
        }
        /// <summary>
        /// 다운로드가 완료되면 발생하는 이벤트
        /// </summary>
        /// <param name="sender">자기자신</param>
        /// <param name="progress">다운로드가 완료된 프로그래스 바</param>
        /// <param name="file"> 다운로드가 완료된 file</param>
        /// <param name="fileid">다운로드가 완료된 파일 아이디</param>
        void down_StreamCompleteCallback(object sender, Control control, CloudFiles file = null, string fileid = null, ListViewGroup group = null)
        {
            if (control.InvokeRequired)
                control.Invoke(SDC, new object[] { sender, control, file, fileid, group });
            else
            {
                //트레이 아이콘에 완료 상태 메시지를 띄워준다.
                TrayIcon.ShowBalloonTip(10000, "다운완료", "파일 다운이 완료 되었습니다", ToolTipIcon.Info);
                MessageBox.Show("다운 완료");
                // 패널에서 다운로드가 완료된 프로그래스 바를 지워준다.
                int index = downflowPanel.Controls.IndexOf(control);
                downflowPanel.Controls.RemoveAt(index);
                Transfer.Stop();       
            }
        }
        /// <summary>
        /// 폴더를 더블클릭하면 실행되는 함수(폴더의 자식 파일들을 불러와준다)
        /// </summary>
        /// <param name="item">더블클릭한 파일</param>
        private void NavigateToItem(CloudFiles item)
        {
            // 현재 화면을 지워준다.
            FileContentView.Items.Clear();
            if ((item) != null)
            {
                // 현재 파일 id를 갱신해준다.
                this.currentid = item.Item.FileID;
                // 파일을 불러온다.
                List<DriveInfo> drive = new List<DriveInfo>();
                drive.Add(item.Item.driveinfo);
                LoadFolder load = new LoadFolder(item.Item.FileID, null, drive);
                path = item.Item.Path;
                ShowWork();
                // 로딩 완료 이벤트 연결
                load.complete += FileLoadCompliet;
                load.Start();
            }
           else
           {
                MessageBox.Show("파일 열기 실패");
            }
        }
        /// <summary>
        /// 연결메뉴 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 연결ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //연결을 누르면 지금 보이는 화면을 비워준다.
            FileContentView.Items.Clear();
            // 새로 로그인을 해주기 위해 폴더 리스트를 비워준다.
            FolderTree.Nodes[0].Nodes.Clear();
            folder.Clear();
            //처음 시작 root경로를 불러온다.
            FileLoad("root", Setting.driveinfo);
        }
        /// <summary>
        /// 업로드 메뉴
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 업로드ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileUpload();
        }
        private void FileUpload()
        {
            // 로그인된 id가 있는지 확인
            if (Setting.driveinfo == null)
            {
                return;
            }
            if (this.allmain)
            {
                // 업로드 할 클라우드를 선택하는 창
                CloudeSelect select = new CloudeSelect(folder);
                DialogResult result = select.ShowDialog(this);

                if (DialogResult.OK == result)
                {
                    this.currentfolder = select.drive;
                }
                else
                {
                    return;
                }
                if (this.currentfolder == null)
                {
                    return;
                }
            }
            // 파일 다이얼로그 열기
            OpenFileDialog open = new OpenFileDialog();
            if (open.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            int i = 0;
            for (i = 0; i < FileContentView.Groups.Count; i++)
            {
                if(currentfolder == FileContentView.Groups[i].Tag)
                {
                    break;
                }
            }
            // 파일의 스트림을 얻어온다.
            System.IO.FileStream filestream = new System.IO.FileStream(open.FileName, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite);
            // 얻어온 스트림에 파일을 업로드 한다.
            if (filestream.Length > 0)
            {
                Transfer = currentfolder.upladfile(filestream, System.IO.Path.GetFileName(open.FileName), currentid, FileContentView.Groups[i]);
            }
            else
            {
                MessageBox.Show("파일의 크기가 0인 파일은 업로드 할 수 없습니다");
                return;
            }
            if (Transfer != null)
            {
                Transfer.StreamControlCallBack += upload_StreamControlCallBack;
                Transfer.StreamProgressCallback += upload_StreamProgressCallback;
                Transfer.StreamCompleteCallback += upload_StreamCompleteCallback;
                Transfer.Start();
            }
        }
        /// <summary>
        /// 업로드 프로그래스 바를 진행시키는 함수
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="maxsize"> 올린 파일의 최대 크기</param>
        /// <param name="Downloaded"> 지금 까지 다운로드 완료된 파일 크기</param>
        /// <param name="progress"> 업로드시킬 프로그래스 바</param>
        void upload_StreamProgressCallback(object sender, long maxsize, long Downloaded, ProgressBar progress)
        {
            if (progress.InvokeRequired)
                progress.Invoke(SDP, new object[] { sender, maxsize, Downloaded, progress });
            else
            {
                if (maxsize > 0)
                {
                    float per = ((float)Downloaded / maxsize) * 100;
                    progress.Value = (int)per;
                    progress.Controls[0].Text = string.Format("{0}%", (int)per);
                }
            }
        }
        /// <summary>
        /// 업로드 완료 되면 생기는 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="progress"></param>
        /// <param name="file"></param>
        /// <param name="fileid"></param>
        void upload_StreamCompleteCallback(object sender, Control control, CloudFiles file = null, string fileid = null, ListViewGroup group = null)
        {
            if (control.InvokeRequired)
                control.Invoke(SUCP, new object[] { sender, control, file, fileid, group });
            else
            {
                MessageBox.Show(file.Item.FileName + " 업로드 완료");
                int index = uploadflowpanel.Controls.IndexOf(control);
                uploadflowpanel.Controls.RemoveAt(index);
                Transfer.Stop();
                TrayIcon.ShowBalloonTip(10000, "업로드완료", "파일의 업로드가 완료 되었습니다", ToolTipIcon.Info);
                if(file.Item.IsFile == false)
                {
                    foreach (TreeNode item in FolderTree.Nodes[0].Nodes)
                    {
                        if(item.Tag == file.Item.driveinfo)
                        {
                            if(fileid == "root")
                            {
                                TreeNode tnode = new TreeNode();
                                tnode.Name = file.Item.FileID;
                                tnode.Text = file.Item.FileName;
                                tnode.Tag = file;
                                item.Nodes.Add(tnode);
                                break;
                            }
                            foreach (TreeNode node in item.Nodes)
                            {
                                if(((CloudFiles)node.Tag).Item.FileID == fileid)
                                {
                                    TreeNode tnode = new TreeNode();
                                    tnode.Name = file.Item.FileID;
                                    tnode.Text = file.Item.FileName;
                                    tnode.Tag = file;
                                    item.Nodes.Add(tnode);                                    
                                }
                            }
                        }
                    }
                }
                if(fileid == "/")
                {
                    fileid = "root";
                }
                if(fileid == currentid)
                {
                    Bitmap imag = FileIcon.GetIcon(file);
                    FileContentView.LargeImageList.Images.Add(imag);
                    ListViewItem listitem = new ListViewItem(file.Item.FileName, FileContentView.LargeImageList.Images.Count - 1);
                    listitem.Tag = file;
                    listitem.SubItems.Add(file.Item.FileSize.ToString());
                    listitem.SubItems.Add(file.Item.modifiedDate);
                    group.Items.Add(listitem);
                    FileContentView.Items.Add(listitem);
                }
            }
        }
        /// <summary>
        /// 업로드 컨트롤을 생성해주는 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="progress"></param>
        /// <param name="label"></param>
        void upload_StreamControlCallBack(object sender, Control control)
        {
            if (uploadflowpanel.InvokeRequired)
                uploadflowpanel.Invoke(SUC, new object[] { sender, control });
            else
            {
                uploadflowpanel.Controls.Add(control);
            }
        }
        /// <summary>
        /// 파일 오픈 메뉴 버튼 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileOpenMenuSteip_Click(object sender, EventArgs e)
        {
            ListView tile = (ListView)ClickMenu.SourceControl;
            CloudFiles item = (CloudFiles)tile.FocusedItem.Tag;
            if(item.Item.IsFile)
            {
                NavigateToItem(item);
            }
            else
            {
                MessageBox.Show("파일은 열수 없습니다");
            }
        }
        /// <summary>
        /// 다운로드 버튼 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownClickMenu_Click(object sender, EventArgs e)
        {
            ListView tile = (ListView)ClickMenu.SourceControl;
            // 리스트 뷰에서 포커스가 가있는 아이템을 가져온다.
            foreach (ListViewItem item in tile.SelectedItems)
            {
                CloudFiles file = (CloudFiles)item.Tag;
                DownLoad(file);
            }
        }
        private void DownLoad(CloudFiles item)
        {
            if(item.Item.IsFile == true)
            {
                SaveFileDialog filedialog = new SaveFileDialog();
                filedialog.AddExtension = true;
                filedialog.Title = "파일 저장";
                filedialog.FileName = item.Item.FileName;
                filedialog.Filter = "모든 파일(*.*)|*.*";
                var result = filedialog.ShowDialog();
                if (result != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }
                Transfer = new FileDownload(filedialog.FileName, item.Item.FileSize, item.Item.DownUrl, item.Item.driveinfo.token.access_token, item.Item.FileID);
                Transfer.StreamCompleteCallback += down_StreamCompleteCallback;
                Transfer.StreamControlCallBack += down_StreamControlCallBack;
                Transfer.StreamProgressCallback += down_StreamProgressCallback;
                Transfer.Start();
            }
            else
            {
                FolderBrowserDialog folderdialog = new FolderBrowserDialog();
                folderdialog.Description = "저장할 폴더 위치 선택 : ";
                var result = folderdialog.ShowDialog();
                if (result != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }
                Transfer = new FolderDownload(item, string.Format("{0}\\{1}", folderdialog.SelectedPath, item.Item.FileName));
                Transfer.StreamCompleteCallback += down_StreamCompleteCallback;
                Transfer.StreamControlCallBack += down_StreamControlCallBack;
                Transfer.StreamProgressCallback += down_StreamProgressCallback;
                Transfer.Start();
            }
        }
        private async void 삭제ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection collect = FileContentView.SelectedItems;
            foreach (ListViewItem item in collect)
            {
                CloudFiles fileitem = (CloudFiles)item.Tag;
                bool Delete = false;
                try
                {
                    Delete = await fileitem.FileDelete();
                    if (Delete)
                    {
                        FileContentView.Items.Remove(item);
                        foreach (var folderitem in folder)
                        {
                            if(fileitem.Item.driveinfo == folderitem.driveinfo)
                            {
                                folderitem.RemoveFile(fileitem);
                            }
                        }
                        MessageBox.Show("삭제 완료");
                    }
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            }
            
        }
        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            this.Visible = true;
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
            TrayIcon.Visible = false;
        }

        private void main_FormClosing(object sender, FormClosingEventArgs e)
        {
            TrayIcon.Visible = false;
        }

        private void TrayFormOpen_Click(object sender, EventArgs e)
        {
            this.Visible = true;
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
            TrayIcon.Visible = false;
        }

        private void FormClose_Click(object sender, EventArgs e)
        {
            TrayIcon.Visible = false;
            Application.Exit();
        }

        private void main_DragDrop(object sender, DragEventArgs e)
        {
            if(Setting.driveinfo != null)
            {
                if(e.Data.GetData(DataFormats.FileDrop) is FileInfo)
                {
                    MessageBox.Show("잘못된 장소입니다");
                    return;
                }
                string[] filepath = (string[])e.Data.GetData(DataFormats.FileDrop);
                if(filepath != null)
                {
                    try
                    {
                        foreach (var path in filepath)
                        {
                            System.IO.FileStream filestream = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite);
                            if(currentid == "root")
                            {
                                CloudeSelect select = new CloudeSelect(folder);
                                DialogResult result = select.ShowDialog(this);
                                if (DialogResult.OK == result)
                                {
                                    currentfolder = select.drive;
                                }
                            }
                            if(currentfolder != null)
                            {
                                int i = 0;
                                for (i = 0; i < FileContentView.Groups.Count; i++)
                                {
                                    if(currentfolder == FileContentView.Groups[i].Tag)
                                    {
                                        break;
                                    }
                                }
                                Transfer = currentfolder.upladfile(filestream, System.IO.Path.GetFileName(path), currentid, FileContentView.Groups[i]);
                            }
                            if (Transfer != null)
                            {
                                Transfer.StreamControlCallBack += upload_StreamControlCallBack;
                                Transfer.StreamProgressCallback += upload_StreamProgressCallback;
                                Transfer.StreamCompleteCallback += upload_StreamCompleteCallback;
                                Transfer.Start();
                            }
                        }
                    }
                    catch(Exception error)
                    {
                        MessageBox.Show("오류" + error.Message);
                    }
                }
            }
        }
        private void main_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }
        private void FileInfo_Click(object sender, EventArgs e)
        {
            ListView tile = (ListView)ClickMenu.SourceControl;
            CloudFiles item = (CloudFiles)tile.FocusedItem.Tag;
            FileInfoForm fileinfoform = new FileInfoForm(item);
            fileinfoform.ShowDialog();
        }

        private void main_Resize(object sender, EventArgs e)
        {
            if(FormWindowState.Minimized == WindowState)
            {
                this.Hide();
                TrayIcon.Visible = true;
                this.Hide();
            }   
        }

        private void FileNameChange_Click(object sender, EventArgs e)
        {
            ListView tile = (ListView)ClickMenu.SourceControl;     
        }

        private void TS_driveinfo_Click(object sender, EventArgs e)
        {
            DriveInfoForm driveinfoform = new DriveInfoForm(Setting.driveinfo);
            driveinfoform.ShowDialog();
        }

        private void doubleck_timer_Tick(object sender, EventArgs e)
        {
            milliseconds += 100;
            if(milliseconds >= SystemInformation.DoubleClickTime)
            {
                doubleck_timer.Stop();

                if(doubleclick == false && ClickTile != null)
                {
                    CloudFiles file = ClickTile;
                   DataObject dataobj = new DataObject();
                   dataobj.SetData(DataFormats.Text, file.Item.FileName);
                   FileContentView.DoDragDrop(new DataObject(DataFormats.FileDrop, file.Item), DragDropEffects.Move);
                }
                ClickTile = null;
                isfirstclick = true;
                doubleclick = false;
                milliseconds = 0;
            }
        }

        private void MenuNewFolder_Click(object sender, EventArgs e)
        {
            NewFolderCreate();
        }

        private void FileContentView_MouseClick(object sender, MouseEventArgs e)
        {
            ListView contentfilelistview = (ListView)sender;
            if(contentfilelistview.FocusedItem != null)
            {
                if(e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    this.FileContentView.DoDragDrop((sender as ListView).SelectedItems, DragDropEffects.Copy);
                }
                else if(e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    ClickMenu.Show(contentfilelistview, new Point(e.X, e.Y));
                }
            }
            else
            {
                this.contextMenuStrip1.Show(contentfilelistview, new Point(e.X, e.Y));
            }
        }

        private void FileContentView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListView contentfilelistview = (ListView)sender;
            if(contentfilelistview.FocusedItem != null)
            {
                if(e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    var item = (CloudFiles)contentfilelistview.FocusedItem.Tag;
                    if (item.DoubleClick())
                    {
                        SaveFileDialog filedialog = new SaveFileDialog();
                        filedialog.AddExtension = true;
                        filedialog.Title = "파일 저장";
                        filedialog.FileName = item.Item.FileName;
                        filedialog.Filter = "모든 파일(*.*)|*.*";
                        var result = filedialog.ShowDialog();
                        if (result != System.Windows.Forms.DialogResult.OK)
                        {
                            return;
                        }
                        Transfer = new FileDownload(filedialog.FileName, item.Item.FileSize, item.Item.DownUrl, item.Item.driveinfo.token.access_token, item.Item.FileID);
                        Transfer.StreamCompleteCallback += down_StreamCompleteCallback;
                        Transfer.StreamControlCallBack += down_StreamControlCallBack;
                        Transfer.StreamProgressCallback += down_StreamProgressCallback;
                        Transfer.Start();
                    }
                    else
                    {
                        Move temp = new googlecloud1.Move();
                        temp.cl = folder;
                        temp.currentid = currentid;
                        temp.allmain = this.allmain;
                        temp.path = Path_NaviGation_TB.Text;
                        move.Push(temp);
                        if(premove.Count > 0)
                        {
                            premove.Clear();
                        }
                        NavigateToItem(item);
                    }
                }
            }
        }
        private void FileContentView_MouseDown(object sender, MouseEventArgs e)
        {
            //ListView content = (ListView)sender;
            //content.FocusedItem = null;
            //if(content.FocusedItem != null)
            //{
            //    if(e.Button == System.Windows.Forms.MouseButtons.Left)
            //    {
            //        if (this.isfirstclick)
            //        {
            //            ClickTile = (CloudFiles)content.FocusedItem.Tag;
            //            doubleck_timer.Start();
            //        }
            //    }
            //}
        }

        private void PrevTool_Click(object sender, EventArgs e)
        {
            if(move.Count > 0)
            {
                googlecloud1.Move prev = new googlecloud1.Move();
                prev.cl = folder;
                prev.currentid = currentid;
                prev.allmain = this.allmain;
                prev.path = Path_NaviGation_TB.Text;
                prev.folder = currentfolder;
                googlecloud1.Move post = move.Pop();
                currentfolder = post.folder;
                currentid = post.currentid;
                this.allmain = post.allmain;
                FileContentView.Clear();
                ImageList list = FileIcon.GetImageList(post.cl);
                list.ImageSize = new System.Drawing.Size(32, 32);
                FileContentView.LargeImageList = list;
                int index = 0;
                foreach (var item in post.cl)
                {
                    index = LoadTile(item, index);
                }
                path = post.path;
                Path_NaviGation_TB.Text = path;
                folder = post.cl;
                premove.Push(prev);
            }
        }
        private void NextTool_Click(object sender, EventArgs e)
        {
            if(premove.Count > 0)
            {
                googlecloud1.Move post = new googlecloud1.Move();
                post.cl = folder;
                post.currentid = currentid;
                post.allmain = this.allmain;
                post.path = Path_NaviGation_TB.Text;
                googlecloud1.Move prev = premove.Pop();
                currentid = prev.currentid;
                this.allmain = prev.allmain;
                FileContentView.Clear();
                ImageList list = FileIcon.GetImageList(prev.cl);
                list.ImageSize = new System.Drawing.Size(32, 32);
                FileContentView.LargeImageList = list;
                int index = 0;
                foreach (var item in prev.cl)
                {
                    index = LoadTile(item, index);
                }
                Path_NaviGation_TB.Text = prev.path;
                folder = prev.cl;
                move.Push(post);
            }
        }
        private void ToolCopy_Click(object sender, EventArgs e)
        {
            ListView tile = (ListView)ClickMenu.SourceControl;
            CloudFiles item = (CloudFiles)tile.FocusedItem.Tag;
            copyfile = item;
            ToolPast.Enabled = true;
            TS_paste.Enabled = true;
        }
        private void ToolPast_Click(object sender, EventArgs e)
        {
            if(copyfile != null)
            {
                ListView tile = (ListView)ClickMenu.SourceControl;
                CloudFiles item = (CloudFiles)tile.FocusedItem.Tag;
                bool serch = false;
                int i = 0;
                for (i = 0; i < tile.Groups.Count; i++)
			    {
                    foreach (ListViewItem listitem in tile.Groups[i].Items)
                    {
                        if (listitem.Tag == item)
                        {
                            serch = true;
                            break;
                        }
                    }
                    if(serch)
                    {
                        break;
                    }
			    }
                DriveToDriveTransFer(item, tile.Groups[i]);
                copyfile = null;
                ToolPast.Enabled = false;
                TS_paste.Enabled = false;
            }
            else
            {
                return;
            }
        }
        private async void DriveToDriveTransFer(CloudFiles item, ListViewGroup group)
        {
            if (item.Item.driveinfo != copyfile.Item.driveinfo)
            {
                if (item.Item.IsFile == false)
                {
                    try
                    {
                        HttpWebResponse rp = await HttpHelper.RequstHttp("GET", copyfile.Item.DownUrl, null, copyfile.Item.driveinfo.token.access_token);
                        System.IO.Stream stream = rp.GetResponseStream();
                        Transfer = new FileUpload(stream, copyfile.Item.FileName, copyfile.Item.FileSize, item.Item.FileID, item.Item.driveinfo, group);
                        Transfer.StreamControlCallBack += upload_StreamControlCallBack;
                        Transfer.StreamProgressCallback += upload_StreamProgressCallback;
                        Transfer.StreamCompleteCallback += upload_StreamCompleteCallback;
                        Transfer.Start();
                    }
                    catch (WebException err)
                    {
                        MessageBox.Show("옮기기 오류 : " + err.Message);
                    }
                    catch (Exception error)
                    {
                        MessageBox.Show("옮기기 오류 : " + error.Message);
                    }
                }
                else
                {
                    MessageBox.Show("아직 지원되지 않는 기능입니다");
                }
            }
            else
            {
                try
                {
                    if(item.Item.IsFile == false)
                    {
                        await copyfile.CopyFile(item.Item);
                        MessageBox.Show("복사 완료");
                    }
                    else
                    {
                        MessageBox.Show("파일로 붙여넣을 수 없습니다");
                        return;
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                return;
            }
        }

        private void NewFolder_Click(object sender, EventArgs e)
        {
            NewFolderCreate();
        }
        private void btn_rename_Click(object sender, EventArgs e)
        {
            rename();
        }
        public void rename()
        {
            if (FileContentView.SelectedItems.Count != 0)
            {
                foreach (ListViewItem item in FileContentView.SelectedItems)
                {
                    CloudFiles file = (CloudFiles)item.Tag;
                    CreateFolder createfolder = new CreateFolder(file, NewFileOption.RENAME);
                    var dialogresult = createfolder.ShowDialog();
                    if (dialogresult == System.Windows.Forms.DialogResult.OK)
                    {
                        item.Text = file.Item.FileName;
                    }
                }
            }
        }
        public void NewFolderCreate()
        {
            if (allmain)
            {
                CloudeSelect select = new CloudeSelect(folder);
                DialogResult result = select.ShowDialog(this);

                if (DialogResult.OK == result)
                {
                    this.currentfolder = select.drive;
                }
                if (this.currentfolder == null)
                {
                    return;
                }
            }
            CreateFolder createfolder = new CreateFolder(currentfolder.driveinfo, currentfolder, currentid, NewFileOption.CREATEFOLDER);
            var createresult = createfolder.ShowDialog();
            if (createresult == System.Windows.Forms.DialogResult.OK)
            {
                CloudFiles addfolder = createfolder.file;
                Bitmap bitmap = FileIcon.GetIcon(addfolder);
                FileContentView.LargeImageList.Images.Add(bitmap);
                ListViewItem listitem = new ListViewItem(addfolder.Item.FileName, FileContentView.LargeImageList.Images.Count - 1);
                listitem.Tag = addfolder;
                listitem.SubItems.Add(addfolder.Item.FileSize.ToString());
                listitem.SubItems.Add(addfolder.Item.modifiedDate);
                for (int i = 0; i < FileContentView.Groups.Count; i++)
                {
                    if (FileContentView.Groups[i].Tag == currentfolder)
                    {
                        FileContentView.Groups[i].Items.Add(listitem);
                    }
                }
                FileContentView.Items.Add(listitem);
            }
        }
        private void CutCopyTool_Click(object sender, EventArgs e)
        {

        }

        private void 폴더업로드testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Upload();
        }
        private void Upload()
        {
            if (allmain)
            {
                CloudeSelect select = new CloudeSelect(folder);
                DialogResult result = select.ShowDialog(this);

                if (DialogResult.OK == result)
                {
                    this.currentfolder = select.drive;
                }
                else
                {
                    return;
                }
                if (this.currentfolder == null)
                {
                    return;
                }
            }
            FolderBrowserDialog folderdialog = new FolderBrowserDialog();
            folderdialog.Description = "저장할 폴더 위치 선택 : ";
            var result1 = folderdialog.ShowDialog();
            if (result1 != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            try
            {
                int i = 0;
                for (i = 0; i < FileContentView.Groups.Count; i++)
                {
                    if(FileContentView.Groups[i].Tag == currentfolder)
                    {
                        break;
                    }
                }
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(folderdialog.SelectedPath);
                Transfer = new FolderUpload(dir, currentfolder, this.currentid, currentfolder.driveinfo, FileContentView.Groups[i]);
                Transfer.StreamCompleteCallback += upload_StreamCompleteCallback;
                Transfer.StreamControlCallBack += upload_StreamControlCallBack;
                Transfer.StreamProgressCallback += upload_StreamProgressCallback;
                Transfer.Start();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        private void FolderTree_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if(e.Node.Text == "통합클라우드")
            {
                return;
            }
            if(e.Node.Nodes.Count > 0)
            {
                foreach (TreeNode item in e.Node.Nodes)
                {
                    if (item.Nodes.Count == 0)
                    {
                        CloudFiles file = item.Tag as CloudFiles;
                        List<DriveInfo> drive = new List<DriveInfo>();
                        drive.Add(file.Item.driveinfo);
                        LoadFolder load = new LoadTreeNode(file.Item.FileID, null, drive, item);
                        load.Start();
                        load.complete += loadcomplete;
                    }
                } 
            }
        }
        private void loadcomplete(object sender)
        {
            if (this.InvokeRequired)
                this.Invoke(STNC, new object[] { sender });
            LoadTreeNode treenode = (LoadTreeNode)sender;
            List<AllFolder> folder = treenode.All;
            foreach (var item in folder)
            {
                CreateNode(item, treenode.node);
            }  
            treenode.Stop();
        }

        private void FolderTree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if(e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if(Setting.driveinfo.Count != 0)
                {
                    if (e.Node.Tag is CloudFiles)
                    {
                        CloudFiles item = (CloudFiles)e.Node.Tag;
                        Move temp = new googlecloud1.Move();
                        temp.cl = folder;
                        temp.currentid = currentid;
                        temp.path = Path_NaviGation_TB.Text;
                        move.Push(temp);
                        if (premove.Count > 0)
                        {
                            premove.Clear();
                        }
                        NavigateToItem(item);
                    }
                    else if (e.Node.Tag is DriveInfo)
                    {
                        FileContentView.Clear();
                        // 현재 파일 id를 갱신해준다.
                        this.currentid = "root";
                        // 파일을 불러온다.
                        Move temp = new googlecloud1.Move();
                        temp.cl = folder;
                        temp.currentid = currentid;
                        temp.path = Path_NaviGation_TB.Text;
                        move.Push(temp);
                        if (premove.Count > 0)
                        {
                            premove.Clear();
                        }
                        currentdrive = (DriveInfo)e.Node.Tag;
                        List<DriveInfo> drive = new List<DriveInfo>();
                        drive.Add((DriveInfo)e.Node.Tag);
                        string id = "root";
                        LoadFolder load = new LoadFolder(id, null, drive);
                        path = "root://" + ((DriveInfo)e.Node.Tag).token.Drive;
                        ShowWork();
                        // 로딩 완료 이벤트 연결
                        load.complete += FileLoadCompliet;
                        load.Start();
                    }
                    else
                    {
                        FileContentView.Clear();
                        string id = "root";
                        LoadFolder load = new LoadFolder(id, null, Setting.driveinfo);
                        ShowWork();
                        path = "root://";
                        // 로딩 완료 이벤트 연결
                        load.complete += FileLoadCompliet;
                        load.Start();
                    }
                }
                else
                {
                    MessageBox.Show("계정 목록이 없습니다");
                }
            }
        }
        private void btn_GoHome_Click(object sender, EventArgs e)
        {
            if(Setting.driveinfo != null)
            {
                FileContentView.Clear();
                string id = "root";
                LoadFolder load = new LoadFolder(id, null, Setting.driveinfo);
                ShowWork();
                path = "root://";
                // 로딩 완료 이벤트 연결
                load.complete += FileLoadCompliet;
                load.Start();
            }
            else
            {

            }
        }
        private void FileContentView_MouseDown_1(object sender, MouseEventArgs e)
        {
            ListView contentfilelistview = (ListView)sender;
            if (contentfilelistview.SelectedItems.Count > 0)
            {
                if(e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    ClickMenu.Show(contentfilelistview, new Point(e.X, e.Y));
                }
            }
            else
            {
                if(e.Button == System.Windows.Forms.MouseButtons.Right)
                {       
                    contextMenuStrip1.Show(contentfilelistview, new Point(e.X, e.Y));
                }
            }
        }
        private void btn_Down_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in FileContentView.SelectedItems)
            {
                CloudFiles file = (CloudFiles)item.Tag;
                DownLoad(file);
            }
        }

        private void TS_paste_Click(object sender, EventArgs e)
        {
            if (copyfile != null)
            {
                if (allmain)
                {
                    CloudeSelect select = new CloudeSelect(folder);
                    DialogResult result = select.ShowDialog(this);

                    if (DialogResult.OK == result)
                    {
                        this.currentfolder = select.drive;
                    }
                    if (this.currentfolder == null)
                    {
                        return;
                    }
                }
                try
                {
                    int i = 0;
                    for (i = 0; i < FileContentView.Groups.Count; i++)
                    {
                        if (currentfolder == FileContentView.Groups[i].Tag)
                        {
                            break;
                        }
                    }
                    CloudFiles item = currentfolder.GetRootFile();
                    DriveToDriveTransFer(item, FileContentView.Groups[i]);
                    copyfile = null;
                    ToolPast.Enabled = false;
                    TS_paste.Enabled = false;
                }
                catch (Exception e1)
                {
                    MessageBox.Show(e1.Message);
                }
            }
            else
            {
                return;
            }
        }

        private void SerchTool_Click(object sender, EventArgs e)
        {
            SerchForm serchform = new SerchForm();
            var result = serchform.ShowDialog();
            if(result == System.Windows.Forms.DialogResult.OK)
            {
                SerchFile serchfile = new SerchFile(serchform.mode, serchform.filename);
                serchfile.Start();
                serchfile.OnComp += serchfile_OnComp;
            }
        }

        void serchfile_OnComp(object sender)
        {
            if(this.InvokeRequired)
                this.Invoke(SC, new object[] {sender});
            SerchFile serchfile = (SerchFile)sender;
           
            //FileContentView.View = View.LargeIcon;
            //// listview의 그룹보기를 true로 설정한다.
            //FileContentView.ShowGroups = true;
            //// 그룹의 이름을 현재 드라이브의 이름과 아이디로 설정한다.
            //ListViewGroup group = new ListViewGroup(folder.DriveName());
            //// 그룹의 태그로 현재 드라이브를 넣는다.
            //group.Tag = folder;
            //foreach (var item in cl)
            //{
            //    // 리스트뷰의 아이템 설정
            //    ListViewItem listitem = new ListViewItem(item.Item.FileName, index);
            //    listitem.Tag = item;
            //    listitem.SubItems.Add(item.Item.FileSize.ToString());
            //    listitem.SubItems.Add(item.Item.modifiedDate);
            //    group.Items.Add(listitem);
            //    FileContentView.Items.Add(listitem);
            //    index++;
            //}
            //FileContentView.Groups.Add(group);
            ////FileContentView.Columns.Add("파일명", 200, HorizontalAlignment.Left);
            ////FileContentView.Columns.Add("사이즈", 70, HorizontalAlignment.Left);
            ////FileContentView.Columns.Add("날짜", 100, HorizontalAlignment.Left);
            //// 리스트 뷰의 업데이트가 끝났음을 알리고 이제 컨트롤이 그려지기를 명령한다.
            //FileContentView.EndUpdate();
        }

        private void menu_Upload_Click(object sender, EventArgs e)
        {
            FileUpload();
        }

        private void 업로드ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FileUpload();
        }

        private void 파일업로드ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FileUpload();
        }

        private void 폴더업로드ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Upload();
        }

        private void 파일업로드ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileUpload();
        }

        private void 폴더업로드ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Upload();
        }

        private void 새폴더생성ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewFolderCreate();
        }

        private void MenuItem_rename_Click(object sender, EventArgs e)
        {
            rename();
        }
    }
}