using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;

namespace TeamodoroClient.Windows
{
    /// <summary>
    /// Interaction logic for TimerWindow.xaml
    /// </summary>
    public partial class TimerWindow : Window
    {
        public TimerWindow()
        {
            InitializeComponent();
        }

        private void WindowDeactivated(object sender, EventArgs e)
        {
            Hide();
        }

        private void HyperlinkClick(object sender, RoutedEventArgs e)
        {
            Process.Start(((Hyperlink)sender).NavigateUri.ToString());
        }
    }
}
