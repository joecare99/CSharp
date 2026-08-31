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
    {
        if (root is null)
            throw new ArgumentNullException(nameof(root));

        _service?.Dispose();
        _service = new AttachedRenderService();
        var width = viewport?.Width > 0 ? viewport.Value.Width : root.size.Width > 0 ? root.size.Width : 80;
        var height = viewport?.Height > 0 ? viewport.Value.Height : root.size.Height > 0 ? root.size.Height : 25;
        _service.Attach(root, new System.Drawing.Size(width, height));
        var snapshot = LastSnapshot = _service.GetSnapshot();
        var result = new StringBuilder(height * (width + Environment.NewLine.Length));
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
                result.Append(snapshot.GetCell(x, y).Character);
            if (y < height - 1)
                result.AppendLine();
        }

        return result.ToString();
    }
}
