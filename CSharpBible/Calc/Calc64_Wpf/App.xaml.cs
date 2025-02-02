using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using BaseLib.Helper;
using System;
using Calc64Base.Models.Interfaces;
using Calc64Base.Models;
using BaseLib.Helper;


namespace Calc64_Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            IServiceCollection BaseServices = new ServiceCollection()
                .AddSingleton<ICalculator, Calc64Model>();
                
            IServiceProvider serviceProvider = BaseServices.BuildServiceProvider();

            IoC.Configure(serviceProvider);

        }
    }

}
