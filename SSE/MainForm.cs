using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;
using ScriptCore;

namespace SSE
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();                    
        }
        
        #region Properties
        private MySettings Settings;
        private ScriptManager _sm;
        private List<MyCheckBox> _checkBoxList;
        private Timer _timer;
        private NotifyIcon _trayIcon;
        #endregion

        #region Form Events
        private void MainForm_Load(object sender, EventArgs e)
        {
            _trayIcon = new NotifyIcon();
            _trayIcon.Text = "Simple Script Environment";
            _trayIcon.ContextMenu = new ContextMenu();
            _trayIcon.ContextMenu.MenuItems.Add(new MenuItem("Exit", new EventHandler(trayExitClick)));
            _trayIcon.Click += trayOpenClick;
            _trayIcon.Visible = true;
            _trayIcon.Icon = this.Icon;
            this.ShowInTaskbar = false;
            _timer = new Timer();
            _timer.Tick += TickEvent;
            _sm = new ScriptManager();
            _checkBoxList = new List<MyCheckBox>();
            if (!Directory.Exists("Scripts"))
                Directory.CreateDirectory("Scripts");
            foreach (string file in Directory.GetFiles("Scripts"))
            {
                AddScript(file, true);
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
            _timer.Interval = 1000;
            _timer.Start();

            LoadSettings(null);

            if (Settings.minimizedStart)
                this.WindowState = FormWindowState.Minimized;
                
       
            //if(Settings.runOnWinStart)
            //kek
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            RegisterInStartup(Settings.runOnWinStart);
            _trayIcon.Visible = false;
        }
        private void panel1_DragDrop(object sender, DragEventArgs e)
        {
            var s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (s != null)
            {
                for (int i = 0; i < s.Length; i++)
                {
                    AddScript(s[i], false);
                }
                RepopulatePanel();
            }
        }

        private void panel1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm stfrm = new SettingsForm();
            stfrm.settings = this.Settings;
            if (stfrm.ShowDialog() == DialogResult.OK)
            {
                var settings = stfrm.GetSettings();
                LoadSettings(settings);
            }
        }
        #endregion

        #region Panel Management

        /// <summary>
        /// Redraws the panel with checkboxes
        /// </summary>
        private void RepopulatePanel()
        {
            int i = 1;
            panel1.Controls.Clear();
            _checkBoxList.Clear();
            foreach (var item in _sm.Scripts)
            {
                var box = new MyCheckBox();
                box.Tag = item.ScriptName;
                box.Text = item.ScriptName;
                box.Script = item;
                box.Checked = item.Run;
                box.AutoSize = true;
                box.CheckedChanged += OnCheckedChanged;
                AddToPanel(box);
            }
        }

        /// <summary>
        /// Refreshes script run parameter in the panel checkboxes
        /// </summary>
        private void RefreshPanel()
        {
            int i = 1;
            panel1.Controls.Clear();
            foreach (MyCheckBox item in panel1.Controls)
            {
                item.Script.Run = item.Checked;
            }
        }

        /// <summary>
        /// Adds a checkbox to the panel
        /// </summary>
        /// <param name="box">MyCheckBox object</param>
        private void AddToPanel(MyCheckBox box)
        {
            int i = 1;
            if (_checkBoxList.Count > 0)
                i += _checkBoxList[_checkBoxList.Count - 1].Location.Y / 20;
            box.Location = new Point(20, i * 20);
            _checkBoxList.Add(box);
            panel1.Controls.Add(box);
        }
        #endregion

        #region My Events

        private void trayExitClick(object sender, EventArgs e)
        {
            this.Close();
        }

        private void trayOpenClick(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
        }

        private void TickEvent(object sender, EventArgs e)
        {
            _sm.Execute();
        }

        private void OnCheckedChanged(object sender, EventArgs e)
        {
            MyCheckBox box = (MyCheckBox)sender;
            foreach (var item in _checkBoxList)
            {
                if (item.Script.FileName == box.Script.FileName)
                {
                    item.Script.Run = box.Checked;
                }
                item.Refresh();
            }
            foreach (var item in _sm.Scripts)
            {
                if (item.FileName == box.Script.FileName)
                {
                    item.Run = box.Checked;
                }
            }
            //RefreshPanel();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Register for autostart on windows startup
        /// </summary>
        /// <param name="isChecked"></param>
        private void RegisterInStartup(bool isChecked)
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey
                    ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (isChecked)
            {
                registryKey.SetValue("ApplicationName", Application.ExecutablePath);
            }
            else
            {
                registryKey.DeleteValue("ApplicationName");
            }
        }

        private void LoadSettings(MySettings settings)
        {
            
            if (settings == null)
                this.Settings = MySettings.Load();
            else
                this.Settings = settings;
        }

        private void AddScript(string file, bool k)
        {
            FileInfo fi = new FileInfo(file);

            if (fi.Extension == ".cs")
            {
                string message;
                if (!_sm.Add(fi.FullName, k, out message))
                    richTextBox1.Text += message + '\n';
            }
        }

        #endregion

        
    }
}
