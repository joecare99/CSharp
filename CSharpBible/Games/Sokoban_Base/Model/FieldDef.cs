// ***********************************************************************
// Assembly         : Sokoban_Base
// Author           : Mir
// Created          : 07-09-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="FieldDef.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;

namespace Sokoban_Base.Model
{
    /// <summary>
    /// Enum FieldDef
    /// </summary>
    public enum FieldDef
    {
        /// <summary>
        /// The empty
        /// </summary>
        Empty, //outside
        /// <summary>
        /// The wall
        /// </summary>
        Wall,  //unmovable Wall
        /// <summary>
        /// The floor
        /// </summary>
        Floor, //Empty hallway
        /// <summary>
        /// The destination
        /// </summary>
        Destination, // a Destination for a Stone
        /// <summary>
        /// The player
        /// </summary>
        Player, // the Player (on a floor-field)
        /// <summary>
        /// The player over dest
        /// </summary>
        PlayerOverDest, // the Player (on a destination-field)
        /// <summary>
        /// The stone
        /// </summary>
        Stone, //  a Stone (on a floor-field)
        /// <summary>
        /// The stone in dest
        /// </summary>
        StoneInDest  //  a Stone (on a Destination-field)
    }

    /// <summary>
    /// Class FieldDefs.
    /// </summary>
    public static class FieldDefs
    {
        /// <summary>
        /// The s definition
        /// </summary>
        public static readonly Dictionary<char, FieldDef> SDef = new() { 
            { ' ', FieldDef.Floor }, 
            { '#', FieldDef.Wall }, 
            { '$', FieldDef.Stone },
            { '*', FieldDef.StoneInDest },
            { '@', FieldDef.Player }, 
            { '.', FieldDef.Destination } };
    }
}
