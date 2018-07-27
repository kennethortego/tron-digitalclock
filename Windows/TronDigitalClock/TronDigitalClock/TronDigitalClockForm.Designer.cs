namespace TronDigitalClock
{
    partial class TronDigitalClockForm
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
            this.components = new System.ComponentModel.Container();
            this.drawDigitTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // TronDigitalClockForm
            // 
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(784, 411);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "TronDigitalClockForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Load += new System.EventHandler(this.TronDigitalClockForm_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TronDigitalClockForm_KeyPress);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TronDigitalClockForm_MouseClick);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TronDigitalClockForm_MouseMove);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer drawDigitTimer;
    }
}

