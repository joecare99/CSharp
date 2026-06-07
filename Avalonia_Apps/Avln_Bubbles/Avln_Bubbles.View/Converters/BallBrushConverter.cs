using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avln_Bubbles.View.ViewModels;

namespace Avln_Bubbles.View.Converters;

/// <summary>
/// Converts a <see cref="BallType"/> into a display brush.
/// </summary>
public sealed class BallBrushConverter : IValueConverter
{
    /// <summary>
    /// Gets the shared converter instance.
    /// </summary>
    public static BallBrushConverter Instance { get; } = new();

    /// <inheritdoc/>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var color = value is BallType ballType ? SelectColor(ballType) : Colors.Transparent;
        return new RadialGradientBrush
        {
            GradientStops = new GradientStops
            {
                new GradientStop(Colors.White, 0.0),
                new GradientStop(color, 0.55),
                new GradientStop(Colors.Black, 1.0)
            }
        };
    }

    /// <inheritdoc/>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();

    private static Color SelectColor(BallType ballType) => ballType switch
    {
        BallType.Ball1 => Colors.OrangeRed,
        BallType.Ball2 => Colors.MediumPurple,
        BallType.Ball3 => Colors.DodgerBlue,
        BallType.Ball4 => Colors.Gold,
        BallType.Ball5 => Colors.ForestGreen,
        _ => Colors.Transparent
    };
}
