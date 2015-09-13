using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SSE
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private SSE.Settings.MySettings Settings;
        private void SettingsButton_Click(object sender, EventArgs e)
        {
            SettingsForm stfrm = new SettingsForm();
            if (stfrm.ShowDialog() == DialogResult.OK)
                Settings = stfrm.GetSettings();
        }
    }
}
