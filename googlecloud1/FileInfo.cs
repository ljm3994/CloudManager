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
using googlecloud1.FileUpDown;
namespace googlecloud1
{
    partial class FileInfoForm : Form
    {
        public CloudFiles file { get; set; }
        public FileInfoForm(CloudFiles file)
        {
            InitializeComponent();
            this.file = file;
        }

        private void FileInfoForm_Load(object sender, EventArgs e)
        {
            CB_ShowSelect.SelectedItem = 0;
            ShowLittleDate();
        }
        private void ShowLittleDate()
        {
            if(file != null)
            {
                TB_FileInfo.Text = "";
                TB_FileInfo.Text = "파일 명 : " + file.Item.FileName + Environment.NewLine;
                TB_FileInfo.Text += "파일 ID : " + file.Item.FileID + Environment.NewLine;
                TB_FileInfo.Text += "파일 크기 : " + ToByte(file.Item.FileSize) + Environment.NewLine;
                TB_FileInfo.Text += "파일 확장자 : " + file.Item.Extention + Environment.NewLine;
                TB_FileInfo.Text += "드라이브 내 경로 : " + file.Item.Path + Environment.NewLine;
                TB_FileInfo.Text += "수정한 날짜 : " + file.Item.modifiedDate + Environment.NewLine;
            }
        }
        private void ShowMetaDate()
        {
            if (file != null)
            {
                TB_FileInfo.Text = "";
                Dictionary<string, object> fileinfo = file.Item.Items.ToDictionary();
                foreach (var item in fileinfo)
                {
                    TB_FileInfo.Text += item.Key + " : " + item.Value + Environment.NewLine;
                }
            }
        }
        private string ToByte(long bytesize)
        {
            string[] SizeFormates = { "bytes", "KB", "MB", "GB", "TB" };

            if (bytesize < 0) { return "-"; }

            int i = 0;
            decimal dvalue = (decimal)bytesize;

            while(Math.Round(dvalue / 1024) >= 1)
            {
                dvalue /= 1024;
                i++;
            }
            return string.Format("{0:n1} {1}", dvalue, SizeFormates[i]);
        }
        private void CB_ShowSelect_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox combo = (ComboBox)sender;
            switch(combo.SelectedIndex)
            {
                case 0:
                    ShowLittleDate();
                    break;
                case 1:
                    ShowMetaDate();
                    break;
            }
        }
    }
}
