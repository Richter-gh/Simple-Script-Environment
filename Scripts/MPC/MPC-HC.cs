using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using ScriptCore;

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

        private bool _mpcLaunched;
        private bool _boxShown;
        public Form frm = new Form();
        public void Execute()
        {
            if (Process.GetProcessesByName("mpc-hc").Length > 0 && !_mpcLaunched)
            {
                _mpcLaunched = true;
            }
            if (_mpcLaunched && !_boxShown)
            {
                //change default output device to TV
                if (MessageBox.Show("Switch audio device to TV?",
                                    "MPC lauched",  
                                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    frm.Show();
                    foreach (var tuple in GetDevices())
                    {
                        var id = tuple.Item1;
                        var deviceName = tuple.Item2;
                        var isInUse = tuple.Item3;
                        MessageBox.Show(deviceName);
                    }

                }
                _boxShown = true;
            }
            if (Process.GetProcessesByName("mpc-hc").Length == 0)
                _mpcLaunched = false;
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
                                    FileName = "EndPointController.exe",
                                    Arguments = "-f \"%d|%ws|%d|%d\""
                                }
            };
            p.Start();
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
                                    FileName = "EndPointController.exe",
                                    Arguments = id.ToString(CultureInfo.InvariantCulture)
                                }
            };
            p.Start();
            p.WaitForExit();
        }

        #endregion
    }
}