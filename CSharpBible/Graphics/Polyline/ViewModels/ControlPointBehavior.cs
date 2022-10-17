// ***********************************************************************
// Assembly         : Polyline_net
// Author           : Mir
// Created          : 06-22-2022
//
// Last Modified By : Mir
// Last Modified On : 06-22-2022
// ***********************************************************************
// <copyright file="ControlPointBehavior.cs" company="Polyline_net">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Windows;
using Microsoft.Xaml.Behaviors;


namespace Polyline.ViewModels
{
    /// <summary>
    /// Class ControlPointBehavior.
    /// Implements the <see cref="Microsoft.Xaml.Behaviors.Behavior{System.Windows.FrameworkElement}" />
    /// </summary>
    /// <seealso cref="Microsoft.Xaml.Behaviors.Behavior{System.Windows.FrameworkElement}" />
    public class ControlPointBehavior : Behavior<FrameworkElement>
    {
        /// <summary>
        /// The mouse down
        /// </summary>
        private bool mouseDown;
        /// <summary>
        /// The delta
        /// </summary>
        private Vector delta;

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>Override this to hook up functionality to the AssociatedObject.</remarks>
        protected override void OnAttached()
        {
            var iObjParent = AssociatedObject.Parent as IInputElement;

            AssociatedObject.MouseLeftButtonDown += (s, e) =>
            {
                mouseDown = true;
                var mousePosition = e.GetPosition(iObjParent);
                var elementPosition = (AssociatedObject.DataContext as Coordinate)?.Point ?? new Point(0,0);
                delta = elementPosition - mousePosition;
                AssociatedObject.CaptureMouse();
            };

            AssociatedObject.MouseMove += (s, e) =>
            {
                if (!mouseDown) return;
                var mousePosition = e.GetPosition(iObjParent);
                var elementPosition = mousePosition + delta;
                var c = (AssociatedObject.DataContext as Coordinate);
                if (c!=null)
                    c.Point = elementPosition;
            };

            AssociatedObject.MouseLeftButtonUp += (s, e) =>
            {
                mouseDown = false;
                AssociatedObject.ReleaseMouseCapture();
            };
        }
    }
}
