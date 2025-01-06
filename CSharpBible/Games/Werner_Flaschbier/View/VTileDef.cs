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
using Werner_Flaschbier_Base.Model;

namespace Werner_Flaschbier_Base.View
{
    /// <summary>
    /// Struct FullColor
    /// </summary>
    public struct FullColor
	{
		/// <summary>
		/// The fore ground
		/// </summary>
		public ConsoleColor foreGround, backGround;
	}

	/// <summary>
	/// Class VTileDef.
	/// </summary>
	public static class VTileDef {
		/// <summary>
		/// The v tile definition string
		/// </summary>
		private static string[][] _vTileDefStr = new string[][]{
			new string[]{"    ", "    " },
			new string[]{"=-=-", "-=-=" },
			new string[]{ "─┴┬─", "─┬┴─"},
			new string[]{ " ╓╖ ", "▓░▒▓" },
			new string[]{ "⌐°@)", " ⌡⌡‼" },
			new string[]{ @"/¯¯\", @"\__/" },
			new string[]{ "]°°[", "_!!_" },
			new string[]{ "◄°@[",@"_!!\" },
			new string[]{ "]oo[", "_!!_" },
			new string[]{ "]@°►", "/!!_" },
			new string[]{ @"/╨╨\", @"\__/" },
			new string[]{ " +*∩", "╘═◊@" },
			new string[]{ "    ", "    "},
			new string[]{ "─┴┬┴", "─┬┴─"},
			new string[]{ "┬┴┬─", "┴┬┴─"},
			new string[]{ "┬┴┬┴", "┴┬┴─"},

			new string[]{ "┬┴┬─", "┴┬┴┬"},
			new string[]{ "┬┴┬┴", "┴┬┴┬"},
			new string[]{ "┬┴┬─", "┴┬┴┬"},
			new string[]{ "┬┴┬┴", "┴┬┴┬"},

			new string[]{ "─┴┬─", "─┬┴─"},
			new string[]{ "─┴┬┴", "─┬┴─"},
			new string[]{ "┬┴┬─", "┴┬┴─"},
			new string[]{ "┬┴┬┴", "┴┬┴─"},

			new string[]{ "┬┴┬─", "┴┬┴┬"},
			new string[]{ "┬┴┬┴", "┴┬┴┬"},
			new string[]{ "┬┴┬─", "┴┬┴┬"},
			new string[]{ "┬┴┬┴", "┴┬┴┬"}
		};

		/// <summary>
		/// The v tile colors
		/// </summary>
		private static byte[][] _vTileColors = new byte[][]
		{
			new byte [] {0x00 },
			new byte [] {0x6E },
			new byte [] { 0x4F },
			new byte [] { 0x0E, 0x0E, 0x0E, 0x0E, 0x2A, 0x22, 0x02, 0x22 },
			new byte [] { 0x6F },
			new byte [] { 0x6E },
			new byte [] { 0x1A,0xA0,0xA0,0x1A,0x1A,0xA0,0xA0,0x1A },
            new byte [] { 0x1A,0xA0,0xA0,0x1A,0x1A,0xA0,0xA0,0x1A },
            new byte [] { 0x1A,0xA0,0xA0,0x1A,0x1A,0xA0,0xA0,0x1A },
            new byte [] { 0x1A,0xA0,0xA0,0x1A,0x1A,0xA0,0xA0,0x1A },
			new byte [] { 0x6E },
			new byte [] { 0x6F },
			new byte [] { 0x6E },
			new byte [] { 0x4F },

		};

		/// <summary>
		/// Gets the tile string.
		/// </summary>
		/// <param name="tile">The tile.</param>
		/// <returns>System.String[].</returns>
		public static string[] GetTileStr(Tiles tile) {
			if ((int)tile < _vTileDefStr.Length)
				return _vTileDefStr[(int)tile];
			else 
				return _vTileDefStr[_vTileDefStr.Length-1];
		}

		/// <summary>
		/// Gets the tile colors.
		/// </summary>
		/// <param name="tile">The tile.</param>
		/// <returns>FullColor[].</returns>
		public static FullColor[] GetTileColors(Tiles tile)
		{
			var result = new FullColor[8];
			byte[] colDef;
			if ((int)tile < _vTileColors.Length)
                colDef = _vTileColors[(int)tile];
            else
				colDef = _vTileColors[_vTileColors.Length - 1];
			for (var i =0;i<result.Length;i++)
            {
				(result[i].foreGround, result[i].backGround) =
					((ConsoleColor)(colDef[i % colDef.Length] & 0xf), (ConsoleColor)(colDef[i % colDef.Length] >> 4));
            }
			return result;
		}

	}
}
