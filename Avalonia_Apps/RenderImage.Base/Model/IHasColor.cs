namespace RenderImage.Base.Model
{
 public interface IHasColor
 {
 RenderColor GetColorAt(RenderPoint point);
 RenderColor this[RenderPoint point] { get; }
 }
}
