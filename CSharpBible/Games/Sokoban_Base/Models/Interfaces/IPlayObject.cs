// ***********************************************************************
// Assembly         : Sokoban
// Author           : Mir
// Created          : 07-09-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="PlayObject.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Drawing;

namespace Sokoban.Models.Interfaces
{
    public interface IPlayObject
    {
        IField? field { get; set; }
        Point OldPosition { get; set; }
        Point Position { get; set; }

        bool TestMove(Direction dir);
        bool TryMove(Direction dir);
    }
}