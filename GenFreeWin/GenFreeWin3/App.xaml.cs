using BaseLib.Helper;
using GenFree.ViewModels.Interfaces;
using GenFreeWpf.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;

namespace GenFreeWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            IoC.Configure(serviceCollection.BuildServiceProvider());
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MainWindow>();
            services.AddTransient<MainWindowViewModel>();
        }

        /// <summary>
        /// Handles the startup event of the application.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Beispiel: Hauptfenster mit DI aufrufen
            // var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            // mainWindow.Show();
        }
    }

}
