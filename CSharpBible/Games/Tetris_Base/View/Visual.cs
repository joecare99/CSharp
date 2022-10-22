using ConsoleDisplay.View;
using System;
using System.Collections.Generic;
using System.Drawing;
using Tetris_Base.Helper;
using Tetris_Base.Model;

namespace Tetris_Base.View {
    /// <summary>
    /// Class Visual.
    /// </summary>
    public static class Visual
    {
        /// <summary>
        /// The ZFFRN
        /// </summary>
        static public string[] zffrn = {@"/\",@"/|",@"^)",@"^/","/|","^]","/^","^/","()",@"()",
                                        @"\/",@" |",@"(_",@"_)","-|","(_","()","/ ","()",@"_/"};

        static private MyConsoleBase _console = new MyConsole();
        static private PlayGround? _playGround = null;

        #region Properties
        /// <summary>
        /// The display1
        /// </summary>
        static public Display display1 = new Display(2, 2, 14, 22);
        /// <summary>
        /// The display2
        /// </summary>
        static public Display display2 = new Display(18, 2, 6, 6);
        private static bool flag;

        /// <summary>
        /// Gets or sets the console.
        /// </summary>
        /// <value>The console.</value>
        static public MyConsoleBase console { get => _console; set => PropertyClass.SetPropertyP(ref _console, value, (o, n) => Display.myConsole = n ); }
        /// <summary>
        /// Gets or sets the play ground.
        /// </summary>
        /// <value>The play ground.</value>
        public static PlayGround? playGround { get => _playGround; set => PropertyClass.SetPropertyP(ref _playGround,value); }

        /// <summary>
        /// Gets or sets the user action.
        /// </summary>
        /// <value>The user action.</value>
        public static Func<UserAction,bool>? userAction { get; set; }
        /// <summary>
        /// The key action
        /// </summary>
        public static Dictionary<char, UserAction> keyAction = new Dictionary<char, UserAction> {
            { 'I', UserAction.RotateLeft },
            { 'J', UserAction.MoveLeft },
            { 'K', UserAction.MoveDown },
            { 'L', UserAction.MoveRight },
            { ' ', UserAction.Drop },
            { '?', UserAction.Help },
            { 'H', UserAction.Help },
            { 'R', UserAction.Restart },
            { 'Q', UserAction.Quit },
            { '\u001b', UserAction.Quit } };

        #endregion


        /// <summary>
        /// Initializes static members of the <see cref="Visual"/> class.
        /// </summary>
        static Visual()
        {
            _playGround = null;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public static void Init()
        {
            _playGround = null;
        }

        #region Methods
        /// <summary>
        /// Previouses the paint.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="c">The c.</param>
        public static void PrevPaint(Point p, ConsoleColor c)
        {
            p.Offset(2, 2);
            display2.PutPixel(p.X, p.Y, c);
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        public static void Update()
        {
            if (playGround == null) return;
            if (playGround.Dirty)
                for (int y = 0; y < playGround.FieldSize.Height; y++)
                    for (int x = 0; x < playGround.FieldSize.Width; x++)
                        display1.PutPixel(x + 1, y, playGround[new Point(x, y)]);
            playGround.Dirty = false;
            display1.Update();
            display2.Update();
            console.SetCursorPosition(0, 20);
        }

        /// <summary>
        /// Updates the score.
        /// </summary>
        /// <param name="e">The e.</param>
        public static void UpdateScore((int,int) e)
        {
            console.SetCursorPosition(20, 12);
            console.Write($"Score: {e.Item1}");
            console.SetCursorPosition(20, 13);
            console.Write($"Level: {e.Item2}");
            console.SetCursorPosition(0, 20);
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
                    console.Beep(1000, 10);
                    break;
                case GameSound.DeepBoom:
                    console.Beep(300, 20);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Checks the user action.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool CheckUserAction()
        {
            bool result = false;
            while (console.KeyAvailable)
            {
                var ch = console.ReadKey()?.KeyChar ?? '\x00';

                ch = Char.ToUpper(ch);
                if (keyAction.ContainsKey(ch))
                {
                    result |= userAction?.Invoke(keyAction[ch]) ?? false;
                    Update();
                }
            }
            return result;
        }
        #endregion

    }
}
