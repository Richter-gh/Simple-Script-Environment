using System;
using System.Windows.Forms;
using SSE.Settings;

namespace SSE
{
    public partial class SettingsForm : Form
    {
        public MySettings settings;

        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsOk_Click(object sender, EventArgs e)
        {
            settings.minimizedStart = MinimizedStartCheckbox.Checked;
            settings.runOnWinStart = AutostartCheckBox.Checked;
            settings.Save();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public MySettings GetSettings()
        {
            return settings;
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            if(settings==null)
                settings = MySettings.Load();
        }
    }
}