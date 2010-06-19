using Virtuoso.Miranda.Plugins.Forms.Controls;
namespace Virtuoso.Miranda.Plugins.Configuration.Forms.Controls
{
    partial class CategoryItemHeader
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ImagePBOX = new System.Windows.Forms.PictureBox();
            this.DescriptionLABEL = new System.Windows.Forms.Label();
            this.panel1 = new Virtuoso.Miranda.Plugins.Forms.Controls.GradientPanel();
            ((System.ComponentModel.ISupportInitialize)(this.ImagePBOX)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ImagePBOX
            // 
            this.ImagePBOX.Image = global::Virtuoso.Miranda.Plugins.Properties.Resources.Icon_256_32x32;
            this.ImagePBOX.Location = new System.Drawing.Point(10, 4);
            this.ImagePBOX.Name = "ImagePBOX";
            this.ImagePBOX.Size = new System.Drawing.Size(32, 32);
            this.ImagePBOX.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.ImagePBOX.TabIndex = 0;
            this.ImagePBOX.TabStop = false;
            // 
            // DescriptionLABEL
            // 
            this.DescriptionLABEL.AutoSize = true;
            this.DescriptionLABEL.Font = new System.Drawing.Font("Tahoma", 8, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.DescriptionLABEL.ForeColor = System.Drawing.Color.Black;
            this.DescriptionLABEL.Location = new System.Drawing.Point(50, 12);
            this.DescriptionLABEL.Name = "DescriptionLABEL";
            this.DescriptionLABEL.Size = new System.Drawing.Size(0, 16);
            this.DescriptionLABEL.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ImagePBOX);
            this.panel1.Controls.Add(this.DescriptionLABEL);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.GradientColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Rotation = -90F;
            this.panel1.Size = new System.Drawing.Size(765, 40);
            this.panel1.TabIndex = 3;
            // 
            // CategoryItemHeader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.MinimumSize = new System.Drawing.Size(300, 40);
            this.Name = "CategoryItemHeader";
            this.Size = new System.Drawing.Size(765, 40);
            ((System.ComponentModel.ISupportInitialize)(this.ImagePBOX)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox ImagePBOX;
        private System.Windows.Forms.Label DescriptionLABEL;
        private GradientPanel panel1;
    }
}
