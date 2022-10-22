// ***********************************************************************
// Assembly         : MVVM_22_WpfCap
// Author           : Mir
// Created          : 08-14-2022
//
// Last Modified By : Mir
// Last Modified On : 08-14-2022
// ***********************************************************************
// <copyright file="EnumToColorConverter.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MVVM_22_WpfCap.Converter
{
    /// <summary>
    /// Class EnumToColorStringConverter.
    /// Implements the <see cref="IValueConverter" />
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public class EnumToColorStringConverter : IValueConverter
    {

        public List<Color> colors { get; set; } = new List<Color> { };
        /// <summary>
        /// The zero color
        /// </summary>
        public static Color ZeroColor = Colors.Black;
        /// <summary>
        /// The true color
        /// </summary>
        public static Color TrueColor = Colors.Green;
        /// <summary>
        /// The false color
        /// </summary>
        public static Color FalseColor = Colors.Red;
        /// <summary>
        /// The three color
        /// </summary>
        public static Color ThreeColor = Colors.Yellow;
        /// <summary>
        /// The four color
        /// </summary>
        public static Color FourColor = Colors.Blue;

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
                bool b when colors.Count>2 => (b ? colors[2] : colors[1]).ToString(),
                bool b => (b ? TrueColor : FalseColor).ToString(),
                bool[] ba when parameter is string s && int.TryParse(s,out _) &&  colors.Count>2=> (ba[int.Parse(s)] ? colors[2] : colors[1]).ToString(),
                bool[] ba when parameter is string s && int.TryParse(s,out _)=> (ba[int.Parse(s)] ? TrueColor : FalseColor).ToString(),
                int[] ia when parameter is string s && int.TryParse(s, out int _is) && _is <= ia.Length && colors.Count > ia[_is] 
                    =>colors[ia[_is]],
                int[] ia when parameter is string s && int.TryParse(s, out int _is) && _is <= ia.Length 
                    =>(ia[_is] switch
                    {
                        1 => FalseColor,
                        2 => TrueColor,
                        3 => ThreeColor,
                        4 => FourColor,
                        _ => ZeroColor
                    }).ToString(),
                null => "",
                () => "",
                _ => throw new NotImplementedException()
            };
#else
            switch (value)
            {
                case bool b when parameter is string s && s.StartsWith("#"): 
                    return ((string)parameter).Split(':')[b ? 1 : 0];
                case bool b when colors.Count>2: 
                    return (b ? colors[2] : colors[1]).ToString();
                case bool b: 
                    return (b ? TrueColor : FalseColor).ToString();
                case bool[] ba when parameter is string s && int.TryParse(s, out _) && colors.Count > 2: 
                    return (ba[int.Parse(s)] ? colors[2] : colors[1]).ToString();
                case bool[] ba when parameter is string s && int.TryParse(s, out _): 
                    return (ba[int.Parse(s)] ? TrueColor : FalseColor).ToString();
                case int[] ia when parameter is string s && int.TryParse(s, out int _is) && _is <= ia.Length && colors.Count > ia[_is]:
                    return colors[ia[_is]];
                case int[] ia when parameter is string s && int.TryParse(s, out int _is) && _is <= ia.Length:
                    switch (ia[_is]) {
                        case 1: return FalseColor.ToString();
                        case 2: return TrueColor.ToString();
                        case 3: return ThreeColor.ToString();
                        case 4: return FourColor.ToString();
                        default: return ZeroColor.ToString(); };
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
