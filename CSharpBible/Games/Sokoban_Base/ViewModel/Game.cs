// ***********************************************************************
// Assembly         : Sokoban_Base
// Author           : Mir
// Created          : 08-04-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Game.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using Sokoban_Base.Model;
using Sokoban_Base.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sokoban_Base.ViewModel
{
	/// <summary>
	/// Class Game.
	/// </summary>
	public static class Game {
		/// <summary>
		/// The vis show
		/// </summary>
		private static Action<UserAction?>? _visShow;
		/// <summary>
		/// The vis update
		/// </summary>
		public static Action? visUpdate;
		/// <summary>
		/// The vis get user action
		/// </summary>
		public static Func<UserAction?, UserAction?>? visGetUserAction;
		/// <summary>
		/// The vis set message
		/// </summary>
		public static Action<string>? visSetMessage;

		/// <summary>
		/// Gets or sets the vis show.
		/// </summary>
		/// <value>The vis show.</value>
		public static Action<UserAction?>? visShow { get => _visShow; set => _visShow = value; }

		/// <summary>
		/// The u action
		/// </summary>
		private static UserAction? uAction = null;

		/// <summary>
		/// The playfield
		/// </summary>
		public static Playfield playfield = new Playfield();

		/// <summary>
		/// Gets the player.
		/// </summary>
		/// <value>The player.</value>
		public static Player? player=>playfield.player;
		/// <summary>
		/// Gets the size of the pf.
		/// </summary>
		/// <value>The size of the pf.</value>
		public static Size PFSize=>playfield.fieldSize;
		/// <summary>
		/// Gets the stones in dest.
		/// </summary>
		/// <value>The stones in dest.</value>
		public static int StonesInDest =>playfield.StonesInDest;
		/// <summary>
		/// Gets the stones.
		/// </summary>
		/// <value>The stones.</value>
		public static IEnumerable<Stone> Stones => playfield.Stones;
		/// <summary>
		/// Gets a value indicating whether [game solved].
		/// </summary>
		/// <value><c>true</c> if [game solved]; otherwise, <c>false</c>.</value>
		public static bool GameSolved =>playfield.GameSolved;

		/// <summary>
		/// Gets the level.
		/// </summary>
		/// <value>The level.</value>
		public static int level { get; private set; }

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		public static void Init() {
			level = 0;
		}

		/// <summary>
		/// Runs this instance.
		/// </summary>
		/// <returns>System.Nullable&lt;UserAction&gt;.</returns>
		public static UserAction? Run() {
			playfield.Setup(LabDefs.SLevels[level]);

			visShow?.Invoke(uAction);

			while (uAction != UserAction.Quit && !playfield.GameSolved && uAction != UserAction.Restart) {

				uAction = visGetUserAction?.Invoke(uAction);
				if (uAction != null && (int?)uAction < typeof(Direction).GetEnumValues().Length)
					if (!playfield.player?.Go((Direction)uAction) ?? false) {
						visSetMessage?.Invoke(string.Format(Resource1.CannotMoveMsg, (Direction)uAction));
					}
					else { visSetMessage?.Invoke(string.Format(Resource1.OK, (Direction)uAction)); }
				if (uAction == UserAction.Help) {
					visShow?.Invoke(uAction);
					uAction = visGetUserAction?.Invoke(uAction);
					visShow?.Invoke(uAction);
				}
				else
					visUpdate?.Invoke();
			}
			if (playfield.GameSolved)
				visSetMessage?.Invoke(string.Format(Resource1.GameSuccess, ""));
			else if (uAction == UserAction.Quit)
				visSetMessage?.Invoke(string.Format(Resource1.EndMessage, ""));
			else if (uAction == UserAction.Restart)
				visSetMessage?.Invoke(string.Format(Resource1.RestartMessage, ""));

			visShow?.Invoke(uAction);

			if (uAction != UserAction.Restart)
				level++;
			if (uAction != UserAction.Quit)
				uAction = visGetUserAction?.Invoke(uAction);
			return uAction;
		}

		/// <summary>
		/// Cleanups this instance.
		/// </summary>
		public static void Cleanup() {
			playfield.Clear();
		}

		/// <summary>
		/// Gets the tile.
		/// </summary>
		/// <param name="p">The p.</param>
		/// <returns>TileDef.</returns>
		static public TileDef GetTile(Point p)
		{
			TileDef result = TileDef.Empty;
			var f = playfield[p]?.fieldDef;
			if (f <= FieldDef.StoneInDest && f != FieldDef.Wall && f != FieldDef.Player)
				result = (TileDef)f;
			else if (f == FieldDef.Player && playfield[p]?.Item is Player pl)
			{
				result = (pl.LastDir) switch
				{
					Direction.East => TileDef.Player_E,
					Direction.South => TileDef.Player_S,
					Direction.West => TileDef.Player_W,
					_ => TileDef.Player
				};  
			}
			else if (f == FieldDef.Wall)
			{
				int w = 0;
				result = TileDef.Wall;
				w += ((playfield[Offsets.DirOffset(Direction.North, p)] is Wall) ? 1 : 0);
				w += ((playfield[Offsets.DirOffset(Direction.West, p)] is Wall) ? 2 : 0);
				w += ((playfield[Offsets.DirOffset(Direction.South, p)] is Wall) ? 4 : 0);
				w += ((playfield[Offsets.DirOffset(Direction.East, p)] is Wall) ? 8 : 0);
				if (w > 0)
					result = (TileDef)(w - 1 + (int)TileDef.Wall_N);
			}
			return result;
		}

		/// <summary>
		/// Gets the old position.
		/// </summary>
		/// <param name="p">The p.</param>
		/// <returns>Point.</returns>
		internal static Point GetOldPos(Point p)
        {
			Point result = new(p.X,p.Y);
			if (playfield[p] is Floor f && f.Item !=null)
			   result = f.Item.OldPosition;
			return result;
		}
	}
}
