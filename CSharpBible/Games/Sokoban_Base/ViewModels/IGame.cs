// ***********************************************************************
// Assembly         : Sokoban_Base
// Author           : Mir
// Created          : 08-04-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Game.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using Sokoban.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Sokoban_Base.ViewModels;

public interface IGame
{
    bool GameSolved { get; }
    int level { get; }
    Size PFSize { get; }
    IPlayer? player { get; }
    IEnumerable<IPlayObject> Stones { get; }
    int StonesInDest { get; }
    Action<UserAction?>? visShow { get; set; }
    Action? visUpdate { get; set; }
    Func<UserAction?, UserAction?>? visGetUserAction { get; set; }
    Action<string>? visSetMessage { get; set; }

    void Cleanup();
    Point GetOldPos(Point p);
    TileDef GetTile(Point p);
    void Init();
    UserAction? Run();
}