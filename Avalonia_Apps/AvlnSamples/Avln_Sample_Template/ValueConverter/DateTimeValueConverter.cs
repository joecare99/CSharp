// ***********************************************************************
// Assembly   : Avln_Sample_Template
// Author    : Mir
// Created    : 01-15-2025
//
// Last Modified By : Mir
// Last Modified On : 01-15-2025
// ***********************************************************************
// <copyright file="DateTimeValueConverter.cs" company="JC-Soft">
//     Copyright (c) JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace Avln_Sample_Template.ValueConverter;

/// <summary>
/// Class DateTimeValueConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class DateTimeValueConverter : IValueConverter
{
    /// <summary>
    /// Converts a value.
    /// </summary>
    /// <param name="value">The value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>A converted value. If the method returns <see langword="null" />, the valid null value is used.</returns>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
 {
   if (value is DateTime dt)
        {
if (parameter is string spar)
 return dt.ToString(spar);
      else
      return dt.ToString(culture ?? CultureInfo.CurrentCulture);
        }
   else
  return value?.ToString() ?? "";
    }

    /// <summary>
    /// Converts a value.
    /// </summary>
    /// <param name="value">The value that is produced by the binding target.</param>
/// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>A converted value. If the method returns <see langword="null" />, the valid null value is used.</returns>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo? culture)
    {
        if (value is string sval && DateTime.TryParse(sval.Trim(), culture ?? CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out var dt))
        return dt;
        else
            return DateTime.MinValue;
 }
}
