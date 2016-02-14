using System.Windows.Forms;
using ScriptCore;

namespace SSE
{
    class MyMenuItem : MenuItem
    {
        public ExecutableScript Script;
        public MyMenuItem(string text, MenuItem[] items) : base(text, items)
        { }
    }
}
