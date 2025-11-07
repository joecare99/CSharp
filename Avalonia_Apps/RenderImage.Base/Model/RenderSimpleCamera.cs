using MathLibrary.RenderImage;

namespace RenderImage.Base.Model
{
 public sealed class RenderSimpleCamera : RenderCamera
 {
 private double _angle;
 public double Angle { get => _angle; set => SetAngle(value); }
 private void SetAngle(double value)
 {
 if (_angle == value) return;
 _angle = value;
 }
 }
}
