using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia;
using Avalonia.Media;
using System;
using System.Globalization;
using System.Text;

namespace Avln_Brushes.ViewModels;

public partial class InteractiveLinearGradientViewModel : ObservableObject
{
    [ObservableProperty]
    private RelativePoint _startPoint = new(0, 0, RelativeUnit.Relative);

    [ObservableProperty]
    private RelativePoint _endPoint = new(1, 1, RelativeUnit.Relative);

    [ObservableProperty]
    private double _opacity = 1.0;

    [ObservableProperty]
    private GradientSpreadMethod _spreadMethod = GradientSpreadMethod.Pad;

    [ObservableProperty]
    private string _startPointText = "0.0000;0.0000";

    [ObservableProperty]
    private string _endPointText = "1.0000;1.0000";

    [ObservableProperty]
    private string _markupOutput = string.Empty;

    [ObservableProperty]
    private double _gradientStop1Offset = 0.0;

    [ObservableProperty]
    private double _gradientStop2Offset = 0.5;

    [ObservableProperty]
    private double _gradientStop3Offset = 1.0;

    [ObservableProperty]
    private Color _gradientStop1Color = Colors.Blue;

    [ObservableProperty]
    private Color _gradientStop2Color = Colors.Purple;

    [ObservableProperty]
    private Color _gradientStop3Color = Colors.Red;

    public InteractiveLinearGradientViewModel()
    {
        UpdateMarkup();
    }

    partial void OnStartPointChanged(RelativePoint value)
    {
        // Use invariant culture for consistent formatting
        StartPointText = $"{value.Point.X.ToString("F4", CultureInfo.InvariantCulture)};{value.Point.Y.ToString("F4", CultureInfo.InvariantCulture)}";
        UpdateMarkup();
    }

    partial void OnEndPointChanged(RelativePoint value)
    {
        // Use invariant culture for consistent formatting
        EndPointText = $"{value.Point.X.ToString("F4", CultureInfo.InvariantCulture)};{value.Point.Y.ToString("F4", CultureInfo.InvariantCulture)}";
        UpdateMarkup();
    }

    partial void OnOpacityChanged(double value) => UpdateMarkup();
    partial void OnSpreadMethodChanged(GradientSpreadMethod value) => UpdateMarkup();
    partial void OnGradientStop1OffsetChanged(double value) => UpdateMarkup();
    partial void OnGradientStop2OffsetChanged(double value) => UpdateMarkup();
    partial void OnGradientStop3OffsetChanged(double value) => UpdateMarkup();
    partial void OnGradientStop1ColorChanged(Color value) => UpdateMarkup();
    partial void OnGradientStop2ColorChanged(Color value) => UpdateMarkup();
    partial void OnGradientStop3ColorChanged(Color value) => UpdateMarkup();

    [RelayCommand]
    private void UpdateStartPointFromText()
    {
        try
        {
            var point = ParsePoint(StartPointText);
            StartPoint = new RelativePoint(point.X, point.Y, RelativeUnit.Relative);
        }
        catch
        {
            // Ignore parse errors - could show validation message here
        }
    }

    [RelayCommand]
    private void UpdateEndPointFromText()
    {
        try
        {
            var point = ParsePoint(EndPointText);
            EndPoint = new RelativePoint(point.X, point.Y, RelativeUnit.Relative);
        }
        catch
        {
            // Ignore parse errors - could show validation message here
        }
    }

    /// <summary>
    /// Parse a point from text. Supports both ";" and "," as separators.
    /// Uses InvariantCulture for number parsing to avoid localization issues.
    /// </summary>
    private Point ParsePoint(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new Point(0, 0);

        // Try semicolon separator first (preferred)
        var parts = text.Split(';');

        // Fall back to comma if semicolon didn't work
        if (parts.Length != 2)
            parts = text.Split(',');

        if (parts.Length != 2)
            throw new FormatException("Point must be in format 'X;Y' or 'X,Y'");

        // Use InvariantCulture to parse numbers consistently
        var x = double.Parse(parts[0].Trim(), CultureInfo.InvariantCulture);
        var y = double.Parse(parts[1].Trim(), CultureInfo.InvariantCulture);

        return new Point(x, y);
    }

    private void UpdateMarkup()
    {
        var sBuilder = new StringBuilder();
        sBuilder.Append("<LinearGradientBrush\n");

        // Use invariant culture for XAML output
        sBuilder.Append($"  StartPoint=\"{StartPoint.Point.X.ToString("F4", CultureInfo.InvariantCulture)},{StartPoint.Point.Y.ToString("F4", CultureInfo.InvariantCulture)}\"\n");
        sBuilder.Append($"  EndPoint=\"{EndPoint.Point.X.ToString("F4", CultureInfo.InvariantCulture)},{EndPoint.Point.Y.ToString("F4", CultureInfo.InvariantCulture)}\"\n");
        sBuilder.Append($"  SpreadMethod=\"{SpreadMethod}\"\n");
        sBuilder.Append($"  Opacity=\"{Opacity.ToString(CultureInfo.InvariantCulture)}\">\n");

        sBuilder.Append($"  <GradientStop Offset=\"{GradientStop1Offset.ToString("F4", CultureInfo.InvariantCulture)}\" Color=\"{GradientStop1Color}\" />\n");
        sBuilder.Append($"  <GradientStop Offset=\"{GradientStop2Offset.ToString("F4", CultureInfo.InvariantCulture)}\" Color=\"{GradientStop2Color}\" />\n");
        sBuilder.Append($"  <GradientStop Offset=\"{GradientStop3Offset.ToString("F4", CultureInfo.InvariantCulture)}\" Color=\"{GradientStop3Color}\" />\n");

        sBuilder.Append("</LinearGradientBrush>");
        MarkupOutput = sBuilder.ToString();
    }
}
