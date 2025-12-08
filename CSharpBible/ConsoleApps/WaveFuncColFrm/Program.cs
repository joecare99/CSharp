using BaseLib.Interfaces;
using BaseLib.Models;
using BaseLib.Models.Interfaces;
using CommunityToolkit.Mvvm.DependencyInjection;
using WaveFunCollapse.Models.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using TestConsole;
using TestConsoleDemo.ViewModels;
using TestConsoleDemo.ViewModels.Interfaces;
using TestConsoleDemo.Views;
using WaveFunCollapse.Views;

namespace TestConsoleDemo
{
    /// <summary>
    /// Class Program.
    /// </summary>
    public static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Init();
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(Ioc.Default.GetRequiredService<Form>());
        }

        public static Action Init { get; set; } = static () =>
        {
            var sp = new ServiceCollection()
             .AddSingleton<IWFCModel, WaveFunCollapse.Models.WFCModel>()
             .AddTransient<IRandom, CRandom>()
             .AddSingleton<IConsole, TstConsole>()
             .AddTransient<ITextConsoleDemoViewModel, TextConsoleDemoViewModel>()
             .AddTransient<IView,WaveFunColView>()
             .AddTransient<Form, TextConsoleDemoForm>()
             .BuildServiceProvider();

            Ioc.Default.ConfigureServices(sp);
        };

    }
}