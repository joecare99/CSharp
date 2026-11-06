using Config.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Property.Editor;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Config.UI.ConfigService.Tests;

/// <summary>
/// Verifies Config.Service persistence adaptation without product model types.
/// </summary>
[TestClass]
public sealed class ConfigServiceUiSectionTests
{
    [TestMethod]
    public async Task PropertyProviderAdapter_UsesSharedApplyAndDraftResetContracts()
    {
        IPropertyProvider provider = Substitute.For<IPropertyProvider>();
        provider.Context.Returns(new PropertyProviderContext(PropertyScope.Project, "Project", "Sample.csproj"));
        provider.Properties.Returns(Array.Empty<IPropertyItem>());
        provider.IsReadOnly.Returns(false);
        PropertyProviderConfigUiSection section = new("Project", "Project settings", provider);

        await section.LoadAsync();
        await section.SaveAsync();
        await section.ResetDraftAsync();

        await provider.Received(1).LoadAsync(Arg.Any<CancellationToken>());
        await provider.Received(1).ApplyAsync(Arg.Any<CancellationToken>());
        await provider.Received(1).ResetDraftAsync(Arg.Any<CancellationToken>());
        Assert.AreEqual(ConfigUiSectionState.Ready, section.State);
        Assert.AreEqual(PropertyScope.Project, section.Context.Scope);
    }

    [TestMethod]
    public void LayerPropertyContext_MapsInheritanceAndTrustedPolicyLock()
    {
        PropertyValueContext inherited = ConfigLayerPropertyContext.Create(
            new ConfigValueResolution("default-model", ConfigValueSource.ProductDefault, IsEnforced: false),
            ConfigLayer.User);
        PropertyValueContext policy = ConfigLayerPropertyContext.Create(
            new ConfigValueResolution("policy-model", ConfigValueSource.System, IsEnforced: true),
            ConfigLayer.User,
            origin: "C:\\ProgramData\\Company\\policy.json");

        Assert.AreEqual(PropertyScope.Application, inherited.Scope);
        Assert.IsTrue(inherited.IsInherited);
        Assert.AreEqual("Product default", inherited.Origin);
        Assert.AreEqual(PropertyScope.System, policy.Scope);
        Assert.IsTrue(policy.IsInherited);
        Assert.IsTrue(policy.IsPolicyLocked);
        Assert.AreEqual("C:\\ProgramData\\Company\\policy.json", policy.Origin);
        Assert.IsFalse(string.IsNullOrWhiteSpace(policy.ReadOnlyReason));
    }

    [TestMethod]
    public void LayerPropertyContext_ValueFromEditedLayerIsNotInherited()
    {
        PropertyValueContext context = ConfigLayerPropertyContext.Create(
            new ConfigValueResolution("user-model", ConfigValueSource.User, IsEnforced: false),
            ConfigLayer.User);

        Assert.AreEqual(PropertyScope.User, context.Scope);
        Assert.IsFalse(context.IsInherited);
        Assert.IsFalse(context.IsPolicyLocked);
    }

    [TestMethod]
    public async Task LayeredUiSection_LoadsEffectiveValueAndAppliesOnlyToSelectedLayer()
    {
        string root = CreateTempDirectory();
        try
        {
            ConfigSectionRegistry registry = new();
            LayeredSectionProvider provider = new();
            registry.AddSection(provider);
            JsonConfigLayerStore userStore = new(ConfigLayer.User, Path.Combine(root, "user.json"));
            ConfigLayeredSettingsResolver resolver = new(registry, [userStore]);
            ConfigLayeredUiSection section = new(
                resolver,
                provider,
                userStore,
                ConfigLayer.User);

            await section.LoadAsync();

            IPropertyItem name = section.Properties.Single(static property => property.Name == nameof(LayeredSettings.Name));
            Assert.AreEqual("product-default", name.Value);
            Assert.AreEqual(PropertyScope.Application, name.ValueContext.Scope);
            Assert.IsTrue(name.ValueContext.IsInherited);
            Assert.IsTrue(name.TrySetValue("user-value"));
            await section.SaveAsync();

            ConfigLayerSnapshot saved = await userStore.LoadAsync();
            Assert.IsTrue(saved.TryGetValue(new ConfigPropertyKey("General", nameof(LayeredSettings.Name)), out ConfigLayerValue? savedValue));
            Assert.AreEqual("user-value", ((System.Text.Json.JsonElement)savedValue.Value!).GetString());
            Assert.AreEqual(ConfigUiSectionState.Ready, section.State);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task LayeredUiSection_AllowsReplacingUnsupportedSavedEnumValue()
    {
        string root = CreateTempDirectory();
        try
        {
            ConfigSectionRegistry registry = new();
            EnumSettingProvider provider = new();
            registry.AddSection(provider);
            JsonConfigLayerStore userStore = new(ConfigLayer.User, Path.Combine(root, "user.json"));
            ConfigLayerSnapshot draft = (await userStore.LoadAsync()).WithValue(
                new ConfigPropertyKey("Modes", nameof(EnumSettings.Mode)),
                new ConfigLayerValue(CredentialMode.Unsupported));
            await userStore.SaveAsync(draft);
            ConfigLayeredSettingsResolver resolver = new(registry, [userStore]);
            ConfigLayeredUiSection section = new(
                resolver,
                provider,
                userStore,
                ConfigLayer.User,
                optionsProvider: static (_, property) =>
                    property.Name == nameof(EnumSettings.Mode)
                        ? [new PropertyOption(CredentialMode.None, "None"), new PropertyOption(CredentialMode.Supported, "Supported")]
                        : null);

            await section.LoadAsync();

            IPropertyItem mode = section.Properties.Single(static property => property.Name == nameof(EnumSettings.Mode));
            Assert.AreEqual(CredentialMode.Unsupported, mode.Value);
            Assert.IsTrue(mode.IsEditable);
            StringAssert.Contains(mode.Options.Single(option => Equals(option.Value, CredentialMode.Unsupported)).DisplayName, "Unavailable");
            Assert.IsTrue(mode.TrySetValue(CredentialMode.Supported));
            await section.SaveAsync();
            Assert.AreEqual(ConfigUiSectionState.Ready, section.State);
            Assert.AreEqual(CredentialMode.Supported, section.Properties.Single().Value);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task LayeredUiSection_ResetDraftRevealsLowerLayerWithoutPersisting()
    {
        string root = CreateTempDirectory();
        try
        {
            ConfigSectionRegistry registry = new();
            LayeredSectionProvider provider = new();
            registry.AddSection(provider);
            JsonConfigLayerStore systemStore = new(ConfigLayer.System, Path.Combine(root, "system.json"));
            JsonConfigLayerStore userStore = new(ConfigLayer.User, Path.Combine(root, "user.json"));
            await systemStore.SaveAsync((await systemStore.LoadAsync()).WithValue(
                new ConfigPropertyKey("General", nameof(LayeredSettings.Name)),
                new ConfigLayerValue("system-value")));
            ConfigLayerSnapshot userDraft = (await userStore.LoadAsync())
                .WithValue(
                    new ConfigPropertyKey("General", nameof(LayeredSettings.Name)),
                    new ConfigLayerValue("user-value"))
                .WithValue(
                    new ConfigPropertyKey("General", nameof(LayeredSettings.HiddenButPersisted)),
                    new ConfigLayerValue("legacy-value"));
            await userStore.SaveAsync(userDraft);
            ConfigLayeredSettingsResolver resolver = new(registry, [systemStore, userStore]);
            ConfigLayeredUiSection section = new(resolver, provider, userStore, ConfigLayer.User);

            await section.LoadAsync();
            await section.ResetDraftAsync();

            IPropertyItem name = section.Properties.Single(static property => property.Name == nameof(LayeredSettings.Name));
            Assert.AreEqual("system-value", name.Value);
            Assert.AreEqual(PropertyScope.System, name.ValueContext.Scope);
            Assert.IsTrue(name.ValueContext.IsInherited);
            ConfigLayerSnapshot persisted = await userStore.LoadAsync();
            Assert.IsTrue(persisted.TryGetValue(
                new ConfigPropertyKey("General", nameof(LayeredSettings.Name)),
                out ConfigLayerValue? persistedValue));
            Assert.AreEqual("user-value", ((System.Text.Json.JsonElement)persistedValue.Value!).GetString());
            Assert.IsTrue(persisted.TryGetValue(
                new ConfigPropertyKey("General", nameof(LayeredSettings.HiddenButPersisted)),
                out ConfigLayerValue? persistedLegacyValue));
            Assert.AreEqual("legacy-value", ((System.Text.Json.JsonElement)persistedLegacyValue.Value!).GetString());
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task LayeredUiSection_DisablesTrustedEnforcedPolicyValue()
    {
        string root = CreateTempDirectory();
        try
        {
            ConfigSectionRegistry registry = new();
            LayeredSectionProvider provider = new();
            registry.AddSection(provider);
            JsonConfigLayerStore systemStore = new(ConfigLayer.System, Path.Combine(root, "system.json"));
            JsonConfigLayerStore userStore = new(ConfigLayer.User, Path.Combine(root, "user.json"));
            await systemStore.SaveAsync((await systemStore.LoadAsync()).WithValue(
                new ConfigPropertyKey("General", nameof(LayeredSettings.Name)),
                new ConfigLayerValue("policy-value", IsEnforced: true)));
            ConfigLayeredSettingsResolver resolver = new(registry, [systemStore, userStore], trustEnforcedSystemValues: true);
            ConfigLayeredUiSection section = new(resolver, provider, userStore, ConfigLayer.User);

            await section.LoadAsync();

            IPropertyItem name = section.Properties.Single(static property => property.Name == nameof(LayeredSettings.Name));
            Assert.AreEqual("policy-value", name.Value);
            Assert.IsFalse(name.IsEditable);
            Assert.IsTrue(name.ValueContext.IsPolicyLocked);
            Assert.IsFalse(string.IsNullOrWhiteSpace(name.ReadOnlyReason));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task LayeredUiSection_ResetAsyncPersistsRemovalOfTargetOverride()
    {
        string root = CreateTempDirectory();
        try
        {
            ConfigSectionRegistry registry = new();
            LayeredSectionProvider provider = new();
            registry.AddSection(provider);
            JsonConfigLayerStore systemStore = new(ConfigLayer.System, Path.Combine(root, "system.json"));
            JsonConfigLayerStore userStore = new(ConfigLayer.User, Path.Combine(root, "user.json"));
            await systemStore.SaveAsync((await systemStore.LoadAsync()).WithValue(
                new ConfigPropertyKey("General", nameof(LayeredSettings.Name)),
                new ConfigLayerValue("system-value")));
            await userStore.SaveAsync((await userStore.LoadAsync())
                .WithValue(
                    new ConfigPropertyKey("General", nameof(LayeredSettings.Name)),
                    new ConfigLayerValue("user-value"))
                .WithValue(
                    new ConfigPropertyKey("General", nameof(LayeredSettings.HiddenButPersisted)),
                    new ConfigLayerValue("legacy-value")));
            ConfigLayeredSettingsResolver resolver = new(registry, [systemStore, userStore]);
            ConfigLayeredUiSection section = new(resolver, provider, userStore, ConfigLayer.User);

            await section.LoadAsync();
            await section.ResetAsync();

            Assert.AreEqual("system-value", section.Properties.Single(static property => property.Name == nameof(LayeredSettings.Name)).Value);
            ConfigLayerSnapshot persisted = await userStore.LoadAsync();
            Assert.IsFalse(persisted.TryGetValue(
                new ConfigPropertyKey("General", nameof(LayeredSettings.Name)),
                out _));
            Assert.IsTrue(persisted.TryGetValue(
                new ConfigPropertyKey("General", nameof(LayeredSettings.HiddenButPersisted)),
                out ConfigLayerValue? legacyValue));
            Assert.AreEqual("legacy-value", ((System.Text.Json.JsonElement)legacyValue.Value!).GetString());
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task LayeredUiSection_StaleDraftIsReportedAndNotPersisted()
    {
        string root = CreateTempDirectory();
        try
        {
            ConfigSectionRegistry registry = new();
            LayeredSectionProvider provider = new();
            registry.AddSection(provider);
            JsonConfigLayerStore userStore = new(ConfigLayer.User, Path.Combine(root, "user.json"));
            ConfigLayeredSettingsResolver resolver = new(registry, [userStore]);
            ConfigLayeredUiSection section = new(resolver, provider, userStore, ConfigLayer.User);
            await section.LoadAsync();

            await userStore.SaveAsync((await userStore.LoadAsync()).WithValue(
                new ConfigPropertyKey("General", nameof(LayeredSettings.Name)),
                new ConfigLayerValue("external-value")));
            Assert.IsTrue(section.Properties.Single(static property => property.Name == nameof(LayeredSettings.Name)).TrySetValue("draft-value"));
            await section.SaveAsync();

            Assert.AreEqual(ConfigUiSectionState.Failed, section.State);
            StringAssert.Contains(section.ErrorMessage!, "changed since it was loaded");
            ConfigLayerSnapshot persisted = await userStore.LoadAsync();
            Assert.IsTrue(persisted.TryGetValue(
                new ConfigPropertyKey("General", nameof(LayeredSettings.Name)),
                out ConfigLayerValue? persistedValue));
            Assert.AreEqual("external-value", ((System.Text.Json.JsonElement)persistedValue.Value!).GetString());
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task LayeredUiSection_CorruptLayerDataFailsWithoutUsingProductDefaults()
    {
        string root = CreateTempDirectory();
        try
        {
            ConfigSectionRegistry registry = new();
            LayeredSectionProvider provider = new();
            registry.AddSection(provider);
            string path = Path.Combine(root, "user.json");
            await File.WriteAllTextAsync(path, "{ invalid json");
            JsonConfigLayerStore userStore = new(ConfigLayer.User, path);
            ConfigLayeredSettingsResolver resolver = new(registry, [userStore]);
            ConfigLayeredUiSection section = new(resolver, provider, userStore, ConfigLayer.User);

            await section.LoadAsync();

            Assert.AreEqual(ConfigUiSectionState.Failed, section.State);
            StringAssert.Contains(section.ErrorMessage!, "invalid JSON");
            Assert.AreEqual(0, section.Properties.Count);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task LayeredUiSection_ResetPreservesTrustedEnforcedSystemPolicy()
    {
        string root = CreateTempDirectory();
        try
        {
            ConfigSectionRegistry registry = new();
            LayeredSectionProvider provider = new();
            registry.AddSection(provider);
            JsonConfigLayerStore systemStore = new(ConfigLayer.System, Path.Combine(root, "system.json"));
            await systemStore.SaveAsync((await systemStore.LoadAsync()).WithValue(
                new ConfigPropertyKey("General", nameof(LayeredSettings.Name)),
                new ConfigLayerValue("policy-value", IsEnforced: true)));
            ConfigLayeredSettingsResolver resolver = new(registry, [systemStore], trustEnforcedSystemValues: true);
            ConfigLayeredUiSection section = new(resolver, provider, systemStore, ConfigLayer.System);

            await section.LoadAsync();
            await section.ResetAsync();

            Assert.AreEqual("policy-value", section.Properties.Single(static property => property.Name == nameof(LayeredSettings.Name)).Value);
            ConfigLayerSnapshot persisted = await systemStore.LoadAsync();
            Assert.IsTrue(persisted.TryGetValue(
                new ConfigPropertyKey("General", nameof(LayeredSettings.Name)),
                out ConfigLayerValue? persistedValue));
            Assert.IsTrue(persistedValue.IsEnforced);
            Assert.AreEqual("policy-value", ((System.Text.Json.JsonElement)persistedValue.Value!).GetString());
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public void LayeredUiSectionRegistry_CreatesOrderedSectionsForOneExplicitLayer()
    {
        ConfigSectionRegistry registry = new();
        LayeredSectionProvider later = new("Zeta", order: 20);
        LayeredSectionProvider earlier = new("Alpha", order: 10);
        registry.AddSection(later);
        registry.AddSection(earlier);
        JsonConfigLayerStore store = new(ConfigLayer.Group, Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "group.json"));
        ConfigLayeredSettingsResolver resolver = new(registry, [store]);

        ConfigLayeredUiSectionRegistry uiRegistry = new(registry, resolver, store, ConfigLayer.Group, isReadOnly: true);

        Assert.AreEqual(2, uiRegistry.Sections.Count);
        Assert.AreEqual("Alpha", uiRegistry.Sections[0].Section.Name);
        Assert.AreEqual("Zeta", uiRegistry.Sections[1].Section.Name);
        Assert.IsTrue(uiRegistry.Sections.All(static section => section.IsReadOnly));
        Assert.IsTrue(uiRegistry.Sections.All(static section =>
            ((IPropertyProvider)section).Context.Scope == PropertyScope.Group));
        Assert.ThrowsExactly<ArgumentException>(() =>
            new ConfigLayeredUiSectionRegistry(
                registry,
                resolver,
                store,
                ConfigLayer.User));
    }

    [TestMethod]
    public async Task LayeredUiSectionRegistry_SharesDraftAndStorageVersionAcrossSections()
    {
        string root = CreateTempDirectory();
        try
        {
            ConfigSectionRegistry registry = new();
            LayeredSectionProvider generalProvider = new("General", order: 10);
            LayeredSectionProvider editorProvider = new("Editor", order: 20);
            registry.AddSection(generalProvider);
            registry.AddSection(editorProvider);
            JsonConfigLayerStore store = new(ConfigLayer.User, Path.Combine(root, "user.json"));
            ConfigLayeredSettingsResolver resolver = new(registry, [store]);
            ConfigLayeredUiSectionRegistry uiRegistry = new(registry, resolver, store, ConfigLayer.User);
            IConfigUiSection general = uiRegistry.Sections.Single(static section => section.Section.Name == "General");
            IConfigUiSection editor = uiRegistry.Sections.Single(static section => section.Section.Name == "Editor");

            await general.LoadAsync();
            await editor.LoadAsync();
            Assert.IsTrue(general.Properties.Single(static property => property.Name == nameof(LayeredSettings.Name)).TrySetValue("general-value"));
            Assert.IsTrue(editor.Properties.Single(static property => property.Name == nameof(LayeredSettings.Name)).TrySetValue("editor-value"));
            await general.SaveAsync();
            await editor.SaveAsync();

            ConfigLayerSnapshot saved = await store.LoadAsync();
            Assert.AreEqual(
                "general-value",
                ((System.Text.Json.JsonElement)saved.Values[new ConfigPropertyKey("General", nameof(LayeredSettings.Name))].Value!).GetString());
            Assert.AreEqual(
                "editor-value",
                ((System.Text.Json.JsonElement)saved.Values[new ConfigPropertyKey("Editor", nameof(LayeredSettings.Name))].Value!).GetString());
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task LayeredUiSection_DiscardReloadsSharedDraftFromStorage()
    {
        string root = CreateTempDirectory();
        try
        {
            ConfigSectionRegistry registry = new();
            LayeredSectionProvider generalProvider = new("General", order: 10);
            LayeredSectionProvider editorProvider = new("Editor", order: 20);
            registry.AddSection(generalProvider);
            registry.AddSection(editorProvider);
            JsonConfigLayerStore store = new(ConfigLayer.User, Path.Combine(root, "user.json"));
            ConfigLayeredSettingsResolver resolver = new(registry, [store]);
            ConfigLayeredUiSectionRegistry uiRegistry = new(registry, resolver, store, ConfigLayer.User);
            IConfigUiSection general = uiRegistry.Sections.Single(static section => section.Section.Name == "General");
            IConfigUiSection editor = uiRegistry.Sections.Single(static section => section.Section.Name == "Editor");
            await general.LoadAsync();
            await editor.LoadAsync();
            Assert.IsTrue(general.Properties.Single(static property => property.Name == nameof(LayeredSettings.Name)).TrySetValue("draft-general"));
            Assert.IsTrue(editor.Properties.Single(static property => property.Name == nameof(LayeredSettings.Name)).TrySetValue("draft-editor"));

            await ((IPropertyProvider)general).DiscardAsync();
            await ((IPropertyProvider)editor).DiscardAsync();

            Assert.AreEqual("product-default", general.Properties.Single(static property => property.Name == nameof(LayeredSettings.Name)).Value);
            Assert.AreEqual("product-default", editor.Properties.Single(static property => property.Name == nameof(LayeredSettings.Name)).Value);
            Assert.AreEqual(0, (await store.LoadAsync()).Values.Count);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task PropertyProviderAdapter_ApplyFailure_ExposesErrorAndFailureState()
    {
        IPropertyProvider provider = Substitute.For<IPropertyProvider>();
        provider.ApplyAsync(Arg.Any<CancellationToken>())
            .Returns<Task>(_ => throw new InvalidOperationException("Review was declined."));
        PropertyProviderConfigUiSection section = new("Project", "Project settings", provider);

        await section.SaveAsync();

        Assert.AreEqual(ConfigUiSectionState.Failed, section.State);
        Assert.AreEqual("Review was declined.", section.ErrorMessage);
    }

    [TestMethod]
    public async Task LoadAsync_ValidModel_ExposesTypedEditableProperties()
    {
        IConfigStore store = Substitute.For<IConfigStore>();
        TestConfig model = new() { Server = "database", Port = 3306, Mode = TestMode.Automatic };
        store.LoadAsync("TestHost.General", Arg.Any<TestConfig>()).Returns(Task.FromResult(model));
        ConfigServiceUiSection section = new(new global::Config.Service.ConfigService(store, "TestHost"), new TestSectionProvider());

        await section.LoadAsync();

        Assert.AreEqual(ConfigUiSectionState.Ready, section.State);
        Assert.AreEqual(4, section.Properties.Count);
        Assert.AreEqual(PropertyScope.User, ((IPropertyProvider)section).Context.Scope);
        Assert.AreEqual(PropertyScope.User, section.Properties.Single(static property => property.Name == "Server").ValueContext.Scope);
        Assert.AreEqual("database", section.Properties.Single(static property => property.Name == "Server").Value);
        Assert.AreEqual(2, section.Properties.Single(static property => property.Name == "Mode").Options.Count);
        Assert.IsTrue(section.Properties.Single(static property => property.Name == "Password").IsSensitive);
        Assert.IsFalse(section.Properties.Any(static property => property.Name == "HiddenButPersisted"));
    }

    [TestMethod]
    public async Task SaveAsync_ChangedProperty_PersistsUpdatedModel()
    {
        IConfigStore store = Substitute.For<IConfigStore>();
        TestConfig model = new() { Server = "database" };
        store.LoadAsync("TestHost.General", Arg.Any<TestConfig>()).Returns(Task.FromResult(model));
        ConfigServiceUiSection section = new(new global::Config.Service.ConfigService(store, "TestHost"), new TestSectionProvider());
        await section.LoadAsync();

        Assert.IsTrue(section.Properties.Single(static property => property.Name == "Server").TrySetValue("updated"));
        await ((IPropertyProvider)section).ApplyAsync();

        await store.Received(1).SaveAsync(
            "TestHost.General",
            Arg.Is<TestConfig>(saved => saved.Server == "updated" && saved.HiddenButPersisted == "legacy"));
        Assert.AreEqual(ConfigUiSectionState.Ready, section.State);
    }

    [TestMethod]
    public async Task ResetAsync_ReadOnlySection_DoesNotMutateStore()
    {
        IConfigStore store = Substitute.For<IConfigStore>();
        ConfigServiceUiSection section = new(new global::Config.Service.ConfigService(store, "TestHost"), new TestSectionProvider(), isReadOnly: true);

        await section.ResetAsync();

        await store.DidNotReceive().ResetAsync(Arg.Any<string>());
        Assert.AreEqual(ConfigUiSectionState.Failed, section.State);
        Assert.IsNotNull(section.ErrorMessage);
    }

    [TestMethod]
    public async Task ResetDraftAsync_EditableSection_ResetsDraftWithoutPersisting()
    {
        IConfigStore store = Substitute.For<IConfigStore>();
        TestConfig model = new() { Server = "custom" };
        store.LoadAsync("TestHost.General", Arg.Any<TestConfig>()).Returns(Task.FromResult(model));
        ConfigServiceUiSection section = new(new global::Config.Service.ConfigService(store, "TestHost"), new TestSectionProvider());
        await section.LoadAsync();
        Assert.IsTrue(section.Properties.Single(static property => property.Name == "Server").TrySetValue("changed"));

        await section.ResetDraftAsync();

        await store.DidNotReceive().ResetAsync(Arg.Any<string>());
        await store.DidNotReceive().SaveAsync(Arg.Any<string>(), Arg.Any<TestConfig>());
        Assert.IsNull(section.Properties.Single(static property => property.Name == "Server").Value);
        Assert.AreEqual(ConfigUiSectionState.Ready, section.State);
    }

    [TestMethod]
    public async Task ResetAsync_EditableSection_ResetsPersistedValues()
    {
        IConfigStore store = Substitute.For<IConfigStore>();
        store.ResetAsync("TestHost.General").Returns(Task.CompletedTask);
        ConfigServiceUiSection section = new(new global::Config.Service.ConfigService(store, "TestHost"), new TestSectionProvider());

        await section.ResetAsync();

        await store.Received(1).ResetAsync("TestHost.General");
        Assert.AreEqual(ConfigUiSectionState.Ready, section.State);
        Assert.AreEqual(1, section.Properties.Single(static property => property.Name == "Port").Value);
    }

    [TestMethod]
    public async Task SaveAsync_InvalidProperty_DoesNotPersist()
    {
        IConfigStore store = Substitute.For<IConfigStore>();
        TestConfig model = new() { Port = 3306 };
        store.LoadAsync("TestHost.General", Arg.Any<TestConfig>()).Returns(Task.FromResult(model));
        ConfigServiceUiSection section = new(new global::Config.Service.ConfigService(store, "TestHost"), new TestSectionProvider());
        await section.LoadAsync();

        Assert.IsFalse(section.Properties.Single(static property => property.Name == "Port").TrySetValue("invalid"));
        await section.SaveAsync();

        await store.DidNotReceive().SaveAsync(Arg.Any<string>(), Arg.Any<TestConfig>());
        Assert.AreEqual(ConfigUiSectionState.Failed, section.State);
    }

    private sealed class TestSectionProvider : IConfigSectionProvider
    {
        public string Name => "General";
        public string DisplayName => "General settings";
        public string? Description => null;
        public int Order => 0;
        public Type ModelType => typeof(TestConfig);
        public object CreateModel() => new TestConfig();
    }

    private sealed class TestConfig
    {
        public string? Server { get; set; }
        public int Port { get; set; } = 1;
        public TestMode Mode { get; set; }
        [SensitiveConfigProperty]
        public string? Password { get; set; }
        [ConfigUiIgnore]
        public string HiddenButPersisted { get; set; } = "legacy";
        [ConfigIgnore]
        public string Ignored => "ignored";
    }

    private static string CreateTempDirectory()
    {
        string root = Path.Combine(Path.GetTempPath(), "ConfigLayeredUiSectionTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        return root;
    }

    private sealed class LayeredSettings
    {
        public string Name { get; set; } = "product-default";

        [ConfigUiIgnore]
        public string HiddenButPersisted { get; set; } = "legacy";
    }

    private sealed class LayeredSectionProvider : IConfigSectionProvider
    {
        public LayeredSectionProvider(string name = "General", int order = 0)
        {
            Name = name;
            Order = order;
        }

        public string Name { get; }
        public string DisplayName => $"{Name} settings";
        public string? Description => null;
        public int Order { get; }
        public Type ModelType => typeof(LayeredSettings);
        public object CreateModel() => new LayeredSettings();
    }

    private sealed class EnumSettings
    {
        public CredentialMode Mode { get; set; } = CredentialMode.None;
    }

    private sealed class EnumSettingProvider : IConfigSectionProvider
    {
        public string Name => "Modes";
        public string DisplayName => "Mode settings";
        public string? Description => null;
        public int Order => 0;
        public Type ModelType => typeof(EnumSettings);
        public object CreateModel() => new EnumSettings();
    }

    private enum TestMode
    {
        Manual,
        Automatic,
    }

    private enum CredentialMode
    {
        None,
        Supported,
        Unsupported,
    }
}
