using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Controls;
using Avln_CommonDialogs.Avalonia;
using Microsoft.Extensions.DependencyInjection;

namespace AA20a_CommonDialogs;

public sealed partial class App : Application
{
    public IServiceProvider Services { get; private set; } = default!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Services = ConfigureServices();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = Services.GetRequiredService<MainWindow>();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        var topLevelAccessor = new TopLevelAccessor();
        services.AddSingleton(topLevelAccessor);

        services.AddSingleton<MainWindow>();
        services.AddSingleton<MainWindowViewModel>();

        services.AddAvaloniaCommonDialogs(() => topLevelAccessor.Current);

        return services.BuildServiceProvider();
    }
}

public sealed class TopLevelAccessor
{
    public TopLevel? Current { get; set; }
}
