using BaseLib.Helper;
using CommunityToolkit.Mvvm.Messaging;
using GenFree.Interfaces.UI;
using GenFree.ViewModels.Interfaces;
using GenFreeWpf.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;

namespace GenFreeWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            IoC.Configure(serviceCollection.BuildServiceProvider());
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MainWindow>()
            .AddSingleton<IMessenger>((s) => WeakReferenceMessenger.Default)
            .AddSingleton<IApplUserTexts, GenFree.Views.ApplUserTexts>()
            .AddTransient<MainWindowViewModel>()
            .AddTransient<IMenu1ViewModel, GenFreeWin.ViewModels.MenueViewModel>()
            .AddTransient<IFraStatisticsViewModel, FraStatisticsViewModel>()
            .AddTransient<IAdresseViewModel, GenFreeWin.ViewModels.AdresseViewModel>()
            .AddTransient<ILizenzViewModel, GenFreeWin.ViewModels.LizenzViewModel>()
            .AddTransient<IFraPersImpQueryViewModel, GenFreeWin.ViewModels.FraPersImpQuerryViewModel>()
            .AddTransient<IPersonenViewModel, GenFreeWin.ViewModels.PersonenViewModel>()
              ;


        }

        /// <summary>
        /// Handles the startup event of the application.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Beispiel: Hauptfenster mit DI aufrufen
            // var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            // mainWindow.Show();
        }
    }

}
