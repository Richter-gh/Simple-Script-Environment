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
        private object _audioDevices = null;
        private MethodInfo SetAsDefault;
        private MethodInfo GetAvailableAudioDevices;
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

        public override string Version
        {
            get
            {
                return "0.1";
            }
        }

        public override void Action()
        {
            if (!_deviceChanged)
            {
                SwitchDevice("PANASONIC");
                _deviceChanged = true;
            }
            else
            {
                SwitchDevice("Динамик");
                _deviceChanged = false;
            }
        }
        public override void OnLoad()
        {
            Assembly asm = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "Scripts\\MPC\\Audio.EndPoint.Controller.Wrapper.dll");
            Type _AudioDeviceWrapper = asm.GetType("AudioEndPointControllerWrapper.AudioDeviceWrapper");
            Type _AudioController = asm.GetType("AudioEndPointControllerWrapper.AudioController");
            //static
            GetAvailableAudioDevices = _AudioController.GetMethod("getAvailableAudioDevices");
            //not static
            SetAsDefault = _AudioDeviceWrapper.GetMethod("SetAsDefault");
            _audioDevices = GetAvailableAudioDevices.Invoke(null, null);
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
                    SwitchDevice("PANASONIC");
                    _deviceChanged = true;
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
                    SwitchDevice("Динамик");
                    _deviceChanged = false;
                }
                _boxShown = false;
                _mpcLaunched = false;
                _deviceChanged = false;
            }
        }

        private void SwitchDevice(string device_name)
        {           
            if (_audioDevices is IEnumerable)
            {
                foreach (var item in (_audioDevices as IEnumerable))
                {
                    Type type = item.GetType();
                    PropertyInfo info = type.GetProperty("FriendlyName");
                    string value = (string)info.GetValue(item, null);
                    if (value.Contains(device_name))
                    {
                        SetAsDefault.Invoke(item, null);
                    }
                }
            }
        }
    }
}