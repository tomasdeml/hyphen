namespace Virtuoso.Miranda.Plugins.Forms.Configuration.Controls
{
    partial class ConfigurationPanel
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigurationPanel));
            this.CategoriesILIST = new System.Windows.Forms.ImageList(this.components);
            this.Toolbar = new System.Windows.Forms.ToolStrip();
            this.ViewsTDBTN = new System.Windows.Forms.ToolStripDropDownButton();
            this.detailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.largeIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CategoryItemsLVIEW = new System.Windows.Forms.ListView();
            this.CategoryItemNameCOLUMN = new System.Windows.Forms.ColumnHeader();
            this.CategoryItemDescriptionCOLUMN = new System.Windows.Forms.ColumnHeader();
            this.CategoryItemsILIST = new System.Windows.Forms.ImageList(this.components);
            this.SpecialItemsILIST = new System.Windows.Forms.ImageList(this.components);
            this.CategoriesLVIEW = new Virtuoso.Miranda.Plugins.Forms.Configuration.Controls.CategoryListView();
            this.NameCOLUMN = new System.Windows.Forms.ColumnHeader();
            this.panel1 = new Virtuoso.Miranda.Plugins.Forms.Controls.GradientPanel();
            this.CategoryDescriptionLABEL = new System.Windows.Forms.Label();
            this.CategoryTitleLABEL = new System.Windows.Forms.Label();
            this.CategoryIconPBOX = new System.Windows.Forms.PictureBox();
            this.Toolbar.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CategoryIconPBOX)).BeginInit();
            this.SuspendLayout();
            // 
            // CategoriesILIST
            // 
            this.CategoriesILIST.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.CategoriesILIST.ImageSize = new System.Drawing.Size(16, 16);
            this.CategoriesILIST.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // Toolbar
            // 
            this.Toolbar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Toolbar.AutoSize = false;
            this.Toolbar.Dock = System.Windows.Forms.DockStyle.None;
            this.Toolbar.Font = new System.Drawing.Font("Tahoma", 8F);
            this.Toolbar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.Toolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ViewsTDBTN});
            this.Toolbar.Location = new System.Drawing.Point(190, 62);
            this.Toolbar.Name = "Toolbar";
            this.Toolbar.Size = new System.Drawing.Size(510, 34);
            this.Toolbar.TabIndex = 2;
            this.Toolbar.Text = "toolStrip2";
            // 
            // ViewsTDBTN
            // 
            this.ViewsTDBTN.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.detailsToolStripMenuItem,
            this.largeIconsToolStripMenuItem,
            this.tilesToolStripMenuItem});
            this.ViewsTDBTN.Image = ((System.Drawing.Image)(resources.GetObject("ViewsTDBTN.Image")));
            this.ViewsTDBTN.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ViewsTDBTN.Name = "ViewsTDBTN";
            this.ViewsTDBTN.Size = new System.Drawing.Size(58, 31);
            this.ViewsTDBTN.Text = "View";
            this.ViewsTDBTN.DropDownOpening += new System.EventHandler(this.ViewsTDBTN_DropDownOpening);
            // 
            // detailsToolStripMenuItem
            // 
            this.detailsToolStripMenuItem.Name = "detailsToolStripMenuItem";
            this.detailsToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.detailsToolStripMenuItem.Tag = "Details";
            this.detailsToolStripMenuItem.Text = "Details";
            this.detailsToolStripMenuItem.Click += new System.EventHandler(this.ViewsItem_Click);
            // 
            // largeIconsToolStripMenuItem
            // 
            this.largeIconsToolStripMenuItem.Name = "largeIconsToolStripMenuItem";
            this.largeIconsToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.largeIconsToolStripMenuItem.Tag = "LargeIcon";
            this.largeIconsToolStripMenuItem.Text = "Large icons";
            this.largeIconsToolStripMenuItem.Click += new System.EventHandler(this.ViewsItem_Click);
            // 
            // tilesToolStripMenuItem
            // 
            this.tilesToolStripMenuItem.Name = "tilesToolStripMenuItem";
            this.tilesToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.tilesToolStripMenuItem.Tag = "Tile";
            this.tilesToolStripMenuItem.Text = "Tiles";
            this.tilesToolStripMenuItem.Click += new System.EventHandler(this.ViewsItem_Click);
            // 
            // CategoryItemsLVIEW
            // 
            this.CategoryItemsLVIEW.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.CategoryItemsLVIEW.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.CategoryItemsLVIEW.CausesValidation = false;
            this.CategoryItemsLVIEW.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.CategoryItemNameCOLUMN,
            this.CategoryItemDescriptionCOLUMN});
            this.CategoryItemsLVIEW.DataBindings.Add(new System.Windows.Forms.Binding("View", global::Virtuoso.Miranda.Plugins.Properties.Settings.Default, "ConfigurationPanel_CategoryItems_View", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.CategoryItemsLVIEW.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.CategoryItemsLVIEW.LargeImageList = this.CategoryItemsILIST;
            this.CategoryItemsLVIEW.Location = new System.Drawing.Point(184, 99);
            this.CategoryItemsLVIEW.MultiSelect = false;
            this.CategoryItemsLVIEW.Name = "CategoryItemsLVIEW";
            this.CategoryItemsLVIEW.ShowGroups = false;
            this.CategoryItemsLVIEW.Size = new System.Drawing.Size(515, 401);
            this.CategoryItemsLVIEW.SmallImageList = this.CategoryItemsILIST;
            this.CategoryItemsLVIEW.TabIndex = 4;
            this.CategoryItemsLVIEW.TileSize = new System.Drawing.Size(500, 60);
            this.CategoryItemsLVIEW.UseCompatibleStateImageBehavior = false;
            this.CategoryItemsLVIEW.View = global::Virtuoso.Miranda.Plugins.Properties.Settings.Default.ConfigurationPanel_CategoryItems_View;
            this.CategoryItemsLVIEW.ItemActivate += new System.EventHandler(this.CategoryItemsLVIEW_ItemActivate);
            // 
            // CategoryItemNameCOLUMN
            // 
            this.CategoryItemNameCOLUMN.Text = "Option";
            // 
            // CategoryItemDescriptionCOLUMN
            // 
            this.CategoryItemDescriptionCOLUMN.Text = "Description";
            // 
            // CategoryItemsILIST
            // 
            this.CategoryItemsILIST.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.CategoryItemsILIST.ImageSize = new System.Drawing.Size(32, 32);
            this.CategoryItemsILIST.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // SpecialItemsILIST
            // 
            this.SpecialItemsILIST.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("SpecialItemsILIST.ImageStream")));
            this.SpecialItemsILIST.TransparentColor = System.Drawing.Color.Transparent;
            this.SpecialItemsILIST.Images.SetKeyName(0, "Warning");
            // 
            // CategoriesLVIEW
            // 
            this.CategoriesLVIEW.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.CategoriesLVIEW.Alignment = System.Windows.Forms.ListViewAlignment.Left;
            this.CategoriesLVIEW.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.CategoriesLVIEW.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.NameCOLUMN});
            this.CategoriesLVIEW.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.CategoriesLVIEW.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.CategoriesLVIEW.HideSelection = false;
            this.CategoriesLVIEW.LargeImageList = this.CategoriesILIST;
            this.CategoriesLVIEW.Location = new System.Drawing.Point(0, 0);
            this.CategoriesLVIEW.Margin = new System.Windows.Forms.Padding(20, 15, 5, 5);
            this.CategoriesLVIEW.MultiSelect = false;
            this.CategoriesLVIEW.Name = "CategoriesLVIEW";
            this.CategoriesLVIEW.ShowGroups = false;
            this.CategoriesLVIEW.Size = new System.Drawing.Size(185, 500);
            this.CategoriesLVIEW.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.CategoriesLVIEW.TabIndex = 0;
            this.CategoriesLVIEW.TileSize = new System.Drawing.Size(180, 30);
            this.CategoriesLVIEW.UseCompatibleStateImageBehavior = false;
            this.CategoriesLVIEW.View = System.Windows.Forms.View.Tile;
            this.CategoriesLVIEW.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.CategoriesLVIEW_ItemSelectionChanged);
            // 
            // NameCOLUMN
            // 
            this.NameCOLUMN.Text = "Name";
            this.NameCOLUMN.Width = 170;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.CategoryDescriptionLABEL);
            this.panel1.Controls.Add(this.CategoryTitleLABEL);
            this.panel1.Controls.Add(this.CategoryIconPBOX);
            this.panel1.GradientColor = System.Drawing.SystemColors.Control;
            this.panel1.Location = new System.Drawing.Point(184, 0);
            this.panel1.Name = "panel1";
            this.panel1.Rotation = 0F;
            this.panel1.Size = new System.Drawing.Size(516, 59);
            this.panel1.TabIndex = 3;
            // 
            // CategoryDescriptionLABEL
            // 
            this.CategoryDescriptionLABEL.AutoSize = true;
            this.CategoryDescriptionLABEL.BackColor = System.Drawing.Color.Transparent;
            this.CategoryDescriptionLABEL.Location = new System.Drawing.Point(54, 28);
            this.CategoryDescriptionLABEL.Name = "CategoryDescriptionLABEL";
            this.CategoryDescriptionLABEL.Size = new System.Drawing.Size(60, 13);
            this.CategoryDescriptionLABEL.TabIndex = 5;
            this.CategoryDescriptionLABEL.Text = "Description";
            // 
            // CategoryTitleLABEL
            // 
            this.CategoryTitleLABEL.AutoSize = true;
            this.CategoryTitleLABEL.BackColor = System.Drawing.Color.Transparent;
            this.CategoryTitleLABEL.Font = new System.Drawing.Font("Franklin Gothic Medium", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.CategoryTitleLABEL.Location = new System.Drawing.Point(52, 6);
            this.CategoryTitleLABEL.Name = "CategoryTitleLABEL";
            this.CategoryTitleLABEL.Size = new System.Drawing.Size(66, 20);
            this.CategoryTitleLABEL.TabIndex = 1;
            this.CategoryTitleLABEL.Text = "Category";
            // 
            // CategoryIconPBOX
            // 
            this.CategoryIconPBOX.BackColor = System.Drawing.Color.Transparent;
            this.CategoryIconPBOX.Location = new System.Drawing.Point(13, 12);
            this.CategoryIconPBOX.Name = "CategoryIconPBOX";
            this.CategoryIconPBOX.Size = new System.Drawing.Size(32, 32);
            this.CategoryIconPBOX.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.CategoryIconPBOX.TabIndex = 0;
            this.CategoryIconPBOX.TabStop = false;
            // 
            // ConfigurationPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.CategoryItemsLVIEW);
            this.Controls.Add(this.CategoriesLVIEW);
            this.Controls.Add(this.Toolbar);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Tahoma", 8F);
            this.MinimumSize = new System.Drawing.Size(700, 500);
            this.Name = "ConfigurationPanel";
            this.Size = new System.Drawing.Size(700, 500);
            this.Toolbar.ResumeLayout(false);
            this.Toolbar.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CategoryIconPBOX)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private CategoryListView CategoriesLVIEW;
        private System.Windows.Forms.ImageList CategoriesILIST;
        private System.Windows.Forms.ColumnHeader NameCOLUMN;
        private System.Windows.Forms.ToolStrip Toolbar;
        private Virtuoso.Miranda.Plugins.Forms.Controls.GradientPanel panel1;
        private System.Windows.Forms.Label CategoryTitleLABEL;
        private System.Windows.Forms.PictureBox CategoryIconPBOX;
        private System.Windows.Forms.ListView CategoryItemsLVIEW;
        private System.Windows.Forms.ImageList CategoryItemsILIST;
        private System.Windows.Forms.ColumnHeader CategoryItemNameCOLUMN;
        private System.Windows.Forms.ColumnHeader CategoryItemDescriptionCOLUMN;
        private System.Windows.Forms.Label CategoryDescriptionLABEL;
        private System.Windows.Forms.ToolStripDropDownButton ViewsTDBTN;
        private System.Windows.Forms.ToolStripMenuItem largeIconsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem detailsToolStripMenuItem;
        private System.Windows.Forms.ImageList SpecialItemsILIST;
    }
}
