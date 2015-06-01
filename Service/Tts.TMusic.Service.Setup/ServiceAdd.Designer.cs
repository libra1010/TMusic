namespace Tts.TMusic.Service.Setup
{
    partial class ServiceAdd
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
            this.lsbMsg = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // lsbMsg
            // 
            this.lsbMsg.FormattingEnabled = true;
            this.lsbMsg.ItemHeight = 12;
            this.lsbMsg.Location = new System.Drawing.Point(13, 13);
            this.lsbMsg.Name = "lsbMsg";
            this.lsbMsg.Size = new System.Drawing.Size(373, 340);
            this.lsbMsg.TabIndex = 0;
            // 
            // ServiceAdd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(398, 369);
            this.Controls.Add(this.lsbMsg);
            this.Name = "ServiceAdd";
            this.Text = "ServiceAdd";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lsbMsg;
    }
}