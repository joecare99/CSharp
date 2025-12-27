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
        if (values.Length < 3)
        {
            return false;
        }

        if (values[2] is not PaletteSelectionMode mode)
        {
            return false;
        }

        var foregroundSelected = values[0] as bool? ?? false;
        var backgroundSelected = values[1] as bool? ?? false;

        return mode == PaletteSelectionMode.Foreground ? foregroundSelected : backgroundSelected;
    }

    /// <inheritdoc />
    public object[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
        => throw new NotSupportedException("Palette selection is command-driven and does not support two-way binding.");
}
