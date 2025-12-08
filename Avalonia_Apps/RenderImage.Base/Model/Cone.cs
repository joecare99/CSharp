using MathLibrary.RenderImage;
using System;

namespace RenderImage.Base.Model
{
    public sealed class Cone : Cylinder
    {
        private readonly double _radius2;
        private readonly RenderPoint _peak;

        public Cone(RenderPoint position, RenderPoint endPosition, double radius, RenderColor baseColor)
        : this(position, endPosition, radius, 0, baseColor, new TFTriple { X = 0.4, Y = 0.6, Z = 0.0 }) { }

        public Cone(RenderPoint position, RenderPoint endPosition, double radius, RenderColor baseColor, TFTriple surface)
        : this(position, endPosition, radius, 0, baseColor, surface) { }

        public Cone(RenderPoint position, RenderPoint endPosition, double radius, double radius2, RenderColor baseColor)
        : this(position, endPosition, radius, radius2, baseColor, new TFTriple { X = 0.4, Y = 0.6, Z = 0.0 }) { }

        public Cone(RenderPoint position, RenderPoint endPosition, double radius, double radius2, RenderColor baseColor, TFTriple surface)
        : base(position, endPosition, Math.Max(radius, radius2), baseColor, surface)
        {
            _radius2 = Math.Min(radius, radius2);
            if (Math.Abs(_radius - _radius2) < 1e-8)
            {
                _peak = Position;
            }
            else
            {
                _peak = new RenderPoint { Value = Position.Value + _innerVec * (1 + 0.5 * _radius2) / (_radius - _radius2) };
            }
        }

        public override bool HitTest(RenderRay ray, out HitData hit)
        {
            // Fallback to cylinder if equal radii
            if (Math.Abs(_radius - _radius2) < 1e-8)
                return base.HitTest(ray, out hit);

            hit = default;
            var startVec = ray.StartPoint.Value - Position.Value;
            var orth = ray.Direction.Value.XMul(_innerVec);
            var spDist = startVec * _innerVec / (_halfHeight * _halfHeight);
            var inside = Math.Abs(spDist) < 1.0;
            if (inside)
            {
                var spFoot = Position.Value - spDist * _innerVec;
                inside = (ray.StartPoint.Value - spFoot).GLen() < (_radius * (0.5 - spDist * 0.5) + _radius2 * (0.5 + spDist * 0.5));
            }
            var sgn = Math.Sign(_innerVec * ray.Direction.Value);
            var fdir = inside ? sgn : -sgn;
            var footVec = -startVec + _innerVec * fdir;
            if (orth.MLen() != 0)
            {
                var lDist = (orth * footVec) / orth.GLen();
                if (!inside && Math.Abs(lDist) > _radius) return false;
            }
            var footInner = footVec * _innerVec;
            if (Math.Sign(footInner) == sgn && sgn != 0)
            {
                var footDistance = footInner / (_innerVec * ray.Direction.Value);
                var footHit = ray.RayPoint(footDistance);
                if ((footHit.Value - ray.StartPoint.Value - footVec).GLen() <= (_radius * (0.5 - fdir * 0.5) + _radius2 * (0.5 + fdir * 0.5)))
                {
                    hit.Distance = footDistance;
                    hit.HitPoint = footHit;
                    hit.Normalvec = _innerVec / _halfHeight * (-sgn);
                    return true;
                }
            }
            if (orth.MLen() == 0) return false;
            var ortoDir = ray.Direction.Value - ray.Direction.Value * _innerVec / (_halfHeight * _halfHeight) * _innerVec;
            var lDist2 = (orth * footVec) / orth.GLen();
            var mDistance = (ortoDir * footVec) / TFTriple.Sqr(ortoDir) - Math.Sqrt(_radius * _radius - lDist2 * lDist2) / ortoDir.GLen();
            if (mDistance >= 0 && (hit.Distance < 0 || mDistance < hit.Distance))
            {
                var mHit = ray.RayPoint(mDistance);
                var ivFak = (mHit.Value - Position.Value) * _innerVec / (_halfHeight * _halfHeight);
                if (Math.Abs(ivFak) <= 1.0)
                {
                    hit.Distance = mDistance;
                    hit.HitPoint = mHit;
                    hit.Normalvec = (mHit.Value - Position.Value - ivFak * _innerVec) / _radius;
                    hit.AmbientVal = _surface.X;
                    hit.ReflectionVal = _surface.Y;
                    hit.Refraction = _surface.Z;
                    return true;
                }
            }
            return false;
        }
    }
}
