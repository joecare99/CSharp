// <copyright file="BoolToColorConverter.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace AA21_Buttons.Converters;

/// <summary>
/// Konvertiert einen booleschen Wert oder Array-Element zu einer Farbe für die Button-Darstellung.
/// </summary>
public class BoolToColorConverter : IValueConverter
{
    /// <summary>
    /// Farbe für true-Werte (aktiver Button)
    /// </summary>
    public IBrush? TrueColor { get; set; } = Brushes.Lime;

    /// <summary>
    /// Farbe für false-Werte (inaktiver Button)
    /// </summary>
    public IBrush? FalseColor { get; set; } = Brushes.Maroon;

    /// <summary>
    /// Konvertiert einen Wert zu einer Farbe.
    /// </summary>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool result = false;

        // Direkter boolescher Wert
        if (value is bool b)
        {
            result = b;
        }
        // Array-/Listen mit Index-Parameter
        else if (parameter != null && TryGetIndex(parameter, out var index))
        {
            if (value is bool[] arr && index >= 0 && index < arr.Length)
                result = arr[index];
            else if (value is IList list && index >= 0 && index < list.Count && list[index] is bool lb)
                result = lb;
            else if (value is IReadOnlyList<bool> ro && index >= 0 && index < ro.Count)
                result = ro[index];
        }

        return result ? TrueColor : FalseColor;
    }

    /// <summary>
    /// Umgekehrte Konvertierung nicht verwendet.
    /// </summary>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => BindingOperations.DoNothing;

    private static bool TryGetIndex(object parameter, out int index)
    {
        index = -1;
        switch (parameter)
        {
            case int i:
                index = i ; // Annahme: Buttons verwenden 1-basierte Indizes
                return true;
            case string s when int.TryParse(s, out var parsed):
                index = parsed ;
                return true;
            default:
                return false;
        }
    }
}
