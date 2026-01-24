// ***********************************************************************
// Assembly         : Werner_Flaschbier_Base
// Author           : Mir
// Created          : 08-02-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="VTileDef.cs" company="Werner_Flaschbier_Base">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Drawing;
using ConsoleDisplay.View;

namespace Werner_Flaschbier.Console2.View
{

	/// <summary>
	/// Class VTileDef.
	/// </summary>
	public class VTileDef : TileDefBase, ITileDef {
		/// <summary>
		/// The v tile definition string
		/// </summary>
		private string[][] _vTileDefStr = [
			["    ", "    "],
			["=-=-", "-=-="],
			["─┴┬─", "─┬┴─"],
			[" ╓╖ ", "▓░▒▓"],
			["⌐°@)", " ⌡⌡‼"],
			[@"/¯¯\", @"\__/"],
			["]°°[", "_!!_"],
			["◄°@[",@"_!!\"],
			["]oo[", "_!!_"],
			["]@°►", "/!!_"],
			[@"/╨╨\", @"\__/"],
			[" +*∩", "╘═◊@"],
			["    ", "    "],
			["─┴┬┴", "─┬┴─"],
			["┬┴┬─", "┴┬┴─"],
			["┬┴┬┴", "┴┬┴─"],

			["┬┴┬─", "┴┬┴┬"],
			["┬┴┬┴", "┴┬┴┬"],
			["┬┴┬─", "┴┬┴┬"],
			["┬┴┬┴", "┴┬┴┬"],

			["─┴┬─", "─┬┴─"],
			["─┴┬┴", "─┬┴─"],
			["┬┴┬─", "┴┬┴─"],
			["┬┴┬┴", "┴┬┴─"],

			["┬┴┬─", "┴┬┴┬"],
			["┬┴┬┴", "┴┬┴┬"],
			["┬┴┬─", "┴┬┴┬"],
			["┬┴┬┴", "┴┬┴┬"]
		];

		/// <summary>
		/// The v tile colors
		/// </summary>
		private byte[][] _vTileColors =
        [
            [0x00],
			[0x6E],
			[0x4F],
			[0x0E, 0x0E, 0x0E, 0x0E, 0x2A, 0x22, 0x02, 0x22],
			[0x6F],
			[0x6E],
			[0x1A,0xA0,0xA0,0x1A,0x1A,0xA0,0xA0,0x1A],
            [0x1A,0xA0,0xA0,0x1A,0x1A,0xA0,0xA0,0x1A],
            [0x1A,0xA0,0xA0,0x1A,0x1A,0xA0,0xA0,0x1A],
            [0x1A,0xA0,0xA0,0x1A,0x1A,0xA0,0xA0,0x1A],
			[0x6E],
			[0x6F],
			[0x6E],
			[0x4F],

		];

        public new Size TileSize => new Size(4,2);

		/// <summary>
		/// Gets the tile colors.
		/// </summary>
		/// <param name="tile">The tile.</param>
		/// <returns>FullColor[].</returns>
		private (ConsoleColor fgr, ConsoleColor bgr)[] GetTileColors(Enum? tile)
		{
			var result = new (ConsoleColor fgr, ConsoleColor bgr)[8];
			byte[] colDef = GetArrayElement(_vTileColors, tile);
			for (var i =0;i<result.Length;i++)
            {
				result[i] =
					((ConsoleColor)(colDef[i % colDef.Length] & 0xf), (ConsoleColor)(colDef[i % colDef.Length] >> 4));
            }
			return result;
		}

        public override (string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors) GetTileDef(Enum? tile) 
			=> (GetArrayElement(_vTileDefStr, tile), GetTileColors((Enum?)tile));
    }
}
