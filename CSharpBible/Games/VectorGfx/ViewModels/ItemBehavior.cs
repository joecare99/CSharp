﻿using Microsoft.Xaml.Behaviors;
using System.Windows;
using VectorGfx.Models.Interfaces;

namespace VectorGfx.ViewModels;

/// <summary>
/// Class ItemBehavior.
/// Implements the <see cref="Microsoft.Xaml.Behaviors.Behavior{System.Windows.FrameworkElement}" />
/// </summary>
/// <seealso cref="Microsoft.Xaml.Behaviors.Behavior{System.Windows.FrameworkElement}" />
public class ItemBehavior : Behavior<FrameworkElement>
{
    private bool mouseDown;
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
            var elementPosition = (AssociatedObject.DataContext as IHasPoint)?.P ?? new Point(0, 0);
            delta = elementPosition - mousePosition;
            AssociatedObject.CaptureMouse();
        };

        AssociatedObject.MouseMove += (s, e) =>
        {
            if (!mouseDown) return;
            var mousePosition = e.GetPosition(iObjParent);
            var elementPosition = mousePosition + delta;
            var c = (AssociatedObject.DataContext as IHasPoint);
            if (c != null)
                c.P = elementPosition;
            (AssociatedObject.DataContext as IHasPoint)?.MouseHover?.Execute(AssociatedObject.DataContext);
        };

        AssociatedObject.MouseLeftButtonUp += (s, e) =>
        {
            mouseDown = false;
            AssociatedObject.ReleaseMouseCapture();
        };

        AssociatedObject.MouseEnter += (s, e) =>
        {
            (AssociatedObject.DataContext as IHasPoint)?.MouseHover?.Execute(AssociatedObject.DataContext);
        };
    }
}