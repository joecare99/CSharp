// <copyright file="BoolToColorConverter.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>

using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace AA21_Buttons.Converters;

/// <summary>
/// Konvertiert einen booleschen Wert oder Array-Element zu einer Farbe für die Button-Darstellung.
/// </summary>
public class BoolToColorConverter : IValueConverter
{
    /// <summary>
    /// Farbe für true-Werte (aktiver Button)
    /// </summary>
    public static Color TrueColor = Colors.Green;

    /// <summary>
    /// Farbe für false-Werte (inaktiver Button)
    /// </summary>
    public static Color FalseColor = Colors.DarkRed;

    /// <summary>
    /// Konvertiert einen Wert zu einer Farbe.
    /// </summary>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            bool b when parameter is string s && s.StartsWith("#") =>
                new SolidColorBrush(ParseColorFromParameter(s, b)),
            bool b =>
                new SolidColorBrush(b ? TrueColor : FalseColor),
            bool[] ba when parameter is string s && int.TryParse(s, out int idx) =>
                new SolidColorBrush(ba[idx] ? TrueColor : FalseColor),
            _ => new SolidColorBrush(FalseColor)
        };
    }

    /// <summary>
    /// Umgekehrte Konvertierung nicht unterstützt.
    /// </summary>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Parsed Farben aus Parameter-String (Format: "#color1:#color2").
    /// </summary>
    private static Color ParseColorFromParameter(string parameter, bool value)
    {
        var colors = parameter.Split(':');
        if (colors.Length < 2)
            return value ? TrueColor : FalseColor;

        string colorStr = colors[value ? 1 : 0];
        return Color.Parse(colorStr);
    }
}
