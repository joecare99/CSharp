using System;
using System.IO;
using System.Windows;
using BaseGenClasses.Helper;
using BaseGenClasses.Helper.Interfaces;
using BaseLib.Helper;
using CommunityToolkit.Mvvm.Messaging;
using GenSecure.Contracts;
using GenSecure.Core;
using Microsoft.Extensions.DependencyInjection;
using WinAhnenNew.Services;
using WinAhnenNew.ViewModels;

namespace WinAhnenNew
{
    /// <summary>
    /// Interaction logic for App.xaml.
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
            services.AddSingleton<IGenealogyModelFactory, BaseGenGenealogyModelFactory>();
            services.AddSingleton<IPersonSelectionService, PersonSelectionService>();
            services.AddTransient<IGenILBuilder, GenILBuilder>();
            services.AddTransient<FrmAhnenWinMainViewModel>();
            services.AddTransient<SelectionPageViewModel>();
            services.AddTransient<EditPageViewModel>();
            services.AddGenSecureStore(options =>
            {
                options.RootDirectory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "JC-Soft",
                    "WinAhnenNew");
            });

            Services = services.BuildServiceProvider();
            IoC.Configure(Services);
        }

        public IServiceProvider Services { get; }
    }

}
