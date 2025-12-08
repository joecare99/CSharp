using All_Graphics.Models;
using BaseLib.Helper;
using BaseLib.Models;
using BaseLib.Models.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace All_Graphics
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var sc = new ServiceCollection()
                .AddSingleton<ITemplateModel, AllExampleModel>()
                .AddTransient<ICyclTimer, TimerProxy>()
                .AddTransient<ISysTime, SysTime>();
            var sp = sc.BuildServiceProvider();

            IoC.Configure(sp);
            base.OnStartup(e);
        }
    }

}
