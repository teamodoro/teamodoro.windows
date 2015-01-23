using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Timers;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using TeamodoroClient.Windows;
using Brush = System.Drawing.Brush;
using Color = System.Drawing.Color;
using Timer = System.Timers.Timer;

namespace TeamodoroClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private NotifyIcon _icon;

        private ContextMenu _menu;

        private Timer _synchronizer;

        private readonly TimerWindow _timerWindow = new TimerWindow();


        private void StartupApplication(object sender, StartupEventArgs e)
        {
            _synchronizer = new Timer(30000);
            _synchronizer.Elapsed += SynchronizerElapsed;
            _synchronizer.Start();

            _menu = new ContextMenu();
            _menu.MenuItems.Add("Exit", OnExit);

            _icon = new NotifyIcon
            {
                Icon = TeamodoroClient.Properties.Resources.DefaultIcon, 
                ContextMenu = _menu,
                Text = @"Connecting..."
            };
            _icon.Click += IconClick;
            _icon.Visible = true;
            
            Api.GetInstance().UpdateState();
            Api.GetInstance().StateChanged += StateChanged;
            Api.GetInstance().TimerTick += TimerTick;

            StateChanged(Api.GetInstance().GetState());
        }

        void IconClick(object sender, EventArgs e)
        {
            _timerWindow.StatusText.Content = Api.GetInstance().GetState().ToString();
            _timerWindow.StatusText.Foreground = new SolidColorBrush(Api.GetInstance().GetMediaColor());
            _timerWindow.Left = Screen.PrimaryScreen.WorkingArea.Width - _timerWindow.Width - 2;
            _timerWindow.Top = Screen.PrimaryScreen.WorkingArea.Height - _timerWindow.Height - 2;
            _timerWindow.Show();
            _timerWindow.Activate();
        }

        void SynchronizerElapsed(object sender, ElapsedEventArgs e)
        {
            Api.GetInstance().UpdateState();
        }

        private void OnExit(object sender, EventArgs e)
        {
            if (_timerWindow != null) _timerWindow.Close();
            _icon.Visible = false;
            Shutdown(0);
        }

        private void StateChanged(State state) {
            _icon.ShowBalloonTip(1000, string.Format("State: {0}", state), string.Format("Remaining time: {0}", Api.GetInstance().GetRemainingTimeString()), ToolTipIcon.Info);
            _timerWindow.Dispatcher.BeginInvoke(new Action(delegate
            {
                _timerWindow.StatusText.Content = Api.GetInstance().GetState().ToString();
                _timerWindow.StatusText.Foreground = new SolidColorBrush(Api.GetInstance().GetMediaColor());
            }));
        }
        
        private void TimerTick()
        {
            _icon.Text = string.Format("\nState: {0}\nRemaining time: {1}", Api.GetInstance().GetState(), Api.GetInstance().GetRemainingTimeString());
            _icon.Icon = GetIcon();
            _timerWindow.Dispatcher.BeginInvoke(new Action(delegate
            {
                _timerWindow.TimerText.Content = Api.GetInstance().GetRemainingTimeString();
                _timerWindow.TimerValue.Value = (double)Api.GetInstance().GetRemainingTime() / (Api.GetInstance().GetRemainingTime() + Api.GetInstance().GetCurrentTime()) * 100;
            }));
        }

        private Icon GetIcon()
        {
            Bitmap bitmap = new Bitmap(16, 16);
            Graphics graphics = Graphics.FromImage(bitmap);

            Brush backBrush = new SolidBrush(Color.Black);
            Brush frontBrush = new SolidBrush(Api.GetInstance().GetColor());

            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.FillEllipse(backBrush, 0, 0, 15, 15);
            graphics.FillPie(frontBrush, 1, 1, 13, 13, -90, 360 * (float)Api.GetInstance().GetRemainingTime() / (Api.GetInstance().GetRemainingTime() + Api.GetInstance().GetCurrentTime()));

            Icon createdIcon = Icon.FromHandle(bitmap.GetHicon());

            graphics.Dispose();
            bitmap.Dispose();

            return createdIcon;
        }

    }
}
