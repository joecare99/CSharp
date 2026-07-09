using System;
using Avln_ImageEditor.Controls.Models;
using Avln_ImageEditor.Controls.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avln_ImageEditor.Controls.Tests.ViewModels;

/// <summary>
/// Tests the editor state that is independent from any concrete host platform.
/// </summary>
[TestClass]
public sealed class ImageEditorViewModelTests
{
    /// <summary>
    /// Verifies the initial editor state.
    /// </summary>
    [TestMethod]
    public void Constructor_ShouldExposeEmptyEditorState()
    {
        var viewModel = new ImageEditorViewModel();

        Assert.IsFalse(viewModel.HasDocument);
        Assert.AreEqual(ImageEditorTool.Select, viewModel.SelectedTool);
        Assert.AreEqual(100.0, viewModel.ZoomPercentage);
        Assert.AreEqual(1.0, viewModel.ZoomFactor);
        Assert.AreEqual("No image loaded", viewModel.DocumentSummary);
    }

    /// <summary>
    /// Verifies that tool selection updates the active tool.
    /// </summary>
    [TestMethod]
    public void SelectTool_ShouldUpdateSelectedTool()
    {
        var viewModel = new ImageEditorViewModel();

        viewModel.SelectTool(ImageEditorTool.Brush);

        Assert.AreEqual(ImageEditorTool.Brush, viewModel.SelectedTool);
    }

    /// <summary>
    /// Verifies that zoom commands keep the zoom state inside supported bounds.
    /// </summary>
    [TestMethod]
    public void ZoomCommands_ShouldClampZoomPercentage()
    {
        var viewModel = new ImageEditorViewModel();

        for (var i = 0; i < 100; i++)
        {
            viewModel.ZoomIn();
        }

        Assert.AreEqual(800.0, viewModel.ZoomPercentage);

        for (var i = 0; i < 100; i++)
        {
            viewModel.ZoomOut();
        }

        Assert.AreEqual(10.0, viewModel.ZoomPercentage);

        viewModel.ResetZoom();

        Assert.AreEqual(100.0, viewModel.ZoomPercentage);
    }

    /// <summary>
    /// Verifies that invalid document names are rejected before document state changes.
    /// </summary>
    [TestMethod]
    public void LoadDocument_ShouldRejectMissingDocumentName()
    {
        var viewModel = new ImageEditorViewModel();

        Assert.ThrowsExactly<ArgumentException>(() => viewModel.LoadDocument(Array.Empty<byte>(), string.Empty));
        Assert.IsFalse(viewModel.HasDocument);
    }
}
