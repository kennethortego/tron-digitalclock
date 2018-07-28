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
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Source Code Pro Medium", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(784, 411);
            this.label1.TabIndex = 0;
            this.label1.Text = "[keypress]";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TronDigitalClockForm
            // 
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(784, 411);
            this.Controls.Add(this.label1);
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
        private System.Windows.Forms.Label label1;
    }
}

