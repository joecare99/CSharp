// ***********************************************************************
// Assembly         : Werner_Flaschbier_Base
// Author           : Mir
// Created          : 08-06-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="UserAction.cs" company="Werner_Flaschbier_Base">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Snake_Base.Model;

namespace Snake_Base.ViewModel
{

    /// <summary>
    /// Enum UserAction
    /// </summary>
    public enum UserAction
    {
        /// <summary>
        /// The go up
        /// </summary>
        GoNorth = Direction.North,
        /// <summary>
        /// The go west
        /// </summary>
        GoWest = Direction.West,
        /// <summary>
        /// The go down
        /// </summary>
        GoSouth = Direction.South,
        /// <summary>
        /// The go east
        /// </summary>
        GoEast = Direction.East,
        /// <summary>
        /// The quit
        /// </summary>
        Quit,
        /// <summary>
        /// The help
        /// </summary>
        Help,
        /// <summary>
        /// The restart
        /// </summary>
        Restart,
        /// <summary>
        /// The nop
        /// </summary>
        Nop
    }

    /// <summary>
    /// Enum GameSound
    /// </summary>
    public enum GameSound
    {
        /// <summary>
        /// The no sound
        /// </summary>
        NoSound,
        /// <summary>
        /// The deep boom
        /// </summary>
        DeepBoom,
        /// <summary>
        /// The tick
        /// </summary>
        Tick
    }
}
