namespace googlecloud1
{
    partial class DriveInfoForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.TB_info = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.PB_quaot = new System.Windows.Forms.ProgressBar();
            this.LB_quatoper = new System.Windows.Forms.Label();
            this.LB_use = new System.Windows.Forms.Label();
            this.drive = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "드라이브 정보";
            // 
            // TB_info
            // 
            this.TB_info.Location = new System.Drawing.Point(12, 42);
            this.TB_info.Multiline = true;
            this.TB_info.Name = "TB_info";
            this.TB_info.ReadOnly = true;
            this.TB_info.Size = new System.Drawing.Size(540, 495);
            this.TB_info.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI Symbol", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(7, 557);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(111, 21);
            this.label3.TabIndex = 3;
            this.label3.Text = "드라이브 용량";
            // 
            // PB_quaot
            // 
            this.PB_quaot.Location = new System.Drawing.Point(328, 557);
            this.PB_quaot.Name = "PB_quaot";
            this.PB_quaot.Size = new System.Drawing.Size(224, 23);
            this.PB_quaot.TabIndex = 5;
            // 
            // LB_quatoper
            // 
            this.LB_quatoper.AutoSize = true;
            this.LB_quatoper.Font = new System.Drawing.Font("Segoe UI Symbol", 12F, System.Drawing.FontStyle.Bold);
            this.LB_quatoper.Location = new System.Drawing.Point(288, 557);
            this.LB_quatoper.Name = "LB_quatoper";
            this.LB_quatoper.Size = new System.Drawing.Size(0, 21);
            this.LB_quatoper.TabIndex = 6;
            // 
            // LB_use
            // 
            this.LB_use.AutoSize = true;
            this.LB_use.Font = new System.Drawing.Font("Segoe UI Symbol", 12F, System.Drawing.FontStyle.Bold);
            this.LB_use.Location = new System.Drawing.Point(156, 557);
            this.LB_use.Name = "LB_use";
            this.LB_use.Size = new System.Drawing.Size(0, 21);
            this.LB_use.TabIndex = 7;
            // 
            // drive
            // 
            this.drive.FormattingEnabled = true;
            this.drive.Location = new System.Drawing.Point(248, 16);
            this.drive.Name = "drive";
            this.drive.Size = new System.Drawing.Size(304, 20);
            this.drive.TabIndex = 8;
            this.drive.SelectedIndexChanged += new System.EventHandler(this.drive_SelectedIndexChanged);
            // 
            // DriveInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(567, 622);
            this.Controls.Add(this.drive);
            this.Controls.Add(this.LB_use);
            this.Controls.Add(this.LB_quatoper);
            this.Controls.Add(this.PB_quaot);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TB_info);
            this.Controls.Add(this.label1);
            this.Name = "DriveInfoForm";
            this.Text = "DriveInfoForm";
            this.Load += new System.EventHandler(this.DriveInfoForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TB_info;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ProgressBar PB_quaot;
        private System.Windows.Forms.Label LB_quatoper;
        private System.Windows.Forms.Label LB_use;
        private System.Windows.Forms.ComboBox drive;
    }
}