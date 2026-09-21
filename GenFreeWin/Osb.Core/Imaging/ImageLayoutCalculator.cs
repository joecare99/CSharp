using System;

namespace Osb.Core.Imaging;

/// <summary>
/// Calculates image target dimensions using the scaling behaviour of the legacy OSB image helpers.
/// </summary>
public static class ImageLayoutCalculator
{
    /// <summary>
    /// Calculates the dimensions for an image resized to a specified height while preserving its aspect ratio.
    /// This characterizes <c>COND.PicResizeByWidth</c>, whose legacy parameter name refers to height.
    /// </summary>
    /// <param name="source">The original image dimensions.</param>
    /// <param name="targetHeight">The required target height in pixels.</param>
    /// <returns>The aspect-ratio-preserving target dimensions.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="targetHeight"/> is less than one pixel.
    /// </exception>
    public static ImageSize CalculateSizeForHeight(ImageSize source, int targetHeight)
    {
        if (targetHeight < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(targetHeight), targetHeight, "The target height must be at least one pixel.");
        }

        var targetWidth = checked((int)Math.Round(
            source.Width * (targetHeight / (double)source.Height),
            MidpointRounding.ToEven));

        return new ImageSize(targetWidth, targetHeight);
    }

    /// <summary>
    /// Calculates the dimensions for an image contained within a maximum rectangle.
    /// Images already within the rectangle retain their dimensions unless stretching is requested.
    /// </summary>
    /// <param name="source">The original image dimensions.</param>
    /// <param name="maximumWidth">The maximum target width in pixels.</param>
    /// <param name="maximumHeight">The maximum target height in pixels.</param>
    /// <param name="stretch">
    /// <see langword="true"/> to enlarge smaller images to the rectangle's limiting dimension;
    /// otherwise <see langword="false"/>.
    /// </param>
    /// <returns>The target dimensions preserving the original aspect ratio.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when either maximum dimension is less than one pixel.
    /// </exception>
    public static ImageSize CalculateContainedSize(
        ImageSize source,
        int maximumWidth,
        int maximumHeight,
        bool stretch = false)
    {
        if (maximumWidth < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumWidth), maximumWidth, "The maximum width must be at least one pixel.");
        }

        if (maximumHeight < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumHeight), maximumHeight, "The maximum height must be at least one pixel.");
        }

        if (!stretch && source.Width <= maximumWidth && source.Height <= maximumHeight)
        {
            return source;
        }

        var sourceAspectRatio = source.Width / (double)source.Height;
        var maximumAspectRatio = maximumWidth / (double)maximumHeight;

        if (sourceAspectRatio <= maximumAspectRatio)
        {
            var targetWidth = checked((int)Math.Round(
                source.Width / (source.Height / (double)maximumHeight),
                MidpointRounding.ToEven));

            return new ImageSize(targetWidth, maximumHeight);
        }

        var targetHeight = checked((int)Math.Round(
            source.Height / (source.Width / (double)maximumWidth),
            MidpointRounding.ToEven));

        return new ImageSize(maximumWidth, targetHeight);
    }
}
