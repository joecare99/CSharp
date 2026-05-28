using AA98_AvlnCodeStudio.UI.ViewModels;
using AA98_AvlnCodeStudio.UI.Views;
using AA98_AvlnCodeStudio.Base.Services;
using AA98_AvlnCodeStudio.Model.Documents;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avln_CommonDialogs.Avalonia;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AA98_AvlnCodeStudio.UI
{
    public partial class App : Application
    {
        /// <summary>
        /// Gets the application service provider.
        /// </summary>
        public IServiceProvider? ServiceProvider { get; private set; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                ServiceProvider = ConfigureServices(desktop);
                desktop.MainWindow = CreateMainWindow(ServiceProvider);
            }

            base.OnFrameworkInitializationCompleted();
        }

        private static IServiceProvider ConfigureServices(IClassicDesktopStyleApplicationLifetime desktop)
        {
            var services = new ServiceCollection();

            services.AddAvaloniaCommonDialogs(() => desktop.MainWindow);
            services.AddSingleton<FileEditorDocument>();
            services.AddSingleton<ITextDocumentStorageService, UI.Services.FileSystemTextDocumentStorageService>();
            services.AddSingleton<IEditorFileDialogService, UI.Services.AvaloniaEditorFileDialogService>();
            services.AddSingleton<EditorViewModel>();
            services.AddSingleton<MainWindowViewModel>();

            return services.BuildServiceProvider();
        }

        private static MainWindow CreateMainWindow(IServiceProvider serviceProvider)
        {
            return new MainWindow
            {
                DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>(),
            };
        }
    }
}