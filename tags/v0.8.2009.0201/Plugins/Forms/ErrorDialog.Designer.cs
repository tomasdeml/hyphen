namespace Virtuoso.Miranda.Plugins.Forms
{
    internal partial class ErrorDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param eventName="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorDialog));
            this.MessageLABEL = new System.Windows.Forms.Label();
            this.DetailsTBOX = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.OkBTN = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.Panel1 = new Virtuoso.Miranda.Plugins.Configuration.Forms.Controls.CategoryItemHeader();
            this.CancelBTN = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SendReportLBTN = new System.Windows.Forms.LinkLabel();
            this.Panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // MessageLABEL
            // 
            resources.ApplyResources(this.MessageLABEL, "MessageLABEL");
            this.MessageLABEL.Name = "MessageLABEL";
            // 
            // DetailsTBOX
            // 
            this.DetailsTBOX.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.DetailsTBOX, "DetailsTBOX");
            this.DetailsTBOX.Name = "DetailsTBOX";
            this.DetailsTBOX.ReadOnly = true;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // OkBTN
            // 
            resources.ApplyResources(this.OkBTN, "OkBTN");
            this.OkBTN.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OkBTN.Name = "OkBTN";
            this.OkBTN.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Name = "label3";
            // 
            // Panel1
            // 
            this.Panel1.BackColor = System.Drawing.Color.Transparent;
            this.Panel1.Color = System.Drawing.SystemColors.ActiveCaption;
            this.Panel1.Controls.Add(this.label3);
            resources.ApplyResources(this.Panel1, "Panel1");
            this.Panel1.HeaderFont = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Panel1.Image = ((System.Drawing.Image)(resources.GetObject("Panel1.Image")));
            this.Panel1.MinimumSize = new System.Drawing.Size(300, 40);
            this.Panel1.Name = "Panel1";
            // 
            // CancelBTN
            // 
            this.CancelBTN.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.CancelBTN, "CancelBTN");
            this.CancelBTN.Name = "CancelBTN";
            this.CancelBTN.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // SendReportLBTN
            // 
            resources.ApplyResources(this.SendReportLBTN, "SendReportLBTN");
            this.SendReportLBTN.Name = "SendReportLBTN";
            this.SendReportLBTN.TabStop = true;
            this.SendReportLBTN.UseCompatibleTextRendering = true;
            this.SendReportLBTN.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.SendReportLBTN_LinkClicked);
            // 
            // ErrorDialog
            // 
            this.AcceptButton = this.OkBTN;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.CancelBTN;
            this.Controls.Add(this.SendReportLBTN);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.CancelBTN);
            this.Controls.Add(this.MessageLABEL);
            this.Controls.Add(this.DetailsTBOX);
            this.Controls.Add(this.OkBTN);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Panel1);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ErrorDialog";
            this.Shown += new System.EventHandler(this.PluginErrorDialog_Shown);
            this.Panel1.ResumeLayout(false);
            this.Panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label MessageLABEL;
        private System.Windows.Forms.TextBox DetailsTBOX;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button OkBTN;
        private System.Windows.Forms.Label label3;
        private Virtuoso.Miranda.Plugins.Configuration.Forms.Controls.CategoryItemHeader Panel1;
        private System.Windows.Forms.Button CancelBTN;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.LinkLabel SendReportLBTN;
    }
}