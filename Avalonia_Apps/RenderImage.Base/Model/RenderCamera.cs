using MathLibrary.RenderImage;

namespace RenderImage.Base.Model
{
    public class RenderCamera : RenderBaseObject
    {
        private RenderPoint _lookAt;
        private TFTriple _right;
        private TFTriple _up;
        private TFTuple _resolution;

        public RenderPoint LookAt
        {
            get => _lookAt;
            set => SetLookAt(value);
        }

        public TFTuple Resolution
        {
            get => _resolution;
            set => SetResolution(value);
        }

        public RenderVector DefaultDirection
        {
            get => GetDefaultDirection();
            set => SetDefaultDirection(value);
        }

        public RenderVector this[IntPoint pnt] => GetRayDirection(pnt);

        protected virtual RenderVector GetRayDirection(IntPoint pnt)
        {
            // dir = norm(look-pos) + right*(x/res.x-0.5) + up*(y - res.y*0.5)/res.x
            var baseDir = (_lookAt.Value - Position.Value).Normalize();
            var dx = pnt.X / _resolution.X - 0.5;
            var dy = (pnt.Y - _resolution.Y * 0.5) / _resolution.X;
            var dir = baseDir + _right * dx + _up * dy;
            return new RenderVector { Value = dir };
        }

        public override bool BoundaryTest(RenderRay aRay, out double distance)
        {
            distance = -1.0; return false;
        }

        private void SetLookAt(RenderPoint value)
        {
            if (_lookAt.Value.Equals(value.Value)) return;
            _lookAt = value;
            _right = (_lookAt.Value - Position.Value).Normalize().XMul(new TFTriple { X = 0, Y = 0, Z = 1 });
            _up = (_lookAt.Value - Position.Value).Normalize().XMul(_right);
        }

        private RenderVector GetDefaultDirection()
        {
            return new RenderVector { Value = _lookAt.Value - Position.Value };
        }

        private void SetDefaultDirection(RenderVector value)
        {
            LookAt = new RenderPoint { Value = Position.Value + value.Value };
        }

        private void SetResolution(TFTuple value)
        {
            if (_resolution.Equals(value)) return;
            _resolution = value;
        }

        public override bool HitTest(RenderRay ray, out HitData hitData)
        {
           hitData = default;
           return false;
        }
    }
}
