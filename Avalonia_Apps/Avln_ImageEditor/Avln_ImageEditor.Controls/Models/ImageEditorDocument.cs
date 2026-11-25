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

        ImageDecodeLimits.ValidateSize(bitmap.PixelSize.Width, bitmap.PixelSize.Height);
        Name = name;
        _bitmap = bitmap;
        PixelSize = bitmap.PixelSize;
        DisplayImage = new PixelCoordinateImage(bitmap);
    }

    /// <summary>
    /// Gets the display name of the image document.
    /// </summary>
    public string Name { get; private set; }

    internal Bitmap Bitmap => _bitmap;

    internal void Rename(string name) => Name = name;

    /// <summary>
    /// Gets the decoded image used by Avalonia controls.
    /// </summary>
    public IImage Image => _bitmap;

    /// <summary>Image view whose logical coordinates correspond to the document's pixels.</summary>
    public IImage DisplayImage { get; }

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

    private sealed class PixelCoordinateImage(Bitmap bitmap) : IImage
    {
        public Size Size => new(bitmap.PixelSize.Width, bitmap.PixelSize.Height);

        public void Draw(DrawingContext context, Rect sourceRect, Rect destRect)
        {
            // The editor uses one logical unit per image pixel, independent of bitmap DPI.
            ((IImage)bitmap).Draw(context, sourceRect, destRect);
        }
    }
}
