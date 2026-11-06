using AA98_AvlnCodeStudio.AxamlDesigner.UI.ViewModels;
using AA98_AvlnCodeStudio.Model.Workspaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Property.Editor;
using Property.Editor.Avalonia;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class AxamlDesignerViewModelTests
{
    [TestMethod]
    public void DesignerViewModel_ApplyUndoRedo_PublishesUpdatedMarkup()
    {
        AxamlDesignerViewModel viewModel = new();
        string? publishedMarkup = null;
        viewModel.MarkupChanged += (_, args) => publishedMarkup = args.Markup;
        viewModel.LoadMarkup(
            "<UserControl xmlns='https://github.com/avaloniaui'><StackPanel><TextBlock Text='Before' /></StackPanel></UserControl>");
        viewModel.SelectedNode = viewModel.PreviewRoot?.Children[0];
        viewModel.SelectedProperty = "Text";
        viewModel.SelectedValue = "After & updated";

        viewModel.ApplySelectedValueCommand.Execute(null);

        Assert.IsNotNull(publishedMarkup);
        StringAssert.Contains(publishedMarkup, "Text='After &amp; updated'");
        Assert.IsTrue(viewModel.CanUndo);
        viewModel.UndoCommand.Execute(null);
        StringAssert.Contains(publishedMarkup, "Text='Before'");
        viewModel.RedoCommand.Execute(null);
        StringAssert.Contains(publishedMarkup, "Text='After &amp; updated'");
        Assert.IsFalse(string.IsNullOrWhiteSpace(viewModel.StatusText));
    }

    [TestMethod]
    public void PropertyRows_DebouncedEditsAreUndoableAsOneAndInvalidDraftRequiresDiscard()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<StackPanel Width='100'><TextBlock Text='Hello' /></StackPanel>");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        AxamlPropertyRowViewModel width = viewModel.PropertyRows.Single(static row => row.Name == "Width");
        Assert.IsFalse(viewModel.PropertyRows.Single(static row => row.Name == "Height").HasValidationError);

        width.Value = "120";
        viewModel.BeginPropertyDraft(width, hasChanged: true);
        viewModel.ApplyPropertyRow(width, finalize: false);
        width.Value = "140";
        viewModel.BeginPropertyDraft(width, hasChanged: true);
        viewModel.ApplyPropertyRow(width, finalize: true);

        StringAssert.Contains(viewModel.PreviewRoot!.Properties["Width"], "140");
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("100", viewModel.PreviewRoot!.Properties["Width"]);

        width = viewModel.PropertyRows.Single(static row => row.Name == "Width");
        width.Value = "-2";
        viewModel.BeginPropertyDraft(width, hasChanged: true);
        Assert.IsTrue(width.HasValidationError);
        AxamlPreviewNode selectedNode = viewModel.SelectedNode!;
        viewModel.SelectedNode = viewModel.PreviewRoot!.Children[0];
        Assert.AreSame(selectedNode, viewModel.SelectedNode);
        Assert.IsTrue(viewModel.HasInvalidPropertyDraft);

        viewModel.DiscardPropertyDraft(width);
        Assert.IsFalse(viewModel.HasInvalidPropertyDraft);
        Assert.AreEqual("100", width.Value);
    }

    [TestMethod]
    public async Task PropertyProvider_UsesSharedPropertyContractAndPreservesUndo()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<StackPanel Width='100' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var width = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "Width");
        PropertyEditorViewModel sharedEditor = new();
        sharedEditor.SetItems(viewModel.PropertyProvider.Properties);

        Assert.AreEqual(PropertyScope.Session, viewModel.PropertyProvider.Context.Scope);
        Assert.AreEqual(1, sharedEditor.Items.Count(static item => item.PropertyItem.Name == "Width"));
        Assert.IsTrue(width.TrySetValue("120"));
        await viewModel.PropertyProvider.ApplyAsync();

        Assert.AreEqual("120", viewModel.PreviewRoot!.Properties["Width"]);
        Assert.IsTrue(viewModel.CanUndo);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("100", viewModel.PreviewRoot!.Properties["Width"]);
    }

    [TestMethod]
    public void SharedPropertyEditor_EditCompletionAppliesValueAsUndoableDesignerChange()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<StackPanel Width='100' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var width = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "Width");
        viewModel.PropertyEditor.SetItems(viewModel.PropertyProvider.Properties);
        viewModel.PropertyEditor.NotifyEditStarted(width);

        Assert.IsTrue(width.TrySetValue("120"));
        viewModel.PropertyEditor.NotifyEditCompleted(width);

        Assert.AreEqual("120", viewModel.PreviewRoot!.Properties["Width"]);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("100", viewModel.PreviewRoot!.Properties["Width"]);
    }

    [TestMethod]
    public async Task PropertyProvider_InvalidDraftBlocksApplyAndCanBeDiscarded()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<StackPanel Width='100' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        var width = viewModel.PropertyProvider.Properties.Single(static property => property.Name == "Width");

        Assert.IsFalse(width.TrySetValue("-2"));
        Assert.IsTrue(viewModel.HasInvalidPropertyDraft);
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            () => viewModel.PropertyProvider.ApplyAsync());

        await viewModel.PropertyProvider.DiscardAsync();
        Assert.IsFalse(viewModel.HasInvalidPropertyDraft);
        Assert.AreEqual("100", viewModel.PropertyRows.Single(static row => row.Name == "Width").Value);
    }

    [TestMethod]
    public void PropertyRows_ChangingSelectedControlDoesNotDuplicateSharedProperties()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup(
            "<StackPanel xmlns='https://github.com/avaloniaui'><TextBox /></StackPanel>");
        AxamlPreviewNode stackPanel = viewModel.PreviewRoot!;
        AxamlPreviewNode textBox = stackPanel.Children[0];
        viewModel.SelectedNode = stackPanel;
        viewModel.SelectedNode = textBox;

        Assert.AreEqual(
            viewModel.PropertyRows.Count,
            viewModel.PropertyRows.Select(static row => row.Name).Distinct(StringComparer.Ordinal).Count());
        Assert.IsTrue(viewModel.PropertyRows.All(static row => row.ControlType == "TextBox"));
        Assert.AreEqual(1, viewModel.PropertyRows.Count(static row => row.Name == "Height"));
        Assert.AreEqual(1, viewModel.PropertyRows.Count(static row => row.Name == "Width"));

        AxamlBindingRowViewModel binding = viewModel.BindingRows.Single();
        binding.NewMemberName = "Caption";
        viewModel.ApplyBindingRow(binding);

        StringAssert.Contains(viewModel.SelectedNode!.Properties["Text"], "Binding Caption");
    }

    [TestMethod]
    public void BindingRows_OfferCompatibleMembersAndApplyExplicitNewBinding()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<Button Command='{Binding OldCommand}' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        viewModel.SetBindingContext(
        [
            new AxamlMvvmSemanticMember("SaveCommand", "ICommand", IsCommand: true, IsWritable: false),
            new AxamlMvvmSemanticMember("Caption", "string", IsCommand: false, IsWritable: true),
        ]);

        AxamlBindingRowViewModel command = viewModel.BindingRows.Single();
        Assert.AreEqual("Command (ICommand)", command.MemberKindLabel);
        Assert.AreEqual("New command member name", command.NewMemberPlaceholder);
        CollectionAssert.AreEqual(new[] { "SaveCommand" }, command.AvailableMembers.ToArray());
        Assert.IsTrue(command.IsCreatingNewMember);
        Assert.IsFalse(command.IsSelectingExistingMember);
        command.NewMemberName = string.Empty;
        Assert.IsNull(command.GetRequestedMember());
        command.SelectExistingMemberSource();
        Assert.IsTrue(command.IsSelectingExistingMember);
        command.SelectedMember = "SaveCommand";
        Assert.AreEqual("SaveCommand", command.GetRequestedMember());
        command.NewMemberName = "DeleteCommand";
        Assert.IsTrue(command.IsCreatingNewMember);
        viewModel.ApplyBindingRow(command);

        StringAssert.Contains(viewModel.PreviewRoot!.Properties["Command"], "DeleteCommand");
        Assert.IsTrue(viewModel.CanUndo);
        Assert.IsFalse(viewModel.BindingRows.Single().HasDiagnostic);

        AxamlDesignerViewModel modeViewModel = new();
        modeViewModel.LoadMarkup("<TextBox Text='{Binding ExistingText, Mode=OneWay}' />");
        modeViewModel.SelectedNode = modeViewModel.PreviewRoot;
        modeViewModel.SetBindingContext(
        [
            new AxamlMvvmSemanticMember("ExistingText", "string", IsCommand: false, IsWritable: true),
        ]);
        AxamlBindingRowViewModel textBinding = modeViewModel.BindingRows.Single();
        Assert.AreEqual(AxamlBindingMode.OneWay, textBinding.BindingMode);
        textBinding.BindingMode = AxamlBindingMode.TwoWay;
        textBinding.UpdateSourceTrigger = AxamlBindingUpdateSourceTrigger.PropertyChanged;
        modeViewModel.ApplyBindingRow(textBinding);
        StringAssert.Contains(modeViewModel.PreviewRoot!.Properties["Text"], "Mode=TwoWay");
        StringAssert.Contains(modeViewModel.PreviewRoot!.Properties["Text"], "UpdateSourceTrigger=PropertyChanged");
        Assert.AreEqual(AxamlBindingMode.TwoWay, modeViewModel.BindingRows.Single().BindingMode);
        Assert.AreEqual(
            AxamlBindingUpdateSourceTrigger.PropertyChanged,
            modeViewModel.BindingRows.Single().UpdateSourceTrigger);
        Assert.IsTrue(modeViewModel.UndoCommand.CanExecute(null));
        modeViewModel.UndoCommand.Execute(null);
        StringAssert.Contains(modeViewModel.PreviewRoot!.Properties["Text"], "Mode=OneWay");
        Assert.IsFalse(modeViewModel.PreviewRoot!.Properties["Text"].Contains(
            "UpdateSourceTrigger",
            StringComparison.Ordinal));

        AxamlDesignerViewModel advancedViewModel = new();
        advancedViewModel.LoadMarkup("<TextBox Text='{Binding User.Name, Mode=TwoWay}' />");
        advancedViewModel.SelectedNode = advancedViewModel.PreviewRoot;
        AxamlBindingRowViewModel advancedBinding = advancedViewModel.BindingRows.Single();
        Assert.IsFalse(advancedBinding.CanEditBinding);
        Assert.IsTrue(advancedBinding.HasUnsupportedBindingExpression);
        Assert.IsNotNull(advancedBinding.BindingExpressionDiagnostic);
        Assert.IsNull(AxamlBindingRowViewModel.ReadBindingPath(
            "{Binding UserName, Mode=OneWay, Mode=TwoWay}"));
        Assert.IsNull(AxamlBindingRowViewModel.ReadBindingPath(
            "{Binding UserName, Converter=Example}"));
        Assert.AreEqual(
            "UserName",
            AxamlBindingRowViewModel.ReadBindingPath("{Binding Path=UserName}", out AxamlBindingMode simplePathMode));
        Assert.AreEqual(AxamlBindingMode.Default, simplePathMode);
        Assert.AreEqual(
            "UserName",
            AxamlBindingRowViewModel.ReadBindingPath(
                "{Binding, Path=UserName, Mode=TwoWay}",
                out AxamlBindingMode explicitPathMode));
        Assert.AreEqual(AxamlBindingMode.TwoWay, explicitPathMode);
        Assert.AreEqual(
            "UserName",
            AxamlBindingRowViewModel.ReadBindingPath(
                "{Binding UserName, Mode=Default}",
                out AxamlBindingMode explicitDefaultMode));
        Assert.AreEqual(AxamlBindingMode.Default, explicitDefaultMode);
        Assert.AreEqual(
            "UserName",
            AxamlBindingRowViewModel.ReadBindingPath(
                "{Binding UserName, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}",
                out AxamlBindingMode updateMode,
                out AxamlBindingUpdateSourceTrigger updateTrigger));
        Assert.AreEqual(AxamlBindingMode.TwoWay, updateMode);
        Assert.AreEqual(AxamlBindingUpdateSourceTrigger.PropertyChanged, updateTrigger);
        Assert.AreEqual(
            "UserName",
            AxamlBindingRowViewModel.ReadBindingPath(
                "{Binding UserName, UpdateSourceTrigger=DataValidation}",
                out _,
                out AxamlBindingUpdateSourceTrigger validationTrigger));
        Assert.AreEqual(AxamlBindingUpdateSourceTrigger.DataValidation, validationTrigger);
        Assert.IsNull(AxamlBindingRowViewModel.ReadBindingPath(
            "{Binding UserName, UpdateSourceTrigger=PropertyChanged, UpdateSourceTrigger=LostFocus}"));

        viewModel.SetBindingContext(
        [
            new AxamlMvvmSemanticMember("SaveCommand", "ICommand", IsCommand: true, IsWritable: false),
            new AxamlMvvmSemanticMember("Caption", "string", IsCommand: false, IsWritable: true),
        ]);
        command = viewModel.BindingRows.Single();
        command.NewMemberName = "Caption";
        viewModel.ApplyBindingRow(command);

        StringAssert.Contains(viewModel.PreviewRoot!.Properties["Command"], "DeleteCommand");
        StringAssert.Contains(command.Diagnostic, "incompatible");

        AxamlDesignerViewModel propertyViewModel = new();
        propertyViewModel.LoadMarkup("<TextBox />");
        propertyViewModel.SelectedNode = propertyViewModel.PreviewRoot;
        propertyViewModel.SetBindingContext([]);
        AxamlBindingRowViewModel property = propertyViewModel.BindingRows.Single();
        Assert.AreEqual("Property (string)", property.MemberKindLabel);
        Assert.AreEqual("New string property name", property.NewMemberPlaceholder);
        CollectionAssert.AreEqual(new[] { "Create new member" }, property.MemberSources.ToArray());
        Assert.IsTrue(property.IsCreatingNewMember);
        Assert.AreEqual(AxamlBindingMode.TwoWay, property.BindingMode);
        Assert.IsTrue(property.SupportsUpdateSourceTrigger);

        AxamlDesignerViewModel displayViewModel = new();
        displayViewModel.LoadMarkup("<TextBlock />");
        displayViewModel.SelectedNode = displayViewModel.PreviewRoot;
        Assert.AreEqual(AxamlBindingMode.OneWay, displayViewModel.BindingRows.Single().BindingMode);
        Assert.IsFalse(displayViewModel.BindingRows.Single().SupportsUpdateSourceTrigger);
    }

    [TestMethod]
    public async Task BindingRows_SendMissingMemberToWorkspaceReviewHandler()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<TextBox Text='unsaved draft' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        viewModel.SetBindingContext([]);
        AxamlBindingRowViewModel row = viewModel.BindingRows.Single();
        row.NewMemberName = "Caption";
        bool invoked = false;
        viewModel.ApplyMissingBindingAsync = requestedRow =>
        {
            invoked = ReferenceEquals(requestedRow, row);
            return Task.FromResult(false);
        };

        await viewModel.ApplyBindingRowAsync(row);

        Assert.IsTrue(invoked);
        Assert.AreEqual("unsaved draft", viewModel.PreviewRoot!.Properties["Text"]);
        Assert.IsFalse(viewModel.CanUndo);
    }

    [TestMethod]
    public void DesignerViewModel_InvalidMarkupSurfacesErrorAndCanRecover()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<StackPanel>");

        Assert.IsNull(viewModel.PreviewRoot);
        Assert.IsTrue(viewModel.HasPreview);
        Assert.IsFalse(string.IsNullOrWhiteSpace(viewModel.ErrorText));

        viewModel.LoadMarkup("<StackPanel />");

        Assert.IsNotNull(viewModel.PreviewRoot);
        Assert.IsTrue(viewModel.HasPreview);
        Assert.AreEqual(string.Empty, viewModel.ErrorText);
    }

    [TestMethod]
    public void DesignerViewModel_PreservesValidPreviewElementsWhenMarkupIsMalformed()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup(
            "<StackPanel xmlns='https://github.com/avaloniaui'><TextBlock Text='Visible' /><Button Content=broken /><Label Content='Also visible' /></StackPanel>");

        Assert.IsNotNull(viewModel.PreviewRoot);
        Assert.IsTrue(viewModel.PreviewRoot.IsRecoveryRoot);
        Assert.AreEqual(3, viewModel.PreviewRoot.Children.Count);
        Assert.IsFalse(viewModel.PreviewRoot.Children[1].IsSupported);
        Assert.IsTrue(viewModel.HasPreview);
        StringAssert.Contains(viewModel.ErrorText, "Partial preview");
        Assert.IsFalse(viewModel.CanUndo);
    }

    [TestMethod]
    public void DesignerViewModel_ReportsCapabilitiesForSupportedAndUnknownControls()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup(
            "<Window xmlns='https://github.com/avaloniaui' xmlns:custom='using:Sample'><TextBox /><custom:SpecialEditor /></Window>");

        viewModel.SelectedNode = viewModel.PreviewRoot!.Children[0];
        StringAssert.Contains(viewModel.SelectedCapabilityText, "Editable: Height, Margin, PlaceholderText, Text, Width");
        StringAssert.Contains(viewModel.SelectedCapabilityText, "Binding analysis: Text");
        StringAssert.Contains(viewModel.SelectedCapabilityText, "MVVM generation: supported for named controls");

        viewModel.SelectedNode = viewModel.PreviewRoot.Children[1];
        StringAssert.Contains(viewModel.SelectedCapabilityText, "Safe placeholder preview only");
        StringAssert.Contains(viewModel.SelectedCapabilityText, "MVVM generation are unavailable");

        viewModel.LoadMarkup(
            "<Window xmlns='https://github.com/avaloniaui' xmlns:custom='urn:sample'><custom:Button /></Window>");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        StringAssert.Contains(viewModel.SelectedCapabilityText, "Safe placeholder preview only");

        viewModel.LoadMarkup("<StackPanel><TextBlock /></StackPanel>");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        StringAssert.Contains(viewModel.SelectedCapabilityText, "appending supported");

        viewModel.LoadMarkup("<StackPanel>unmodeled text<TextBlock /></StackPanel>");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        StringAssert.Contains(viewModel.SelectedCapabilityText, "correct invalid or unsupported markup");
        viewModel.AddButtonCommand.Execute(null);
        StringAssert.Contains(viewModel.StatusText, "Correct invalid inline text");

        viewModel.LoadMarkup("<DockPanel><TextBlock /></DockPanel>");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        StringAssert.Contains(viewModel.SelectedCapabilityText, "docked children stay before Client");
        Assert.IsTrue(viewModel.IsDockPanelSelected);

        viewModel.LoadMarkup("<DockPanel LastChildFill='False'><TextBlock /></DockPanel>");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        StringAssert.Contains(viewModel.SelectedCapabilityText, "LastChildFill is disabled");

        viewModel.LoadMarkup("<Grid />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        StringAssert.Contains(viewModel.SelectedCapabilityText, "next unoccupied cell");

        viewModel.LoadMarkup("<Grid RowDefinitions='Auto,,*' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        StringAssert.Contains(viewModel.SelectedCapabilityText, "correct invalid or unsupported markup");
    }

    [TestMethod]
    public void DesignerViewModel_DockPanelPaletteKeepsClientLastAndAddsDockedChildren()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<DockPanel />");
        viewModel.SelectedNode = viewModel.PreviewRoot;

        Assert.IsTrue(viewModel.IsDockPanelSelected);
        Assert.AreEqual(AxamlDockPanelPlacement.Client, viewModel.SelectedDockPanelPlacement);
        viewModel.AddTextBlockCommand.Execute(null);

        viewModel.SelectedDockPanelPlacement = AxamlDockPanelPlacement.Top;
        viewModel.AddButtonCommand.Execute(null);
        viewModel.SelectedDockPanelPlacement = AxamlDockPanelPlacement.Bottom;
        viewModel.AddSeparatorCommand.Execute(null);

        Assert.AreEqual("Button", viewModel.PreviewRoot?.Children[0].TypeName);
        Assert.AreEqual("Top", viewModel.PreviewRoot?.Children[0].Properties["DockPanel.Dock"]);
        Assert.AreEqual("Separator", viewModel.PreviewRoot?.Children[1].TypeName);
        Assert.AreEqual("Bottom", viewModel.PreviewRoot?.Children[1].Properties["DockPanel.Dock"]);
        Assert.AreEqual("TextBlock", viewModel.PreviewRoot?.Children[2].TypeName);
        Assert.IsTrue(viewModel.CanUndo);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual(2, viewModel.PreviewRoot?.Children.Count);
    }

    [TestMethod]
    public void DesignerViewModel_AddControlPublishesMarkupAndReportsInvalidContainer()
    {
        AxamlDesignerViewModel viewModel = new();
        string? publishedMarkup = null;
        viewModel.MarkupChanged += (_, args) => publishedMarkup = args.Markup;
        viewModel.LoadMarkup("<StackPanel />");
        viewModel.SelectedNode = viewModel.PreviewRoot;

        viewModel.AddTextBlockCommand.Execute(null);

        StringAssert.Contains(publishedMarkup, "<TextBlock />");
        StringAssert.Contains(viewModel.StatusText, "inserted");
        Assert.IsTrue(viewModel.CanUndo);
        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("<StackPanel />", publishedMarkup);

        viewModel.LoadMarkup("<TextBlock />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        viewModel.AddButtonCommand.Execute(null);
        StringAssert.Contains(viewModel.StatusText, "cannot contain another child");
    }

    [TestMethod]
    public void DesignerViewModel_PaletteAddsCatalogInputAndLayoutControls()
    {
        (string Markup, Action<AxamlDesignerViewModel> Execute)[] paletteActions =
        [
            ("<Label />", viewModel => viewModel.AddLabelCommand.Execute(null)),
            ("<CheckBox />", viewModel => viewModel.AddCheckBoxCommand.Execute(null)),
            ("<ComboBox />", viewModel => viewModel.AddComboBoxCommand.Execute(null)),
            ("<NumericUpDown />", viewModel => viewModel.AddNumericUpDownCommand.Execute(null)),
            ("<RadioButton />", viewModel => viewModel.AddRadioButtonCommand.Execute(null)),
            ("<Slider />", viewModel => viewModel.AddSliderCommand.Execute(null)),
            ("<ToggleSwitch />", viewModel => viewModel.AddToggleSwitchCommand.Execute(null)),
            ("<Border />", viewModel => viewModel.AddBorderCommand.Execute(null)),
            ("<ContentControl />", viewModel => viewModel.AddContentControlCommand.Execute(null)),
            ("<DockPanel />", viewModel => viewModel.AddDockPanelCommand.Execute(null)),
            ("<Expander />", viewModel => viewModel.AddExpanderCommand.Execute(null)),
            ("<GroupBox />", viewModel => viewModel.AddGroupBoxCommand.Execute(null)),
            ("<Grid />", viewModel => viewModel.AddGridCommand.Execute(null)),
            ("<ProgressBar />", viewModel => viewModel.AddProgressBarCommand.Execute(null)),
            ("<ScrollViewer />", viewModel => viewModel.AddScrollViewerCommand.Execute(null)),
            ("<Separator />", viewModel => viewModel.AddSeparatorCommand.Execute(null)),
            ("<StackPanel />", viewModel => viewModel.AddStackPanelCommand.Execute(null)),
            ("<WrapPanel />", viewModel => viewModel.AddWrapPanelCommand.Execute(null)),
            ("<Viewbox />", viewModel => viewModel.AddViewboxCommand.Execute(null)),
            ("<UniformGrid />", viewModel => viewModel.AddUniformGridCommand.Execute(null)),
        ];

        foreach ((string expectedControl, Action<AxamlDesignerViewModel> execute) in paletteActions)
        {
            AxamlDesignerViewModel viewModel = new();
            viewModel.LoadMarkup("<StackPanel />");
            viewModel.SelectedNode = viewModel.PreviewRoot;

            execute(viewModel);

            StringAssert.Contains(viewModel.PreviewRoot?.Children.Single().SourceStartTag, expectedControl);
            Assert.IsTrue(viewModel.CanUndo);
            Assert.IsFalse(string.IsNullOrWhiteSpace(viewModel.StatusText));
        }
    }

    [TestMethod]
    public void DesignerViewModel_ResizeElementPublishesOneUndoableLayoutEdit()
    {
        AxamlDesignerViewModel viewModel = new();
        string? publishedMarkup = null;
        viewModel.MarkupChanged += (_, args) => publishedMarkup = args.Markup;
        viewModel.LoadMarkup("<StackPanel><TextBlock Text='Resize me' /></StackPanel>");
        AxamlPreviewNode node = viewModel.PreviewRoot!;

        Assert.IsTrue(viewModel.ResizeElement(node, 180.5, 72.5));
        StringAssert.Contains(publishedMarkup, "Width=\"180.5\"");
        StringAssert.Contains(publishedMarkup, "Height=\"72.5\"");
        Assert.AreEqual("Width", viewModel.SelectedProperty);

        viewModel.UndoCommand.Execute(null);
        Assert.AreEqual("<StackPanel><TextBlock Text='Resize me' /></StackPanel>", publishedMarkup);
        viewModel.RedoCommand.Execute(null);
        StringAssert.Contains(publishedMarkup, "Width=\"180.5\"");
        StringAssert.Contains(publishedMarkup, "Height=\"72.5\"");
    }

    [TestMethod]
    public void DesignerViewModel_ResizeElementClampsDimensionsAndRejectsUnsupportedMarkup()
    {
        AxamlDesignerViewModel viewModel = new();
        viewModel.LoadMarkup("<StackPanel />");

        Assert.IsTrue(viewModel.ResizeElement(viewModel.PreviewRoot!, 9000, 0));
        StringAssert.Contains(viewModel.PreviewRoot!.SourceStartTag, "Width=\"4096\"");
        StringAssert.Contains(viewModel.PreviewRoot.SourceStartTag, "Height=\"16\"");

        viewModel.LoadMarkup("<StackPanel xmlns='https://github.com/avaloniaui'><StackPanel.Width>120</StackPanel.Width></StackPanel>");
        string originalMarkup = viewModel.PreviewRoot!.SourceStartTag!;
        Assert.IsFalse(viewModel.ResizeElement(viewModel.PreviewRoot, 200, 80));
        Assert.IsFalse(viewModel.CanUndo);
        Assert.AreEqual(originalMarkup, viewModel.PreviewRoot.SourceStartTag);
        StringAssert.Contains(viewModel.StatusText, "resize failed");
    }
}
