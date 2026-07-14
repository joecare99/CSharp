using BaseLib.Helper;
using CommunityToolkit.Mvvm.Messaging;
using GenFree.Interfaces.UI;
using GenFree.ViewModels.Interfaces;
using GenFree.Views;
using GenFreeWin.ViewModels;
using GenFreeWin.Views;
using GenFreeWin2.ViewModels;
using GenFreeWin2.ViewModels.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Forms;

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
            Application.Run(IoC.GetRequiredService<MainForm>());
        }

        public static void Init()
        {
            var sp = new ServiceCollection()
                .AddSingleton<IApplUserTexts, ApplUserTexts>()
                .AddSingleton<IAdresseViewModel, AdresseViewModel>()
                .AddSingleton<IMessenger>((s) => WeakReferenceMessenger.Default)
                .AddSingleton<IMenu1ViewModel, MenueViewModel>()
                .AddTransient<IFraStatisticsViewModel, FraStatisticsViewModel>()
                .AddTransient<IMainFormViewModel, MainFormViewModel>()
                .AddTransient<ILizenzViewModel, LizenzViewModel>()
                .AddTransient<IFraPersImpQueryViewModel, FraPersImpQuerryViewModel>()
                .AddTransient<IPersonenViewModel, PersonenViewModel>()
                .AddTransient<IShowDlgMsg, ShowDlgMsg>()
                .AddTransient<IShowFrmMsg, ShowFrmMsg>()
                .AddSingleton<MainForm>()
                .AddTransient<Lizenz>()
                .AddTransient<Menue>()
                .AddTransient<Adresse>()
                .BuildServiceProvider();

            IoC.Configure(sp);
        }
    }
}