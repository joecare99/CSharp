using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VTileEdit.WPF.Converters;

/// <summary>
/// Determines whether a character from the charmap equals the currently selected glyph character.
/// </summary>
public sealed class CharacterSelectionConverter : IMultiValueConverter
{
    /// <inheritdoc />
    public object? Convert(object[] values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Length < 2)
        {
            return false;
        }

        var candidate = values[0];
        var selected = values[1];

        if (candidate is null || selected is null ||
            ReferenceEquals(candidate, DependencyProperty.UnsetValue) ||
            ReferenceEquals(selected, DependencyProperty.UnsetValue))
        {
            return false;
        }

        if (candidate is char candidateChar && selected is char selectedChar)
        {
            return candidateChar == selectedChar;
        }

        var candidateText = candidate.ToString();
        var selectedText = selected.ToString();
        return string.Equals(candidateText, selectedText, StringComparison.Ordinal);
    }

    /// <inheritdoc />
    public object[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
        => throw new NotSupportedException("Character selection highlighting is one-way only.");
}
