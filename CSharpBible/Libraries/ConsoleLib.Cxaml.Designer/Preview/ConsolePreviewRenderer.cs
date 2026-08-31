using System;
using System.Drawing;
using System.Text;
using ConsoleLib.Interfaces;
using ConsoleLib.Rendering;

namespace ConsoleLib.Cxaml.Designer.Preview;

/// <summary>Creates a deterministic, monospace representation of a ConsoleLib control tree.</summary>
public sealed class ConsolePreviewRenderer
{
    private AttachedRenderService? _service;
    public IRenderFrameSnapshot? LastSnapshot { get; private set; }

    public string Render(IControl root)
        => Render(root, null);

    public string Render(IControl root, Size? viewport)
        => Render(root, viewport, null);

    public string Render(IControl root, Size? viewport, IControl? selectedControl)
    {
        if (root is null)
            throw new ArgumentNullException(nameof(root));

        _service?.Dispose();
        _service = new AttachedRenderService();
        var width = viewport?.Width > 0 ? viewport.Value.Width : root.size.Width > 0 ? root.size.Width : 80;
        var height = viewport?.Height > 0 ? viewport.Value.Height : root.size.Height > 0 ? root.size.Height : 25;
        _service.Attach(root, new System.Drawing.Size(width, height));
        var snapshot = LastSnapshot = _service.GetSnapshot();
        var characters = new char[width, height];
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
                characters[x, y] = snapshot.GetCell(x, y).Character;
        }

        if (selectedControl is not null)
            DrawSelectionOutline(selectedControl, characters, width, height);

        var result = new StringBuilder(height * (width + Environment.NewLine.Length));
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
                result.Append(characters[x, y]);
            if (y < height - 1)
                result.AppendLine();
        }
        return result.ToString();
    }

    private static void DrawSelectionOutline(IControl control, char[,] characters, int width, int height)
    {
        var left = Math.Max(0, control.Position.X);
        var top = Math.Max(0, control.Position.Y);
        var right = Math.Min(width - 1, control.Position.X + Math.Max(1, control.size.Width) - 1);
        var bottom = Math.Min(height - 1, control.Position.Y + Math.Max(1, control.size.Height) - 1);
        if (left > right || top > bottom)
            return;

        if (left == right && top == bottom)
        {
            characters[left, top] = '╳';
            return;
        }

        for (var x = left + 1; x < right; x++)
        {
            characters[x, top] = '═';
            if (bottom != top)
                characters[x, bottom] = '═';
        }

        for (var y = top + 1; y < bottom; y++)
        {
            characters[left, y] = '║';
            if (right != left)
                characters[right, y] = '║';
        }

        characters[left, top] = '╔';
        if (right != left)
            characters[right, top] = '╗';
        if (bottom != top)
        {
            characters[left, bottom] = '╚';
            if (right != left)
                characters[right, bottom] = '╝';
        }
    }
}
