using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace BoxFlight;

public partial class App : Application
{
    private ServiceProvider? _provider;

    protected override void OnStartup(StartupEventArgs e)
    {
        var services = new ServiceCollection();
        services.AddSingleton<ViewModels.IBoxFlightViewModel, ViewModels.BoxFlightViewModel>();
        services.AddSingleton<Views.BoxFlightWindow>();
        _provider = services.BuildServiceProvider();
        _provider.GetRequiredService<Views.BoxFlightWindow>().Show();
    }
}