// ***********************************************************************
// Assembly         : ConsoleDisplay
// Author           : Mir
// Created          : 08-19-2022
//
// Last Modified By : Mir
// Last Modified On : 08-27-2022
// ***********************************************************************
// <copyright file="TileDisplay.cs" company="ConsoleDisplay">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Drawing;

namespace ConsoleDisplay.View {
	/// <summary>
	/// Class TileDef.
	/// </summary>
	/// <typeparam name="Enum">The type of the enum.</typeparam>
	public abstract class TileDefBase
    {
        /// <summary>
        /// Gets the tile definition.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>The visual defintion of the tile</returns>
        public abstract (string[] lines, (ConsoleColor fgr,ConsoleColor bgr)[] colors) GetTileDef(Enum? tile);

        /// <summary>
        /// Converts a Byte to 2  Console-colors (fore- and background).
        /// </summary>
        /// <param name="colDef">The (Console-)color definition.</param>
        /// <returns>The Tuple of Forground- and background-color</returns>
        protected static (ConsoleColor fgr, ConsoleColor bgr) ByteTo2ConsColor(byte colDef) => ((ConsoleColor)(colDef & 0xf), (ConsoleColor)(colDef >> 4));

        /// <summary>
        /// Gets the array element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">The array.</param>
        /// <param name="tile">The tile.</param>
        /// <returns>T.</returns>
        protected static T GetArrayElement<T>(T[] array, Enum tile) => Tile2Int(tile) < array.Length ? array[Tile2Int(tile)] : array[array.Length - 1];

        /// <summary>
        /// Tile2s the int.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>System.Int32.</returns>
        protected static int Tile2Int(Enum tile) { return ((int)((object)tile ?? 0)); }

        public Size TileSize { get; protected set; }
    }
}
