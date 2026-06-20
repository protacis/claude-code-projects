using System;
using System.Runtime.InteropServices;
using Inventor;

namespace InventorParamsAddin
{
    [Guid("7A3F8D21-C4B6-4E9A-B251-8D3A0F5C7E9B")]
    [ComVisible(true)]
    public class AddinServer : Inventor.ApplicationAddInServer
    {
        public static Inventor.Application App { get; private set; }

        private DockableWindow _dockWin;
        private ParamsPanel   _panel;

        public void Activate(ApplicationAddInSite addInSite, bool firstTime)
        {
            App = addInSite.Application;

            _panel = new ParamsPanel();
            _panel.CreateControl();

            _dockWin = App.UserInterfaceManager.DockableWindows.Add(
                "{7A3F8D21-C4B6-4E9A-B251-8D3A0F5C7E9B}",
                "InventorParamsAddin.ParamsPanel",
                "Key Parameters");

            _dockWin.AddChild(_panel.Handle);
            _dockWin.SetMinimumSize(280, 160);
            _dockWin.DockingState = DockingStateEnum.kDockRight;
            _dockWin.Visible      = true;
        }

        public void Deactivate()
        {
            _panel?.Dispose();
            _panel   = null;
            _dockWin = null;
            App      = null;
        }

        public void ExecuteCommand(int commandID) { }
        public object Automation => null;
    }
}
