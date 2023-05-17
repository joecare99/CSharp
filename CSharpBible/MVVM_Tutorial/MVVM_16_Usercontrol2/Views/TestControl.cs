using System.Windows;
using System.Windows.Controls;

namespace MVVM_16_UserControl2.Views
{
    public class TestControl : ContentControl
    {
        public static readonly DependencyProperty _Text2Property = 
			DependencyProperty.Register(nameof(Text2), typeof(object), typeof(TestControl), 
				new FrameworkPropertyMetadata((object?)null, (PropertyChangedCallback)OnText2Changed));

		private static void OnText2Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
			=> ((TestControl)d).OnText2Changed(e.OldValue, e.NewValue); // Übergang in die Instanz


        private void OnText2Changed(object _, object newValue)
        {
            if (newValue is string @s)
                Content = $"Text2: {@s}";
        }

        public object Text2 // Nicht Anfassen (setter) !!!
        {
            get => GetValue(_Text2Property);
            set
            {
                _ = GetValue(_Text2Property);
                SetValue(_Text2Property, value); // speichert den Wert - überschreibt ggf. die Bindung zum "Binding"-Object 
//                OnText2Changed(oldValue, value);
            }
        }
    }
}
