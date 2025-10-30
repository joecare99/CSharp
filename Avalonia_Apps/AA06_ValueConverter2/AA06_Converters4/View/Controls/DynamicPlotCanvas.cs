// ***********************************************************************
// Assembly         : AA06_Converters_4
// Author           : Mir
// Created          : 12-20-2024
//
// Last Modified By : Mir
// Last Modified On : 12-20-2024
// ***********************************************************************
// <copyright file="DynamicPlotCanvas.cs" company="JC-Soft">
//     (c) by Joe Care 2024
// </copyright>
// <summary>Dynamisches Canvas-Control für Avalonia mit MVVM-Support</summary>
// ***********************************************************************

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.Collections.ObjectModel;
using System.Drawing; // Nur für PointF, RectangleF
using AA06_Converters_4.ViewModels;
using AvaloniaColor = Avalonia.Media.Color; // Alias für Avalonia Color

namespace AA06_Converters_4.View.Controls;

/// <summary>
/// Dynamisches Canvas-Control für Koordinatensystem-Darstellung
/// Unterstützt Pan, Zoom und dynamische Elemente via MVVM
/// </summary>
public class DynamicPlotCanvas : Control
{
    private PointF? _dragStartPos;
    private bool _isDragging;
    private RectangleF _isometricViewport; // Cache für isometrischen Viewport
    private bool _isRenderScheduled; // Flag für Render-Throttling
    private DispatcherTimer? _renderThrottleTimer; // Timer für Render-Throttling

    /// <summary>
    /// Styled Property für das ViewModel (DataContext wird automatisch gebunden)
    /// </summary>
    public static readonly StyledProperty<PlotFrameViewModel?> ViewModelProperty =
        AvaloniaProperty.Register<DynamicPlotCanvas, PlotFrameViewModel?>(nameof(ViewModel));

 /// <summary>
    /// Styled Property für den Hintergrund
    /// </summary>
  public static readonly StyledProperty<IBrush?> BackgroundProperty =
   AvaloniaProperty.Register<DynamicPlotCanvas, IBrush?>(
            nameof(Background), 
   defaultValue: Brushes.White);

    /// <summary>
    /// Gets or sets the ViewModel
    /// </summary>
    public PlotFrameViewModel? ViewModel
    {
        get => GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <summary>
    /// Gets or sets the Background brush
    /// </summary>
    public IBrush? Background
    {
        get => GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
}

    static DynamicPlotCanvas()
    {
AffectsRender<DynamicPlotCanvas>(ViewModelProperty, BoundsProperty, BackgroundProperty);
        ViewModelProperty.Changed.AddClassHandler<DynamicPlotCanvas>((x, e) => x.OnViewModelChanged(e));
    }

    public DynamicPlotCanvas()
  {
        ClipToBounds = true;
        
     // Render-Throttle-Timer initialisieren (16ms ? 60 FPS)
        _renderThrottleTimer = new DispatcherTimer
   {
            Interval = TimeSpan.FromMilliseconds(16)
        };
 _renderThrottleTimer.Tick += (s, e) =>
        {
            _isRenderScheduled = false;
          _renderThrottleTimer?.Stop();
       InvalidateVisual();
    };
        
        // Event-Handler für Interaktivität
        PointerPressed += OnPointerPressed;
        PointerMoved += OnPointerMoved;
   PointerReleased += OnPointerReleased;
   PointerWheelChanged += OnPointerWheelChanged;
    }

    private void OnViewModelChanged(AvaloniaPropertyChangedEventArgs e)
    {
 if (e.OldValue is PlotFrameViewModel oldVm)
        {
   oldVm.PropertyChanged -= ViewModel_PropertyChanged;
     
       // Entferne Model-Listener
            if (oldVm.AGVModel != null)
            {
          oldVm.AGVModel.PropertyChanged -= Model_PropertyChanged;
            }
        }

        if (e.NewValue is PlotFrameViewModel newVm)
  {
            newVm.PropertyChanged += ViewModel_PropertyChanged;
     newVm.WindowSize = new Avalonia.Size(Bounds.Width, Bounds.Height);
  
          // Registriere Model-Listener
            if (newVm.AGVModel != null)
      {
        newVm.AGVModel.PropertyChanged += Model_PropertyChanged;
        }
}

        ScheduleRender();
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        // Während des Dragging nur bei bestimmten Properties rendern
        if (_isDragging && e.PropertyName == nameof(PlotFrameViewModel.VPWindow))
        {
     // Viewport-Änderung während Drag -> Throttled Render
        ScheduleRender();
       return;
     }

  if (e.PropertyName == nameof(PlotFrameViewModel.VPWindow) ||
     e.PropertyName == nameof(PlotFrameViewModel.WindowPort) ||
            e.PropertyName == nameof(PlotFrameViewModel.Arrows) ||
       e.PropertyName == nameof(PlotFrameViewModel.Circles) ||
       e.PropertyName == nameof(PlotFrameViewModel.Polynomes))
     {
          ScheduleRender();
     }
    }

    private void Model_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        // Bei Model-Änderungen nur außerhalb von Drag-Operationen sofort rendern
        if (!_isDragging)
        {
            ScheduleRender();
        }
    }

    /// <summary>
    /// Throttled Render - verhindert zu viele Render-Aufrufe
 /// </summary>
    private void ScheduleRender()
    {
        if (_isRenderScheduled) return;

        _isRenderScheduled = true;
        
  if (_isDragging)
        {
            // Während Dragging: Throttle auf 60 FPS
      _renderThrottleTimer?.Stop();
  _renderThrottleTimer?.Start();
        }
        else
    {
            // Außerhalb Dragging: Sofort rendern
  Dispatcher.UIThread.Post(() =>
   {
        _isRenderScheduled = false;
         InvalidateVisual();
            }, DispatcherPriority.Render);
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
    base.OnPropertyChanged(change);

        if (change.Property == BoundsProperty && ViewModel != null)
        {
         ViewModel.WindowSize = new Avalonia.Size(Bounds.Width, Bounds.Height);
            // Viewport anpassen für isometrische Darstellung
         UpdateIsometricViewport();
        }
    }

    /// <summary>
    /// Berechnet einen isometrischen Viewport (gleiche Skalierung in X und Y)
    /// </summary>
    private void UpdateIsometricViewport()
    {
        if (ViewModel == null || Bounds.Width == 0 || Bounds.Height == 0)
            return;

        const double margin = 50;
   var availableWidth = Bounds.Width - 2 * margin;
     var availableHeight = Bounds.Height - 2 * margin;

        var viewport = ViewModel.VPWindow;
  var aspectRatio = availableWidth / availableHeight;
        var viewportAspect = viewport.Width / viewport.Height;

        RectangleF adjustedViewport;

        if (viewportAspect > aspectRatio)
        {
 // Viewport ist breiter -> Höhe anpassen
       var newHeight = (float)(viewport.Width / aspectRatio);
        var centerY = viewport.Top + viewport.Height / 2;
  adjustedViewport = new RectangleF(
      viewport.Left,
      centerY - newHeight / 2,
             viewport.Width,
         newHeight);
        }
    else
        {
  // Viewport ist höher -> Breite anpassen
          var newWidth = (float)(viewport.Height * aspectRatio);
  var centerX = viewport.Left + viewport.Width / 2;
     adjustedViewport = new RectangleF(
      centerX - newWidth / 2,
           viewport.Top,
    newWidth,
           viewport.Height);
        }

        _isometricViewport = adjustedViewport;
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (Bounds.Width == 0 || Bounds.Height == 0)
    return;

        // Hintergrund (verwendet Background-Property)
  if (Background != null)
      {
       context.FillRectangle(Background, new Rect(Bounds.Size));
    }

        if (ViewModel == null)
            return;

        UpdateIsometricViewport();
        var viewport = _isometricViewport;
      var windowPort = ViewModel.WindowPort;

        // Zeichne Koordinatensystem
     DrawCoordinateSystem(context, viewport, windowPort);

        // Zeichne Polygone (Fahrzeug)
        if (ViewModel.Polynomes?.Count > 0)
        {
            foreach (var poly in ViewModel.Polynomes)
            {
            DrawPolygon(context, viewport, poly);
            }
        }

        // Zeichne Kreise (Drehschemel)
    if (ViewModel.Circles?.Count > 0)
        {
            foreach (var circle in ViewModel.Circles)
      {
           DrawCircle(context, viewport, circle);
            }
   }

        // Zeichne Pfeile (Geschwindigkeiten)
        if (ViewModel.Arrows?.Count > 0)
        {
         foreach (var arrow in ViewModel.Arrows)
            {
   DrawArrow(context, viewport, arrow);
     }
        }

      // Zeichne AGV (wenn vorhanden und nicht schon durch Polygone gezeichnet)
      if (ViewModel.AGVModel != null && (ViewModel.Polynomes == null || ViewModel.Polynomes.Count == 0))
        {
       DrawAGV(context, viewport);
        }
    }

    private void DrawPolygon(DrawingContext context, RectangleF viewport, PolynomeData poly)
    {
        if (poly.Points == null || poly.Points.Count < 2)
        return;

        var points = new Avalonia.Point[poly.Points.Count];
        for (int i = 0; i < poly.Points.Count; i++)
        {
      points[i] = Real2Vis(poly.Points[i], viewport);
        }

        var geometry = new PolylineGeometry(points, true);
    var brush = ViewModel?.Polynomes?.Pen?.Brush ?? Brushes.Blue;
 var thickness = ViewModel?.Polynomes?.Pen?.Thickness ?? 2.0;
        
        context.DrawGeometry(null, new Pen(brush, thickness), geometry);
    }

    private void DrawCircle(DrawingContext context, RectangleF viewport, CircleData circle)
    {
        var center = Real2Vis(circle.Center, viewport);
  
        // Radius isometrisch umrechnen (durchschnittliche Skalierung)
    var radiusPoint = new PointF(circle.Center.X + (float)circle.Radius, circle.Center.Y);
        var radiusVis = Real2Vis(radiusPoint, viewport);
        var radius = Math.Abs(radiusVis.X - center.X);

        var brush = ViewModel?.Circles?.Pen?.Brush ?? Brushes.Green;
  var thickness = ViewModel?.Circles?.Pen?.Thickness ?? 1.0;

        context.DrawEllipse(null, new Pen(brush, thickness), center, radius, radius);
    }

    private void DrawArrow(DrawingContext context, RectangleF viewport, ArrowData arrow)
    {
        var start = Real2Vis(arrow.Start, viewport);
     var end = Real2Vis(arrow.End, viewport);

   var brush = ViewModel?.Arrows?.Pen?.Brush ?? Brushes.Red;
        var thickness = ViewModel?.Arrows?.Pen?.Thickness ?? 2.0;

      // Hauptlinie
        context.DrawLine(new Pen(brush, thickness), start, end);

        // Pfeilspitze
        DrawArrowHead(context, start, end, brush, thickness);
  }

  private void DrawCoordinateSystem(DrawingContext context, RectangleF viewport, SWindowPort windowPort)
    {
        var pen = new Pen(Brushes.LightGray, 0.5);
        var axisPen = new Pen(Brushes.Black, 1.5);
        var textBrush = Brushes.Black;

        // Berechne Schrittweite für Gitterlinien
     double gridStep = CalculateGridStep(viewport.Width, viewport.Height);

        // Vertikale Gitterlinien
   double startX = Math.Floor(viewport.Left / gridStep) * gridStep;
    for (double x = startX; x <= viewport.Right; x += gridStep)
        {
          var p1 = Real2Vis(new PointF((float)x, viewport.Top), viewport);
         var p2 = Real2Vis(new PointF((float)x, viewport.Bottom), viewport);

            // Koordinatenachse dicker
            var currentPen = Math.Abs(x) < gridStep / 10 ? axisPen : pen;
            context.DrawLine(currentPen, p1, p2);

      // Beschriftung
 if (Math.Abs(x) > gridStep / 10)
            {
 var text = new FormattedText(
        $"{x:0}",
             System.Globalization.CultureInfo.CurrentCulture,
        FlowDirection.LeftToRight,
      new Typeface("Arial"),
  10,
      textBrush);
     
    context.DrawText(text, new Avalonia.Point(p1.X - text.Width / 2, Bounds.Height - 15));
      }
        }

        // Horizontale Gitterlinien
        double startY = Math.Floor(viewport.Top / gridStep) * gridStep;
      for (double y = startY; y <= viewport.Bottom; y += gridStep)
     {
            var p1 = Real2Vis(new PointF(viewport.Left, (float)y), viewport);
            var p2 = Real2Vis(new PointF(viewport.Right, (float)y), viewport);

            var currentPen = Math.Abs(y) < gridStep / 10 ? axisPen : pen;
        context.DrawLine(currentPen, p1, p2);

// Beschriftung
            if (Math.Abs(y) > gridStep / 10)
            {
         var text = new FormattedText(
     $"{y:0}",
         System.Globalization.CultureInfo.CurrentCulture,
        FlowDirection.LeftToRight,
           new Typeface("Arial"),
      10,
      textBrush);

       context.DrawText(text, new Avalonia.Point(5, p1.Y - text.Height / 2));
          }
        }

        // Nullpunkt markieren
        var origin = Real2Vis(new PointF(0, 0), viewport);
        context.DrawEllipse(Brushes.Red, new Pen(Brushes.DarkRed, 2), origin, 5, 5);
    }

    private void DrawAGV(DrawingContext context, RectangleF viewport)
  {
        if (ViewModel?.AGVModel == null)
            return;

        var agv = ViewModel.AGVModel;
    var vehicleDim = agv.VehicleDim;
    var swivelKoor = agv.SwivelKoor;

        // Zeichne Fahrzeug-Rechteck (zentriert am Ursprung)
      var halfWidth = (float)(vehicleDim.x / 2);
        var halfHeight = (float)(vehicleDim.y / 2);

        var corners = new[]
        {
     Real2Vis(new PointF(-halfWidth, -halfHeight), viewport),
         Real2Vis(new PointF(halfWidth, -halfHeight), viewport),
       Real2Vis(new PointF(halfWidth, halfHeight), viewport),
          Real2Vis(new PointF(-halfWidth, halfHeight), viewport)
        };

        var geometry = new PolylineGeometry(corners, true);
        context.DrawGeometry(new SolidColorBrush(AvaloniaColor.FromArgb(100, 0, 100, 255)), 
            new Pen(Brushes.Blue, 2), geometry);

        // Zeichne Drehschemel
        DrawSwivel(context, viewport, new PointF((float)swivelKoor.x, (float)swivelKoor.y), agv.Swivel1Angle, agv.AxisOffset);
        DrawSwivel(context, viewport, new PointF((float)-swivelKoor.x, (float)-swivelKoor.y), agv.Swivel2Angle, agv.AxisOffset);

        // Zeichne Geschwindigkeitsvektor
        DrawVelocityVector(context, viewport, agv.AGVVelocity);
    }

    private void DrawSwivel(DrawingContext context, RectangleF viewport, 
        PointF position, double angle, double axisOffset)
    {
      var center = Real2Vis(position, viewport);
        
        // Drehschemel-Kreis
        context.DrawEllipse(Brushes.LightBlue, new Pen(Brushes.DarkBlue, 1.5), center, 8, 8);

        // Ausrichtungs-Pfeil
        var endX = position.X + (float)(Math.Cos(angle) * 50);
        var endY = position.Y + (float)(Math.Sin(angle) * 50);
        var endPoint = Real2Vis(new PointF(endX, endY), viewport);

      context.DrawLine(new Pen(Brushes.DarkBlue, 2), center, endPoint);

     // Räder
    var wheelHalfOffset = axisOffset / 2;
     var perpX = -Math.Sin(angle);
        var perpY = Math.Cos(angle);

     var wheel1Pos = new PointF(
     position.X + (float)(perpX * wheelHalfOffset),
   position.Y + (float)(perpY * wheelHalfOffset));
        var wheel2Pos = new PointF(
            position.X - (float)(perpX * wheelHalfOffset),
      position.Y - (float)(perpY * wheelHalfOffset));

        context.DrawEllipse(Brushes.Black, null, Real2Vis(wheel1Pos, viewport), 4, 4);
        context.DrawEllipse(Brushes.Black, null, Real2Vis(wheel2Pos, viewport), 4, 4);
    }

    private void DrawVelocityVector(DrawingContext context, RectangleF viewport, 
      MathLibrary.TwoDim.Math2d.Vector velocity)
    {
        if (velocity.Length() < 0.1) return;

        var origin = Real2Vis(new PointF(0, 0), viewport);
      var endPoint = Real2Vis(new PointF((float)velocity.x / 10, (float)velocity.y / 10), viewport);

        var pen = new Pen(Brushes.Green, 3);
        context.DrawLine(pen, origin, endPoint);

        // Pfeilspitze
        DrawArrowHead(context, origin, endPoint, Brushes.Green, 3);
    }

    private void DrawArrowHead(DrawingContext context, Avalonia.Point start, Avalonia.Point end, IBrush brush, double thickness = 3)
    {
        const double arrowLength = 10;
        const double arrowAngle = Math.PI / 6;

        var dx = end.X - start.X;
        var dy = end.Y - start.Y;
    var angle = Math.Atan2(dy, dx);

        var p1 = new Avalonia.Point(
            end.X - arrowLength * Math.Cos(angle - arrowAngle),
    end.Y - arrowLength * Math.Sin(angle - arrowAngle));

        var p2 = new Avalonia.Point(
    end.X - arrowLength * Math.Cos(angle + arrowAngle),
  end.Y - arrowLength * Math.Sin(angle + arrowAngle));

        var geometry = new PolylineGeometry(new[] { p1, end, p2 }, false);
        context.DrawGeometry(null, new Pen(brush, thickness), geometry);
    }

    private double CalculateGridStep(double width, double height)
 {
        double maxDim = Math.Max(width, height);
   double baseStep = Math.Pow(10, Math.Floor(Math.Log10(maxDim)));

 if (maxDim / baseStep < 3)
return baseStep / 5;
        else if (maxDim / baseStep < 6)
         return baseStep / 2;
     else
            return baseStep;
    }

    private Avalonia.Point Real2Vis(PointF realPoint, RectangleF viewport)
    {
        const double margin = 50; // Rand für Beschriftungen

        var x = margin + (realPoint.X - viewport.Left) * (Bounds.Width - 2 * margin) / viewport.Width;
        var y = Bounds.Height - margin - (realPoint.Y - viewport.Top) * (Bounds.Height - 2 * margin) / viewport.Height;

        return new Avalonia.Point(x, y);
    }

    private PointF Vis2Real(Avalonia.Point visPoint, RectangleF viewport)
    {
        const double margin = 50;

      var x = viewport.Left + (visPoint.X - margin) * viewport.Width / (Bounds.Width - 2 * margin);
  var y = viewport.Top + (Bounds.Height - margin - visPoint.Y) * viewport.Height / (Bounds.Height - 2 * margin);

      return new PointF((float)x, (float)y);
    }

    // Interaktivität
    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (ViewModel == null) return;

   var point = e.GetCurrentPoint(this);
        if (point.Properties.IsLeftButtonPressed)
        {
      _dragStartPos = Vis2Real(point.Position, _isometricViewport);
     _isDragging = true;
  e.Handled = true;
        }
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
     if (!_isDragging || ViewModel == null || _dragStartPos == null) return;

        var currentPos = Vis2Real(e.GetPosition(this), _isometricViewport);
   var delta = new PointF(
            _dragStartPos.Value.X - currentPos.X,
    _dragStartPos.Value.Y - currentPos.Y);

 var newViewport = ViewModel.VPWindow;
        newViewport.Offset(delta.X, delta.Y);
        
        // Während Dragging: Direkt setzen ohne PropertyChanged zu triggern
        // (verhindert zusätzliche Event-Kaskaden)
    ViewModel.VPWindow = newViewport;

        e.Handled = true;
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _isDragging = false;
        _dragStartPos = null;
        
    // Nach Dragging: Ein finales Render ohne Throttling
        _isRenderScheduled = false;
        InvalidateVisual();
    }

    private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
   if (ViewModel == null) return;

        var viewport = ViewModel.VPWindow;
        var zoomFactor = e.Delta.Y > 0 ? 0.9f : 1.1f;

     var newWidth = viewport.Width * zoomFactor;
        var newHeight = viewport.Height * zoomFactor;

  var centerX = viewport.Left + viewport.Width / 2;
        var centerY = viewport.Top + viewport.Height / 2;

        ViewModel.VPWindow = new RectangleF(
 centerX - newWidth / 2,
 centerY - newHeight / 2,
          newWidth,
            newHeight);

  e.Handled = true;
    }
}
