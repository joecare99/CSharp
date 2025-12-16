using System.Configuration;
using System.Data;
using System.Windows;
using BaseLib.Models;
using BaseLib.Models.Interfaces;
using Game.Model;
using Game.Model.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Snake.WPF.Views;
using Snake_Base.Models;
using Snake_Base.Models.Interfaces;
using Snake_Base.ViewModels;

namespace Snake.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ServiceProvider Services { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var services = new ServiceCollection();
            ConfigureServices(services);
            Services = services.BuildServiceProvider();
            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Register core ViewModel from Snake_Base
            services.AddSingleton<ISnakeViewModel, SnakeViewModel>()
                .AddSingleton<IPlayfield2D<ISnakeGameObject>,Playfield2D<ISnakeGameObject>>()
                .AddSingleton<ISnakeGame,SnakeGame>()
                .AddTransient<IRandom,CRandom>()
                .AddTransient<MainWindow>();
        }
    }
}
