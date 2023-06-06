using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CanvasWPF2_CTItemTemplateSelector.ViewModel
{
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
                var elementPosition = (AssociatedObject.DataContext as ShapeData)?.point ?? new Point(0, 0);
                delta = elementPosition - mousePosition;
                AssociatedObject.CaptureMouse();
            };

            AssociatedObject.MouseMove += (s, e) =>
            {
                if (!mouseDown) return;
                var mousePosition = e.GetPosition(iObjParent);
                var elementPosition = mousePosition + delta;
                var c = (AssociatedObject.DataContext as ShapeData);
                if (c != null)
                    c.point = elementPosition;
                (AssociatedObject.DataContext as ShapeData)?.MouseHover.Execute(AssociatedObject.DataContext);
            };

            AssociatedObject.MouseLeftButtonUp += (s, e) =>
            {
                mouseDown = false;
                AssociatedObject.ReleaseMouseCapture();
            };

            AssociatedObject.MouseEnter += (s, e) =>
            {
                (AssociatedObject.DataContext as ShapeData)?.MouseHover.Execute(AssociatedObject.DataContext);
            };
        }
    }
}
