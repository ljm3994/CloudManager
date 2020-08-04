using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Nemiro.OAuth.Clients;
using Nemiro.OAuth;
using googlecloud1.UriCollect;
using googlecloud1.login;
using googlecloud1.Folder;
namespace googlecloud1
{
    public partial class Setting : Form
    {
        delegate void SafeLoadComplete(object sender);
        Authentication login;
        Authentication.TokenResult token;
        SafeLoadComplete loadcompelte;
        public static List<DriveInfo> driveinfo = new List<DriveInfo>();
        public List<DriveInfo> currentdriveinfo { get; set; }
        public bool logout { get; set; }
        FileDataStore datastore;
        public Setting()
        {
            InitializeComponent();
            logout = false;
            loadcompelte = new SafeLoadComplete(login_complete);
            currentdriveinfo = new List<DriveInfo>();
            if(driveinfo.Count != 0)
            {
                foreach (var item in driveinfo)
                {
                    listBox1.Items.Add(item.token.Drive + " - 이름 : " + item.DisplayName + ", ID : " + item.UserID);
                }
            }
            datastore = new login.FileDataStore(@"C:\Test");
        }
        private void ShowProgress()
        {
            this.loadingimg.Image = global::googlecloud1.Properties.Resources.loadingimg2;
            textBox1.ReadOnly = true;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            this.listBox1.Visible = false;
            this.loadingimg.Visible = true;
        }
        private void HideProgress()
        {
            this.listBox1.Visible = true;
            this.loadingimg.Visible = false;
            this.loadingimg.Image = null;
            textBox1.ReadOnly = false;
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
        }
        void login_complete(object sender)
        {
            if (listBox1.InvokeRequired)
            {
                listBox1.Invoke(loadcompelte, new object[] { sender });
                return;
            }
            token = login.token;
            if(token != null)
            {
                listBox1.Items.Add(login.driveinfo.token.Drive +" - 이름 : " + login.driveinfo.DisplayName + ", ID : " + login.driveinfo.UserID);
                login.driveinfo.username = this.textBox1.Text;
                currentdriveinfo.Add(login.driveinfo);
                HideProgress();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text != null)
                {
                    LoginProcess(Client.GoogleClientID, Client.GoogleClientSecret, UriCollection.GoogleAuthorizationUrl, UriCollection.GoogleinstalledAppRedirectUri, UriCollection.GoogleTokenUri,
                    textBox1.Text +"Google", textBox1.Text, @"C:\Test", ScopeCollect.GoogleScope1, LoginOption.GoogleDrive);
                }
            }
            catch
            {
                MessageBox.Show("인터넷 연결이 실패했습니다. 인터넷 연결을 확인해주세요");
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text != null)
                {
                    LoginProcess(Client.OneDriveClientID, Client.GoogleClientSecret, UriCollection.OneDriveAuthorizationUri,
                    UriCollection.OneDriveinstalledAppRedirectUri, UriCollection.OneDriveTokenUri, textBox1.Text + "OneDrive",
                    textBox1.Text, @"C:\Test", ScopeCollect.OneDriveScope1, LoginOption.OneDrive);
                }
            }
            catch
            {
                MessageBox.Show("인터넷 연결이 실패했습니다. 인터넷 연결을 확인해주세요");
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text != null)
                {
                    LoginProcess(Client.DropBoxClientID, Client.DropBoxClientSecret, UriCollection.DropBoxAuthorizationUri, null,
                        UriCollection.DropBoxTokenUri, textBox1.Text + "DropBox", textBox1.Text, @"C:\Test", null, LoginOption.DropBox);
                }
            }
            catch
            {
                MessageBox.Show("인터넷 연결이 실패했습니다. 인터넷 연결을 확인해주세요");
            }
        }
        private async void LoginProcess(string clientid, string clientsecret, string AuthorizationUri, string RedirectUri, string TokenUri, string savenaem, string username, string path, string[] scope, LoginOption option)
        {
            FileDataStore datastore = new FileDataStore(path);
            Authentication.UserNameToken token = await Authentication.LoadToken(datastore, savenaem, CancellationToken.None);
            string code = null;
            if (token == null)
            {
                code = await loginform.GetAuthenticationToken(clientid, scope, username, AuthorizationUri, RedirectUri, option);
            }
            if ((token != null && code == null) || (token == null && code != null))
            {
                ShowProgress();
                login = new Authentication(clientid, clientsecret, username, path, scope, option, code);
                login.complete += login_complete;
                login.start();
            }
        }

        private void logout_btn_Click(object sender, EventArgs e)
        {
            if(listBox1.SelectedItems.Count != 0)
            {
                List<DriveInfo> dr = driveinfo.ToList();
                foreach (var item in currentdriveinfo)
                {
                    dr.Add(item);
                }
                for (int i = 0; i < driveinfo.Count; i++ )
                {
                    if(listBox1.SelectedItem.ToString().Contains(dr[i].UserID))
                    {
                        listBox1.Items.RemoveAt(listBox1.SelectedIndex);
                        datastore.DeleteAsync<Authentication.TokenResult>(dr[i].username);
                        driveinfo.RemoveAt(i);
                        MessageBox.Show("성공적으로 로그아웃 하였습니다");
                        logout = true;
                        break;
                    }
                }
            }
            
        }

        private void DriveInfo_btn_Click(object sender, EventArgs e)
        {
            List<DriveInfo> dr = driveinfo.ToList();
            foreach (var item in currentdriveinfo)
            {
                dr.Add(item);
            }
            if (listBox1.SelectedItems.Count != 0)
            {
                for (int i = 0; i < dr.Count; i++)
                {
                    if (listBox1.SelectedItem.ToString().Contains(dr[i].UserID))
                    {
                        DriveInfoForm driveinfoform = new DriveInfoForm(dr[i]);
                        driveinfoform.ShowDialog();
                    }
                }
            }
        }

        private void DriveRemove_btn_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            foreach (var item in currentdriveinfo)
            {
                driveinfo.Add(item);
            }
            Close();
        }

        private void Setting_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(this.DialogResult == System.Windows.Forms.DialogResult.Cancel)
            {
                if(MessageBox.Show("정말로 로그인 작업을 취소 하시겠습니까?", "로그인 작업 취소", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                {
                    e.Cancel = true;
                    MessageBox.Show("로그인을 완료하려면 확인 버튼을 눌러주시기 바랍니다");
                }
                else
                {
                    foreach (var item in currentdriveinfo)
                    {
                        datastore.DeleteAsync<Authentication.TokenResult>(item.username);
                    }
                    currentdriveinfo.Clear();
                    this.DialogResult = System.Windows.Forms.DialogResult.Abort;
                }
            }
        }
    }
}
