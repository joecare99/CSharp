using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using AA06_Converters_4.Models.Interfaces;
using AA06_Converters_4.View.Controls;
using AA06_Converters_4.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.MSTest;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using AvaloniaSize = Avalonia.Size;

namespace AA06_Converters_4.View.Controls.Tests;

[TestClass]
public class DynamicPlotCanvasTests
{
    private IAGVModel _model = null!;
    private PlotFrameViewModel _viewModel = null!;

    [TestInitialize]
    public void Initialize()
    {
        _model = Substitute.For<IAGVModel>();
        _model.VehicleDim.Returns(new MathLibrary.TwoDim.Math2d.Vector(2000d, 1200d));
        _model.SwivelKoor.Returns(new MathLibrary.TwoDim.Math2d.Vector(800d, -200d));
        _model.AxisOffset.Returns(400d);
        _model.AGVVelocity.Returns(new MathLibrary.TwoDim.Math2d.Vector(20d, 10d));
        _viewModel = new PlotFrameViewModel(_model);
    }

    [AvaloniaTestMethod]
    public void Constructor_SetsDefaultsAndCreatesProperties()
    {
        var canvas = new DynamicPlotCanvas();

        Assert.IsTrue(canvas.ClipToBounds);
        Assert.AreSame(Brushes.White, canvas.Background);
        Assert.IsNull(canvas.ViewModel);
        Assert.IsNotNull(DynamicPlotCanvas.ViewModelProperty);
        Assert.IsNotNull(DynamicPlotCanvas.BackgroundProperty);
    }

    [AvaloniaTestMethod]
    public void Properties_RoundTripValues()
    {
        var canvas = new DynamicPlotCanvas();
        var background = Brushes.LightBlue;

        canvas.ViewModel = _viewModel;
        canvas.Background = background;

        Assert.AreSame(_viewModel, canvas.ViewModel);
        Assert.AreSame(background, canvas.Background);
    }

    [AvaloniaTestMethod]
    public void ViewModelAssignment_UpdatesWindowSizeAndSubscribesToModel()
    {
        var canvas = new DynamicPlotCanvas
        {
            ViewModel = _viewModel
        };

        canvas.Width = 800;
        canvas.Height = 600;
        canvas.Measure(new AvaloniaSize(800, 600));
        canvas.Arrange(new Rect(0, 0, 800, 600));

        Assert.AreEqual(new AvaloniaSize(800, 600), _viewModel.WindowSize);
        _model.PropertyChanged += Raise.Event<System.ComponentModel.PropertyChangedEventHandler>(
            _model,
            new System.ComponentModel.PropertyChangedEventArgs(nameof(IAGVModel.VehicleDim)));
    }

    [AvaloniaTestMethod]
    public void BoundsChange_UpdatesViewModelWindowSize()
    {
        var canvas = new DynamicPlotCanvas
        {
            ViewModel = _viewModel
        };

        canvas.Measure(new AvaloniaSize(640, 480));
        canvas.Arrange(new Rect(0, 0, 640, 480));

        Assert.AreEqual(new AvaloniaSize(640, 480), _viewModel.WindowSize);
    }

    [AvaloniaTestMethod]
    public void ViewModelReplacement_RemovesOldModelSubscription()
    {
        var oldModel = _model;
        var newModel = Substitute.For<IAGVModel>();
        newModel.VehicleDim.Returns(new MathLibrary.TwoDim.Math2d.Vector(1, 1));
        newModel.SwivelKoor.Returns(new MathLibrary.TwoDim.Math2d.Vector(1, 1));
        var newViewModel = new PlotFrameViewModel(newModel);
        var canvas = new DynamicPlotCanvas { ViewModel = _viewModel };

        canvas.ViewModel = newViewModel;

        oldModel.PropertyChanged -= Arg.Any<System.ComponentModel.PropertyChangedEventHandler>();
        newModel.PropertyChanged += Arg.Any<System.ComponentModel.PropertyChangedEventHandler>();
        Assert.AreSame(newViewModel, canvas.ViewModel);
    }

    [AvaloniaTestMethod]
    public void Render_HandlesEmptyCanvasAndConfiguredBackground()
    {
        var canvas = new DynamicPlotCanvas
        {
            Width = 400,
            Height = 300,
            Background = Brushes.Beige
        };
        canvas.Measure(new AvaloniaSize(400, 300));
        canvas.Arrange(new Rect(0, 0, 400, 300));

        using var bitmap = new RenderTargetBitmap(new PixelSize(400, 300));
        using var context = bitmap.CreateDrawingContext();
        canvas.Render(context);

        Assert.AreEqual(400, canvas.Bounds.Width);
        Assert.AreEqual(300, canvas.Bounds.Height);
    }

    [AvaloniaTestMethod]
    public void Render_DrawsConfiguredViewModelData()
    {
        var canvas = new DynamicPlotCanvas
        {
            Width = 800,
            Height = 600,
            ViewModel = _viewModel
        };
        canvas.Measure(new AvaloniaSize(800, 600));
        canvas.Arrange(new Rect(0, 0, 800, 600));

        using var bitmap = new RenderTargetBitmap(new PixelSize(800, 600));
        using var context = bitmap.CreateDrawingContext();
        canvas.Render(context);

        Assert.AreEqual(800, canvas.Bounds.Width);
        Assert.AreEqual(600, canvas.Bounds.Height);
    }

    [TestMethod]
    public void CalculateGridStep_UsesExpectedScaleBands()
    {
        var canvas = new DynamicPlotCanvas();

        Assert.AreEqual(0.2d, InvokePrivate<double>(canvas, "CalculateGridStep", 1d, 2d));
        Assert.AreEqual(0.5d, InvokePrivate<double>(canvas, "CalculateGridStep", 3d, 4d));
        Assert.AreEqual(1d, InvokePrivate<double>(canvas, "CalculateGridStep", 6d, 4d));
        Assert.AreEqual(100d, InvokePrivate<double>(canvas, "CalculateGridStep", 600d, 400d));
    }

    [AvaloniaTestMethod]
    public void CoordinateTransforms_AreInverseOperations()
    {
        var canvas = new DynamicPlotCanvas();
        canvas.Measure(new AvaloniaSize(800, 600));
        canvas.Arrange(new Rect(0, 0, 800, 600));
        var viewport = new RectangleF(-100, -50, 200, 100);
        var realPoint = new PointF(25, 10);

        var visualPoint = InvokePrivate<Avalonia.Point>(canvas, "Real2Vis", realPoint, viewport);
        var convertedPoint = InvokePrivate<PointF>(canvas, "Vis2Real", visualPoint, viewport);

        Assert.AreEqual(realPoint.X, convertedPoint.X, 0.001f);
        Assert.AreEqual(realPoint.Y, convertedPoint.Y, 0.001f);
    }

    [TestMethod]
    public void IsometricViewport_UsesControlAspectRatio()
    {
        var canvas = new DynamicPlotCanvas { ViewModel = _viewModel };
        canvas.Measure(new AvaloniaSize(800, 600));
        canvas.Arrange(new Rect(0, 0, 800, 600));
        InvokePrivate(canvas, "UpdateIsometricViewport");

        var viewport = GetPrivate<RectangleF>(canvas, "_isometricViewport");

        Assert.AreEqual(_viewModel.VPWindow.Width, viewport.Width, 0.001f);
        Assert.IsTrue(viewport.Height > _viewModel.VPWindow.Height);
    }

    [TestMethod]
    public void PointerHandlers_IgnoreEventsWithoutViewModel()
    {
        var canvas = new DynamicPlotCanvas();

        InvokeEventHandler(canvas, "OnPointerPressed", "PointerPressedEventArgs");
        InvokeEventHandler(canvas, "OnPointerMoved", "PointerEventArgs");
        InvokeEventHandler(canvas, "OnPointerWheelChanged", "PointerWheelEventArgs");

        Assert.IsFalse(GetPrivate<bool>(canvas, "_isDragging"));
        Assert.IsNull(GetPrivate<PointF?>(canvas, "_dragStartPos"));
        Assert.IsNull(GetPrivate<RectangleF?>(canvas, "_dragStartViewport"));
    }

    [TestMethod]
    public void PointerReleased_ResetsDragState()
    {
        var canvas = new DynamicPlotCanvas();

        SetPrivate(canvas, "_isDragging", true);
        SetPrivate(canvas, "_dragStartPos", new PointF(1, 2));
        SetPrivate(canvas, "_dragStartViewport", new RectangleF(1, 2, 3, 4));

        InvokeEventHandler(canvas, "OnPointerReleased", "PointerReleasedEventArgs");

        Assert.IsFalse(GetPrivate<bool>(canvas, "_isDragging"));
        Assert.IsNull(GetPrivate<PointF?>(canvas, "_dragStartPos"));
        Assert.IsNull(GetPrivate<RectangleF?>(canvas, "_dragStartViewport"));
    }

    private static T InvokePrivate<T>(object target, string name, params object[] arguments)
    {
        return (T)InvokePrivate(target, name, arguments)!;
    }

    private static object? InvokePrivate(object target, string name, params object[] arguments)
    {
        var method = target.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(method, $"Private method '{name}' was not found.");
        return method!.Invoke(target, arguments);
    }

    private static T GetPrivate<T>(object target, string name)
    {
        var field = target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(field, $"Private field '{name}' was not found.");
        return (T)field!.GetValue(target)!;
    }

    private static void SetPrivate(object target, string name, object? value)
    {
        var field = target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(field, $"Private field '{name}' was not found.");
        field!.SetValue(target, value);
    }

    private static void InvokeEventHandler(object target, string name, string eventTypeName)
    {
        var method = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
            .Single(candidate => candidate.Name == name
                && candidate.GetParameters().Length == 2
                && candidate.GetParameters()[1].ParameterType.Name == eventTypeName);
        method.Invoke(target, new object?[] { null, null });
    }
}
