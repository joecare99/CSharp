// ***********************************************************************
// Assembly         : PolySpline_net
// Author           : Mir
// Created          : 06-22-2022
//
// Last Modified By : Mir
// Last Modified On : 08-20-2022
// ***********************************************************************
// <copyright file="Coordinate.cs" company="PolySpline_net">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM.ViewModel;
using System.Windows;


namespace PolySpline.ViewModels
{
    /// <summary>
    /// Class Coordinate.
    /// Implements the <see cref="NotificationObject" />
    /// </summary>
    /// <seealso cref="NotificationObject" />
    public class Coordinate : NotificationObject
    {
        /// <summary>
        /// The x
        /// </summary>
        private double x;
        /// <summary>
        /// The y
        /// </summary>
        private double y;

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        /// <value>The x.</value>
        public double X { get => x; set => SetProperty(ref x, value, (o, n) => RaisePropertyChanged(nameof(Point))); }
        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        /// <value>The y.</value>
        public double Y { get => y; set => SetProperty(ref y, value, (o, n) => RaisePropertyChanged(nameof(Point))); }
        /// <summary>
        /// Gets or sets the point.
        /// </summary>
        /// <value>The point.</value>
        public Point Point { get => new Point(x, y); set { (X, Y) = (value.X, value.Y); }
        }
    }
}
