// ***********************************************************************
// Assembly         : Werner_Flaschbier_Base
// Author           : Mir
// Created          : 07-31-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="FieldDef.cs" company="Werner_Flaschbier_Base">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;

namespace Werner_Flaschbier_Base.Model
{
    /// <summary>
    /// Enum FieldDef
    /// </summary>
    public enum FieldDef
    {
        /// <summary>
        /// The empty
        /// </summary>
        Empty, //Empty Space
        /// <summary>
        /// The dirt
        /// </summary>
        Dirt, //Dirt can be digged, stops Stones & Enemies
        /// <summary>
        /// The wall
        /// </summary>
        Wall,  //unmovable Wall
        /// <summary>
        /// The destination
        /// </summary>
        Destination, // a Destination for the PLayer
        /// <summary>
        /// The player
        /// </summary>
        Player, // the Player (on a floor-field)
        /// <summary>
        /// The stone
        /// </summary>
        Stone, //  a Stone (on an Empty-field)
        /// <summary>
        /// The enemy
        /// </summary>
        Enemy  //  an Enemy (on an Empty-field)
    }

    /// <summary>
    /// Class FieldDefs.
    /// </summary>
    public static class FieldDefs
    {
        /// <summary>
        /// The s definition
        /// </summary>
        public static Dictionary<char, FieldDef> SDef = new()
        { { ' ', FieldDef.Empty }, 
            { '#', FieldDef.Wall }, 
            { 'O', FieldDef.Stone }, 
            { 'w', FieldDef.Player },
            { 'g', FieldDef.Enemy },
            { '=', FieldDef.Dirt },
            { 'i', FieldDef.Destination } };
    }
}
