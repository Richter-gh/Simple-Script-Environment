using System;
using ScriptCore;

namespace SSE.MyControls
{
    public interface IScriptControl
    {
        ExecutableScript Script { get; set; }
    }
}
