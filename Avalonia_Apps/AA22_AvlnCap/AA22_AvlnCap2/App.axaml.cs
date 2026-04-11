using AA22_AvlnCap2.ViewModels;
using AA22_AvlnCap2.Views;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Views.Extension;
using BaseLib.Models;
using BaseLib.Models.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using AA22_AvlnCap2.Model;
using AA22_AvlnCap2.ViewModels.Factories;
using AA22_AvlnCap2.ViewModels.Interfaces;

namespace AA22_AvlnCap2
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var services = new ServiceCollection();
            services.AddTransient<IWpfCapModel, CWpfCapModel>();
            services.AddTransient<IRandom, CRandom>();
            services.AddTransient<IWpfCapViewModel, WpfCapViewModel>();
            services.AddTransient<IRowDataFactory, RowDataFactory>();
            services.AddTransient<IColDataFactory, ColDataFactory>();
            services.AddTransient<WpfCapView>();

            ServiceProvider container = services.BuildServiceProvider();
            IoC.GetReqSrv = (type) => container.GetRequiredService(type);
            IoC.GetSrv = (type) => container.GetService(type);

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
                // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

    }
}