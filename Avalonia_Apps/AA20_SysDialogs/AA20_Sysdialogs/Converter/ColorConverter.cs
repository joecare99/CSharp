// ***********************************************************************
// Assembly         : MVVM_20_Sysdialogs
// Author           : Mir
// Created          : 08-12-2022
//
// Last Modified By : Mir
// Last Modified On : 08-12-2022
// ***********************************************************************
// <copyright file="ColorConverter.cs" company="MVVM_20_Sysdialogs">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace AA20_SysDialogs.Converter;

/// <summary>
/// Class ColorConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class ColorConverter : IValueConverter
{
    /// <summary>
    /// Converts a value.
    /// </summary>
    /// <param name="value">The value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>A converted value. If the method returns <see langword="null" />, the valid null value is used.</returns>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is System.Drawing.Color drawingColor)
        {
            return new SolidColorBrush(Color.FromArgb(
                drawingColor.A,
                drawingColor.R,
                drawingColor.G,
                drawingColor.B));
        }
        return new SolidColorBrush(Colors.White);
    }

    /// <summary>
    /// Konvertiert einen Wert vom Bindungsziel zurück zum Bindungsquelltyp.
    /// Wandelt einen Avalonia <see cref="SolidColorBrush"/> zurück in einen <see cref="System.Drawing.Color"/>.
    /// </summary>
    /// <param name="value">Der Wert, der vom Bindungsziel erzeugt wird. Erwartet wird ein <see cref="SolidColorBrush"/>-Objekt.</param>
    /// <param name="targetType">Der Typ, in den konvertiert werden soll. In diesem Fall <see cref="System.Drawing.Color"/>.</param>
    /// <param name="parameter">Der zu verwendende Konverterparameter. Wird in dieser Implementierung nicht verwendet.</param>
    /// <param name="culture">Die im Konverter zu verwendende Kultur. Wird in dieser Implementierung nicht verwendet.</param>
    /// <returns>Ein <see cref="System.Drawing.Color"/>-Objekt, das aus den ARGB-Werten des Avalonia-Farbpinsels erstellt wurde.</returns>
    /// <exception cref="NotImplementedException">Wird ausgelöst, wenn der übergebene Wert kein <see cref="SolidColorBrush"/> ist.</exception>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is SolidColorBrush brush)
        {
            var color = brush.Color;
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }
        throw new NotImplementedException("ConvertBack requires a SolidColorBrush");
    }
}
