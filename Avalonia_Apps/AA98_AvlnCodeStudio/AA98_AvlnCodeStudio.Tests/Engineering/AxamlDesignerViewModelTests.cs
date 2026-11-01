using AA98_AvlnCodeStudio.AxamlDesigner.UI.ViewModels;
using AA98_AvlnCodeStudio.Model.Workspaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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
