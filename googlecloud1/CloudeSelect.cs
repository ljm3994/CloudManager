using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using googlecloud1.login;
using googlecloud1.Folder;
namespace googlecloud1
{
    public partial class CloudeSelect : Form
    {
        List<AllFolder> folder;
        public AllFolder drive { get; set; }
        public CloudeSelect(List<AllFolder> folder)
        {
            InitializeComponent();
            this.folder = folder;
        }

        private void CloudeSelect_Load(object sender, EventArgs e)
        {
            if (folder != null)
            {
                foreach (var item in folder)
                {
                    RadioButton radio = new RadioButton();
                    radio.Text = item.driveinfo.token.Drive + " Id : " + item.driveinfo.DriveID;
                    radio.Tag = item;
                    flowLayoutPanel1.Controls.Add(radio);
                }
            }
        }

        private void Ok_btn_Click(object sender, EventArgs e)
        {
            foreach (RadioButton item in flowLayoutPanel1.Controls)
            {
                if(item.Checked)
                {
                    drive = (AllFolder)item.Tag;
                }
            }
            CloseWindow();
        }
        private void CloseWindow()
        {
            const int interval = 100;
            var t = new System.Threading.Timer(new System.Threading.TimerCallback((state) =>
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.BeginInvoke(new MethodInvoker(() => this.Close()));
            }), null, interval, System.Threading.Timeout.Infinite);
        }
        private void Cancel_btn_Click(object sender, EventArgs e)
        {
            CloseCancleWindow();
        }
        private void CloseCancleWindow()
        {
            const int interval = 100;
            var t = new System.Threading.Timer(new System.Threading.TimerCallback((state) =>
            {
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                this.BeginInvoke(new MethodInvoker(() => this.Close()));
            }), null, interval, System.Threading.Timeout.Infinite);
        }
    }
}
