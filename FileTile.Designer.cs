namespace googlecloud1
{
    partial class FileTile
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.filename = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.SeaGreen;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(182, 116);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.Control_Click);
            this.pictureBox1.DoubleClick += new System.EventHandler(this.Control_DoubleClick);
            // 
            // filename
            // 
            this.filename.AutoEllipsis = true;
            this.filename.BackColor = System.Drawing.Color.Transparent;
            this.filename.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.filename.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.filename.ForeColor = System.Drawing.Color.White;
            this.filename.Location = new System.Drawing.Point(0, 85);
            this.filename.Margin = new System.Windows.Forms.Padding(0);
            this.filename.Name = "filename";
            this.filename.Padding = new System.Windows.Forms.Padding(0, 0, 8, 6);
            this.filename.Size = new System.Drawing.Size(182, 31);
            this.filename.TabIndex = 0;
            this.filename.Text = "Documents (Google)";
            this.filename.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.filename.Click += new System.EventHandler(this.Control_Click);
            this.filename.DoubleClick += new System.EventHandler(this.Control_DoubleClick);
            // 
            // FileTile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.MediumSeaGreen;
            this.Controls.Add(this.filename);
            this.Controls.Add(this.pictureBox1);
            this.Name = "FileTile";
            this.Size = new System.Drawing.Size(182, 116);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label filename;
    }
}
