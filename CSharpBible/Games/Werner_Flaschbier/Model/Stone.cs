// ***********************************************************************
// Assembly         : Werner_Flaschbier_Base
// Author           : Mir
// Created          : 07-31-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Stone.cs" company="Werner_Flaschbier_Base">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace Werner_Flaschbier_Base.Model
{
    /// <summary>
    /// Stones (Boulders) are Objects that fall if an <see cref="Empty" /> space
    /// is below them, or slip to the left or right if sitting on a
    /// <see cref="Wall" /> or another <see cref="Stone" /> if the places next
    /// to them and the place below that are <see cref="Empty" />
    /// </summary>
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
        public override bool TestMove(Direction? dir = null)
        {
            var result = false;

            var dest = field?.Parent?[Offsets.DirOffset(Direction.Down, Position)];
            if (dest is Space @s && @s.Item == null && dest is not Destination)
                result = true;
            else if (dest is Space @s2 && @s2.Item is Stone || dest is Wall)
            {
                var zwd= field?.Parent?[Offsets.DirOffset(Direction.West, Position)];
                var dest2= field?.Parent?[Offsets.DirOffset(Direction.WestDown, Position)];
                if (zwd is Space @zs && @zs.Item==null && @zs.OldItem == null && dest2 is Space @ds2 && @ds2.Item == null)
                    result =true;
                else
                {
                    zwd = field?.Parent?[Offsets.DirOffset(Direction.East, Position)];
                    dest2 = field?.Parent?[Offsets.DirOffset(Direction.EastDown, Position)];
                    if (zwd is Space @zs2 && @zs2.Item == null && @zs2.OldItem == null && dest2 is Space @ds3 && @ds3.Item == null)
                        result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// Tries to move the object in the given direction.
        /// </summary>
        /// <param name="dir">The direction to move</param>
        /// <returns>true: if the object has moved in the direction</returns>
        public override bool TryMove(Direction? dir=null)
        {
            Handled=true;
            var dest= field?.Parent?[Offsets.DirOffset(Direction.Down, Position)];
            if (dest is Space @s && dest is not Destination && (s.Item == null && s.OldItem == null || s.Item is Player))
                if (@s.Item == null)
                    @s.Item = this;
                else if (@s.Item is Player @p && (OldPosition != Position))
                    p.IsAlive = false;
                else { OldPosition = Position; return false; }
            else if (dest is Space @s2 && @s2.Item is Stone || dest is Wall)
            {
                var zwd = field?.Parent?[Offsets.DirOffset(Direction.West, Position)];
                var dest2 = field?.Parent?[Offsets.DirOffset(Direction.WestDown, Position)];
                if (zwd is Space @zs && zwd is not Destination && @zs.Item == null 
                    && dest2 is Space @ds2 && dest2 is not Destination && @ds2.Item == null && @ds2.OldItem == null)
                    @ds2.Item = this;
                else
                {
                    zwd = field?.Parent?[Offsets.DirOffset(Direction.East, Position)];
                    dest2 = field?.Parent?[Offsets.DirOffset(Direction.EastDown, Position)];
                    if (zwd is Space @zs2 && zwd is not Destination && @zs2.Item == null 
                        && dest2 is Space @ds3 && dest2 is not Destination && @ds3.Item == null && @ds3.OldItem == null)
                        @ds3.Item = this;
                    else
                    { OldPosition = Position; return false; }
                }
            }
            else
            { OldPosition = Position; return false; }
            return true;
        }
    }
}
