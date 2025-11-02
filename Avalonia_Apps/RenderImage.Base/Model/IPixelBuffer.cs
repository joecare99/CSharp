namespace RenderImage.Base.Model
{
 public interface IPixelBuffer
 {
 int Width { get; }
 int Height { get; }
 void SetPixel(int x, int y, RenderColor color);
 }
}
