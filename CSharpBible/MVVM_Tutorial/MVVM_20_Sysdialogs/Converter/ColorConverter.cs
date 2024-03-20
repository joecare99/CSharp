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
using System.Windows.Data;
using System.Windows.Media;

namespace MVVM_20_Sysdialogs.Converter
{
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
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color c)
                return c.ToString();
            if (value is System.Drawing.Color cl)
            {
                Color cc = default;
                cc.A = cl.A;
                cc.B = cl.B; 
                cc.R = cl.R;
                cc.G = cl.G;
                return cc.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns <see langword="null" />, the valid null value is used.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
