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
using TeamodoroClient.Windows;
using TeamodoroClient.Windows.Controls;

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

        private TimerWindow timerWindow = new TimerWindow();


        private void startup(object sender, StartupEventArgs e)
        {
            _synchronizer = new System.Timers.Timer(30000);
            _synchronizer.Elapsed += SynchronizerElapsed;
            _synchronizer.Start();

            _menu = new ContextMenu();
            _menu.MenuItems.Add("Exit", OnExit);

            _icon = new NotifyIcon();
            _icon.Icon = TeamodoroClient.Properties.Resources.DefaultIcon;
            _icon.ContextMenu = _menu;
            _icon.Click += IconClick;
            _icon.Visible = true;
            
            Api.getInstance().updateState();
            Api.getInstance().StateChanged += StateChanged;
            Api.getInstance().TimerTick += TimerTick;

            StateChanged(Api.getInstance().getState());
        }

        void IconClick(object sender, EventArgs e)
        {
            timerWindow.statusText.Content = Api.getInstance().getState().ToString();
            timerWindow.statusText.Foreground = new System.Windows.Media.SolidColorBrush(Api.getInstance().getMediaColor());
            timerWindow.Left = Screen.PrimaryScreen.WorkingArea.Width - timerWindow.Width - 2;
            timerWindow.Top = Screen.PrimaryScreen.WorkingArea.Height - timerWindow.Height - 2;
            timerWindow.Show();
            timerWindow.Activate();
        }

        void SynchronizerElapsed(object sender, ElapsedEventArgs e)
        {
            Api.getInstance().updateState();
        }

        private void OnExit(object sender, EventArgs e)
        {
            if (timerWindow != null) timerWindow.Close();
            _icon.Visible = false;
            Shutdown(0);
        }

        private void StateChanged(State state) {
            _icon.ShowBalloonTip(1000, "State: " + state.ToString(), "Remaining time: " + Api.getInstance().getRemainingTimeString(), ToolTipIcon.Info);
            timerWindow.Dispatcher.BeginInvoke(new Action(delegate()
            {
                timerWindow.statusText.Content = Api.getInstance().getState().ToString();
                timerWindow.statusText.Foreground = new System.Windows.Media.SolidColorBrush(Api.getInstance().getMediaColor());
            }));
        }
        
        private void TimerTick()
        {
            _icon.Text = "State: " + Api.getInstance().getState().ToString() + "\nRemaining time: " + Api.getInstance().getRemainingTimeString();
            _icon.Icon = GetIcon();
            timerWindow.Dispatcher.BeginInvoke(new Action(delegate()
            {
                timerWindow.timerText.Content = Api.getInstance().getRemainingTimeString();
                timerWindow.timerValue.Value = (double)Api.getInstance().getRemainingTime() / (Api.getInstance().getRemainingTime() + Api.getInstance().getCurrentTime()) * 100;
            }));
        }

        private Icon GetIcon()
        {
            Bitmap bitmap = new Bitmap(16, 16);
            Graphics graphics = Graphics.FromImage(bitmap);

            Pen linePen = new System.Drawing.Pen(new SolidBrush(System.Drawing.Color.White), 2);
            Pen centerPen = new System.Drawing.Pen(Api.getInstance().getColor(), 4);

            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphics.DrawEllipse(centerPen, 6, 6, 4, 4);
            graphics.DrawArc(linePen, 1, 1, 14, 14, -90, 360 * (float)Api.getInstance().getRemainingTime() / (Api.getInstance().getRemainingTime() + Api.getInstance().getCurrentTime()));

            Icon createdIcon = Icon.FromHandle(bitmap.GetHicon());

            graphics.Dispose();
            bitmap.Dispose();

            return createdIcon;
        }

    }
}
