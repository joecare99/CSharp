using System.Drawing;

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