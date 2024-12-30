// ***********************************************************************
// Assembly         : Calc64WF
// Author           : Mir
// Created          : 08-31-2022
//
// Last Modified By : Mir
// Last Modified On : 08-31-2022
// ***********************************************************************
// <copyright file="OperationModeToShortString.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using Calc64Base;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Calc64WF.Visual.Converter
{
    /// <summary>
    /// Class OperationModeToShortString.
    /// Implements the <see cref="IValueConverter" />
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public class OperationModeToShortString : IValueConverter
    {
        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>System.Object.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value switch
        {
            Calc64Model.EOpMode opMode when targetType == typeof(string) 
              => Properties.Resources.ResourceManager.GetString($"OPMode_{opMode}", Properties.Resources.Culture) ?? "",
            _ => "",
        };

        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
