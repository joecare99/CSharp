// ***********************************************************************
// Assembly         : Snake_Base
// Author           : Mir
// Created          : 08-24-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Playfield2D.cs" company="JC-Soft">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Game_Base.Model;

public interface IPlayfield2D<T> where T : class
{
    T? this[Point p] { get;set; }

    Size PfSize { get; set; }
    Rectangle Rect { get; }
    IEnumerable<T> Items { get; }

    event EventHandler<(string prop, object? oldVal, object? newVal)>? OnDataChanged;

    bool IsInside(Point P);
}