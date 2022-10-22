// ***********************************************************************
// Assembly         : MVVM_21_Buttons
// Author           : Mir
// Created          : 08-12-2022
//
// Last Modified By : Mir
// Last Modified On : 08-14-2022
// ***********************************************************************
// <copyright file="BoolToColorConverter.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MVVM_21_Buttons.Converter
{
    /// <summary>
    /// Class BoolToColorStringConverter.
    /// Implements the <see cref="IValueConverter" />
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public class BoolToColorStringConverter : IValueConverter
    {
        /// <summary>
        /// The true color
        /// </summary>
        public static Color TrueColor = Colors.Green;
        /// <summary>
        /// The false color
        /// </summary>
        public static Color FalseColor = Colors.DarkRed;

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
#if NET5_0_OR_GREATER
            return value switch
            {
                bool b when parameter is string s && s.StartsWith("#") => ((string)parameter).Split(':')[b ? 1 : 0],
                bool b => (b ? TrueColor : FalseColor).ToString(),
                bool[] ba when parameter is string s && int.TryParse(s,out _)=> (ba[int.Parse(s)] ? TrueColor : FalseColor).ToString(),
                null => "",
                () => "",
                _ => throw new NotImplementedException()
            };
#else
            switch (value)
            {
                case bool b when parameter is string s && s.StartsWith("#"): return ((string)parameter).Split(':')[b ? 1 : 0];
       
                case bool b: return (b ? TrueColor : FalseColor).ToString();
                case bool[] ba when parameter is string s && int.TryParse(s, out _): return (ba[int.Parse(s)] ? TrueColor : FalseColor).ToString();
                default:
                    return "";
            }
#endif
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
}
