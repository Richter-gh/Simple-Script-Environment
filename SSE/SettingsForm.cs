using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SSE.Settings;
namespace SSE
{

    public partial class SettingsForm : Form
    {
        private MySettings settings;

        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsOk_Click(object sender, EventArgs e)
        {
            settings.minimizedStart = MinimizedStartCheckbox.Checked;
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
            settings = MySettings.Load();
        }
    }

}
