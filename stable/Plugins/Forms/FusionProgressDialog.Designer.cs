namespace Virtuoso.Miranda.Plugins.Forms
{
    partial class FusionProgressDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FusionProgressDialog));
            this.FusionWorker = new System.ComponentModel.BackgroundWorker();
            this.BackgroundPBOX = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.BackgroundPBOX)).BeginInit();
            this.SuspendLayout();
            // 
            // FusionWorker
            // 
            this.FusionWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.FusionWorker_DoWork);
            this.FusionWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.FusionWorker_RunWorkerCompleted);
            // 
            // BackgroundPBOX
            // 
            this.BackgroundPBOX.Image = ((System.Drawing.Image)(resources.GetObject("BackgroundPBOX.Image")));
            this.BackgroundPBOX.Location = new System.Drawing.Point(0, 0);
            this.BackgroundPBOX.Name = "BackgroundPBOX";
            this.BackgroundPBOX.Size = new System.Drawing.Size(200, 100);
            this.BackgroundPBOX.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.BackgroundPBOX.TabIndex = 2;
            this.BackgroundPBOX.TabStop = false;
            // 
            // FusionProgressDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ClientSize = new System.Drawing.Size(200, 100);
            this.ControlBox = false;
            this.Controls.Add(this.BackgroundPBOX);
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            this.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FusionProgressDialog";
            this.Opacity = 0.75;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Loading plugins";
            this.TransparencyKey = System.Drawing.Color.LightSteelBlue;
            ((System.ComponentModel.ISupportInitialize)(this.BackgroundPBOX)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.ComponentModel.BackgroundWorker FusionWorker;
        private System.Windows.Forms.PictureBox BackgroundPBOX;
    }
}