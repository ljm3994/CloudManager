using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using googlecloud1;
namespace googlecloud1
{
    public partial class DriveInfoForm : Form
    {
        List<DriveInfo> driveinfos;
        DriveInfo driveinfo;
        public DriveInfoForm(List<DriveInfo> driveinfo)
        {
            InitializeComponent();
            this.driveinfos = driveinfo;
        }
        public DriveInfoForm(DriveInfo driveinfo)
        {
            InitializeComponent();
            this.driveinfo = driveinfo;
        }
        private void DriveInfoForm_Load(object sender, EventArgs e)
        {
            if(driveinfos == null)
            {
                drive.Text = "드라이브 : " + driveinfo.token.Drive + " ID : " + driveinfo.UserID;
                ShowDriveInfo(driveinfo);
            }
            else
            {
                foreach (var item in driveinfos)
                {
                    drive.Items.Add("드라이브 : " + item.token.Drive + " ID : " + item.UserID);
                }
            }
        }
        private void ShowDriveInfo(DriveInfo driveinfo)
        {
            TB_info.Text = "사용자 이름 : " + driveinfo.DisplayName + Environment.NewLine;
            TB_info.Text += "사용자 ID : " + driveinfo.UserID + Environment.NewLine;
            TB_info.Text += "드라이브 ID : " + driveinfo.DriveID + Environment.NewLine;
            if (driveinfo.Status != null)
            {
               TB_info.Text += "드라이브 상태 : " + driveinfo.Status + Environment.NewLine;
            }
            TB_info.Text += "드라이브 타입 : " + driveinfo.DriveType + Environment.NewLine;
            TB_info.Text += "전체 용량 : " + ToByte(driveinfo.TotalSize) + Environment.NewLine;
            TB_info.Text += "사용 용량 : " + ToByte(driveinfo.UseSize) + Environment.NewLine;
            TB_info.Text += "남은 용량 : " + ToByte(driveinfo.EmptySize) + Environment.NewLine;
            if (driveinfo.token.Drive != "DropBox")
            {
               TB_info.Text += "휴지통 용량 : " + ToByte(driveinfo.DeleteSize) + Environment.NewLine;
            }
            ProgresBarChange(driveinfo);
        }
        private void ProgresBarChange(DriveInfo driveinfo)
        {
            float per = ((float)driveinfo.UseSize / driveinfo.TotalSize) * 100;
            PB_quaot.Value = (int)per;
            LB_use.Text = string.Format("{0} 사용함", ToByte(driveinfo.UseSize));
            LB_quatoper.Text = string.Format("{0}%", (int)per);
        }
        private string ToByte(long bytesize)
        {
            string[] SizeFormates = { "bytes", "KB", "MB", "GB", "TB" };

            if (bytesize < 0) { return "-"; }

            int i = 0;
            decimal dvalue = (decimal)bytesize;

            while (Math.Round(dvalue / 1024) >= 1)
            {
                dvalue /= 1024;
                i++;
            }
            return string.Format("{0:n1} {1}", dvalue, SizeFormates[i]);
        }

        private void drive_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox combo = (ComboBox)sender;
            foreach (var item in driveinfos)
	        {
                if (combo.SelectedItem.ToString().Contains("드라이브 : " + item.token.Drive + " ID : " + item.UserID))
                {
                    ShowDriveInfo(item);
                }
	        }
        }
    }
}
