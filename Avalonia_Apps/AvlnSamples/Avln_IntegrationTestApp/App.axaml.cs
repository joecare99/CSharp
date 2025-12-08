using System.Windows.Input;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Input;
using IntegrationTestApp.Models;
using IntegrationTestApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IntegrationTestApp
{
    public class App : Application
    {
        private MainWindow? _mainWindow;
        private IHost? _host;

        public App()
        {
            TrayIconCommand = new RelayCommand<string>(name =>
            {
                _mainWindow!.Get<CheckBox>(name).IsChecked = true;
            });
            DataContext = this;
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            _host = Host.CreateDefaultBuilder()
                       .ConfigureServices(services =>
                       {
                           services.AddSingleton<IPageProvider, DefaultPageProvider>();
                           services.AddTransient<MainWindowViewModel>(sp =>
                               new MainWindowViewModel(sp.GetRequiredService<IPageProvider>().GetPages()));
                           // Register MainWindow via factory to set DataContext from DI
                           services.AddTransient<MainWindow>(sp =>
                           {
                               var w = new MainWindow();
                               w.DataContext = sp.GetRequiredService<MainWindowViewModel>();
                               return w;
                           });
                       })
                       .Build();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Resolve MainWindow from DI (ViewModel injected via factory)
                var window = _host.Services.GetRequiredService<MainWindow>();
                desktop.MainWindow = _mainWindow = window;
            }

            base.OnFrameworkInitializationCompleted();
        }

        public ICommand TrayIconCommand { get; }
    }
}
