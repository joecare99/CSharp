using System;
using System.Globalization;
using System.Windows.Data;
using GenFreeBrowser.Map;

namespace MapDemo.Converters;

/// <summary>
/// Converts a GeoPoint (Lon, Lat in decimal degrees) to a DMS string:
///   9° 30' 12.34" E, 50° 00' 00.00" N
/// </summary>
public sealed class GeoPointDmsConverter : IValueConverter
{
    public int SecondsDecimals { get; set; } = 2;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            GeoPoint p => FormatPoint(p, SecondsDecimals, culture),
            _ => string.Empty
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();

    private static string FormatPoint(GeoPoint p, int secDec, CultureInfo culture)
    {
        var lon = FormatCoord(p.Lon, false, secDec, culture);
        var lat = FormatCoord(p.Lat, true, secDec, culture);
        return lon + ", " + lat;
    }

    private static string FormatCoord(double decDeg, bool isLat, int secDec, CultureInfo culture)
    {
        var dir = GetDirection(decDeg, isLat);
        decDeg = Math.Abs(decDeg);
        var deg = (int)Math.Floor(decDeg);
        var minFull = (decDeg - deg) * 60.0;
        var min = (int)Math.Floor(minFull);
        var sec = (minFull - min) * 60.0;
        var secFmt = sec.ToString("F" + secDec, culture);
        return $"{deg}° {min:00}' {secFmt.PadLeft(secFmt.Length < 4 ? 4 : secFmt.Length)}\" {dir}";
    }

    private static string GetDirection(double v, bool isLat)
        => isLat ? (v >= 0 ? "N" : "S") : (v >= 0 ? "E" : "W");
}
