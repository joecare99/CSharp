// ***********************************************************************
// Assembly         : Snake_Console
// Author           : Mir
// Created          : 08-02-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Visual.cs" company="JC-Soft">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using ConsoleDisplay.View;
using System;
using System.Collections.Generic;
using System.Drawing;
using Snake_Base.ViewModel;

namespace Snake_Console.View
{
	/// <summary>
	/// Class Visual.
	/// </summary>
	public class Visual : IVisual {

        #region Properties

        /// The game
        /// </summary>
        private  ISnakeViewModel? _game;

		/// <summary>
		/// My console
		/// </summary>
		public  IConsole myConsole;

		private ITileDisplay _tileDisplay;	
		/// <summary>
		/// The key action
		/// </summary>
		public static Dictionary<char, UserAction> keyAction = new Dictionary<char, UserAction> {
			{ 'I', UserAction.GoNorth },
			{ 'J', UserAction.GoWest },
			{ 'K', UserAction.GoSouth },
			{ 'L', UserAction.GoEast },
			{ '?', UserAction.Help },
			{ 'H', UserAction.Help },
			{ 'R', UserAction.Restart },
			{ 'Q', UserAction.Quit },
#if DEBUG
#endif
			{ '\u001b', UserAction.Quit } };
		#endregion

		#region Methods
		static Visual() {
            myConsole = new MyConsole();
            TileDisplay.myConsole = myConsole;
        }
        /// <summary>
        /// Sets the game.
        /// </summary>
        /// <param name="g">The g.</param>
        public static void SetGame(Game g)
        {
			_game = g;
            _tileDisplay = new TileDisplay(new Point(1, 1), g.size, new TileDef());
			_tileDisplay.DispOffset = new Point(-1, -1);
            _tileDisplay.FncGetTile = (p)=>(Enum)_game.GetTile(p);
			_tileDisplay.FncOldPos = _game.OldPos;
            g.visUpdate += G_visUpdate;
			g.visFullRedraw += FullRedraw;
			FullRedraw();
        }


		/// <summary>
		/// gs the vis update.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">if set to <c>true</c> [e].</param>
		private static void G_visUpdate(object? sender, bool e)
        {
			_tileDisplay.Update(e);

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

			_tileDisplay.FullRedraw();
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
            myConsole.Write($"\t{_game.Level + 1}\t\t{_game.Score}\t\t{_game.Lives}/{Game.MaxLives}\t\x08");
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
