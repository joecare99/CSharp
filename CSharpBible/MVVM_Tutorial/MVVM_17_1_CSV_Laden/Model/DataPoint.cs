// ***********************************************************************
// Assembly         : MVVM_17_1_CSV_Laden
// Author           : Mir
// Created          : 07-03-2022
//
// Last Modified By : Mir
// Last Modified On : 07-04-2022
// ***********************************************************************
// <copyright file="DataPoint.cs" company="MVVM_17_1_CSV_Laden">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Globalization;

namespace MVVM_17_1_CSV_Laden.Model
{
    /// <summary>
    /// Class DataPoint.
    /// </summary>
    public class DataPoint
    {
        /// <summary>
        /// Gets or sets the time stamp.
        /// </summary>
        /// <value>The time stamp.</value>
        public string TimeStamp { get; set; }
        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        /// <value>The x.</value>
        public double X { get; set; }
        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        /// <value>The y.</value>
        public double Y { get; set; }
        /// <summary>
        /// Gets the dt.
        /// </summary>
        /// <value>The dt.</value>
        public DateTime dt => DateTime.TryParse(TimeStamp, CultureInfo.CurrentUICulture.DateTimeFormat, DateTimeStyles.AssumeLocal, out DateTime d) ? d : DateTime.MinValue;
    }
}
