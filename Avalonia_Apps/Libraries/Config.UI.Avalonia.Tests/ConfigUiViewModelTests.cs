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
