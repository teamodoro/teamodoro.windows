using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Shapes;

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
            _state = State.Settings;
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

        private void HyperlinkClick(object sender, RoutedEventArgs e)
        {
            Process.Start(((Hyperlink)sender).NavigateUri.ToString());
        }

        private void ModalBlock(State state, ModalAction action)
        {
            bool show = action == ModalAction.Show;

            SettingsButton.Visibility = show ? Visibility.Hidden : Visibility.Visible;

            switch (state)
            {
                case State.Settings:
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

        private void SettingsCloseButtonMouseDown(object sender, MouseButtonEventArgs e)
        {
            HideModal(_state);
        }

        private void SettingsButtonMouseDown(object sender, MouseButtonEventArgs e)
        {
            ShowModal(State.Settings);
        }

        private void ButtonMouseEnterOrLeave(object sender, MouseEventArgs e)
        {
            ((Rectangle) sender).Opacity = 1 - ((Rectangle) sender).Opacity;
        }

    }
}
