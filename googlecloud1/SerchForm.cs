using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace googlecloud1
{
    public enum SerchMode { ALL, FOLDER, GOOGLE, ONEDRIVE, DROPBOX }
    public partial class SerchForm : Form
    {
        public string filename { get; set; }
        public SerchMode mode { get; set; }
        public SerchForm()
        {
            InitializeComponent();
            mode = SerchMode.ALL;
        }
        private void btn_ok_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show(tb_filename + "로 검색하시겠습니까?", "검색", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                filename = tb_filename.Text;
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                Close();
            }
            else
            {
                return;
            }
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox obj = (ComboBox)sender;
            switch(obj.SelectedIndex)
            {
                case 0:
                    mode = SerchMode.ALL;
                    break;
                case 1:
                    mode = SerchMode.FOLDER;
                    break;
                case 2:
                    mode = SerchMode.GOOGLE;
                    break;
                case 3:
                    mode = SerchMode.ONEDRIVE;
                    break;
                case 4:
                    mode = SerchMode.DROPBOX;
                    break;
            }
        }
    }
}
