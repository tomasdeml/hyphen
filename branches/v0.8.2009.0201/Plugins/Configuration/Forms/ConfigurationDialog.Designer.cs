namespace Virtuoso.Miranda.Plugins.Configuration.Forms
{
    partial class ConfigurationDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigurationDialog));
            this.TopPanel = new System.Windows.Forms.Panel();
            this.RibbonPageSwitcher = new RibbonStyle.TabPageSwitcher();
            this.RibbonStrip = new RibbonStyle.TabStrip();
            this.BottomPanel = new System.Windows.Forms.Panel();
            this.CancelBTN = new System.Windows.Forms.Button();
            this.OkBTN = new System.Windows.Forms.Button();
            this.ControlPanel = new System.Windows.Forms.Panel();
            this.WelcomePanel = new Virtuoso.Miranda.Plugins.Configuration.Forms.Controls.CategoryItemHeader();
            this.TopPanel.SuspendLayout();
            this.BottomPanel.SuspendLayout();
            this.ControlPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // TopPanel
            // 
            this.TopPanel.Controls.Add(this.RibbonPageSwitcher);
            this.TopPanel.Controls.Add(this.RibbonStrip);
            this.TopPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TopPanel.Location = new System.Drawing.Point(0, 0);
            this.TopPanel.Name = "TopPanel";
            this.TopPanel.Size = new System.Drawing.Size(794, 127);
            this.TopPanel.TabIndex = 0;
            // 
            // RibbonPageSwitcher
            // 
            this.RibbonPageSwitcher.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this.RibbonPageSwitcher.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RibbonPageSwitcher.Location = new System.Drawing.Point(0, 26);
            this.RibbonPageSwitcher.Name = "RibbonPageSwitcher";
            this.RibbonPageSwitcher.SelectedTabStripPage = null;
            this.RibbonPageSwitcher.Size = new System.Drawing.Size(794, 101);
            this.RibbonPageSwitcher.TabIndex = 1;
            this.RibbonPageSwitcher.TabStrip = this.RibbonStrip;
            this.RibbonPageSwitcher.Text = "RibbonPageSwitcher";
            // 
            // RibbonStrip
            // 
            this.RibbonStrip.AutoSize = false;
            this.RibbonStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this.RibbonStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.RibbonStrip.Location = new System.Drawing.Point(0, 0);
            this.RibbonStrip.Name = "RibbonStrip";
            this.RibbonStrip.Padding = new System.Windows.Forms.Padding(60, 3, 30, 0);
            this.RibbonStrip.SelectedTab = null;
            this.RibbonStrip.ShowItemToolTips = false;
            this.RibbonStrip.Size = new System.Drawing.Size(794, 26);
            this.RibbonStrip.TabIndex = 0;
            this.RibbonStrip.TabOverlap = 0;
            this.RibbonStrip.Text = "tabStrip1";
            // 
            // BottomPanel
            // 
            this.BottomPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this.BottomPanel.Controls.Add(this.CancelBTN);
            this.BottomPanel.Controls.Add(this.OkBTN);
            this.BottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.BottomPanel.Location = new System.Drawing.Point(0, 529);
            this.BottomPanel.Name = "BottomPanel";
            this.BottomPanel.Size = new System.Drawing.Size(794, 39);
            this.BottomPanel.TabIndex = 1;
            // 
            // CancelBTN
            // 
            this.CancelBTN.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBTN.Location = new System.Drawing.Point(93, 7);
            this.CancelBTN.Name = "CancelBTN";
            this.CancelBTN.Size = new System.Drawing.Size(75, 23);
            this.CancelBTN.TabIndex = 0;
            this.CancelBTN.Text = "Cancel";
            this.CancelBTN.UseVisualStyleBackColor = true;
            this.CancelBTN.Click += new System.EventHandler(this.CancelBTN_Click);
            // 
            // OkBTN
            // 
            this.OkBTN.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OkBTN.Location = new System.Drawing.Point(12, 7);
            this.OkBTN.Name = "OkBTN";
            this.OkBTN.Size = new System.Drawing.Size(75, 23);
            this.OkBTN.TabIndex = 0;
            this.OkBTN.Text = "OK";
            this.OkBTN.UseVisualStyleBackColor = true;
            this.OkBTN.Click += new System.EventHandler(this.OkBTN_Click);
            // 
            // ControlPanel
            // 
            this.ControlPanel.AutoScroll = true;
            this.ControlPanel.Controls.Add(this.WelcomePanel);
            this.ControlPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ControlPanel.Location = new System.Drawing.Point(0, 127);
            this.ControlPanel.Name = "ControlPanel";
            this.ControlPanel.Size = new System.Drawing.Size(794, 402);
            this.ControlPanel.TabIndex = 2;
            // 
            // WelcomePanel
            // 
            this.WelcomePanel.BackColor = System.Drawing.Color.Transparent;
            this.WelcomePanel.Color = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this.WelcomePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.WelcomePanel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.WelcomePanel.HeaderFont = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.WelcomePanel.HeaderText = " Select a tab and pick a section to configure...";
            this.WelcomePanel.Image = ((System.Drawing.Image)(resources.GetObject("WelcomePanel.Image")));
            this.WelcomePanel.Location = new System.Drawing.Point(0, 0);
            this.WelcomePanel.MinimumSize = new System.Drawing.Size(300, 40);
            this.WelcomePanel.Name = "WelcomePanel";
            this.WelcomePanel.Size = new System.Drawing.Size(794, 100);
            this.WelcomePanel.TabIndex = 1;
            // 
            // ConfigurationDialog
            // 
            this.AcceptButton = this.OkBTN;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.CancelBTN;
            this.ClientSize = new System.Drawing.Size(794, 568);
            this.Controls.Add(this.ControlPanel);
            this.Controls.Add(this.TopPanel);
            this.Controls.Add(this.BottomPanel);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "ConfigurationDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Hyphen Configuration Center";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConfigurationDialog_FormClosing);
            this.TopPanel.ResumeLayout(false);
            this.BottomPanel.ResumeLayout(false);
            this.ControlPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel TopPanel;
        private RibbonStyle.TabPageSwitcher RibbonPageSwitcher;
        private RibbonStyle.TabStrip RibbonStrip;
        private System.Windows.Forms.Button CancelBTN;
        private System.Windows.Forms.Button OkBTN;
        private System.Windows.Forms.Panel ControlPanel;
        private System.Windows.Forms.Panel BottomPanel;
        private Virtuoso.Miranda.Plugins.Configuration.Forms.Controls.CategoryItemHeader WelcomePanel;
    }
}