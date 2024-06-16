using System.Windows;
using Microsoft.Xaml.Behaviors;


namespace MVVM_TiledDisplay.ViewModel
{
    /// <summary>
    /// Class TileBehavior.
    /// Implements the <see cref="Microsoft.Xaml.Behaviors.Behavior{System.Windows.FrameworkElement}" />
    /// </summary>
    /// <seealso cref="Microsoft.Xaml.Behaviors.Behavior{System.Windows.FrameworkElement}" />
    public class TileBehavior : Behavior<FrameworkElement>
    {

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>Override this to hook up functionality to the AssociatedObject.</remarks>
        protected override void OnAttached()
        {
            var iObjParent = AssociatedObject.Parent as IInputElement;

            AssociatedObject.MouseLeftButtonDown += (s, e) =>
            {
            };

            AssociatedObject.MouseMove += (s, e) =>
            {
            };

            AssociatedObject.MouseLeftButtonUp += (s, e) =>
            {
            };
        }
    }
}
