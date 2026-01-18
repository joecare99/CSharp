using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using SharpHack.ViewModel;
using SharpHack.Wpf.Services;

namespace SharpHack.WPF2D.Converters;

public sealed class TileToImageConverter : Freezable, IValueConverter
{
    public static readonly DependencyProperty TileServiceProperty = DependencyProperty.Register(
        nameof(TileService),
        typeof(ITileService),
        typeof(TileToImageConverter),
        new PropertyMetadata(null));

    public ITileService? TileService
    {
        get => (ITileService?)GetValue(TileServiceProperty);
        set => SetValue(TileServiceProperty, value);
    }

    protected override Freezable CreateInstanceCore() => new TileToImageConverter();

    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not DisplayTile tile)
        {
            return null;
        }

        return TileService?.GetTile(tile);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
