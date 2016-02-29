using SSE.Interfaces;
using SSE.MyControls;
using SSE.EventArguments;
using ScriptCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace SSE.Presenter
{
    public class MainPresenter
    {
        private readonly IMainView _mainForm;
        private readonly IScriptManager _sm;

        //private ScriptManager _sm;
        private Task mainLoop;
        private CancellationTokenSource token;

        public MainPresenter(IMainView view,IScriptManager sm)
        {
            _mainForm = view;
            _sm = sm;
            _mainForm.FormLoad += _mainForm_FormLoad;
            _mainForm.ScriptPanelActionClick += _mainForm_ScriptPanelActionClick;
            _mainForm.ScriptPanelCheckedChanged += _mainForm_ScriptPanelCheckedChanged;
            _mainForm.ScriptPanelDragDrop += _mainForm_ScriptPanelDragDrop;
            _mainForm.AddFileToolstripClick += _mainForm_AddFileToolstripClick;
            _mainForm.AddFolderToolstripClick += _mainForm_AddFolderToolstripClick;
            _mainForm.FormStop += _mainForm_FormClosing;
            StartLoop();
        }

        #region MainForm events

        private void _mainForm_FormLoad(object sender, EventArgs e)
        {

            foreach (string file in Directory.GetFiles(_mainForm.ScriptsFolder, "*.cs"))
            {
                AddScript(file, true);
            }
            foreach (string directory in Directory.GetDirectories(_mainForm.ScriptsFolder))
            {
                AddScript(directory, true);
            }
            //load script status from json
            if (_mainForm.LeSettings.ScriptStatus != null)
            {
                foreach(string key in _mainForm.LeSettings.ScriptStatus.Keys)
                {
                    var tempScript = _sm.Scripts.FirstOrDefault(z => z.ScriptName == key);
                    tempScript.Enabled = _mainForm.LeSettings.ScriptStatus[key];
                }
            }
            else
            {
                _mainForm.LeSettings.ScriptStatus = new System.Collections.Generic.Dictionary<string, bool>();
                foreach (ExecutableScript scriptName in _sm.Scripts)
                {
                    _mainForm.LeSettings.ScriptStatus.Add(scriptName.ScriptName, true);
                }
            }
            foreach (var item in _sm.Scripts)
            {
                var a = new MenuItem[] { };

                var box = new MyCheckBox();
                var button = new MyButton();
                button.Script = item;

                box.Tag = item.ScriptName;
                box.Text = item.ScriptName;
                box.Script = item;
                box.Checked = item.Enabled;

                _mainForm.AddScriptControlsToPanel(box, button);

                var menuItem = new MenuItem(item.ScriptName);

                var myMenuEnable = new MyMenuItem();
                myMenuEnable.Text = item.Enabled ? "Disable" : "Enable";
                myMenuEnable.Click += _mainForm_TrayScriptEnableClick;
                myMenuEnable.Script = item;

                var myMenuAction = new MyMenuItem();
                myMenuAction.Text = "Action";
                myMenuAction.Click += _mainForm_TrayScriptActionClick;
                myMenuAction.Script = item;

                menuItem.MenuItems.Add(myMenuEnable);
                menuItem.MenuItems.Add(myMenuAction);
                _mainForm.TrayIcon.ContextMenu.MenuItems.Add(menuItem);

            }

            RepopulatePanel();

            StartLoop();
        }
        private void _mainForm_FormClosing(object sender, EventArgs e)
        {
            StopLoop();
            _mainForm.LeSettings.Save();
        }
        private void _mainForm_AddFolderToolstripClick(object sender, EventArgs e)
        {
            var browser = new FolderBrowserDialog();
            browser.Description = "Select Script folder";
            if (browser.ShowDialog() == DialogResult.OK)
            {
                AddScript(browser.SelectedPath, false);
            }
        }
        private void _mainForm_AddFileToolstripClick(object sender, EventArgs e)
        {
            var browser = new OpenFileDialog();
            browser.Title = "Select Script file";
            browser.Filter = "CS files|*.cs";
            if (browser.ShowDialog() == DialogResult.OK)
            {
                string path = Path.GetDirectoryName(browser.FileName) + "\\" + browser.SafeFileName;
                AddScript(path, false);
            }
        }
        private void _mainForm_TrayScriptEnableClick(object sender, ScriptEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            if (e.Script.Enabled)
            {
                e.Script.Enabled = false;
                menuItem.Text = "Enable";
            }
            else
            {
                e.Script.Enabled = true;
                menuItem.Text = "Disable";
            }
            RefreshPanel();
        }
        private void _mainForm_TrayScriptActionClick(object sender, ScriptEventArgs e)
        {
            e.Script.Enabled = false;
            e.Script.Action();
            RefreshPanel();
        }
        private void _mainForm_ScriptPanelDragDrop(object sender, DragEventArgs e)
        {
            var s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            for (int i = 0; i < s.Length; i++)
            {
                AddScript(s[i], false);
            }
            RepopulatePanel();
        }
        private void _mainForm_ScriptPanelCheckedChanged(object sender, ScriptEventArgs e)
        {
            var box = (MyCheckBox)sender;
            var script = box.Script;
            script.Enabled = box.Checked;
            _mainForm.LeSettings.ScriptStatus[script.ScriptName] = false;
            box.Refresh();
        }
        private void _mainForm_ScriptPanelActionClick(object sender, ScriptEventArgs e)
        {
            e.Script.Enabled = false;
            e.Script.Action();
            RefreshPanel();
        }

        #endregion

        #region View Management

        private void RefreshPanel()
        {
            foreach (var item in _mainForm.ScriptPanel.Controls)
            {
                if (item is MyCheckBox)
                    (item as MyCheckBox).Checked = (item as MyCheckBox).Script.Enabled;
            }
        }
        /// <summary>
        /// Repopulates the ScriptPanel with scripts from the current ScriptManager instance.
        /// </summary>
        private void RepopulatePanel()
        {
            _mainForm.ScriptPanel.Controls.Clear();
            _mainForm.CheckBoxList.Clear();
            _mainForm.ActionButtonList.Clear();
            foreach (var item in _sm.Scripts)
            {
                var box = new MyCheckBox();
                var button = new MyButton();
                button.Script = item;

                box.Tag = item.ScriptName;
                box.Text = item.ScriptName;
                box.Script = item;
                box.Checked = item.Enabled;

                _mainForm.AddScriptControlsToPanel(box,button);
            }
        }

        #endregion

        #region Script Management        
        /// <summary>
        /// Adds the script to the ScriptManager instance.
        /// </summary>
        /// <param name="text">Scripts file or the script itself</param>
        /// <param name="k">if set to <c>true</c> script will be executed in a loop.</param>
        private void AddScript(string text, bool k)
        {
            if (Directory.Exists(text))
            {
                bool copied = false;
                foreach (string file in Directory.GetFiles(text, "*.cs", SearchOption.AllDirectories))
                {
                    var fi = new FileInfo(file);
                    if (fi.Extension == ".cs")
                    {
                        string message;
                        if (!_sm.Add(fi.FullName, k, out message))
                            _mainForm.LogTextBox.Text += message + '\n';
                        else
                        {
                            if (!copied && !Directory.Exists(_mainForm.ScriptsFolder + (new DirectoryInfo(text)).Name))
                            {
                                DirectoryCopy(text, _mainForm.ScriptsFolder);
                                copied = true;
                            }
                        }
                    }
                }
            }
            else
            {
                var fi = new FileInfo(text);
                if (fi.Extension == ".cs")
                {
                    string message;
                    if (!_sm.Add(fi.FullName, k, out message))
                        _mainForm.LogTextBox.Text += message + '\n';
                    else
                    {
                        if (!File.Exists(_mainForm.ScriptsFolder + fi.Name))
                            File.Copy(text, _mainForm.ScriptsFolder + fi.Name, true);
                    }
                }
            }
            //RepopulatePanel();
        }
        /// <summary>
        /// Starts endless loop executing all enabled scripts
        /// Supports cancellationToken
        /// </summary>
        private void StartLoop()
        {
            token = new CancellationTokenSource();
            var canellationToken = token.Token;
            mainLoop = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    canellationToken.ThrowIfCancellationRequested();
                    try { _sm.Execute(); }
                    catch(Exception e) { MessageBox.Show(e.ToString()); }
                    Thread.Sleep(50);
                }
            }, canellationToken);
        }
        private void StopLoop()
        {
            token.Cancel();
            try
            {
                mainLoop.Wait();
            }
            catch (AggregateException ex)
            {
                if (mainLoop.IsFaulted)
                    MessageBox.Show(ex.Flatten().ToString());
            }
        }
       
        #endregion

        #region Misc
        
        /// <summary>
        /// Register for autostart on windows startup
        /// </summary>
        /// <param name="isChecked"></param>
        public static void RegisterInStartup(bool isChecked)
        {
            /*RegistryKey registryKey = Registry.CurrentUser.OpenSubKey
                    ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (isChecked)
            {
                registryKey.SetValue("SSE", Application.ExecutablePath);
            }
            else
            {
                registryKey.DeleteValue("SSE");
            }*/
        }

        
        /// <summary>
        /// Copies recursively source directory to destination
        /// </summary>
        /// <param name="sourceDirectory"></param>
        /// <param name="targetDirectory"></param>
        public void DirectoryCopy(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory + diSource.Name);

            CopyAll(diSource, diTarget);
        }

        public void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            // Check if the target directory exists; if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
        
        #endregion
    }
}
