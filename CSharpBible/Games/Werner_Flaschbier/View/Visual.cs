// ***********************************************************************
// Assembly         : Werner_Flaschbier_Base
// Author           : Mir
// Created          : 08-02-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Visual.cs" company="Werner_Flaschbier_Base">
//     Copyright (c) HP Inc.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using ConsoleDisplay.View;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Werner_Flaschbier_Base.ViewModel;

namespace Werner_Flaschbier_Base.View {
	/// <summary>
	/// Class Visual.
	/// </summary>
	public static class Visual {

		#region Properties
		/// <summary>
		/// The buffer
		/// </summary>
		private static Tiles[] _buffer = new Tiles[12 * 20];
		/// <summary>
		/// The game
		/// </summary>
		private static Game? _game;

		/// <summary>
		/// My console
		/// </summary>
		public static MyConsoleBase myConsole = new MyConsole();

		/// <summary>
		/// The key action
		/// </summary>
		public static Dictionary<char, UserAction> keyAction = new Dictionary<char, UserAction> {
			{ 'I', UserAction.GoUp },
			{ 'J', UserAction.GoWest },
			{ 'K', UserAction.GoDown },
			{ 'L', UserAction.GoEast },
			{ '?', UserAction.Help },
			{ 'H', UserAction.Help },
			{ 'R', UserAction.Restart },
			{ 'Q', UserAction.Quit },
#if DEBUG
			{ 'N', UserAction.NextLvl },
			{ 'V', UserAction.PrevLvl },
#endif
			{ '\u001b', UserAction.Quit } };
		#endregion

		#region Methods
		/// <summary>
		/// Sets the game.
		/// </summary>
		/// <param name="g">The g.</param>
		public static void SetGame(Game g)
        {
			_game = g;
            g.visUpdate += G_visUpdate;
			g.visFullRedraw += FullRedraw;
			FullRedraw();
        }

		/// <summary>
		/// Determines whether the specified e is enemy.
		/// </summary>
		/// <param name="e">The e.</param>
		/// <returns><c>true</c> if the specified e is enemy; otherwise, <c>false</c>.</returns>
		private static bool IsEnemy(Tiles e) => e == Tiles.Enemy_Dwn 
			|| e == Tiles.Enemy_Up 
			|| e == Tiles.Enemy_Left 
			|| e == Tiles.Enemy_Right;

		/// <summary>
		/// gs the vis update.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">if set to <c>true</c> [e].</param>
		private static void G_visUpdate(object? sender, bool e)
        {
			List<(Point, Tiles, Point)> diffFields = new();
		    var p = new Point();
			if (_game==null) return;

			for (p.Y = 0; p.Y < _game.size.Height; p.Y++)
				for (p.X = 0; p.X < _game.size.Width; p.X++)
				{
					var td = _game[p];
					if (td != _buffer[p.X + p.Y * _game.size.Width] || (IsEnemy(td) && _game.OldPos(p)!=p))
					{
						Point pp = new Point(p.X,p.Y);
#pragma warning disable CS8604 // Mögliches Nullverweisargument.
						diffFields.Add((pp, td, _game.OldPos(p)));
#pragma warning restore CS8604 // Mögliches Nullverweisargument.
						if (!e)	
						   _buffer[p.X + p.Y * _game.size.Width] = td;
					}
				}
			if (e)
            {
				foreach (var f in diffFields)
					if (f.Item1 == f.Item3)
					{
						WriteTile(f.Item1, f.Item2);
					}
				foreach (var f in diffFields)
					if (f.Item1 != f.Item3 && Math.Abs(f.Item1.X - f.Item3.X) < 2)
					{
						var zPos = new PointF((f.Item1.X + f.Item3.X) * 0.5f, (f.Item1.Y + f.Item3.Y) * 0.5f);						
						WriteTile(zPos, f.Item2);
					}
			}
			else
				foreach (var f in diffFields)
                {
					if (f.Item1.X != f.Item3.X && f.Item1.Y != f.Item3.Y)
					{
						var p1 = new Point(f.Item1.X, f.Item3.Y);
						WriteTile(p1, _game[p1]);
						var p2 = new Point(f.Item3.X, f.Item1.Y);
						WriteTile(p2, _game[p2]);
					}
					WriteTile(f.Item1, f.Item2);
                }

			ShowStatistics();
		}

		/// <summary>
		/// Fulls the redraw.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		public static void FullRedraw(object? sender=null,EventArgs? e=null)
        {
            // basic Checks
            if (_game == null) return;

            // Draw playfield
            Point p = new Point();
            for (p.Y = 0; p.Y < _game.size.Height; p.Y++)
                for (p.X = 0; p.X < _game.size.Width; p.X++)
                    WriteTile(p, _buffer[p.X + p.Y * _game.size.Width] = _game[p]);

            // Draw statistics
            ShowStatistics();
        }

		/// <summary>
		/// Shows the statistics.
		/// </summary>
		private static void ShowStatistics()
        {
			if (_game == null) return;
			myConsole.SetCursorPosition(0, 24);
            myConsole.BackgroundColor = ConsoleColor.Black;
            myConsole.ForegroundColor = ConsoleColor.Yellow;
            myConsole.Write($"\t{_game.level + 1}\t\t{_game.Score}\t\t{_game.Lives}/{Game.MaxLives} \t\t{Math.Floor(_game.TimeLeft)}\t\x08");
        }

		/// <summary>
		/// Writes the tile.
		/// </summary>
		/// <param name="p">The p.</param>
		/// <param name="tile">The tile.</param>
		public static void WriteTile(PointF p, Tiles tile)
        {
			Size s = new Size(4, 2);
			var colors = VTileDef.GetTileColors(tile);
			var sTileStr = VTileDef.GetTileStr(tile);
			for (int i = 0; i < sTileStr.Length; i++)
				for (int x = 0; x < sTileStr[i].Length; x++)
				{
					myConsole.ForegroundColor = colors[x + i * s.Width].foreGround;
					myConsole.BackgroundColor = colors[x + i * s.Width].backGround;
					myConsole.SetCursorPosition((int)(p.X * s.Width) + x, (int)(p.Y * s.Height) + i);
					myConsole.Write(sTileStr[i][x]);
				}
		}

		/// <summary>
		/// Sounds the specified gs.
		/// </summary>
		/// <param name="gs">The gs.</param>
		public static void Sound(GameSound gs)
		{
			switch (gs)
			{
				case GameSound.NoSound:
					break;
				case GameSound.Tick:
					myConsole.Beep(1000, 10);
					break;
				case GameSound.DeepBoom:
					myConsole.Beep(300, 20);
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// Checks the user action.
		/// </summary>
		/// <param name="action">The action.</param>
		/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
		public static bool CheckUserAction(out UserAction action)
		{
			bool result = false;
			action = UserAction.Nop;
			if (myConsole.KeyAvailable)
			{
				var ch = myConsole.ReadKey()?.KeyChar ?? '\x00';

				ch = Char.ToUpper(ch);
				if (keyAction.ContainsKey(ch))
				{
					action = keyAction[ch];
					result = true;
				}
			}
			return result;
		}

        #endregion
    }
}
