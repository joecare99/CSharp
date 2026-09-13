using Config.UI.Avalonia.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Property.Editor;
using Property.Editor.Avalonia;
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
        ConfigUiViewModel viewModel = new(CreateRegistry(section), new PropertyEditorViewModel());

        await viewModel.ResetCommand.ExecuteAsync(null);

        await section.Received().ResetAsync();
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
}
