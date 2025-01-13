// ***********************************************************************
// Assembly         : Snake_Base
// Author           : Mir
// Created          : 08-25-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="SnakeGame.cs" company="JC-Soft">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Game_Base.Model;
using Snake_Base.Models.Data;
using System;
using System.Drawing;

namespace Snake_Base.Models.Interfaces;

public interface ISnakeGame
{
    IPlayfield2D<ISnakeGameObject> Playfield { get; }
    int Level { get; }
    bool IsRunning { get; }
    int Score { get; }
    int Lives { get; }
    int MaxLives { get; }
    Size size { get; }
    bool UserQuit { set; }

    SnakeTiles this[Point p] { get; }

    event EventHandler<bool>? visUpdate;
    event EventHandler<EventArgs?>? visFullRedraw;

    Point GetOldPos(Point p);
    void SetSnakeDir(Direction action);
    int GameStep();
    void Setup(int v);
}