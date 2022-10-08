// ***********************************************************************
// Assembly         : JCAMS
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 09-22-2022
// ***********************************************************************
// <copyright file="BoolClassConverter.cs" company="JC-Soft">
//     Copyright © JC-Soft 2008-2015
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.ComponentModel;
using System.Globalization;

namespace JCAMS.Core.Converter
{
    /// <summary>
    /// Class BoolClassConverter.
    /// Implements the <see cref="BooleanConverter" />
    /// </summary>
    /// <seealso cref="BooleanConverter" />
    public class BoolClassConverter : BooleanConverter
    {
        private const string cJa = "Ja";
        private const string cNein = "Nein";

        /// <summary>
        /// Converts to.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="value">The value.</param>
        /// <param name="destType">Type of the dest.</param>
        /// <returns>System.Object.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
        {
            return (bool)value ? cJa : cNein;
        }

        /// <summary>
        /// Converts from.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="value">The value.</param>
        /// <returns>System.Object.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return (string)value == cJa;
        }
    }
}
