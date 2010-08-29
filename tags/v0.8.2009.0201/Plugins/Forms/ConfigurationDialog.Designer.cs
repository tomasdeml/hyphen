using Virtuoso.Miranda.Plugins.Forms.Controls;
namespace Virtuoso.Miranda.Plugins.Forms
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
            this.OkBTN = new System.Windows.Forms.Button();
            this.CancelBTN = new System.Windows.Forms.Button();
            this.MainPanel = new Virtuoso.Miranda.Plugins.Forms.Controls.ConfigurationPanel();
            this.HideExpertOptionsCHBOX = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // OkBTN
            // 
            this.OkBTN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OkBTN.Location = new System.Drawing.Point(556, 524);
            this.OkBTN.Name = "OkBTN";
            this.OkBTN.Size = new System.Drawing.Size(75, 23);
            this.OkBTN.TabIndex = 1;
            this.OkBTN.Text = "Save";
            this.OkBTN.UseVisualStyleBackColor = true;
            this.OkBTN.Click += new System.EventHandler(this.OkBTN_Click);
            // 
            // CancelBTN
            // 
            this.CancelBTN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelBTN.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBTN.Location = new System.Drawing.Point(637, 524);
            this.CancelBTN.Name = "CancelBTN";
            this.CancelBTN.Size = new System.Drawing.Size(75, 23);
            this.CancelBTN.TabIndex = 1;
            this.CancelBTN.Text = "Cancel";
            this.CancelBTN.UseVisualStyleBackColor = true;
            this.CancelBTN.Click += new System.EventHandler(this.CancelBTN_Click);
            // 
            // MainPanel
            // 
            this.MainPanel.Font = new System.Drawing.Font("Tahoma", 8F);
            this.MainPanel.HideExpertOptions = false;
            this.MainPanel.Location = new System.Drawing.Point(14, 13);
            this.MainPanel.MinimumSize = new System.Drawing.Size(700, 500);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.Size = new System.Drawing.Size(700, 500);
            this.MainPanel.TabIndex = 0;
            // 
            // HideExpertOptionsCHBOX
            // 
            this.HideExpertOptionsCHBOX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.HideExpertOptionsCHBOX.AutoSize = true;
            this.HideExpertOptionsCHBOX.Checked = global::Virtuoso.Miranda.Plugins.Properties.Settings.Default.ConfigurationDialog_HideExpertOptions_Checked;
            this.HideExpertOptionsCHBOX.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Virtuoso.Miranda.Plugins.Properties.Settings.Default, "ConfigurationDialog_HideExpertOptions_Checked", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.HideExpertOptionsCHBOX.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.HideExpertOptionsCHBOX.Location = new System.Drawing.Point(12, 530);
            this.HideExpertOptionsCHBOX.Name = "HideExpertOptionsCHBOX";
            this.HideExpertOptionsCHBOX.Size = new System.Drawing.Size(137, 17);
            this.HideExpertOptionsCHBOX.TabIndex = 2;
            this.HideExpertOptionsCHBOX.Text = "Hide expert options";
            this.HideExpertOptionsCHBOX.UseVisualStyleBackColor = true;
            this.HideExpertOptionsCHBOX.CheckedChanged += new System.EventHandler(this.HideExpertOptionsCHBOX_CheckedChanged);
            // 
            // ConfigurationDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(724, 559);
            this.Controls.Add(this.HideExpertOptionsCHBOX);
            this.Controls.Add(this.CancelBTN);
            this.Controls.Add(this.OkBTN);
            this.Controls.Add(this.MainPanel);
            this.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ConfigurationDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuration";
            this.Shown += new System.EventHandler(this.ConfigurationDialog_Shown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConfigurationDialog_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OkBTN;
        private System.Windows.Forms.Button CancelBTN;
        private ConfigurationPanel MainPanel;
        private System.Windows.Forms.CheckBox HideExpertOptionsCHBOX;



    }
}