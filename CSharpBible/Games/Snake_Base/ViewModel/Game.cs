using Snake_Base.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake_Base.ViewModel
{
    /// <summary>
    /// Class Game.
    /// </summary>
    public class Game
    {
        #region Properties
        #region private Properties
        private static Random? _rnd;
        #endregion
        /// <summary>
        /// Gets or sets the playfield.
        /// </summary>
        /// <value>The playfield.</value>
        public Playfield2D<SnakeGameObject> Playfield { get; set; } = new Playfield2D<SnakeGameObject>();
        /// <summary>
        /// Gets or sets the snake.
        /// </summary>
        /// <value>The snake.</value>
        public Snake Snake { get; set; }
        /// <summary>
        /// The get next random
        /// </summary>
        public Func<int, int> GetNextRnd = (i) => (_rnd ?? (_rnd = new Random())).Next(i);
        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value><c>true</c> if this instance is running; otherwise, <c>false</c>.</value>
        public bool IsRunning => Snake.alive;
        #endregion

        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class.
        /// </summary>
        public Game()
        {
            Playfield.PfSize = new Size(20, 20);
            Playfield.OnDataChanged += PfDataChange;
            Snake = new Snake(new Point(10,10),Playfield);
        }

        private void PfDataChange(object? sender, (string prop, object? oldVal, object? newVal) e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Games the step.
        /// </summary>
        /// <returns>System.Int32.</returns>
        public int GameStep()
        {

            return 50;
        }
        #endregion
    }
}
