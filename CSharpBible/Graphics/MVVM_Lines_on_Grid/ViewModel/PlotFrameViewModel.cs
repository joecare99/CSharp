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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MVVM_Lines_on_Grid.ViewModel
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
        /// Gets or sets the window port.
        /// </summary>
        /// <value>The window port.</value>
        public SWindowPort WindowPort { get => _viewPort; set => SetProperty(ref _viewPort, value); }

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
        } 
    }
}
