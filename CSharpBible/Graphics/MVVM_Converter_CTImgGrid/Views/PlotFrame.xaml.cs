using MVVM_Converter_CTImgGrid.Views.Converter;
using System.Windows;
using System.Windows.Controls;

namespace MVVM_Converter_CTImgGrid.Views
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
