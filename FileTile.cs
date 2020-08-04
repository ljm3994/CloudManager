using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Drive.v2;
namespace googlecloud1
{
    public partial class FileTile : UserControl
    {
         private string sourceitem;
        private string connection;
        private bool _selected;

        public FileTile(string name, string connection)
        {
            InitializeComponent();
            this.sourceitem = name;
            this.connection = connection;
            this.SourceItemChanged();
        }

        public string Connection
        {
            get { return connection; }
            set
            {
                if (value == connection)
                    return;
                connection = value;
                SourceItemChanged();
            }
        }
        public string sourceitem1
        {
            get { return sourceitem; }
            set
            {
                if (value == sourceitem)
                    return;

                sourceitem = value;
                SourceItemChanged();
            }
        }

        private void SourceItemChanged()
        {
            if (null == sourceitem) return;

            this.filename.Text = sourceitem;

            LoadThumbnail();
        }

        private void LoadThumbnail()
        {
            var thumbnail = connection;
            if (null != thumbnail)
            {
                string thumbnailUri = thumbnail;
                pictureBox1.ImageLocation = thumbnailUri;
            }
        }

        private void Control_Click(object sender, EventArgs e)
        {
            OnClick(EventArgs.Empty);
        }

        private void Control_DoubleClick(object sender, EventArgs e)
        {
            OnDoubleClick(EventArgs.Empty);
        }

        public bool Selected
        {
            get { return _selected; }
            set
            {
                if (value != _selected)
                {
                    _selected = value;
                    this.filename.Font = _selected ? new Font(this.filename.Font, FontStyle.Bold) : new Font(this.filename.Font, FontStyle.Regular);
                }
            }
        }
    }
}
