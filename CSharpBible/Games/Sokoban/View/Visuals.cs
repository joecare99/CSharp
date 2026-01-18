// ***********************************************************************
// Assembly         : Sokoban
// Author           : Mir
// Created          : 08-04-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Visuals.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using BaseLib.Interfaces;
using BaseLib.Models;
using ConsoleDisplay.View;
using Sokoban;
using Sokoban.Models;
using Sokoban.Properties;
using Sokoban.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace Sokoban.View
{
    /// <summary>
    /// The class that handles the UI
    /// </summary>
    public sealed class Visuals
    {
        private readonly IConsole _console;
        private readonly ITileDef _visualDef;

        /// <summary>
        /// Gets the game instance.
        /// </summary>
        public IGame SokobanGame { get; }

        #region Properties
        /// <summary>
        /// The d buffer
        /// </summary>
        private TileDef[] dBuffer = new TileDef[] { };

        /// <summary>
        /// My console
        /// is a Console-Proxy for debugging &amp; Testing
        /// </summary>
        /// <summary>
        /// Initializes a new instance of the <see cref="Visuals"/> class.
        /// </summary>
        /// <param name="console">The console implementation to use.</param>
        /// <param name="game">The game instance to render.</param>
        public Visuals(IConsole console, IGame game)
        {
            _console = console ?? throw new ArgumentNullException(nameof(console));
            SokobanGame = game ?? throw new ArgumentNullException(nameof(game));
            _visualDef = new VisualsDef();
        }
        /// <summary>
        /// The key action
        /// </summary>
        public static Dictionary<char, UserAction> KeyAction = new Dictionary<char, UserAction> {
            { 'I', UserAction.GoNorth }, 
            { 'J', UserAction.GoWest }, 
            { 'K', UserAction.GoSouth }, 
            { 'L', UserAction.GoEast },
            { '?', UserAction.Help },
            { 'H', UserAction.Help },
            { 'R', UserAction.Restart },
            { 'Q', UserAction.Quit },
            { '\u001b', UserAction.Quit } };

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; set; } = "";
        #endregion

        #region Methods
        /// <summary>
        /// Initializes static members of the <see cref="Visuals" /> class.
        /// </summary>
        static Visuals()
        {
            foreach (var dir in typeof(Direction).GetEnumValues())
            try
            {
                KeyAction.Add(dir.ToString()?[0] ?? '\0', (UserAction)dir);
            }
            catch { }

            // instance visuals are created via DI/ctor.
        }

        /// <summary>
        /// Shows the specified u action.
        /// </summary>
        /// <param name="uAction">The u action.</param>
        public void Show(UserAction? uAction=null)
        {
			_console.Clear();
			if (!_console.IsOutputRedirected)
				_console.WindowHeight = Math.Min(_console.LargestWindowHeight, 40); 

			if (uAction == UserAction.Help)
            {
                ShowHelp();
                return;
            }

            dBuffer = new TileDef[SokobanGame.PFSize.Width* SokobanGame.PFSize.Height];
            var p = new Point();
            for (p.Y = 0; p.Y < SokobanGame.PFSize.Height; p.Y++)
                for (p.X = 0; p.X < SokobanGame.PFSize.Width; p.X++)                
                    WriteTile(p, dBuffer[p.X + p.Y * SokobanGame.PFSize.Width] = SokobanGame.GetTile(p));
                
			ShowStatistics();
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        public void Update()
        {
            List<(Point, TileDef, Point)> diffFields = ComputeUpdateList();

            ShowIntermediateTiles(diffFields);

            _console.ForegroundColor = ConsoleColor.Gray;
            _console.BackgroundColor = ConsoleColor.Black;
            _console.SetCursorPosition(40, SokobanGame.PFSize.Height * 2 + 7);

            Thread.Sleep(40);
            for (int i = 0; i < 3; i++)
                if (!_console.KeyAvailable)
                    Thread.Sleep(40);

            foreach (var f in diffFields)
                WriteTile(f.Item1, f.Item2);

            ShowStatistics();
        }

        /// <summary>
        /// Shows the intermediate tiles.
        /// </summary>
        /// <param name="diffFields">The difference fields.</param>
        private void ShowIntermediateTiles(List<(Point, TileDef, Point)> diffFields)
        {
            foreach (var f in diffFields)
                if (f.Item1 == f.Item3)
                {
                    WriteTile(f.Item1, f.Item2);
                }
            foreach (var f in diffFields)
                if (f.Item1 != f.Item3)
                {
                    var zPos = new PointF((f.Item1.X + f.Item3.X) * 0.5f, (f.Item1.Y + f.Item3.Y) * 0.5f);
                    WriteTile(zPos, f.Item2);
                }
        }

        /// <summary>
        /// Computes the update list.
        /// </summary>
        /// <returns>List&lt;System.ValueTuple&lt;Point, TileDef, Point&gt;&gt;.</returns>
        private List<(Point, TileDef, Point)> ComputeUpdateList()
        {
            List<(Point, TileDef, Point)> diffFields = new();
            for (int y = 0; y < SokobanGame.PFSize.Height; y++)
            {
                for (int x = 0; x < SokobanGame.PFSize.Width; x++)
                {
                    var p = new Point(x, y);
                    var td = SokobanGame.GetTile(p);
                    if (td != dBuffer[x + y * SokobanGame.PFSize.Width])
                    {
#pragma warning disable CS8604 // Mögliches Nullverweisargument.
                        diffFields.Add((p, td, SokobanGame.GetOldPos(p)));
#pragma warning restore CS8604 // Mögliches Nullverweisargument.
                        dBuffer[x + y * SokobanGame.PFSize.Width] = td;
                    }
                }
            }

            return diffFields;
        }

        /// <summary>
        /// Waitfors the user.
        /// </summary>
        /// <param name="uAction">The u action.</param>
        /// <returns>System.Nullable&lt;UserAction&gt;.</returns>
        public UserAction? WaitforUser(UserAction? uAction)
        {
            _console.Write(Resource1.SelectAction);
            if (SokobanGame.GameSolved || uAction==UserAction.Help || uAction == UserAction.Restart) 
                _console.Write(Resource1.Continue);
            else 
                foreach (Direction dir in Enum.GetValues(typeof(Direction))) Console.Write($", {MarkFirst(dir.ToString())}"); 
            _console.Write("\t=>");

            var ch= char.ToUpper(_console.ReadKey()?.KeyChar ?? '\x00');
            
            uAction = null;
            if (uAction == null && KeyAction.ContainsKey(ch))
                uAction = KeyAction[ch];

            return uAction;
        }

        #region Private Methods
        /// <summary>
        /// Shows the help.
        /// </summary>
        private void ShowHelp()
        {
            var sDirs = "";
            foreach (string d in typeof(Direction).GetEnumNames()) sDirs += $", {MarkFirst(d)}";

            _console.WriteLine(string.Format(Resource1.InfoText,
                Resource1.stone,
                sDirs.TrimStart(','),
                $"{_visualDef.GetTileDef(TileDef.Stone).lines[0]}\r\n{_visualDef.GetTileDef(TileDef.Stone).lines[1]}",
                $"{_visualDef.GetTileDef(TileDef.Player).lines[0]} \r\n {_visualDef.GetTileDef(TileDef.Player).lines[1]}",
                $"{_visualDef.GetTileDef(TileDef.Wall).lines[0]} \r\n {_visualDef.GetTileDef(TileDef.Wall).lines[1]}",
                $"{_visualDef.GetTileDef(TileDef.Destination).lines[0]}  \r\n  {_visualDef.GetTileDef(TileDef.Destination).lines[1]}"
                ));
            var cp = _console.GetCursorPosition();
            WriteTile(new PointF(0, (cp.Top - 3) * 0.5f), TileDef.Destination);
            WriteTile(new PointF(0, (cp.Top - 6) * 0.5f), TileDef.Wall);
            WriteTile(new PointF(0, (cp.Top - 9) * 0.5f), TileDef.Player_E);
            WriteTile(new PointF(0, (cp.Top - 12) * 0.5f), TileDef.Stone);
            _console.SetCursorPosition(cp.Left, cp.Top);
            _console.ForegroundColor = ConsoleColor.Gray;
            _console.BackgroundColor = ConsoleColor.Black;
            _console.WriteLine(typeof(Visuals).Assembly.FullName);
            foreach (var l in Resource1.Version.Split(new string[] { "\r\n" }, StringSplitOptions.None))
                if (l.Contains("Revision") || l.Contains("Date"))
                    _console.WriteLine(l.Substring(l.IndexOf("]") + 1));
            _console.WriteLine();
        }

        /// <summary>
        /// Shows the statistics.
        /// </summary>
        private void ShowStatistics()
        {
            _console.ForegroundColor = ConsoleColor.Gray;
            _console.BackgroundColor = ConsoleColor.Black;
            _console.SetCursorPosition(0, SokobanGame.PFSize.Height * 2);
#if DEBUG    
            foreach (var s in SokobanGame.Stones)
                Console.Write($" {Resource1.stone} ({s.Position.X},{s.Position.Y}) {(s.field is Destination ? "OK " : "   ")}\t");
#endif
            _console.Write($"\r\n" + String.Format(Resource1.StonesInDest, SokobanGame.StonesInDest));


            if (SokobanGame.player != null)
            {
                _console.SetCursorPosition(0, SokobanGame.PFSize.Height * 2 + 3);
#if DEBUG
                Console.WriteLine($"Player ({SokobanGame.player.Position.X},{SokobanGame.player.Position.Y})");
                Console.Write(Resource1.PosibMoves);
                foreach (var d in SokobanGame.player.MoveableDirs())
                {
                    Console.Write($"\t{typeof(Direction).GetEnumName(d)},");
                }
                Console.WriteLine("                     ");
#endif
                _console.SetCursorPosition(0, SokobanGame.PFSize.Height * 2 + 6);
                _console.Write(Message + "                                   ");
                _console.SetCursorPosition(0, SokobanGame.PFSize.Height * 2 + 7);

            }
        }

#if DEBUG
        /// <summary>
        /// Writes the tile.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="td">The td.</param>
        public
#else
        private
#endif
        void WriteTile(PointF p, TileDef td)
        {
            var tld = _visualDef.GetTileDef(td);
            _console.ForegroundColor = tld.colors[0].fgr;
			_console.BackgroundColor = tld.colors[0].bgr;
            for (int i = 0; i < tld.lines.Length; i++)
            {
               _console.SetCursorPosition((int)(p.X * 4), (int)(p.Y * 2) + i);
               _console.Write(tld.lines[i]);
            }
        }

#if DEBUG
        /// <summary>
        /// Marks the first.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <returns>System.String.</returns>
        public
#else
        private
#endif
        static string MarkFirst(string v) => $"({v[0]}){v[1..]}";
        #endregion
        #endregion
    }
}
