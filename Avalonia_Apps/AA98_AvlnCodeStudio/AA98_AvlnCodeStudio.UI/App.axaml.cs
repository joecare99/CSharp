using AA98_AvlnCodeStudio.UI.ViewModels;
using AA98_AvlnCodeStudio.UI.Views;
using AA98_AvlnCodeStudio.Editor.Components;
using AA98_AvlnCodeStudio.Editor.Services;
using AA98_AvlnCodeStudio.Model.Documents;
using AA98_AvlnCodeStudio.UI.DependencyInjection;
using Avalonia;
using Avalonia.Controls;
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
                InitializeDesktop(desktop);
            }

            base.OnFrameworkInitializationCompleted();
        }

        public MainWindow InitializeDesktop(IClassicDesktopStyleApplicationLifetime desktop)
        {
            ArgumentNullException.ThrowIfNull(desktop);

            try
            {
                ServiceProvider = CreateServiceProvider(() => desktop.MainWindow);
                var mainWindow = CreateMainWindow(ServiceProvider);
                desktop.MainWindow = mainWindow;
                return mainWindow;
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException("AA98 shell startup failed during desktop initialization. Check desktop lifetime, service registrations, and platform dialog availability.", exception);
            }
        }

        public static ServiceProvider CreateServiceProvider(Func<TopLevel?> topLevelProvider)
        {
            ArgumentNullException.ThrowIfNull(topLevelProvider);

            var services = new ServiceCollection();

            services.AddAvaloniaCommonDialogs(topLevelProvider);
            services.AddCodeStudioFoundation();
            services.AddSingleton<IFileEditorDocumentFactory, FileEditorDocumentFactory>();
            services.AddSingleton<IEditorWorkflowFactory, EditorWorkflowFactory>();
            services.AddSingleton<UI.ViewModels.IEditorViewModelFactory, UI.ViewModels.EditorViewModelFactory>();
            services.AddSingleton<UI.Services.IEditorViewFactory, UI.Services.AvaloniaEditorViewFactory>();
            services.AddSingleton<UI.Components.AvaloniaEditorComponentFactory>();
            services.AddSingleton<UI.Components.IAvaloniaEditorComponentFactory>(serviceProvider => serviceProvider.GetRequiredService<UI.Components.AvaloniaEditorComponentFactory>());
            services.AddSingleton<IEditorComponentFactory>(serviceProvider => serviceProvider.GetRequiredService<UI.Components.AvaloniaEditorComponentFactory>());
            services.AddSingleton<UI.Components.IAvaloniaEditorComponent>(serviceProvider => serviceProvider.GetRequiredService<UI.Components.IAvaloniaEditorComponentFactory>().Create());
            services.AddSingleton<MainWindowViewModel>(serviceProvider => new MainWindowViewModel(
                serviceProvider.GetRequiredService<UI.Components.IAvaloniaEditorComponent>(),
                serviceProvider.GetRequiredService<UI.ViewModels.PlanningExplorerViewModel>()));
            services.AddSingleton<MainWindow>();

            return services.BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateOnBuild = true,
                ValidateScopes = true
            });
        }

        public static MainWindow CreateMainWindow(IServiceProvider serviceProvider)
        {
            ArgumentNullException.ThrowIfNull(serviceProvider);

            try
            {
                return serviceProvider.GetRequiredService<MainWindow>();
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException("AA98 main window creation failed. Check UI component and editor registrations.", exception);
            }
        }
    }
}