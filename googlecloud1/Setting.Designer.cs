namespace googlecloud1
{
    partial class Setting
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Setting));
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.loadingimg = new System.Windows.Forms.PictureBox();
            this.logout_btn = new System.Windows.Forms.Button();
            this.DriveInfo_btn = new System.Windows.Forms.Button();
            this.DriveRemove_btn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.loadingimg)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 10F);
            this.label1.Location = new System.Drawing.Point(61, 85);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "로그인";
            // 
            // button1
            // 
            this.button1.BackgroundImage = global::googlecloud1.Properties.Resources.google;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Location = new System.Drawing.Point(128, 72);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(90, 40);
            this.button1.TabIndex = 1;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button2.BackgroundImage")));
            this.button2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button2.Location = new System.Drawing.Point(320, 72);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(90, 40);
            this.button2.TabIndex = 3;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(2, 176);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(609, 376);
            this.listBox1.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 158);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "로그인 상태";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(128, 35);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 21);
            this.textBox1.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("굴림", 10F);
            this.label4.Location = new System.Drawing.Point(42, 37);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 14);
            this.label4.TabIndex = 7;
            this.label4.Text = "유저 이름";
            // 
            // button3
            // 
            this.button3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button3.BackgroundImage")));
            this.button3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button3.Location = new System.Drawing.Point(224, 72);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(90, 40);
            this.button3.TabIndex = 8;
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // loadingimg
            // 
            this.loadingimg.BackColor = System.Drawing.Color.White;
            this.loadingimg.Location = new System.Drawing.Point(2, 176);
            this.loadingimg.Name = "loadingimg";
            this.loadingimg.Size = new System.Drawing.Size(609, 376);
            this.loadingimg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.loadingimg.TabIndex = 9;
            this.loadingimg.TabStop = false;
            this.loadingimg.Visible = false;
            // 
            // logout_btn
            // 
            this.logout_btn.Location = new System.Drawing.Point(203, 579);
            this.logout_btn.Name = "logout_btn";
            this.logout_btn.Size = new System.Drawing.Size(137, 40);
            this.logout_btn.TabIndex = 10;
            this.logout_btn.Text = "로그아웃";
            this.logout_btn.UseVisualStyleBackColor = true;
            this.logout_btn.Click += new System.EventHandler(this.logout_btn_Click);
            // 
            // DriveInfo_btn
            // 
            this.DriveInfo_btn.Location = new System.Drawing.Point(371, 579);
            this.DriveInfo_btn.Name = "DriveInfo_btn";
            this.DriveInfo_btn.Size = new System.Drawing.Size(138, 40);
            this.DriveInfo_btn.TabIndex = 11;
            this.DriveInfo_btn.Text = "드라이브 정보보기";
            this.DriveInfo_btn.UseVisualStyleBackColor = true;
            this.DriveInfo_btn.Click += new System.EventHandler(this.DriveInfo_btn_Click);
            // 
            // DriveRemove_btn
            // 
            this.DriveRemove_btn.Location = new System.Drawing.Point(55, 579);
            this.DriveRemove_btn.Name = "DriveRemove_btn";
            this.DriveRemove_btn.Size = new System.Drawing.Size(122, 40);
            this.DriveRemove_btn.TabIndex = 12;
            this.DriveRemove_btn.Text = "확인";
            this.DriveRemove_btn.UseVisualStyleBackColor = true;
            this.DriveRemove_btn.Click += new System.EventHandler(this.DriveRemove_btn_Click);
            // 
            // Setting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(615, 645);
            this.Controls.Add(this.DriveRemove_btn);
            this.Controls.Add(this.DriveInfo_btn);
            this.Controls.Add(this.logout_btn);
            this.Controls.Add(this.loadingimg);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Name = "Setting";
            this.Text = "Setting";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Setting_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.loadingimg)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.PictureBox loadingimg;
        private System.Windows.Forms.Button logout_btn;
        private System.Windows.Forms.Button DriveInfo_btn;
        private System.Windows.Forms.Button DriveRemove_btn;
    }
}