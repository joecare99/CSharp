using System;
using System.Diagnostics;
using System.Threading;
using Tetris_Base.Model;
using Tetris_Base.View;

namespace Tetris_Base
{
    /// <summary>
    /// Class Program.
    /// </summary>
    public class Program
    {
        #region Static_Fields
        /// <summary>
        /// The tetris
        /// </summary>
        public static Game tetris = new Game();
        private static Stopwatch timer = new Stopwatch();
        #endregion

        /// <summary>
        /// Gets a value indicating whether [game not ended].
        /// </summary>
        /// <value><c>true</c> if [game not ended]; otherwise, <c>false</c>.</value>
        public static bool gameNotEnded { get => tetris.notEnded; }
        /// <summary>
        /// Gets the play field.
        /// </summary>
        /// <value>The play field.</value>
        public static PlayField playField { get => tetris.playField; }

        /// <summary>
        /// Initializes static members of the <see cref="Program"/> class.
        /// </summary>
        static Program()
        {
        }

        #region Methods
        private static void Init()
        {
            Visual.Init();
            playField.PrevPaint = Visual.PrevPaint;
            Visual.playGround = playField.playGround;
            Visual.display1.PutLine(0, 0, 0, Visual.display1.dSize.Height - 1, ConsoleColor.White);
            Visual.display1.PutLine(Visual.display1.dSize.Width-1, 0, Visual.display1.dSize.Width - 1, Visual.display1.dSize.Height - 1, ConsoleColor.White);
            Visual.display1.PutLine(0,Visual.display1.dSize.Height - 1, Visual.display1.dSize.Width - 1, Visual.display1.dSize.Height - 1, ConsoleColor.White);
            tetris.VUpdate += new EventHandler((o,e)=>Visual.Update());
            Visual.userAction = tetris.DoUserAction;
            tetris.UpdateScore += new EventHandler<(int,int)>((o, e) => Visual.UpdateScore(e));
        }

        private static void Run()
        {
            int wait;
            
            while (gameNotEnded)
            {
                Visual.Update();
                tetris.GameStep(out wait);
                timer.Reset();
                timer.Start();
                while (gameNotEnded && timer.ElapsedMilliseconds < wait)
                {
                   if (Visual.CheckUserAction())
                    {
                        timer.Restart();
                    };                 
                    Thread.Sleep(1);
                } 
            }
        }

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            Init();
            Run();
        }
        #endregion
    }
}