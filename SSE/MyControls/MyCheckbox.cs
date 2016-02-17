using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ScriptCore;
using SSE.EventArguments;

namespace SSE.MyControls
{
    public class MyCheckBox : CheckBox, IScriptControl

    {
        private ExecutableScript _script;
        public ExecutableScript Script
        {
            get
            {
                return _script;
            }

            set
            {
                _script = value;
            }
        }

        public new EventHandler<ScriptEventArgs> CheckedChanged;
        protected override void OnCheckedChanged(EventArgs e)
        {
            base.OnCheckedChanged(e);

            if (CheckedChanged != null)
            {
                var handler = CheckedChanged;
                if (handler != null)
                    handler(this, new ScriptEventArgs { Script = _script });
            }
        }

    }

}
