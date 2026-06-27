using System;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace Avln_ImageEditor.Controls.Models;

/// <summary>
/// Represents an image document loaded into the editor surface.
/// </summary>
public sealed class ImageEditorDocument : IDisposable
{
    private readonly Bitmap _bitmap;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageEditorDocument"/> class.
    /// </summary>
    /// <param name="name">The display name of the image document.</param>
    /// <param name="bitmap">The decoded Avalonia bitmap.</param>
    public ImageEditorDocument(string name, Bitmap bitmap)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(bitmap);

        Name = name;
        _bitmap = bitmap;
        PixelSize = bitmap.PixelSize;
    }

    /// <summary>
    /// Gets the display name of the image document.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the decoded image used by Avalonia controls.
    /// </summary>
    public IImage Image => _bitmap;

    /// <summary>
    /// Gets the document size in physical pixels.
    /// </summary>
    public PixelSize PixelSize { get; }

    /// <summary>
    /// Gets a compact display summary for the document.
    /// </summary>
    public string Summary => $"{Name} · {PixelSize.Width} × {PixelSize.Height} px";

    /// <inheritdoc/>
    public void Dispose()
    {
        _bitmap.Dispose();
    }
}
