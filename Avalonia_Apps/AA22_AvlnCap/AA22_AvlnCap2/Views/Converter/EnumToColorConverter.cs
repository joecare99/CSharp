// ***********************************************************************
// Assembly         : AA22_AvlnCap
// Author           : Mir
// Created          : 08-14-2022
//
// Last Modified By : Mir
// Last Modified On : 08-14-2022
// ***********************************************************************
// <copyright file="EnumToColorConverter.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace AA22_AvlnCap2.Views.Converter;

/// <summary>
/// Class EnumToColorConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class EnumToColorConverter : IValueConverter
{

    public List<Color> colors { get; set; } = new List<Color> { };
    /// <summary>
    /// The zero color
    /// </summary>
    public static Color ZeroColor = Colors.Black;
    /// <summary>
    /// The true color
    /// </summary>
    public static Color TrueColor = Colors.Green;
    /// <summary>
    /// The false color
    /// </summary>
    public static Color FalseColor = Colors.Red;
    /// <summary>
    /// The three color
    /// </summary>
    public static Color ThreeColor = Colors.Yellow;
    /// <summary>
    /// The four color
    /// </summary>
    public static Color FourColor = Colors.Blue;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // Return an Avalonia Brush for UI elements
        static IBrush BrushFromColor(Color c) => new SolidColorBrush(c);

        return value switch
        {
            // single tile value
            int i when colors.Count > i && i >= 0 => BrushFromColor(colors[i]),
            int i => BrushFromColor(i switch
            {
                1 => FalseColor,
                2 => TrueColor,
                3 => ThreeColor,
                4 => FourColor,
                _ => ZeroColor
            }),

            bool b when colors.Count>2 => BrushFromColor(b ? colors[2] : colors[1]),
            bool b => BrushFromColor(b ? TrueColor : FalseColor),

            bool[] ba when parameter is string s && int.TryParse(s,out _) &&  colors.Count>2=> BrushFromColor(ba[int.Parse(s)] ? colors[2] : colors[1]),
            bool[] ba when parameter is string s && int.TryParse(s,out _)=> BrushFromColor(ba[int.Parse(s)] ? TrueColor : FalseColor),                
            int[] ia when parameter is string s && int.TryParse(s, out int _is) && _is < ia.Length && colors.Count > ia[_is] 
                => BrushFromColor(colors[ia[_is]]),
            int[] ia when parameter is string s && int.TryParse(s, out int _is) && _is < ia.Length 
                => BrushFromColor(ia[_is] switch
                {
                    1 => FalseColor,
                    2 => TrueColor,
                    3 => ThreeColor,
                    4 => FourColor,
                    _ => ZeroColor
                }),
            null => Brushes.Transparent,
            bool[] => Brushes.Transparent,
            int[]=> Brushes.Transparent,
            _ => Brushes.Transparent
        };
    }


    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
