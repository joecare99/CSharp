using MathLibrary.RenderImage;

namespace RenderImage.Base.Model
{
    public struct RenderPoint
    {
        public TFTriple Value;
        public static implicit operator TFTriple(RenderPoint p) => p.Value;
        public static implicit operator RenderPoint(TFTriple v) => new() { Value = v };
        public override string ToString() => Value.ToString();
    }
}
