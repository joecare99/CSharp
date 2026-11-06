using Config.UI.Avalonia.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Property.Editor;
using Property.Editor.Avalonia;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Config.UI.Avalonia.Tests;

/// <summary>
/// Verifies reusable Config.UI selection and lifecycle command behavior.
/// </summary>
[TestClass]
public sealed class ConfigUiViewModelTests
{
    [TestMethod]
    public void Constructor_CustomDialogTitle_ExposesProviderSpecificTitle()
    {
        ConfigUiViewModel viewModel = new(
            CreateRegistry(CreateSection("Project", 0)),
            new PropertyEditorViewModel(),
            "Project Settings - Sample");

        Assert.AreEqual("Project Settings - Sample", viewModel.DialogTitle);
    }

    [TestMethod]
    public async Task LoadCommand_FirstRegisteredSection_LoadsAndDisplaysProperties()
    {
        IConfigUiSection section = CreateSection("First", 0);
        IConfigUiSectionRegistry registry = CreateRegistry(section);
        ConfigUiViewModel viewModel = new(registry, new PropertyEditorViewModel());

        await viewModel.LoadCommand.ExecuteAsync(null);

        await section.Received().LoadAsync();
        Assert.AreSame(section, viewModel.SelectedSection);
        Assert.AreEqual(1, viewModel.PropertyEditor.Items.Count);
    }

    [TestMethod]
    public async Task SelectedSection_Changes_LoadsNewSection()
    {
        IConfigUiSection first = CreateSection("First", 0);
        IConfigUiSection second = CreateSection("Second", 1);
        IConfigUiSectionRegistry registry = CreateRegistry(first, second);
        ConfigUiViewModel viewModel = new(registry, new PropertyEditorViewModel());

        viewModel.SelectedSection = second;
        await viewModel.LoadCommand.ExecuteAsync(null);

        await second.Received().LoadAsync();
        Assert.AreSame(second, viewModel.SelectedSection);
    }

    [TestMethod]
    public async Task SaveCommand_ReadOnlySection_DoesNotSave()
    {
        IConfigUiSection section = CreateSection("ReadOnly", 0, isReadOnly: true);
        ConfigUiViewModel viewModel = new(CreateRegistry(section), new PropertyEditorViewModel());

        await viewModel.SaveCommand.ExecuteAsync(null);

        await section.DidNotReceive().SaveAsync();
        Assert.IsFalse(viewModel.CanSave);
    }

    [TestMethod]
    public async Task ResetCommand_ReadySection_ResetsAndRefreshesProperties()
    {
        IConfigUiSection section = CreateSection("General", 0);
        section.ResetDraftAsync().Returns(Task.CompletedTask);
        ConfigUiViewModel viewModel = new(CreateRegistry(section), new PropertyEditorViewModel());

        await viewModel.ResetCommand.ExecuteAsync(null);

        await section.Received().ResetDraftAsync();
        Assert.AreEqual(1, viewModel.PropertyEditor.Items.Count);
        Assert.IsTrue(viewModel.HasUnsavedChanges);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task ResetCommand_SharedDraftException_RequiresDiscardRecovery(bool cancelled)
    {
        IConfigUiSection section = Substitute.For<IConfigUiSection, IConfigUiDraftDiscard>();
        IConfigUiDraftDiscard draft = (IConfigUiDraftDiscard)section;
        draft.DraftIdentity.Returns(new object());
        section.Section.Returns(new ConfigUiSection("General", "General"));
        section.State.Returns(ConfigUiSectionState.Ready);
        section.Properties.Returns(
        [
            new PropertyItem(new PropertyCategory("General", "General"), "Name", "Name", typeof(string), "Original"),
        ]);
        section.ResetDraftAsync().Returns(_ => cancelled
            ? Task.FromException(new OperationCanceledException())
            : Task.FromException(new InvalidOperationException("Reset failed")));
        ConfigUiViewModel viewModel = new(CreateRegistry(section), new PropertyEditorViewModel());

        if (cancelled)
        {
            await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => viewModel.ResetCommand.ExecuteAsync(null));
        }
        else
        {
            await viewModel.ResetCommand.ExecuteAsync(null);
            StringAssert.Contains(viewModel.OperationMessage, "Reset failed");
        }

        Assert.IsFalse(viewModel.IsBusy);
        Assert.IsTrue(viewModel.HasUnsavedChanges);
        Assert.IsTrue(viewModel.NeedsReloadRecovery);
        Assert.IsFalse(viewModel.CanApplyChanges);
        Assert.AreEqual(0, viewModel.PropertyEditor.Items.Count);
        Assert.IsTrue(await viewModel.DiscardChangesAsync(), viewModel.OperationMessage);
        await draft.Received(1).DiscardDraftAsync();
        Assert.IsFalse(viewModel.NeedsReloadRecovery);
        Assert.IsFalse(viewModel.HasUnsavedChanges);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task ResetCommand_IndependentDraftFailure_RequiresRecoveryWithoutDiscardingOtherDraft(bool cancelled)
    {
        IConfigUiSection first = CreateSection("First", 0);
        IConfigUiSection second = CreateSection("Second", 1);
        first.ResetDraftAsync().Returns(_ => cancelled
            ? Task.FromException(new OperationCanceledException())
            : Task.FromException(new InvalidOperationException("Reset failed")));
        ConfigUiViewModel viewModel = new(CreateRegistry(first, second), new PropertyEditorViewModel());
        viewModel.SelectedSection = second;
        Assert.IsTrue(viewModel.PropertyEditor.Items[0].PropertyItem.TrySetValue("Other draft"));
        viewModel.SelectedSection = first;

        if (cancelled)
        {
            await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => viewModel.ResetCommand.ExecuteAsync(null));
        }
        else
        {
            await viewModel.ResetCommand.ExecuteAsync(null);
            StringAssert.Contains(viewModel.OperationMessage, "Reset failed");
        }

        Assert.IsFalse(viewModel.IsBusy);
        Assert.IsTrue(viewModel.NeedsReloadRecovery);
        Assert.IsFalse(viewModel.CanApplyChanges);
        Assert.AreEqual(0, viewModel.PropertyEditor.Items.Count);
        await viewModel.RetryFailedReloadsCommand.ExecuteAsync(null);
        Assert.IsFalse(viewModel.NeedsReloadRecovery);
        Assert.IsTrue(viewModel.HasUnsavedChanges);
        Assert.IsTrue(viewModel.CanApplyChanges);
        viewModel.SelectedSection = second;
        Assert.AreEqual("Other draft", viewModel.PropertyEditor.Items[0].PropertyItem.Value);
    }

    [TestMethod]
    public async Task ResetCommand_IndependentDraftReportsFailure_RequiresReload()
    {
        IConfigUiSection section = CreateSection("General", 0);
        section.ResetDraftAsync().Returns(_ =>
        {
            section.State.Returns(ConfigUiSectionState.Failed);
            section.ErrorMessage.Returns("Reset refresh failed");
            return Task.CompletedTask;
        });
        section.LoadAsync().Returns(_ =>
        {
            section.State.Returns(ConfigUiSectionState.Ready);
            return Task.CompletedTask;
        });
        ConfigUiViewModel viewModel = new(CreateRegistry(section), new PropertyEditorViewModel());

        await viewModel.ResetCommand.ExecuteAsync(null);

        Assert.IsTrue(viewModel.HasUnsavedChanges);
        Assert.IsTrue(viewModel.NeedsReloadRecovery);
        Assert.IsFalse(viewModel.CanApplyChanges);
        StringAssert.Contains(viewModel.OperationMessage, "Reset refresh failed");
        await viewModel.RetryFailedReloadsCommand.ExecuteAsync(null);
        Assert.IsFalse(viewModel.NeedsReloadRecovery);
        Assert.IsFalse(viewModel.HasUnsavedChanges);
        Assert.AreEqual(1, viewModel.PropertyEditor.Items.Count);
    }

    [TestMethod]
    public async Task LoadCommand_FailedSection_ExposesFailureMessage()
    {
        IConfigUiSection section = CreateSection("Failed", 0);
        section.State.Returns(ConfigUiSectionState.Failed);
        section.ErrorMessage.Returns("Unable to load configuration.");
        ConfigUiViewModel viewModel = new(CreateRegistry(section), new PropertyEditorViewModel());

        await viewModel.LoadCommand.ExecuteAsync(null);

        Assert.AreEqual("Unable to load configuration.", viewModel.ErrorMessage);
    }

    [TestMethod]
    public async Task SectionChange_PreservesLoadedSectionDraftUntilExplicitReload()
    {
        PropertyItem firstItem = new(new PropertyCategory("First", "First"), "Name", "Name", typeof(string), "Original");
        IConfigUiSection first = CreateSection("First", 0, properties: [firstItem]);
        IConfigUiSection second = CreateSection("Second", 1);
        ConfigUiViewModel viewModel = new(CreateRegistry(first, second), new PropertyEditorViewModel());

        await viewModel.LoadCommand.ExecuteAsync(null);
        Assert.IsTrue(firstItem.TrySetValue("Draft"));
        Assert.IsTrue(viewModel.HasUnsavedChanges);

        viewModel.SelectedSection = second;
        await viewModel.LoadCommand.ExecuteAsync(null);
        viewModel.SelectedSection = first;

        Assert.AreEqual("Draft", viewModel.PropertyEditor.Items[0].PropertyItem.Value);
        Assert.IsTrue(viewModel.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task ApplyChangesAsync_SavesChangedSectionsAndNotifiesHost()
    {
        PropertyItem property = new(new PropertyCategory("General", "General"), "Name", "Name", typeof(string), "Original");
        IConfigUiSection section = CreateSection("General", 0, properties: [property]);
        ConfigUiViewModel viewModel = new(CreateRegistry(section), new PropertyEditorViewModel());
        int appliedSections = 0;
        viewModel.SectionAppliedAsync = _ =>
        {
            appliedSections++;
            return Task.CompletedTask;
        };
        await viewModel.LoadCommand.ExecuteAsync(null);

        Assert.IsTrue(property.TrySetValue("Draft"));
        Assert.IsTrue(await viewModel.ApplyChangesAsync());

        await section.Received(1).SaveAsync();
        Assert.AreEqual(1, appliedSections);
        Assert.IsFalse(viewModel.HasUnsavedChanges);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task SavingSection_RebindsReplacementPropertiesForFurtherEdits(bool useApply)
    {
        PropertyCategory category = new("General", "General");
        PropertyItem original = new(category, "Name", "Name", typeof(string), "Original");
        PropertyItem replacement = new(category, "Name", "Name", typeof(string), "Saved");
        IConfigUiSection section = CreateSection("General", 0, properties: [original]);
        section.SaveAsync().Returns(_ =>
        {
            section.Properties.Returns([replacement]);
            return Task.CompletedTask;
        });
        ConfigUiViewModel viewModel = new(CreateRegistry(section), new PropertyEditorViewModel());
        await viewModel.LoadCommand.ExecuteAsync(null);
        Assert.IsTrue(original.TrySetValue("Saved"));

        if (useApply)
        {
            Assert.IsTrue(await viewModel.ApplyChangesAsync());
        }
        else
        {
            await viewModel.SaveCommand.ExecuteAsync(null);
        }

        Assert.AreSame(replacement, viewModel.PropertyEditor.Items[0].PropertyItem);
        Assert.IsTrue(original.TrySetValue("Obsolete"));
        Assert.IsFalse(viewModel.HasUnsavedChanges);
        Assert.IsTrue(replacement.TrySetValue("Next draft"));
        Assert.IsTrue(viewModel.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task ApplyChangesAsync_RebindsSubscriptionsForNonselectedSections()
    {
        PropertyCategory category = new("First", "First");
        PropertyItem old = new(category, "Name", "Name", typeof(string), "Original");
        PropertyItem replacement = new(category, "Name", "Name", typeof(string), "Saved");
        IConfigUiSection first = CreateSection("First", 0, properties: [old]);
        IConfigUiSection second = CreateSection("Second", 1);
        first.SaveAsync().Returns(_ =>
        {
            first.Properties.Returns([replacement]);
            return Task.CompletedTask;
        });
        ConfigUiViewModel viewModel = new(CreateRegistry(first, second), new PropertyEditorViewModel());
        Assert.IsTrue(old.TrySetValue("Draft"));
        viewModel.SelectedSection = second;
        Assert.IsTrue(await viewModel.ApplyChangesAsync());
        Assert.IsFalse(viewModel.HasUnsavedChanges);
        Assert.IsTrue(old.TrySetValue("Old object"));
        Assert.IsFalse(viewModel.HasUnsavedChanges);
        Assert.IsTrue(replacement.TrySetValue("New object"));
        Assert.IsTrue(viewModel.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task ApplyChangesAsync_HostValidationBlocksAllWritesAndPreservesDraft()
    {
        PropertyItem property = new(new PropertyCategory("General", "General"), "Name", "Name", typeof(string), "Original");
        IConfigUiSection section = CreateSection("General", 0, properties: [property]);
        ConfigUiViewModel viewModel = new(CreateRegistry(section), new PropertyEditorViewModel());
        viewModel.ValidateChangesAsync = _ => Task.FromResult<string?>("Invalid endpoint");
        await viewModel.LoadCommand.ExecuteAsync(null);
        Assert.IsTrue(property.TrySetValue("Draft"));

        Assert.IsFalse(await viewModel.ApplyChangesAsync());
        await section.DidNotReceive().SaveAsync();
        Assert.IsTrue(viewModel.HasUnsavedChanges);
        StringAssert.Contains(viewModel.OperationMessage, "Invalid endpoint");

        viewModel.ValidateChangesAsync = _ => Task.FromResult<string?>(null);
        Assert.IsTrue(await viewModel.ApplyChangesAsync());
        await section.Received(1).SaveAsync();
    }

    [TestMethod]
    public async Task ApplyChangesAsync_AsyncValidationBlocksEverySectionAndKeepsUiBusy()
    {
        PropertyItem firstProperty = new(new PropertyCategory("First", "First"), "Name", "Name", typeof(string), "Original");
        PropertyItem secondProperty = new(new PropertyCategory("Second", "Second"), "Name", "Name", typeof(string), "Original");
        IConfigUiSection first = CreateSection("First", 0, properties: [firstProperty]);
        IConfigUiSection second = CreateSection("Second", 1, properties: [secondProperty]);
        ConfigUiViewModel viewModel = new(CreateRegistry(first, second), new PropertyEditorViewModel());
        await viewModel.LoadCommand.ExecuteAsync(null);
        Assert.IsTrue(firstProperty.TrySetValue("Draft 1"));
        viewModel.SelectedSection = second;
        await viewModel.LoadCommand.ExecuteAsync(null);
        Assert.IsTrue(secondProperty.TrySetValue("Draft 2"));
        TaskCompletionSource<string?> validation = new();
        int validatedSections = 0;
        viewModel.ValidateChangesAsync = sections =>
        {
            validatedSections = sections.Count;
            return validation.Task;
        };

        Task<bool> applying = viewModel.ApplyChangesAsync();
        Assert.IsTrue(viewModel.IsBusy);
        Assert.AreEqual(2, validatedSections);
        Assert.IsFalse(await viewModel.DiscardChangesAsync());
        Assert.IsTrue(viewModel.HasUnsavedChanges);
        validation.SetResult("Invalid endpoint");
        Assert.IsFalse(await applying);
        Assert.IsFalse(viewModel.IsBusy);
        await first.DidNotReceive().SaveAsync();
        await second.DidNotReceive().SaveAsync();
        Assert.IsTrue(viewModel.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task ApplyChangesAsync_ValidationFailureIsVisibleAndDoesNotSave()
    {
        PropertyItem property = new(new PropertyCategory("General", "General"), "Name", "Name", typeof(string), "Original");
        IConfigUiSection section = CreateSection("General", 0, properties: [property]);
        ConfigUiViewModel viewModel = new(CreateRegistry(section), new PropertyEditorViewModel());
        viewModel.ValidateChangesAsync = _ => Task.FromException<string?>(new InvalidOperationException("Layer unavailable"));
        await viewModel.LoadCommand.ExecuteAsync(null);
        Assert.IsTrue(property.TrySetValue("Draft"));

        Assert.IsFalse(await viewModel.ApplyChangesAsync());
        await section.DidNotReceive().SaveAsync();
        StringAssert.Contains(viewModel.OperationMessage, "Layer unavailable");
        Assert.IsTrue(viewModel.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task DiscardChangesAsync_ReloadsChangedSections()
    {
        PropertyItem property = new(new PropertyCategory("General", "General"), "Name", "Name", typeof(string), "Original");
        IConfigUiSection section = CreateSection("General", 0, properties: [property]);
        ConfigUiViewModel viewModel = new(CreateRegistry(section), new PropertyEditorViewModel());
        await viewModel.LoadCommand.ExecuteAsync(null);
        Assert.IsTrue(property.TrySetValue("Draft"));

        section.ClearReceivedCalls();
        Assert.IsTrue(await viewModel.DiscardChangesAsync());

        await section.Received(1).LoadAsync();
        Assert.IsFalse(viewModel.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task DiscardChangesAsync_BlocksOverlappingApplyAndDiscardUntilReloadCompletes()
    {
        PropertyItem property = new(new PropertyCategory("General", "General"), "Name", "Name", typeof(string), "Original");
        IConfigUiSection section = CreateSection("General", 0, properties: [property]);
        ConfigUiViewModel viewModel = new(CreateRegistry(section), new PropertyEditorViewModel());
        await viewModel.LoadCommand.ExecuteAsync(null);
        Assert.IsTrue(property.TrySetValue("Draft"));
        TaskCompletionSource reload = new(TaskCreationOptions.RunContinuationsAsynchronously);
        section.LoadAsync().Returns(_ => reload.Task);
        section.ClearReceivedCalls();

        Task<bool> discard = viewModel.DiscardChangesAsync();
        Assert.IsTrue(viewModel.IsBusy);
        Assert.IsFalse(viewModel.CanApplyChanges);
        Assert.IsFalse(await viewModel.ApplyChangesAsync());
        Assert.IsFalse(await viewModel.DiscardChangesAsync());
        await viewModel.LoadCommand.ExecuteAsync(null);
        await section.Received(1).LoadAsync();
        await section.DidNotReceive().SaveAsync();
        Assert.IsTrue(viewModel.HasUnsavedChanges);
        reload.SetResult();
        Assert.IsTrue(await discard);
        Assert.IsFalse(viewModel.IsBusy);
        Assert.IsFalse(viewModel.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task DiscardChangesAsync_FailedReloadReleasesBusyAndRetainsDraft()
    {
        PropertyItem property = new(new PropertyCategory("General", "General"), "Name", "Name", typeof(string), "Original");
        IConfigUiSection section = CreateSection("General", 0, properties: [property]);
        ConfigUiViewModel viewModel = new(CreateRegistry(section), new PropertyEditorViewModel());
        await viewModel.LoadCommand.ExecuteAsync(null);
        Assert.IsTrue(property.TrySetValue("Draft"));
        section.LoadAsync().Returns(_ => Task.FromException(new InvalidOperationException("Reload failed")));

        Assert.IsFalse(await viewModel.DiscardChangesAsync());
        Assert.IsFalse(viewModel.IsBusy);
        Assert.IsTrue(viewModel.HasUnsavedChanges);
        StringAssert.Contains(viewModel.OperationMessage, "Reload failed");
    }

    [TestMethod]
    public async Task DiscardChangesAsync_CancelledReloadReleasesBusyAndRetainsDraft()
    {
        PropertyItem property = new(new PropertyCategory("General", "General"), "Name", "Name", typeof(string), "Original");
        IConfigUiSection section = CreateSection("General", 0, properties: [property]);
        ConfigUiViewModel viewModel = new(CreateRegistry(section), new PropertyEditorViewModel());
        await viewModel.LoadCommand.ExecuteAsync(null);
        Assert.IsTrue(property.TrySetValue("Draft"));
        section.LoadAsync().Returns(_ => Task.FromException(new OperationCanceledException()));

        await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => viewModel.DiscardChangesAsync());
        Assert.IsFalse(viewModel.IsBusy);
        Assert.IsTrue(viewModel.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task InitialLoad_BlocksApplyAndDiscardUntilLoadCompletes()
    {
        IConfigUiSection section = CreateSection("General", 0);
        TaskCompletionSource loading = new(TaskCreationOptions.RunContinuationsAsynchronously);
        section.LoadAsync().Returns(_ => loading.Task);
        ConfigUiViewModel viewModel = new(CreateRegistry(section), new PropertyEditorViewModel());

        Assert.IsTrue(viewModel.IsBusy);
        Assert.IsFalse(await viewModel.ApplyChangesAsync());
        Assert.IsFalse(await viewModel.DiscardChangesAsync());
        TaskCompletionSource ready = new(TaskCreationOptions.RunContinuationsAsynchronously);
        viewModel.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(ConfigUiViewModel.IsBusy) && !viewModel.IsBusy)
            {
                ready.TrySetResult();
            }
        };
        loading.SetResult();
        await ready.Task;
        Assert.IsFalse(viewModel.IsBusy);
    }

    [TestMethod]
    public async Task ResetDraft_BlocksConcurrentApplyAndDiscard()
    {
        IConfigUiSection section = CreateSection("General", 0);
        TaskCompletionSource resetting = new(TaskCreationOptions.RunContinuationsAsynchronously);
        section.ResetDraftAsync().Returns(_ => resetting.Task);
        ConfigUiViewModel viewModel = new(CreateRegistry(section), new PropertyEditorViewModel());
        Task reset = viewModel.ResetCommand.ExecuteAsync(null);

        Assert.IsTrue(viewModel.IsBusy);
        Assert.IsFalse(await viewModel.ApplyChangesAsync());
        Assert.IsFalse(await viewModel.DiscardChangesAsync());
        resetting.SetResult();
        await reset;
        Assert.IsFalse(viewModel.IsBusy);
        Assert.IsTrue(viewModel.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task LoadCommand_SharedLayerDraftReloadsAndRefreshesEverySection()
    {
        PropertyItem firstProperty = new(new PropertyCategory("First", "First"), "Name", "Name", typeof(string), "Original");
        IConfigUiSection first = CreateSharedSection("First", 0, [firstProperty]);
        IConfigUiSection second = CreateSharedSection("Second", 1);
        object draftIdentity = new();
        ((IConfigUiDraftDiscard)first).DraftIdentity.Returns(draftIdentity);
        ((IConfigUiDraftDiscard)second).DraftIdentity.Returns(draftIdentity);
        IConfigUiDraftDiscard secondDraft = (IConfigUiDraftDiscard)second;
        IConfigUiSectionRegistry registry = CreateRegistry(first, second);
        ConfigUiViewModel viewModel = new(registry, new PropertyEditorViewModel());

        await first.Received().LoadAsync();
        first.ClearReceivedCalls();
        viewModel.SelectedSection = second;
        await second.Received().LoadAsync();
        second.ClearReceivedCalls();
        Assert.IsTrue(firstProperty.TrySetValue("Draft"));
        Assert.IsTrue(viewModel.HasUnsavedChanges);

        await viewModel.LoadCommand.ExecuteAsync(null);

        await secondDraft.Received(1).DiscardDraftAsync();
        await first.Received(1).LoadAsync();
        Assert.IsFalse(viewModel.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task LoadCommand_DoesNotReloadOrClearUnrelatedSharedDraft()
    {
        PropertyItem firstProperty = new(new PropertyCategory("First", "First"), "Name", "Name", typeof(string), "Original");
        PropertyItem secondProperty = new(new PropertyCategory("Second", "Second"), "Name", "Name", typeof(string), "Original");
        IConfigUiSection first = CreateSharedSection("First", 0, [firstProperty]);
        IConfigUiSection second = CreateSharedSection("Second", 1, [secondProperty]);
        ((IConfigUiDraftDiscard)first).DraftIdentity.Returns(new string('x', 1));
        ((IConfigUiDraftDiscard)second).DraftIdentity.Returns(new string('x', 1));
        ConfigUiViewModel viewModel = new(CreateRegistry(first, second), new PropertyEditorViewModel());
        Assert.IsTrue(firstProperty.TrySetValue("Draft 1"));
        viewModel.SelectedSection = second;
        Assert.IsTrue(secondProperty.TrySetValue("Draft 2"));
        first.ClearReceivedCalls();

        await viewModel.LoadCommand.ExecuteAsync(null);

        await first.DidNotReceive().LoadAsync();
        Assert.AreEqual("Draft 1", firstProperty.Value);
        Assert.IsTrue(viewModel.HasUnsavedChanges);
        Assert.IsTrue(await viewModel.ApplyChangesAsync());
        await first.Received(1).SaveAsync();
        await second.DidNotReceive().SaveAsync();
    }

    [TestMethod]
    public async Task LoadCommand_FailedSharedReloadRetainsAllDirtySections()
    {
        PropertyItem firstProperty = new(new PropertyCategory("First", "First"), "Name", "Name", typeof(string), "Original");
        PropertyItem secondProperty = new(new PropertyCategory("Second", "Second"), "Name", "Name", typeof(string), "Original");
        IConfigUiSection first = CreateSharedSection("First", 0, [firstProperty]);
        IConfigUiSection second = CreateSharedSection("Second", 1, [secondProperty]);
        object identity = new();
        ((IConfigUiDraftDiscard)first).DraftIdentity.Returns(identity);
        ((IConfigUiDraftDiscard)second).DraftIdentity.Returns(identity);
        ConfigUiViewModel viewModel = new(CreateRegistry(first, second), new PropertyEditorViewModel());
        Assert.IsTrue(firstProperty.TrySetValue("Draft 1"));
        viewModel.SelectedSection = second;
        Assert.IsTrue(secondProperty.TrySetValue("Draft 2"));
        bool failed = true;
        second.State.Returns(_ => failed ? ConfigUiSectionState.Failed : ConfigUiSectionState.Ready);
        second.ErrorMessage.Returns("Storage unavailable");
        ((IConfigUiDraftDiscard)second).DiscardDraftAsync().Returns(Task.CompletedTask);
        first.ClearReceivedCalls();
        await viewModel.LoadCommand.ExecuteAsync(null);
        await first.DidNotReceive().LoadAsync();
        Assert.IsTrue(viewModel.HasUnsavedChanges);
        StringAssert.Contains(viewModel.OperationMessage, "Storage unavailable");
        Assert.IsFalse(await viewModel.ApplyChangesAsync());
        failed = false;
        await viewModel.LoadCommand.ExecuteAsync(null);
        Assert.IsFalse(viewModel.HasUnsavedChanges);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task DiscardChangesAsync_SharedFailureDoesNotClearSiblingAndCanRetry(bool reportFailureViaState)
    {
        PropertyItem firstProperty = new(new PropertyCategory("First", "First"), "Name", "Name", typeof(string), "Original");
        PropertyItem secondProperty = new(new PropertyCategory("Second", "Second"), "Name", "Name", typeof(string), "Original");
        IConfigUiSection first = CreateSharedSection("First", 0, [firstProperty]);
        IConfigUiSection second = CreateSharedSection("Second", 1, [secondProperty]);
        object identity = new();
        IConfigUiDraftDiscard firstDraft = (IConfigUiDraftDiscard)first;
        firstDraft.DraftIdentity.Returns(identity);
        ((IConfigUiDraftDiscard)second).DraftIdentity.Returns(identity);
        ConfigUiViewModel viewModel = new(CreateRegistry(first, second), new PropertyEditorViewModel());
        Assert.IsTrue(firstProperty.TrySetValue("Draft 1"));
        viewModel.SelectedSection = second;
        Assert.IsTrue(secondProperty.TrySetValue("Draft 2"));
        bool shouldFail = true;
        bool failedState = false;
        first.State.Returns(_ => failedState ? ConfigUiSectionState.Failed : ConfigUiSectionState.Ready);
        first.ErrorMessage.Returns("Storage unavailable");
        firstDraft.DiscardDraftAsync().Returns(_ =>
        {
            if (!shouldFail)
            {
                failedState = false;
                return Task.CompletedTask;
            }
            if (!reportFailureViaState)
            {
                return Task.FromException(new InvalidOperationException("Storage unavailable"));
            }
            failedState = true;
            return Task.CompletedTask;
        });
        second.ClearReceivedCalls();
        Assert.IsFalse(await viewModel.DiscardChangesAsync());
        await second.DidNotReceive().LoadAsync();
        Assert.IsTrue(viewModel.HasUnsavedChanges);
        StringAssert.Contains(viewModel.OperationMessage, "Storage unavailable");

        shouldFail = false;
        Assert.IsTrue(await viewModel.DiscardChangesAsync());
        await second.Received(1).LoadAsync();
        Assert.IsFalse(viewModel.HasUnsavedChanges);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task LoadCommand_FailedRelatedRefreshBlocksApplyUntilSectionReloads(bool reportFailureViaState)
    {
        PropertyItem firstProperty = new(new PropertyCategory("First", "First"), "Name", "Name", typeof(string), "Original");
        PropertyItem secondProperty = new(new PropertyCategory("Second", "Second"), "Name", "Name", typeof(string), "Original");
        IConfigUiSection first = CreateSharedSection("First", 0, [firstProperty]);
        IConfigUiSection second = CreateSharedSection("Second", 1, [secondProperty]);
        object identity = new();
        ((IConfigUiDraftDiscard)first).DraftIdentity.Returns(identity);
        ((IConfigUiDraftDiscard)second).DraftIdentity.Returns(identity);
        ConfigUiViewModel viewModel = new(CreateRegistry(first, second), new PropertyEditorViewModel());
        Assert.IsTrue(firstProperty.TrySetValue("Draft 1"));
        viewModel.SelectedSection = second;
        Assert.IsTrue(secondProperty.TrySetValue("Draft 2"));
        bool failed = true;
        first.State.Returns(_ => failed && reportFailureViaState ? ConfigUiSectionState.Failed : ConfigUiSectionState.Ready);
        first.ErrorMessage.Returns("Refresh failed");
        first.LoadAsync().Returns(_ => failed && !reportFailureViaState
            ? Task.FromException(new InvalidOperationException("Refresh failed"))
            : Task.CompletedTask);

        await viewModel.LoadCommand.ExecuteAsync(null);

        Assert.IsTrue(viewModel.HasUnsavedChanges);
        Assert.IsFalse(viewModel.CanApplyChanges);
        StringAssert.Contains(viewModel.OperationMessage, "Refresh failed");
        Assert.IsFalse(await viewModel.ApplyChangesAsync());
        await first.DidNotReceive().SaveAsync();
        failed = false;
        viewModel.SelectedSection = first;
        Assert.IsFalse(viewModel.HasUnsavedChanges);
        Assert.AreEqual(1, viewModel.PropertyEditor.Items.Count);
    }

    [TestMethod]
    public async Task DiscardChangesAsync_CancelledSharedDiscardBlocksEditingUntilRetry()
    {
        PropertyItem firstProperty = new(new PropertyCategory("First", "First"), "Name", "Name", typeof(string), "Original");
        PropertyItem secondProperty = new(new PropertyCategory("Second", "Second"), "Name", "Name", typeof(string), "Original");
        IConfigUiSection first = CreateSharedSection("First", 0, [firstProperty]);
        IConfigUiSection second = CreateSharedSection("Second", 1, [secondProperty]);
        object identity = new();
        IConfigUiDraftDiscard firstDraft = (IConfigUiDraftDiscard)first;
        firstDraft.DraftIdentity.Returns(identity);
        ((IConfigUiDraftDiscard)second).DraftIdentity.Returns(identity);
        ConfigUiViewModel viewModel = new(CreateRegistry(first, second), new PropertyEditorViewModel());
        Assert.IsTrue(firstProperty.TrySetValue("Draft 1"));
        viewModel.SelectedSection = second;
        Assert.IsTrue(secondProperty.TrySetValue("Draft 2"));
        bool cancel = true;
        firstDraft.DiscardDraftAsync().Returns(_ => cancel
            ? Task.FromException(new OperationCanceledException())
            : Task.CompletedTask);

        await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => viewModel.DiscardChangesAsync());
        Assert.IsTrue(viewModel.HasUnsavedChanges);
        Assert.IsFalse(viewModel.CanApplyChanges);
        viewModel.SelectedSection = first;
        Assert.AreEqual(0, viewModel.PropertyEditor.Items.Count);
        cancel = false;
        Assert.IsTrue(await viewModel.DiscardChangesAsync());
        Assert.IsFalse(viewModel.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task RetryFailedReloads_PreservesIndependentDirtyDraft()
    {
        PropertyItem firstProperty = new(new PropertyCategory("First", "First"), "Name", "Name", typeof(string), "Original");
        PropertyItem secondProperty = new(new PropertyCategory("Second", "Second"), "Name", "Name", typeof(string), "Original");
        IConfigUiSection first = CreateSharedSection("First", 0, [firstProperty]);
        IConfigUiSection second = CreateSharedSection("Second", 1, [secondProperty]);
        ((IConfigUiDraftDiscard)first).DraftIdentity.Returns(new object());
        ((IConfigUiDraftDiscard)second).DraftIdentity.Returns(new object());
        ConfigUiViewModel viewModel = new(CreateRegistry(first, second), new PropertyEditorViewModel());
        Assert.IsTrue(firstProperty.TrySetValue("Draft 1"));
        viewModel.SelectedSection = second;
        Assert.IsTrue(secondProperty.TrySetValue("Draft 2"));
        viewModel.SelectedSection = first;
        bool fail = true;
        ((IConfigUiDraftDiscard)first).DiscardDraftAsync().Returns(_ => fail
            ? Task.FromException(new InvalidOperationException("Reload failed"))
            : Task.CompletedTask);
        await viewModel.LoadCommand.ExecuteAsync(null);
        Assert.IsTrue(viewModel.NeedsReloadRecovery);

        fail = false;
        await viewModel.RetryFailedReloadsCommand.ExecuteAsync(null);

        Assert.IsFalse(viewModel.NeedsReloadRecovery);
        Assert.IsTrue(viewModel.HasUnsavedChanges);
        Assert.AreEqual("Draft 2", secondProperty.Value);
        Assert.IsTrue(await viewModel.ApplyChangesAsync());
        await first.DidNotReceive().SaveAsync();
        await second.Received(1).SaveAsync();
    }

    [TestMethod]
    public async Task FailedCleanSectionRefresh_BlocksSavingNewEditsInSharedDraft()
    {
        PropertyItem property = new(new PropertyCategory("First", "First"), "Name", "Name", typeof(string), "Original");
        IConfigUiSection first = CreateSharedSection("First", 0, [property]);
        IConfigUiSection second = CreateSharedSection("Second", 1);
        object identity = new();
        ((IConfigUiDraftDiscard)first).DraftIdentity.Returns(identity);
        ((IConfigUiDraftDiscard)second).DraftIdentity.Returns(identity);
        ConfigUiViewModel viewModel = new(CreateRegistry(first, second), new PropertyEditorViewModel());
        viewModel.SelectedSection = second;
        viewModel.SelectedSection = first;
        bool fail = true;
        second.LoadAsync().Returns(_ => fail
            ? Task.FromException(new InvalidOperationException("Refresh failed"))
            : Task.CompletedTask);
        await viewModel.LoadCommand.ExecuteAsync(null);

        Assert.IsTrue(viewModel.NeedsReloadRecovery);
        Assert.IsTrue(property.TrySetValue("New draft"));
        Assert.IsFalse(viewModel.CanSave);
        Assert.IsFalse(viewModel.CanApplyChanges);
        Assert.IsFalse(await viewModel.ApplyChangesAsync());

        fail = false;
        await viewModel.RetryFailedReloadsCommand.ExecuteAsync(null);
        Assert.IsFalse(viewModel.NeedsReloadRecovery);
        Assert.IsTrue(viewModel.CanSave);
        Assert.IsTrue(await viewModel.ApplyChangesAsync());
        await first.Received(1).SaveAsync();
    }

    [TestMethod]
    public async Task DiscardChangesAsync_FailedGroupDoesNotPreventIndependentDiscard()
    {
        PropertyItem firstProperty = new(new PropertyCategory("First", "First"), "Name", "Name", typeof(string), "Original");
        PropertyItem secondProperty = new(new PropertyCategory("Second", "Second"), "Name", "Name", typeof(string), "Original");
        IConfigUiSection first = CreateSharedSection("First", 0, [firstProperty]);
        IConfigUiSection second = CreateSharedSection("Second", 1, [secondProperty]);
        ((IConfigUiDraftDiscard)first).DraftIdentity.Returns(new object());
        ((IConfigUiDraftDiscard)second).DraftIdentity.Returns(new object());
        ConfigUiViewModel viewModel = new(CreateRegistry(first, second), new PropertyEditorViewModel());
        Assert.IsTrue(firstProperty.TrySetValue("Draft 1"));
        viewModel.SelectedSection = second;
        Assert.IsTrue(secondProperty.TrySetValue("Draft 2"));
        bool failed = true;
        ((IConfigUiDraftDiscard)first).DiscardDraftAsync().Returns(_ => failed
            ? Task.FromException(new InvalidOperationException("Storage unavailable"))
            : Task.CompletedTask);

        Assert.IsFalse(await viewModel.DiscardChangesAsync());

        await ((IConfigUiDraftDiscard)second).Received(1).DiscardDraftAsync();
        Assert.IsTrue(viewModel.HasUnsavedChanges);
        Assert.IsTrue(viewModel.IsSelectedSectionReady);
        Assert.IsFalse(await viewModel.ApplyChangesAsync());
        failed = false;
        Assert.IsTrue(await viewModel.DiscardChangesAsync());
        await second.DidNotReceive().SaveAsync();
    }

    [TestMethod]
    public async Task DiscardChangesAsync_RefreshesCleanLoadedSharedSection()
    {
        PropertyItem firstProperty = new(new PropertyCategory("First", "First"), "Name", "Name", typeof(string), "Original");
        IConfigUiSection first = CreateSharedSection("First", 0, [firstProperty]);
        IConfigUiSection second = CreateSharedSection("Second", 1);
        object identity = new();
        ((IConfigUiDraftDiscard)first).DraftIdentity.Returns(identity);
        ((IConfigUiDraftDiscard)second).DraftIdentity.Returns(identity);
        ConfigUiViewModel viewModel = new(CreateRegistry(first, second), new PropertyEditorViewModel());
        Assert.IsTrue(firstProperty.TrySetValue("Draft"));
        viewModel.SelectedSection = second;
        second.ClearReceivedCalls();
        Assert.IsTrue(await viewModel.DiscardChangesAsync());
        await second.Received(1).LoadAsync();
        Assert.IsFalse(viewModel.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task DiscardChangesAsync_ReloadsSharedLayerOnceAndRefreshesAllChangedSections()
    {
        PropertyItem firstProperty = new(new PropertyCategory("First", "First"), "Name", "Name", typeof(string), "Original");
        PropertyItem secondProperty = new(new PropertyCategory("Second", "Second"), "Name", "Name", typeof(string), "Original");
        IConfigUiSection first = CreateSharedSection("First", 0, [firstProperty]);
        IConfigUiSection second = CreateSharedSection("Second", 1, [secondProperty]);
        object draftIdentity = new();
        IConfigUiDraftDiscard firstDraft = (IConfigUiDraftDiscard)first;
        IConfigUiDraftDiscard secondDraft = (IConfigUiDraftDiscard)second;
        firstDraft.DraftIdentity.Returns(draftIdentity);
        secondDraft.DraftIdentity.Returns(draftIdentity);
        ConfigUiViewModel viewModel = new(CreateRegistry(first, second), new PropertyEditorViewModel());

        await first.Received().LoadAsync();
        viewModel.SelectedSection = second;
        await second.Received().LoadAsync();
        Assert.IsTrue(firstProperty.TrySetValue("Draft first"));
        Assert.IsTrue(secondProperty.TrySetValue("Draft second"));
        first.ClearReceivedCalls();
        second.ClearReceivedCalls();

        Assert.IsTrue(await viewModel.DiscardChangesAsync());

        await firstDraft.Received(1).DiscardDraftAsync();
        await secondDraft.DidNotReceive().DiscardDraftAsync();
        await second.Received(1).LoadAsync();
        Assert.IsFalse(viewModel.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task ApplyChangesAsync_InvalidDraft_DoesNotSaveChangedSection()
    {
        PropertyItem invalidProperty = new(
            new PropertyCategory("General", "General"),
            "InvalidName",
            "Invalid name",
            typeof(string),
            "Original",
            validator: value => Equals(value, "Draft")
                ? PropertyValidationResult.Invalid("Draft is not allowed.")
                : PropertyValidationResult.Valid);
        PropertyItem changedProperty = new(
            new PropertyCategory("General", "General"),
            "Name",
            "Name",
            typeof(string),
            "Original");
        IConfigUiSection section = CreateSection("General", 0, properties: [invalidProperty, changedProperty]);
        ConfigUiViewModel viewModel = new(CreateRegistry(section), new PropertyEditorViewModel());
        await viewModel.LoadCommand.ExecuteAsync(null);

        Assert.IsFalse(invalidProperty.TrySetValue("Draft"));
        Assert.IsTrue(changedProperty.TrySetValue("Changed"));
        Assert.IsFalse(await viewModel.ApplyChangesAsync());

        await section.DidNotReceive().SaveAsync();
        StringAssert.Contains(viewModel.OperationMessage, "Draft is not allowed.");
    }

    [TestMethod]
    public async Task ApplyChangesAsync_ActivationFailure_KeepsDraftForRetry()
    {
        PropertyItem property = new(new PropertyCategory("General", "General"), "Name", "Name", typeof(string), "Original");
        IConfigUiSection section = CreateSection("General", 0, properties: [property]);
        ConfigUiViewModel viewModel = new(CreateRegistry(section), new PropertyEditorViewModel());
        int activationCalls = 0;
        viewModel.SectionAppliedAsync = _ =>
        {
            activationCalls++;
            return activationCalls == 1
                ? Task.FromException(new InvalidOperationException("Runtime reload failed."))
                : Task.CompletedTask;
        };
        await viewModel.LoadCommand.ExecuteAsync(null);
        Assert.IsTrue(property.TrySetValue("Draft"));

        Assert.IsFalse(await viewModel.ApplyChangesAsync());
        Assert.IsTrue(viewModel.HasUnsavedChanges);
        StringAssert.Contains(viewModel.OperationMessage, "was saved, but its runtime changes could not be activated");

        Assert.IsTrue(await viewModel.ApplyChangesAsync());
        Assert.IsFalse(viewModel.HasUnsavedChanges);
        Assert.AreEqual(2, activationCalls);
    }

    private static IConfigUiSectionRegistry CreateRegistry(params IConfigUiSection[] sections)
    {
        IConfigUiSectionRegistry registry = Substitute.For<IConfigUiSectionRegistry>();
        registry.Sections.Returns(sections);
        return registry;
    }

    private static IConfigUiSection CreateSection(
        string name,
        int order,
        bool isReadOnly = false,
        IReadOnlyList<IPropertyItem>? properties = null)
    {
        IConfigUiSection section = Substitute.For<IConfigUiSection>();
        section.Section.Returns(new ConfigUiSection(name, name, sortOrder: order));
        section.Properties.Returns(properties ??
        [
            new PropertyItem(new PropertyCategory(name, name), "Name", "Name", typeof(string), "Value"),
        ]);
        section.IsReadOnly.Returns(isReadOnly);
        section.State.Returns(ConfigUiSectionState.Ready);
        return section;
    }

    private static IConfigUiSection CreateSharedSection(
        string name,
        int order,
        IReadOnlyList<IPropertyItem>? properties = null)
    {
        IConfigUiSection section = Substitute.For<IConfigUiSection, IConfigUiDraftDiscard>();
        section.Section.Returns(new ConfigUiSection(name, name, sortOrder: order));
        section.Properties.Returns(properties ?? []);
        section.IsReadOnly.Returns(false);
        section.State.Returns(ConfigUiSectionState.Ready);
        return section;
    }
}
