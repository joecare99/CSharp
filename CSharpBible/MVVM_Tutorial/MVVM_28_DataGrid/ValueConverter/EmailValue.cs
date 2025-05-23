﻿// ***********************************************************************
// Assembly         : MVVM_28_DataGrid
// Author           : Mir
// Created          : 05-12-2023
//
// Last Modified By : Mir
// Last Modified On : 05-12-2023
// ***********************************************************************
// <copyright file="EmailValue.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Globalization;
using System.Windows.Data;

/// <summary>
/// The ValueConverter namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_28_DataGrid.ValueConverter;

/// <summary>
/// Class EmailValue.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
/// <autogeneratedoc />
public class EmailValue : IValueConverter
{
    /// <summary>
    /// The prefix
    /// </summary>
    /// <autogeneratedoc />
    const string Prefix = "mailto:";
    /// <summary>
    /// Konvertiert einen Wert.
    /// </summary>
    /// <param name="value">Der von der Bindungsquelle erzeugte Wert.</param>
    /// <param name="targetType">Der Typ der Bindungsziel-Eigenschaft.</param>
    /// <param name="parameter">Der zu verwendende Konverterparameter.</param>
    /// <param name="culture">Die im Konverter zu verwendende Kultur.</param>
    /// <returns>Ein konvertierter Wert. Wenn die Methode null zurückgibt, wird der gültige NULL-Wert verwendet.</returns>
    /// <autogeneratedoc />
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is string email && !string.IsNullOrEmpty(email)? $"{Prefix}{email}" : string.Empty;
    }

    /// <summary>
    /// Konvertiert einen Wert.
    /// </summary>
    /// <param name="value">Der Wert, der vom Bindungsziel erzeugt wird.</param>
    /// <param name="targetType">Der Typ, in den konvertiert werden soll.</param>
    /// <param name="parameter">Der zu verwendende Konverterparameter.</param>
    /// <param name="culture">Die im Konverter zu verwendende Kultur.</param>
    /// <returns>Ein konvertierter Wert. Wenn die Methode null zurückgibt, wird der gültige NULL-Wert verwendet.</returns>
    /// <autogeneratedoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is string email && email.ToLower().StartsWith(Prefix) ? email.Substring(Prefix.Length)  : value ?? string.Empty; 
    }
}
