namespace googlecloud1
{
    partial class main
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

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.flowLayoutPanel_filecontent = new System.Windows.Forms.FlowLayoutPanel();
            this.menuStrip2 = new System.Windows.Forms.MenuStrip();
            this.열기ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.로그인ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.파일업로드ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.downflowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.uploadflowpanel = new System.Windows.Forms.FlowLayoutPanel();
            this.원드라이브로그인ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Location = new System.Drawing.Point(0, 24);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1062, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(853, 0);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(209, 24);
            this.progressBar1.TabIndex = 2;
            this.progressBar1.Value = 100;
            // 
            // flowLayoutPanel_filecontent
            // 
            this.flowLayoutPanel_filecontent.AutoScroll = true;
            this.flowLayoutPanel_filecontent.Location = new System.Drawing.Point(0, 51);
            this.flowLayoutPanel_filecontent.Name = "flowLayoutPanel_filecontent";
            this.flowLayoutPanel_filecontent.Size = new System.Drawing.Size(1062, 521);
            this.flowLayoutPanel_filecontent.TabIndex = 3;
            // 
            // menuStrip2
            // 
            this.menuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.열기ToolStripMenuItem});
            this.menuStrip2.Location = new System.Drawing.Point(0, 0);
            this.menuStrip2.Name = "menuStrip2";
            this.menuStrip2.Size = new System.Drawing.Size(1062, 24);
            this.menuStrip2.TabIndex = 4;
            this.menuStrip2.Text = "menuStrip2";
            // 
            // 열기ToolStripMenuItem
            // 
            this.열기ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.로그인ToolStripMenuItem,
            this.파일업로드ToolStripMenuItem,
            this.원드라이브로그인ToolStripMenuItem});
            this.열기ToolStripMenuItem.Name = "열기ToolStripMenuItem";
            this.열기ToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.열기ToolStripMenuItem.Text = "열기";
            // 
            // 로그인ToolStripMenuItem
            // 
            this.로그인ToolStripMenuItem.Name = "로그인ToolStripMenuItem";
            this.로그인ToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.로그인ToolStripMenuItem.Text = "로그인";
            // 
            // 파일업로드ToolStripMenuItem
            // 
            this.파일업로드ToolStripMenuItem.Name = "파일업로드ToolStripMenuItem";
            this.파일업로드ToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.파일업로드ToolStripMenuItem.Text = "파일 업로드";
            this.파일업로드ToolStripMenuItem.Click += new System.EventHandler(this.파일업로드ToolStripMenuItem_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 27);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1062, 21);
            this.flowLayoutPanel1.TabIndex = 5;
            // 
            // downflowPanel
            // 
            this.downflowPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.downflowPanel.Location = new System.Drawing.Point(0, 628);
            this.downflowPanel.Name = "downflowPanel";
            this.downflowPanel.Size = new System.Drawing.Size(1062, 37);
            this.downflowPanel.TabIndex = 8;
            // 
            // uploadflowpanel
            // 
            this.uploadflowpanel.Location = new System.Drawing.Point(0, 578);
            this.uploadflowpanel.Name = "uploadflowpanel";
            this.uploadflowpanel.Size = new System.Drawing.Size(1062, 44);
            this.uploadflowpanel.TabIndex = 0;
            // 
            // 원드라이브로그인ToolStripMenuItem
            // 
            this.원드라이브로그인ToolStripMenuItem.Name = "원드라이브로그인ToolStripMenuItem";
            this.원드라이브로그인ToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.원드라이브로그인ToolStripMenuItem.Text = "원드라이브 로그인";
            this.원드라이브로그인ToolStripMenuItem.Click += new System.EventHandler(this.원드라이브로그인ToolStripMenuItem_Click);
            // 
            // main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1062, 663);
            this.Controls.Add(this.uploadflowpanel);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.flowLayoutPanel_filecontent);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.menuStrip2);
            this.Controls.Add(this.downflowPanel);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "main";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.main_Load);
            this.menuStrip2.ResumeLayout(false);
            this.menuStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel_filecontent;
        private DriveObjectBrowser dob;
        private System.Windows.Forms.MenuStrip menuStrip2;
        private System.Windows.Forms.ToolStripMenuItem 열기ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 로그인ToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel downflowPanel;
        private System.Windows.Forms.ToolStripMenuItem 파일업로드ToolStripMenuItem;
        private System.Windows.Forms.FlowLayoutPanel uploadflowpanel;
        private System.Windows.Forms.ToolStripMenuItem 원드라이브로그인ToolStripMenuItem;
    }
}

