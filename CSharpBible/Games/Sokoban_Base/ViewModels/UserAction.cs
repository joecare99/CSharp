// ***********************************************************************
// Assembly         : Sokoban_Base
// Author           : Mir
// Created          : 07-24-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="UserAction.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using Sokoban_Base.Model;

namespace Sokoban_Base.ViewModel
{

    /// <summary>
    /// Enum UserAction
    /// </summary>
    public enum UserAction
    {
        /// <summary>
        /// The go north
        /// </summary>
        GoNorth = Direction.North,
        /// <summary>
        /// The none
        /// </summary>
        GoWest = Direction.West,
        /// <summary>
        /// The move left
        /// </summary>
        GoSouth = Direction.South,
        /// <summary>
        /// The move right
        /// </summary>
        GoEast = Direction.East,
        /// <summary>
        /// The move down
        /// </summary>
        Quit,
        /// <summary>
        /// The rotate left
        /// </summary>
        Help,
        /// <summary>
        /// The rotate right
        /// </summary>
        Restart
    }
}
