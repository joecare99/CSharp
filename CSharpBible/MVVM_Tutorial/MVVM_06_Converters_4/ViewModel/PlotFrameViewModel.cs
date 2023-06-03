// ***********************************************************************
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
using MathLibrary.TwoDim;
using MVVM.ViewModel;
using MVVM_06_Converters_4.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Documents;
using System.Windows.Media;

namespace MVVM_06_Converters_4.ViewModel
{
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
    public class PlotFrameViewModel : BaseViewModel
    {
        /// <summary>
        /// The view port
        /// </summary>
        private SWindowPort _viewPort;
        /// <summary>
        /// The dataset
        /// </summary>
        private DataSet _dataset;

        private ArrowList _arrows;
        private AGV_Model _agv_Model;
        private CircleList _circles;
        private PolynomeList _polynomes;

        /// <summary>
        /// Gets or sets the window port.
        /// </summary>
        /// <value>The window port.</value>
        public SWindowPort WindowPort { get => _viewPort; set => SetProperty(ref _viewPort, value); }

        /// <summary>
        /// Gets or sets the dataset.
        /// </summary>
        /// <value>The dataset.</value>
        public DataSet Dataset1 { get => _dataset; set => SetProperty(ref _dataset, value); }

        /// <summary>
        /// Gets or sets the dataset.
        /// </summary>
        /// <value>The dataset.</value>
        public ArrowList Arrows { get => _arrows; set => SetProperty(ref _arrows, value); }

        /// <summary>
        /// Gets or sets the dataset.
        /// </summary>
        /// <value>The dataset.</value>
        public CircleList Circles { get => _circles; set => SetProperty(ref _circles, value); }

        /// <summary>
        /// Gets or sets the dataset.
        /// </summary>
        /// <value>The dataset.</value>
        public PolynomeList Polynomes { get => _polynomes; set => SetProperty(ref _polynomes, value); }


        /// <summary>
        /// Gets or sets the vp window.
        /// </summary>
        /// <value>The vp window.</value>
        public RectangleF VPWindow { get => _viewPort.port; set => SetProperty(ref _viewPort.port, value, new string[] { nameof(WindowPort) }); }
        /// <summary>
        /// Gets or sets the size of the window.
        /// </summary>
        /// <value>The size of the window.</value>
        public System.Windows.Size WindowSize
        {
            get => _viewPort.WindowSize;
            set => SetProperty(ref _viewPort.WindowSize, value, new string[] { nameof(WindowPort) });
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
            _viewPort.Parent = this;

            _dataset = new DataSet();
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

            _agv_Model = AGV_Model.Instance;
            _agv_Model.PropertyChanged += OnModelPropChanged;

        }

        private void DemoData()
        {
            
            _dataset.Datapoints = new PointF[400];
            _dataset.Pen = new System.Windows.Media.Pen(System.Windows.Media.Brushes.Red, 1.0);
            for (int i = 0; i < _dataset.Datapoints.Length; i++)
            {
                _dataset.Datapoints[i] = GetPoint(i);
            }
           
            for (int i = 0; i < 20; i++)
            {
                var ar = new ArrowData()
                {
                    Start = GetPoint(i * 12),
                    End = GetPoint(i * 12 + 18)
                };
                _arrows.Add(ar);
            }


            for (int i = 0; i < 20; i++)
            {
                var ar = new CircleData()
                {
                    Center = GetPoint(i * 12),
                    Radius = 0.2f
                };
                _circles.Add(ar);
            }


            for (int i = 0; i < 20; i++)
            {
                var pd = new PolynomeData()
                {
                    Points = new()
                };
                for (int j = 0; j < 4; j++)
                    pd.Points.Add(GetPoint(i * 12 + j));
                for (int j = 0; j < 4; j++)
                    pd.Points.Add(GetPoint(i * 12 + 3 - j, 0.8f));
                _polynomes.Add(pd);
            }

            static PointF GetPoint(int i, float f = 1f)
            {
                return new PointF((float)(Math.Sin(i / 100.0f * Math.PI) * f + Math.Sin(i / 16.0f * Math.PI) * 1.25 * f), (float)(Math.Cos(i / 100.0f * Math.PI) * f + Math.Cos(i / 16.0f * Math.PI) * 1.25 * f));
            }

        }

        private void OnModelPropChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Has to be Optimized
            _polynomes.Clear();
            _circles.Clear();
            _arrows.Clear();
            _polynomes.Add(new()
            {
                Points = MakeRect(_agv_Model.VehicleDim,0.5f,0.5f)
            });
            _circles.Add(new()
            {
                Center = new((float)_agv_Model.SwivelKoor.x, (float)_agv_Model.SwivelKoor.y),
                Radius = _agv_Model.AxisOffset * 0.4f
            });
            _circles.Add(new()
            {
                Center = new(-(float)_agv_Model.SwivelKoor.x, -(float)_agv_Model.SwivelKoor.y),
                Radius = _agv_Model.AxisOffset * 0.4f
            });
            // Rad 1
            _polynomes.Add(new()
            {
                Points = MakeRotRect(_agv_Model.SwivelKoor,_agv_Model.Swivel1Angle,Math2d.ByLengthAngle(_agv_Model.AxisOffset*0.5,Math.PI*0.5), 0.1f, 0.3f)
            });
            _arrows.Add(new()
            {
                Start = MakeRotArrow(_agv_Model.SwivelKoor, _agv_Model.Swivel1Angle, Math2d.ByLengthAngle(1, Math.PI * 0.5), 0d, _agv_Model.AxisOffset * 0.5),
                End = MakeRotArrow(_agv_Model.SwivelKoor, _agv_Model.Swivel1Angle, Math2d.ByLengthAngle(1, Math.PI * 0.5), _agv_Model.Wheel1Velocity, _agv_Model.AxisOffset * 0.5)
            });
            // Rad 2
            _polynomes.Add(new()
            {
                Points = MakeRotRect(_agv_Model.SwivelKoor, _agv_Model.Swivel1Angle, Math2d.ByLengthAngle(_agv_Model.AxisOffset * 0.5, -Math.PI * 0.5), 0.1f, 0.3f)
            });
            _arrows.Add(new()
            {
                Start = MakeRotArrow(_agv_Model.SwivelKoor, _agv_Model.Swivel1Angle, Math2d.ByLengthAngle(1, Math.PI * 0.5), 0d, _agv_Model.AxisOffset * -0.5),
                End = MakeRotArrow(_agv_Model.SwivelKoor, _agv_Model.Swivel1Angle, Math2d.ByLengthAngle(1, Math.PI * 0.5), _agv_Model.Wheel2Velocity, _agv_Model.AxisOffset * -0.5)
            });
            _arrows.Add(new()
            {
                Start = MakeRotArrow(_agv_Model.SwivelKoor, _agv_Model.Swivel1Angle, Math2d.ByLengthAngle(1, Math.PI * 0.5), 0d, 0),
                End = MakeRotArrow(_agv_Model.SwivelKoor, _agv_Model.Swivel1Angle, Math2d.ByLengthAngle(1, Math.PI * 0.5), _agv_Model.Swivel1Velocity, 0)
            });
            _arrows.Add(new()
            {
                Start = MakeRotArrow(_agv_Model.SwivelKoor, _agv_Model.Swivel1Angle, Math2d.ByLengthAngle(1, Math.PI * 0.5), 0d, _agv_Model.AxisOffset * 0.4),
                End = MakeRotArrow(_agv_Model.SwivelKoor, _agv_Model.Swivel1Angle, Math2d.ByLengthAngle(1, Math.PI * 0.5), _agv_Model.Swivel1Rot * _agv_Model.AxisOffset * -0.4, _agv_Model.AxisOffset * 0.4)
            });
            // Rad 3
            _polynomes.Add(new()
            {
                Points = MakeRotRect(_agv_Model.SwivelKoor.Mult(-1), _agv_Model.Swivel2Angle, Math2d.ByLengthAngle(_agv_Model.AxisOffset * 0.5, Math.PI * 0.5), 0.1f, 0.3f)
            });
            _arrows.Add(new()
            {
                Start = MakeRotArrow(_agv_Model.SwivelKoor.Mult(-1), _agv_Model.Swivel2Angle, Math2d.ByLengthAngle(1, Math.PI * 0.5), 0d, _agv_Model.AxisOffset * 0.5),
                End = MakeRotArrow(_agv_Model.SwivelKoor.Mult(-1), _agv_Model.Swivel2Angle, Math2d.ByLengthAngle(1, Math.PI * 0.5), _agv_Model.Wheel3Velocity, _agv_Model.AxisOffset * 0.5)
            });
            // Rad 4
            _polynomes.Add(new()
            {
                Points = MakeRotRect(_agv_Model.SwivelKoor.Mult(-1), _agv_Model.Swivel2Angle, Math2d.ByLengthAngle(_agv_Model.AxisOffset * 0.5, -Math.PI * 0.5), 0.1f, 0.3f)
            });
            _arrows.Add(new()
            {
                Start = MakeRotArrow(_agv_Model.SwivelKoor.Mult(-1), _agv_Model.Swivel2Angle, Math2d.ByLengthAngle(1, Math.PI * 0.5), 0d, _agv_Model.AxisOffset * -0.5),
                End = MakeRotArrow(_agv_Model.SwivelKoor.Mult(-1), _agv_Model.Swivel2Angle, Math2d.ByLengthAngle(1, Math.PI * 0.5), _agv_Model.Wheel4Velocity, _agv_Model.AxisOffset * -0.5)
            });
            _arrows.Add(new()
            {
                Start = MakeRotArrow(_agv_Model.SwivelKoor.Mult(-1), _agv_Model.Swivel2Angle, Math2d.ByLengthAngle(1, Math.PI * 0.5), 0d, 0 ),
                End = MakeRotArrow(_agv_Model.SwivelKoor.Mult(-1), _agv_Model.Swivel2Angle, Math2d.ByLengthAngle(1, Math.PI * 0.5), _agv_Model.Swivel2Velocity, 0)
            });
            _arrows.Add(new()
            {
                Start = MakeRotArrow(_agv_Model.SwivelKoor.Mult(-1), _agv_Model.Swivel2Angle, Math2d.ByLengthAngle(1, Math.PI * 0.5), 0d, _agv_Model.AxisOffset * 0.4),
                End = MakeRotArrow(_agv_Model.SwivelKoor.Mult(-1), _agv_Model.Swivel2Angle, Math2d.ByLengthAngle(1, Math.PI * 0.5), _agv_Model.Swivel2Rot* _agv_Model.AxisOffset*-0.4, _agv_Model.AxisOffset * 0.4)
            });
            // AGV-Velocity
            _arrows.Add(new()
            {
                Start = PointF.Empty,
                End = MakeRotArrow(_agv_Model.AGVVelocity, 0, Math2d.eY,0d,0d)
            });

            RaisePropertyChanged(nameof(Polynomes));
            RaisePropertyChanged(nameof(Circles));
            RaisePropertyChanged(nameof(Arrows));

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
}
