namespace googlecloud1
{
    partial class FileInfoForm
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
            this.CB_ShowSelect = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.TB_FileInfo = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // CB_ShowSelect
            // 
            this.CB_ShowSelect.FormattingEnabled = true;
            this.CB_ShowSelect.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.CB_ShowSelect.Items.AddRange(new object[] {
            "간략 정보 보기",
            "메타데이터 보기"});
            this.CB_ShowSelect.Location = new System.Drawing.Point(490, 6);
            this.CB_ShowSelect.Name = "CB_ShowSelect";
            this.CB_ShowSelect.Size = new System.Drawing.Size(121, 20);
            this.CB_ShowSelect.TabIndex = 5;
            this.CB_ShowSelect.Text = "간략 정보 보기";
            this.CB_ShowSelect.SelectedValueChanged += new System.EventHandler(this.CB_ShowSelect_SelectedValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "파일 정보";
            // 
            // TB_FileInfo
            // 
            this.TB_FileInfo.Location = new System.Drawing.Point(7, 32);
            this.TB_FileInfo.Multiline = true;
            this.TB_FileInfo.Name = "TB_FileInfo";
            this.TB_FileInfo.ReadOnly = true;
            this.TB_FileInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TB_FileInfo.Size = new System.Drawing.Size(604, 622);
            this.TB_FileInfo.TabIndex = 3;
            // 
            // FileInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(619, 666);
            this.Controls.Add(this.CB_ShowSelect);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TB_FileInfo);
            this.Name = "FileInfoForm";
            this.Text = "FileInfo";
            this.Load += new System.EventHandler(this.FileInfoForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox CB_ShowSelect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TB_FileInfo;

    }
}