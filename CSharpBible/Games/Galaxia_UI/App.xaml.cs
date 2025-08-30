using Galaxia.Models;
using Galaxia.Models.Interfaces;
using Galaxia.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Galaxia.UI
{
    /// <summary>
    /// Anwendungseintritt mit Generic Host (DI + Logging).
    /// </summary>
    public partial class App : Application
    {
        public static IHost HostInstance { get; private set; }

        public App()
        {
            HostInstance = Microsoft.Extensions.Hosting.Host
                .CreateDefaultBuilder()
                .ConfigureServices(ConfigureServices)
                .Build();
        }

        private void ConfigureServices(HostBuilderContext ctx, IServiceCollection services)
        {
            services.AddSingleton<ISpace, Space>(sp =>
            {
                var space = new Space();
                space.Initialize(); // Grundinitialisierung
                return space;
            });

            // Factory für ICorporation ohne Zirkularität
            services.AddSingleton<ICorporation>(sp =>
            {
                var space = sp.GetRequiredService<ISpace>();
                var firstSector = space.Sectors.Values.First();
                var homeStar = firstSector.Starsystems.First();
                // Verwende erweiterten Corporation-Konstruktor (siehe aktualisierte Klasse unten)
                var corp = new Corporation("DemoCorp", "Demonstration", Color.Aqua,
                                           homeStar,
                                           new System.Collections.Generic.List<IStarsystem> { homeStar },
                                           space);
                return corp;
            });

            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MainWindow>();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await HostInstance.StartAsync();

            var mainWindow = HostInstance.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await HostInstance.StopAsync();
            HostInstance.Dispose();
            base.OnExit(e);
        }
    }
}