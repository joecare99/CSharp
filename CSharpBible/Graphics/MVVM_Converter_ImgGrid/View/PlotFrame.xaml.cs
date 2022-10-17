using MVVM_Converter_ImgGrid.View.Converter;
using MVVM_Converter_ImgGrid.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MVVM_Converter_ImgGrid.View
{
    /// <summary>
    /// Interaktionslogik für PlotFrame.xaml
    /// </summary>
    public partial class PlotFrame : Page
    {
        public PlotFrame()
        {
            InitializeComponent();
            SizeChanged += (object sender, SizeChangedEventArgs e) =>
            {
                if (this.Resources["vcPortGrid"] is WindowPortToTileDisplay pc)
                {
                    pc.WindowSize = e.NewSize;
                    (DataContext as PlotFrameViewModel).WindowSize = e.NewSize;
                }
            };

        }
    }
}
