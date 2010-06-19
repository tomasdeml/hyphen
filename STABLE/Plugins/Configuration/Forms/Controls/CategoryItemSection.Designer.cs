using Virtuoso.Miranda.Plugins.Forms.Controls;
namespace Virtuoso.Miranda.Plugins.Configuration.Forms.Controls
{
    partial class CategoryItemSection
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
            this.panel1 = new Virtuoso.Miranda.Plugins.Forms.Controls.GradientPanel();
            this.SectionLABEL = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.SectionLABEL);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.GradientColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Rotation = 90F;
            this.panel1.Size = new System.Drawing.Size(765, 20);
            this.panel1.TabIndex = 1;
            // 
            // SectionLABEL
            // 
            this.SectionLABEL.AutoSize = true;
            this.SectionLABEL.ForeColor = System.Drawing.SystemColors.MenuText;
            this.SectionLABEL.Location = new System.Drawing.Point(8, 4);
            this.SectionLABEL.Name = "SectionLABEL";
            this.SectionLABEL.Size = new System.Drawing.Size(0, 13);
            this.SectionLABEL.TabIndex = 0;
            // 
            // CategoryItemSection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ForeColor = System.Drawing.Color.Black;
            this.MinimumSize = new System.Drawing.Size(300, 20);
            this.Name = "CategoryItemSection";
            this.Size = new System.Drawing.Size(765, 20);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label SectionLABEL;
        private GradientPanel panel1;
    }
}
