using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Timers;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using TeamodoroClient.Windows;
using Application = System.Windows.Application;
using Brush = System.Drawing.Brush;
using Color = System.Drawing.Color;
using Pen = System.Drawing.Pen;
using Timer = System.Timers.Timer;

namespace TeamodoroClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private NotifyIcon _icon;

        private ContextMenu _menu;

        private Timer _synchronizer;

        private TimerWindow timerWindow = new TimerWindow();


        private void startup(object sender, StartupEventArgs e)
        {
            _synchronizer = new Timer(30000);
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
            timerWindow.statusText.Foreground = new SolidColorBrush(Api.getInstance().getMediaColor());
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
                timerWindow.statusText.Foreground = new SolidColorBrush(Api.getInstance().getMediaColor());
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

            Pen linePen = new Pen(new SolidBrush(Color.White), 2);
            Brush backBrush = new SolidBrush(Color.Black);
            Brush centerBrush = new SolidBrush(Api.getInstance().getColor());

            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.FillEllipse(backBrush, 0, 0, 15, 15);
            graphics.FillEllipse(centerBrush, 4, 4, 7, 7);
            graphics.DrawArc(linePen, 2, 2, 11, 11, -90, 360 * (float)Api.getInstance().getRemainingTime() / (Api.getInstance().getRemainingTime() + Api.getInstance().getCurrentTime()));

            Icon createdIcon = Icon.FromHandle(bitmap.GetHicon());

            graphics.Dispose();
            bitmap.Dispose();

            return createdIcon;
        }

    }
}
