// ***********************************************************************
// Assembly         : DataGridExWPF
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
using System.Windows.Data;

namespace DataGridExWPF.Worker
{
    //Converts the mailto uri to a string with just the customer alias
    /// <summary>
    /// Class EmailConverter.
    /// Implements the <see cref="IValueConverter" />
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public class EmailConverter : IValueConverter
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
        /// Konvertiert einen Wert.
        /// </summary>
        /// <param name="value">Der Wert, der vom Bindungsziel erzeugt wird.</param>
        /// <param name="targetType">Der Typ, in den konvertiert werden soll.</param>
        /// <param name="parameter">Der zu verwendende Konverterparameter.</param>
        /// <param name="culture">Die im Konverter zu verwendende Kultur.</param>
        /// <returns>Ein konvertierter Wert.
        /// Wenn die Methode <see langword="null" /> zurückgibt, wird der gültige NULL-Wert verwendet.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Uri email = new Uri((string)value);
            return email;
        }
    }
}
