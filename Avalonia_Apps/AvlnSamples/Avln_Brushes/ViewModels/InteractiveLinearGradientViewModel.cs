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
 private string _startPointText = "0.0000,0.0000";

  [ObservableProperty]
  private string _endPointText = "1.0000,1.0000";

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
    StartPointText = $"{value.Point.X:F4},{value.Point.Y:F4}";
        UpdateMarkup();
    }

 partial void OnEndPointChanged(RelativePoint value)
    {
    EndPointText = $"{value.Point.X:F4},{value.Point.Y:F4}";
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
   var point = Point.Parse(StartPointText);
     StartPoint = new RelativePoint(point.X, point.Y, RelativeUnit.Relative);
 }
      catch
      {
  // Ignore parse errors
        }
    }

    [RelayCommand]
    private void UpdateEndPointFromText()
    {
        try
  {
 var point = Point.Parse(EndPointText);
    EndPoint = new RelativePoint(point.X, point.Y, RelativeUnit.Relative);
   }
 catch
        {
            // Ignore parse errors
  }
    }

  private void UpdateMarkup()
  {
        var sBuilder = new StringBuilder();
        sBuilder.Append("<LinearGradientBrush\n");
        sBuilder.Append($"  StartPoint=\"{StartPoint.Point.X:F4},{StartPoint.Point.Y:F4}\"\n");
     sBuilder.Append($"  EndPoint=\"{EndPoint.Point.X:F4},{EndPoint.Point.Y:F4}\"\n");
    sBuilder.Append($"  SpreadMethod=\"{SpreadMethod}\"\n");
   sBuilder.Append($"  Opacity=\"{Opacity.ToString(CultureInfo.InvariantCulture)}\">\n");

sBuilder.Append($"  <GradientStop Offset=\"{GradientStop1Offset:F4}\" Color=\"{GradientStop1Color}\" />\n");
sBuilder.Append($"  <GradientStop Offset=\"{GradientStop2Offset:F4}\" Color=\"{GradientStop2Color}\" />\n");
  sBuilder.Append($"  <GradientStop Offset=\"{GradientStop3Offset:F4}\" Color=\"{GradientStop3Color}\" />\n");

  sBuilder.Append("</LinearGradientBrush>");
        MarkupOutput = sBuilder.ToString();
    }
}
