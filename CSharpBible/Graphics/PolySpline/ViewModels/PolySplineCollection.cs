// ***********************************************************************
// Assembly         : PolySpline_net
// Author           : Mir
// Created          : 06-22-2022
//
// Last Modified By : Mir
// Last Modified On : 06-22-2022
// ***********************************************************************
// <copyright file="PolySplineCollection.cs" company="PolySpline_net">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.Linq;


namespace PolySpline.ViewModels
{
    /// <summary>
    /// Class PolySplineCollection.
    /// Implements the <see cref="System.Collections.Generic.List{PolySpline.ViewModels.PolySpline}" />
    /// </summary>
    /// <seealso cref="System.Collections.Generic.List{PolySpline.ViewModels.PolySpline}" />
    public class PolySplineCollection : List<PolySpline>
    {
        /// <summary>
        /// Gets the segments.
        /// </summary>
        /// <value>The segments.</value>
        public IEnumerable<Segment> Segments
        {
            get
            {
                foreach (var PolySpline in this)
                {
                    var last = PolySpline.FirstOrDefault();
                    bool flag = false;
                    Coordinate middle = PolySpline.FirstOrDefault();
                    foreach (var coordinate in PolySpline.Skip(1))
                    {
                        if (flag = !flag)
                            middle = coordinate;
                        else
                        {
                            yield return new Segment { Start = last ?? coordinate, Middle = middle, End = coordinate };
                            last = coordinate;
                        }
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
                foreach (var PolySpline in this)
                {
                    foreach (var coordinate in PolySpline)
                        yield return coordinate;
                }
            }
        }
    }
}
