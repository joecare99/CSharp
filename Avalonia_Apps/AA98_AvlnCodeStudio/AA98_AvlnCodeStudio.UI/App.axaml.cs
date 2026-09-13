using AA98_AvlnCodeStudio.UI.ViewModels;
using AA98_AvlnCodeStudio.UI.Views;
using AA98_AvlnCodeStudio.Editor.Components;
using AA98_AvlnCodeStudio.Editor.Navigation;
using AA98_AvlnCodeStudio.Editor.Services;
using AA98_AvlnCodeStudio.Diagnostics.UI.DependencyInjection;
using AA98_AvlnCodeStudio.Model.Documents;
using AA98_AvlnCodeStudio.UI.DependencyInjection;
using AA98_AvlnCodeStudio.UI.Navigation;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avln_CommonDialogs.Avalonia;
using Code.Navigation;
using Config.Service;
using Config.UI.Avalonia.DependencyInjection;
using Config.UI.Avalonia.ViewModels;
using Config.UI.ConfigService.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Project.Explorer;
using Project.Explorer.Avalonia.DependencyInjection;
using Project.Explorer.Avalonia.ViewModels;
using Project.Explorer.FileSystem;
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
            ConfigureServices(services, topLevelProvider);

            return services.BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateOnBuild = true,
                ValidateScopes = true
            });
        }

        /// <summary>
        /// Registers all services required by the CodeStudio workbench.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        /// <param name="topLevelProvider">Provides the current Avalonia top-level window.</param>
        public static void ConfigureServices(
            IServiceCollection services,
            Func<TopLevel?> topLevelProvider)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(topLevelProvider);

            services.AddAvaloniaCommonDialogs(topLevelProvider);
            services.AddCodeStudioFoundation();
            services
                .AddConfigService("JC-Soft", "AA98-CodeStudio")
                .AddConfigSection<Configuration.CodeStudioSettings>(new Configuration.CodeStudioSettingsProvider())
                .AddConfigServiceUi()
                .AddConfigUiAvalonia();
            services.AddSingleton<IFileEditorDocumentFactory, FileEditorDocumentFactory>();
            services.AddSingleton<IEditorWorkflowFactory, EditorWorkflowFactory>();
            services.AddSingleton<UI.ViewModels.IEditorViewModelFactory, UI.ViewModels.EditorViewModelFactory>();
            services.AddSingleton<UI.Services.IEditorViewFactory, UI.Services.AvaloniaEditorViewFactory>();
            services.AddSingleton<UI.Components.AvaloniaEditorComponentFactory>();
            services.AddSingleton<UI.Components.IAvaloniaEditorComponentFactory>(serviceProvider => serviceProvider.GetRequiredService<UI.Components.AvaloniaEditorComponentFactory>());
            services.AddSingleton<IEditorComponentFactory>(serviceProvider => serviceProvider.GetRequiredService<UI.Components.AvaloniaEditorComponentFactory>());
            services.AddSingleton<UI.Components.IAvaloniaEditorComponent>(serviceProvider => serviceProvider.GetRequiredService<UI.Components.IAvaloniaEditorComponentFactory>().Create());
            services.AddSingleton<IEditorDocumentHost, WorkbenchEditorDocumentHost>();
            services.AddSingleton<IEditorCaretService, WorkbenchEditorCaretService>();
            services.AddSingleton<ICodeNavigator, EditorCodeNavigator>();
            services.AddSingleton<IProjectExplorerSource>(
                new FileSystemProjectExplorerSource([".cs", ".axaml", ".csproj", ".sln", ".md", ".json"]));
            services.AddSingleton<IProjectExplorerState, ProjectExplorerState>();
            services.AddSingleton<IProjectExplorerItemOpener, CodeNavigatorProjectExplorerItemOpener>();
            services.AddProjectExplorerAvalonia();
            services.AddDiagnosticsUi();
            services.AddSingleton<MainWindowViewModel>(serviceProvider => new MainWindowViewModel(
                serviceProvider.GetRequiredService<UI.Components.IAvaloniaEditorComponent>(),
                serviceProvider.GetRequiredService<UI.ViewModels.PlanningExplorerViewModel>(),
                serviceProvider.GetRequiredService<AA98_AvlnCodeStudio.Diagnostics.UI.ViewModels.DiagnosticCollectionViewModel>(),
                serviceProvider.GetRequiredService<ConfigUiViewModel>(),
                serviceProvider.GetRequiredService<ProjectExplorerViewModel>()));
            services.AddSingleton<MainWindow>();
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