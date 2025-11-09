using MathLibrary.RenderImage;
using System;

namespace RenderImage.Base.Model
{
    public sealed class BoundaryHollowCylinder : BoundaryCylinder
    {
        private double _radiusInner;

        public BoundaryHollowCylinder(RenderPoint position, RenderPoint normal, double height, double radiusOuter, double radiusInner)
        : base(position, normal, height, radiusOuter)
        {
            _radiusInner = radiusInner;
        }

        public override bool BoundaryTest(RenderRay ray, out double distance)
        {
            var okOuter = base.BoundaryTest(ray, out distance);
            if (distance == 0.0)
            {
                var sgn = Math.Sign(_normal * ray.Direction.Value);
                var startVec = ray.StartPoint.Value - Position.Value;
                var footVec = _normal * (-sgn) * _halfHeight - startVec;
                var spDist = -startVec * _normal * _halfHeight;
                if (Math.Abs(spDist) < _halfHeight && (footVec - spDist * _normal).GLen() > _radiusInner)
                {
                    distance = 0.0;
                    return true;
                }
                var footInnerV = footVec * _normal;
                if (Math.Sign(footInnerV) == sgn && sgn != 0)
                {
                    distance = footInnerV / (_normal * ray.Direction.Value);
                    if ((ray.Direction.Value * distance - footVec).GLen() <= _radius)
                        return true;
                }
                var ortoDir = ray.Direction.Value - (ray.Direction.Value * _normal) * _normal;
                var lDist = Math.Sqrt(TFTriple.Sqr((startVec + spDist * _normal)) - Math.Pow((ortoDir * startVec) / ortoDir.GLen(), 2));
                distance = (ortoDir * footVec) / TFTriple.Sqr(ortoDir) - Math.Sqrt(_radius * _radius - lDist * lDist) / ortoDir.GLen();
                if (distance >= 0 && Math.Abs((ray.Direction.Value * distance + startVec) * _normal) <= _halfHeight)
                    return true;
            }
            return okOuter;
        }
    }
}
