using BaseGenClasses.Helper;
using BaseGenClasses.Helper.Interfaces;
using BaseLib.Helper;
using CommunityToolkit.Mvvm.Messaging;
using GenSecure.Contracts;
using GenSecure.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Windows;
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
            services.AddSingleton<IApplicationShutdownService, WpfApplicationShutdownService>();
            services.AddTransient<IGenFactBuilder, GenFactBuilder>();
            services.AddTransient<IGenILBuilder, GenILBuilder>();
            services.AddTransient<FrmAhnenWinMainViewModel>();
            services.AddTransient<SelectionPageViewModel>();
            services.AddTransient<EditPageViewModel>();
            services.AddTransient<DetailPageViewModel>();
            services.AddTransient<RelationshipsPageViewModel>();
            services.AddTransient<SiblingsPageViewModel>();
            services.AddTransient<TextPageViewModel>();
            services.AddTransient<AdditionalPageViewModel>();
            services.AddTransient<AddressPageViewModel>();
            services.AddTransient<ImagesPageViewModel>();
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
