// ***********************************************************************
// Assembly         : Snake_Base
// Author           : Mir
// Created          : 08-25-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="SnakeGame.cs" company="JC-Soft">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using BaseLib.Models.Interfaces;
using Game_Base.Model;
using Snake_Base.Models.Data;
using Snake_Base.Models.Interfaces;
using System;
using System.Drawing;

/// <summary>
/// The ViewModel namespace.
/// </summary>
/// <autogeneratedoc />
namespace Snake_Base.Models
{
    /// <summary>
    /// Class SnakeGame.
    /// </summary>
    public class SnakeGame: ISnakeGame
    {
        #region Properties
        #region private Properties
        /// <summary>
        /// The random
        /// </summary>
        /// <autogeneratedoc />
        private static IRandom? _rnd;

        private Direction sDir;
        private bool _userQuit = false;
        private bool xHalfStep;

        #endregion
        /// <summary>
        /// Gets or sets the playfield.
        /// </summary>
        /// <value>The playfield.</value>
        public IPlayfield2D<ISnakeGameObject> Playfield { get; } 
        /// <summary>
        /// Gets or sets the snake.
        /// </summary>
        /// <value>The snake.</value>
        public Snake Snake { get; set; }
        /// <summary>
        /// The get next random
        /// </summary>
        public int Level { get; }
        public event EventHandler<bool>? visUpdate;
        public event EventHandler<EventArgs?>? visFullRedraw;
        public static readonly int MaxLives;

        /// <summary>
        /// Gets the <see cref="SnakeTiles" /> with the specified p.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns>SnakeTiles.</returns>
        public SnakeTiles this[Point p] => GetTile(p);

        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value><c>true</c> if this instance is running; otherwise, <c>false</c>.</value>
        public bool IsRunning => Snake.alive && !_userQuit;

        public int Score { get; private set; }
        public int Lives { get; private set; }
        public Size size { get => new Size(Playfield.PfSize.Width+2,Playfield.PfSize.Height+2);  }

        int ISnakeGame.MaxLives => MaxLives;

        public bool UserQuit { set => _userQuit=value; }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="SnakeGame" /> class.
        /// </summary>
        public SnakeGame(IRandom random, IPlayfield2D<ISnakeGameObject> playfield2D)
        {     
            _rnd = random;
            Playfield = playfield2D;
            Playfield.PfSize = new Size(20, 20);
            Playfield.OnDataChanged += PfDataChange;
            SnakeGameObject.DefaultParent = Playfield;
            Snake = new Snake(new Point(10,10));
            Snake.OnSnakeEatsApple += OnSnakeEatsApple;
        }

        private void OnSnakeEatsApple(object? sender, EventArgs e)
        {
            NewApple();
        }

        private void NewApple()
        {
            // Getplace for apple
            Point p;
            //   Get Random Startplace
            var w = Playfield.PfSize.Width;
            var sp = _rnd?.Next(Playfield.PfSize.Height * w)??0;
            while (Playfield[p=new Point(sp % w,sp / w)]!=null)
                sp++;
            //
            var a=new Apple(p, Playfield);
            a.ResetOldPlace();
        }

        /// <summary>
        /// Pfs the data change.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <autogeneratedoc />
        private void PfDataChange(object? sender, (string prop, object? oldVal, object? newVal) e)
        {
            //          throw new NotImplementedException();
        }

        /// <summary>
        /// Games the step.
        /// </summary>
        /// <returns>System.Int32.</returns>
        public int GameStep()
        {
            xHalfStep = !xHalfStep;
            if (xHalfStep)
            {
                foreach (var i in Playfield.Items)
                    i.ResetOldPlace();
                Snake.SnakeMove(sDir);
            }
            visUpdate?.Invoke(this, xHalfStep);
            return 150;
        }

        /// <summary>
        /// Gets the tile.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns>SnakeTiles.</returns>
        public SnakeTiles GetTile(Point p)
        {
            SnakeTiles result = SnakeTiles.Empty;
            var field = Playfield[p];
            switch(field)
            {
                case Apple:
                    result = SnakeTiles.Apple; break;
                case SnakeHead sh:
                    {
                        var np = Point.Subtract(sh.Place, (Size?)sh.NextPart?.Place??Size.Empty);
                        switch ((np.X, np.Y))
                        {
                            case (1, 0): result = SnakeTiles.SnakeHead_E; break;
                            case (-1, 0): result = SnakeTiles.SnakeHead_W; break;
                            case (0, 1): result = SnakeTiles.SnakeHead_S; break;
                            case (0, -1): result = SnakeTiles.SnakeHead_N; break;
                        }
                        break;
                    }
                case SnakeTail st: {
                        var np = Point.Subtract(st.Place, (Size?)st.PrevPart?.Place??Size.Empty);
                        switch ((np.X, np.Y))
                        {
                            case (1, 0): result = SnakeTiles.SnakeTail_W; break;
                            case (-1, 0): result = SnakeTiles.SnakeTail_E; break;
                            case (0, 1): result = SnakeTiles.SnakeTail_N; break;
                            case (0, -1): result = SnakeTiles.SnakeTail_S; break;
                        }
                        break; }
                case SnakeBodyPart sb: {
                        var nx = Point.Subtract(sb.Place, (Size?)sb.NextPart?.Place ?? Size.Empty);
                        var np = Point.Subtract(sb.Place, (Size?)sb.PrevPart?.Place ?? Size.Empty);
                        switch ((np.X+nx.X, np.Y+nx.Y))
                        {
                            case (0, 0) when nx.Y==0: result = SnakeTiles.SnakeBody_WE; break;
                            case (0, 0) when nx.X == 0: result = SnakeTiles.SnakeBody_NS; break;
                            case (1, 1): result = SnakeTiles.SnakeBody_NE; break;
                            case (-1, 1): result = SnakeTiles.SnakeBody_NW; break;
                            case (-1, -1): result = SnakeTiles.SnakeBody_SW; break;
                            case (1, -1): result = SnakeTiles.SnakeBody_SE; break;
                        }
                        break; 
                    }
                default:
                    if (Playfield.IsInside(p))
                        result = SnakeTiles.Empty;
                    else
                        result = SnakeTiles.Wall;
                    break;
            }
            return result;
        }

        public Point GetOldPos(Point arg)
        {
            return Playfield[arg]?.OldPlace ?? arg;
        }

        public void Setup(int v)
        {
            NewApple();
            //throw new NotImplementedException();
        }

        public void SetSnakeDir(Direction action)
        {
            sDir = action;
        }

        #endregion
    }
}
