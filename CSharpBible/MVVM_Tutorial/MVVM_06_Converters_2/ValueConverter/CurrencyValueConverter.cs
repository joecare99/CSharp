// ***********************************************************************
// Assembly         : MVVM_6_Converters_2
// Author           : Mir
// Created          : 07-03-2022
//
// Last Modified By : Mir
// Last Modified On : 07-19-2022
// ***********************************************************************
// <copyright file="CurrencyValueConverter.cs" company="MVVM_6_Converters_2">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Globalization;
using System.Windows.Data;

namespace MVVM_06_Converters_2.ValueConverter
{
    /// <summary>
    /// Class CurrencyValueConverter.
    /// Implements the <see cref="IValueConverter" />
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public class CurrencyValueConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns <see langword="null" />, the valid null value is used.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal dval)
                return dval.ToString("0.00€",culture);
            else
                return value.ToString() ?? "";

        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns <see langword="null" />, the valid null value is used.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string sval && parameter is string spar)
                return decimal.Parse(sval.Replace(spar.Substring(spar.Length - 1), "").Trim(),culture);
            else
                return "";
        }
    }
}
