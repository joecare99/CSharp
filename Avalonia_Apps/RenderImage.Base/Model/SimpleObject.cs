using MathLibrary.RenderImage;

namespace RenderImage.Base.Model
{
    public class SimpleObject : RenderBaseObject, IHasColor
    {
        protected RenderColor _baseColor;
        protected RenderBoundary? _boundary;
        protected TFTriple _surface;

        public SimpleObject(RenderPoint position, RenderColor baseColor, TFTriple surface)
        {
            Position = position;
            _baseColor = baseColor;
            _surface = surface;
        }

        public virtual RenderColor GetColorAt(RenderPoint point) => _baseColor;

        public override bool BoundaryTest(RenderRay ray, out double distance)
        {
            if (_boundary != null)
                return _boundary.BoundaryTest(ray, out distance);
            // Fallback like Pascal: use distance to center and direction alignment
            distance = (ray.StartPoint.Value - Position.Value).GLen();
            return (Position.Value - ray.StartPoint.Value) * ray.Direction.Value > distance * 0.5;
        }

        public override bool HitTest(RenderRay ray, out HitData hitData)
        {
            // abstract in Pascal; here keep default not hit
            hitData = default;
            return false;
        }

        public RenderColor this[RenderPoint point] => GetColorAt(point);
    }
}
