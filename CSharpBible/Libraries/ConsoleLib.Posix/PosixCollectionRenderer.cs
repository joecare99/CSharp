using System;
using ConsoleLib.CommonControls;
using ConsoleLib.Interfaces;

namespace ConsoleLib.Posix;

/// <summary>Deterministic ANSI renderer for tree and tile collection controls.</summary>
public sealed class PosixCollectionRenderer : ITreeViewRenderer, ITileViewRenderer
{
    private readonly IAnsiOutput _output;

    public PosixCollectionRenderer(IAnsiOutput output) =>
        _output = output ?? throw new ArgumentNullException(nameof(output));

    public void DrawTreeView(TreeView treeView)
    {
        if (treeView is null)
            throw new ArgumentNullException(nameof(treeView));

        var row = treeView.RealDim.Top + 1;
        foreach (var node in treeView.GetVisibleNodes())
        {
            var depth = GetDepth(node);
            Write(treeView.RealDim.Left + 1, row++, $"{(node.Children.Count > 0 ? (node.IsExpanded ? '-' : '+') : ' ')} {new string(' ', depth * 2)}{node.Text}",
                node.IsSelected ? ConsoleColor.Yellow : ConsoleColor.White);
        }
    }

    public void DrawTileView(TileView tileView)
    {
        if (tileView is null)
            throw new ArgumentNullException(nameof(tileView));

        var width = Math.Max(1, tileView.TileWidth);
        var height = Math.Max(1, tileView.TileHeight);
        var columns = Math.Max(1, tileView.size.Width / width);
        var visible = tileView.GetVisibleItems();
        for (var index = 0; index < visible.Count; index++)
        {
            var column = index % columns;
            var row = index / columns;
            var x = tileView.RealDim.Left + column * width + 1;
            var y = tileView.RealDim.Top + row * height + 1;
            var text = visible[index].Text.Length > width ? visible[index].Text[..width] : visible[index].Text.PadRight(width);
            Write(x, y, text, ReferenceEquals(visible[index], tileView.SelectedItem) ? ConsoleColor.Yellow : ConsoleColor.White);
        }
    }

    private void Write(int column, int row, string text, ConsoleColor color)
    {
        _output.MoveCursorAsync(column, row).GetAwaiter().GetResult();
        _output.SetForegroundAsync(color).GetAwaiter().GetResult();
        _output.WriteAsync(text).GetAwaiter().GetResult();
        _output.ResetAsync().GetAwaiter().GetResult();
    }

    private static int GetDepth(TreeNode node)
    {
        var depth = 0;
        for (var parent = node.Parent; parent is not null; parent = parent.Parent)
            depth++;
        return depth;
    }
}
