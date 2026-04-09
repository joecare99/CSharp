using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WpfApp.Services;
using WpfApp.ViewModels;
using WpfApp.Views;

namespace WpfApp;

/// <summary>
/// Application entry point.
/// </summary>
public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    /// <inheritdoc />
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var oServices = new ServiceCollection();
        oServices.AddSingleton<ITextDocumentService, TextDocumentService>();
        oServices.AddSingleton<IFileDialogService, FileDialogService>();
        oServices.AddSingleton<MainWindowViewModel>();
        oServices.AddSingleton<Frm_W2kMain>();

        _serviceProvider = oServices.BuildServiceProvider();

        var oMainWindow = _serviceProvider.GetRequiredService<Frm_W2kMain>();
        oMainWindow.Show();
    }

    /// <inheritdoc />
    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}
