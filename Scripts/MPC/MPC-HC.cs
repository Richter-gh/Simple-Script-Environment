using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using System.Threading;
using ScriptCore;
//https://github.com/marcjoha/AudioSwitcher
// thanks to this project^
namespace Scripts
{
    internal class MPCMonitor : IExecutable
    {
        public string Author
        {
            get
            {
                return "Me";
            }
        }

        public string Name
        {
            get
            {
                return "MPC-HC Monitor";
            }
        }

        public string Version
        {
            get
            {
                return "0.1";
            }
        }

        private bool _mpcLaunched = false;
        private bool _boxShown = false;
        private bool _deviceChanged = false;
        public Form frm = new Form();//test later
        public void Execute()
        {
            bool mpc = (Process.GetProcessesByName("mpc-hc").Length > 0 ||
                 Process.GetProcessesByName("mpc-hc64").Length > 0);
            if (mpc && !_mpcLaunched)
            {
                _mpcLaunched = true;
            }
            if (_mpcLaunched && !_boxShown && mpc)
            {
                //device 1 - TV
                //device 3 - speakers
                _boxShown = true;
                //change default output device to TV
                frm.TopMost = true;
                //if (MessageBox.Show(frm,"Switch audio device to TV?","MPC lauched",MessageBoxButtons.YesNo) == //DialogResult.Yes)
                {
                    if (!_deviceChanged)
                    {
                        SelectDevice(1);
                        _deviceChanged = true;
                    }
                }
            }
            if (!mpc && _mpcLaunched)
            {
                if (_deviceChanged)
                {
                    _mpcLaunched = false;
                    SelectDevice(3);
                    _deviceChanged = false;
                }
            }
        }

        #region EndPointController.exe interaction

        private static IEnumerable<Tuple<int, string, bool>> GetDevices()
        {
            var p = new Process
            {
                StartInfo =
                                {
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true,
                                    CreateNoWindow = true,
                                    FileName = AppDomain.CurrentDomain.BaseDirectory+"Scripts\\EndPointController.exe",
                                    Arguments = "-f \"%d|%ws|%d|%d\""
                                }
            };
            p.Start();
            Thread.Sleep(500);
            p.WaitForExit();
            var stdout = p.StandardOutput.ReadToEnd().Trim();
            var devices = new List<Tuple<int, string, bool>>();

            foreach (var line in stdout.Split('\n'))
            {
                var elems = line.Trim().Split('|');
                var deviceInfo = new Tuple<int, string, bool>(int.Parse(elems[0]), elems[1], elems[3].Equals("1"));
                devices.Add(deviceInfo);
            }

            return devices;
        }

        private static void SelectDevice(int id)
        {
            var p = new Process
            {
                StartInfo =
                                {
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true,
                                    CreateNoWindow = true,
                                    FileName = AppDomain.CurrentDomain.BaseDirectory+"Scripts\\EndPointController.exe",
                                    Arguments = id.ToString(CultureInfo.InvariantCulture)
                                }
            };
            p.Start();
            p.WaitForExit();
        }

        #endregion
    }
}