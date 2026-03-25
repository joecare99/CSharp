namespace ScreenX.Base;

public static class Color32
{
 // ARGB32 packing helpers
 public static uint FromArgb(byte a, byte r, byte g, byte b)
 => (uint)(a <<24 | r <<16 | g <<8 | b);
 public static uint FromRgb(byte r, byte g, byte b)
 => FromArgb(255, r, g, b);
}
