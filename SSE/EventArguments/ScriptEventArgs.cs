using System;
using ScriptCore;

namespace SSE.EventArguments
{
    public class ScriptEventArgs: EventArgs
    {
        public ExecutableScript Script;
        public ScriptEventArgs()
            :base()
        {
        }
    }
}
