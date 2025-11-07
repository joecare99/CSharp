using System;

namespace RenderImage.Base.Model
{
 public sealed class ArrayPixelBuffer : IPixelBuffer
 {
 public int Width { get; }
 public int Height { get; }
 public RenderColor[,] Pixels { get; }

 public ArrayPixelBuffer(int width, int height)
 {
 if (width <=0 || height <=0) throw new ArgumentOutOfRangeException();
 Width = width; Height = height;
 Pixels = new RenderColor[height, width];
 }

 public void SetPixel(int x, int y, RenderColor color)
 {
 if ((uint)x >= (uint)Width || (uint)y >= (uint)Height) return;
 Pixels[y, x] = color;
 }
 }
}
