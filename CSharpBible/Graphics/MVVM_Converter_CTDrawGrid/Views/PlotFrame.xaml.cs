using MVVM_Converter_CTDrawGrid.Views.Converter;
using MVVM_Converter_CTDrawGrid.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace MVVM_Converter_CTDrawGrid.Views
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
                if (Resources["vcPortGrid"] is WindowPortToTileDisplay pc && DataContext is PlotFrameViewModel vc)
                {
                    pc.WindowSize = e.NewSize;
                }
            };

        }
    }
}
