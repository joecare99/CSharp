using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using VectorGfx.Models.Interfaces;

namespace VectorGfx.Models.Interfaces
{
    public interface IVfxGameObject: IGameObject
    {
        public PointF Position { get; set; }
        public PointF Destination { get; set; }
        public float Rotation { get; set; }

        public bool CollisionTest(IVfxGameObject other);
    }
}