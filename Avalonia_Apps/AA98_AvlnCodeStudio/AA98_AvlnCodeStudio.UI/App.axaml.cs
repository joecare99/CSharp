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
using Config.UI;
using Config.UI.ConfigService;
using Config.UI.ConfigService.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.IO;
using Project.Explorer;
using Project.Explorer.Avalonia.DependencyInjection;
using Project.Explorer.Avalonia.ViewModels;
using Project.Explorer.FileSystem;
using System;
using AA98_AvlnCodeStudio.Config.Secrets;
using Property.Editor;

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

        public static ServiceProvider CreateServiceProvider(
            Func<TopLevel?> topLevelProvider,
            string? userConfigRoot = null,
            string? systemConfigRoot = null,
            string? groupConfigRoot = null,
            string? groupId = null)
        {
            ArgumentNullException.ThrowIfNull(topLevelProvider);

            var services = new ServiceCollection();
            ConfigureServices(services, topLevelProvider, userConfigRoot, systemConfigRoot, groupConfigRoot, groupId);

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
            Func<TopLevel?> topLevelProvider,
            string? userConfigRoot = null,
            string? systemConfigRoot = null,
            string? groupConfigRoot = null,
            string? groupId = null)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(topLevelProvider);

            string? configuredGroupRoot = groupConfigRoot ?? Environment.GetEnvironmentVariable("AVLN_CONFIG_GROUP_ROOT");
            string? configuredGroupId = groupId ?? Environment.GetEnvironmentVariable("AVLN_CONFIG_GROUP_ID");
            if (string.IsNullOrWhiteSpace(configuredGroupRoot) != string.IsNullOrWhiteSpace(configuredGroupId))
            {
                throw new InvalidOperationException(
                    "Group configuration requires both AVLN_CONFIG_GROUP_ROOT and AVLN_CONFIG_GROUP_ID.");
            }

            services.AddAvaloniaCommonDialogs(topLevelProvider);
            services.AddCodeStudioFoundation();
            services
                .AddConfigService("JC-Soft", "AA98-CodeStudio")
                .AddConfigSection<Configuration.CodeStudioSettings>(new Configuration.CodeStudioSettingsProvider())
                .AddConfigUiAvalonia();
            services.AddSingleton(serviceProvider =>
            {
                string? configuredRoot = userConfigRoot ?? Environment.GetEnvironmentVariable("CONFIG_ROOT");
                ConfigLayerStorePathResolver pathResolver = new(
                    "JC-Soft",
                    "AA98-CodeStudio",
                    userRoot: configuredRoot);
                return pathResolver.CreateUserStore();
            });
            services.AddSingleton<ConfigLayerStorePathResolver>(serviceProvider =>
                new ConfigLayerStorePathResolver(
                    "JC-Soft",
                    "AA98-CodeStudio",
                    userRoot: userConfigRoot ?? Environment.GetEnvironmentVariable("CONFIG_ROOT"),
                    systemRoot: systemConfigRoot,
                    groupRoot: configuredGroupRoot));
            services.AddSingleton<IConfigLayerStore>(serviceProvider =>
                new JsonConfigLayerStore(
                    ConfigLayer.System,
                    serviceProvider.GetRequiredService<ConfigLayerStorePathResolver>().ResolveSystemPath()));
            if (!string.IsNullOrWhiteSpace(configuredGroupId))
            {
                services.AddSingleton<IConfigLayerStore>(serviceProvider =>
                    new JsonConfigLayerStore(
                        ConfigLayer.Group,
                        serviceProvider.GetRequiredService<ConfigLayerStorePathResolver>().ResolveGroupPath(
                            configuredGroupId!)));
            }
            services.AddSingleton<ConfigLayeredSettingsResolver>(serviceProvider =>
                new ConfigLayeredSettingsResolver(
                    serviceProvider.GetRequiredService<IConfigSectionRegistry>(),
                    [
                        serviceProvider.GetRequiredService<JsonConfigLayerStore>(),
                        ..serviceProvider.GetServices<IConfigLayerStore>(),
                    ],
                    trustEnforcedSystemValues: false));
            services.AddSingleton<IConfigUiSectionRegistry>(serviceProvider =>
                new ConfigLayeredUiSectionRegistry(
                    serviceProvider.GetRequiredService<IConfigSectionRegistry>(),
                    serviceProvider.GetRequiredService<ConfigLayeredSettingsResolver>(),
                    serviceProvider.GetRequiredService<JsonConfigLayerStore>(),
                    ConfigLayer.User,
                    storageLocation: serviceProvider.GetRequiredService<JsonConfigLayerStore>().FilePath,
                    optionsProvider: static (section, property) =>
                    {
                        if (!string.Equals(section.Name, "Workbench", StringComparison.Ordinal)
                            || !string.Equals(property.Name, nameof(Configuration.CodeStudioSettings.CredentialManager), StringComparison.Ordinal))
                        {
                            return null;
                        }

                        List<PropertyOption> options =
                        [
                            new(Configuration.CredentialManagerSelection.None, "None (session only)"),
                            new(Configuration.CredentialManagerSelection.EncryptedFileVault, "Encrypted local vault (passphrase)"),
                        ];
                        if (OperatingSystem.IsWindows())
                        {
                            options.Add(new(Configuration.CredentialManagerSelection.WindowsCredentialManager, "Windows Credential Manager"));
                        }
                        else if (OperatingSystem.IsLinux())
                        {
                            options.Add(new(Configuration.CredentialManagerSelection.LinuxSecretService, "Linux Secret Service"));
                        }

                        return options;
                    }));
            services.AddSingleton(serviceProvider =>
            {
                IConfigSectionRegistry sectionRegistry = serviceProvider.GetRequiredService<IConfigSectionRegistry>();
                JsonConfigLayerStore userStore = serviceProvider.GetRequiredService<JsonConfigLayerStore>();
                ConfigLayerLegacyJsonMigrator migrator = new(sectionRegistry);
                string legacyPath = ResolveLegacyWorkbenchSettingsPath(userConfigRoot);
                migrator.MigrateSectionToUserLayerAsync(legacyPath, "Workbench", userStore)
                    .GetAwaiter()
                    .GetResult();

                IReadOnlyDictionary<string, ConfigValueResolution> resolved =
                    serviceProvider.GetRequiredService<ConfigLayeredSettingsResolver>()
                        .ResolveSectionAsync("Workbench")
                        .GetAwaiter()
                        .GetResult();
                Configuration.CodeStudioSettings settings = new();
                foreach (System.Reflection.PropertyInfo property in
                    typeof(Configuration.CodeStudioSettings).GetProperties(
                        System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
                {
                    if (property.CanWrite
                        && property.GetIndexParameters().Length == 0
                        && resolved.TryGetValue(property.Name, out ConfigValueResolution? value))
                    {
                        property.SetValue(settings, value.Value);
                    }
                }

                return settings;
            });
            services.AddSingleton(serviceProvider =>
            {
                Configuration.CodeStudioSettings settings =
                    serviceProvider.GetRequiredService<Configuration.CodeStudioSettings>();
                return new AA98_AvlnCodeStudio.Base.AI.Models.OpenAICompatibleAISettings
                {
                    Endpoint = settings.GetEffectiveAIEndpoint(),
                    ModelId = settings.GetEffectiveAIModelId(),
                    ApiKey = string.Empty,
                };
            });
            services.AddSingleton(serviceProvider => new EncryptedFileCredentialProvider(
                ResolveCredentialVaultPath(userConfigRoot)));
            services.AddSingleton(serviceProvider => new CredentialProviderCatalog(
            [
                new WindowsCredentialProvider(),
                new LinuxSecretServiceProvider(),
                serviceProvider.GetRequiredService<EncryptedFileCredentialProvider>(),
            ]));
            services.AddSingleton<Configuration.WorkbenchSettingsRuntimeActivator>();
            services.AddSingleton<IFileEditorDocumentFactory, FileEditorDocumentFactory>();
            services.AddSingleton<Services.IWorkspaceFileDialogService, Services.AvaloniaWorkspaceFileDialogService>();
            services.AddSingleton<AA98_AvlnCodeStudio.Model.Workspaces.CSharpProjectTemplateService>();
            services.AddSingleton<AA98_AvlnCodeStudio.Model.Workspaces.ICSharpProjectPackageService,
                AA98_AvlnCodeStudio.Model.Workspaces.DotNetCSharpProjectPackageService>();
            services.AddSingleton<AA98_AvlnCodeStudio.Model.Workspaces.AxamlPreviewService>();
            services.AddSingleton<AA98_AvlnCodeStudio.Model.Workspaces.AxamlMvvmGenerator>();
            services.AddSingleton<AA98_AvlnCodeStudio.AxamlDesigner.UI.ViewModels.AxamlDesignerViewModel>(serviceProvider =>
                new AA98_AvlnCodeStudio.AxamlDesigner.UI.ViewModels.AxamlDesignerViewModel(
                    serviceProvider.GetRequiredService<AA98_AvlnCodeStudio.Model.Workspaces.AxamlPreviewService>()));
            services.AddSingleton<UI.Services.CSharpSyntaxDiagnosticService>();
            services.AddSingleton<IEditorWorkflowFactory, EditorWorkflowFactory>();
            services.AddSingleton<UI.ViewModels.IEditorViewModelFactory, UI.ViewModels.EditorViewModelFactory>();
            services.AddSingleton<UI.Services.IEditorViewFactory, UI.Services.AvaloniaEditorViewFactory>();
            services.AddSingleton<UI.Components.AvaloniaEditorComponentFactory>();
            services.AddSingleton<UI.Components.IAvaloniaEditorComponentFactory>(serviceProvider => serviceProvider.GetRequiredService<UI.Components.AvaloniaEditorComponentFactory>());
            services.AddSingleton<IEditorComponentFactory>(serviceProvider => serviceProvider.GetRequiredService<UI.Components.AvaloniaEditorComponentFactory>());
            services.AddSingleton<UI.Components.IAvaloniaEditorComponent>(serviceProvider => serviceProvider.GetRequiredService<UI.Components.IAvaloniaEditorComponentFactory>().Create());
            services.AddSingleton<UI.ViewModels.EditorDocumentWorkspace>(serviceProvider => new UI.ViewModels.EditorDocumentWorkspace(
                serviceProvider.GetRequiredService<UI.Components.IAvaloniaEditorComponent>(),
                serviceProvider.GetRequiredService<UI.Components.IAvaloniaEditorComponentFactory>(),
                serviceProvider.GetServices<IDocumentEditorFactory<EditorTabViewModel>>()));
            services.AddSingleton<AA98_AvlnCodeStudio.Model.Workspaces.IWorkspaceOpenDocumentProvider>(
                serviceProvider => serviceProvider.GetRequiredService<UI.ViewModels.EditorDocumentWorkspace>());
            services.AddSingleton<AA98_AvlnCodeStudio.Model.Workspaces.WorkspaceFileChangeService>();
            services.AddSingleton<Func<UI.ViewModels.EditorDocumentWorkspace>>(serviceProvider =>
                () => serviceProvider.GetRequiredService<UI.ViewModels.EditorDocumentWorkspace>());
            services.AddSingleton<IEditorDocumentHost, WorkbenchEditorDocumentHost>();
            services.AddSingleton<IEditorCaretService, WorkbenchEditorCaretService>();
            services.AddSingleton<ICodeNavigator, EditorCodeNavigator>();
            services.AddSingleton<IProjectExplorerSource>(
                new FileSystemProjectExplorerSource());
            services.AddSingleton<IProjectExplorerState, ProjectExplorerState>();
            services.AddSingleton<IProjectExplorerItemOpener>(serviceProvider =>
                new CodeNavigatorProjectExplorerItemOpener(
                    serviceProvider.GetRequiredService<ICodeNavigator>(),
                    serviceProvider.GetRequiredService<Func<UI.ViewModels.EditorDocumentWorkspace>>()));
            services.AddProjectExplorerAvalonia();
            services.AddDiagnosticsUi();
            services.AddSingleton<MainWindowViewModel>(serviceProvider => new MainWindowViewModel(
                serviceProvider.GetRequiredService<UI.Components.IAvaloniaEditorComponent>(),
                serviceProvider.GetRequiredService<UI.ViewModels.PlanningExplorerViewModel>(),
                serviceProvider.GetRequiredService<AA98_AvlnCodeStudio.Diagnostics.UI.ViewModels.DiagnosticCollectionViewModel>(),
                serviceProvider.GetRequiredService<ConfigUiViewModel>(),
                serviceProvider.GetRequiredService<ProjectExplorerViewModel>(),
                serviceProvider.GetRequiredService<AA98_AvlnCodeStudio.Model.Workspaces.CSharpWorkspaceLoader>(),
                serviceProvider.GetRequiredService<Services.IWorkspaceFileDialogService>(),
                serviceProvider.GetRequiredService<AA98_AvlnCodeStudio.Base.Building.Services.ICodeStudioBuilderService>(),
                serviceProvider.GetRequiredService<AA98_AvlnCodeStudio.Base.Versioning.Services.IVersionControlService>(),
                serviceProvider.GetRequiredService<UI.ViewModels.EditorDocumentWorkspace>(),
                serviceProvider.GetRequiredService<AA98_AvlnCodeStudio.Base.UI.Services.IEditorFileDialogService>(),
                serviceProvider.GetRequiredService<UI.Services.CSharpSyntaxDiagnosticService>(),
                serviceProvider.GetRequiredService<AA98_AvlnCodeStudio.Base.Testing.Services.ITestExecutionService>(),
                serviceProvider.GetRequiredService<AA98_AvlnCodeStudio.Base.Debugging.Services.IDebugSessionService>(),
                serviceProvider.GetRequiredService<AA98_AvlnCodeStudio.Model.Workspaces.CSharpProjectTemplateService>(),
                serviceProvider.GetRequiredService<AA98_AvlnCodeStudio.Model.Workspaces.ICSharpProjectPackageService>(),
                serviceProvider.GetRequiredService<AA98_AvlnCodeStudio.AxamlDesigner.UI.ViewModels.AxamlDesignerViewModel>(),
                serviceProvider.GetRequiredService<AA98_AvlnCodeStudio.Base.AI.Services.AIChatService>(),
                serviceProvider.GetRequiredService<AA98_AvlnCodeStudio.Base.AI.Models.OpenAICompatibleAISettings>(),
                serviceProvider.GetRequiredService<Configuration.CodeStudioSettings>(),
                serviceProvider.GetRequiredService<global::Config.Service.ConfigService>(),
                serviceProvider.GetRequiredService<AA98_AvlnCodeStudio.Versioning.Svn.Services.ISvnWorkingCopyOperations>(),
                serviceProvider.GetRequiredService<AA98_AvlnCodeStudio.Model.Workspaces.WorkspaceFileChangeService>(),
                serviceProvider.GetRequiredService<AA98_AvlnCodeStudio.Model.Workspaces.AxamlMvvmGenerator>(),
                serviceProvider.GetRequiredService<Configuration.WorkbenchSettingsRuntimeActivator>(),
                serviceProvider.GetRequiredService<CredentialProviderCatalog>(),
                testDiscoveryService: serviceProvider.GetRequiredService<AA98_AvlnCodeStudio.Base.Testing.Services.ITestDiscoveryService>(),
                testCaseExecutionService: serviceProvider.GetRequiredService<AA98_AvlnCodeStudio.Base.Testing.Services.ITestCaseExecutionService>()));
            services.AddSingleton<MainWindow>(serviceProvider =>
                new MainWindow(
                    serviceProvider.GetRequiredService<MainWindowViewModel>(),
                    serviceProvider));
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

        private static string ResolveCredentialVaultPath(string? userConfigRoot)
        {
            string? configuredRoot = userConfigRoot ?? Environment.GetEnvironmentVariable("CONFIG_ROOT");
            string root = !string.IsNullOrWhiteSpace(configuredRoot)
                ? configuredRoot
                : Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (string.IsNullOrWhiteSpace(root))
            {
                throw new InvalidOperationException("The local application-data directory is unavailable for the credential vault.");
            }

            return Path.Combine(root, "JC-Soft", "AA98-CodeStudio", "secrets", "vault.json");
        }

        private static string ResolveLegacyWorkbenchSettingsPath(string? userConfigRoot)
        {
            string? configuredRoot = userConfigRoot ?? Environment.GetEnvironmentVariable("CONFIG_ROOT");
            string root = !string.IsNullOrEmpty(configuredRoot)
                ? configuredRoot
                : Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(root, "JC-Soft", "AA98-CodeStudio", "config", "Workbench.json");
        }
    }
}