using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SSE.EventArguments;
using SSE.MyControls;

namespace SSE.Interfaces
{
    public interface IMainView:IView
    {
        event EventHandler<DragEventArgs> ScriptPanelDragDrop;
        event EventHandler<ScriptEventArgs> ScriptPanelCheckedChanged;
        event EventHandler<ScriptEventArgs> ScriptPanelActionClick;
        event EventHandler AddFolderToolstripClick;
        event EventHandler AddFileToolstripClick;
        Panel ScriptPanel { get; }

        NotifyIcon TrayIcon { get; }

        List<MyCheckBox> CheckBoxList { get; }

        List<MyButton> ActionButtonList{ get; }

        RichTextBox LogTextBox { get; }        
        
        string ScriptsFolder { get; }

        void AddScriptControlsToPanel(MyCheckBox box, MyButton button);
        
    }
}
