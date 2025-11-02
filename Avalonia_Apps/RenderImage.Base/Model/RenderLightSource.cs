using MathLibrary.RenderImage;

namespace RenderImage.Base.Model
{
    public class RenderLightSource : RenderBaseObject
    {
        public RenderLightSource(RenderPoint position)
        {
            Position = position;
        }

        public virtual RenderColor ProjectedColor(RenderVector direction) => new RenderColor { Red = 1, Green = 1, Blue = 1 };

        public virtual double FalloffIntensity(RenderVector direction)
        {
            var d = direction.Value.GLen();
            return 1.0 / (d * d);
        }

        public virtual double MaxIntensity(RenderVector direction)
        {
            var d = direction.Value.GLen();
            return 1.0 / (d * d);
        }

        public override bool BoundaryTest(RenderRay aRay, out double distance)
        {
            distance = -1.0;
            return false;
        }

        public override bool HitTest(RenderRay aRay, out HitData hitData)
        {
            hitData = default;
            return false;
        }
    }
}
