using MathLibrary.RenderImage;
using System;

namespace RenderImage.Base.Model
{
    public struct RenderRay
    {
        private TFTriple _direction;
        private TFTriple _startPoint;

        public static RenderRay Init(RenderPoint start, RenderVector dir)
        {
            var r = new RenderRay
            {
                _startPoint = start.Value,
                _direction = dir.Value
            };
            if (!r._direction.Equals(TFTriple.Zero))
                r._direction = r._direction / r._direction.GLen();
            return r;
        }

        public RenderPoint StartPoint
        {
            readonly get => new() { Value = _startPoint };
            set => SetStartPoint(value);
        }

        public RenderVector Direction
        {
            readonly get => new() { Value = _direction };
            set => SetDirection(value);
        }

        private void SetDirection(RenderVector value)
        {
            var v = value.Value;
            if (v.Equals(_direction)) return;
            _direction = v;
            if (!_direction.Equals(TFTriple.Zero))
                _direction = _direction / _direction.GLen();
        }

        private void SetStartPoint(RenderPoint value)
        {
            var v = value.Value;
            if (v.Equals(_startPoint)) return;
            _startPoint = v;
        }

        public RenderPoint RayPoint(double distance) => new() { Value = _startPoint + _direction * distance };

        public RenderVector ReflectDir(RenderVector normal) => new() { Value = _direction + (_direction * normal.Value) * (-2.0) * normal.Value };

        public double SqrDistance(RenderPoint point)
        {
            var l = point.Value - _startPoint;
            var dot = l * _direction;
            return TFTriple.Sqr(l) - dot * dot;
        }

        public double Distance(RenderPoint point) => Math.Sqrt(SqrDistance(point));
    }
}
