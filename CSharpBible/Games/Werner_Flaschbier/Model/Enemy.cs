// ***********************************************************************
// Assembly         : Werner_Flaschbier_Base
// Author           : Mir
// Created          : 08-01-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Enemy.cs" company="Werner_Flaschbier_Base">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace Werner_Flaschbier_Base.Model
{
    /// <summary>
    /// Class Enemy.
    /// Implements the <see cref="Werner_Flaschbier_Base.Model.PlayObject" />
    /// </summary>
    /// <seealso cref="Werner_Flaschbier_Base.Model.PlayObject" />
    public class Enemy : PlayObject
    {
        /// <summary>
        /// The direction
        /// </summary>
        private Direction _direction = Direction.Up;
        /// <summary>
        /// Gets the direction.
        /// </summary>
        /// <value>The direction.</value>
        public Direction direction => _direction;

        /// <summary>
        /// Initializes a new instance of the <see cref="Enemy" /> class.
        /// </summary>
        /// <param name="aField">a field.</param>
        public Enemy(Field? aField) : base(aField)
        {
//            _direction = Direction.Up;
        }

        /// <summary>
        /// Tests if the object can move in the given direction.
        /// </summary>
        /// <param name="dir">The directon to test</param>
        /// <returns>true: if the object can move in the direction</returns>
        public override bool TestMove(Direction? dir = null)
        {
            if (dir == null) dir = _direction;
            var result = false;

            var dest = field?.Parent?[Offsets.DirOffset(dir, Place)];
            if (dest is Space s )
            {
                result = s.Item == null || s.Item is Player;
            }
            return result;
        }

        /// <summary>
        /// Tries to move the object in the given direction.
        /// </summary>
        /// <param name="dir">The directon to move</param>
        /// <returns>true: if the object has moveed in the direction</returns>
        public override bool TryMove(Direction? dir = null)
        {
            var result = false;
            Handled = true;
            if (dir == null) dir = _direction;
            foreach (int dd in new int[] { 0, 3, 1, 2 })
            {
                var dir2 = (Direction)(((int)dir + dd) % 4);
                if (field?.Parent?[Offsets.DirOffset(dir2, Place)] is Space s && s is not Destination)
                {
                    result = s.Item == null || s.Item is Player || (s.Item is Enemy e && !e.Handled && e.TryMove());
                    if (s.Item == null)
                    {
                        s.Item = this;
                        _direction = dir2;
                    }
                    else if (s.Item is Player p)
                        p.IsAlive = false;
                    if (result)
                      break;
                }
            }
            
            return result;
        }
    }
}
