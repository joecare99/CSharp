// ***********************************************************************
// Assembly         : MVVM_18_MultiConverters
// Author           : Mir
// Created          : 07-05-2022
//
// Last Modified By : Mir
// Last Modified On : 07-05-2022
// ***********************************************************************
// <copyright file="TimeSpanConverter.cs" company="JC-Soft">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM_18_MultiConverters.Model;
using System;
using System.Globalization;
using System.Windows.Data;

namespace MVVM_18_MultiConverters.ValueConverter
{
    /// <summary>
    /// Class TimeSpanConverter.
    /// Implements the <see cref="IMultiValueConverter" />
    /// </summary>
    /// <seealso cref="IMultiValueConverter" />
    public class TimeSpanConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts source values to a value for the binding target. The data binding engine calls this method when it propagates the values from source bindings to the binding target.
        /// </summary>
        /// <param name="values">The array of values that the source bindings in the <see cref="T:System.Windows.Data.MultiBinding" /> produces. The value <see cref="F:System.Windows.DependencyProperty.UnsetValue" /> indicates that the source binding has no value to provide for conversion.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value.
        /// If the method returns <see langword="null" />, the valid <see langword="null" /> value is used.
        /// A return value of <see cref="T:System.Windows.DependencyProperty" />.<see cref="F:System.Windows.DependencyProperty.UnsetValue" /> indicates that the converter did not produce a value, and that the binding will use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue" /> if it is available, or else will use the default value.
        /// A return value of <see cref="T:System.Windows.Data.Binding" />.<see cref="F:System.Windows.Data.Binding.DoNothing" /> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue" /> or the default value.</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            DateDifFormat df = DateDifFormat.Days;
            if (values?.Length >= 2 && values[1] is DateDifFormat _d)
                df = _d;

            if (values?.Length >= 1 && values[0] is TimeSpan ts)
            {
                switch (df)
                {
                    case DateDifFormat.Days:
                        return ts.TotalDays.ToString((string)parameter);
                        
                    case DateDifFormat.Hours:
                        return ts.TotalHours.ToString((string)parameter);
                        
                    case DateDifFormat.Minutes:
                        return ts.TotalMinutes.ToString((string)parameter);
                        
                    case DateDifFormat.Seconds:
                        return ts.TotalSeconds.ToString((string)parameter);
                        
                    default: return "0";
                }

            }
            else
                return "0";
        }

        /// <summary>
        /// Converts a binding target value to the source binding values.
        /// </summary>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
