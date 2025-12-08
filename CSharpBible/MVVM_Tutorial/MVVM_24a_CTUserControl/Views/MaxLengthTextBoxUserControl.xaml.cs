using System.Windows;
using System.Windows.Controls;

namespace MVVM_24a_CTUserControl.Views;

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
	
		public static readonly DependencyProperty TextHintProperty = 
			DependencyProperty.Register(nameof(TextHint), typeof(string), 
				typeof(MaxLengthTextBoxUserControl),
				new FrameworkPropertyMetadata(default(string)));
		public MaxLengthTextBoxUserControl()
		{
			InitializeComponent();
			DataContext = this;
		}

		public string Caption { get; set; } = "";
		public int MaxLength { get; set; }

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		public string TextHint
		{
			get { return (string)GetValue(TextHintProperty); }
			set { SetValue(TextHintProperty, value); }
		}
	}
