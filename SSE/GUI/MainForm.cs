using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SSE.Interfaces;
using SSE.EventArguments;
using SSE.MyControls;
using ScriptCore;

namespace SSE
{

    public partial class MainForm : Form, IMainView
    {

        public MainForm()
        {
            InitializeComponent();
        }

        #region Properties

        public MySettings LeSettings
        {
            get
            {
                return _settings;
            }            
        }
        private MySettings _settings;
        private Panel _scriptPanel;
        private List<MyCheckBox> _checkBoxList;
        private List<MyButton> _actionButtonList;
        private NotifyIcon _trayIcon;
        private string _scriptsFolder = AppDomain.CurrentDomain.BaseDirectory + "Scripts\\";
        #endregion

        #region IMainForm implementation
        public Panel ScriptPanel
        {
            get
            {
                return _scriptPanel;
            }
        }

        public string ScriptsFolder
        {
            get
            {
                return _scriptsFolder;
            }
        }
        public NotifyIcon TrayIcon
        {
            get
            {
                return _trayIcon;
            }
        }

        public List<MyCheckBox> CheckBoxList
        {
            get
            {
                return _checkBoxList;
            }
        }
        public List<MyButton> ActionButtonList
        {
            get
            {
                return _actionButtonList;
            }
        }

        public RichTextBox LogTextBox
        {
            get
            {
                return richTextBox1;
            }
        }

        public event EventHandler<DragEventArgs> ScriptPanelDragDrop;
        public event EventHandler<ScriptEventArgs> ScriptPanelCheckedChanged;
        public event EventHandler<ScriptEventArgs> ScriptPanelActionClick;
        public event EventHandler AddFolderToolstripClick;
        public event EventHandler AddFileToolstripClick;
        public event EventHandler SettingsWindowClosed;
        public event EventHandler FormLoad;
        public event EventHandler FormStop;


        #endregion

        #region Form Events
        private void MainForm_Load(object sender, System.EventArgs e)
        {
            _scriptPanel = panel1;
            _trayIcon = new NotifyIcon();
            _trayIcon.Text = "Simple Script Environment";
            _trayIcon.ContextMenu = new ContextMenu();
            _trayIcon.MouseClick += trayOpenClick;
            _trayIcon.Visible = true;
            _trayIcon.Icon = this.Icon;

            _checkBoxList = new List<MyCheckBox>();
            _actionButtonList = new List<MyButton>();
            if (!Directory.Exists(_scriptsFolder))
                Directory.CreateDirectory(_scriptsFolder);
            LoadSettings(null);

            if (LeSettings.minimizedStart)
                this.WindowState = FormWindowState.Minimized;
            if (FormLoad != null)
            {
                if (FormLoad != null)
                    FormLoad(this, EventArgs.Empty);
            }
            this.ShowInTaskbar = true;

            _trayIcon.ContextMenu.MenuItems.Add(new MenuItem("Exit", new EventHandler(trayExitClick)));
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void MainForm_Resize(object sender, System.EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                //this.Hide();
                //this.ShowInTaskbar = true;
            }
        }

        private void panel1_DragDrop(object sender, DragEventArgs e)
        {
            var s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (s != null)
            {
                if (ScriptPanelDragDrop != null)
                    ScriptPanelDragDrop(this, e);                
            }
        }

        private void panel1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void settingsToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            var stfrm = new SettingsForm();
            stfrm.settings = this.LeSettings;
            if (stfrm.ShowDialog() == DialogResult.OK)
            {
                var settings = stfrm.GetSettings();
                if (SettingsWindowClosed != null)
                    SettingsWindowClosed(this, new SettingsEventArgs { Settings = settings });
                LoadSettings(settings);
            }
        }
        private void fromFolderToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (AddFolderToolstripClick != null)
                AddFolderToolstripClick(this, EventArgs.Empty);
        }

        private void fromFileToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (AddFileToolstripClick != null)
                AddFileToolstripClick(this, EventArgs.Empty);
        }

        #endregion

        private void LoadSettings(MySettings settings)
        {
            if (settings == null)
                this._settings = MySettings.Load();
            else
                this._settings = settings;
        }

        #region Panel Management

        /// <summary>
        /// Adds a checkbox to the panel
        /// </summary>
        /// <param name="box">MyCheckBox object</param>
        /// <param name="button">MyButton object</param>
        public void AddScriptControlsToPanel(MyCheckBox box, MyButton button)
        {
            int i = 1;
            if (_checkBoxList.Count > 0)
                i += _checkBoxList[_checkBoxList.Count - 1].Location.Y / 20;
            box.Location = new Point(30, i * 20);
            box.AutoSize = true;
            box.CheckedChanged += ScriptPanelCheckedChanged;
            button.Location = new Point(10, (i * 20));
            button.Text = "action";
            button.Visible = true;
            button.Size = new Size(15, 15);
            button.Click += ScriptPanelActionClick;
            _actionButtonList.Add(button);
            ScriptPanel.Controls.Add(button);
            _checkBoxList.Add(box);            
            ScriptPanel.Controls.Add(box);
        }

        private void trayExitClick(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void trayOpenClick(object sender, MouseEventArgs e)
        {
            //so the form won't hide itself on rightclick everytime
            if (e.Button == MouseButtons.Left)
            {
                if (this.WindowState != FormWindowState.Normal)
                {
                    this.Show();
                    //this.ShowInTaskbar = true;
                    this.WindowState = FormWindowState.Normal;
                }
                else
                {
                    this.WindowState = FormWindowState.Minimized;
                    this.Hide();
                }
            }
        }

        #endregion

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (FormStop != null)
                FormStop(this, EventArgs.Empty);
        }
    }
}
