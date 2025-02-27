﻿// ***********************************************************************
// Assembly         : MVVM_Lines_on_Grid
// Author           : Mir
// Created          : 08-28-2022
//
// Last Modified By : Mir
// Last Modified On : 08-28-2022
// ***********************************************************************
// <copyright file="PlotFrameViewModel.cs" company="JC-Soft">
//     (c) by Joe Care 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.ComponentModel;
using MathLibrary.TwoDim;
using BaseLib.Helper;
using MVVM.ViewModel;
using MVVM_06_Converters_4.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;

namespace MVVM_06_Converters_4.ViewModels;

/// <summary>
/// Struct SWindowPort
/// </summary>
public struct SWindowPort
{
    /// <summary>
    /// The port
    /// </summary>
    public RectangleF port;
    /// <summary>
    /// The window size
    /// </summary>
    public System.Windows.Size WindowSize;
    /// <summary>
    /// The parent
    /// </summary>
    public PlotFrameViewModel Parent;
}

/// <summary>
/// Struct DataSet
/// </summary>
public struct DataSet
{
    /// <summary>
    /// The datapoints
    /// </summary>
    public PointF[] Datapoints;
    /// <summary>
    /// The name
    /// </summary>
    public string Name;
    /// <summary>
    /// The description
    /// </summary>
    public string Description;
    /// <summary>
    /// The pen
    /// </summary>
    public System.Windows.Media.Pen Pen;
}

/// <summary>
/// Struct DataSet
/// </summary>
public class ArrowList : List<ArrowData>
{
    /// <summary>
    /// The name
    /// </summary>
    public string? Name;
    /// <summary>
    /// The description
    /// </summary>
    public string? Description;
    /// <summary>
    /// The pen
    /// </summary>
    public System.Windows.Media.Pen? Pen;
}

public struct ArrowData
{
    /// <summary>
    /// The datapoints
    /// </summary>
    public PointF Start;
    /// <summary>
    /// The datapoints
    /// </summary>
    public PointF End;
}

/// <summary>
/// Struct DataSet
/// </summary>
public class CircleList : List<CircleData>
{
    /// <summary>
    /// The name
    /// </summary>
    public string? Name;
    /// <summary>
    /// The description
    /// </summary>
    public string? Description;
    /// <summary>
    /// The pen
    /// </summary>
    public System.Windows.Media.Pen? Pen;
}
public struct CircleData
{
    /// <summary>
    /// The datapoints
    /// </summary>
    public PointF Center;
    /// <summary>
    /// The datapoints
    /// </summary>
    public double Radius;
}

/// <summary>
/// Struct DataSet
/// </summary>
public class PolynomeList : List<PolynomeData>
{
    /// <summary>
    /// The name
    /// </summary>
    public string? Name;
    /// <summary>
    /// The description
    /// </summary>
    public string? Description;
    /// <summary>
    /// The pen
    /// </summary>
    public System.Windows.Media.Pen? Pen;
}
public struct PolynomeData
{
    /// <summary>
    /// The datapoints
    /// </summary>
    public List<PointF> Points;
}


/// <summary>
/// Class PlotFrameViewModel.
/// Implements the <see cref="BaseViewModel" />
/// </summary>
/// <seealso cref="BaseViewModel" />
public partial class PlotFrameViewModel : BaseViewModelCT
{
    /// <summary>
    /// The view port
    /// </summary> 
    private SWindowPort _windowPort;
    /// <summary>
    /// The dataset
    /// </summary>
    [ObservableProperty]
    private DataSet _dataset1;
    /// <summary>
    /// Gets or sets the Arrows.
    /// </summary>
    /// <value>The dataset.</value>
    [ObservableProperty]
    private ArrowList _arrows;

    /// <summary>
    /// Gets or sets the Circles.
    /// </summary>
    /// <value>The dataset.</value>
    [ObservableProperty]
    private CircleList _circles;

    /// <summary>
    /// Gets or sets the Polinomes.
    /// </summary>
    /// <value>The dataset.</value>
    [ObservableProperty]
    private PolynomeList _polynomes;

    private IAGVModel _agv_Model;

    public SWindowPort WindowPort { get => _windowPort; set => SetProperty(ref _windowPort, value); }
    /// <summary>
    /// Gets or sets the vp window.
    /// </summary>
    /// <value>The vp window.</value>
    public RectangleF VPWindow { get => WindowPort.port; set => SetProperty(ref _windowPort.port, value ); }
    /// <summary>
    /// Gets or sets the size of the window.
    /// </summary>
    /// <value>The size of the window.</value>
    public System.Windows.Size WindowSize
    {
        get => WindowPort.WindowSize;
        set => SetProperty(ref _windowPort.WindowSize, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlotFrameViewModel"/> class.
    /// </summary>
    public PlotFrameViewModel()
    {
        //           VPWindow = new RectangleF(-300, 300, 9, 6);
        VPWindow = new RectangleF(-2000, -1500, 5000, 3000);
        //           VPWindow = new RectangleF(-3, -3, 900, 600);
        //           VPWindow = new RectangleF(-0.03f, -0.03f, 0.09f, 0.06f);
        WindowSize = new System.Windows.Size(600, 400);
        _windowPort.Parent = this;

        _dataset1 = new DataSet();
        _arrows = new ArrowList();
        _arrows.Pen = new System.Windows.Media.Pen(System.Windows.Media.Brushes.Red, 2.0);
        _circles = new CircleList();
        _circles.Pen = new System.Windows.Media.Pen(System.Windows.Media.Brushes.Green, 1.0);
        _polynomes = new PolynomeList();
        _polynomes.Pen = new System.Windows.Media.Pen(System.Windows.Media.Brushes.Blue, 2.0);

        //            DemoData();

        AddPropertyDependency(nameof(Dataset1), nameof(WindowPort), true);
        AddPropertyDependency(nameof(Arrows), nameof(WindowPort), true);
        AddPropertyDependency(nameof(Circles), nameof(WindowPort), true);
        AddPropertyDependency(nameof(Polynomes), nameof(WindowPort), true);
        AddPropertyDependency(nameof(WindowPort), nameof(WindowSize), true);
        AddPropertyDependency(nameof(WindowPort), nameof(VPWindow), true);

        _agv_Model = IoC.GetRequiredService<IAGVModel>();
        _agv_Model.PropertyChanged += OnModelPropChanged;
        AsyncInit();
    }

    async void AsyncInit()
    {
        await Task.Delay(100);
        OnModelPropChanged(_agv_Model, new PropertyChangedEventArgs(nameof(IAGVModel.VehicleDim)));
    }

    private void OnModelPropChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Has to be Optimized
        Polynomes.Clear();
        Circles.Clear();
        Arrows.Clear();
        Polynomes.Add(new()
        {
            Points = MakeRect(_agv_Model.VehicleDim,0.5f,0.5f)
        });
        if (double.IsNaN(_agv_Model.SwivelKoor.x)  || _agv_Model.SwivelKoor.y == double.NaN) return;
        Circles.Add(new()
        {
            Center = new((float)_agv_Model.SwivelKoor.x, (float)_agv_Model.SwivelKoor.y),
            Radius = _agv_Model.AxisOffset * 0.4f
        });
        Circles.Add(new()
        {
            Center = new(-(float)_agv_Model.SwivelKoor.x, -(float)_agv_Model.SwivelKoor.y),
            Radius = _agv_Model.AxisOffset * 0.4f
        });
        // Rad 1
        Polynomes.Add(new()
        {
            Points = MakeRotRect(_agv_Model.SwivelKoor,_agv_Model.Swivel1Angle,Math2d.ByLengthAngle(_agv_Model.AxisOffset*0.5,Math.PI*0.5), 0.1f, 0.3f)
        });
        Arrows.Add(new()
        {
            Start = MakeRotArrow(_agv_Model.SwivelKoor, _agv_Model.Swivel1Angle, Math2d.ByLengthAngle(1, Math.PI * 0.5), 0d, _agv_Model.AxisOffset * 0.5),
            End = MakeRotArrow(_agv_Model.SwivelKoor, _agv_Model.Swivel1Angle, Math2d.ByLengthAngle(1, Math.PI * 0.5), _agv_Model.Wheel1Velocity, _agv_Model.AxisOffset * 0.5)
        });
        // Rad 2
        Polynomes.Add(new()
        {
            Points = MakeRotRect(_agv_Model.SwivelKoor, _agv_Model.Swivel1Angle, Math2d.ByLengthAngle(_agv_Model.AxisOffset * 0.5, -Math.PI * 0.5), 0.1f, 0.3f)
        });
        Arrows.Add(new()
        {
            Start = MakeRotArrow(_agv_Model.SwivelKoor, _agv_Model.Swivel1Angle, Math2d.ByLengthAngle(1, Math.PI * 0.5), 0d, _agv_Model.AxisOffset * -0.5),
            End = MakeRotArrow(_agv_Model.SwivelKoor, _agv_Model.Swivel1Angle, Math2d.ByLengthAngle(1, Math.PI * 0.5), _agv_Model.Wheel2Velocity, _agv_Model.AxisOffset * -0.5)
        });
        Arrows.Add(new()
        {
            Start = MakeRotArrow(_agv_Model.SwivelKoor, _agv_Model.Swivel1Angle, Math2d.ByLengthAngle(1, Math.PI * 0.5), 0d, 0),
            End = MakeRotArrow(_agv_Model.SwivelKoor, _agv_Model.Swivel1Angle, Math2d.ByLengthAngle(1, Math.PI * 0.5), _agv_Model.Swivel1Velocity, 0)
        });
        Arrows.Add(new()
        {
            Start = MakeRotArrow(_agv_Model.SwivelKoor, _agv_Model.Swivel1Angle, Math2d.ByLengthAngle(1, Math.PI * 0.5), 0d, _agv_Model.AxisOffset * 0.4),
            End = MakeRotArrow(_agv_Model.SwivelKoor, _agv_Model.Swivel1Angle, Math2d.ByLengthAngle(1, Math.PI * 0.5), _agv_Model.Swivel1Rot * _agv_Model.AxisOffset * -0.4, _agv_Model.AxisOffset * 0.4)
        });
        // Rad 3
        Polynomes.Add(new()
        {
            Points = MakeRotRect(_agv_Model.SwivelKoor.Mult(-1), _agv_Model.Swivel2Angle, Math2d.ByLengthAngle(_agv_Model.AxisOffset * 0.5, Math.PI * 0.5), 0.1f, 0.3f)
        });
        Arrows.Add(new()
        {
            Start = MakeRotArrow(_agv_Model.SwivelKoor.Mult(-1), _agv_Model.Swivel2Angle, Math2d.ByLengthAngle(1, Math.PI * 0.5), 0d, _agv_Model.AxisOffset * 0.5),
            End = MakeRotArrow(_agv_Model.SwivelKoor.Mult(-1), _agv_Model.Swivel2Angle, Math2d.ByLengthAngle(1, Math.PI * 0.5), _agv_Model.Wheel3Velocity, _agv_Model.AxisOffset * 0.5)
        });
        // Rad 4
        Polynomes.Add(new()
        {
            Points = MakeRotRect(_agv_Model.SwivelKoor.Mult(-1), _agv_Model.Swivel2Angle, Math2d.ByLengthAngle(_agv_Model.AxisOffset * 0.5, -Math.PI * 0.5), 0.1f, 0.3f)
        });
        Arrows.Add(new()
        {
            Start = MakeRotArrow(_agv_Model.SwivelKoor.Mult(-1), _agv_Model.Swivel2Angle, Math2d.ByLengthAngle(1, Math.PI * 0.5), 0d, _agv_Model.AxisOffset * -0.5),
            End = MakeRotArrow(_agv_Model.SwivelKoor.Mult(-1), _agv_Model.Swivel2Angle, Math2d.ByLengthAngle(1, Math.PI * 0.5), _agv_Model.Wheel4Velocity, _agv_Model.AxisOffset * -0.5)
        });
        Arrows.Add(new()
        {
            Start = MakeRotArrow(_agv_Model.SwivelKoor.Mult(-1), _agv_Model.Swivel2Angle, Math2d.ByLengthAngle(1, Math.PI * 0.5), 0d, 0 ),
            End = MakeRotArrow(_agv_Model.SwivelKoor.Mult(-1), _agv_Model.Swivel2Angle, Math2d.ByLengthAngle(1, Math.PI * 0.5), _agv_Model.Swivel2Velocity, 0)
        });
        Arrows.Add(new()
        {
            Start = MakeRotArrow(_agv_Model.SwivelKoor.Mult(-1), _agv_Model.Swivel2Angle, Math2d.ByLengthAngle(1, Math.PI * 0.5), 0d, _agv_Model.AxisOffset * 0.4),
            End = MakeRotArrow(_agv_Model.SwivelKoor.Mult(-1), _agv_Model.Swivel2Angle, Math2d.ByLengthAngle(1, Math.PI * 0.5), _agv_Model.Swivel2Rot* _agv_Model.AxisOffset*-0.4, _agv_Model.AxisOffset * 0.4)
        });
        // AGV-Velocity
        Arrows.Add(new()
        {
            Start = PointF.Empty,
            End = MakeRotArrow(_agv_Model.AGVVelocity, 0, Math2d.eY,0d,0d)
        });

        OnPropertyChanged(nameof(Polynomes));
        OnPropertyChanged(nameof(Circles));
        OnPropertyChanged(nameof(Arrows));

        List<PointF> MakeRect(Math2d.Vector vs, float xSize,float ySize)
        {
            return new List<PointF>() {
                new ((float)vs.x * xSize,(float)vs.y * ySize),
                new ((float)vs.x * -xSize, (float)vs.y * ySize),
                new ((float)vs.x * -xSize, (float)vs.y * -ySize),
                new ((float)vs.x * xSize, (float)vs.y * -ySize)
            };
        }

        List<PointF> MakeRotRect(Math2d.Vector vO,double dAngle, Math2d.Vector vO2, double xSize, double ySize)
        {
            var vO2r=vO2.Rotate(dAngle);
            var vO2rn = vO2r.Rot90();
            return new List<PointF>() {
                new ((float)(vO.x + vO2r.x *(1f+xSize) + vO2rn.x *(+ySize)) 
                   ,(float)(vO.y + vO2r.y *(1f+xSize) + vO2rn.y *(+ySize))),
                new ((float)(vO.x + vO2r.x *(1f-xSize) + vO2rn.x *(+ySize))
                   ,(float)(vO.y + vO2r.y *(1f-xSize) + vO2rn.y *(+ySize))),
                new ((float)(vO.x + vO2r.x *(1f-xSize) + vO2rn.x *(-ySize))
                   ,(float)(vO.y + vO2r.y *(1f-xSize) + vO2rn.y *(-ySize))),
                new ((float)(vO.x + vO2r.x *(1f+xSize) + vO2rn.x *(-ySize))
                   ,(float)(vO.y + vO2r.y *(1f+xSize) + vO2rn.y *(-ySize)))
            };
        }
        PointF MakeRotArrow(Math2d.Vector vO, double dAngle, Math2d.Vector vO2, double xSize, double yOffs)
        {
            var vO2r = vO2.Rotate(dAngle).Mult(1 /vO2.Length());
            var vO2rn = vO2r.Rot90();
            return 
                new ((float)(vO.x + vO2r.x * yOffs - vO2rn.x * xSize)
                   ,(float)(vO.y + vO2r.y * yOffs - vO2rn.y * xSize));
        }
    }
}
