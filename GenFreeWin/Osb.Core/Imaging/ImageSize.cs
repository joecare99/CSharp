using System;

namespace Osb.Core.Imaging;

/// <summary>
/// Represents a non-zero image dimension without introducing a dependency on an image-rendering API.
/// </summary>
public readonly record struct ImageSize
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageSize"/> struct.
    /// </summary>
    /// <param name="width">The image width in pixels.</param>
    /// <param name="height">The image height in pixels.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when either dimension is less than one pixel.
    /// </exception>
    public ImageSize(int width, int height)
    {
        if (width < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(width), width, "The width must be at least one pixel.");
        }

        if (height < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(height), height, "The height must be at least one pixel.");
        }

        Width = width;
        Height = height;
    }

    /// <summary>
    /// Gets the image width in pixels.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Gets the image height in pixels.
    /// </summary>
    public int Height { get; }
}
