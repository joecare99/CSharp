// ***********************************************************************
// Assembly         : Sokoban
// Author           : Mir
// Created          : 07-09-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Player.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using Sokoban.Models;
using System.Collections.Generic;

namespace Sokoban.Models.Interfaces
{
    public interface IPlayer: IPlayObject
    {
        Direction? LastDir { get; set; }

        bool Go(Direction? aDir);
        IEnumerable<Direction> MoveableDirs();
    }
}