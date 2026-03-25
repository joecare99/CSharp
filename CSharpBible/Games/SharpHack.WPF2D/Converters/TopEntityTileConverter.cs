using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using SharpHack.ViewModel;
using SharpHack.Wpf.Services;

namespace SharpHack.WPF2D.Converters;

public sealed class TopEntityTileConverter : Freezable, IValueConverter
{
    public static readonly DependencyProperty TileServiceProperty = DependencyProperty.Register(
        nameof(TileService),
        typeof(ITileService),
        typeof(TopEntityTileConverter),
        new PropertyMetadata(null));

    public ITileService? TileService
    {
        get => (ITileService?)GetValue(TileServiceProperty);
        set => SetValue(TileServiceProperty, value);
    }

    protected override Freezable CreateInstanceCore() => new TopEntityTileConverter();

    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not IReadOnlyList<LayeredEntity> entities || entities.Count == 0)
        {
            return null;
        }

        return TileService?.GetTile(entities[0].Tile);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
