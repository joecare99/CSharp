using System;
using System.Drawing;
using System.Threading.Tasks;
using ConsoleLib;
using ConsoleLib.CommonControls;
using ConsoleLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#if NET5_0_OR_GREATER
using ConsoleLib.Posix;
#endif
namespace ConsoleLibTests;

[TestClass]
public sealed class AnsiFrameRendererTests
{
    #if NET5_0_OR_GREATER
    [TestMethod]
    public async Task Renderer_WritesFrameCellsAndResetsAttributes()
    {
        await using var transport = new InMemoryTerminalTransport(
            new TerminalCapabilities(true, true, true, false, false, false, false, false));
        await transport.OpenAsync();
        var output = new AnsiOutputWriter(transport);
        var frame = new InMemoryRenderContext(new Size(2, 1));
        frame.SetCell(0, 0, new TerminalCell('O', ConsoleColor.Green, ConsoleColor.Black));
        frame.SetCell(1, 0, new TerminalCell('K', ConsoleColor.Green, ConsoleColor.Black));

        await new AnsiFrameRenderer(output).RenderAsync(frame);

        StringAssert.Contains(transport.Output, "\u001b[1;1H\u001b[92m\u001b[40mOK\u001b[0m");
    }

    [TestMethod]
    public async Task CollectionRenderer_DrawsSelectedTreeAndTileItemsAtControlPositions()
    {
        await using var transport = new InMemoryTerminalTransport();
        await transport.OpenAsync();
        var renderer = new PosixCollectionRenderer(new AnsiOutputWriter(transport));

        var tree = new TreeView { Dimension = new Rectangle(2, 3, 20, 4) };
        var root = new TreeNode("Root");
        root.Add(new TreeNode("Child"));
        tree.Nodes.Add(root);
        Assert.IsTrue(tree.SelectNext());
        renderer.DrawTreeView(tree);

        var tiles = new TileView { Dimension = new Rectangle(4, 5, 6, 2), TileWidth = 3, TileHeight = 1 };
        tiles.SetItems(new[] { new TileItem("One"), new TileItem("Two") });
        renderer.DrawTileView(tiles);

        StringAssert.Contains(transport.Output, "\u001b[4;3H");
        StringAssert.Contains(transport.Output, "Root");
        StringAssert.Contains(transport.Output, "\u001b[6;5H");
        StringAssert.Contains(transport.Output, "One");
        StringAssert.Contains(transport.Output, "Two");
    }

    [TestMethod]
    public async Task FormRenderer_DrawsControlsWithSelectionAndValueState()
    {
        await using var transport = new InMemoryTerminalTransport();
        await transport.OpenAsync();
        var renderer = new PosixFormRenderer(new AnsiOutputWriter(transport));

        var checkBox = new CheckBox { Dimension = new Rectangle(1, 1, 6, 1), IsChecked = true };
        checkBox.SetText("Ready");
        renderer.DrawCheckBox(checkBox);

        var progress = new ProgressBar { Dimension = new Rectangle(1, 2, 4, 1), Value = 50 };
        renderer.DrawProgressBar(progress);

        var combo = new ComboBox { Dimension = new Rectangle(1, 3, 8, 1) };
        combo.Items.Add("Choice");
        combo.SelectNext();
        renderer.DrawComboBox(combo);

        var status = new StatusBar { Dimension = new Rectangle(1, 4, 8, 1), Status = "Saved" };
        renderer.DrawStatusBar(status);

        var tabs = new TabControl { Dimension = new Rectangle(1, 5, 12, 1) };
        tabs.Items.Add(new TabItem("One"));
        tabs.Items.Add(new TabItem("Two"));
        Assert.IsTrue(tabs.SelectNext());
        renderer.DrawTabControl(tabs);

        StringAssert.Contains(transport.Output, "[x] R");
        StringAssert.Contains(transport.Output, "##--");
        StringAssert.Contains(transport.Output, "[Choice]");
        StringAssert.Contains(transport.Output, "Saved");
        StringAssert.Contains(transport.Output, "[One] Two");
    }
    #endif
}
