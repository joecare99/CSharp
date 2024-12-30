// ***********************************************************************
// Assembly         : Werner_Flaschbier_Base
// Author           : Mir
// Created          : 08-02-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Visual.cs" company="Werner_Flaschbier_Base">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using ConsoleDisplay.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Werner_Flaschbier_Base.Model;
using Werner_Flaschbier_Base.ViewModels;

namespace Werner_Flaschbier_Base.View
{
    /// <summary>
    /// Class Visual.
    /// </summary>
    public class Visual : IVisual {

		#region Properties
		/// <summary>
		/// The buffer
		/// </summary>
		private Tiles[] _buffer = new Tiles[12 * 20];
		/// <summary>
		/// The game
		/// </summary>
		private IWernerViewModel? _game;

		/// <summary>
		/// My console
		/// </summary>
		private IConsole _console ;

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
		public Visual(IWernerViewModel viewModel,IConsole console)
		{
			_game = viewModel;
			_game.PropertyChanged += OnPropertyChanged;
			_console = console;
            FullRedraw();
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Tiles")
                G_visUpdate(sender, _game.HalfStep);
            else if (e.PropertyName == "Level")
                FullRedraw();
        }

        /// <summary>
        /// Determines whether the specified e is enemy.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns><c>true</c> if the specified e is enemy; otherwise, <c>false</c>.</returns>
        private bool IsEnemy(Tiles e) => e == Tiles.Enemy_Dwn 
			|| e == Tiles.Enemy_Up 
			|| e == Tiles.Enemy_Left 
			|| e == Tiles.Enemy_Right;

		/// <summary>
		/// gs the vis update.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">if set to <c>true</c> [e].</param>
		private void G_visUpdate(object? sender, bool e)
        {
			List<(Point, Tiles, Point)> diffFields = new();
		    var p = new Point();
			if (_game==null) return;

			for (p.Y = 0; p.Y < _game.size.Height; p.Y++)
				for (p.X = 0; p.X < _game.size.Width; p.X++)
				{
					var td = _game.Tiles[p];
					if ((Tiles)td != _buffer[p.X + p.Y * _game.size.Width] || (IsEnemy((Tiles)td) && _game.OldPos(p)!=p))
					{
						Point pp = new Point(p.X,p.Y);
#pragma warning disable CS8604 // Mögliches Nullverweisargument.
						diffFields.Add((pp, (Tiles)td, _game.OldPos(p)));
#pragma warning restore CS8604 // Mögliches Nullverweisargument.
						if (!e)	
						   _buffer[p.X + p.Y * _game.size.Width] = (Tiles)td;
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
						WriteTile(p1, (Tiles)_game.Tiles[p1]);
						var p2 = new Point(f.Item3.X, f.Item1.Y);
						WriteTile(p2, (Tiles)_game.Tiles[p2]);
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
		public void FullRedraw(object? sender=null,EventArgs? e=null)
        {
            // basic Checks
            if (_game == null) return;

            // Draw playfield
            Point p = new Point();
            for (p.Y = 0; p.Y < _game.size.Height; p.Y++)
                for (p.X = 0; p.X < _game.size.Width; p.X++)
                    WriteTile(p, _buffer[p.X + p.Y * _game.size.Width] = (Tiles)_game.Tiles[p]);

            // Draw statistics
            ShowStatistics();
        }

		/// <summary>
		/// Shows the statistics.
		/// </summary>
		private void ShowStatistics()
        {
			if (_game == null) return;
			_console.SetCursorPosition(0, 24);
            _console.BackgroundColor = ConsoleColor.Black;
            _console.ForegroundColor = ConsoleColor.Yellow;
            _console.Write($"\t{_game.Level + 1}\t\t{_game.Score}\t\t{_game.Lives}/{_game.MaxLives} \t\t{Math.Floor(_game.TimeLeft)}\t\x08");
        }

		/// <summary>
		/// Writes the tile.
		/// </summary>
		/// <param name="p">The p.</param>
		/// <param name="tile">The tile.</param>
		public void WriteTile(PointF p, Tiles tile)
        {
			Size s = new Size(4, 2);
			var colors = VTileDef.GetTileColors(tile);
			var sTileStr = VTileDef.GetTileStr(tile);
			for (int i = 0; i < sTileStr.Length; i++)
				for (int x = 0; x < sTileStr[i].Length; x++)
				{
					_console.ForegroundColor = colors[x + i * s.Width].foreGround;
					_console.BackgroundColor = colors[x + i * s.Width].backGround;
					_console.SetCursorPosition((int)(p.X * s.Width) + x, (int)(p.Y * s.Height) + i);
					_console.Write(sTileStr[i][x]);
				}
		}

		/// <summary>
		/// Sounds the specified gs.
		/// </summary>
		/// <param name="gs">The gs.</param>
		public void Sound(GameSound gs)
		{
			switch (gs)
			{
				case GameSound.NoSound:
					break;
				case GameSound.Tick:
					_console.Beep(1000, 10);
					break;
				case GameSound.DeepBoom:
					_console.Beep(300, 20);
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
		public bool CheckUserAction()
		{
			bool result = false;
			UserAction action = UserAction.Nop;
			if (_console.KeyAvailable)
			{
				var ch = _console.ReadKey()?.KeyChar ?? '\x00';

				ch = Char.ToUpper(ch);
				if (keyAction.ContainsKey(ch))
				{
					action = keyAction[ch];
					result = true;
				}
			}
            _game?.HandleUserAction(action);
            return result;
		}

        #endregion
    }
}
