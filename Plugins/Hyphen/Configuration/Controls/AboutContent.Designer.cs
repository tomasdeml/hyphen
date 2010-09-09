using Virtuoso.Miranda.Plugins.Configuration.Forms.Controls;
namespace Virtuoso.Hyphen.Configuration.Controls
{
    partial class AboutContent
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutContent));
            this.panel1 = new Virtuoso.Miranda.Plugins.Configuration.Forms.Controls.CategoryItemHeader();
            this.panel2 = new Virtuoso.Miranda.Plugins.Configuration.Forms.Controls.CategoryItemSection();
            this.VersionLABEL = new System.Windows.Forms.Label();
            this.HomepageLINK = new System.Windows.Forms.LinkLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.categoryItemSection1 = new Virtuoso.Miranda.Plugins.Configuration.Forms.Controls.CategoryItemSection();
            this.label1 = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.panel1.HeaderFont = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.panel1.HeaderText = "About Hyphen";
            this.panel1.Image = global::Virtuoso.Miranda.Plugins.Properties.Resources.Icon_232_32x32;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.MinimumSize = new System.Drawing.Size(300, 40);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(792, 40);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Color = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.panel2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.panel2.ForeColor = System.Drawing.Color.Black;
            this.panel2.Location = new System.Drawing.Point(10, 46);
            this.panel2.MinimumSize = new System.Drawing.Size(300, 20);
            this.panel2.Name = "panel2";
            this.panel2.SectionName = "Hyphen";
            this.panel2.Size = new System.Drawing.Size(765, 20);
            this.panel2.TabIndex = 1;
            // 
            // VersionLABEL
            // 
            this.VersionLABEL.AutoSize = true;
            this.VersionLABEL.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.VersionLABEL.Location = new System.Drawing.Point(127, 72);
            this.VersionLABEL.Name = "VersionLABEL";
            this.VersionLABEL.Size = new System.Drawing.Size(51, 13);
            this.VersionLABEL.TabIndex = 2;
            this.VersionLABEL.Text = "v0.0.0.0";
            // 
            // HomepageLINK
            // 
            this.HomepageLINK.AutoSize = true;
            this.HomepageLINK.Location = new System.Drawing.Point(130, 85);
            this.HomepageLINK.Name = "HomepageLINK";
            this.HomepageLINK.Size = new System.Drawing.Size(121, 13);
            this.HomepageLINK.TabIndex = 3;
            this.HomepageLINK.TabStop = true;
            this.HomepageLINK.Text = "© (Assembly copyright)";
            this.HomepageLINK.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.HomepageLINK_LinkClicked);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(21, 72);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(100, 50);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // categoryItemSection1
            // 
            this.categoryItemSection1.BackColor = System.Drawing.Color.Transparent;
            this.categoryItemSection1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.categoryItemSection1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.categoryItemSection1.ForeColor = System.Drawing.Color.Black;
            this.categoryItemSection1.Location = new System.Drawing.Point(10, 149);
            this.categoryItemSection1.MinimumSize = new System.Drawing.Size(300, 20);
            this.categoryItemSection1.Name = "categoryItemSection1";
            this.categoryItemSection1.SectionName = "Components";
            this.categoryItemSection1.Size = new System.Drawing.Size(765, 20);
            this.categoryItemSection1.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.Location = new System.Drawing.Point(18, 172);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(239, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Hyphen uses these 3rd party assemblies:";
            // 
            // listBox1
            // 
            this.listBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Items.AddRange(new object[] {
            "RibbonPanel, © Juan Pablo G.C."});
            this.listBox1.Location = new System.Drawing.Point(33, 188);
            this.listBox1.Name = "listBox1";
            this.listBox1.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.listBox1.Size = new System.Drawing.Size(224, 104);
            this.listBox1.Sorted = true;
            this.listBox1.TabIndex = 6;
            // 
            // AboutContent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.HomepageLINK);
            this.Controls.Add(this.VersionLABEL);
            this.Controls.Add(this.categoryItemSection1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "AboutContent";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CategoryItemHeader panel1;
        private CategoryItemSection panel2;
        private System.Windows.Forms.Label VersionLABEL;
        private System.Windows.Forms.LinkLabel HomepageLINK;
        private System.Windows.Forms.PictureBox pictureBox1;
        private CategoryItemSection categoryItemSection1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBox1;
    }
}
