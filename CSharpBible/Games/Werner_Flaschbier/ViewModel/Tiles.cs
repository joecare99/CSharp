// ***********************************************************************
// Assembly         : Werner_Flaschbier_Base
// Author           : Mir
// Created          : 08-02-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Tiles.cs" company="Werner_Flaschbier_Base">
//     Copyright (c) HP Inc.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Werner_Flaschbier_Base.Model;

namespace Werner_Flaschbier_Base.ViewModel
{
    /// <summary>
    /// Enum Tiles
    /// </summary>
    public enum Tiles
    {
        /// <summary>
        /// The empty
        /// </summary>
        Empty = FieldDef.Empty,
        /// <summary>
        /// The dirt
        /// </summary>
        Dirt = FieldDef.Dirt,
        /// <summary>
        /// The wall
        /// </summary>
        Wall = FieldDef.Wall,
        /// <summary>
        /// The destination
        /// </summary>
        Destination = FieldDef.Destination,
        /// <summary>
        /// The player
        /// </summary>
        Player = FieldDef.Player,
        /// <summary>
        /// The stone
        /// </summary>
        Stone = FieldDef.Stone,
        /// <summary>
        /// The enemy up
        /// </summary>
        Enemy_Up = FieldDef.Enemy,
        /// <summary>
        /// The enemy right
        /// </summary>
        Enemy_Right = 7,
        /// <summary>
        /// The enemy DWN
        /// </summary>
        Enemy_Dwn = 8,
        /// <summary>
        /// The enemy left
        /// </summary>
        Enemy_Left = 9,
        /// <summary>
        /// The stone moving
        /// </summary>
        StoneMoving = 10,
        /// <summary>
        /// The player dead
        /// </summary>
        PlayerDead = 11,
        /// <summary>
        /// The dummy
        /// </summary>
        Dummy = 12,
        /// <summary>
        /// The wall u
        /// </summary>
        Wall_U,
        /// <summary>
        /// The wall w
        /// </summary>
        Wall_W,
        /// <summary>
        /// The wall uw
        /// </summary>
        Wall_UW,
        /// <summary>
        /// The wall d
        /// </summary>
        Wall_D,
        /// <summary>
        /// The wall ud
        /// </summary>
        Wall_UD,
        /// <summary>
        /// The wall wd
        /// </summary>
        Wall_WD,
        /// <summary>
        /// The wall uwd
        /// </summary>
        Wall_UWD,
        /// <summary>
        /// The wall e
        /// </summary>
        Wall_E,
        /// <summary>
        /// The wall ue
        /// </summary>
        Wall_UE,
        /// <summary>
        /// The wall we
        /// </summary>
        Wall_WE,
        /// <summary>
        /// The wall uwe
        /// </summary>
        Wall_UWE,
        /// <summary>
        /// The wall de
        /// </summary>
        Wall_DE,
        /// <summary>
        /// The wall ude
        /// </summary>
        Wall_UDE,
        /// <summary>
        /// The wall wde
        /// </summary>
        Wall_WDE,
        /// <summary>
        /// The wall uwde
        /// </summary>
        Wall_UWDE
    };

}
