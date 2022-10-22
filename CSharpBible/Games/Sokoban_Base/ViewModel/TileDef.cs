// ***********************************************************************
// Assembly         : Sokoban_Base
// Author           : Mir
// Created          : 08-04-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="TileDef.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using Sokoban_Base.Model;

namespace Sokoban_Base.ViewModel
{
    /// <summary>
    /// Enum TileDef
    /// </summary>
    public enum TileDef
    {
        /// <summary>
        /// The empty
        /// </summary>
        Empty = FieldDef.Empty,
        /// <summary>
        /// The wall
        /// </summary>
        Wall = FieldDef.Wall,
        /// <summary>
        /// The floor
        /// </summary>
        Floor = FieldDef.Floor,
        /// <summary>
        /// The destination
        /// </summary>
        Destination = FieldDef.Destination,
        /// <summary>
        /// The player
        /// </summary>
        Player = FieldDef.Player,
        /// <summary>
        /// The player over dest
        /// </summary>
        PlayerOverDest = FieldDef.PlayerOverDest,
        /// <summary>
        /// The stone
        /// </summary>
        Stone = FieldDef.Stone,
        /// <summary>
        /// The stone in dest
        /// </summary>
        StoneInDest = FieldDef.StoneInDest,
        /// <summary>
        /// The floor marked
        /// </summary>
        Floor_Marked,
        /// <summary>
        /// The wall n
        /// </summary>
        Wall_N,
        /// <summary>
        /// The wall w
        /// </summary>
        Wall_W,
        /// <summary>
        /// The wall nw
        /// </summary>
        Wall_NW,
        /// <summary>
        /// The wall s
        /// </summary>
        Wall_S,
        /// <summary>
        /// The wall ns
        /// </summary>
        Wall_NS,
        /// <summary>
        /// The wall ws
        /// </summary>
        Wall_WS,
        /// <summary>
        /// The wall NWS
        /// </summary>
        Wall_NWS,
        /// <summary>
        /// The wall e
        /// </summary>
        Wall_E,
        /// <summary>
        /// The wall ne
        /// </summary>
        Wall_NE,
        /// <summary>
        /// The wall we
        /// </summary>
        Wall_WE,
        /// <summary>
        /// The wall nwe
        /// </summary>
        Wall_NWE,
        /// <summary>
        /// The wall se
        /// </summary>
        Wall_SE,
        /// <summary>
        /// The wall nse
        /// </summary>
        Wall_NSE,
        /// <summary>
        /// The wall wse
        /// </summary>
        Wall_WSE,
        /// <summary>
        /// The wall nwse
        /// </summary>
        Wall_NWSE,
        /// <summary>
        /// The player w
        /// </summary>
        Player_W,
        /// <summary>
        /// The player over dest w
        /// </summary>
        PlayerOverDest_W,
        /// <summary>
        /// The player s
        /// </summary>
        Player_S,
        /// <summary>
        /// The player over dest s
        /// </summary>
        PlayerOverDest_S,
        /// <summary>
        /// The player e
        /// </summary>
        Player_E,
        /// <summary>
        /// The player over dest e
        /// </summary>
        PlayerOverDest_E

    };
}
