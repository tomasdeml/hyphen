using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Virtuoso.Miranda.Plugins.Forms.Controls;
using Virtuoso.Miranda.Plugins.Infrastructure;
using Virtuoso.Miranda.Plugins.Resources;
using Virtuoso.Miranda.Plugins.Properties;

namespace Virtuoso.Miranda.Plugins.Forms
{
    public sealed partial class ConfigurationDialog : SingletonDialog
    {
        #region Fields

        private bool Ok;
        private IConfigurablePlugin ConfigurableEntity;

        #endregion

        #region .ctors

        private ConfigurationDialog(IConfigurablePlugin configurableEntity) : base(configurableEntity.Name)
        {            
            InitializeComponent();

            this.MainPanel.HideExpertOptions = HideExpertOptionsCHBOX.Checked;
            this.ConfigurableEntity = configurableEntity;
            this.Text = TextResources.UI_Caption_Configure_ + configurableEntity.Name;
            
            configurableEntity.PopulateConfigurationPanel(MainPanel);
        }

        public static void Present(IConfigurablePlugin configurableEntity, bool modal)
        {
            Present(configurableEntity, null, modal);
        }

        public static void Present(IConfigurablePlugin configurableEntity, string path, bool modal)
        {
            if (configurableEntity == null) 
                throw new ArgumentNullException("configurableEntity");

            ConfigurationDialog singleton = ConfigurationDialog.GetSingleton<ConfigurationDialog>(false, configurableEntity.Name) ?? 
                new ConfigurationDialog(configurableEntity);

            if (singleton.MainPanel.Categories.Count == 0)
                MessageBox.Show(TextResources.MsgBox_Text_NoOptionsAvailable, TextResources.MsgBox_Caption_NoOptionsAvailable, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                singleton.MainPanel.SetPath(path);
                singleton.ShowSingleton(modal);
            }
        }

        #endregion

        #region Methods

        public static string CreatePath(string categoryName, string itemName)
        {
            return ConfigurationPanel.CreatePath(categoryName, itemName);
        }

        #endregion

        #region UI Handlers

        private void ConfigurationDialog_Shown(object sender, EventArgs e)
        {
            MainPanel.Initialize();
        }

        private void OkBTN_Click(object sender, EventArgs e)
        {
            Ok = true;
            Close();
        }

        private void CancelBTN_Click(object sender, EventArgs e)
        {
            Ok = false;
            Close();
        }

        private void ConfigurationDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Ok)
            {
                ConfigurableEntity.Configuration.Save();
                Settings.Default.Save();
            }
            else
            {
                ConfigurableEntity.ReloadConfiguration();
                Settings.Default.Reload();
            }
        }

        private void HideExpertOptionsCHBOX_CheckedChanged(object sender, EventArgs e)
        {
            MainPanel.HideExpertOptions = HideExpertOptionsCHBOX.Checked;
        }

        #endregion      
    }
}