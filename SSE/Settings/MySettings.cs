using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSE.Settings;

namespace SSE
{
    public class MySettings : AppSettings<MySettings>
    {
        public bool minimizedStart;
        public bool runOnWinStart;
    }
}
