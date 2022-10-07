// ***********************************************************************
// Assembly         : Polyline_net
// Author           : Mir
// Created          : 06-20-2022
//
// Last Modified By : Mir
// Last Modified On : 06-22-2022
// ***********************************************************************
// <copyright file="MainWindowViewModel.cs" company="Polyline_net">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace Polyline.ViewModels
{
    /// <summary>
    /// Class MainWindowViewModel.
    /// </summary>
    public class MainWindowViewModel
    {
        /// <summary>
        /// Gets or sets the pl collection.
        /// </summary>
        /// <value>The pl collection.</value>
        public PolylineCollection plCollection { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            plCollection = new PolylineCollection();
            var p = new Polyline();
            p.Add(new Coordinate() { X = 100, Y = 100 });
            p.Add(new Coordinate() { X = 200, Y = 150 });
            p.Add(new Coordinate() { X = 250, Y = 200 });
            p.Add(new Coordinate() { X = 150, Y = 250 });
            p.Add(new Coordinate() { X = 250, Y = 300 });
            plCollection.Add(p);
        }   
    }
}
