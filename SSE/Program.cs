using System;
using System.Windows.Forms;
using SSE.Presenter;

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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var view = new MainForm();
            ScriptCore.IScriptManager sm = new ScriptCore.ScriptManager();
            var presenter = new MainPresenter(view,sm);
            Application.Run(view);
        }
    }
}