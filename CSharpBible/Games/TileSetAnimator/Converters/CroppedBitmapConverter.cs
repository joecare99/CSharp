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

        var cropped = new CroppedBitmap(source, rect);
        cropped.Freeze();
        return cropped;
    }

    /// <inheritdoc />
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotSupportedException();
}
