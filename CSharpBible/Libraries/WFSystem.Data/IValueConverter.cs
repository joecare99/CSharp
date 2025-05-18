// ***********************************************************************
// Assembly         : WFSystem.Windows.Data
// Author           : Mir
// Created          : 08-31-2022
//
// Last Modified By : Mir
// Last Modified On : 08-31-2022
// ***********************************************************************
// <copyright file="IValueConverter.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Globalization;

namespace System.Windows.Data
{
    #if !NET8_0_OR_GREATER
    //
    // Zusammenfassung:
    //     Provides a way to apply custom logic to a binding.
    /// <summary>
    /// Interface IValueConverter
    /// </summary>
    public interface IValueConverter
    {
        //
        // Zusammenfassung:
        //     Converts a value.
        //
        // Parameter:
        //   value:
        //     The value produced by the binding source.
        //
        //   targetType:
        //     The type of the binding target property.
        //
        //   parameter:
        //     The converter parameter to use.
        //
        //   culture:
        //     The culture to use in the converter.
        //
        // Rückgabewerte:
        //     A converted value. If the method returns null, the valid null value is used.
        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>System.Object.</returns>
        object Convert(object value, Type targetType, object parameter, CultureInfo culture);
        //
        // Zusammenfassung:
        //     Converts a value.
        //
        // Parameter:
        //   value:
        //     The value that is produced by the binding target.
        //
        //   targetType:
        //     The type to convert to.
        //
        //   parameter:
        //     The converter parameter to use.
        //
        //   culture:
        //     The culture to use in the converter.
        //
        // Rückgabewerte:
        //     A converted value. If the method returns null, the valid null value is used.
        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>System.Object.</returns>
        object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
    }
#endif
}