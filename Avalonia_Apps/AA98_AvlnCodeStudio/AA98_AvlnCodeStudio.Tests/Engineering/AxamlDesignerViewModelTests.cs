using AA98_AvlnCodeStudio.AxamlDesigner.UI.ViewModels;
using AA98_AvlnCodeStudio.Model.Workspaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

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
        StringAssert.Contains(viewModel.SelectedCapabilityText, "appending unavailable");

        viewModel.LoadMarkup("<Grid />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        StringAssert.Contains(viewModel.SelectedCapabilityText, "next unoccupied cell");

        viewModel.LoadMarkup("<Grid RowDefinitions='Auto,,*' />");
        viewModel.SelectedNode = viewModel.PreviewRoot;
        StringAssert.Contains(viewModel.SelectedCapabilityText, "placement unavailable");
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
            ("<Grid />", viewModel => viewModel.AddGridCommand.Execute(null)),
            ("<ProgressBar />", viewModel => viewModel.AddProgressBarCommand.Execute(null)),
            ("<ScrollViewer />", viewModel => viewModel.AddScrollViewerCommand.Execute(null)),
            ("<Separator />", viewModel => viewModel.AddSeparatorCommand.Execute(null)),
            ("<StackPanel />", viewModel => viewModel.AddStackPanelCommand.Execute(null)),
            ("<WrapPanel />", viewModel => viewModel.AddWrapPanelCommand.Execute(null)),
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
