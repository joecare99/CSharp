using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.UI.Xaml;

namespace UWP_00_Test
{
    public partial class App : Application
    {
        private Window? _window;

        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            _window.Activate();
        }
    }
}
