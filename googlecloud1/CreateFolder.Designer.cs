namespace googlecloud1
{
    partial class CreateFolder
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
            this.tb_NAME = new System.Windows.Forms.TextBox();
            this.btn_OK = new System.Windows.Forms.Button();
            this.btn_CANCEL = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_PATH = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 72);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "폴더 명 : ";
            // 
            // tb_NAME
            // 
            this.tb_NAME.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tb_NAME.Location = new System.Drawing.Point(85, 70);
            this.tb_NAME.Name = "tb_NAME";
            this.tb_NAME.Size = new System.Drawing.Size(522, 21);
            this.tb_NAME.TabIndex = 1;
            // 
            // btn_OK
            // 
            this.btn_OK.Location = new System.Drawing.Point(40, 111);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(75, 23);
            this.btn_OK.TabIndex = 2;
            this.btn_OK.Text = "확인";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // btn_CANCEL
            // 
            this.btn_CANCEL.Location = new System.Drawing.Point(532, 111);
            this.btn_CANCEL.Name = "btn_CANCEL";
            this.btn_CANCEL.Size = new System.Drawing.Size(75, 23);
            this.btn_CANCEL.TabIndex = 3;
            this.btn_CANCEL.Text = "취소";
            this.btn_CANCEL.UseVisualStyleBackColor = true;
            this.btn_CANCEL.Click += new System.EventHandler(this.btn_CANCEL_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "폴더 경로 :";
            // 
            // tb_PATH
            // 
            this.tb_PATH.Location = new System.Drawing.Point(85, 32);
            this.tb_PATH.Name = "tb_PATH";
            this.tb_PATH.ReadOnly = true;
            this.tb_PATH.Size = new System.Drawing.Size(522, 21);
            this.tb_PATH.TabIndex = 6;
            // 
            // CreateFolder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(635, 146);
            this.ControlBox = false;
            this.Controls.Add(this.tb_PATH);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btn_CANCEL);
            this.Controls.Add(this.btn_OK);
            this.Controls.Add(this.tb_NAME);
            this.Controls.Add(this.label1);
            this.Name = "CreateFolder";
            this.Text = "CreateFolder";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tb_NAME;
        private System.Windows.Forms.Button btn_OK;
        private System.Windows.Forms.Button btn_CANCEL;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_PATH;
    }
}