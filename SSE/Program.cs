using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using SSE.Presenter;

namespace SSE
{
    internal static class Program
    {
        // Sets the window to be foreground
        [DllImport("User32")]
        private static extern int SetForegroundWindow(IntPtr hwnd);

        // Activate or minimize a window
        [DllImportAttribute("User32.DLL")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int SW_RESTORE = 9;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Process currentProc = Process.GetCurrentProcess();
            foreach (Process proc in Process.GetProcessesByName(currentProc.ProcessName))
            {
                if (proc.Id != currentProc.Id)
                {
                    ShowWindow(proc.MainWindowHandle, SW_RESTORE);
                    SetForegroundWindow(proc.MainWindowHandle);
                    return;   // Exit application
                }
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var view = new MainForm();
            ScriptCore.IScriptManager sm = new ScriptCore.ScriptManager();
            var presenter = new MainPresenter(view, sm);
            Application.Run(view);
            
        }
    }
}