using System;
using System.Collections.Generic;
using System.Drawing;
using ConsoleLib;
using ConsoleLib.CommonControls;
using ConsoleLib.Rendering;
using ConsoleLib.Showcase.Apps;
using ConsoleLib.Showcase.Apps.Gallery;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleLib.Showcase.Apps.Gallery.Tests;

[TestClass]
public sealed class GalleryModuleTests
{
    [TestMethod]
    public void Register_ExposesIndependentControlsGalleryDescriptor()
    {
        var context = new ShowcaseAppRegistrationContext(new EmptyServiceProvider());

        new GalleryAppModule().Register(context);

        var descriptor = context.Apps[GalleryAppModule.AppId];
        Assert.AreEqual("Controls Gallery", descriptor.Title);
        Assert.AreEqual(ShowcaseAppCategory.Controls, descriptor.Category);
        var viewModel = descriptor.CreateViewModel(context.Services);
        Assert.IsInstanceOfType<GalleryViewModel>(viewModel);
        var page = descriptor.LoadPage(context.Services, viewModel);
        Assert.IsInstanceOfType<Page>(page.Root);
        Assert.IsTrue(page.NamedControls.ContainsKey("SampleMenuBar"));
    }

    [TestMethod]
    public void Load_ContainsEveryGalleryControlAndUsesTheCxamlShell()
    {
        var result = GalleryPage.Load(new GalleryViewModel());

        Assert.IsInstanceOfType<Page>(result.Root);
        CollectionAssert.IsSubsetOf(
            new[]
            {
                "GalleryTitle", "AdvanceButton", "SampleLabel", "ResetButton", "SampleCheckBox",
                "CompactRadioButton", "ThemeComboBox", "SampleListBox", "SampleTextBox",
                "SampleProgressBar", "SampleStatusBar", "SampleScrollBar", "SampleScrollViewer",
                "ControlsPanel", "SampleStackPanel", "SampleDockPanel", "SampleGrid",
                "SampleTabControl", "SampleTileView", "SampleTreeView", "SampleMenuBar", "SampleMenuPopup"
            },
            new List<string>(result.NamedControls.Keys));

        Assert.IsInstanceOfType<StackPanel>(result.NamedControls["SampleStackPanel"]);
        Assert.IsInstanceOfType<DockPanel>(result.NamedControls["SampleDockPanel"]);
        Assert.IsInstanceOfType<Grid>(result.NamedControls["SampleGrid"]);
        Assert.IsInstanceOfType<MenuPopup>(result.NamedControls["SampleMenuPopup"]);
    }

    [TestMethod]
    public void CxamlShell_ValidatesAndLoadsAsPage()
    {
        var result = GalleryPage.LoadCxamlPage(new GalleryViewModel());

        Assert.IsInstanceOfType<Page>(result.Root);
        Assert.IsTrue(result.NamedControls.ContainsKey("GalleryTitle"));
        Assert.IsTrue(result.NamedControls.ContainsKey("AdvanceButton"));
    }

    [TestMethod]
    public void RenderedGallery_HasCanonicalHeaderSnapshotAndRespondsToAdvance()
    {
        var viewModel = new GalleryViewModel();
        var result = GalleryPage.Load(viewModel);
        using var renderer = new AttachedRenderService();

        renderer.Attach(result.Root, new Size(80, 28));
        Assert.AreEqual(
            "┌──────────────────────────────────────────────────────────────────────────────┐\n" +
            "│View   Help                                                                   │\n" +
            "│ ConsoleLib Controls Gallery                                    Advance       │",
            SnapshotRows(renderer.GetSnapshot(), 0, 3));

        result.NamedControls["AdvanceButton"].Click();
        renderer.Render();

        StringAssert.Contains(SnapshotRows(renderer.GetSnapshot(), 26, 1), "Progress advanced to 60%.");
        Assert.AreEqual(60, ((ProgressBar)result.NamedControls["SampleProgressBar"]).Value);
    }

    private static string SnapshotRows(IRenderFrameSnapshot snapshot, int startRow, int count)
    {
        var rows = new string[count];
        for (var y = 0; y < count; y++)
        {
            var characters = new char[snapshot.Size.Width];
            for (var x = 0; x < snapshot.Size.Width; x++)
                characters[x] = snapshot.GetCell(x, startRow + y).Character;
            rows[y] = new string(characters);
        }
        return string.Join("\n", rows);
    }

    private sealed class EmptyServiceProvider : IServiceProvider
    {
        public object? GetService(Type serviceType) => null;
    }
}
