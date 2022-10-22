// ***********************************************************************
// Assembly         : DataGridEx
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 04-10-2020
// ***********************************************************************
// <copyright file="EmailConverter.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Data;

namespace DataGridExWPF.Worker
{
    //Converts the mailto uri to a string with just the customer alias
    /// <summary>
    /// Class EmailConverter.
    /// </summary>
    public class EmailConverter 

    {
        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>System.Object.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                string email = value.ToString();
                int index = email.IndexOf("@");
                string alias = email.Substring(7, index - 7);
                return alias;
            }
            else
            {
                string email = "";
                return email;
            }
        }

        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>System.Object.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Uri email = new Uri((string)value);
            return email;
        }
    }
}
