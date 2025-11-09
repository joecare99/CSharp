using MathLibrary.RenderImage;
using System;

namespace RenderImage.Base.Model
{
    public class BoundaryCylinder : RenderBoundary
    {
        protected TFTriple _normal;
        protected double _halfHeight;
        protected double _radius;

        public BoundaryCylinder(RenderPoint position, RenderPoint normal, double height, double radius) : base(position)
        {
            _normal = normal.Value / normal.Value.GLen();
            _halfHeight = height;
            _radius = radius;
        }

        public override bool BoundaryTest(RenderRay ray, out double distance)
        {
            var sgn = Math.Sign(_normal * ray.Direction.Value);
            var startVec = ray.StartPoint.Value - Position.Value;
            var footVec = _normal * (-sgn) * _halfHeight - startVec;
            var spDist = -startVec * _normal;
            if (Math.Abs(spDist) < _halfHeight && (startVec + spDist * _normal).GLen() < _radius)
            {
                distance = 0.0;
                return true;
            }
            // outside definitively?
            var orth = ray.Direction.Value.XMul(_normal);
            if (orth.MLen() != 0)
            {
                var lDist = (orth * footVec) / orth.GLen();
                distance = -1.0;
                if (Math.Abs(lDist) > _radius)
                {
                    return false;
                }
            }
            var footInnerV = footVec * _normal;
            if (Math.Sign(footInnerV) == sgn && sgn != 0)
            {
                distance = footInnerV / (_normal * ray.Direction.Value);
                if ((ray.Direction.Value * distance - footVec).GLen() <= _radius)
                    return true;
            }
            distance = -1.0;
            if (orth.MLen() == 0)
                return false;
            var ortoDir = ray.Direction.Value - (ray.Direction.Value * _normal) * _normal;
            // nearest side hit distance along ray on lateral surface
            var lDist2 = (orth * footVec) / orth.GLen();
            distance = (ortoDir * footVec) / TFTriple.Sqr(ortoDir) - Math.Sqrt(_radius * _radius - lDist2 * lDist2) / ortoDir.GLen();
            var hitVec = ray.Direction.Value * distance + startVec;
            if (distance >= 0 && Math.Abs(hitVec * _normal) <= _halfHeight)
                return true;
            distance = -1;
            return false;
        }
    }
}
