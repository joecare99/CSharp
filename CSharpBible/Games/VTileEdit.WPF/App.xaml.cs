using BaseLib.Helper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;
using System.Windows;
using VTileEdit.Models;
using VTileEdit.ViewModels;
using VTileEdit.WPF.ViewModels;

namespace VTileEdit.WPF;

/// <summary>
/// Interaction logic for App.xaml.
/// </summary>
public partial class App : Application
{
    private static IServiceProvider? _serviceProvider;

    /// <summary>
    /// Handles application startup and configures the dependency injection container.
    /// </summary>
    /// <param name="e">The startup event data.</param>
    protected override void OnStartup(StartupEventArgs e)
    {
        _serviceProvider ??= ConfigureServices(this);
        base.OnStartup(e);
    }

    /// <summary>
    /// Builds and configures the dependency injection container for the supplied application instance.
    /// </summary>
    /// <param name="appInstance">The current application instance.</param>
    /// <returns>The fully configured service provider.</returns>
    /// <remarks>
    /// Registers the supplied <see cref="Application"/> instance, builds the service provider, and wires it into the shared IoC helper.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="appInstance"/> is <c>null</c>.</exception>
    public static IServiceProvider ConfigureServices(App appInstance)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        ArgumentNullException.ThrowIfNull(appInstance);

        var services = new ServiceCollection();
        services.AddSingleton<Application>(_ => appInstance);
        services.AddSingleton<App>(_ => appInstance);
        services.AddSingleton<IVTEModel,VTEModel>();
        services.AddTransient<IVTEViewModel,VTEViewModel>();
        services.AddTransient<MainWindowViewModel>();

        var provider = services.BuildServiceProvider();
        IoC.Configure(provider);
        _serviceProvider = provider;

        return provider;
    }
}