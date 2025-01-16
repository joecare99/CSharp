// ***********************************************************************
// Assembly         : MVVM_22_WpfCap
// Author           : Mir
// Created          : 08-14-2022
//
// Last Modified By : Mir
// Last Modified On : 08-14-2022
// ***********************************************************************
// <copyright file="IntToStringConverter.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Globalization;
using System.Windows.Data;

namespace MVVM_22_WpfCap.Converter;

/// <summary>
/// Class IntToStringConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class IntToStringConverter : IValueConverter
{
    /// <summary>
    /// Konvertiert einen Wert.
    /// </summary>
    /// <param name="value">Der von der Bindungsquelle erzeugte Wert.</param>
    /// <param name="targetType">Der Typ der Bindungsziel-Eigenschaft.</param>
    /// <param name="parameter">Der zu verwendende Konverterparameter.</param>
    /// <param name="culture">Die im Konverter zu verwendende Kultur.</param>
    /// <returns>Ein konvertierter Wert.
    /// Wenn die Methode <see langword="null" /> zurückgibt, wird der gültige NULL-Wert verwendet.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            int i => i.ToString(CultureInfo.InvariantCulture),
            null => "",
            _ => value.ToString() ?? "",
        };
    }

    /// <summary>
    /// Konvertiert einen Wert.
    /// </summary>
    /// <param name="value">Der Wert, der vom Bindungsziel erzeugt wird.</param>
    /// <param name="targetType">Der Typ, in den konvertiert werden soll.</param>
    /// <param name="parameter">Der zu verwendende Konverterparameter.</param>
    /// <param name="culture">Die im Konverter zu verwendende Kultur.</param>
    /// <returns>Ein konvertierter Wert.
    /// Wenn die Methode <see langword="null" /> zurückgibt, wird der gültige NULL-Wert verwendet.</returns>
    /// <exception cref="System.NotImplementedException"></exception>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
