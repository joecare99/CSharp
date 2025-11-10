using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Windows;
using ContentControlStyle.ViewModels;
using ContentControlStyle.Views;

namespace ContentControlStyle;

public partial class App
{
    private IHost? _host;

    private void ConfigureHost()
    {
        _host = Host.CreateDefaultBuilder()
        .ConfigureServices(services =>
        {
            services.AddSingleton<MainWindowViewModel>();
            services.AddTransient<MainWindow>();
        })
        .Build();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        ConfigureHost();
        _host!.Start();
        var window = _host.Services.GetRequiredService<MainWindow>();
        window.DataContext = _host.Services.GetRequiredService<MainWindowViewModel>();
        window.Show();
        base.OnStartup(e);
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
