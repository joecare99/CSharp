using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;

namespace GenFreeBrowser.Views.Behaviors
{
    public class PanZoomBehavior : Behavior<FrameworkElement>
    {
        public double MinScale
        {
            get => (double)GetValue(MinScaleProperty);
            set => SetValue(MinScaleProperty, value);
        }
        public static readonly DependencyProperty MinScaleProperty =
            DependencyProperty.Register(nameof(MinScale), typeof(double), typeof(PanZoomBehavior), new PropertyMetadata(0.1));

        public double MaxScale
        {
            get => (double)GetValue(MaxScaleProperty);
            set => SetValue(MaxScaleProperty, value);
        }
        public static readonly DependencyProperty MaxScaleProperty =
            DependencyProperty.Register(nameof(MaxScale), typeof(double), typeof(PanZoomBehavior), new PropertyMetadata(8.0));

        public double ZoomStep
        {
            get => (double)GetValue(ZoomStepProperty);
            set => SetValue(ZoomStepProperty, value);
        }
        public static readonly DependencyProperty ZoomStepProperty =
            DependencyProperty.Register(nameof(ZoomStep), typeof(double), typeof(PanZoomBehavior), new PropertyMetadata(0.1));

        public bool CtrlWheelToZoom
        {
            get => (bool)GetValue(CtrlWheelToZoomProperty);
            set => SetValue(CtrlWheelToZoomProperty, value);
        }
        public static readonly DependencyProperty CtrlWheelToZoomProperty =
            DependencyProperty.Register(nameof(CtrlWheelToZoom), typeof(bool), typeof(PanZoomBehavior), new PropertyMetadata(true));

        private Point? _panStart;
        private TranslateTransform? _translate;
        private ScaleTransform? _scale;

        // Keep handler instances so we can remove them
        private MouseWheelEventHandler? _wheelHandler;
        private MouseButtonEventHandler? _downHandler;
        private MouseEventHandler? _moveHandler;
        private MouseButtonEventHandler? _upHandler;

        protected override void OnAttached()
        {
            base.OnAttached();
            EnsureTransforms();

            AssociatedObject.Loaded += AssociatedObject_Loaded;
            AssociatedObject.MouseLeave += OnMouseLeave;
            AssociatedObject.KeyDown += OnKeyDown;
            AssociatedObject.Focusable = true;

            // Use Preview* and handledEventsToo=true so we receive events even if children mark them handled
            _wheelHandler = OnMouseWheel;
            _downHandler = OnMouseDown;
            _moveHandler = OnMouseMove;
            _upHandler = OnMouseUp;

            AssociatedObject.AddHandler(UIElement.PreviewMouseWheelEvent, _wheelHandler, true);
            AssociatedObject.AddHandler(UIElement.PreviewMouseDownEvent, _downHandler, true);
            AssociatedObject.AddHandler(UIElement.PreviewMouseMoveEvent, _moveHandler, true);
            AssociatedObject.AddHandler(UIElement.PreviewMouseUpEvent, _upHandler, true);
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            AssociatedObject.MouseLeave -= OnMouseLeave;
            AssociatedObject.KeyDown -= OnKeyDown;

            if (_wheelHandler != null)
                AssociatedObject.RemoveHandler(UIElement.PreviewMouseWheelEvent, _wheelHandler);
            if (_downHandler != null)
                AssociatedObject.RemoveHandler(UIElement.PreviewMouseDownEvent, _downHandler);
            if (_moveHandler != null)
                AssociatedObject.RemoveHandler(UIElement.PreviewMouseMoveEvent, _moveHandler);
            if (_upHandler != null)
                AssociatedObject.RemoveHandler(UIElement.PreviewMouseUpEvent, _upHandler);

            base.OnDetaching();
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            EnsureTransforms();
        }

        private void EnsureTransforms()
        {
            if (AssociatedObject == null) return;

            if (AssociatedObject.RenderTransform is not TransformGroup group)
            {
                group = new TransformGroup();
                _scale = new ScaleTransform(1.0, 1.0);
                _translate = new TranslateTransform(0, 0);
                group.Children.Add(_scale);
                group.Children.Add(_translate);
                AssociatedObject.RenderTransform = group;
                AssociatedObject.RenderTransformOrigin = new Point(0, 0);
            }
            else
            {
                _scale = null;
                _translate = null;
                foreach (var t in group.Children)
                {
                    if (t is ScaleTransform s) _scale ??= s;
                    if (t is TranslateTransform tr) _translate ??= tr;
                }
                _scale ??= new ScaleTransform(1.0, 1.0);
                _translate ??= new TranslateTransform(0, 0);
                if (!group.Children.Contains(_scale)) group.Children.Insert(0, _scale);
                if (!group.Children.Contains(_translate)) group.Children.Add(_translate);
            }
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (AssociatedObject == null || _scale == null || _translate == null) return;

            bool zooming = !CtrlWheelToZoom || (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
            if (!zooming) return; // Let ScrollViewer handle scrolling

            e.Handled = true;
            AssociatedObject.Focus();

            var position = e.GetPosition(AssociatedObject);
            var oldScale = _scale.ScaleX;
            var delta = e.Delta > 0 ? ZoomStep : -ZoomStep;
            var newScale = Math.Clamp(oldScale + delta, MinScale, MaxScale);
            if (Math.Abs(newScale - oldScale) < 0.0001) return;

            // Zoom around mouse position
            var scaleFactor = newScale / oldScale;
            _scale.ScaleX = newScale;
            _scale.ScaleY = newScale;

            // Adjust translation so the zoom centers on the mouse position
            _translate.X = position.X - scaleFactor * (position.X - _translate.X);
            _translate.Y = position.Y - scaleFactor * (position.Y - _translate.Y);
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (AssociatedObject == null || _translate == null) return;

            bool clickOnBackground = ReferenceEquals(e.OriginalSource, AssociatedObject);

            if (e.ChangedButton is MouseButton.Middle || e.RightButton == MouseButtonState.Pressed ||
                e.LeftButton == MouseButtonState.Pressed && (Keyboard.Modifiers == ModifierKeys.Control || clickOnBackground || Keyboard.IsKeyDown(Key.Space)))
            {
                _panStart = e.GetPosition(AssociatedObject);
                AssociatedObject.CaptureMouse();
                AssociatedObject.Cursor = Cursors.SizeAll;
                e.Handled = true;
                AssociatedObject.Focus();
            }
            else if (e.ChangedButton == MouseButton.Left)
            {
                // Left double click to reset (on background or with Ctrl)
                if (e.ClickCount == 2 && (Keyboard.Modifiers == ModifierKeys.Control || clickOnBackground))
                {
                    ResetTransform();
                    e.Handled = true;
                }
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_panStart is null || AssociatedObject == null || _translate == null) return;
            var current = e.GetPosition(AssociatedObject);
            var dx = current.X - _panStart.Value.X;
            var dy = current.Y - _panStart.Value.Y;
            _translate.X += dx;
            _translate.Y += dy;
            _panStart = current;
            e.Handled = true;
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_panStart is not null)
            {
                _panStart = null;
                AssociatedObject?.ReleaseMouseCapture();
                AssociatedObject!.Cursor = null;
                e.Handled = true;
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (_panStart is not null)
            {
                _panStart = null;
                AssociatedObject?.ReleaseMouseCapture();
                AssociatedObject!.Cursor = null;
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (_scale == null || _translate == null) return;
            const double panStep = 30;

            if (e.Key == Key.D0 && Keyboard.Modifiers == ModifierKeys.Control)
            {
                ResetTransform();
                e.Handled = true;
                return;
            }

            if ((e.Key == Key.Add || e.Key == Key.OemPlus) && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                ZoomAtCenter(+ZoomStep);
                e.Handled = true;
                return;
            }
            if ((e.Key == Key.Subtract || e.Key == Key.OemMinus) && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                ZoomAtCenter(-ZoomStep);
                e.Handled = true;
                return;
            }

            switch (e.Key)
            {
                case Key.Left:
                    _translate.X += panStep;
                    e.Handled = true;
                    break;
                case Key.Right:
                    _translate.X -= panStep;
                    e.Handled = true;
                    break;
                case Key.Up:
                    _translate.Y += panStep;
                    e.Handled = true;
                    break;
                case Key.Down:
                    _translate.Y -= panStep;
                    e.Handled = true;
                    break;
            }
        }

        private void ZoomAtCenter(double delta)
        {
            if (AssociatedObject == null || _scale == null || _translate == null) return;
            var oldScale = _scale.ScaleX;
            var newScale = Math.Clamp(oldScale + delta, MinScale, MaxScale);
            if (Math.Abs(newScale - oldScale) < 0.0001) return;

            var center = new Point(AssociatedObject.ActualWidth / 2, AssociatedObject.ActualHeight / 2);
            var scaleFactor = newScale / oldScale;
            _scale.ScaleX = newScale;
            _scale.ScaleY = newScale;
            _translate.X = center.X - scaleFactor * (center.X - _translate.X);
            _translate.Y = center.Y - scaleFactor * (center.Y - _translate.Y);
        }

        private void ResetTransform()
        {
            if (_scale == null || _translate == null) return;
            _scale.ScaleX = 1.0;
            _scale.ScaleY = 1.0;
            _translate.X = 0;
            _translate.Y = 0;
        }
    }
}
