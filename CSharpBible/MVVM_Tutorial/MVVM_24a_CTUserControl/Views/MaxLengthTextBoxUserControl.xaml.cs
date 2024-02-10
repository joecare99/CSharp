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

namespace MVVM_24a_CTUserControl.Views
{
	/// <summary>
	/// Interaktionslogik für MaxLengthTextBoxUserControl.xaml
	/// </summary>
	public partial class MaxLengthTextBoxUserControl : UserControl
	{
		public static readonly DependencyProperty TextProperty = 
			DependencyProperty.Register(nameof(Text), typeof(string), 
				typeof(MaxLengthTextBoxUserControl),
				new FrameworkPropertyMetadata(default(string),
					FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		public MaxLengthTextBoxUserControl()
		{
			InitializeComponent();
			DataContext = this;
		}

		public string Caption { get; set; }
		public int MaxLength { get; set; }

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
	}
}
