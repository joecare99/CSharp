using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris_Base.Model
{
    /// <summary>
    /// Enum UserAction
    /// </summary>
    public enum UserAction
    {
        /// <summary>
        /// The none
        /// </summary>
        None,
        /// <summary>
        /// The move left
        /// </summary>
        MoveLeft,
        /// <summary>
        /// The move right
        /// </summary>
        MoveRight,
        /// <summary>
        /// The move down
        /// </summary>
        MoveDown,
        /// <summary>
        /// The rotate left
        /// </summary>
        RotateLeft,
        /// <summary>
        /// The rotate right
        /// </summary>
        RotateRight,
        /// <summary>
        /// The drop
        /// </summary>
        Drop,
        /// <summary>
        /// The quit
        /// </summary>
        Quit,
        /// <summary>
        /// The help
        /// </summary>
        Help,
        /// <summary>
        /// The restart
        /// </summary>
        Restart
    }

    /// <summary>
    /// Enum GameSound
    /// </summary>
    public enum GameSound 
    {
        /// <summary>
        /// The no sound
        /// </summary>
        NoSound,
        /// <summary>
        /// The deep boom
        /// </summary>
        DeepBoom,
        /// <summary>
        /// The tick
        /// </summary>
        Tick,
    }

    /// <summary>
    /// Class Game.
    /// </summary>
    public class Game
    {
        #region Properties

        #region ClassProperties
        private static Random rnd = new Random();
        /// <summary>
        /// Gets or sets the random int.
        /// </summary>
        /// <value>The random int.</value>
        public static Func<int, int>? rndInt { get; set; } = (i) => rnd.Next(i);
        private static int GetRndInt(int iMax) => rndInt?.Invoke(iMax) ?? 0;
        #endregion

        /// <summary>
        /// The play field
        /// </summary>
        public PlayField playField;
        private int removeLines;

        /// <summary>
        /// Gets the score.
        /// </summary>
        /// <value>The score.</value>
        public int score { get; private set; }
        /// <summary>
        /// Gets a value indicating whether [not ended].
        /// </summary>
        /// <value><c>true</c> if [not ended]; otherwise, <c>false</c>.</value>
        public bool notEnded { get; private set; } = true;
        /// <summary>
        /// Gets the level.
        /// </summary>
        /// <value>The level.</value>
        public int level { get; private set; } = 1;


        #region Actions;
        /// <summary>
        /// Gets or sets the sound.
        /// </summary>
        /// <value>The sound.</value>
        public Action<GameSound>? Sound { get; set; }
        /// <summary>
        /// Occurs when [v update].
        /// </summary>
        public event EventHandler VUpdate;
        /// <summary>
        /// Occurs when [update score].
        /// </summary>
        public event EventHandler<(int,int)> UpdateScore;
        #endregion

        #endregion

        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class.
        /// </summary>
        public Game()
        {
            playField = new PlayField();
            score = 0;
            removeLines = 0;
            notEnded = true;
        }

        /// <summary>
        /// Games the step.
        /// </summary>
        /// <param name="wait">The wait.</param>
        public void GameStep(out int wait)
        {
            wait = 30000 / ((level+10)*2);
            if (playField == null) return;
            if (playField.NextBlock == null)
            {
                playField.NextBlock = new Block((BlockType)GetRndInt(typeof(BlockType).GetEnumValues().Length));
            }
            Block act;
            if (playField.ActualBlock == null)
            {
                act = playField.ActualBlock = playField.NextBlock;
                playField.NextBlock = new Block((BlockType)GetRndInt(typeof(BlockType).GetEnumValues().Length));
                act.Position = new Point(6,0);
                act.Show();
                Sound?.Invoke(GameSound.Tick);
                return;
            }
            else act = playField.ActualBlock;

            Point Ofs = new Point(0, 1);
            if (!act.CollisionTest(Ofs, act.ActBlockAngle))
            {
                Sound?.Invoke(GameSound.Tick);
                act.Move(Ofs);
            }
            else
            {
                Sound?.Invoke(GameSound.DeepBoom);
                score += (level+10)/10;
                playField.ActualBlock = null;
                var r = playField.TestRemoveLine();
                removeLines += r;    
                score += (r*r) * level*10;
                level = (1 + (int)Math.Floor(Math.Sqrt(removeLines*2)));
                UpdateScore?.Invoke(this, (score,level));
                wait = 0;
            }
            
        }

        /// <summary>
        /// Does the user action.
        /// </summary>
        /// <param name="uAction">The u action.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool DoUserAction(UserAction uAction)
        {
            if (playField.ActualBlock == null) return false;
            bool result = false;
            var act = playField.ActualBlock;
            Point Ofs;
            BlockAngle newAngle;
            switch (uAction)
            {
                case UserAction.MoveLeft:
                    Ofs = new Point(-1, 0);
                    if (!act.CollisionTest(Ofs, act.ActBlockAngle))
                        act.Move(Ofs);
                    break;
                case UserAction.MoveRight:
                    Ofs = new Point(1, 0);
                    if (!act.CollisionTest(Ofs, act.ActBlockAngle))
                        act.Move(Ofs);
                    break;
                case UserAction.MoveDown:
                    Ofs = new Point(0, 1);
                    if (!act.CollisionTest(Ofs, act.ActBlockAngle))
                        act.Move(Ofs);
                    result = true;
                    break;
                case UserAction.RotateLeft:
                    newAngle = (BlockAngle)(((int)act.ActBlockAngle + 1) % 4);
                    if (!act.CollisionTest(Point.Empty, newAngle))
                        act.Move(Point.Empty, newAngle);
                    break;
                case UserAction.RotateRight:
                    newAngle = (BlockAngle)(((int)act.ActBlockAngle + 3) % 4);
                    if (!act.CollisionTest(Point.Empty, newAngle))
                        act.Move(Point.Empty, newAngle);
                    break;
                case UserAction.Drop:
                    Ofs = new Point(0, 1);
                    while (!act.CollisionTest(Ofs, act.ActBlockAngle))
                    {
                        act.Move(Ofs);
                        score += (level + 10) / 10;
                    }
                    result = true;
                    break;
                case UserAction.Quit:
                    notEnded = false;
                    break;
                case UserAction.None:
                    break;
                case UserAction.Help:
                    break;
                case UserAction.Restart:
                    playField.Clear();
                    removeLines = 0;
                    score = 0;
                    level = 1;
                    UpdateScore?.Invoke(this, (score, level));
                    result = true;
                    break;
            }
            return result;
        }
        #endregion

    }
}
