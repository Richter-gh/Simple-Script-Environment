using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using System.Threading;
using ScriptCore;
using System.Reflection;
//https://github.com/marcjoha/AudioSwitcher
//https://github.com/Belphemur/AudioEndPointController
// thanks to this project^
namespace Scripts
{
    internal class MPCMonitor : ScriptBase, IExecutable
    {
        private bool _boxShown = false;
        private bool _deviceChanged = false;
        private bool _mpcLaunched = false;

        public override string Author
        {
            get
            {
                return "Me";
            }
        }

        public override bool IsRunnable
        {
            get
            {
                return true;
            }
        }

        public override string Name
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

        public override void Action()
        {
            throw new NotImplementedException();
        }

        public override void Run()
        {
            bool mpc = (Process.GetProcessesByName("mpc-hc").Length > 0 ||
                 Process.GetProcessesByName("mpc-hc64").Length > 0);
            if (mpc && !_mpcLaunched)
            {
                _mpcLaunched = true;
            }
            if (_mpcLaunched && !_boxShown && mpc)
            {
                _boxShown = true;
                if (!_deviceChanged)
                {
                    Assembly asm = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "Scripts\\MPC\\Audio.EndPoint.Controller.Wrapper.dll");
                    var _AudioDeviceWrapper = asm.GetType("AudioEndPointControllerWrapper.AudioDeviceWrapper");
                    var _AudioController = asm.GetType("AudioEndPointControllerWrapper.AudioController");
                    //static
                    var getAvailableAudioDevices = _AudioController.GetMethod("getAvailableAudioDevices");
                    //not static
                    var SetAsDefault = _AudioDeviceWrapper.GetMethod("SetAsDefault");
                    var _audioDevices = getAvailableAudioDevices.Invoke(null, null);
                    if (_audioDevices is IEnumerable)
                    {
                        foreach (var item in (_audioDevices as IEnumerable))
                        {
                            Type type = item.GetType();
                            PropertyInfo info = type.GetProperty("FriendlyName");
                            string value = (string)info.GetValue(item, null);
                            if (value.Contains("PANASONIC"))
                            {
                                SetAsDefault.Invoke(item, null);
                                _deviceChanged = true;
                            }

                        }
                    }
                }
                /*
                if (MessageBox.Show("Switch audio device to TV?","MPC lauched",MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (!_deviceChanged)
                    {
                        SelectDevice(1);
                        
                    }
                }*/
            }
            if (!mpc && _mpcLaunched)
            {
                if (_deviceChanged)
                {
                    Assembly asm = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "Scripts\\MPC\\Audio.EndPoint.Controller.Wrapper.dll");
                    var _AudioDeviceWrapper = asm.GetType("AudioEndPointControllerWrapper.AudioDeviceWrapper");
                    var _AudioController = asm.GetType("AudioEndPointControllerWrapper.AudioController");
                    //static
                    var getAvailableAudioDevices = _AudioController.GetMethod("getAvailableAudioDevices");
                    //not static
                    var SetAsDefault = _AudioDeviceWrapper.GetMethod("SetAsDefault");
                    var _audioDevices = getAvailableAudioDevices.Invoke(null, null);
                    if (_audioDevices is IEnumerable)
                    {
                        foreach (var item in (_audioDevices as IEnumerable))
                        {
                            Type type = item.GetType();
                            PropertyInfo info = type.GetProperty("FriendlyName");
                            string value = (string)info.GetValue(item, null);
                            if (value.Contains("Динамик"))
                            {
                                SetAsDefault.Invoke(item, null);
                                _deviceChanged = false;
                            }

                        }
                    }
                }
                _boxShown = false;
                _mpcLaunched = false;
                _deviceChanged = false;
            }
        }
    }
}