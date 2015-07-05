namespace HardwareToSerialWriter
{
    using System.Windows;
    using Viewmodels;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            App.SplashScreen.AddMessage("Loading");

            LoadViewModel();
            
            App.SplashScreen.AddMessage("Done!");
            App.SplashScreen.LoadComplete();
            InitializeComponent();
        }

        private void LoadViewModel()
        {
            var vm = new MainViewModel();
            DataContext = vm;
        }
    }
}