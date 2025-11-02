using MathLibrary.RenderImage;

namespace RenderImage.Base.Model
{
    public abstract class RenderBaseObject
    {
        private int _id;
        private TFTriple _position;

        public int ID
        {
            get => _id;
            set => _id = value;
        }

        public RenderPoint Position
        {
            get => new() { Value = _position };
            set => SetPosition(value);
        }

        protected virtual void SetPosition(RenderPoint aValue)
        {
            if (_position.Equals(aValue.Value)) return;
            _position = aValue.Value;
        }

        public abstract bool HitTest(RenderRay ray, out HitData hitData);
        public abstract bool BoundaryTest(RenderRay ray, out double distance);
    }
}
