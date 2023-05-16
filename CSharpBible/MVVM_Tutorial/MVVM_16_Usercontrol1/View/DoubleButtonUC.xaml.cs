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

namespace MVVM_16_UserControl1.View
{
    /// <summary>
    /// Interaktionslogik für CurrencyView.xaml
    /// </summary>
    public partial class DoubleButtonUC : UserControl
    {
        public DoubleButtonUC()
        {
            InitializeComponent();
			visData1 = Visibility.Visible;
			visData2 = Visibility.Visible;
		}

		public ICommand Command1 { get; set; }
		public static readonly DependencyProperty CommandProperty1 = DependencyProperty.Register("Command1", typeof(ICommand), typeof(DoubleButtonUC), new UIPropertyMetadata(null));
		public ICommand Command2 { get; set; }
		public static readonly DependencyProperty CommandProperty2 = DependencyProperty.Register("Command2", typeof(ICommand), typeof(DoubleButtonUC), new UIPropertyMetadata(null));

		public Visibility visData1 { get; set; }
		public Visibility visData2 { get; set; }

		public string CommandParameter1 { get; set; }
		public string CommandParameter2 { get; set; }

		public string Tooltip1 { get; set; }
		public string ToolTip2 { get; set; }

		public string Image1 { get; set; }
		public string Image2 { get; set; }
	}
}
