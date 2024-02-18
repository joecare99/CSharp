using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MVVM_24c_CTUserControl.Views
{
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
}
