using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace TeamodoroClient.Windows
{
    internal enum ModalAction
    {
        Show,
        Hide
    }

    /// <summary>
    /// Interaction logic for TimerWindow.xaml
    /// </summary>
    public partial class TimerWindow
    {
        private State _state;

        public TimerWindow()
        {
            InitializeComponent();
            BackgroundImage.Source = Api.GetInstance().GetBackgroundImage();
            _state = State.None;
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            HideModal(_state);
            Hide();
        }

        private void WindowDeactivated(object sender, EventArgs e)
        {
            if (_state == State.None) Hide();
        }

        private void ModalBlock(State state, ModalAction action)
        {
            bool show = action == ModalAction.Show;

            SettingsButton.Visibility = show ? Visibility.Hidden : Visibility.Visible;
            LoginButton.Visibility = show ? Visibility.Hidden : Visibility.Visible;

            switch (state)
            {
                case State.Settings:
                    SoonLabel.Visibility = show ? Visibility.Visible : Visibility.Hidden;
                    break;
            }

            CloseModalButton.Visibility = show ? Visibility.Visible : Visibility.Hidden;
            ModalBackground.Visibility = show ? Visibility.Visible : Visibility.Hidden;

            _state = show ? state : State.None;
        }

        private void ShowModal(State state)
        {
            ModalBlock(state, ModalAction.Show);
        }

        private void HideModal(State state)
        {
            ModalBlock(state, ModalAction.Hide);
        }

        private void CloseModalButtonMouseDown(object sender, MouseButtonEventArgs e)
        {
            HideModal(_state);
        }

        private void ButtonMouseEnterOrLeave(object sender, MouseEventArgs e)
        {
            double maxOpacity = 1.0;
            if (((Rectangle) sender).Tag != null) double.TryParse(((Rectangle) sender).Tag.ToString(), out maxOpacity);
            ((Rectangle)sender).Opacity = maxOpacity - ((Rectangle)sender).Opacity;
        }

        private void LogoButtonMouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("http://teamodoro.sdfgh153.ru");
        }

        private void SettingsButtonMouseDown(object sender, MouseButtonEventArgs e)
        {
            ShowModal(State.Settings);
        }

        private void BackButtonMouseDown(object sender, MouseButtonEventArgs e)
        {
            BackgroundImage.Source = Api.GetInstance().GetPreviousBackgroundImage();
        }

        private void NextButtonMouseDown(object sender, MouseButtonEventArgs e)
        {
            BackgroundImage.Source = Api.GetInstance().GetNextBackgroundImage();
        }
    }
}
