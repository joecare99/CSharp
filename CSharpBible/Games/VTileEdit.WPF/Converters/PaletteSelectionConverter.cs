using System;
using System.Globalization;
using System.Windows.Data;
using VTileEdit.WPF.ViewModels;

namespace VTileEdit.WPF.Converters;

/// <summary>
/// Projects the appropriate selection flag for palette swatches based on the selector mode.
/// </summary>
public sealed class PaletteSelectionConverter : IMultiValueConverter
{
    /// <inheritdoc />
    public object? Convert(object[] values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Length < 2)
        {
            return false;
        }

        if (values[0] is not ColorSwatchViewModel swatch || values[1] is not PaletteSelectionMode mode)
        {
            return false;
        }

        return mode == PaletteSelectionMode.Foreground ? swatch.IsForegroundSelection : swatch.IsBackgroundSelection;
    }

    /// <inheritdoc />
    public object[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
        => throw new NotSupportedException("Palette selection is command-driven and does not support two-way binding.");
}
