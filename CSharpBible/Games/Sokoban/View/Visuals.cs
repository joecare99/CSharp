// ***********************************************************************
// Assembly         : Sokoban_Base
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
using ConsoleDisplay.View;
using Sokoban_Base.Model;
using Sokoban_Base.Properties;
using Sokoban_Base.ViewModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace Sokoban_Base.View
{
    /// <summary>
    /// The class that handles the UI
    /// </summary>
    public static class Visuals
    {

        #region Properties
        /// <summary>
        /// The d buffer
        /// </summary>
        private static TileDef[] dBuffer=new TileDef[] { };

        /// <summary>
        /// My console
        /// is a Console-Proxy for debugging &amp; Testing
        /// </summary>
        public static MyConsole myConsole = new MyConsole();
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

        private static ITileDef _visualDef;

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public static string Message { get; set; } = "";
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

            _visualDef = new VisualsDef();
        }

        /// <summary>
        /// Shows the specified u action.
        /// </summary>
        /// <param name="uAction">The u action.</param>
        public static void Show(UserAction? uAction=null)
        {
            myConsole.Clear();
			if (!myConsole.IsOutputRedirected)
				myConsole.WindowHeight = Math.Min(myConsole.LargestWindowHeight, 40); 

			if (uAction == UserAction.Help)
            {
                ShowHelp();
                return;
            }

            dBuffer = new TileDef[Game.PFSize.Width* Game.PFSize.Height];
            var p = new Point();
            for (p.Y = 0; p.Y < Game.PFSize.Height; p.Y++)
                for (p.X = 0; p.X < Game.PFSize.Width; p.X++)                
                    WriteTile(p, dBuffer[p.X + p.Y * Game.PFSize.Width] = Game.GetTile(p));
                
			ShowStatistics();
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        public static void Update()
        {
            List<(Point, TileDef, Point)> diffFields = ComputeUpdateList();

            ShowIntermediateTiles(diffFields);

            myConsole.ForegroundColor = ConsoleColor.Gray;
            myConsole.BackgroundColor = ConsoleColor.Black;
            myConsole.SetCursorPosition(40, Game.PFSize.Height * 2 + 7);

            Thread.Sleep(40);
            for (int i = 0; i < 3; i++)
                if (!myConsole.KeyAvailable)
                    Thread.Sleep(40);

            foreach (var f in diffFields)
                WriteTile(f.Item1, f.Item2);

            ShowStatistics();
        }

        /// <summary>
        /// Shows the intermediate tiles.
        /// </summary>
        /// <param name="diffFields">The difference fields.</param>
        private static void ShowIntermediateTiles(List<(Point, TileDef, Point)> diffFields)
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
        private static List<(Point, TileDef, Point)> ComputeUpdateList()
        {
            List<(Point, TileDef, Point)> diffFields = new();
            for (int y = 0; y < Game.PFSize.Height; y++)
            {
                for (int x = 0; x < Game.PFSize.Width; x++)
                {
                    var p = new Point(x, y);
                    var td = Game.GetTile(p);
                    if (td != dBuffer[x + y * Game.PFSize.Width])
                    {
#pragma warning disable CS8604 // Mögliches Nullverweisargument.
                        diffFields.Add((p, td, Game.GetOldPos(p)));
#pragma warning restore CS8604 // Mögliches Nullverweisargument.
                        dBuffer[x + y * Game.PFSize.Width] = td;
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
        public static UserAction? WaitforUser(UserAction? uAction)
        {
            myConsole.Write(Resource1.SelectAction);
            if (Game.GameSolved || uAction==UserAction.Help || uAction == UserAction.Restart) 
                myConsole.Write(Resource1.Continue);
            else 
                foreach (Direction dir in Enum.GetValues(typeof(Direction))) Console.Write($", {MarkFirst(dir.ToString())}"); 
            myConsole.Write("\t=>");

            var ch= char.ToUpper(myConsole.ReadKey()?.KeyChar ?? '\x00');
            
            uAction = null;
            if (uAction == null && KeyAction.ContainsKey(ch))
                uAction = KeyAction[ch];

            return uAction;
        }

        #region Private Methods
        /// <summary>
        /// Shows the help.
        /// </summary>
        private static void ShowHelp()
        {
            var sDirs = "";
            foreach (string d in typeof(Direction).GetEnumNames()) sDirs += $", {MarkFirst(d)}";

            myConsole.WriteLine(string.Format(Resource1.InfoText,
                Resource1.stone,
                sDirs.TrimStart(','),
                $"{_visualDef.GetTileDef(TileDef.Stone).lines[0]}\r\n{_visualDef.GetTileDef(TileDef.Stone).lines[1]}",
                $"{_visualDef.GetTileDef(TileDef.Player).lines[0]} \r\n {_visualDef.GetTileDef(TileDef.Player).lines[1]}",
                $"{_visualDef.GetTileDef(TileDef.Wall).lines[0]} \r\n {_visualDef.GetTileDef(TileDef.Wall).lines[1]}",
                $"{_visualDef.GetTileDef(TileDef.Destination).lines[0]}  \r\n  {_visualDef.GetTileDef(TileDef.Destination).lines[1]}"
                ));
            var cp = myConsole.GetCursorPosition();
            WriteTile(new PointF(0, (cp.Top - 3) * 0.5f), TileDef.Destination);
            WriteTile(new PointF(0, (cp.Top - 6) * 0.5f), TileDef.Wall);
            WriteTile(new PointF(0, (cp.Top - 9) * 0.5f), TileDef.Player_E);
            WriteTile(new PointF(0, (cp.Top - 12) * 0.5f), TileDef.Stone);
            myConsole.SetCursorPosition(cp.Left, cp.Top);
            myConsole.ForegroundColor = ConsoleColor.Gray;
            myConsole.BackgroundColor = ConsoleColor.Black;
            myConsole.WriteLine(typeof(Visuals).Assembly.FullName);
            foreach (var l in Resource1.Version.Split(new string[] { "\r\n" }, StringSplitOptions.None))
                if (l.Contains("Revision") || l.Contains("Date"))
                    Console.WriteLine(l.Substring(l.IndexOf("]") + 1));
            myConsole.WriteLine();
        }

        /// <summary>
        /// Shows the statistics.
        /// </summary>
        private static void ShowStatistics()
        {
            myConsole.ForegroundColor = ConsoleColor.Gray;
            myConsole.BackgroundColor = ConsoleColor.Black;
            myConsole.SetCursorPosition(0, Game.PFSize.Height * 2);
#if DEBUG    
            foreach (var s in Game.Stones)
                myConsole.Write($" {Resource1.stone} ({s.Position.X},{s.Position.Y}) {(s.field is Destination ? "OK " : "   ")}\t");
#endif
            myConsole.Write($"\r\n" + String.Format(Resource1.StonesInDest, Game.StonesInDest));


            if (Game.player != null)
            {
                myConsole.SetCursorPosition(0, Game.PFSize.Height * 2 + 3);
#if DEBUG
                Console.WriteLine($"Player ({Game.player.Position.X},{Game.player.Position.Y})");
                Console.Write(Resource1.PosibMoves);
                foreach (var d in Game.player.MoveableDirs())
                {
                    Console.Write($"\t{typeof(Direction).GetEnumName(d)},");
                }
                Console.WriteLine("                     ");
#endif
                myConsole.SetCursorPosition(0, Game.PFSize.Height * 2 + 6);
                myConsole.Write(Message + "                                   ");
                myConsole.SetCursorPosition(0, Game.PFSize.Height * 2 + 7);

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
        static void WriteTile(PointF p, TileDef td)
        {
            var tld = _visualDef.GetTileDef(td);
            myConsole.ForegroundColor = tld.colors[0].fgr;
			myConsole.BackgroundColor = tld.colors[0].bgr;
            for (int i = 0; i < tld.lines.Length; i++)
            {
               myConsole.SetCursorPosition((int)(p.X * 4), (int)(p.Y * 2) + i);
               myConsole.Write(tld.lines[i]);
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
