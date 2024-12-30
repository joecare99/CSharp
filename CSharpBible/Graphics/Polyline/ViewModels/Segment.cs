// ***********************************************************************
// Assembly         : Polyline_net
// Author           : Mir
// Created          : 06-22-2022
//
// Last Modified By : Mir
// Last Modified On : 08-23-2022
// ***********************************************************************
// <copyright file="Segment.cs" company="Polyline_net">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace Polyline.ViewModels
{
    /// <summary>
    /// Class Segment.
    /// </summary>
    public class Segment
    {
        /// <summary>
        /// Gets or sets the start.
        /// </summary>
        /// <value>The start.</value>
        public Coordinate? Start { get; set; }
        /// <summary>
        /// Gets or sets the end.
        /// </summary>
        /// <value>The end.</value>
        public Coordinate? End { get; set; }
    }
}
