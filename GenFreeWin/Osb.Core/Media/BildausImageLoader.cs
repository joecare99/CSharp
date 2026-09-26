using System;
using System.Drawing;
using System.IO;

namespace Osb.Core.Media;

/// <summary>
/// Simple image loader that ensures file streams are closed and returns a Bitmap instance.
/// </summary>
public static class BildausImageLoader
{
    /// <summary>
    /// Loads an image from the filesystem and returns a <see cref="Bitmap"/>.
    /// Throws <see cref="FileNotFoundException"/> when the file is missing.
    /// </summary>
    public static Bitmap LoadBitmap(string path)
    {
        if (string.IsNullOrEmpty(path)) throw new ArgumentException("Path must not be empty.", nameof(path));

        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        return new Bitmap(fs);
    }
}
