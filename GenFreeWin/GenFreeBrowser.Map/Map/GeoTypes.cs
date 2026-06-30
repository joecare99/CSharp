using System;
using System.Windows;

namespace GenFreeBrowser.Map;

public static class MapConstants
{
    public const int TileSize = 256; // Equivalent to TILE_SIZE in Pascal
    public const double MinLatitude = -85.05112878; // Web Mercator constraint
    public const double MaxLatitude = 85.05112878;
    public const double MinLongitude = -180;
    public const double MaxLongitude = 180;
}

/// <summary>
/// Geographic point (Longitude, Latitude) in degrees (WGS84)
/// </summary>
public readonly record struct GeoPoint(double Lon, double Lat)
{
    public static GeoPoint Clamp(GeoPoint p)
    {
        var lon = Math.Clamp(p.Lon, MapConstants.MinLongitude, MapConstants.MaxLongitude);
        var lat = Math.Clamp(p.Lat, MapConstants.MinLatitude, MapConstants.MaxLatitude);
        return new GeoPoint(lon, lat);
    }
}

public readonly record struct GeoArea(GeoPoint TopLeft, GeoPoint BottomRight)
{
    public bool Intersects(GeoArea other)
    {
        return !(other.BottomRight.Lat > TopLeft.Lat ||
                 other.TopLeft.Lat < BottomRight.Lat ||
                 other.TopLeft.Lon > BottomRight.Lon ||
                 other.BottomRight.Lon < TopLeft.Lon);
    }
}

internal static class WebMercator
{
    public static (double x, double y) Project(GeoPoint p, int zoom)
    {
        // Uses Slippy map tiling (spherical mercator)
        var latRad = p.Lat * Math.PI / 180.0;
        var n = 1 << zoom;
        var xtile = (p.Lon + 180.0) / 360.0 * n;
        var ytile = (1.0 - Math.Log(Math.Tan(latRad) + 1.0 / Math.Cos(latRad)) / Math.PI) / 2.0 * n;
        return (xtile, ytile);
    }

    public static GeoPoint Unproject(double x, double y, int zoom)
    {
        var n = 1 << zoom;
        var lon = x / n * 360.0 - 180.0;
        var latRad = Math.Atan(Math.Sinh(Math.PI * (1 - 2 * y / n)));
        var lat = latRad * 180.0 / Math.PI;
        return new GeoPoint(lon, lat);
    }
}
