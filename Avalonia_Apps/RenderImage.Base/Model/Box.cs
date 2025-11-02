using MathLibrary.RenderImage;
using System;

namespace RenderImage.Base.Model
{
    public sealed class Box : SimpleObject
    {
        private readonly TFTriple _size;

        public Box(RenderPoint position, RenderVector size, RenderColor baseColor)
        : this(position, size, baseColor, new TFTriple { X = 0.6, Y = 0.4, Z = 0.0 }) { }

        public Box(RenderPoint position, RenderVector size, RenderColor baseColor, TFTriple surface)
        : base(position, baseColor, surface)
        {
            _size = size.Value;
            _boundary = new BoundaryBox(position, size);
        }

        public override bool HitTest(RenderRay ray, out HitData hit)
        {
            hit = default;
            var lDist = Position.Value - ray.StartPoint.Value;
            var inside = Math.Abs(lDist.X) < _size.X * 0.5 && Math.Abs(lDist.Y) < _size.Y * 0.5 && Math.Abs(lDist.Z) < _size.Z * 0.5;
            if (!inside && (((lDist.X - _size.X * 0.5 > 0) && (ray.Direction.Value.X <= 0)) ||
            ((lDist.X + _size.X * 0.5 < 0) && (ray.Direction.Value.X >= 0)) ||
            ((lDist.Y - _size.Y * 0.5 > 0) && (ray.Direction.Value.Y <= 0)) ||
            ((lDist.Y + _size.Y * 0.5 < 0) && (ray.Direction.Value.Y >= 0)) ||
            ((lDist.Z - _size.Z * 0.5 > 0) && (ray.Direction.Value.Z <= 0)) ||
            ((lDist.Z + _size.Z * 0.5 < 0) && (ray.Direction.Value.Z >= 0))))
            { return false; }
            hit.AmbientVal = _surface.X;
            hit.ReflectionVal = _surface.Y;
            hit.Refraction = _surface.Z;
            // Z faces
            if (Math.Abs(ray.Direction.Value.Z) > 1e-12)
            {
                var normal = new TFTriple { X = 0, Y = 0, Z = -Math.Sign(ray.Direction.Value.Z) };
                if (inside)
                    hit.Distance = (lDist.Z - normal.Z * 0.5 * _size.Z) / ray.Direction.Value.Z;
                else
                    hit.Distance = (lDist.Z + normal.Z * 0.5 * _size.Z) / ray.Direction.Value.Z;
                hit.HitPoint = ray.RayPoint(hit.Distance);
                var tp = hit.HitPoint.Value - Position.Value;
                if (Math.Abs(tp.X) <= _size.X * 0.5 && Math.Abs(tp.Y) <= _size.Y * 0.5)
                { hit.Normalvec = normal; return true; }
            }
            // Y faces
            if (Math.Abs(ray.Direction.Value.Y) > 1e-12)
            {
                var normal = new TFTriple { X = 0, Y = -Math.Sign(ray.Direction.Value.Y), Z = 0 };
                if (inside)
                    hit.Distance = (lDist.Y - normal.Y * 0.5 * _size.Y) / ray.Direction.Value.Y;
                else
                    hit.Distance = (lDist.Y + normal.Y * 0.5 * _size.Y) / ray.Direction.Value.Y;
                hit.HitPoint = ray.RayPoint(hit.Distance);
                var tp = hit.HitPoint.Value - Position.Value;
                if (Math.Abs(tp.X) <= _size.X * 0.5 && Math.Abs(tp.Z) <= _size.Z * 0.5)
                { hit.Normalvec = normal; return true; }
            }
            // X faces
            if (Math.Abs(ray.Direction.Value.X) > 1e-12)
            {
                var normal = new TFTriple { X = -Math.Sign(ray.Direction.Value.X), Y = 0, Z = 0 };
                if (inside)
                    hit.Distance = (lDist.X - normal.X * 0.5 * _size.X) / ray.Direction.Value.X;
                else
                    hit.Distance = (lDist.X + normal.X * 0.5 * _size.X) / ray.Direction.Value.X;
                hit.HitPoint = ray.RayPoint(hit.Distance);
                var tp = hit.HitPoint.Value - Position.Value;
                if (Math.Abs(tp.Y) <= _size.Y * 0.5 && Math.Abs(tp.Z) <= _size.Z * 0.5)
                { hit.Normalvec = normal; return true; }
            }
            hit.Distance = -1.0;
            return false;
        }
    }
}
