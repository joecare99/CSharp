using MVVM_Converter_DrawGrid.View.Converter;
using MVVM_Converter_DrawGrid.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace MVVM_Converter_DrawGrid.View
{
    /// <summary>
    /// Interaktionslogik für PlotFrame.xaml
    /// </summary>
    public partial class PlotFrame : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlotFrame"/> class.
        /// </summary>
        public PlotFrame()
        {
            InitializeComponent();
            SizeChanged += (object sender, SizeChangedEventArgs e) =>
            {
                if (Resources["vcPortGrid"] is WindowPortToTileDisplay pc)
                {
                    pc.WindowSize = e.NewSize;
                }
            };

        }
    }
}
