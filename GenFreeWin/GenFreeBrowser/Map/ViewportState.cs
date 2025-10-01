using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace GenFreeBrowser.Map;

public sealed class ViewportState : INotifyPropertyChanged
{
    private GeoPoint _center = new(0,50);
    private int _zoom = 5;
    private Size _pixelSize;

    public GeoPoint Center
    {
        get => _center;
        set { if(!_center.Equals(value)) { _center = GeoPoint.Clamp(value); OnChanged(); } }
    }

    public int Zoom
    {
        get => _zoom;
        set { value = Math.Clamp(value, 0, 22); if (_zoom != value) { _zoom = value; OnChanged(); } }
    }

    public Size PixelSize
    {
        get => _pixelSize;
        set { if (_pixelSize != value) { _pixelSize = value; OnChanged(); } }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public (double tileX, double tileY) CenterTileXY()
    {
        var (x, y) = WebMercator.Project(Center, Zoom);
        return (x, y);
    }
}
