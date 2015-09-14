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

        private Settings.MySettings Settings;
        private void SettingsButton_Click(object sender, EventArgs e)
        {
            SettingsForm stfrm = new SettingsForm();
            if (stfrm.ShowDialog() == DialogResult.OK)
                Settings = stfrm.GetSettings();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var tst = new Dictionary<string, bool>()
            {
                {"some text",true}
            };
            tst.Add("some more text", true);
            ScriptCore.ScriptManager sm = new ScriptCore.ScriptManager(tst);
        }
    }
}
