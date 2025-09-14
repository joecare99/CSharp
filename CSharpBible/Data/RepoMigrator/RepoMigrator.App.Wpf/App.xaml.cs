// RepoMigrator.App.Wpf/App.xaml.cs
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace RepoMigrator.App.Wpf;

public partial class App : Application
{
    public static ServiceProvider Services { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        Services = Bootstrap.Create();
        var win = new MainWindow
        {
            DataContext = Services.GetRequiredService<ViewModels.MainViewModel>()
        };
        win.Show();
    }
}
