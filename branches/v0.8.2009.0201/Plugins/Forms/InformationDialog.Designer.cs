namespace Virtuoso.Miranda.Plugins.Forms
{
    partial class InformationDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InformationDialog));
            this.InformationLABEL = new System.Windows.Forms.Label();
            this.OkBTN = new System.Windows.Forms.Button();
            this.DialogHeader = new Virtuoso.Miranda.Plugins.Configuration.Forms.Controls.CategoryItemHeader();
            this.CaptionLABEL = new System.Windows.Forms.Label();
            this.BackgroundPBOX = new System.Windows.Forms.PictureBox();
            this.DialogHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BackgroundPBOX)).BeginInit();
            this.SuspendLayout();
            // 
            // InformationLABEL
            // 
            this.InformationLABEL.BackColor = System.Drawing.Color.Transparent;
            this.InformationLABEL.ForeColor = System.Drawing.SystemColors.WindowText;
            this.InformationLABEL.Location = new System.Drawing.Point(9, 56);
            this.InformationLABEL.Name = "InformationLABEL";
            this.InformationLABEL.Size = new System.Drawing.Size(430, 155);
            this.InformationLABEL.TabIndex = 2;
            this.InformationLABEL.Text = "Information";
            // 
            // OkBTN
            // 
            this.OkBTN.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.OkBTN.Location = new System.Drawing.Point(12, 220);
            this.OkBTN.Name = "OkBTN";
            this.OkBTN.Size = new System.Drawing.Size(75, 23);
            this.OkBTN.TabIndex = 0;
            this.OkBTN.Text = "OK";
            this.OkBTN.UseVisualStyleBackColor = true;
            // 
            // DialogHeader
            // 
            this.DialogHeader.BackColor = System.Drawing.Color.Transparent;
            this.DialogHeader.Color = System.Drawing.SystemColors.ActiveCaption;
            this.DialogHeader.Controls.Add(this.CaptionLABEL);
            this.DialogHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.DialogHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.DialogHeader.HeaderFont = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.DialogHeader.HeaderText = "Caption";
            this.DialogHeader.Image = global::Virtuoso.Miranda.Plugins.Properties.Resources.Icon_232_32x32;
            this.DialogHeader.Location = new System.Drawing.Point(0, 0);
            this.DialogHeader.MinimumSize = new System.Drawing.Size(300, 40);
            this.DialogHeader.Name = "DialogHeader";
            this.DialogHeader.Size = new System.Drawing.Size(451, 53);
            this.DialogHeader.TabIndex = 1;
            // 
            // CaptionLABEL
            // 
            this.CaptionLABEL.AutoSize = true;
            this.CaptionLABEL.BackColor = System.Drawing.Color.Transparent;
            this.CaptionLABEL.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.CaptionLABEL.Location = new System.Drawing.Point(12, 23);
            this.CaptionLABEL.Name = "CaptionLABEL";
            this.CaptionLABEL.Size = new System.Drawing.Size(59, 13);
            this.CaptionLABEL.TabIndex = 0;
            this.CaptionLABEL.Text = "(caption)";
            // 
            // BackgroundPBOX
            // 
            this.BackgroundPBOX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BackgroundPBOX.Image = ((System.Drawing.Image)(resources.GetObject("BackgroundPBOX.Image")));
            this.BackgroundPBOX.Location = new System.Drawing.Point(330, 127);
            this.BackgroundPBOX.Name = "BackgroundPBOX";
            this.BackgroundPBOX.Size = new System.Drawing.Size(120, 129);
            this.BackgroundPBOX.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.BackgroundPBOX.TabIndex = 3;
            this.BackgroundPBOX.TabStop = false;
            this.BackgroundPBOX.Visible = false;
            // 
            // InformationDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(451, 255);
            this.Controls.Add(this.BackgroundPBOX);
            this.Controls.Add(this.OkBTN);
            this.Controls.Add(this.InformationLABEL);
            this.Controls.Add(this.DialogHeader);
            this.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InformationDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Information";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.InformationDialog_Shown);
            this.DialogHeader.ResumeLayout(false);
            this.DialogHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BackgroundPBOX)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Virtuoso.Miranda.Plugins.Configuration.Forms.Controls.CategoryItemHeader DialogHeader;
        private System.Windows.Forms.Label InformationLABEL;
        private System.Windows.Forms.Label CaptionLABEL;
        private System.Windows.Forms.Button OkBTN;
        private System.Windows.Forms.PictureBox BackgroundPBOX;
    }
}