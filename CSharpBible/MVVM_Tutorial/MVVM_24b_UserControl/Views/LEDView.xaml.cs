using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MVVM_24b_UserControl.Views;

/// <summary>
/// Interaktionslogik für LEDView.xaml
/// </summary>
public partial class LEDView : UserControl
	{
		public static readonly DependencyProperty LEDBrushProperty =
			DependencyProperty.Register(nameof(LEDBrush), typeof(Brush),
				typeof(LEDView),
				new FrameworkPropertyMetadata(Brushes.Pink));

		public LEDView()
		{
			InitializeComponent();
			DataContext = this;
		}

		public Brush LEDBrush
		{
			get { return (Brush)GetValue(LEDBrushProperty); }
			set { SetValue(LEDBrushProperty, value); }
		}
	}
