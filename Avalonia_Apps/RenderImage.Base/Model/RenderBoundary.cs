using System;

namespace RenderImage.Base.Model
{
    public abstract class RenderBoundary : RenderBaseObject
    {
        protected RenderBoundary(RenderPoint position)
        {
            Position = position;
        }

        public override bool HitTest(RenderRay ray, out HitData hitData)
        {
            throw new NotSupportedException("HitTest should not be called in a Boundary-Object");
        }
    }
}
