// ***********************************************************************
// Assembly         : Werner_Flaschbier_Base
// Author           : Mir
// Created          : 08-02-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="WernerGame.cs" company="Werner_Flaschbier_Base">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Drawing;

namespace Werner_Flaschbier_Base.Model
{
    /// <summary>
    /// Class WernerGame.
    /// </summary>
    public class WernerGame : IWernerGame 
    {
        #region Properties
        #region private Properties & Fields

        #endregion
        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <value>The size.</value>
        public Size size => playfield.size;
        /// <summary>
        /// Gets the lives.
        /// </summary>
        /// <value>The lives.</value>
        public int Lives { get; private set; }
        /// <summary>
        /// Gets the time left.
        /// </summary>
        /// <value>The time left.</value>
        public float TimeLeft { get; private set; }
        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value><c>true</c> if this instance is running; otherwise, <c>false</c>.</value>
        public bool isRunning { get => !_userQuit && (Lives > 0); }

        /// <summary>
        /// Occurs when [vis update].
        /// </summary>
        public event EventHandler<bool>? visUpdate;
        /// <summary>
        /// Occurs when [vis full redraw].
        /// </summary>
        public event EventHandler? visFullRedraw;
        /// <summary>
        /// Occurs when [vis show help].
        /// </summary>
        public event EventHandler? visShowHelp;

        /// <summary>
        /// Gets the <see cref="Tiles" /> with the specified p.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns>Tiles.</returns>
        public Enum this[Point p]=>GetTile(p);
        /// <summary>
        /// The level
        /// </summary>
        private int level = 0;
        /// <summary>
        /// The maximum lives
        /// </summary>
        private readonly int maxLives = 10;
        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>The score.</value>
        public int Score { get; set; } = 0;

        public int MaxLives => maxLives;

        public int Level { get => level; set => level = value; }

        #region private Properties
        /// <summary>
        /// The playfield
        /// </summary>
        private readonly Playfield playfield = new();
        /// <summary>
        /// The half step
        /// </summary>
        private bool halfStep=false;
        /// <summary>
        /// The act dir
        /// </summary>
        private Direction? actDir;
        /// <summary>
        /// The next dir
        /// </summary>
        private Direction? nextDir;
        /// <summary>
        /// The next dir2
        /// </summary>
        private Direction? nextDir2;
        /// <summary>
        /// The user quit
        /// </summary>
        private bool _userQuit=false;
        #endregion
        #endregion

        #region Methods
        /// <summary>
        /// Gets the tile.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns>Tiles.</returns>
        public Tiles GetTile(Point p)
        {
            var field = playfield[p];
            Tiles result;
            switch (field?.fieldDef)
            {
                case FieldDef.Enemy:
                    result = Tiles.Enemy_Up;
                    if (field?.Item is Enemy e)
                        result = (Tiles)((int)result + (int)(e.direction));
                    break;
                case FieldDef.Wall:
                    int w = 0;
                    result = Tiles.Wall;
                    for (var d = (Direction)0; (int)d < 4; d++)
                        w += ((playfield[Offsets.DirOffset(d, p)] is Wall) ? 1 << (int)d : 0);
                    if (w > 0)
                        result = (Tiles)(w - 1 + (int)Tiles.Wall_U);
                    break;
                case FieldDef.Player:
                    if ((field.Item is Player plr) && (plr.IsAlive))
                        result = Tiles.Player;
                    else
                        result = Tiles.PlayerDead;
                    break;
                case FieldDef.Stone:
                    if ((field.Item is Stone stn) && (stn.Place == stn.OldPosition))
                        result = Tiles.Stone;
                    else
                        result = Tiles.StoneMoving;
                    break;
                default:
                    result = (Tiles?)field?.fieldDef ?? Tiles.Empty;
                    break;
            }
            return result;
        }

        /// <summary>
        /// Olds the position.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns>Point.</returns>
        public Point OldPos(Point p)
        {
            var field = playfield[p];
            if (field is Space s && s.Item != null)
                return s.Item.OldPosition;
            else
                return p;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WernerGame" /> class.
        /// </summary>
        public WernerGame()
        {
            Setup();
            Lives = maxLives;
        }

        /// <summary>
        /// Setups the specified new level.
        /// </summary>
        /// <param name="newLevel">The new level.</param>
        public void Setup(int? newLevel=null)
        {
            Level = newLevel ?? Level;
            playfield.Setup(LevelDefs.GetLevel(Level));
            TimeLeft = 99f;
            visFullRedraw?.Invoke(this,new EventArgs());
        }

        public void HandleGameLogic()
        {
            if (!playfield.player?.IsAlive ?? false && Lives > 0)
            {
                Lives--;
                if (Lives > 0) Setup();
            }
            if (playfield.player?.field is Destination)
            {
                Score += (int)TimeLeft;
                Setup(Level + 1);
            }
        }

        public void MovePlayer(Direction dir)
        {
            if (actDir == null) actDir = dir;
            else if (nextDir == null) nextDir = dir;
            else if (nextDir2 == null) nextDir2 = dir;
            else { actDir = nextDir; nextDir = nextDir2; nextDir2 = dir; }
        }

        public void PrevLvl() => Setup(Math.Max(Level - 1, 0));

        public void NextLvl() => Setup(Level + 1);

        public void ReqRestart()
        {
            Lives--;
            if (Lives > 0) Setup();
        }

        public void ReqHelp() => visShowHelp?.Invoke(this, new EventArgs());

        public void ReqQuit() => _userQuit = true;

        /// <summary>
        /// Games the step.
        /// </summary>
        /// <returns>System.Int32.</returns>
        public int GameStep()
        {
            halfStep = !halfStep;
            if (TimeLeft > 0.1)
               TimeLeft -= 0.06f;
            else if (playfield.player!=null)
                playfield.player.IsAlive = false;
            if (halfStep)
            {
                foreach (PlayObject po in playfield.ActiveObjects)
                    po.Handled = false;
                foreach (Space sp in playfield.Spaces)
                    sp.OldItem = sp.Item;
                playfield.player?.TryMove(actDir);
                actDir = nextDir;
                nextDir = nextDir2;
                nextDir2 = null;
                foreach (PlayObject po in playfield.ActiveObjects)
                    if (!po.Handled)
                {
                        if (po is not Player)
                            po.TryMove();
                }
            }

            visUpdate?.Invoke(this, halfStep);    
            
            return 60;
        }
        #endregion
    }
}
