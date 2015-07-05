namespace HardwareToSerialWriter.WPF
{
    using System;
    using System.Windows;

    public partial class SplashScreenWindow : Window, ISplashScreen
    {
        public SplashScreenWindow()
        {
            InitializeComponent();
        }

        public void AddMessage(string message)
        {
            Dispatcher.Invoke((Action)delegate()
            {
                this.UpdateMessageTextBox.Text = message;
            });
        }

        public void LoadComplete()
        {
            Dispatcher.InvokeShutdown();
        }
    }
}
