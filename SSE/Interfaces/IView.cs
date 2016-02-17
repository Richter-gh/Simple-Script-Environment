using System;
using System.Windows.Forms;

namespace SSE.Interfaces
{
    public interface IView
    {
        event EventHandler FormLoad;
        event EventHandler FormStop;
    }
}
