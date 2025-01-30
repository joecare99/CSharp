using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using MVVM.View.Extension;
using System;


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
