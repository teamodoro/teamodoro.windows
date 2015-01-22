using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Timers;
using TeamodoroClient.Json;

namespace TeamodoroClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private NotifyIcon _icon;

        private ContextMenu _menu;

        private System.Timers.Timer _synchronizer;

        private void startup(object sender, StartupEventArgs e)
        {
            _synchronizer = new System.Timers.Timer(10000);
            _synchronizer.Elapsed += SynchronizerElapsed;

            _menu = new ContextMenu();
            _menu.MenuItems.Add("Exit", OnExit);

            _icon = new NotifyIcon();
            _icon.Icon = TeamodoroClient.Properties.Resources.DefaultIcon;
            _icon.ContextMenu = _menu;
            _icon.Visible = true;
            
            Api.getInstance().updateState();
            Api.getInstance().StateChanged += StateChanged;

            _icon.ShowBalloonTip(1000, Api.getInstance().getState().ToString(), Api.getInstance().getRemainingTimeString(), ToolTipIcon.Info);
        }

        void SynchronizerElapsed(object sender, ElapsedEventArgs e)
        {
            Api.getInstance().updateState();
        }

        private void OnExit(object sender, EventArgs e)
        {
            _icon.Visible = false;
            Shutdown(0);
        }

        private void StateChanged(State state) {
            _icon.ShowBalloonTip(1000, state.ToString(), Api.getInstance().getRemainingTimeString(), ToolTipIcon.Info);
        }

    }
}
