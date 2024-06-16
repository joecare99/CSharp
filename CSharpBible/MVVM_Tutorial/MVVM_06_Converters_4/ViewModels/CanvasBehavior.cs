// ***********************************************************************
// Assembly         : MVVM_Lines_on_Grid
// Author           : Mir
// Created          : 08-28-2022
//
// Last Modified By : Mir
// Last Modified On : 08-29-2022
// ***********************************************************************
// <copyright file="CanvasBehavior.cs" company="JC-Soft">
//     (c) by Joe Care 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;
using MVVM_06_Converters_4.View.Converter;

namespace MVVM_06_Converters_4.ViewModels
{
    /// <summary>
    /// Class CanvasBehavior.
    /// Implements the <see cref="Microsoft.Xaml.Behaviors.Behavior{System.Windows.FrameworkElement}" />
    /// </summary>
    /// <seealso cref="Microsoft.Xaml.Behaviors.Behavior{System.Windows.FrameworkElement}" />
    public class CanvasBehavior : Behavior<FrameworkElement>
    {
        public PointF? RealPos { get; private set; }

        private bool xPressed;

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>Override this to hook up functionality to the AssociatedObject.</remarks>
        protected override void OnAttached()
        {
            var iObjParent = AssociatedObject.Parent as Page;
            var cCoordinateConverter = iObjParent?.Resources["vcPortGrid"] as WindowPortToGridLines;            

            AssociatedObject.MouseWheel += (s, e) =>
            {
                if (AssociatedObject.DataContext is PlotFrameViewModel vm)
                {
                    var mousePosition = e.GetPosition(s as IInputElement);
                    System.Drawing.RectangleF ActPort = vm.VPWindow;
                    if (e.Delta < 0)
                    {
                        ActPort.Inflate(ActPort.Size.Width * 0.1f, ActPort.Size.Height * 0.1f);
                        vm.VPWindow = ActPort;
                    }
                    else if (e.Delta > 0)
                    {
                        ActPort.Inflate(-ActPort.Size.Width * 0.1f, -ActPort.Size.Height * 0.1f);
                        vm.VPWindow = ActPort;
                    }

                }
            };

            AssociatedObject.MouseLeftButtonDown += (s, e) =>
            {
                if (AssociatedObject.DataContext is PlotFrameViewModel vm)
                { 
                    var mousePosition = e.GetPosition(s as IInputElement);
                    RealPos = cCoordinateConverter?.Vis2RealP(mousePosition, cCoordinateConverter.GetAdjustedRect(vm.WindowPort));
                    xPressed = true;
                }
            };

            AssociatedObject.MouseLeftButtonUp += (s, e) =>
            {
                if (AssociatedObject.DataContext is PlotFrameViewModel vm)
                {
                    var mousePosition = e.GetPosition(s as IInputElement);
                    var NewPos = cCoordinateConverter?.Vis2RealP(mousePosition, cCoordinateConverter.GetAdjustedRect(vm.WindowPort));
                    xPressed = false;
                }
            };

            AssociatedObject.MouseMove += (s, e) =>
            {
                if (AssociatedObject.DataContext is PlotFrameViewModel vm)
                {
                    var mousePosition = e.GetPosition(s as IInputElement);
                    var NewPos = cCoordinateConverter?.Vis2RealP(mousePosition, cCoordinateConverter.GetAdjustedRect(vm.WindowPort));
                    if (xPressed && NewPos!=null)
                    {
                        System.Drawing.RectangleF ActPort = vm.VPWindow;
                        ActPort.Offset( -NewPos.Value.X + RealPos?.X ?? 0f,-NewPos.Value.Y+RealPos?.Y ?? 0f);
                        vm.VPWindow = ActPort;
                    }
                }
            };


        }
    }
}
