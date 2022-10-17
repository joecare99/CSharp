// ***********************************************************************
// Assembly         : PlotgraphWPF
// Author           : Mir
// Created          : 07-01-2022
//
// Last Modified By : Mir
// Last Modified On : 07-01-2022
// ***********************************************************************
// <copyright file="MyModel.cs" company="PlotgraphWPF">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Windows.Media;

namespace PlotgraphWPF.Model
{
    /// <summary>
    /// Class MyModel.
    /// </summary>
    public class MyModel
    {
        /// <summary>
        /// Gets or sets the points.
        /// </summary>
        /// <value>The points.</value>
        public PointCollection points { get; set; } = new PointCollection();
        /// <summary>
        /// Gets or sets the color of the plot.
        /// </summary>
        /// <value>The color of the plot.</value>
        public Color PlotColor { get; set; }
    }
}
