using MVVM_Converter_ImgGrid.Views.Converter;
using System.Windows;
using System.Windows.Controls;

namespace MVVM_Converter_ImgGrid.Views
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
                }
            };

        }
    }
}
