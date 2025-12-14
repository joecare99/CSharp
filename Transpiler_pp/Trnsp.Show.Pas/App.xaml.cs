using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Trnsp.Show.Pas.Services;
using Trnsp.Show.Pas.ViewModels;

namespace Transp.Show.Pas
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public new static App Current => (App)Application.Current;

        public IServiceProvider Services { get; }

        public App()
        {
            Services = ConfigureServices();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Services
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<IPascalParserService, PascalParserService>();

            // ViewModels
            services.AddTransient<MainViewModel>();

            return services.BuildServiceProvider();
        }
    }
}
