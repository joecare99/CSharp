// ***********************************************************************
// Assembly         : Sokoban_Base
// Author           : Mir
// Created          : 08-05-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="VisualsDef.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using Sokoban_Base.ViewModel;

namespace Sokoban_Base.View {
	/// <summary>
	/// Class VisualsDef.
	/// </summary>
	public static class VisualsDef {

		/// <summary>
		/// The g STR1
		/// </summary>
		static string[] gStr1 = { "    ", "[_|]", ". . ", "    ", @" <°)", @" <°)",@"/^^\", ",--," ,
								  "    ", "[|_]", " . .", "<==>", @">-||", @">_|_",@"\__/", "|__|"};
		/// <summary>
		/// The g colors
		/// </summary>
		static ConsoleColor[] gColors = { ConsoleColor.Gray,ConsoleColor.Gray,ConsoleColor.Green,ConsoleColor.White,ConsoleColor.White,ConsoleColor.White,ConsoleColor.Yellow,ConsoleColor.White,
										  ConsoleColor.Black,ConsoleColor.DarkRed,ConsoleColor.DarkGreen,ConsoleColor.DarkGreen,ConsoleColor.DarkGreen,ConsoleColor.DarkGreen,ConsoleColor.DarkYellow,ConsoleColor.DarkYellow};
		/// <summary>
		/// The w STR1
		/// </summary>
		static string[] wStr1 = { "[_|]", "[_|_", "|_|]", "|_|_",
								  "[|_]", "[|_|", "_|_]", "_|_|" };

		/// <summary>
		/// The p STR1
		/// </summary>
		static string[] pStr1 = { "|()|",  @" <°)", "(°°)",  @"(°> ",
								  "(__)",  @">-||", "|^^|",  @"||-<" };

		/// <summary>
		/// Gets the tile string.
		/// </summary>
		/// <param name="tileDef">The tile definition.</param>
		/// <returns>System.String[].</returns>
		public static string[] GetTileStr(TileDef tileDef) {
			String[] result ={ "","" };
			if ((int)tileDef < gStr1.Length / 2 && tileDef != TileDef.Player)
				(result[0], result[1]) = (gStr1[(int)tileDef], gStr1[(int)tileDef + gStr1.Length / 2]);
			else
				switch (tileDef)
				{
					case TileDef.Floor_Marked:
						result = GetTileStr(TileDef.Floor);
						break;
					case TileDef.Wall_N:
					case TileDef.Wall_S:
					case TileDef.Wall_NS:
						(result[0], result[1]) = (wStr1[0], wStr1[0 + wStr1.Length / 2]);
						break;
					case TileDef.Wall_W:
					case TileDef.Wall_NW:
					case TileDef.Wall_WS:
					case TileDef.Wall_NWS:
						(result[0], result[1]) = (wStr1[2], wStr1[2 + wStr1.Length / 2]);
						break;
					case TileDef.Wall_E:
					case TileDef.Wall_NE:
					case TileDef.Wall_SE:
					case TileDef.Wall_NSE:
						(result[0], result[1]) = (wStr1[1], wStr1[1 + wStr1.Length / 2]);
						break;
					case TileDef.Wall_WE:
					case TileDef.Wall_NWE:
					case TileDef.Wall_WSE:
					case TileDef.Wall_NWSE:
						(result[0], result[1]) = (wStr1[3], wStr1[3 + wStr1.Length / 2]);
						break;
					case TileDef.Player:
					case TileDef.Player_W:
					case TileDef.Player_S:
					case TileDef.Player_E:
						int td = 0;
						if (tileDef!= TileDef.Player) 							
						   td = ((int)tileDef - (int)TileDef.Player_W +2)/2;
						(result[0], result[1]) = (pStr1[td], pStr1[td + pStr1.Length / 2]);
						break;
					case TileDef.PlayerOverDest_E:
					case TileDef.PlayerOverDest_S:
					case TileDef.PlayerOverDest_W:
						 td = (int)TileDef.PlayerOverDest;
						(result[0], result[1]) = (gStr1[td], gStr1[td + gStr1.Length / 2]);
						break;
					default:
						break;
				}
			return result;				
		}

		/// <summary>
		/// Gets the tile colors.
		/// </summary>
		/// <param name="tileDef">The tile definition.</param>
		/// <returns>ConsoleColor[].</returns>
		public static ConsoleColor[] GetTileColors(TileDef tileDef)
        {
			ConsoleColor[] result = new ConsoleColor[2] { ConsoleColor.Gray, ConsoleColor.Black };
			if ((int)tileDef < gColors.Length / 2)
				(result[0], result[1]) = (gColors[(int)tileDef], gColors[(int)tileDef + gColors.Length / 2]);
			else if (tileDef == TileDef.Floor_Marked)
				(result[0], result[1]) = (gColors[(int)TileDef.Floor], gColors[(int)TileDef.Floor + gColors.Length / 2]);
			else if (tileDef >= TileDef.Player_W && tileDef <= TileDef.PlayerOverDest_E)
            {
				var td = (int)tileDef % 2 + (int)TileDef.Player; 
				(result[0], result[1]) = (gColors[td], gColors[td + gColors.Length / 2]);
            }
			else
				(result[0], result[1]) = (gColors[(int)TileDef.Wall], gColors[(int)TileDef.Wall + gColors.Length / 2]);
			return result;
        }
	}
}
