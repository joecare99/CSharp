using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake_Base.Model
{
    /// <summary>
    /// Class Apple.
    /// Implements the <see cref="Snake_Base.Model.SnakeGameObject" />
    /// </summary>
    /// <seealso cref="Snake_Base.Model.SnakeGameObject" />
    public class Apple : SnakeGameObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Apple"/> class.
        /// </summary>
        public Apple():base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Apple"/> class.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="plf">The PLF.</param>
        public Apple(Point point, Playfield2D<SnakeGameObject>? plf = null) : base(point, plf)
        {
        }
    }
}
