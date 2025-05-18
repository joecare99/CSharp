using System;
using System.Windows.Forms;
using BaseLib.Helper;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.DependencyInjection;
using GenFree.Interfaces.UI;
using GenFreeWin2.ViewModels.Interfaces;
using GenFree.ViewModels.Interfaces;
using GenFreeWin2.ViewModels;
using GenFreeWin.ViewModels;
using GenFreeWin.Views;
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
            Application.Run(IoC.GetRequiredService<MainForm>());
        }

        public static void Init()
        {
            var sp = new ServiceCollection()
                .AddSingleton<IApplUserTexts, ApplUserTexts>()
                .AddSingleton<IAdresseViewModel, AdresseViewModel>()
                .AddSingleton<IMessenger>((s)=>WeakReferenceMessenger.Default)
                .AddSingleton<IMenu1ViewModel, MenueViewModel>()
                .AddTransient<IFraStatisticsViewModel, FraStatisticsViewModel>()
                .AddTransient<IMainFormViewModel,MainFormViewModel>()
                .AddTransient<ILizenzViewModel,LizenzViewModel>()
                .AddTransient<IFraPersImpQuerryViewModel, FraPersImpQuerryViewModel>()
                .AddTransient<IPersonenViewModel, PersonenViewModel>()
                .AddTransient<IShowDlgMsg,ShowDlgMsg>()
                .AddTransient<IShowFrmMsg,ShowFrmMsg>()
                .AddSingleton<MainForm>()
                .AddTransient<Lizenz>()
                .AddTransient<Menue>()
                .AddTransient<Adresse>()
                .BuildServiceProvider();

            IoC.Configure(sp);
        }
    }
}