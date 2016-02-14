using System;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace SSE
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            //Assembly asm = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory+"ScriptCore.dll");
            //AppDomain.CurrentDomain.Load(asm.GetName());
            //Thread.Sleep(10000);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}