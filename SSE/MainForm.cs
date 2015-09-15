using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ScriptCore;

namespace SSE
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private Settings.MySettings Settings;
        private ScriptManager _sm;
        private List<MyCheckBox> _checkBoxList;
        private void SettingsButton_Click(object sender, EventArgs e)
        {
            SettingsForm stfrm = new SettingsForm();
            if (stfrm.ShowDialog() == DialogResult.OK)
                Settings = stfrm.GetSettings();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _sm = new ScriptManager();
            _checkBoxList = new List<MyCheckBox>();
            if (!Directory.Exists("Scripts"))
                Directory.CreateDirectory("Scripts");
            foreach (string file in Directory.GetFiles("Scripts"))
            {
                FileInfo fi = new FileInfo(file);
                string message;
                if (fi.Extension == ".cs")
                    _sm.Add(file, true, out message);
            }
            foreach (var item in _sm.Scripts)
            {
                var box = new MyCheckBox();
                box.Tag = item.ScriptName;
                box.Text = item.ScriptName;
                box.Script = item;
                box.Checked = true;
                box.AutoSize = true;
                box.CheckedChanged += OnCheckedChanged;
                AddToPanel(box);
            }
            
        }
        private void OnCheckedChanged(object sender, EventArgs e)
        {
            MyCheckBox box = (MyCheckBox)sender;
            foreach (var item in _checkBoxList)
            {
                if (item.Script.FileName == box.Script.FileName)
                    item.Script.Run = box.Checked;
                item.Refresh();
            }
            //RefreshPanel();
        }
        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            var s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (s != null)
            {                
                for (int i = 0; i < s.Length; i++)
                {
                    if (s[i].Contains(".cs"))
                    {
                        string message;
                        _sm.Add(s[i], false, out message);
                    }
                }
                RepopulatePanel();
            }
        }
        private void RepopulatePanel()
        {
            int i = 1;
            panel1.Controls.Clear();
            foreach (var item in _checkBoxList)
            {                
                panel1.Controls.Add(item);
            }
        }
        private void RefreshPanel()
        {
            int i = 1;
            panel1.Controls.Clear();
            foreach (MyCheckBox item in panel1.Controls)
            {
                item.Script.Run = item.Checked;
            }
        }
        private void AddToPanel(MyCheckBox box)
        {
            int i = 1;
            if(_checkBoxList.Count>0)
                i = _checkBoxList[_checkBoxList.Count-1].Location.Y/50;
            box.Location = new Point(10, i * 50);
            panel1.Controls.Add(box);
        }

    }
}
