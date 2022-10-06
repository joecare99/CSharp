// ***********************************************************************
// Assembly         : Polyline_net
// Author           : Mir
// Created          : 06-22-2022
//
// Last Modified By : Mir
// Last Modified On : 06-22-2022
// ***********************************************************************
// <copyright file="PolylineCollection.cs" company="Polyline_net">
//     Copyright (c) HP Inc.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.Linq;


namespace Polyline.ViewModels
{
    /// <summary>
    /// Class PolylineCollection.
    /// Implements the <see cref="System.Collections.Generic.List{Polyline.ViewModels.Polyline}" />
    /// </summary>
    /// <seealso cref="System.Collections.Generic.List{Polyline.ViewModels.Polyline}" />
    public class PolylineCollection : List<Polyline>
    {
        /// <summary>
        /// Gets the segments.
        /// </summary>
        /// <value>The segments.</value>
        public IEnumerable<Segment> Segments
        {
            get
            {
                foreach (var polyline in this)
                {
                    var last = polyline.FirstOrDefault();
                    foreach (var coordinate in polyline.Skip(1))
                    {
                        yield return new Segment { Start = last ?? coordinate, End = coordinate };
                        last = coordinate;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the control points.
        /// </summary>
        /// <value>The control points.</value>
        public IEnumerable<Coordinate> ControlPoints
        {
            get
            {
                foreach (var polyline in this)
                {
                    foreach (var coordinate in polyline)
                        yield return coordinate;
                }
            }
        }
    }
}
