using AA98_AvlnCodeStudio.UI;
using AA98_AvlnCodeStudio.Base.Components.Commands;
using AA98_AvlnCodeStudio.Model.Workspaces;
using AA98_AvlnCodeStudio.UI.ViewModels;
using AA98_AvlnCodeStudio.UI.Services;
using AA98_AvlnCodeStudio.UI.Configuration;
using AA98_AvlnCodeStudio.Editor.Navigation;
using AA98_AvlnCodeStudio.Diagnostics.UI.ViewModels;
using AA98_AvlnCodeStudio.Base.Testing.Services;
using AA98_AvlnCodeStudio.Base.Debugging.Services;
using Avalonia.Controls.ApplicationLifetimes;
using Avln_CommonDialogs.Base.Interfaces;
using Code.Navigation;
using Config.Service;
using Config.UI;
using Config.UI.Avalonia.ViewModels;
using Config.UI.ConfigService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Project.Explorer;
using Project.Explorer.Avalonia.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Property.Editor;

namespace AA98_AvlnCodeStudio.Tests.Startup;

/// <summary>
/// Verifies the AA98 shell startup composition behavior.
/// </summary>
[TestClass]
public class AppStartupCompositionTests
{
    /// <summary>
    /// Verifies that the application service provider can be created without a realized desktop window.
    /// </summary>
    [TestMethod]
    public void CreateServiceProvider_RegistersCoreStartupServices()
    {
        using var serviceProvider = App.CreateServiceProvider(static () => null);

        var viewModelFactory = serviceProvider.GetRequiredService<AA98_AvlnCodeStudio.UI.ViewModels.IEditorViewModelFactory>();
        var workflowFactory = serviceProvider.GetRequiredService<AA98_AvlnCodeStudio.Editor.Services.IEditorWorkflowFactory>();
        var dialogService = serviceProvider.GetRequiredService<IOpenFileDialog>();
        var planningExplorer = serviceProvider.GetRequiredService<PlanningExplorerViewModel>();
        var documentHost = serviceProvider.GetRequiredService<IEditorDocumentHost>();
        var caretService = serviceProvider.GetRequiredService<IEditorCaretService>();
        var navigator = serviceProvider.GetRequiredService<ICodeNavigator>();
        var diagnostics = serviceProvider.GetRequiredService<DiagnosticCollectionViewModel>();
        var projectExplorer = serviceProvider.GetRequiredService<ProjectExplorerViewModel>();
        var commandCatalog = serviceProvider.GetRequiredService<IWorkbenchCommandCatalog>();
        var workspaceLoader = serviceProvider.GetRequiredService<CSharpWorkspaceLoader>();
        var projectTemplateService = serviceProvider.GetRequiredService<CSharpProjectTemplateService>();
        var workspaceFileDialog = serviceProvider.GetRequiredService<IWorkspaceFileDialogService>();
        var testExecutionService = serviceProvider.GetRequiredService<ITestExecutionService>();
        var debugSessionService = serviceProvider.GetRequiredService<IDebugSessionService>();
        CodeStudioSettings workbenchSettings = serviceProvider.GetRequiredService<CodeStudioSettings>();
        var aiSettings = serviceProvider.GetRequiredService<AA98_AvlnCodeStudio.Base.AI.Models.OpenAICompatibleAISettings>();
        AA98_AvlnCodeStudio.Config.Secrets.CredentialProviderCatalog credentialProviders =
            serviceProvider.GetRequiredService<AA98_AvlnCodeStudio.Config.Secrets.CredentialProviderCatalog>();

        Assert.IsNotNull(viewModelFactory);
        Assert.IsNotNull(workflowFactory);
        Assert.IsNotNull(dialogService);
        Assert.IsNotNull(planningExplorer);
        Assert.IsNotNull(documentHost);
        Assert.IsNotNull(caretService);
        Assert.IsNotNull(navigator);
        Assert.IsNotNull(projectExplorer);
        Assert.IsNotNull(commandCatalog);
        Assert.IsNotNull(workspaceLoader);
        Assert.IsNotNull(projectTemplateService);
        Assert.IsNotNull(workspaceFileDialog);
        Assert.IsInstanceOfType<DotNetTestExecutionService>(testExecutionService);
        Assert.IsInstanceOfType<NetCoreDbgDebugSessionService>(debugSessionService);
        Assert.AreEqual(workbenchSettings.GetEffectiveAIEndpoint(), aiSettings.Endpoint);
        Assert.AreEqual(workbenchSettings.GetEffectiveAIModelId(), aiSettings.ModelId);
        Assert.AreEqual(string.Empty, aiSettings.ApiKey);
        CollectionAssert.AreEquivalent(
            new[] { "linux-secret-service", "windows-credential-manager" },
            credentialProviders.ProviderIds.ToArray());
        Assert.ThrowsExactly<KeyNotFoundException>(() => credentialProviders.GetRequired("automatic-fallback"));
        diagnostics.SelectedDiagnostic = new AppKomponentBaseLib.Diagnostics.Diagnostic
        {
            SourcePath = Path.GetTempFileName(),
            LineNumber = 1,
            ColumnNumber = 1,
        };

        Assert.IsNotNull(diagnostics);
        Assert.IsTrue(diagnostics.IsSelectedDiagnosticNavigable);
    }

    [TestMethod]
    public async Task CreateServiceProvider_ProjectExplorerUsesCodeNavigationForFiles()
    {
        ICodeNavigator navigator = Substitute.For<ICodeNavigator>();
        ServiceCollection services = new();
        App.ConfigureServices(services, static () => null);
        services.AddSingleton(navigator);
        using ServiceProvider serviceProvider = services.BuildServiceProvider();
        IProjectExplorerItemOpener opener = serviceProvider.GetRequiredService<IProjectExplorerItemOpener>();
        ProjectExplorerItem file = new(
            "file:sample",
            "sample.cs",
            Path.GetTempFileName(),
            ProjectExplorerItemKind.File);

        await opener.OpenAsync(file);

        await navigator.Received(1).NavigateAsync(
            Arg.Is<CodeLocation>(location => location.Path == file.Path),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Verifies that CodeStudio registers only its host-owned section and a
    /// storage identity distinct from RnzTrauer.
    /// </summary>
    [TestMethod]
    public void CreateServiceProvider_RegistersIsolatedConfigUi()
    {
        string configRoot = Path.Combine(Path.GetTempPath(), "CodeStudioLayeredConfig", Guid.NewGuid().ToString("N"));
        try
        {
            string legacyDirectory = Path.Combine(configRoot, "JC-Soft", "AA98-CodeStudio", "config");
            Directory.CreateDirectory(legacyDirectory);
            string legacyPath = Path.Combine(legacyDirectory, "Workbench.json");
            const string legacySettings = """
                {
                  "aiEndpoint": "https://legacy.example.test/chat",
                  "aiModelId": "legacy-model",
                  "workspacePath": "C:\\legacy-workspace",
                  "reopenLastDocument": false
                }
                """;
            File.WriteAllText(legacyPath, legacySettings);

            using var serviceProvider = App.CreateServiceProvider(static () => null, configRoot);

            ConfigService configService = serviceProvider.GetRequiredService<ConfigService>();
            IConfigUiSectionRegistry registry = serviceProvider.GetRequiredService<IConfigUiSectionRegistry>();
            CodeStudioSettings settings = serviceProvider.GetRequiredService<CodeStudioSettings>();
            ConfigLayerSnapshot userLayer = serviceProvider.GetRequiredService<JsonConfigLayerStore>()
                .LoadAsync()
                .GetAwaiter()
                .GetResult();

            Assert.AreEqual("JC-Soft", configService.VendorName);
            Assert.AreEqual("AA98-CodeStudio", configService.ApplicationName);
            Assert.AreEqual(1, registry.Sections.Count);
            Assert.AreEqual("Workbench", registry.Sections[0].Section.Name);
            Assert.AreEqual(PropertyScope.User, registry.Sections[0].Context.Scope);
            Assert.IsTrue(registry.Sections[0].Context.StorageLocation!.EndsWith(
                Path.Combine("config", "layers", "user.json"),
                StringComparison.OrdinalIgnoreCase));
            Assert.AreEqual("https://legacy.example.test/chat", settings.AIEndpoint);
            Assert.AreEqual("legacy-model", settings.AIModelId);
            Assert.AreEqual("C:\\legacy-workspace", settings.WorkspacePath);
            Assert.IsFalse(settings.ReopenLastDocument);
            Assert.IsTrue(userLayer.TryGetValue(
                new ConfigPropertyKey("Workbench", nameof(CodeStudioSettings.AIModelId)),
                out _));
            Assert.AreEqual(legacySettings, File.ReadAllText(legacyPath));
            Assert.IsNotNull(serviceProvider.GetRequiredService<ConfigUiViewModel>());
        }
        finally
        {
            if (Directory.Exists(configRoot))
            {
                Directory.Delete(configRoot, recursive: true);
            }
        }
    }

    [TestMethod]
    public async Task CreateServiceProvider_ResolvesGroupAndSystemLayersWithoutChangingUserEditTarget()
    {
        string root = Path.Combine(Path.GetTempPath(), "CodeStudioLayerComposition", Guid.NewGuid().ToString("N"));
        string userRoot = Path.Combine(root, "user");
        string systemRoot = Path.Combine(root, "system");
        string groupRoot = Path.Combine(root, "groups");
        const string groupId = "Engineering";
        ConfigLayerStorePathResolver pathResolver = new(
            "JC-Soft",
            "AA98-CodeStudio",
            userRoot,
            systemRoot,
            groupRoot);
        string systemPath = pathResolver.ResolveSystemPath();
        string groupPath = pathResolver.ResolveGroupPath(groupId);
        Directory.CreateDirectory(Path.GetDirectoryName(systemPath)!);
        Directory.CreateDirectory(Path.GetDirectoryName(groupPath)!);

        try
        {
            JsonConfigLayerStore systemStore = new(ConfigLayer.System, systemPath);
            JsonConfigLayerStore groupStore = new(ConfigLayer.Group, groupPath);
            ConfigPropertyKey modelIdKey = new("Workbench", nameof(CodeStudioSettings.AIModelId));
            await systemStore.SaveAsync((await systemStore.LoadAsync()).WithValue(
                modelIdKey,
                new ConfigLayerValue("system-model", IsEnforced: true)));
            await groupStore.SaveAsync((await groupStore.LoadAsync()).WithValue(
                modelIdKey,
                new ConfigLayerValue("group-model")));

            using ServiceProvider serviceProvider = App.CreateServiceProvider(
                static () => null,
                userRoot,
                systemRoot,
                groupRoot,
                groupId);
            IConfigUiSectionRegistry registry = serviceProvider.GetRequiredService<IConfigUiSectionRegistry>();
            ConfigLayeredSettingsResolver resolver = serviceProvider.GetRequiredService<ConfigLayeredSettingsResolver>();
            CodeStudioSettings settings = serviceProvider.GetRequiredService<CodeStudioSettings>();
            JsonConfigLayerStore userStore = serviceProvider.GetRequiredService<JsonConfigLayerStore>();

            CollectionAssert.AreEquivalent(
                new[] { ConfigLayer.System, ConfigLayer.Group },
                serviceProvider.GetServices<IConfigLayerStore>()
                    .Select(static store => store.Layer)
                    .ToArray());
            Assert.AreEqual(PropertyScope.User, registry.Sections.Single().Context.Scope);
            Assert.AreEqual("group-model", settings.AIModelId);
            ConfigValueResolution groupResolution = await resolver.ResolveAsync(
                "Workbench",
                nameof(CodeStudioSettings.AIModelId));
            Assert.AreEqual(ConfigValueSource.Group, groupResolution.Source);

            IConfigUiSection workbenchSection = registry.Sections.Single();
            await workbenchSection.LoadAsync();
            Assert.IsTrue(((IPropertyProvider)workbenchSection).Context.StorageLocation!.EndsWith(
                Path.Combine("config", "layers", "user.json"),
                StringComparison.OrdinalIgnoreCase));
            IPropertyItem modelIdProperty = workbenchSection.Properties.Single(
                static property => property.Name == nameof(CodeStudioSettings.AIModelId));
            Assert.AreEqual(PropertyScope.Group, modelIdProperty.ValueContext.Scope);
            Assert.AreEqual("Group configuration", modelIdProperty.ValueContext.Origin);
            Assert.IsTrue(modelIdProperty.ValueContext.IsInherited);
            Assert.IsTrue(modelIdProperty.IsEditable);
            Assert.IsTrue(modelIdProperty.TrySetValue("user-model"));
            await workbenchSection.SaveAsync();

            ConfigValueResolution userResolution = await resolver.ResolveAsync(
                "Workbench",
                nameof(CodeStudioSettings.AIModelId));
            Assert.AreEqual("user-model", userResolution.Value);
            Assert.AreEqual(ConfigValueSource.User, userResolution.Source);

            await userStore.SaveAsync((await userStore.LoadAsync()).WithoutValue(modelIdKey));
            await groupStore.SaveAsync((await groupStore.LoadAsync()).WithoutValue(modelIdKey));

            ConfigValueResolution untrustedSystemResolution = await resolver.ResolveAsync(
                "Workbench",
                nameof(CodeStudioSettings.AIModelId));
            Assert.AreEqual("system-model", untrustedSystemResolution.Value);
            Assert.AreEqual(ConfigValueSource.System, untrustedSystemResolution.Source);
            Assert.IsFalse(untrustedSystemResolution.IsEnforced);

            await ((IPropertyProvider)workbenchSection).DiscardAsync();
            IPropertyItem systemProperty = workbenchSection.Properties.Single(
                static property => property.Name == nameof(CodeStudioSettings.AIModelId));
            Assert.AreEqual(PropertyScope.System, systemProperty.ValueContext.Scope);
            Assert.IsFalse(systemProperty.ValueContext.IsPolicyLocked);
            Assert.IsTrue(systemProperty.IsEditable);
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }
    }

    [TestMethod]
    public async Task CreateServiceProvider_DoesNotCreateSystemOrGroupLayersAndRequiresCompleteGroupConfiguration()
    {
        string root = Path.Combine(Path.GetTempPath(), "CodeStudioUnprovisionedLayers", Guid.NewGuid().ToString("N"));
        string userRoot = Path.Combine(root, "user");
        string systemRoot = Path.Combine(root, "system");
        string groupRoot = Path.Combine(root, "groups");
        try
        {
            using ServiceProvider serviceProvider = App.CreateServiceProvider(
                static () => null,
                userRoot,
                systemRoot,
                groupRoot,
                "Engineering");
            ConfigValueResolution resolution = await serviceProvider
                .GetRequiredService<ConfigLayeredSettingsResolver>()
                .ResolveAsync("Workbench", nameof(CodeStudioSettings.AIModelId));

            Assert.AreEqual(ConfigValueSource.ProductDefault, resolution.Source);
            Assert.IsTrue(Directory.Exists(Path.Combine(userRoot, "JC-Soft", "AA98-CodeStudio", "config", "layers")));
            Assert.IsFalse(Directory.Exists(systemRoot));
            Assert.IsFalse(Directory.Exists(groupRoot));

            Assert.ThrowsExactly<InvalidOperationException>(() =>
                App.CreateServiceProvider(static () => null, userRoot, systemRoot, groupRoot, groupId: " "));
            Assert.ThrowsExactly<InvalidOperationException>(() =>
                App.CreateServiceProvider(static () => null, userRoot, systemRoot, groupConfigRoot: " ", groupId: "Engineering"));
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }
    }

    [TestMethod]
    public async Task WorkbenchSettings_HideUnusedOptionsButRetainLegacySerialization()
    {
        IConfigStore store = Substitute.For<IConfigStore>();
        store.LoadAsync("JC-Soft.AA98-CodeStudio.Workbench", Arg.Any<CodeStudioSettings>())
            .Returns(Task.FromResult(new CodeStudioSettings()));
        ConfigService configService = new(store, "JC-Soft.AA98-CodeStudio");
        ConfigServiceUiSection section = new(configService, new CodeStudioSettingsProvider());

        await section.LoadAsync();

        Assert.AreEqual(ConfigUiSectionState.Ready, section.State);
        CollectionAssert.AreEquivalent(
            new[]
            {
                nameof(CodeStudioSettings.AIEndpoint),
                nameof(CodeStudioSettings.AIModelId),
                nameof(CodeStudioSettings.AIProvider),
                nameof(CodeStudioSettings.CredentialManager),
                nameof(CodeStudioSettings.UseAIProviderProfiles),
                nameof(CodeStudioSettings.AIProfileOpenAIEndpoint),
                nameof(CodeStudioSettings.AIProfileOpenAIModelId),
                nameof(CodeStudioSettings.AIProfileOllamaEndpoint),
                nameof(CodeStudioSettings.AIProfileOllamaModelId),
                nameof(CodeStudioSettings.AIProfileIFMEndpoint),
                nameof(CodeStudioSettings.AIProfileIFMModelId),
                nameof(CodeStudioSettings.AIProfileCustomEndpoint),
                nameof(CodeStudioSettings.AIProfileCustomModelId),
            },
            section.Properties.Select(static property => property.Name).ToArray());

        string serializedLegacySettings = JsonSerializer.Serialize(new CodeStudioSettings
        {
            WorkspacePath = "C:\\legacy-workspace",
            ReopenLastDocument = false,
        });
        StringAssert.Contains(serializedLegacySettings, "\"WorkspacePath\"");
        StringAssert.Contains(serializedLegacySettings, "\"ReopenLastDocument\"");
    }

    [TestMethod]
    public async Task WorkbenchUserLayer_ApplyActivatesResolvedRuntimeSettings()
    {
        string configRoot = Path.Combine(Path.GetTempPath(), "CodeStudioLayeredApply", Guid.NewGuid().ToString("N"));
        try
        {
            using ServiceProvider serviceProvider = App.CreateServiceProvider(static () => null, configRoot);
            ConfigUiViewModel configUi = serviceProvider.GetRequiredService<ConfigUiViewModel>();
            IConfigUiSectionRegistry registry = serviceProvider.GetRequiredService<IConfigUiSectionRegistry>();
            IConfigUiSection section = registry.Sections.Single();
            CodeStudioSettings settings = serviceProvider.GetRequiredService<CodeStudioSettings>();
            var activator = serviceProvider.GetRequiredService<WorkbenchSettingsRuntimeActivator>();
            var aiSettings = serviceProvider.GetRequiredService<AA98_AvlnCodeStudio.Base.AI.Models.OpenAICompatibleAISettings>();
            aiSettings.ApiKey = "old-endpoint-key";
            configUi.SectionAppliedAsync = activator.ActivateAsync;
            await configUi.LoadCommand.ExecuteAsync(null);

            IPropertyItem endpoint = configUi.PropertyEditor.Items
                .Select(static item => item.PropertyItem)
                .Single(static property => property.Name == nameof(CodeStudioSettings.AIEndpoint));
            Assert.IsTrue(endpoint.TrySetValue("http://localhost:11434/v1/test"));

            bool applied = await configUi.ApplyChangesAsync();

            Assert.IsTrue(applied, configUi.OperationMessage);
            Assert.AreEqual("http://localhost:11434/v1/test", settings.AIEndpoint);
            Assert.AreEqual(
                "http://localhost:11434/v1/test",
                serviceProvider.GetRequiredService<AA98_AvlnCodeStudio.Base.AI.Models.OpenAICompatibleAISettings>().Endpoint);
            Assert.AreEqual(string.Empty, aiSettings.ApiKey);
            ConfigLayerSnapshot savedLayer = await serviceProvider.GetRequiredService<JsonConfigLayerStore>().LoadAsync();
            Assert.IsTrue(savedLayer.TryGetValue(
                new ConfigPropertyKey("Workbench", nameof(CodeStudioSettings.AIEndpoint)),
                out ConfigLayerValue? savedValue));
            Assert.AreEqual(
                "http://localhost:11434/v1/test",
                ((JsonElement)savedValue.Value!).GetString());
        }
        finally
        {
            if (Directory.Exists(configRoot))
            {
                Directory.Delete(configRoot, recursive: true);
            }
        }
    }

    [TestMethod]
    public async Task WorkbenchUserLayer_RejectsInvalidEndpointBeforeSavingAndAllowsRetry()
    {
        string configRoot = Path.Combine(Path.GetTempPath(), "CodeStudioEndpointValidation", Guid.NewGuid().ToString("N"));
        try
        {
            using ServiceProvider serviceProvider = App.CreateServiceProvider(static () => null, configRoot);
            ConfigUiViewModel configUi = serviceProvider.GetRequiredService<ConfigUiViewModel>();
            var activator = serviceProvider.GetRequiredService<WorkbenchSettingsRuntimeActivator>();
            var aiSettings = serviceProvider.GetRequiredService<AA98_AvlnCodeStudio.Base.AI.Models.OpenAICompatibleAISettings>();
            configUi.SectionAppliedAsync = activator.ActivateAsync;
            configUi.ValidateChangesAsync = activator.ValidateChangesAsync;
            await configUi.LoadCommand.ExecuteAsync(null);
            IPropertyItem endpoint = configUi.PropertyEditor.Items
                .Select(static item => item.PropertyItem)
                .Single(static property => property.Name == nameof(CodeStudioSettings.AIEndpoint));
            string originalEndpoint = aiSettings.Endpoint;
            Assert.IsTrue(endpoint.TrySetValue("http://remote.example/v1/chat/completions"));

            Assert.IsFalse(await configUi.ApplyChangesAsync());
            StringAssert.Contains(configUi.OperationMessage, "HTTPS");
            Assert.IsTrue(configUi.HasUnsavedChanges);
            Assert.AreEqual(originalEndpoint, aiSettings.Endpoint);
            ConfigLayerSnapshot saved = await serviceProvider.GetRequiredService<JsonConfigLayerStore>().LoadAsync();
            Assert.IsFalse(saved.TryGetValue(new ConfigPropertyKey("Workbench", nameof(CodeStudioSettings.AIEndpoint)), out _));

            Assert.IsTrue(endpoint.TrySetValue("https://remote.example/v1/chat/completions"));
            Assert.IsTrue(await configUi.ApplyChangesAsync(), configUi.OperationMessage);
            Assert.AreEqual("https://remote.example/v1/chat/completions", aiSettings.Endpoint);
        }
        finally
        {
            if (Directory.Exists(configRoot))
            {
                Directory.Delete(configRoot, recursive: true);
            }
        }
    }

    [TestMethod]
    public async Task WorkbenchValidation_UsesCurrentSystemTemplateAndAllowsExplicitOverride()
    {
        string root = Path.Combine(Path.GetTempPath(), "CodeStudioTemplateValidation", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            ConfigSectionRegistry registry = new();
            CodeStudioSettingsProvider provider = new();
            registry.AddSection(provider);
            JsonConfigLayerStore system = new(ConfigLayer.System, Path.Combine(root, "system.json"));
            JsonConfigLayerStore user = new(ConfigLayer.User, Path.Combine(root, "user.json"));
            ConfigLayeredSettingsResolver resolver = new(registry, [system, user]);
            ConfigLayeredUiSection section = new(resolver, provider, user, ConfigLayer.User);
            await section.LoadAsync();
            CodeStudioSettings settings = new();
            Assert.IsNull(await WorkbenchSettingsValidation.ValidateAsync([section], settings, resolver));

            await system.SaveAsync((await system.LoadAsync()).WithValue(
                new ConfigPropertyKey("Workbench", nameof(CodeStudioSettings.KnownAIProviderTemplates)),
                new ConfigLayerValue(new List<AIProviderTemplate>
                {
                    new() { Id = "ollama", Endpoint = "http://remote.example/v1/chat/completions" },
                })));

            StringAssert.Contains(await WorkbenchSettingsValidation.ValidateAsync([section], settings, resolver), "HTTPS");
            IPropertyItem endpoint = section.Properties.Single(property => property.Name == nameof(CodeStudioSettings.AIEndpoint));
            Assert.IsTrue(endpoint.TrySetValue("https://safe.example/v1/chat/completions"));
            Assert.IsNull(await WorkbenchSettingsValidation.ValidateAsync([section], settings, resolver));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [DataTestMethod]
    [DataRow(AIProviderPreset.Custom, "", false)]
    [DataRow(AIProviderPreset.Ollama, "", true)]
    [DataRow(AIProviderPreset.Custom, "http://localhost:11434/v1/chat/completions", true)]
    [DataRow(AIProviderPreset.OpenAI, "ftp://example.test", false)]
    [DataRow(AIProviderPreset.OpenAI, "https://user:secret@example.test/chat", false)]
    public async Task WorkbenchValidation_ValidatesEffectiveEndpoint(
        AIProviderPreset provider, string endpoint, bool valid)
    {
        IConfigUiSection section = Substitute.For<IConfigUiSection>();
        section.Section.Returns(new ConfigUiSection("Workbench", "Workbench", null, 0));
        PropertyCategory category = new("Workbench", "Workbench");
        section.Properties.Returns(
        [
            new PropertyItem(category, nameof(CodeStudioSettings.AIProvider), "Provider", typeof(AIProviderPreset), provider),
            new PropertyItem(category, nameof(CodeStudioSettings.AIEndpoint), "Endpoint", typeof(string), endpoint),
        ]);

        string? error = await WorkbenchSettingsValidation.ValidateAsync([section], new CodeStudioSettings());
        Assert.AreEqual(valid, error is null);
        if (error is not null)
        {
            Assert.IsFalse(error.Contains("secret", StringComparison.Ordinal));
        }
    }

    [TestMethod]
    public async Task AIProviderDraft_ProviderChangePreservesOverridesAndTemplateButtonsAreDraftOnly()
    {
        string root = Path.Combine(Path.GetTempPath(), "CodeStudioProviderDraft", Guid.NewGuid().ToString("N"));
        try
        {
            using ServiceProvider services = App.CreateServiceProvider(static () => null, root);
            ConfigUiViewModel config = services.GetRequiredService<ConfigUiViewModel>();
            CodeStudioSettings settings = services.GetRequiredService<CodeStudioSettings>();
            WorkbenchSettingsRuntimeActivator activator = services.GetRequiredService<WorkbenchSettingsRuntimeActivator>();
            config.ValidateChangesAsync = activator.ValidateChangesAsync;
            config.SectionAppliedAsync = activator.ActivateAsync;
            await config.LoadCommand.ExecuteAsync(null);
            using AIProviderDraftViewModel preview = new(config, settings);

            IPropertyItem Get(string name) => config.PropertyEditor.Items.Single(item => item.PropertyItem.Name == name).PropertyItem;
            IPropertyItem endpoint = Get(nameof(CodeStudioSettings.AIEndpoint));
            IPropertyItem model = Get(nameof(CodeStudioSettings.AIModelId));
            IPropertyItem provider = Get(nameof(CodeStudioSettings.AIProvider));
            Assert.IsTrue(endpoint.TrySetValue("https://old.example/v1/chat?token=private"));
            Assert.IsTrue(model.TrySetValue("legacy-model"));
            Assert.IsTrue(provider.TrySetValue(AIProviderPreset.OpenAI));
            StringAssert.Contains(preview.Summary, "old.example");
            StringAssert.Contains(preview.Summary, "legacy-model (override)");
            Assert.IsFalse(preview.Summary.Contains("private", StringComparison.Ordinal));

            preview.UseTemplateEndpointCommand.Execute(null);
            preview.UseTemplateModelCommand.Execute(null);
            StringAssert.Contains(preview.Summary, "api.openai.com");
            StringAssert.Contains(preview.Summary, "gpt-4o-mini (template)");
            ConfigLayerSnapshot before = await services.GetRequiredService<JsonConfigLayerStore>().LoadAsync();
            Assert.IsFalse(before.TryGetValue(new ConfigPropertyKey("Workbench", nameof(CodeStudioSettings.AIEndpoint)), out _));
            Assert.IsTrue(await config.DiscardChangesAsync());
            Assert.AreEqual(AIProviderPreset.Ollama, Get(nameof(CodeStudioSettings.AIProvider)).Value);
            Assert.AreEqual(string.Empty, Get(nameof(CodeStudioSettings.AIEndpoint)).Value);
            StringAssert.Contains(preview.Summary, "localhost:11434");

            Assert.IsTrue(Get(nameof(CodeStudioSettings.AIEndpoint)).TrySetValue("https://legacy.example/chat"));
            Assert.IsTrue(Get(nameof(CodeStudioSettings.AIProvider)).TrySetValue(AIProviderPreset.OpenAI));
            Assert.IsTrue(await config.ApplyChangesAsync(), config.OperationMessage);
            Assert.AreEqual("https://legacy.example/chat", settings.GetEffectiveAIEndpoint());
            Assert.IsTrue(Get(nameof(CodeStudioSettings.AIProvider)).TrySetValue(AIProviderPreset.Ollama));
            StringAssert.Contains(preview.Summary, "legacy.example");
            Assert.IsTrue(await config.DiscardChangesAsync());
            Assert.AreEqual(AIProviderPreset.OpenAI, Get(nameof(CodeStudioSettings.AIProvider)).Value);
            Assert.AreEqual("https://legacy.example/chat", Get(nameof(CodeStudioSettings.AIEndpoint)).Value);
            preview.UseTemplateEndpointCommand.Execute(null);
            Assert.IsTrue(await config.ApplyChangesAsync(), config.OperationMessage);
            Assert.AreEqual("https://api.openai.com/v1/chat/completions", settings.GetEffectiveAIEndpoint());
            Assert.IsFalse(preview.UseTemplateEndpointCommand.CanExecute(null));
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }
    }

    [TestMethod]
    public async Task AIProviderProfiles_OptInPreservesLegacyValuesAndKeepsProviderOverridesSeparate()
    {
        string root = Path.Combine(Path.GetTempPath(), "CodeStudioProfiles", Guid.NewGuid().ToString("N"));
        try
        {
            using (ServiceProvider services = App.CreateServiceProvider(static () => null, root))
            {
                ConfigUiViewModel config = services.GetRequiredService<ConfigUiViewModel>();
                CodeStudioSettings settings = services.GetRequiredService<CodeStudioSettings>();
                var runtime = services.GetRequiredService<AA98_AvlnCodeStudio.Base.AI.Models.OpenAICompatibleAISettings>();
                WorkbenchSettingsRuntimeActivator activator = services.GetRequiredService<WorkbenchSettingsRuntimeActivator>();
                config.ValidateChangesAsync = activator.ValidateChangesAsync;
                config.SectionAppliedAsync = activator.ActivateAsync;
                await config.LoadCommand.ExecuteAsync(null);
                using AIProviderDraftViewModel preview = new(config, settings);
                IPropertyItem Get(string name) => config.PropertyEditor.Items.Single(item => item.PropertyItem.Name == name).PropertyItem;

                Assert.IsTrue(Get(nameof(CodeStudioSettings.AIEndpoint)).TrySetValue("https://legacy.example/chat"));
                Assert.IsTrue(Get(nameof(CodeStudioSettings.AIModelId)).TrySetValue("legacy-model"));
                Assert.IsTrue(Get(nameof(CodeStudioSettings.AIProfileOpenAIEndpoint)).TrySetValue("https://openai.example/chat"));
                Assert.IsTrue(Get(nameof(CodeStudioSettings.AIProfileOpenAIModelId)).TrySetValue("openai-model"));
                Assert.IsTrue(Get(nameof(CodeStudioSettings.AIProfileOllamaEndpoint)).TrySetValue("http://localhost:22334/chat"));
                Assert.IsTrue(Get(nameof(CodeStudioSettings.AIProfileOllamaModelId)).TrySetValue("local-model"));
                Assert.IsTrue(await config.ApplyChangesAsync(), config.OperationMessage);
                Assert.AreEqual("https://legacy.example/chat", settings.GetEffectiveAIEndpoint());
                runtime.ApiKey = "session-key";

                Assert.IsTrue(Get(nameof(CodeStudioSettings.UseAIProviderProfiles)).TrySetValue(true));
                Assert.IsFalse(preview.CanManageActiveCredentials);
                StringAssert.Contains(preview.Summary, "localhost:22334");
                Assert.IsTrue(await config.ApplyChangesAsync(), config.OperationMessage);
                Assert.IsTrue(preview.CanManageActiveCredentials);
                Assert.AreEqual("local-model", settings.GetEffectiveAIModelId());
                Assert.AreEqual(string.Empty, runtime.ApiKey);
                Assert.IsTrue(Get(nameof(CodeStudioSettings.AIProvider)).TrySetValue(AIProviderPreset.OpenAI));
                Assert.IsFalse(preview.CanManageActiveCredentials);
                StringAssert.Contains(preview.Summary, "openai.example");
                Assert.IsTrue(await config.ApplyChangesAsync(), config.OperationMessage);
                Assert.AreEqual("https://openai.example/chat", settings.GetEffectiveAIEndpoint());
                Assert.AreEqual("openai-model", settings.GetEffectiveAIModelId());

                preview.UseTemplateEndpointCommand.Execute(null);
                StringAssert.Contains(preview.Summary, "api.openai.com");
                Assert.IsTrue(await config.DiscardChangesAsync());
                Assert.AreEqual("https://openai.example/chat", settings.GetEffectiveAIEndpoint());
                StringAssert.Contains(preview.Summary, "openai.example");
            }

            using ServiceProvider reopened = App.CreateServiceProvider(static () => null, root);
            CodeStudioSettings saved = reopened.GetRequiredService<CodeStudioSettings>();
            Assert.IsTrue(saved.UseAIProviderProfiles);
            Assert.AreEqual("https://openai.example/chat", saved.GetEffectiveAIEndpoint());
            Assert.AreEqual("http://localhost:22334/chat", saved.AIProfileOllamaEndpoint);
            Assert.AreEqual("https://legacy.example/chat", saved.AIEndpoint);
            saved.UseAIProviderProfiles = false;
            Assert.AreEqual("https://legacy.example/chat", saved.GetEffectiveAIEndpoint());
            Assert.AreEqual("legacy-model", saved.GetEffectiveAIModelId());
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }
    }

    [TestMethod]
    public async Task WorkbenchSettings_ReplacedPropertyCannotChangeNextLayerDraft()
    {
        string root = Path.Combine(Path.GetTempPath(), "CodeStudioStaleProperty", Guid.NewGuid().ToString("N"));
        try
        {
            using ServiceProvider services = App.CreateServiceProvider(static () => null, root);
            ConfigUiViewModel config = services.GetRequiredService<ConfigUiViewModel>();
            var activator = services.GetRequiredService<WorkbenchSettingsRuntimeActivator>();
            config.ValidateChangesAsync = activator.ValidateChangesAsync;
            config.SectionAppliedAsync = activator.ActivateAsync;
            await config.LoadCommand.ExecuteAsync(null);
            IPropertyItem Get(string name) => config.PropertyEditor.Items.Single(item => item.PropertyItem.Name == name).PropertyItem;
            IPropertyItem old = Get(nameof(CodeStudioSettings.AIProfileIFMEndpoint));
            Assert.IsTrue(old.TrySetValue("https://first.example/chat"));
            Assert.IsTrue(await config.ApplyChangesAsync(), config.OperationMessage);
            Assert.IsFalse(old.TrySetValue("https://obsolete.example/chat"));
            Assert.IsFalse(config.HasUnsavedChanges);
            Assert.IsTrue(Get(nameof(CodeStudioSettings.AIProfileCustomModelId)).TrySetValue("new-model"));
            Assert.IsTrue(await config.ApplyChangesAsync(), config.OperationMessage);
            Assert.AreEqual("https://first.example/chat", services.GetRequiredService<CodeStudioSettings>().AIProfileIFMEndpoint);
            Assert.AreEqual("new-model", services.GetRequiredService<CodeStudioSettings>().AIProfileCustomModelId);
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }
    }

    [TestMethod]
    public async Task AIProviderProfiles_InvalidInactiveEndpointBlocksApplyWithoutWriting()
    {
        string root = Path.Combine(Path.GetTempPath(), "CodeStudioProfileValidation", Guid.NewGuid().ToString("N"));
        try
        {
            using ServiceProvider services = App.CreateServiceProvider(static () => null, root);
            ConfigUiViewModel config = services.GetRequiredService<ConfigUiViewModel>();
            config.ValidateChangesAsync = services.GetRequiredService<WorkbenchSettingsRuntimeActivator>().ValidateChangesAsync;
            await config.LoadCommand.ExecuteAsync(null);
            IPropertyItem endpoint = config.PropertyEditor.Items.Single(
                item => item.PropertyItem.Name == nameof(CodeStudioSettings.AIProfileIFMEndpoint)).PropertyItem;
            Assert.IsTrue(endpoint.TrySetValue("http://remote.example/chat"));
            Assert.IsFalse(await config.ApplyChangesAsync());
            StringAssert.Contains(config.OperationMessage, "IFM profile");
            Assert.IsTrue(config.HasUnsavedChanges);
            Assert.IsFalse((await services.GetRequiredService<JsonConfigLayerStore>().LoadAsync()).TryGetValue(
                new ConfigPropertyKey("Workbench", nameof(CodeStudioSettings.AIProfileIFMEndpoint)), out _));
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }
    }

    [TestMethod]
    public async Task AIProviderDraft_ShowAllAllowsFixingInactiveProfileWithoutChangingActiveProvider()
    {
        string root = Path.Combine(Path.GetTempPath(), "CodeStudioProfileRepair", Guid.NewGuid().ToString("N"));
        try
        {
            using ServiceProvider services = App.CreateServiceProvider(static () => null, root);
            ConfigUiViewModel config = services.GetRequiredService<ConfigUiViewModel>();
            var activator = services.GetRequiredService<WorkbenchSettingsRuntimeActivator>();
            config.ValidateChangesAsync = activator.ValidateChangesAsync;
            config.SectionAppliedAsync = activator.ActivateAsync;
            await config.LoadCommand.ExecuteAsync(null);
            using AIProviderDraftViewModel preview = new(config, services.GetRequiredService<CodeStudioSettings>());
            Property.Editor.Avalonia.PropertyEditorItemViewModel Item(string name) =>
                config.PropertyEditor.Items.Single(item => item.PropertyItem.Name == name);
            var inactive = Item(nameof(CodeStudioSettings.AIProfileIFMEndpoint));
            Assert.IsFalse(inactive.IsVisible);
            preview.ShowAllProviderFields = true;
            Assert.IsTrue(inactive.IsVisible);
            Assert.IsFalse(config.HasUnsavedChanges);
            Assert.IsTrue(inactive.PropertyItem.TrySetValue("http://remote.example/chat"));
            Assert.IsTrue(Item(nameof(CodeStudioSettings.AIProfileCustomEndpoint)).PropertyItem.TrySetValue("https://user:secret@other.example/chat?token=hidden"));
            Assert.IsTrue(preview.HasInvalidProfileEndpoints);
            StringAssert.Contains(preview.ProfileValidationMessage, "IFM profile");
            StringAssert.Contains(preview.ProfileValidationMessage, "Custom profile");
            Assert.IsFalse(preview.ProfileValidationMessage.Contains("remote.example", StringComparison.Ordinal));
            Assert.IsFalse(preview.ProfileValidationMessage.Contains("secret", StringComparison.Ordinal));
            Assert.IsFalse(preview.ShowInvalidProfileFieldsCommand.CanExecute(null));
            preview.ShowAllProviderFields = false;
            Assert.IsFalse(inactive.IsVisible);
            Assert.IsTrue(preview.ShowInvalidProfileFieldsCommand.CanExecute(null));
            Assert.IsFalse(await config.ApplyChangesAsync());
            StringAssert.Contains(config.OperationMessage, "Custom profile");
            StringAssert.Contains(config.OperationMessage, "IFM profile");
            StringAssert.Contains(config.OperationMessage, "Show all provider fields");
            Assert.IsFalse(config.OperationMessage.Contains("other.example", StringComparison.Ordinal));
            Assert.IsFalse(config.OperationMessage.Contains("secret", StringComparison.Ordinal));
            Assert.AreEqual(AIProviderPreset.Ollama, Item(nameof(CodeStudioSettings.AIProvider)).PropertyItem.Value);
            Assert.AreEqual(false, Item(nameof(CodeStudioSettings.UseAIProviderProfiles)).PropertyItem.Value);

            preview.ShowInvalidProfileFieldsCommand.Execute(null);
            Assert.IsTrue(inactive.IsVisible);
            Assert.IsTrue(preview.ShowAllProviderFields);
            Assert.IsFalse(preview.ShowInvalidProfileFieldsCommand.CanExecute(null));
            Assert.IsTrue(Item(nameof(CodeStudioSettings.AIProfileCustomEndpoint)).PropertyItem.TrySetValue("https://other.example/chat"));
            Assert.IsFalse(preview.ProfileValidationMessage.Contains("Custom profile", StringComparison.Ordinal));
            Assert.IsFalse(await config.ApplyChangesAsync());
            StringAssert.Contains(config.OperationMessage, "IFM profile");
            Assert.IsTrue(inactive.PropertyItem.TrySetValue("https://valid.example/chat"));
            Assert.IsFalse(preview.HasInvalidProfileEndpoints);
            Assert.IsTrue(await config.ApplyChangesAsync(), config.OperationMessage);
            Assert.IsTrue(Item(nameof(CodeStudioSettings.AIProfileIFMEndpoint)).IsVisible);
            Assert.AreEqual(AIProviderPreset.Ollama, services.GetRequiredService<CodeStudioSettings>().AIProvider);
            Assert.IsFalse(services.GetRequiredService<CodeStudioSettings>().UseAIProviderProfiles);
            preview.ShowAllProviderFields = false;
            Assert.IsFalse(Item(nameof(CodeStudioSettings.AIProfileIFMEndpoint)).IsVisible);
            Assert.IsFalse(config.HasUnsavedChanges);
            ConfigLayerSnapshot saved = await services.GetRequiredService<JsonConfigLayerStore>().LoadAsync();
            Assert.IsTrue(saved.TryGetValue(
                new ConfigPropertyKey("Workbench", nameof(CodeStudioSettings.AIProfileIFMEndpoint)), out _));
            preview.ShowAllProviderFields = true;
            Assert.IsTrue(Item(nameof(CodeStudioSettings.AIProfileIFMEndpoint)).PropertyItem.TrySetValue("http://discard.example/chat"));
            Assert.IsTrue(preview.HasInvalidProfileEndpoints);
            Assert.IsTrue(await config.DiscardChangesAsync());
            Assert.IsTrue(Item(nameof(CodeStudioSettings.AIProfileIFMEndpoint)).IsVisible);
            Assert.AreEqual("https://valid.example/chat", Item(nameof(CodeStudioSettings.AIProfileIFMEndpoint)).PropertyItem.Value);
            Assert.AreEqual(AIProviderPreset.Ollama, Item(nameof(CodeStudioSettings.AIProvider)).PropertyItem.Value);
            Assert.IsFalse(preview.HasInvalidProfileEndpoints);
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }
    }

    [TestMethod]
    public async Task AIProviderDraft_InspectIssueNavigatesWithoutChangingOrSavingDraft()
    {
        string root = Path.Combine(Path.GetTempPath(), "CodeStudioProfileIssueNavigation", Guid.NewGuid().ToString("N"));
        try
        {
            using ServiceProvider services = App.CreateServiceProvider(static () => null, root);
            ConfigUiViewModel config = services.GetRequiredService<ConfigUiViewModel>();
            var activator = services.GetRequiredService<WorkbenchSettingsRuntimeActivator>();
            config.ValidateChangesAsync = activator.ValidateChangesAsync;
            config.SectionAppliedAsync = activator.ActivateAsync;
            await config.LoadCommand.ExecuteAsync(null);
            using AIProviderDraftViewModel preview = new(config, services.GetRequiredService<CodeStudioSettings>());
            Property.Editor.Avalonia.PropertyEditorItemViewModel Item(string name) =>
                config.PropertyEditor.Items.Single(item => item.PropertyItem.Name == name);
            Assert.IsTrue(Item(nameof(CodeStudioSettings.AIProfileIFMEndpoint)).PropertyItem.TrySetValue("http://ifm.example/chat"));
            Assert.IsTrue(Item(nameof(CodeStudioSettings.AIProfileCustomEndpoint)).PropertyItem.TrySetValue("http://custom.example/chat"));
            Assert.AreEqual(2, preview.ProfileIssues.Count);
            AIProfileIssueViewModel ifm = preview.ProfileIssues.Single(issue => issue.Issue.Provider == AIProviderPreset.IFM);
            AIProfileIssueViewModel custom = preview.ProfileIssues.Single(issue => issue.Issue.Provider == AIProviderPreset.Custom);
            List<(AIProviderPreset Provider, long Revision)> requests = [];
            preview.ProfileEndpointNavigationRequested += (provider, revision) => requests.Add((provider, revision));
            preview.ShowAllProviderFields = true;
            TaskCompletionSource<string?> validation = new(TaskCreationOptions.RunContinuationsAsynchronously);
            config.ValidateChangesAsync = _ => validation.Task;
            Task<bool> apply = config.ApplyChangesAsync();
            Assert.IsTrue(config.IsBusy);
            Assert.IsFalse(ifm.InspectCommand.CanExecute(null));
            ifm.InspectCommand.Execute(null);
            Assert.AreEqual(AIProfileInspection.ActiveConnection, preview.InspectedProfile);
            Assert.IsTrue(preview.ShowAllProviderFields);
            Assert.AreEqual(0, requests.Count);
            validation.SetResult("Test validation blocked the save.");
            Assert.IsFalse(await apply);
            config.ValidateChangesAsync = activator.ValidateChangesAsync;
            Assert.IsTrue(ifm.InspectCommand.CanExecute(null));
            ifm.InspectCommand.Execute(null);
            Assert.AreEqual(1, requests.Count);
            Assert.AreEqual(AIProviderPreset.IFM, requests[0].Provider);
            Assert.IsTrue(preview.IsProfileNavigationCurrent(requests[0].Provider, requests[0].Revision));
            Assert.AreEqual(AIProfileInspection.IFM, preview.InspectedProfile);
            Assert.IsFalse(preview.ShowAllProviderFields);
            Assert.IsTrue(Item(nameof(CodeStudioSettings.AIProfileIFMEndpoint)).IsVisible);
            Assert.IsFalse(Item(nameof(CodeStudioSettings.AIProfileCustomEndpoint)).IsVisible);
            Assert.AreEqual(AIProviderPreset.Ollama, Item(nameof(CodeStudioSettings.AIProvider)).PropertyItem.Value);
            Assert.AreEqual(false, Item(nameof(CodeStudioSettings.UseAIProviderProfiles)).PropertyItem.Value);
            Assert.IsTrue(Item(nameof(CodeStudioSettings.AIProfileIFMEndpoint)).PropertyItem.TrySetValue("http://different.example/chat"));
            Assert.IsFalse(preview.IsProfileNavigationCurrent(requests[0].Provider, requests[0].Revision));
            Assert.IsTrue(Item(nameof(CodeStudioSettings.AIProfileIFMEndpoint)).PropertyItem.TrySetValue("https://ifm.example/chat"));
            Assert.IsFalse(preview.IsProfileNavigationCurrent(requests[0].Provider, requests[0].Revision));
            Assert.IsFalse(ifm.InspectCommand.CanExecute(null));
            custom.InspectCommand.Execute(null);
            Assert.AreEqual(AIProviderPreset.Custom, requests[1].Provider);
            Assert.IsTrue(preview.IsProfileNavigationCurrent(requests[1].Provider, requests[1].Revision));
            preview.InspectedProfile = AIProfileInspection.IFM;
            preview.InspectedProfile = AIProfileInspection.Custom;
            Assert.IsFalse(preview.IsProfileNavigationCurrent(requests[1].Provider, requests[1].Revision));
            Assert.AreEqual(AIProfileInspection.Custom, preview.InspectedProfile);
            Assert.IsTrue(Item(nameof(CodeStudioSettings.AIProfileCustomEndpoint)).IsVisible);
            Assert.IsTrue(Item(nameof(CodeStudioSettings.AIProfileCustomEndpoint)).PropertyItem.TrySetValue("https://custom.example/chat"));
            Assert.AreEqual(0, preview.ProfileIssues.Count);
            Assert.IsTrue(await config.ApplyChangesAsync(), config.OperationMessage);
            Assert.IsFalse(config.HasUnsavedChanges);
            Assert.AreEqual(AIProviderPreset.Ollama, services.GetRequiredService<CodeStudioSettings>().AIProvider);
            Assert.IsFalse(services.GetRequiredService<CodeStudioSettings>().UseAIProviderProfiles);
            Assert.IsTrue(Item(nameof(CodeStudioSettings.AIProfileCustomEndpoint)).PropertyItem.TrySetValue("http://discard.example/chat"));
            preview.ProfileIssues.Single().InspectCommand.Execute(null);
            Assert.IsTrue(await config.DiscardChangesAsync());
            Assert.AreEqual(0, preview.ProfileIssues.Count);
            Assert.AreEqual("https://custom.example/chat", Item(nameof(CodeStudioSettings.AIProfileCustomEndpoint)).PropertyItem.Value);
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }
    }

    [TestMethod]
    public async Task AIProviderDraft_InspectSavedInvalidProfileIsDisplayOnly()
    {
        string root = Path.Combine(Path.GetTempPath(), "CodeStudioSavedProfileIssue", Guid.NewGuid().ToString("N"));
        try
        {
            using ServiceProvider services = App.CreateServiceProvider(static () => null, root);
            JsonConfigLayerStore store = services.GetRequiredService<JsonConfigLayerStore>();
            ConfigLayerSnapshot saved = await store.LoadAsync();
            await store.SaveAsync(saved.WithValue(
                new ConfigPropertyKey("Workbench", nameof(CodeStudioSettings.AIProfileIFMEndpoint)),
                new ConfigLayerValue("http://invalid.example/chat")));
            ConfigUiViewModel config = services.GetRequiredService<ConfigUiViewModel>();
            await config.LoadCommand.ExecuteAsync(null);
            using AIProviderDraftViewModel preview = new(config, services.GetRequiredService<CodeStudioSettings>());
            string version = (await store.LoadAsync()).StorageVersion;
            preview.ProfileIssues.Single().InspectCommand.Execute(null);
            Assert.AreEqual(AIProfileInspection.IFM, preview.InspectedProfile);
            Assert.IsFalse(config.HasUnsavedChanges);
            Assert.AreEqual(version, (await store.LoadAsync()).StorageVersion);
            Assert.AreEqual(AIProviderPreset.Ollama, services.GetRequiredService<CodeStudioSettings>().AIProvider);
            AIProfileIssueViewModel issue = preview.ProfileIssues.Single();
            long revision = 0;
            preview.ProfileEndpointNavigationRequested += (_, value) => revision = value;
            issue.InspectCommand.Execute(null);
            Assert.IsTrue(preview.IsProfileNavigationCurrent(AIProviderPreset.IFM, revision));
            preview.Dispose();
            Assert.IsFalse(preview.IsProfileNavigationCurrent(AIProviderPreset.IFM, revision));
            Assert.IsFalse(issue.InspectCommand.CanExecute(null));
            issue.InspectCommand.Execute(null);
            Assert.IsTrue(config.PropertyEditor.Items.All(item => item.IsVisible));
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }
    }

    [TestMethod]
    public async Task AIProviderDraft_InspectionEditsInactiveProfilesWithoutChangingActiveConnection()
    {
        string root = Path.Combine(Path.GetTempPath(), "CodeStudioProfileInspection", Guid.NewGuid().ToString("N"));
        try
        {
            using ServiceProvider services = App.CreateServiceProvider(static () => null, root);
            ConfigUiViewModel config = services.GetRequiredService<ConfigUiViewModel>();
            var activator = services.GetRequiredService<WorkbenchSettingsRuntimeActivator>();
            config.ValidateChangesAsync = activator.ValidateChangesAsync;
            config.SectionAppliedAsync = activator.ActivateAsync;
            await config.LoadCommand.ExecuteAsync(null);
            using AIProviderDraftViewModel preview = new(config, services.GetRequiredService<CodeStudioSettings>());
            Property.Editor.Avalonia.PropertyEditorItemViewModel Item(string name) =>
                config.PropertyEditor.Items.Single(item => item.PropertyItem.Name == name);
            var runtime = services.GetRequiredService<AA98_AvlnCodeStudio.Base.AI.Models.OpenAICompatibleAISettings>();
            string originalEndpoint = runtime.Endpoint;
            runtime.ApiKey = "session-key";

            preview.InspectedProfile = AIProfileInspection.IFM;
            Assert.IsFalse(config.HasUnsavedChanges);
            Assert.IsFalse(Item(nameof(CodeStudioSettings.AIEndpoint)).IsVisible);
            Assert.IsTrue(Item(nameof(CodeStudioSettings.AIProfileIFMEndpoint)).IsVisible);
            Assert.IsTrue(Item(nameof(CodeStudioSettings.AIProfileIFMModelId)).IsVisible);
            Assert.IsFalse(Item(nameof(CodeStudioSettings.AIProfileOllamaEndpoint)).IsVisible);
            preview.ShowAllProviderFields = true;
            Assert.IsTrue(Item(nameof(CodeStudioSettings.AIEndpoint)).IsVisible);
            Assert.IsTrue(Item(nameof(CodeStudioSettings.AIProfileOllamaEndpoint)).IsVisible);
            preview.ShowAllProviderFields = false;
            Assert.IsFalse(Item(nameof(CodeStudioSettings.AIProfileOllamaEndpoint)).IsVisible);
            Assert.IsFalse(preview.UseTemplateEndpointCommand.CanExecute(null));
            Assert.IsFalse(preview.CopyLegacyEndpointCommand.CanExecute(null));
            Assert.IsTrue(Item(nameof(CodeStudioSettings.AIProfileIFMEndpoint)).PropertyItem.TrySetValue("https://ifm.example/chat"));
            preview.InspectedProfile = AIProfileInspection.OpenAI;
            Assert.IsFalse(Item(nameof(CodeStudioSettings.AIProfileIFMEndpoint)).IsVisible);
            Assert.IsTrue(Item(nameof(CodeStudioSettings.AIProfileOpenAIEndpoint)).PropertyItem.TrySetValue("https://openai.example/chat"));
            Assert.AreEqual(AIProviderPreset.Ollama, Item(nameof(CodeStudioSettings.AIProvider)).PropertyItem.Value);
            Assert.IsTrue(await config.ApplyChangesAsync(), config.OperationMessage);
            Assert.AreEqual(originalEndpoint, runtime.Endpoint);
            Assert.AreEqual("session-key", runtime.ApiKey);
            Assert.IsFalse(services.GetRequiredService<CodeStudioSettings>().UseAIProviderProfiles);
            Assert.AreEqual("https://ifm.example/chat", services.GetRequiredService<CodeStudioSettings>().AIProfileIFMEndpoint);
            Assert.IsTrue(Item(nameof(CodeStudioSettings.AIProfileOpenAIEndpoint)).IsVisible);
            Assert.IsFalse(Item(nameof(CodeStudioSettings.AIProfileIFMEndpoint)).IsVisible);

            Assert.IsTrue(Item(nameof(CodeStudioSettings.UseAIProviderProfiles)).PropertyItem.TrySetValue(true));
            Assert.IsTrue(Item(nameof(CodeStudioSettings.AIProfileOpenAIEndpoint)).IsVisible);
            Assert.IsTrue(Item(nameof(CodeStudioSettings.AIProfileOpenAIEndpoint)).PropertyItem.TrySetValue("https://discard.example/chat"));
            preview.InspectedProfile = AIProfileInspection.IFM;
            Assert.IsTrue(await config.DiscardChangesAsync());
            Assert.AreEqual(AIProfileInspection.IFM, preview.InspectedProfile);
            Assert.IsTrue(Item(nameof(CodeStudioSettings.AIProfileIFMEndpoint)).IsVisible);
            Assert.AreEqual("https://ifm.example/chat", Item(nameof(CodeStudioSettings.AIProfileIFMEndpoint)).PropertyItem.Value);
            Assert.AreEqual(false, Item(nameof(CodeStudioSettings.UseAIProviderProfiles)).PropertyItem.Value);
            preview.InspectedProfile = AIProfileInspection.ActiveConnection;
            Assert.IsTrue(Item(nameof(CodeStudioSettings.AIEndpoint)).IsVisible);
            Assert.IsFalse(Item(nameof(CodeStudioSettings.AIProfileIFMEndpoint)).IsVisible);
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => preview.InspectedProfile = (AIProfileInspection)99);
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }
    }

    [TestMethod]
    public async Task AIProviderDraft_ShowsOnlySelectedProfileAndPreservesHiddenDrafts()
    {
        string root = Path.Combine(Path.GetTempPath(), "CodeStudioProfileVisibility", Guid.NewGuid().ToString("N"));
        try
        {
            using ServiceProvider services = App.CreateServiceProvider(static () => null, root);
            ConfigUiViewModel config = services.GetRequiredService<ConfigUiViewModel>();
            var activator = services.GetRequiredService<WorkbenchSettingsRuntimeActivator>();
            config.ValidateChangesAsync = activator.ValidateChangesAsync;
            config.SectionAppliedAsync = activator.ActivateAsync;
            await config.LoadCommand.ExecuteAsync(null);
            using AIProviderDraftViewModel preview = new(config, services.GetRequiredService<CodeStudioSettings>());
            Property.Editor.Avalonia.PropertyEditorItemViewModel Item(string name) =>
                config.PropertyEditor.Items.Single(item => item.PropertyItem.Name == name);

            Assert.IsTrue(Item(nameof(CodeStudioSettings.AIEndpoint)).IsVisible);
            Assert.IsFalse(Item(nameof(CodeStudioSettings.AIProfileOllamaEndpoint)).IsVisible);
            Assert.IsTrue(Item(nameof(CodeStudioSettings.UseAIProviderProfiles)).PropertyItem.TrySetValue(true));
            Assert.IsFalse(Item(nameof(CodeStudioSettings.AIEndpoint)).IsVisible);
            Assert.IsTrue(Item(nameof(CodeStudioSettings.AIProfileOllamaEndpoint)).IsVisible);
            Assert.IsFalse(Item(nameof(CodeStudioSettings.AIProfileOpenAIEndpoint)).IsVisible);
            Assert.IsTrue(Item(nameof(CodeStudioSettings.AIProfileIFMEndpoint)).PropertyItem.TrySetValue("http://hidden.example/chat"));
            Assert.IsTrue(preview.ShowInvalidProfileFieldsCommand.CanExecute(null));
            preview.ShowInvalidProfileFieldsCommand.Execute(null);
            Assert.IsTrue(Item(nameof(CodeStudioSettings.AIProfileIFMEndpoint)).IsVisible);
            Assert.AreEqual(AIProviderPreset.Ollama, Item(nameof(CodeStudioSettings.AIProvider)).PropertyItem.Value);
            StringAssert.Contains(preview.Summary, "Draft: Ollama (provider profile)");
            Assert.IsTrue(Item(nameof(CodeStudioSettings.AIProfileIFMEndpoint)).PropertyItem.TrySetValue(string.Empty));
            preview.ShowAllProviderFields = false;
            Assert.IsTrue(Item(nameof(CodeStudioSettings.AIProfileOllamaEndpoint)).PropertyItem.TrySetValue("http://localhost:22334/chat"));
            Assert.IsTrue(Item(nameof(CodeStudioSettings.AIProvider)).PropertyItem.TrySetValue(AIProviderPreset.OpenAI));
            Assert.IsFalse(Item(nameof(CodeStudioSettings.AIProfileOllamaEndpoint)).IsVisible);
            Assert.IsTrue(Item(nameof(CodeStudioSettings.AIProfileOpenAIEndpoint)).IsVisible);
            Assert.IsTrue(Item(nameof(CodeStudioSettings.AIProfileOpenAIEndpoint)).PropertyItem.TrySetValue("https://openai.example/chat"));
            Assert.IsTrue(await config.ApplyChangesAsync(), config.OperationMessage);
            Assert.IsTrue(Item(nameof(CodeStudioSettings.AIProfileOpenAIEndpoint)).IsVisible);
            Assert.IsFalse(Item(nameof(CodeStudioSettings.AIProfileOllamaEndpoint)).IsVisible);
            Assert.IsTrue(Item(nameof(CodeStudioSettings.AIProvider)).PropertyItem.TrySetValue(AIProviderPreset.Ollama));
            Assert.AreEqual("http://localhost:22334/chat", Item(nameof(CodeStudioSettings.AIProfileOllamaEndpoint)).PropertyItem.Value);
            Assert.IsTrue(await config.DiscardChangesAsync());
            Assert.IsTrue(Item(nameof(CodeStudioSettings.AIProfileOpenAIEndpoint)).IsVisible);
            preview.Dispose();
            Assert.IsTrue(config.PropertyEditor.Items.All(item => item.IsVisible));
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }
    }

    [TestMethod]
    public async Task AIProviderDraft_CopiesLegacyValuesOnlyIntoEmptySelectedProfile()
    {
        string root = Path.Combine(Path.GetTempPath(), "CodeStudioProfileCopy", Guid.NewGuid().ToString("N"));
        try
        {
            using ServiceProvider services = App.CreateServiceProvider(static () => null, root);
            ConfigUiViewModel config = services.GetRequiredService<ConfigUiViewModel>();
            await config.LoadCommand.ExecuteAsync(null);
            using AIProviderDraftViewModel preview = new(config, services.GetRequiredService<CodeStudioSettings>());
            IPropertyItem Get(string name) => config.PropertyEditor.Items.Single(item => item.PropertyItem.Name == name).PropertyItem;
            Assert.IsTrue(Get(nameof(CodeStudioSettings.AIEndpoint)).TrySetValue("https://legacy.example/chat"));
            Assert.IsTrue(Get(nameof(CodeStudioSettings.AIModelId)).TrySetValue("legacy-model"));
            Assert.IsFalse(preview.CopyLegacyEndpointCommand.CanExecute(null));
            Assert.IsTrue(Get(nameof(CodeStudioSettings.UseAIProviderProfiles)).TrySetValue(true));
            Assert.IsTrue(preview.CopyLegacyEndpointCommand.CanExecute(null));
            Assert.IsTrue(preview.CopyLegacyModelCommand.CanExecute(null));
            preview.InspectedProfile = AIProfileInspection.OpenAI;
            Assert.IsFalse(preview.CopyLegacyEndpointCommand.CanExecute(null));
            Assert.IsFalse(preview.CopyLegacyModelCommand.CanExecute(null));
            preview.CopyLegacyEndpointCommand.Execute(null);
            Assert.AreEqual(string.Empty, Get(nameof(CodeStudioSettings.AIProfileOllamaEndpoint)).Value);
            Assert.AreEqual(string.Empty, Get(nameof(CodeStudioSettings.AIProfileOpenAIEndpoint)).Value);
            preview.InspectedProfile = AIProfileInspection.ActiveConnection;
            preview.CopyLegacyEndpointCommand.Execute(null);
            preview.CopyLegacyModelCommand.Execute(null);
            Assert.AreEqual("https://legacy.example/chat", Get(nameof(CodeStudioSettings.AIProfileOllamaEndpoint)).Value);
            Assert.AreEqual("legacy-model", Get(nameof(CodeStudioSettings.AIProfileOllamaModelId)).Value);
            Assert.IsFalse(preview.CopyLegacyEndpointCommand.CanExecute(null));
            Assert.IsTrue(Get(nameof(CodeStudioSettings.AIEndpoint)).TrySetValue("https://different.example/chat"));
            preview.CopyLegacyEndpointCommand.Execute(null);
            Assert.AreEqual("https://legacy.example/chat", Get(nameof(CodeStudioSettings.AIProfileOllamaEndpoint)).Value);
            Assert.IsTrue(Get(nameof(CodeStudioSettings.AIProvider)).TrySetValue(AIProviderPreset.OpenAI));
            Assert.IsTrue(preview.CopyLegacyEndpointCommand.CanExecute(null));
            Assert.IsTrue(await config.DiscardChangesAsync());
            Assert.AreEqual(string.Empty, Get(nameof(CodeStudioSettings.AIProfileOllamaEndpoint)).Value);
            Assert.AreEqual(string.Empty, Get(nameof(CodeStudioSettings.AIEndpoint)).Value);
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }
    }

    [TestMethod]
    public async Task AIProviderDraft_DoesNotCopyIntoReadOnlyProfile()
    {
        IConfigUiSection section = Substitute.For<IConfigUiSection>();
        section.Section.Returns(new ConfigUiSection("Workbench", "Workbench", null, 0));
        section.State.Returns(ConfigUiSectionState.Ready);
        PropertyCategory category = new("Workbench", "Workbench");
        PropertyItem target = new(category, nameof(CodeStudioSettings.AIProfileOpenAIEndpoint), "Endpoint",
            typeof(string), string.Empty, isEditable: false);
        PropertyItem inactive = new(category, nameof(CodeStudioSettings.AIProfileIFMEndpoint), "IFM Endpoint",
            typeof(string), "http://remote.example", isEditable: false);
        section.Properties.Returns(
        [
            new PropertyItem(category, nameof(CodeStudioSettings.AIEndpoint), "Global endpoint", typeof(string), "https://legacy.example/chat"),
            new PropertyItem(category, nameof(CodeStudioSettings.AIModelId), "Global model", typeof(string), "model"),
            new PropertyItem(category, nameof(CodeStudioSettings.AIProvider), "Provider", typeof(AIProviderPreset), AIProviderPreset.OpenAI),
            new PropertyItem(category, nameof(CodeStudioSettings.UseAIProviderProfiles), "Profiles", typeof(bool), true),
            target,
            inactive,
            new PropertyItem(category, nameof(CodeStudioSettings.AIProfileOpenAIModelId), "Model", typeof(string), string.Empty),
        ]);
        IConfigUiSectionRegistry registry = Substitute.For<IConfigUiSectionRegistry>();
        registry.Sections.Returns([section]);
        ConfigUiViewModel config = new(registry, new Property.Editor.Avalonia.PropertyEditorViewModel());
        await config.LoadCommand.ExecuteAsync(null);
        using AIProviderDraftViewModel preview = new(config, new CodeStudioSettings());
        Assert.IsFalse(preview.CopyLegacyEndpointCommand.CanExecute(null));
        preview.CopyLegacyEndpointCommand.Execute(null);
        Assert.AreEqual(string.Empty, target.Value);
        Assert.IsTrue(preview.CopyLegacyModelCommand.CanExecute(null));
        var inactiveEditor = config.PropertyEditor.Items.Single(item => item.PropertyItem == inactive);
        Assert.IsFalse(inactiveEditor.IsVisible);
        preview.InspectedProfile = AIProfileInspection.IFM;
        Assert.IsTrue(inactiveEditor.IsVisible);
        Assert.IsFalse(inactive.TrySetValue("https://corrected.example"));
        Assert.IsFalse(config.HasUnsavedChanges);
        preview.InspectedProfile = AIProfileInspection.ActiveConnection;
        Assert.IsTrue(preview.HasInvalidProfileEndpoints);
        preview.ProfileIssues.Single().InspectCommand.Execute(null);
        Assert.IsTrue(inactiveEditor.IsVisible);
        Assert.IsFalse(inactive.TrySetValue("https://corrected.example"));
        Assert.IsFalse(config.HasUnsavedChanges);
        preview.InspectedProfile = AIProfileInspection.ActiveConnection;
        Assert.IsTrue(preview.ShowInvalidProfileFieldsCommand.CanExecute(null));
        preview.ShowInvalidProfileFieldsCommand.Execute(null);
        Assert.IsTrue(inactiveEditor.IsVisible);
        Assert.IsFalse(inactive.IsEditable);
        Assert.IsFalse(inactive.TrySetValue("https://corrected.example"));
        Assert.IsFalse(config.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task AIProviderProfiles_TemplateDriftUsesSelectedProfileNotLegacyOverride()
    {
        string root = Path.Combine(Path.GetTempPath(), "CodeStudioProfileTemplate", Guid.NewGuid().ToString("N"));
        try
        {
            using ServiceProvider services = App.CreateServiceProvider(static () => null, root);
            ConfigUiViewModel config = services.GetRequiredService<ConfigUiViewModel>();
            await config.LoadCommand.ExecuteAsync(null);
            using AIProviderDraftViewModel preview = new(config, services.GetRequiredService<CodeStudioSettings>());
            IPropertyItem Get(string name) => config.PropertyEditor.Items.Single(item => item.PropertyItem.Name == name).PropertyItem;
            Assert.IsTrue(Get(nameof(CodeStudioSettings.AIEndpoint)).TrySetValue("https://legacy.example/chat"));
            Assert.IsTrue(Get(nameof(CodeStudioSettings.UseAIProviderProfiles)).TrySetValue(true));

            string? warning = await preview.ConfirmCurrentTemplatesAsync(config.Sections.ToArray(),
                () => Task.FromResult<IReadOnlyList<AIProviderTemplate>?>(
                    [new() { Id = "Ollama", Endpoint = "https://updated.example/chat", DefaultModelId = "updated" }]));

            StringAssert.Contains(warning, "Review");
            StringAssert.Contains(preview.Summary, "updated.example");
            Assert.IsFalse(preview.Summary.Contains("legacy.example", StringComparison.Ordinal));
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }
    }

    [TestMethod]
    public async Task AIProviderDraft_CustomProviderDoesNotAllowTemplateActionsOrRevealCredentials()
    {
        string root = Path.Combine(Path.GetTempPath(), "CodeStudioCustomDraft", Guid.NewGuid().ToString("N"));
        try
        {
            using ServiceProvider services = App.CreateServiceProvider(static () => null, root);
            ConfigUiViewModel config = services.GetRequiredService<ConfigUiViewModel>();
            await config.LoadCommand.ExecuteAsync(null);
            using AIProviderDraftViewModel preview = new(config, services.GetRequiredService<CodeStudioSettings>());
            IPropertyItem Get(string name) => config.PropertyEditor.Items.Single(item => item.PropertyItem.Name == name).PropertyItem;
            Assert.IsTrue(Get(nameof(CodeStudioSettings.AIEndpoint)).TrySetValue("https://user:secret@example.test/chat"));
            Assert.IsTrue(Get(nameof(CodeStudioSettings.AIModelId)).TrySetValue("custom-model"));
            Assert.IsTrue(Get(nameof(CodeStudioSettings.AIProvider)).TrySetValue(AIProviderPreset.Custom));
            Assert.IsFalse(preview.UseTemplateEndpointCommand.CanExecute(null));
            Assert.IsFalse(preview.UseTemplateModelCommand.CanExecute(null));
            StringAssert.Contains(preview.Summary, "invalid");
            Assert.IsFalse(preview.Summary.Contains("secret", StringComparison.Ordinal));
            Assert.IsTrue(Get(nameof(CodeStudioSettings.AIEndpoint)).TrySetValue(string.Empty));
            StringAssert.Contains(preview.Summary, "(not configured) (unset)");
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }
    }

    [TestMethod]
    public async Task AIProviderDraft_DisablesTemplateActionForReadOnlyProperty()
    {
        IConfigUiSection section = Substitute.For<IConfigUiSection>();
        section.Section.Returns(new ConfigUiSection("Workbench", "Workbench", null, 0));
        section.State.Returns(ConfigUiSectionState.Ready);
        PropertyCategory category = new("Workbench", "Workbench");
        PropertyItem endpoint = new(category, nameof(CodeStudioSettings.AIEndpoint), "Endpoint", typeof(string),
            "https://policy.example/chat", isEditable: false);
        section.Properties.Returns(
        [
            endpoint,
            new PropertyItem(category, nameof(CodeStudioSettings.AIProvider), "Provider", typeof(AIProviderPreset), AIProviderPreset.OpenAI),
            new PropertyItem(category, nameof(CodeStudioSettings.AIModelId), "Model", typeof(string), "legacy"),
        ]);
        IConfigUiSectionRegistry registry = Substitute.For<IConfigUiSectionRegistry>();
        registry.Sections.Returns([section]);
        ConfigUiViewModel config = new(registry, new Property.Editor.Avalonia.PropertyEditorViewModel());
        await config.LoadCommand.ExecuteAsync(null);
        using AIProviderDraftViewModel preview = new(config, new CodeStudioSettings());

        Assert.IsFalse(preview.UseTemplateEndpointCommand.CanExecute(null));
        Assert.IsTrue(preview.UseTemplateModelCommand.CanExecute(null));
        Assert.AreEqual("https://policy.example/chat", endpoint.Value);
    }

    [TestMethod]
    public async Task AIProviderDraft_ChangedSystemTemplateRequiresReviewBeforeApply()
    {
        string root = Path.Combine(Path.GetTempPath(), "CodeStudioTemplateDrift", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            ConfigSectionRegistry registry = new();
            registry.AddSection(new CodeStudioSettingsProvider());
            JsonConfigLayerStore system = new(ConfigLayer.System, Path.Combine(root, "system.json"));
            JsonConfigLayerStore user = new(ConfigLayer.User, Path.Combine(root, "user.json"));
            ConfigLayeredSettingsResolver resolver = new(registry, [system, user]);
            ConfigUiViewModel config = new(
                new ConfigLayeredUiSectionRegistry(registry, resolver, user, ConfigLayer.User),
                new Property.Editor.Avalonia.PropertyEditorViewModel());
            CodeStudioSettings settings = new();
            var runtime = new AA98_AvlnCodeStudio.Base.AI.Models.OpenAICompatibleAISettings
            {
                Endpoint = settings.GetEffectiveAIEndpoint(),
                ModelId = settings.GetEffectiveAIModelId(),
            };
            WorkbenchSettingsRuntimeActivator activator = new(resolver, settings, runtime);
            await config.LoadCommand.ExecuteAsync(null);
            using AIProviderDraftViewModel preview = new(config, settings);
            config.ValidateChangesAsync = async sections =>
                await preview.ConfirmCurrentTemplatesAsync(sections, activator.LoadProviderTemplatesAsync)
                ?? await activator.ValidateChangesAsync(sections);
            config.SectionAppliedAsync = activator.ActivateAsync;
            IPropertyItem provider = config.PropertyEditor.Items.Single(
                item => item.PropertyItem.Name == nameof(CodeStudioSettings.AIProvider)).PropertyItem;
            Assert.IsTrue(provider.TrySetValue(AIProviderPreset.OpenAI));
            Assert.IsFalse(preview.CanManageActiveCredentials);
            await system.SaveAsync((await system.LoadAsync()).WithValue(
                new ConfigPropertyKey("Workbench", nameof(CodeStudioSettings.KnownAIProviderTemplates)),
                new ConfigLayerValue(new List<AIProviderTemplate>
                {
                    new() { Id = "OpenAI", Endpoint = "https://new.example/chat", DefaultModelId = "new-model" },
                })));

            Assert.IsFalse(await config.ApplyChangesAsync());
            StringAssert.Contains(config.OperationMessage, "Review");
            StringAssert.Contains(preview.Summary, "https://new.example/chat");
            StringAssert.Contains(preview.Summary, "new-model");
            Assert.IsTrue(config.HasUnsavedChanges);
            Assert.AreEqual("http://localhost:11434/v1/chat/completions", runtime.Endpoint);
            Assert.IsFalse((await user.LoadAsync()).TryGetValue(
                new ConfigPropertyKey("Workbench", nameof(CodeStudioSettings.AIProvider)), out _));
            Assert.IsTrue(await config.ApplyChangesAsync(), config.OperationMessage);
            Assert.AreEqual("https://new.example/chat", runtime.Endpoint);
            Assert.AreEqual("new-model", runtime.ModelId);
            Assert.IsTrue(preview.CanManageActiveCredentials);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task AIProviderDraft_OverriddenTemplateChangesDoNotBlockApply()
    {
        string root = Path.Combine(Path.GetTempPath(), "CodeStudioOverrideDrift", Guid.NewGuid().ToString("N"));
        try
        {
            using ServiceProvider services = App.CreateServiceProvider(static () => null, root);
            ConfigUiViewModel config = services.GetRequiredService<ConfigUiViewModel>();
            await config.LoadCommand.ExecuteAsync(null);
            using AIProviderDraftViewModel preview = new(config, services.GetRequiredService<CodeStudioSettings>());
            IPropertyItem Get(string name) => config.PropertyEditor.Items.Single(item => item.PropertyItem.Name == name).PropertyItem;
            Assert.IsTrue(preview.CanManageActiveCredentials);
            Assert.IsTrue(Get(nameof(CodeStudioSettings.AIEndpoint)).TrySetValue("https://override.example/chat"));
            Assert.IsTrue(Get(nameof(CodeStudioSettings.AIModelId)).TrySetValue("override-model"));
            Assert.IsFalse(preview.CanManageActiveCredentials);
            string? error = await preview.ConfirmCurrentTemplatesAsync(config.Sections.ToArray(),
                () => Task.FromResult<IReadOnlyList<AIProviderTemplate>?>(
                    [new() { Id = "Ollama", Endpoint = "https://changed.example", DefaultModelId = "changed" }]));
            Assert.IsNull(error);
            StringAssert.Contains(preview.Summary, "override.example");
            Assert.IsTrue(await config.DiscardChangesAsync());
            Assert.IsTrue(preview.CanManageActiveCredentials);
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }
    }

    [TestMethod]
    public async Task AIProviderDraft_TemplateReadFailureKeepsDraftAndBlocksPersistence()
    {
        string root = Path.Combine(Path.GetTempPath(), "CodeStudioTemplateReadFailure", Guid.NewGuid().ToString("N"));
        try
        {
            using ServiceProvider services = App.CreateServiceProvider(static () => null, root);
            ConfigUiViewModel config = services.GetRequiredService<ConfigUiViewModel>();
            await config.LoadCommand.ExecuteAsync(null);
            using AIProviderDraftViewModel preview = new(config, services.GetRequiredService<CodeStudioSettings>());
            config.ValidateChangesAsync = sections => preview.ConfirmCurrentTemplatesAsync(
                sections, () => Task.FromException<IReadOnlyList<AIProviderTemplate>?>(
                    new IOException("System layer unavailable")));
            IPropertyItem provider = config.PropertyEditor.Items.Single(
                item => item.PropertyItem.Name == nameof(CodeStudioSettings.AIProvider)).PropertyItem;
            Assert.IsTrue(provider.TrySetValue(AIProviderPreset.OpenAI));

            Assert.IsFalse(await config.ApplyChangesAsync());
            StringAssert.Contains(config.OperationMessage, "System layer unavailable");
            Assert.IsTrue(config.HasUnsavedChanges);
            StringAssert.Contains(preview.Summary, "api.openai.com");
            Assert.IsFalse((await services.GetRequiredService<JsonConfigLayerStore>().LoadAsync()).TryGetValue(
                new ConfigPropertyKey("Workbench", nameof(CodeStudioSettings.AIProvider)), out _));
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }
    }

    /// <summary>
    /// Verifies that the workbench composition injects the navigator into the
    /// diagnostics consumer and forwards activation to its editor services.
    /// </summary>
    [TestMethod]
    public async Task CreateServiceProvider_DiagnosticActivationUsesRegisteredEditorNavigator()
    {
        var documentHost = Substitute.For<IEditorDocumentHost>();
        var caretService = Substitute.For<IEditorCaretService>();
        var document = Substitute.For<IEditorDocument>();
        var location = new CodeLocation(Path.GetTempFileName(), 2, 3);
        documentHost.OpenOrActivateAsync(location.Path, Arg.Any<CancellationToken>()).Returns(document);

        ServiceCollection services = new();
        App.ConfigureServices(services, static () => null);
        services.AddSingleton(documentHost);
        services.AddSingleton(caretService);
        using var serviceProvider = services.BuildServiceProvider();

        var diagnostics = serviceProvider.GetRequiredService<DiagnosticCollectionViewModel>();

        await diagnostics.ActivateAsync(new AppKomponentBaseLib.Diagnostics.Diagnostic
        {
            SourcePath = location.Path,
            LineNumber = location.Line,
            ColumnNumber = location.Column,
        });

        await documentHost.Received(1).OpenOrActivateAsync(location.Path, Arg.Any<CancellationToken>());
        await caretService.Received(1).FocusAndMoveCaretAsync(
            document,
            Arg.Is<CodeLocation>(candidate => candidate == location),
            Arg.Any<CancellationToken>());
        Assert.AreEqual(string.Empty, diagnostics.ActivationErrorText);
    }

    /// <summary>
    /// Verifies that service-provider creation rejects a missing top-level provider delegate.
    /// </summary>
    [TestMethod]
    public void CreateServiceProvider_RejectsMissingTopLevelProvider()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => App.CreateServiceProvider(null!));
    }

    /// <summary>
    /// Verifies that desktop initialization wraps main-window creation failures with actionable diagnostics.
    /// </summary>
    [TestMethod]
    public void InitializeDesktop_WhenUiCreationFails_ThrowsStartupDiagnostic()
    {
        var app = new App();
        var desktop = Substitute.For<IClassicDesktopStyleApplicationLifetime>();

        var exception = Assert.ThrowsExactly<InvalidOperationException>(() => app.InitializeDesktop(desktop));

        Assert.IsNotNull(app.ServiceProvider);
        StringAssert.Contains(exception.Message, "AA98 shell startup failed during desktop initialization");
        Assert.IsInstanceOfType<InvalidOperationException>(exception.InnerException);
    }

    /// <summary>
    /// Verifies that desktop initialization rejects a missing desktop lifetime argument.
    /// </summary>
    [TestMethod]
    public void InitializeDesktop_RejectsMissingDesktopLifetime()
    {
        var app = new App();

        Assert.ThrowsExactly<ArgumentNullException>(() => app.InitializeDesktop(null!));
    }

    /// <summary>
    /// Verifies that main window creation reports a focused startup error when required registrations are missing.
    /// </summary>
    [TestMethod]
    public void CreateMainWindow_WhenRegistrationMissing_ThrowsStartupDiagnostic()
    {
        using var serviceProvider = new ServiceCollection().BuildServiceProvider();

        var exception = Assert.ThrowsExactly<InvalidOperationException>(() => App.CreateMainWindow(serviceProvider));

        StringAssert.Contains(exception.Message, "AA98 main window creation failed");
        Assert.IsNotNull(exception.InnerException);
    }

    /// <summary>
    /// Verifies that main-window creation rejects a missing service provider argument.
    /// </summary>
    [TestMethod]
    public void CreateMainWindow_RejectsMissingServiceProvider()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => App.CreateMainWindow(null!));
    }
}