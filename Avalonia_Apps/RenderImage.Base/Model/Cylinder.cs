using MathLibrary.RenderImage;
using System;

namespace RenderImage.Base.Model
{
    public class Cylinder : SimpleObject
    {
        protected double _radius;
        protected TFTriple _innerVec; // from center to end *0.5
        protected double _halfHeight;

        public Cylinder(RenderPoint position, RenderPoint endPosition, double radius, RenderColor baseColor)
        : this(position, endPosition, radius, baseColor, new TFTriple { X = 0.6, Y = 0.4, Z = 0.0 }) { }

        public Cylinder(RenderPoint position, RenderPoint endPosition, double radius, RenderColor baseColor, TFTriple surface)
        : base(new RenderPoint { Value = (position.Value + endPosition.Value) * 0.5 }, baseColor, surface)
        {
            _innerVec = (endPosition.Value - position.Value) * 0.5;
            _radius = radius;
            _halfHeight = _innerVec.GLen();
            _boundary = new BoundaryCylinder(Position, new RenderPoint { Value = _innerVec }, _halfHeight, _radius);
        }

        public override bool HitTest(RenderRay ray, out HitData hit)
        {
            hit  = default;
            var orth = ray.Direction.Value.XMul(_innerVec / _halfHeight);
            var spDist = (Position.Value - ray.StartPoint.Value) * _innerVec / (_halfHeight * _halfHeight);
            var inside = Math.Abs(spDist) < 1.0;
            if (inside)
            {
                var spFoot = Position.Value - spDist * _innerVec;
                inside = (ray.StartPoint.Value - spFoot).GLen() < _radius;
            }
            var sgn = Math.Sign(_innerVec * ray.Direction.Value);
            var footVec = inside ? Position.Value - ray.StartPoint.Value + _innerVec * sgn : Position.Value - ray.StartPoint.Value - _innerVec * sgn;
            if (orth.MLen() != 0)
            {
                var lDist = (orth * footVec) / orth.GLen();
                if (Math.Abs(lDist) > _radius) return false;
            }
            var footInner = footVec * _innerVec;
            if (Math.Sign(footInner) == sgn && sgn != 0)
            {
                var footDistance = footInner / (_innerVec * ray.Direction.Value);
                var footHit = ray.RayPoint(footDistance);
                if ((footHit.Value - ray.StartPoint.Value - footVec).GLen() <= _radius)
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
