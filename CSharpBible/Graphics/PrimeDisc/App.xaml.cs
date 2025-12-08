using System.Windows;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using PrimePlotter.Services;
using PrimePlotter.ViewModels;
using PrimePlotter.Views;
using PrimePlotter.Services.Interfaces;

namespace PrimeDisc
{
    public partial class App : Application
    {
        private IHost? _host;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IPrimeService, PrimeService>();
                    services.AddSingleton<IPlotService, PlotService>();
                    services.AddSingleton<MainViewModel>();
                    services.AddSingleton<MainWindow>();
                })
                .Build();

            var window = _host.Services.GetRequiredService<MainWindow>();
            window.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            if (_host != null)
            {
                await _host.StopAsync();
                _host.Dispose();
            }
            base.OnExit(e);
        }
    }
}
