using BaseLib.Helper;
using BaseLib.Models;
using BaseLib.Models.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;

namespace Cifar10.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var sc = new ServiceCollection()
                .AddTransient<IRandom, CRandom>();

            var sp = sc.BuildServiceProvider();
            IoC.Configure(sp);
            base.OnStartup(e);
        }

    }

}
