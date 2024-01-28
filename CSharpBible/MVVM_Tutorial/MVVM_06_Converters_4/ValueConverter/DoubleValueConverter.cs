﻿// ***********************************************************************
// Assembly         : MVVM_6_Converters_4
// Author           : Mir
// Created          : 07-03-2022
//
// Last Modified By : Mir
// Last Modified On : 07-04-2022
// ***********************************************************************
// <copyright file="CurrencyValueConverter.cs" company="MVVM_6_Converters_4">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Globalization;
using System.Windows.Data;

namespace MVVM_06_Converters_4.ValueConverter
{
    /// <summary>
    /// Class CurrencyValueConverter.
    /// Implements the <see cref="IValueConverter" />
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public class DoubleValueConverter : IValueConverter
    {
        public double FixedFactor { get; set; } = 1.0d;
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
            return value switch
            {
                double dval when parameter is string spar => (dval * FixedFactor).ToString(spar),
                double dval => (dval * FixedFactor).ToString(),
                _ => value?.ToString() ?? ""
            };

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
            return value switch
            {
                string sval when
                double.TryParse(sval, NumberStyles.Float, culture, out double dval) => dval / FixedFactor,
                _ when parameter is string spar && spar.Contains("{")
                    => double.NaN, // Todo:
                string sval when parameter is string spar => InnerParse(sval, spar),
                _ => double.NaN
            };

            double InnerParse(string sval, string spar)
            {
                var pp = spar.LastIndexOf('0');
                if (double.TryParse(sval.Replace(spar.Substring(pp + 1), "").Trim(), NumberStyles.Float, culture, out var dVal))
                    return dVal / FixedFactor;
                else
                    return double.NaN;

            }
        }
    }
}
