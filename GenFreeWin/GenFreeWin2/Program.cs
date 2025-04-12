using System;
using System.Windows.Forms;
using BaseLib.Helper;
using Microsoft.Extensions.DependencyInjection;
using Gen_FreeWin.ViewModels;
using Gen_FreeWin.ViewModels.Interfaces;
using Gen_FreeWin.Views;
using GenFreeWin.ViewModels;
using GenFree.Interfaces.UI;
using GenFree.Views;

namespace GenFreeWin2
{
    public static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
#if NET5_0_OR_GREATER
            ApplicationConfiguration.Initialize();
#endif
            Init();
            Application.Run(IoC.GetRequiredService<Menue>());
        }

        public static void Init()
        {
            var sp = new ServiceCollection()
                .AddSingleton<IApplUserTexts, ApplUserTexts>()
                .AddSingleton<IAdresseViewModel, AdresseViewModel>()
                .AddSingleton<IMenu1ViewModel, MenueViewModel>()
                .AddTransient<IFraStatisticsViewModel, FraStatisticsViewModel>()
                .AddSingleton<Menue>()
                .AddSingleton<Adresse>()
                .BuildServiceProvider();

            IoC.Configure(sp);
        }
    }
}