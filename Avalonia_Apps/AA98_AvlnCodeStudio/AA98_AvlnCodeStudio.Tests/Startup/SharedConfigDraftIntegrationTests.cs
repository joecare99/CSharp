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
    public async Task LegacyReloadFailureAfterModelReplacement_BlocksStaleDraftUntilRetry()
    {
        IConfigStore store = Substitute.For<IConfigStore>();
        FragileSettings model = new("Original");
        store.LoadAsync("TestHost.Legacy", Arg.Any<FragileSettings>())
            .Returns(_ => Task.FromResult(model));
        IConfigSectionProvider provider = Substitute.For<IConfigSectionProvider>();
        provider.Name.Returns("Legacy");
        provider.DisplayName.Returns("Legacy");
        provider.ModelType.Returns(typeof(FragileSettings));
        provider.CreateModel().Returns(_ => new FragileSettings("Default"));
        ConfigServiceUiSection section = new(new global::Config.Service.ConfigService(store, "TestHost"), provider);
        IConfigUiSectionRegistry registry = Substitute.For<IConfigUiSectionRegistry>();
        registry.Sections.Returns([section]);
        ConfigUiViewModel config = new(registry, new PropertyEditorViewModel());
        Assert.IsTrue(config.PropertyEditor.Items.Single().PropertyItem.TrySetValue("Draft"));
        model = new FragileSettings("Stored", failGetter: true);

        await config.LoadCommand.ExecuteAsync(null);

        Assert.IsTrue(config.HasUnsavedChanges);
        Assert.IsTrue(config.NeedsReloadRecovery);
        Assert.AreEqual(0, config.PropertyEditor.Items.Count);
        Assert.IsFalse(await config.ApplyChangesAsync());
        await store.DidNotReceive().SaveAsync(Arg.Any<string>(), Arg.Any<FragileSettings>());
        model = new FragileSettings("Stored");
        await config.RetryFailedReloadsCommand.ExecuteAsync(null);
        Assert.IsFalse(config.NeedsReloadRecovery);
        Assert.IsFalse(config.HasUnsavedChanges);
        Assert.AreEqual("Stored", config.PropertyEditor.Items.Single().PropertyItem.Value);
    }

    [TestMethod]
    public async Task LayeredLoad_CancellationReleasesSectionAndRetainsOtherDrafts()
    {
        ConfigSectionRegistry registry = new();
        registry.AddSection(CreateProvider("First", 0));
        registry.AddSection(CreateProvider("Sibling", 1));
        IConfigLayerStore user = Substitute.For<IConfigLayerStore>();
        IConfigLayerStore system = Substitute.For<IConfigLayerStore>();
        user.Layer.Returns(ConfigLayer.User);
        system.Layer.Returns(ConfigLayer.System);
        user.LoadAsync(Arg.Any<CancellationToken>()).Returns(new ConfigLayerSnapshot(ConfigLayer.User, []));
        bool cancel = false;
        system.LoadAsync(Arg.Any<CancellationToken>()).Returns(_ => cancel
            ? Task.FromException<ConfigLayerSnapshot>(new OperationCanceledException())
            : Task.FromResult(new ConfigLayerSnapshot(ConfigLayer.System, [])));
        ConfigLayeredSettingsResolver resolver = new(registry, [system, user]);
        var shared = new ConfigLayeredUiSectionRegistry(registry, resolver, user, ConfigLayer.User).Sections;
        IConfigUiSection independent = Substitute.For<IConfigUiSection>();
        independent.Section.Returns(new ConfigUiSection("Independent", "Independent"));
        independent.State.Returns(ConfigUiSectionState.Ready);
        PropertyItem independentProperty = new(new PropertyCategory("Independent", "Independent"), "Name", "Name", typeof(string), "Original");
        independent.Properties.Returns([independentProperty]);
        IConfigUiSectionRegistry combined = Substitute.For<IConfigUiSectionRegistry>();
        combined.Sections.Returns([shared[0], shared[1], independent]);
        ConfigUiViewModel config = new(combined, new PropertyEditorViewModel());
        Assert.IsTrue(config.PropertyEditor.Items.Single().PropertyItem.TrySetValue("First draft"));
        config.SelectedSection = shared[1];
        Assert.IsTrue(config.PropertyEditor.Items.Single().PropertyItem.TrySetValue("Sibling draft"));
        config.SelectedSection = independent;
        Assert.IsTrue(independentProperty.TrySetValue("Independent draft"));
        config.SelectedSection = shared[0];
        cancel = true;

        await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => shared[0].LoadAsync());

        Assert.AreEqual(ConfigUiSectionState.Failed, shared[0].State);
        Assert.IsFalse(config.IsBusy);
        Assert.IsTrue(config.HasUnsavedChanges);
        Assert.AreEqual("Independent draft", independentProperty.Value);
        cancel = false;
        await shared[0].LoadAsync();
        await shared[1].LoadAsync();
        Assert.AreEqual("First draft", shared[0].Properties.Single().Value);
        Assert.AreEqual("Sibling draft", shared[1].Properties.Single().Value);
        Assert.AreEqual("Independent draft", independentProperty.Value);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task CancelledSharedReload_ReleasesDialogAndPreservesIndependentDraft(bool afterReplacement)
    {
        ConfigSectionRegistry registry = new();
        registry.AddSection(CreateProvider("First", 0));
        registry.AddSection(CreateProvider("Sibling", 1));
        IConfigLayerStore user = Substitute.For<IConfigLayerStore>();
        IConfigLayerStore system = Substitute.For<IConfigLayerStore>();
        user.Layer.Returns(ConfigLayer.User);
        system.Layer.Returns(ConfigLayer.System);
        ConfigLayerSnapshot persisted = new(ConfigLayer.User, []);
        bool cancel = false;
        user.LoadAsync(Arg.Any<CancellationToken>()).Returns(_ => cancel && !afterReplacement
            ? Task.FromException<ConfigLayerSnapshot>(new OperationCanceledException())
            : Task.FromResult(persisted));
        system.LoadAsync(Arg.Any<CancellationToken>()).Returns(_ => cancel && afterReplacement
            ? Task.FromException<ConfigLayerSnapshot>(new OperationCanceledException())
            : Task.FromResult(new ConfigLayerSnapshot(ConfigLayer.System, [])));
        ConfigLayeredSettingsResolver resolver = new(registry, [system, user]);
        var shared = new ConfigLayeredUiSectionRegistry(registry, resolver, user, ConfigLayer.User).Sections;
        IConfigUiSection independent = Substitute.For<IConfigUiSection>();
        independent.Section.Returns(new ConfigUiSection("Independent", "Independent"));
        independent.State.Returns(ConfigUiSectionState.Ready);
        PropertyItem independentProperty = new(new PropertyCategory("Independent", "Independent"), "Name", "Name", typeof(string), "Original");
        independent.Properties.Returns([independentProperty]);
        IConfigUiSectionRegistry combined = Substitute.For<IConfigUiSectionRegistry>();
        combined.Sections.Returns([shared[0], shared[1], independent]);
        ConfigUiViewModel config = new(combined, new PropertyEditorViewModel());
        IPropertyItem first = config.PropertyEditor.Items.Single().PropertyItem;
        Assert.IsTrue(first.TrySetValue("First draft"));
        config.SelectedSection = shared[1];
        IPropertyItem sibling = config.PropertyEditor.Items.Single().PropertyItem;
        Assert.IsTrue(sibling.TrySetValue("Sibling draft"));
        config.SelectedSection = independent;
        Assert.IsTrue(independentProperty.TrySetValue("Independent draft"));
        config.SelectedSection = shared[0];
        cancel = true;

        await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => config.LoadCommand.ExecuteAsync(null));

        Assert.AreEqual(ConfigUiSectionState.Failed, shared[0].State);
        Assert.IsFalse(config.IsBusy);
        Assert.IsTrue(config.NeedsReloadRecovery);
        Assert.IsFalse(config.CanApplyChanges);
        Assert.AreEqual(0, config.PropertyEditor.Items.Count);
        Assert.AreEqual("Independent draft", independentProperty.Value);
        if (afterReplacement)
        {
            Assert.IsFalse(first.TrySetValue("Stale first"));
            Assert.IsFalse(sibling.TrySetValue("Stale sibling"));
        }
        cancel = false;
        await config.RetryFailedReloadsCommand.ExecuteAsync(null);
        Assert.IsFalse(config.NeedsReloadRecovery);
        Assert.AreEqual("Default", config.PropertyEditor.Items.Single().PropertyItem.Value);
        Assert.IsTrue(config.HasUnsavedChanges);
        Assert.AreEqual("Independent draft", independentProperty.Value);
        Assert.IsTrue(await config.ApplyChangesAsync());
        await independent.Received(1).SaveAsync();
    }

    [TestMethod]
    public async Task CancelledSiblingLoad_AfterSharedReloadRemainsRecoverable()
    {
        ConfigSectionRegistry registry = new();
        registry.AddSection(CreateProvider("First", 0));
        registry.AddSection(CreateProvider("Sibling", 1));
        IConfigLayerStore user = Substitute.For<IConfigLayerStore>();
        IConfigLayerStore system = Substitute.For<IConfigLayerStore>();
        user.Layer.Returns(ConfigLayer.User);
        system.Layer.Returns(ConfigLayer.System);
        user.LoadAsync(Arg.Any<CancellationToken>()).Returns(new ConfigLayerSnapshot(ConfigLayer.User, []));
        bool cancel = false;
        int loads = 0;
        system.LoadAsync(Arg.Any<CancellationToken>()).Returns(_ => cancel && ++loads == 2
            ? Task.FromException<ConfigLayerSnapshot>(new OperationCanceledException())
            : Task.FromResult(new ConfigLayerSnapshot(ConfigLayer.System, [])));
        ConfigLayeredSettingsResolver resolver = new(registry, [system, user]);
        var shared = new ConfigLayeredUiSectionRegistry(registry, resolver, user, ConfigLayer.User).Sections;
        IConfigUiSection independent = Substitute.For<IConfigUiSection>();
        independent.Section.Returns(new ConfigUiSection("Independent", "Independent"));
        independent.State.Returns(ConfigUiSectionState.Ready);
        PropertyItem independentProperty = new(new PropertyCategory("Independent", "Independent"), "Name", "Name", typeof(string), "Original");
        independent.Properties.Returns([independentProperty]);
        IConfigUiSectionRegistry combined = Substitute.For<IConfigUiSectionRegistry>();
        combined.Sections.Returns([shared[0], shared[1], independent]);
        ConfigUiViewModel config = new(combined, new PropertyEditorViewModel());
        Assert.IsTrue(config.PropertyEditor.Items.Single().PropertyItem.TrySetValue("First draft"));
        config.SelectedSection = shared[1];
        IPropertyItem sibling = config.PropertyEditor.Items.Single().PropertyItem;
        Assert.IsTrue(sibling.TrySetValue("Sibling draft"));
        config.SelectedSection = independent;
        Assert.IsTrue(independentProperty.TrySetValue("Independent draft"));
        config.SelectedSection = shared[0];
        cancel = true;

        await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => config.LoadCommand.ExecuteAsync(null));

        Assert.AreEqual(ConfigUiSectionState.Failed, shared[1].State);
        Assert.IsFalse(config.IsBusy);
        Assert.IsTrue(config.NeedsReloadRecovery);
        Assert.IsFalse(config.IsSelectedSectionReady);
        Assert.IsFalse(config.CanApplyChanges);
        Assert.IsFalse(sibling.TrySetValue("Stale sibling"));
        cancel = false;
        await config.RetryFailedReloadsCommand.ExecuteAsync(null);
        Assert.IsFalse(config.NeedsReloadRecovery);
        Assert.IsTrue(config.IsSelectedSectionReady);
        Assert.AreEqual("Default", config.PropertyEditor.Items.Single().PropertyItem.Value);
        Assert.AreEqual("Independent draft", independentProperty.Value);
        Assert.IsTrue(config.HasUnsavedChanges);
        Assert.IsTrue(await config.ApplyChangesAsync());
        await independent.Received(1).SaveAsync();
    }

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
    [DataRow(false)]
    [DataRow(true)]
    public async Task ApplyRefreshFailureAfterWrite_RetainsIndependentDraftAndAllowsRetry(bool cancelled)
    {
        ConfigSectionRegistry registry = new();
        registry.AddSection(CreateProvider("First", 0));
        registry.AddSection(CreateProvider("Sibling", 1));
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
            ? Task.FromException<ConfigLayerSnapshot>(cancelled
                ? new OperationCanceledException()
                : new IOException("System refresh failed"))
            : Task.FromResult(new ConfigLayerSnapshot(ConfigLayer.System, [])));
        ConfigLayeredSettingsResolver resolver = new(registry, [system, user]);
        var sharedSections = new ConfigLayeredUiSectionRegistry(registry, resolver, user, ConfigLayer.User).Sections;
        IConfigUiSection shared = sharedSections[0];
        IConfigUiSection sibling = sharedSections[1];
        IConfigUiSection independent = Substitute.For<IConfigUiSection>();
        independent.Section.Returns(new ConfigUiSection("Independent", "Independent"));
        independent.State.Returns(ConfigUiSectionState.Ready);
        PropertyItem independentProperty = new(new PropertyCategory("Independent", "Independent"), "Name", "Name", typeof(string), "Original");
        independent.Properties.Returns([independentProperty]);
        IConfigUiSectionRegistry combined = Substitute.For<IConfigUiSectionRegistry>();
        combined.Sections.Returns([shared, sibling, independent]);
        ConfigUiViewModel config = new(combined, new PropertyEditorViewModel());
        int activated = 0;
        config.SectionAppliedAsync = _ => { activated++; return Task.CompletedTask; };
        Assert.IsTrue(config.PropertyEditor.Items.Single().PropertyItem.TrySetValue("Saved"));
        config.SelectedSection = sibling;
        IPropertyItem siblingProperty = config.PropertyEditor.Items.Single().PropertyItem;
        Assert.IsTrue(siblingProperty.TrySetValue("Sibling draft"));
        config.SelectedSection = independent;
        Assert.IsTrue(independentProperty.TrySetValue("Independent draft"));
        config.SelectedSection = shared;

        if (cancelled)
        {
            await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => config.ApplyChangesAsync());
        }
        else
        {
            Assert.IsFalse(await config.ApplyChangesAsync());
        }

        Assert.IsTrue(persisted.TryGetValue(new ConfigPropertyKey("First", nameof(TestSettings.Name)), out ConfigLayerValue? saved));
        Assert.AreEqual("Saved", saved.Value);
        Assert.IsFalse(config.IsBusy);
        Assert.IsTrue(config.HasUnsavedChanges);
        Assert.IsTrue(config.CanApplyChanges);
        StringAssert.Contains(config.OperationMessage, "saved");
        Assert.AreEqual("Independent draft", independentProperty.Value);
        Assert.AreEqual("Sibling draft", siblingProperty.Value);
        Assert.IsTrue(persisted.TryGetValue(new ConfigPropertyKey("Sibling", nameof(TestSettings.Name)), out ConfigLayerValue? savedSibling));
        Assert.AreEqual("Sibling draft", savedSibling.Value);
        if (cancelled)
        {
            Assert.AreEqual(0, activated);
            await independent.DidNotReceive().SaveAsync();
        }

        user.SaveAsync(Arg.Any<ConfigLayerSnapshot>(), Arg.Any<CancellationToken>())
            .Returns(call => Task.FromResult(call.Arg<ConfigLayerSnapshot>()));
        failRefresh = false;
        Assert.IsTrue(await config.ApplyChangesAsync(), config.OperationMessage);
        Assert.IsFalse(config.HasUnsavedChanges);
        await independent.Received(1).SaveAsync();
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task SaveCommand_CancellationReleasesDialogAndRetainsDraft(bool afterWrite)
    {
        ConfigSectionRegistry registry = new();
        registry.AddSection(CreateProvider("First", 0));
        IConfigLayerStore user = Substitute.For<IConfigLayerStore>();
        IConfigLayerStore system = Substitute.For<IConfigLayerStore>();
        user.Layer.Returns(ConfigLayer.User);
        system.Layer.Returns(ConfigLayer.System);
        ConfigLayerSnapshot persisted = new(ConfigLayer.User, []);
        bool cancel = true;
        user.LoadAsync(Arg.Any<CancellationToken>()).Returns(_ => Task.FromResult(persisted));
        user.SaveAsync(Arg.Any<ConfigLayerSnapshot>(), Arg.Any<CancellationToken>()).Returns(call =>
        {
            if (cancel && !afterWrite)
            {
                return Task.FromException<ConfigLayerSnapshot>(new OperationCanceledException());
            }
            persisted = call.Arg<ConfigLayerSnapshot>();
            return Task.FromResult(persisted);
        });
        system.LoadAsync(Arg.Any<CancellationToken>()).Returns(_ => cancel && afterWrite && persisted.Values.Count > 0
            ? Task.FromException<ConfigLayerSnapshot>(new OperationCanceledException())
            : Task.FromResult(new ConfigLayerSnapshot(ConfigLayer.System, [])));
        ConfigLayeredSettingsResolver resolver = new(registry, [system, user]);
        ConfigUiViewModel config = new(
            new ConfigLayeredUiSectionRegistry(registry, resolver, user, ConfigLayer.User),
            new PropertyEditorViewModel());
        Assert.IsTrue(config.PropertyEditor.Items.Single().PropertyItem.TrySetValue("Draft"));

        await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => config.SaveCommand.ExecuteAsync(null));

        Assert.IsFalse(config.IsBusy);
        Assert.IsTrue(config.HasUnsavedChanges);
        Assert.IsTrue(config.CanApplyChanges);
        StringAssert.Contains(config.OperationMessage, afterWrite ? "Settings were saved" : "may already have been saved");
        Assert.AreEqual(afterWrite, persisted.TryGetValue(new ConfigPropertyKey("First", nameof(TestSettings.Name)), out _));
        cancel = false;
        Assert.IsTrue(await config.ApplyChangesAsync(), config.OperationMessage);
        Assert.IsFalse(config.HasUnsavedChanges);
    }

    [TestMethod]
    [DataRow(false, false, true)]
    [DataRow(false, false, false)]
    [DataRow(false, true, false)]
    [DataRow(true, true, false)]
    public async Task PersistentReset_CancellationReleasesSectionAndPreservesOtherDrafts(bool afterWrite, bool writeOccurred, bool beforeSave)
    {
        ConfigSectionRegistry registry = new();
        registry.AddSection(CreateProvider("First", 0));
        registry.AddSection(CreateProvider("Sibling", 1));
        IConfigLayerStore user = Substitute.For<IConfigLayerStore>();
        IConfigLayerStore system = Substitute.For<IConfigLayerStore>();
        user.Layer.Returns(ConfigLayer.User);
        system.Layer.Returns(ConfigLayer.System);
        ConfigPropertyKey firstKey = new("First", nameof(TestSettings.Name));
        ConfigPropertyKey siblingKey = new("Sibling", nameof(TestSettings.Name));
        ConfigLayerSnapshot persisted = new ConfigLayerSnapshot(ConfigLayer.User, [])
            .WithValue(firstKey, new ConfigLayerValue("Stored"));
        bool cancel = false;
        bool saved = false;
        user.LoadAsync(Arg.Any<CancellationToken>()).Returns(_ => Task.FromResult(persisted));
        user.SaveAsync(Arg.Any<ConfigLayerSnapshot>(), Arg.Any<CancellationToken>()).Returns(call =>
        {
            if (writeOccurred)
            {
                persisted = call.Arg<ConfigLayerSnapshot>();
            }
            if (cancel && !afterWrite)
            {
                return Task.FromException<ConfigLayerSnapshot>(new OperationCanceledException());
            }
            saved = true;
            return Task.FromResult(persisted);
        });
        system.LoadAsync(Arg.Any<CancellationToken>()).Returns(_ => cancel && (saved || beforeSave)
            ? Task.FromException<ConfigLayerSnapshot>(new OperationCanceledException())
            : Task.FromResult(new ConfigLayerSnapshot(ConfigLayer.System, [])));
        ConfigLayeredSettingsResolver resolver = new(registry, [system, user]);
        var shared = new ConfigLayeredUiSectionRegistry(registry, resolver, user, ConfigLayer.User).Sections;
        IConfigUiSection independent = Substitute.For<IConfigUiSection>();
        independent.Section.Returns(new ConfigUiSection("Independent", "Independent"));
        independent.State.Returns(ConfigUiSectionState.Ready);
        PropertyItem independentProperty = new(new PropertyCategory("Independent", "Independent"), "Name", "Name", typeof(string), "Original");
        independent.Properties.Returns([independentProperty]);
        IConfigUiSectionRegistry combined = Substitute.For<IConfigUiSectionRegistry>();
        combined.Sections.Returns([shared[0], shared[1], independent]);
        ConfigUiViewModel config = new(combined, new PropertyEditorViewModel());
        Assert.IsTrue(config.PropertyEditor.Items.Single().PropertyItem.TrySetValue("First draft"));
        config.SelectedSection = shared[1];
        Assert.IsTrue(config.PropertyEditor.Items.Single().PropertyItem.TrySetValue("Sibling draft"));
        config.SelectedSection = independent;
        Assert.IsTrue(independentProperty.TrySetValue("Independent draft"));
        config.SelectedSection = shared[0];
        cancel = true;

        await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => shared[0].ResetAsync());

        Assert.AreEqual(ConfigUiSectionState.Failed, shared[0].State);
        Assert.IsFalse(config.IsBusy);
        Assert.IsTrue(config.HasUnsavedChanges);
        StringAssert.Contains(shared[0].ErrorMessage, afterWrite
            ? "reset was saved"
            : beforeSave ? "before saving" : "may already have been saved");
        Assert.AreEqual(!writeOccurred, persisted.TryGetValue(firstKey, out _));
        Assert.AreEqual(writeOccurred, persisted.TryGetValue(siblingKey, out _));
        Assert.AreEqual("Independent draft", independentProperty.Value);
        await independent.DidNotReceive().SaveAsync();
        if (beforeSave)
        {
            await user.DidNotReceive().SaveAsync(Arg.Any<ConfigLayerSnapshot>(), Arg.Any<CancellationToken>());
        }
        cancel = false;
        await shared[0].LoadAsync();
        await shared[1].LoadAsync();
        Assert.AreEqual(beforeSave ? "First draft" : "Default", shared[0].Properties.Single().Value);
        Assert.AreEqual("Sibling draft", shared[1].Properties.Single().Value);
        Assert.AreEqual("Independent draft", independentProperty.Value);
    }

    [TestMethod]
    public async Task CancelledDraftReset_ReleasesDialogAndRecoveryPreservesIndependentDraft()
    {
        ConfigSectionRegistry registry = new();
        registry.AddSection(CreateProvider("First", 0));
        registry.AddSection(CreateProvider("Sibling", 1));
        IConfigLayerStore user = Substitute.For<IConfigLayerStore>();
        IConfigLayerStore system = Substitute.For<IConfigLayerStore>();
        user.Layer.Returns(ConfigLayer.User);
        system.Layer.Returns(ConfigLayer.System);
        ConfigPropertyKey key = new("First", nameof(TestSettings.Name));
        ConfigLayerSnapshot persisted = new ConfigLayerSnapshot(ConfigLayer.User, [])
            .WithValue(key, new ConfigLayerValue("Stored"));
        user.LoadAsync(Arg.Any<CancellationToken>()).Returns(_ => Task.FromResult(persisted));
        int loads = 0;
        bool cancel = false;
        system.LoadAsync(Arg.Any<CancellationToken>()).Returns(_ => cancel && ++loads == 2
            ? Task.FromException<ConfigLayerSnapshot>(new OperationCanceledException())
            : Task.FromResult(new ConfigLayerSnapshot(ConfigLayer.System, [])));
        ConfigLayeredSettingsResolver resolver = new(registry, [system, user]);
        var shared = new ConfigLayeredUiSectionRegistry(registry, resolver, user, ConfigLayer.User).Sections;
        IConfigUiSection independent = Substitute.For<IConfigUiSection>();
        independent.Section.Returns(new ConfigUiSection("Independent", "Independent"));
        independent.State.Returns(ConfigUiSectionState.Ready);
        PropertyItem independentProperty = new(new PropertyCategory("Independent", "Independent"), "Name", "Name", typeof(string), "Original");
        independent.Properties.Returns([independentProperty]);
        IConfigUiSectionRegistry combined = Substitute.For<IConfigUiSectionRegistry>();
        combined.Sections.Returns([shared[0], shared[1], independent]);
        ConfigUiViewModel config = new(combined, new PropertyEditorViewModel());
        config.SelectedSection = shared[1];
        Assert.IsTrue(config.PropertyEditor.Items.Single().PropertyItem.TrySetValue("Sibling draft"));
        config.SelectedSection = independent;
        Assert.IsTrue(independentProperty.TrySetValue("Independent draft"));
        config.SelectedSection = shared[0];
        cancel = true;

        await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => config.ResetCommand.ExecuteAsync(null));

        Assert.IsFalse(config.IsBusy);
        Assert.IsTrue(config.NeedsReloadRecovery);
        Assert.IsFalse(config.CanApplyChanges);
        Assert.AreEqual(0, config.PropertyEditor.Items.Count);
        Assert.IsTrue(persisted.TryGetValue(key, out _));
        await user.DidNotReceive().SaveAsync(Arg.Any<ConfigLayerSnapshot>(), Arg.Any<CancellationToken>());
        cancel = false;
        await config.RetryFailedReloadsCommand.ExecuteAsync(null);
        Assert.IsFalse(config.NeedsReloadRecovery);
        Assert.AreEqual("Stored", config.PropertyEditor.Items.Single().PropertyItem.Value);
        Assert.IsTrue(config.HasUnsavedChanges);
        Assert.AreEqual("Independent draft", independentProperty.Value);
        Assert.IsTrue(await config.ApplyChangesAsync());
        await independent.Received(1).SaveAsync();
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task ResetFailureAfterDraftMutation_BlocksSharedDraftUntilDiscard(bool hasOtherDraft)
    {
        ConfigSectionRegistry registry = new();
        registry.AddSection(CreateProvider("First", 0));
        registry.AddSection(CreateProvider("Second", 1));
        IConfigLayerStore user = Substitute.For<IConfigLayerStore>();
        IConfigLayerStore system = Substitute.For<IConfigLayerStore>();
        user.Layer.Returns(ConfigLayer.User);
        system.Layer.Returns(ConfigLayer.System);
        ConfigPropertyKey firstKey = new("First", nameof(TestSettings.Name));
        ConfigLayerSnapshot persisted = new ConfigLayerSnapshot(ConfigLayer.User, [])
            .WithValue(firstKey, new ConfigLayerValue("Stored"));
        user.LoadAsync(Arg.Any<CancellationToken>()).Returns(_ => Task.FromResult(persisted));
        int systemLoads = 0;
        bool failRefresh = false;
        system.LoadAsync(Arg.Any<CancellationToken>()).Returns(_ =>
        {
            systemLoads++;
            return failRefresh && systemLoads == 2
                ? Task.FromException<ConfigLayerSnapshot>(new IOException("System refresh failed"))
                : Task.FromResult(new ConfigLayerSnapshot(ConfigLayer.System, []));
        });
        ConfigLayeredSettingsResolver resolver = new(registry, [system, user]);
        ConfigUiViewModel config = new(
            new ConfigLayeredUiSectionRegistry(registry, resolver, user, ConfigLayer.User),
            new PropertyEditorViewModel());
        config.SelectedSection = config.Sections[1];
        if (hasOtherDraft)
        {
            Assert.IsTrue(config.PropertyEditor.Items.Single().PropertyItem.TrySetValue("Other draft"));
        }
        config.SelectedSection = config.Sections[0];
        systemLoads = 0;
        failRefresh = true;

        await config.ResetCommand.ExecuteAsync(null);

        Assert.IsTrue(config.HasUnsavedChanges);
        Assert.IsTrue(config.NeedsReloadRecovery);
        Assert.IsFalse(config.CanApplyChanges);
        Assert.AreEqual(0, config.PropertyEditor.Items.Count);
        StringAssert.Contains(config.OperationMessage, "may have changed the draft");
        Assert.IsFalse(await config.ApplyChangesAsync());
        await user.DidNotReceive().SaveAsync(Arg.Any<ConfigLayerSnapshot>(), Arg.Any<CancellationToken>());
        failRefresh = false;
        Assert.IsTrue(await config.DiscardChangesAsync(), config.OperationMessage);
        Assert.IsFalse(config.NeedsReloadRecovery);
        Assert.IsFalse(config.HasUnsavedChanges);
        Assert.AreEqual("Stored", config.PropertyEditor.Items.Single().PropertyItem.Value);
        config.SelectedSection = config.Sections[1];
        Assert.AreEqual("Default", config.PropertyEditor.Items.Single().PropertyItem.Value);
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

    public sealed class FragileSettings(string name, bool failGetter = false)
    {
        private string _name = name;

        public string Name
        {
            get => failGetter ? throw new InvalidOperationException("Property unavailable") : _name;
            set => _name = value;
        }
    }
}
