// ***********************************************************************
// Assembly         : ConsoleDisplay
// Author           : Mir
// Created          : 08-19-2022
//
// Last Modified By : Mir
// Last Modified On : 08-27-2022
// ***********************************************************************
// <copyright file="TileDisplay.cs" company="ConsoleDisplay">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Drawing;

namespace ConsoleDisplay.View
{
    public interface ITileDisplay<T>
    {
        T? this[Point Idx] { get; set; }

        Point Position { get; }
        Size DispSize { get; }
        Size TileSize { get; }
//        IConsole console { get; }
        Point DispOffset { get; set; }
        Func<Point, T>? FncGetTile { get; set; }
        Func<Point, Point>? FncOldPos { get; set; }

        void FullRedraw();
        void SetDispSize(Size size);
        void Update(bool e);
        void WriteTile(PointF p, T tile);
    }
}