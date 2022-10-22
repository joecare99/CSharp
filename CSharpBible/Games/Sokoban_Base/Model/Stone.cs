// ***********************************************************************
// Assembly         : Sokoban_Base
// Author           : Mir
// Created          : 07-09-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Stone.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace Sokoban_Base.Model
{
    /// <summary>
    /// Class Stone.
    /// Implements the <see cref="Sokoban_Base.Model.PlayObject" />
    /// </summary>
    /// <seealso cref="Sokoban_Base.Model.PlayObject" />
    public class Stone : PlayObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Stone" /> class.
        /// </summary>
        /// <param name="aField">a field.</param>
        public Stone(Field? aField) : base(aField)
        {
        }

        /// <summary>
        /// Tests if the object can move in the given direction.
        /// </summary>
        /// <param name="dir">The directon to test</param>
        /// <returns>true: if the object can move in the direction</returns>
        public override bool TestMove(Direction dir)
        {
            var result = false;
            var dest = field?.Parent?[Offsets.DirOffset(dir, Position)];
            if (dest is Floor s && s.Item == null)
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Tries to move the object in the given direction.
        /// </summary>
        /// <param name="dir">The directon to move</param>
        /// <returns>true: if the object has moveed in the direction</returns>
        public override bool TryMove(Direction dir)
        {
            var newfield = field?.Parent?[Offsets.DirOffset(dir, Position)];
            if (newfield == null || !(newfield is Floor f)) return false;
            else
            {
                if (f.Item == null) { f.Item = this; return true; }
                else
                    return false;
            }
        }
    }
}
