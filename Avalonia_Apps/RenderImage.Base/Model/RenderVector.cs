using MathLibrary.RenderImage;

namespace RenderImage.Base.Model
{
    public struct RenderVector
    {
        public TFTriple Value;
        public static implicit operator TFTriple(RenderVector v) => v.Value;
        public static implicit operator RenderVector(TFTriple v) => new() { Value = v };
        public override string ToString() => Value.ToString();
    }
}
