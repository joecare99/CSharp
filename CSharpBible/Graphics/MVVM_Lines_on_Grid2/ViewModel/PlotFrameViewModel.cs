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
using MVVM.ViewModel;
using System;
using System.Drawing;
using System.Windows.Media;

namespace MVVM_Lines_on_Grid2.ViewModel
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
        /// Gets or sets the vp window.
        /// </summary>
        /// <value>The vp window.</value>
        public RectangleF VPWindow { get => _viewPort.port; set => SetProperty(ref _viewPort.port, value,new string[] { nameof(WindowPort) }); }
        /// <summary>
        /// Gets or sets the size of the window.
        /// </summary>
        /// <value>The size of the window.</value>
        public System.Windows.Size WindowSize { get=>_viewPort.WindowSize; 
            set => SetProperty(ref _viewPort.WindowSize, value, new string[] { nameof(WindowPort) }); }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlotFrameViewModel"/> class.
        /// </summary>
        public PlotFrameViewModel()
        {
 //           VPWindow = new RectangleF(-300, 300, 9, 6);
            VPWindow = new RectangleF(-3, -3, 9, 6);
            //           VPWindow = new RectangleF(-3, -3, 900, 600);
 //           VPWindow = new RectangleF(-0.03f, -0.03f, 0.09f, 0.06f);
            WindowSize = new System.Windows.Size(600, 400);
            _viewPort.Parent = this;
            _dataset = new DataSet();
            _dataset.Datapoints = new PointF[100];
            _dataset.Pen = new System.Windows.Media.Pen(Brushes.Red, 3.0);
            for (int i = 0; i < 100; i++)
            {
                _dataset.Datapoints[i] = new PointF((float)(Math.Sin(i / 50.0f * Math.PI)+ Math.Sin(i / 8.0f * Math.PI)*1.25), (float)(Math.Cos(i/50.0f * Math.PI) + Math.Cos(i / 8.0f * Math.PI) * 1.25));
            }
            AddPropertyDependency(nameof(Dataset1), nameof(WindowPort));
        } 
    }
}
