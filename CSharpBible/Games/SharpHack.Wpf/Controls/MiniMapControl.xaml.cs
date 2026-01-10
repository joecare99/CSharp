using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SharpHack.Wpf.Controls;

public partial class MiniMapControl : UserControl
{
    public static readonly DependencyProperty MiniMapProperty = DependencyProperty.Register(
        nameof(MiniMap),
        typeof(byte[]),
        typeof(MiniMapControl),
        new PropertyMetadata(null, OnMiniMapChanged));

    public static readonly DependencyProperty MapWidthProperty = DependencyProperty.Register(
        nameof(MapWidth),
        typeof(int),
        typeof(MiniMapControl),
        new PropertyMetadata(0, OnMiniMapChanged));

    public static readonly DependencyProperty MapHeightProperty = DependencyProperty.Register(
        nameof(MapHeight),
        typeof(int),
        typeof(MiniMapControl),
        new PropertyMetadata(0, OnMiniMapChanged));

    private byte[]? _lastMiniMap;
    private int _lastWidth;
    private int _lastHeight;
    private WriteableBitmap? _bitmap;

    public MiniMapControl()
    {
        InitializeComponent();
    }

    public byte[]? MiniMap
    {
        get => (byte[]?)GetValue(MiniMapProperty);
        set => SetValue(MiniMapProperty, value);
    }

    public int MapWidth
    {
        get => (int)GetValue(MapWidthProperty);
        set => SetValue(MapWidthProperty, value);
    }

    public int MapHeight
    {
        get => (int)GetValue(MapHeightProperty);
        set => SetValue(MapHeightProperty, value);
    }

    private static void OnMiniMapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((MiniMapControl)d).Render();
    }

    private void Render()
    {
        if (MiniMap is null || MapWidth <= 0 || MapHeight <= 0)
        {
            _lastMiniMap = null;
            _lastWidth = 0;
            _lastHeight = 0;
            _bitmap = null;
            MiniMapImage.Source = null;
            return;
        }

        if (ReferenceEquals(_lastMiniMap, MiniMap) && _lastWidth == MapWidth && _lastHeight == MapHeight)
        {
            return;
        }

        int expected = MapWidth * MapHeight;
        if (MiniMap.Length < expected)
        {
            _lastMiniMap = null;
            _bitmap = null;
            MiniMapImage.Source = null;
            return;
        }

        _lastMiniMap = MiniMap;
        _lastWidth = MapWidth;
        _lastHeight = MapHeight;

        if (_bitmap is null || _bitmap.PixelWidth != MapWidth || _bitmap.PixelHeight != MapHeight)
        {
            _bitmap = new WriteableBitmap(MapWidth, MapHeight, 96, 96, PixelFormats.Bgra32, null);
            MiniMapImage.Source = _bitmap;
        }

        var pixels = new byte[expected * 4];

        for (int i = 0; i < expected; i++)
        {
            byte v = MiniMap[i];

            bool explored = v != 0;
            bool floor = (v & 0x01) != 0;
            bool item = (v & 0x02) != 0;
            bool special = (v & 0x08) != 0 || (v & 0x04) != 0;
            bool enemy = (v & 0x40) != 0;
            bool player = (v & 0x80) != 0;

            byte r = 0, g = 0, b = 0, a = 255;

            if (!explored)
            {
                a = 0;
            }
            else if (player)
            {
                r = 255; g = 255; b = 0;
            }
            else if (enemy)
            {
                r = 0; g = 255; b = 0;
            }
            else if (item)
            {
                r = 0; g = 255; b = 255;
            }
            else if (floor)
            {
                r = 80; g = 80; b = 80;
            }
            else if (special)
            {
                r = 200; g = 200; b = 200;
            }
            else
            {
                r = 30; g = 30; b = 30;
            }

            int p = i * 4;
            pixels[p + 0] = b;
            pixels[p + 1] = g;
            pixels[p + 2] = r;
            pixels[p + 3] = a;
        }

        _bitmap.WritePixels(new Int32Rect(0, 0, MapWidth, MapHeight), pixels, MapWidth * 4, 0);
    }
}
