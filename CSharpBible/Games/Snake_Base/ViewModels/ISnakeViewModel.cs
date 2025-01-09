// ***********************************************************************
// Assembly         : Snake_Console
// Author           : Mir
// Created          : 08-02-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Visual.cs" company="JC-Soft">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Snake_Base.Models.Data;
using System;
using System.ComponentModel;
using System.Drawing;

namespace Snake_Base.ViewModels;

public interface ISnakeViewModel: INotifyPropertyChanged
{
    public interface ITileProxy<T> 
    {
        public T this[Point p] { get; }
    }

    UserAction UserAction { get; set; }
    ITileProxy<SnakeTiles> Tiles { get; }

    Func<Point,Point> GetOldPos { get; }
    Size size { get; }
    int Level { get; }
    int Score { get; }
    int Lives { get; }
    int MaxLives { get; }
    bool HalfStep { get; }
}