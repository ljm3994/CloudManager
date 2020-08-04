using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using googlecloud1.Files;
using googlecloud1.Folder;
namespace googlecloud1
{
    public enum NewFileOption { RENAME, CREATEFOLDER }
    public partial class CreateFolder : Form
    {
        DriveInfo drive;
        public CloudFiles file{get; set;}
        AllFolder folder;
        string parentid;
        NewFileOption option;
        public CreateFolder(DriveInfo drive, AllFolder folder, string parentid, NewFileOption option)
        {
            InitializeComponent();
            this.drive = drive;
            this.folder = folder;
            this.parentid = parentid;
            this.option = option;
        }
        public CreateFolder(CloudFiles file, NewFileOption option)
        {
            InitializeComponent();
            this.file = file;
            label1.Text = "파일 명 : ";
            tb_PATH.Text = file.Item.Path;
            this.option = option;
        }
        private async void btn_OK_Click(object sender, EventArgs e)
        {
            if(tb_NAME.Text != "")
            {
                if (NewFileOption.CREATEFOLDER == option)
                {
                    if (MessageBox.Show(tb_NAME.Text + "로 폴더를 생성하시겠습니까?", "폴더 생성", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {
                        try
                        {
                            file = await folder.CreateFolder(tb_NAME.Text, parentid);
                            this.DialogResult = System.Windows.Forms.DialogResult.OK;
                            Close();
                        }
                        catch (Exception er)
                        {
                            MessageBox.Show("에러 : " + er.Message);
                        }
                        
                    }
                    else
                    {
                        return;
                    }
                }
                else if (NewFileOption.RENAME == option)
                {
                    if (MessageBox.Show(tb_NAME.Text + "로 파일명을 변경하시겠습니까?", "파일명 변경", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {
                        try
                        {
                            await file.ChageFileName(tb_NAME.Text);
                            this.DialogResult = System.Windows.Forms.DialogResult.OK;
                            Close();
                        }
                        catch (Exception error)
                        {
                            MessageBox.Show("에러 : " + error.Message);
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("폴더명을 입력해야 합니다");
                return;
            }
        }

        private void btn_CANCEL_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

    }
}
