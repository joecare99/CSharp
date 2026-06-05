using AA98_AvlnCodeStudio.UI.ViewModels;
using AA98_AvlnCodeStudio.UI.Views;
using AA98_AvlnCodeStudio.Editor.Components;
using AA98_AvlnCodeStudio.Editor.Services;
using AA98_AvlnCodeStudio.Model.Documents;
using AA98_AvlnCodeStudio.UI.DependencyInjection;
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
            services.AddCodeStudioFoundation();
            services.AddSingleton<IFileEditorDocumentFactory, FileEditorDocumentFactory>();
            services.AddSingleton<IEditorWorkflowFactory, EditorWorkflowFactory>();
            services.AddSingleton<UI.ViewModels.IEditorViewModelFactory, UI.ViewModels.EditorViewModelFactory>();
            services.AddSingleton<UI.Services.IEditorViewFactory, UI.Services.AvaloniaEditorViewFactory>();
            services.AddSingleton<UI.Components.AvaloniaEditorComponentFactory>();
            services.AddSingleton<UI.Components.IAvaloniaEditorComponentFactory>(serviceProvider => serviceProvider.GetRequiredService<UI.Components.AvaloniaEditorComponentFactory>());
            services.AddSingleton<IEditorComponentFactory>(serviceProvider => serviceProvider.GetRequiredService<UI.Components.AvaloniaEditorComponentFactory>());
            services.AddSingleton<UI.Components.IAvaloniaEditorComponent>(serviceProvider => serviceProvider.GetRequiredService<UI.Components.IAvaloniaEditorComponentFactory>().Create());
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<MainWindow>();

            return services.BuildServiceProvider();
        }

        private static MainWindow CreateMainWindow(IServiceProvider serviceProvider)
        {
            return serviceProvider.GetRequiredService<MainWindow>();
        }
    }
}