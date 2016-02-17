using System;
using System.Windows.Forms;
using ScriptCore;
using SSE.EventArguments;

namespace SSE.MyControls
{
    public class MyMenuItem : MenuItem,IScriptControl
    {
        
        public MyMenuItem() : base()
        { }
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

        public new EventHandler<ScriptEventArgs> Click;
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (Click != null)
            {
                var handler = Click;
                if (handler != null)
                    handler(this, new ScriptEventArgs { Script = _script });
            }
        }
    }
}
