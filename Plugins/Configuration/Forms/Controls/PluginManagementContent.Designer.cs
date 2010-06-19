using Virtuoso.Miranda.Plugins.Configuration.Forms.Controls;

namespace Virtuoso.Miranda.Plugins.Configuration.Forms.Controls
{
    partial class PluginManagementContent
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
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Enabled and running", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Disabled by the user", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Crashed", System.Windows.Forms.HorizontalAlignment.Left);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PluginManagementContent));
            this.EnablePluginBTN = new System.Windows.Forms.Button();
            this.DisablePluginBTN = new System.Windows.Forms.Button();
            this.StatusColumn = new System.Windows.Forms.ColumnHeader();
            this.PluginsLVIEW = new System.Windows.Forms.ListView();
            this.NameColumn = new System.Windows.Forms.ColumnHeader();
            this.AuthorColumn = new System.Windows.Forms.ColumnHeader();
            this.VersionColumn = new System.Windows.Forms.ColumnHeader();
            this.panel1 = new Virtuoso.Miranda.Plugins.Configuration.Forms.Controls.CategoryItemHeader();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.DescriptionLABEL = new System.Windows.Forms.Label();
            this.panel2 = new Virtuoso.Miranda.Plugins.Configuration.Forms.Controls.CategoryItemSection();
            this.HomePageLBTN = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // EnablePluginBTN
            // 
            this.EnablePluginBTN.AutoSize = true;
            this.EnablePluginBTN.Enabled = false;
            this.EnablePluginBTN.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.EnablePluginBTN.Location = new System.Drawing.Point(678, 46);
            this.EnablePluginBTN.Name = "EnablePluginBTN";
            this.EnablePluginBTN.Size = new System.Drawing.Size(95, 23);
            this.EnablePluginBTN.TabIndex = 11;
            this.EnablePluginBTN.Text = "Enable plugin";
            this.EnablePluginBTN.UseVisualStyleBackColor = false;
            this.EnablePluginBTN.Click += new System.EventHandler(this.EnablePluginBTN_Click);
            // 
            // DisablePluginBTN
            // 
            this.DisablePluginBTN.AutoSize = true;
            this.DisablePluginBTN.Enabled = false;
            this.DisablePluginBTN.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.DisablePluginBTN.Location = new System.Drawing.Point(678, 75);
            this.DisablePluginBTN.Name = "DisablePluginBTN";
            this.DisablePluginBTN.Size = new System.Drawing.Size(95, 23);
            this.DisablePluginBTN.TabIndex = 10;
            this.DisablePluginBTN.Text = "Disable plugin";
            this.DisablePluginBTN.UseVisualStyleBackColor = false;
            this.DisablePluginBTN.Click += new System.EventHandler(this.DisablePluginBTN_Click);
            // 
            // StatusColumn
            // 
            this.StatusColumn.Text = "Status";
            // 
            // PluginsLVIEW
            // 
            this.PluginsLVIEW.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.NameColumn,
            this.AuthorColumn,
            this.VersionColumn,
            this.StatusColumn});
            this.PluginsLVIEW.FullRowSelect = true;
            listViewGroup1.Header = "Enabled and running";
            listViewGroup1.Name = "EnabledGroup";
            listViewGroup2.Header = "Disabled by the user";
            listViewGroup2.Name = "DisabledByUserGroup";
            listViewGroup3.Header = "Crashed";
            listViewGroup3.Name = "DisabledByCrashGroup";
            this.PluginsLVIEW.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3});
            this.PluginsLVIEW.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.PluginsLVIEW.HideSelection = false;
            this.PluginsLVIEW.Location = new System.Drawing.Point(12, 46);
            this.PluginsLVIEW.MultiSelect = false;
            this.PluginsLVIEW.Name = "PluginsLVIEW";
            this.PluginsLVIEW.ShowItemToolTips = true;
            this.PluginsLVIEW.Size = new System.Drawing.Size(650, 257);
            this.PluginsLVIEW.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.PluginsLVIEW.TabIndex = 9;
            this.PluginsLVIEW.UseCompatibleStateImageBehavior = false;
            this.PluginsLVIEW.View = System.Windows.Forms.View.Details;
            this.PluginsLVIEW.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.PluginsLVIEW_ItemSelectionChanged);
            // 
            // NameColumn
            // 
            this.NameColumn.Text = "Title";
            // 
            // AuthorColumn
            // 
            this.AuthorColumn.Text = "Author";
            // 
            // VersionColumn
            // 
            this.VersionColumn.Text = "Version";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.panel1.HeaderFont = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.panel1.HeaderText = "The following list contains installed Hyphen plugins.";
            this.panel1.Image = ((System.Drawing.Image)(resources.GetObject("panel1.Image")));
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.MinimumSize = new System.Drawing.Size(300, 40);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(790, 40);
            this.panel1.TabIndex = 16;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureBox1.Location = new System.Drawing.Point(625, 170);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(212, 235);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 15;
            this.pictureBox1.TabStop = false;
            // 
            // DescriptionLABEL
            // 
            this.DescriptionLABEL.AutoEllipsis = true;
            this.DescriptionLABEL.BackColor = System.Drawing.Color.Transparent;
            this.DescriptionLABEL.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.DescriptionLABEL.Location = new System.Drawing.Point(20, 335);
            this.DescriptionLABEL.Name = "DescriptionLABEL";
            this.DescriptionLABEL.Size = new System.Drawing.Size(586, 53);
            this.DescriptionLABEL.TabIndex = 12;
            this.DescriptionLABEL.Text = "(...)";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Color = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.panel2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.panel2.ForeColor = System.Drawing.Color.Black;
            this.panel2.Location = new System.Drawing.Point(12, 309);
            this.panel2.MinimumSize = new System.Drawing.Size(300, 20);
            this.panel2.Name = "panel2";
            this.panel2.SectionName = "Description";
            this.panel2.Size = new System.Drawing.Size(543, 20);
            this.panel2.TabIndex = 17;
            // 
            // HomePageLBTN
            // 
            this.HomePageLBTN.AutoSize = true;
            this.HomePageLBTN.Enabled = false;
            this.HomePageLBTN.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.HomePageLBTN.Location = new System.Drawing.Point(561, 313);
            this.HomePageLBTN.Name = "HomePageLBTN";
            this.HomePageLBTN.Size = new System.Drawing.Size(58, 13);
            this.HomePageLBTN.TabIndex = 18;
            this.HomePageLBTN.TabStop = true;
            this.HomePageLBTN.Text = "Homepage";
            // 
            // PluginManagementContent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.HomePageLBTN);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.EnablePluginBTN);
            this.Controls.Add(this.DisablePluginBTN);
            this.Controls.Add(this.PluginsLVIEW);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.DescriptionLABEL);
            this.Name = "PluginManagementContent";
            this.Size = new System.Drawing.Size(790, 398);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button EnablePluginBTN;
        private System.Windows.Forms.Button DisablePluginBTN;
        private System.Windows.Forms.ColumnHeader StatusColumn;
        private System.Windows.Forms.ListView PluginsLVIEW;
        private System.Windows.Forms.ColumnHeader NameColumn;
        private System.Windows.Forms.ColumnHeader AuthorColumn;
        private System.Windows.Forms.ColumnHeader VersionColumn;
        private Virtuoso.Miranda.Plugins.Configuration.Forms.Controls.CategoryItemHeader panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label DescriptionLABEL;
        private CategoryItemSection panel2;
        private System.Windows.Forms.LinkLabel HomePageLBTN;
    }
}
