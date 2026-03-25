using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace TileSetAnimator.Converters;

/// <summary>
/// Creates cropped bitmaps that follow the current tile selection.
/// </summary>
public sealed class CroppedBitmapConverter : IMultiValueConverter
{
    /// <inheritdoc />
    public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2 || values[0] is not BitmapSource source || values[1] is not Int32Rect rect)
        {
            return null;
        }

        if (rect.Width <= 0 || rect.Height <= 0)
        {
            return null;
        }

        // During image reloads/layout updates we can temporarily receive stale/out-of-range rectangles.
        // Guard against invalid crop rectangles to avoid sporadic exceptions.
        if (source.PixelWidth <= 0 || source.PixelHeight <= 0)
        {
            return null;
        }

        if (rect.X < 0 || rect.Y < 0 || rect.X >= source.PixelWidth || rect.Y >= source.PixelHeight)
        {
            return null;
        }

        if (rect.X + rect.Width > source.PixelWidth || rect.Y + rect.Height > source.PixelHeight)
        {
            return null;
        }

        try
        {
            var cropped = new CroppedBitmap(source, rect);
            cropped.Freeze();
            return cropped;
        }
        catch (ArgumentException)
        {
            // invalid region
            return null;
        }
        catch (InvalidOperationException)
        {
            // source not ready / disposed during reload
            return null;
        }
    }

    /// <inheritdoc />
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotSupportedException();
}
