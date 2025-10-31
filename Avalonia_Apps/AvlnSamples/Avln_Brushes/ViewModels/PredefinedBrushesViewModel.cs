using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Media;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;

namespace Avln_Brushes.ViewModels;

public partial class PredefinedBrushesViewModel : ObservableObject
{
    public ObservableCollection<ColorInfo> PredefinedColors { get; }

    [ObservableProperty]
    private ColorSortMode _selectedSortMode = ColorSortMode.Alphabetical;

    public Array SortModes => Enum.GetValues(typeof(ColorSortMode));

    private readonly List<ColorInfo> _allColors = new();

    public PredefinedBrushesViewModel()
    {
    PredefinedColors = new ObservableCollection<ColorInfo>();
        LoadPredefinedColors();
    }

    private void LoadPredefinedColors()
    {
        // Get all static properties from Avalonia.Media.Colors that return Color
        var colorType = typeof(Colors);
        var colorProperties = colorType
   .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(p => p.PropertyType == typeof(Color));

        foreach (var property in colorProperties)
        {
         var color = (Color)property.GetValue(null)!;
     var colorInfo = new ColorInfo
      {
  Name = property.Name,
     Color = color,
     HexValue = $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}"
 };
          _allColors.Add(colorInfo);
      }

// Initial sort
        ApplySorting();
    }

    partial void OnSelectedSortModeChanged(ColorSortMode value)
    {
        ApplySorting();
    }

    [RelayCommand]
    private void ApplySorting()
    {
        IEnumerable<ColorInfo> sorted = SelectedSortMode switch
        {
 ColorSortMode.Alphabetical => _allColors.OrderBy(c => c.Name),
    ColorSortMode.Hue => _allColors.OrderBy(c => c.HueValue),
            ColorSortMode.Brightness => _allColors.OrderByDescending(c => c.Brightness),
            ColorSortMode.Saturation => _allColors.OrderByDescending(c => c.Saturation),
        ColorSortMode.Red => _allColors.OrderByDescending(c => c.Color.R),
    ColorSortMode.Green => _allColors.OrderByDescending(c => c.Color.G),
      ColorSortMode.Blue => _allColors.OrderByDescending(c => c.Color.B),
            ColorSortMode.HilbertRGB => _allColors.OrderBy(c => c.HilbertIndexRGB),
  ColorSortMode.HilbertHSV => _allColors.OrderBy(c => c.HilbertIndexHSV),
      _ => _allColors.OrderBy(c => c.Name)
        };

        PredefinedColors.Clear();
        foreach (var color in sorted)
        {
            PredefinedColors.Add(color);
        }
    }
}

public class ColorInfo
{
    public string Name { get; set; } = string.Empty;
    public Color Color { get; set; }
    public string HexValue { get; set; } = string.Empty;

    // Computed properties for sorting
    public double HueValue
    {
        get
        {
 // Convert RGB to HSV to get Hue
            double r = Color.R / 255.0;
            double g = Color.G / 255.0;
            double b = Color.B / 255.0;

         double max = Math.Max(r, Math.Max(g, b));
     double min = Math.Min(r, Math.Min(g, b));
          double delta = max - min;

            if (delta == 0)
          return 0;

          double hue;
       if (max == r)
     hue = ((g - b) / delta) % 6;
 else if (max == g)
           hue = (b - r) / delta + 2;
     else
            hue = (r - g) / delta + 4;

            hue *= 60;
            if (hue < 0)
         hue += 360;

            return hue;
        }
    }

    public double Brightness
    {
        get
        {
        // Perceived brightness (luminance)
            return 0.299 * Color.R + 0.587 * Color.G + 0.114 * Color.B;
}
    }

    public double Saturation
    {
        get
        {
       // HSV Saturation
    double r = Color.R / 255.0;
            double g = Color.G / 255.0;
  double b = Color.B / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
     double min = Math.Min(r, Math.Min(g, b));

         if (max == 0)
     return 0;

            return (max - min) / max;
        }
    }

    // Hilbert Curve Index for RGB space
    public long HilbertIndexRGB
    {
        get
        {
        // Map RGB values to 3D Hilbert Curve
       // Using 8-bit precision (256 levels per channel)
       return HilbertCurve.Encode3D(Color.R, Color.G, Color.B, 8);
        }
    }

    // Hilbert Curve Index for HSV space
    public long HilbertIndexHSV
    {
   get
    {
        // Convert to HSV first
            double r = Color.R / 255.0;
        double g = Color.G / 255.0;
   double b = Color.B / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
      double min = Math.Min(r, Math.Min(g, b));
   double delta = max - min;

       // Hue (0-360)
  double hue = 0;
      if (delta != 0)
            {
  if (max == r)
     hue = ((g - b) / delta) % 6;
else if (max == g)
             hue = (b - r) / delta + 2;
                else
     hue = (r - g) / delta + 4;

           hue *= 60;
            if (hue < 0)
          hue += 360;
          }

         // Saturation (0-1)
        double saturation = max == 0 ? 0 : delta / max;

            // Value (0-1)
        double value = max;

  // Map to 8-bit precision
        int h = (int)(hue / 360.0 * 255.0);
   int s = (int)(saturation * 255.0);
        int v = (int)(value * 255.0);

  return HilbertCurve.Encode3D(h, s, v, 8);
        }
    }
}

/// <summary>
/// 3D Hilbert Curve implementation for color space traversal
/// </summary>
public static class HilbertCurve
{
    /// <summary>
    /// Encode 3D coordinates to Hilbert index
    /// </summary>
    /// <param name="x">X coordinate (0-255)</param>
    /// <param name="y">Y coordinate (0-255)</param>
    /// <param name="z">Z coordinate (0-255)</param>
    /// <param name="bits">Number of bits per dimension (default 8 for 0-255)</param>
    /// <returns>Hilbert curve index</returns>
    public static long Encode3D(int x, int y, int z, int bits)
    {
    long index = 0;
 for (int i = bits - 1; i >= 0; i--)
        {
     int xi = (x >> i) & 1;
     int yi = (y >> i) & 1;
   int zi = (z >> i) & 1;

        index = (index << 3) | Morton3D(xi, yi, zi);

  // Apply Hilbert rotation
       int rotation = MortonToHilbert(index & 7);
      (x, y, z) = RotatePoint(x, y, z, rotation, i);
        }

        return index;
    }

    /// <summary>
    /// 3D Morton encoding (Z-order curve)
    /// </summary>
    private static int Morton3D(int x, int y, int z)
    {
        return (x << 2) | (y << 1) | z;
    }

  /// <summary>
    /// Convert Morton code to Hilbert rotation
    /// </summary>
    private static int MortonToHilbert(long morton)
    {
        // Simplified rotation table for 3D Hilbert curve
    int[] rotations = { 0, 1, 3, 2, 7, 6, 4, 5 };
        return rotations[morton & 7];
    }

    /// <summary>
    /// Rotate point in 3D space for Hilbert curve generation
    /// </summary>
    private static (int, int, int) RotatePoint(int x, int y, int z, int rotation, int bit)
    {
        // Simplified rotation for demonstration
        // In a full implementation, this would apply proper 3D rotations
        return rotation switch
  {
            1 => (y, z, x),
            2 => (z, x, y),
      3 => (x, z, y),
            4 => (y, x, z),
     5 => (z, y, x),
            _ => (x, y, z)
        };
    }
}

public enum ColorSortMode
{
    Alphabetical,
    Hue,
    Brightness,
    Saturation,
    Red,
    Green,
    Blue,
    HilbertRGB,
  HilbertHSV
}
