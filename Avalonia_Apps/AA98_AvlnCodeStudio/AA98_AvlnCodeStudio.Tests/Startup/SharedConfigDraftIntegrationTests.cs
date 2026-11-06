using Config.Service;
using Config.UI;
using Config.UI.Avalonia.ViewModels;
using Config.UI.ConfigService;
using AA98_AvlnCodeStudio.UI.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Property.Editor;
using Property.Editor.Avalonia;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Startup;

[TestClass]
public sealed class SharedConfigDraftIntegrationTests
{
    [TestMethod]
    public async Task ReloadFailureAfterSnapshotReplacement_BlocksStaleDraftUntilRetry()
    {
        ConfigSectionRegistry registry = new();
        registry.AddSection(CreateProvider("First", 0));
        registry.AddSection(CreateProvider("Second", 1));
        IConfigLayerStore user = Substitute.For<IConfigLayerStore>();
        IConfigLayerStore system = Substitute.For<IConfigLayerStore>();
        user.Layer.Returns(ConfigLayer.User);
        system.Layer.Returns(ConfigLayer.System);
        user.LoadAsync(Arg.Any<CancellationToken>()).Returns(new ConfigLayerSnapshot(ConfigLayer.User, []));
        bool failSystem = false;
        system.LoadAsync(Arg.Any<CancellationToken>()).Returns(_ => failSystem
            ? Task.FromException<ConfigLayerSnapshot>(new IOException("System layer unavailable"))
            : Task.FromResult(new ConfigLayerSnapshot(ConfigLayer.System, [])));
        ConfigLayeredSettingsResolver resolver = new(registry, [system, user]);
        ConfigUiViewModel config = new(
            new ConfigLayeredUiSectionRegistry(registry, resolver, user, ConfigLayer.User),
            new PropertyEditorViewModel());

        IPropertyItem first = config.PropertyEditor.Items.Single().PropertyItem;
        Assert.IsTrue(first.TrySetValue("First draft"));
        config.SelectedSection = config.Sections[1];
        IPropertyItem second = config.PropertyEditor.Items.Single().PropertyItem;
        Assert.IsTrue(second.TrySetValue("Second draft"));
        user.ClearReceivedCalls();
        failSystem = true;

        await config.LoadCommand.ExecuteAsync(null);

        await user.Received().LoadAsync(Arg.Any<CancellationToken>());
        Assert.IsTrue(config.HasUnsavedChanges);
        Assert.IsFalse(config.CanApplyChanges);
        Assert.IsTrue(config.NeedsReloadRecovery);
        Assert.AreEqual(0, config.PropertyEditor.Items.Count);
        Assert.IsFalse(first.TrySetValue("Stale first"));
        Assert.IsFalse(second.TrySetValue("Stale second"));
        StringAssert.Contains(first.ValidationResult.ErrorMessage, "replaced");
        StringAssert.Contains(second.ValidationResult.ErrorMessage, "replaced");
        StringAssert.Contains(config.OperationMessage, "may already have been discarded");
        config.SelectedSection = config.Sections[0];
        Assert.AreEqual(0, config.PropertyEditor.Items.Count);
        Assert.IsFalse(await config.ApplyChangesAsync());

        failSystem = false;
        await config.RetryFailedReloadsCommand.ExecuteAsync(null);
        Assert.IsFalse(config.NeedsReloadRecovery);
        Assert.IsFalse(config.HasUnsavedChanges);
        Assert.AreEqual("Default", config.PropertyEditor.Items.Single().PropertyItem.Value);
        config.SelectedSection = config.Sections[1];
        Assert.AreEqual("Default", config.PropertyEditor.Items.Single().PropertyItem.Value);
    }

    [TestMethod]
    public async Task ApplyFailureAfterStoreWrite_ReportsSavedValuesAndRetriesActivation()
    {
        ConfigSectionRegistry registry = new();
        registry.AddSection(CreateProvider("First", 0));
        IConfigLayerStore user = Substitute.For<IConfigLayerStore>();
        IConfigLayerStore system = Substitute.For<IConfigLayerStore>();
        user.Layer.Returns(ConfigLayer.User);
        system.Layer.Returns(ConfigLayer.System);
        ConfigLayerSnapshot persisted = new(ConfigLayer.User, []);
        bool failRefresh = false;
        user.LoadAsync(Arg.Any<CancellationToken>()).Returns(_ => Task.FromResult(persisted));
        user.SaveAsync(Arg.Any<ConfigLayerSnapshot>(), Arg.Any<CancellationToken>()).Returns(call =>
        {
            persisted = call.Arg<ConfigLayerSnapshot>();
            failRefresh = true;
            return Task.FromResult(persisted);
        });
        system.LoadAsync(Arg.Any<CancellationToken>()).Returns(_ => failRefresh
            ? Task.FromException<ConfigLayerSnapshot>(new IOException("System layer unavailable"))
            : Task.FromResult(new ConfigLayerSnapshot(ConfigLayer.System, [])));
        ConfigLayeredSettingsResolver resolver = new(registry, [system, user]);
        ConfigUiViewModel config = new(
            new ConfigLayeredUiSectionRegistry(registry, resolver, user, ConfigLayer.User),
            new PropertyEditorViewModel());
        int activated = 0;
        config.SectionAppliedAsync = _ =>
        {
            activated++;
            return Task.CompletedTask;
        };
        Assert.IsTrue(config.PropertyEditor.Items.Single().PropertyItem.TrySetValue("saved"));

        Assert.IsFalse(await config.ApplyChangesAsync());

        Assert.IsTrue(persisted.TryGetValue(new ConfigPropertyKey("First", nameof(TestSettings.Name)), out ConfigLayerValue? saved));
        Assert.AreEqual("saved", saved.Value);
        Assert.IsTrue(config.HasUnsavedChanges);
        Assert.AreEqual(0, activated);
        StringAssert.Contains(config.OperationMessage, "were saved");
        StringAssert.Contains(config.OperationMessage, "could not be refreshed");
        StringAssert.Contains(config.OperationMessage, "Runtime activation was not attempted");

        user.SaveAsync(Arg.Any<ConfigLayerSnapshot>(), Arg.Any<CancellationToken>())
            .Returns(call => Task.FromResult(call.Arg<ConfigLayerSnapshot>()));
        failRefresh = false;
        Assert.IsTrue(await config.ApplyChangesAsync(), config.OperationMessage);
        Assert.AreEqual(1, activated);
        Assert.IsFalse(config.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task AIProviderPreview_DisabledForSharedDraftRecoveryAndRestoredAfterRetry()
    {
        IConfigUiSection workbench = Substitute.For<IConfigUiSection, IConfigUiDraftDiscard>();
        IConfigUiSection other = Substitute.For<IConfigUiSection, IConfigUiDraftDiscard>();
        object identity = new();
        ((IConfigUiDraftDiscard)workbench).DraftIdentity.Returns(identity);
        ((IConfigUiDraftDiscard)other).DraftIdentity.Returns(identity);
        workbench.Section.Returns(new ConfigUiSection("Workbench", "Workbench"));
        other.Section.Returns(new ConfigUiSection("Other", "Other"));
        workbench.State.Returns(ConfigUiSectionState.Ready);
        other.State.Returns(ConfigUiSectionState.Ready);
        PropertyCategory category = new("Workbench", "Workbench");
        PropertyItem endpoint = new(category, nameof(CodeStudioSettings.AIEndpoint), "Endpoint", typeof(string), "https://example.test");
        workbench.Properties.Returns(
        [
            new PropertyItem(category, nameof(CodeStudioSettings.AIProvider), "Provider", typeof(AIProviderPreset), AIProviderPreset.OpenAI),
            endpoint,
            new PropertyItem(category, nameof(CodeStudioSettings.AIModelId), "Model", typeof(string), "custom-model"),
        ]);
        other.Properties.Returns([]);
        IConfigUiSectionRegistry registry = Substitute.For<IConfigUiSectionRegistry>();
        registry.Sections.Returns([workbench, other]);
        ConfigUiViewModel config = new(registry, new PropertyEditorViewModel());
        using AIProviderDraftViewModel preview = new(config, new CodeStudioSettings());
        Assert.IsTrue(preview.CanManageActiveCredentials);
        config.SelectedSection = other;
        bool fail = true;
        workbench.LoadAsync().Returns(_ => fail
            ? Task.FromException(new IOException("Refresh failed"))
            : Task.CompletedTask);
        await config.LoadCommand.ExecuteAsync(null);
        config.SelectedSection = workbench;

        Assert.IsTrue(config.NeedsReloadRecovery);
        Assert.IsFalse(preview.IsVisible);
        Assert.IsFalse(preview.CanManageActiveCredentials);
        Assert.IsFalse(preview.UseTemplateEndpointCommand.CanExecute(null));
        preview.UseTemplateEndpointCommand.Execute(null);
        Assert.AreEqual("https://example.test", endpoint.Value);

        fail = false;
        await config.RetryFailedReloadsCommand.ExecuteAsync(null);
        Assert.IsFalse(config.NeedsReloadRecovery);
        Assert.IsTrue(preview.IsVisible);
        Assert.IsTrue(preview.CanManageActiveCredentials);
        Assert.IsTrue(preview.UseTemplateEndpointCommand.CanExecute(null));
    }

    [TestMethod]
    public async Task AIProviderPreview_DisabledWhenAnotherSectionInSameDraftNeedsRecovery()
    {
        IConfigUiSection workbench = Substitute.For<IConfigUiSection, IConfigUiDraftDiscard>();
        IConfigUiSection other = Substitute.For<IConfigUiSection, IConfigUiDraftDiscard>();
        object identity = new();
        ((IConfigUiDraftDiscard)workbench).DraftIdentity.Returns(identity);
        ((IConfigUiDraftDiscard)other).DraftIdentity.Returns(identity);
        workbench.Section.Returns(new ConfigUiSection("Workbench", "Workbench"));
        other.Section.Returns(new ConfigUiSection("Other", "Other"));
        workbench.State.Returns(ConfigUiSectionState.Ready);
        other.State.Returns(ConfigUiSectionState.Ready);
        PropertyCategory category = new("Workbench", "Workbench");
        workbench.Properties.Returns(
        [
            new PropertyItem(category, nameof(CodeStudioSettings.AIProvider), "Provider", typeof(AIProviderPreset), AIProviderPreset.OpenAI),
            new PropertyItem(category, nameof(CodeStudioSettings.AIEndpoint), "Endpoint", typeof(string), "https://example.test"),
            new PropertyItem(category, nameof(CodeStudioSettings.AIModelId), "Model", typeof(string), "custom-model"),
        ]);
        other.Properties.Returns([]);
        IConfigUiSectionRegistry registry = Substitute.For<IConfigUiSectionRegistry>();
        registry.Sections.Returns([workbench, other]);
        ConfigUiViewModel config = new(registry, new PropertyEditorViewModel());
        using AIProviderDraftViewModel preview = new(config, new CodeStudioSettings());
        config.SelectedSection = other;
        config.SelectedSection = workbench;
        bool fail = true;
        other.LoadAsync().Returns(_ => fail
            ? Task.FromException(new IOException("Refresh failed"))
            : Task.CompletedTask);
        await config.LoadCommand.ExecuteAsync(null);

        Assert.IsTrue(config.NeedsReloadRecovery);
        Assert.IsFalse(config.HasUnsavedChanges);
        Assert.IsFalse(config.IsSelectedSectionReady);
        Assert.IsFalse(preview.IsVisible);
        Assert.IsFalse(preview.CanManageActiveCredentials);
        Assert.IsFalse(preview.UseTemplateModelCommand.CanExecute(null));

        fail = false;
        await config.RetryFailedReloadsCommand.ExecuteAsync(null);
        Assert.IsTrue(config.IsSelectedSectionReady);
        Assert.IsTrue(preview.IsVisible);
        Assert.IsTrue(preview.CanManageActiveCredentials);
    }

    private static IConfigSectionProvider CreateProvider(string name, int order)
    {
        IConfigSectionProvider provider = Substitute.For<IConfigSectionProvider>();
        provider.Name.Returns(name);
        provider.DisplayName.Returns(name);
        provider.Order.Returns(order);
        provider.ModelType.Returns(typeof(TestSettings));
        provider.CreateModel().Returns(_ => new TestSettings());
        return provider;
    }

    public sealed class TestSettings
    {
        public string Name { get; set; } = "Default";
    }
}
