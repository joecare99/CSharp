using MVVM_Converter_Grid3_NonLin.View.Converter;
using MVVM_Converter_Grid3_NonLin.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MVVM_Converter_Grid3_NonLin.View
{
    /// <summary>
    /// Interaktionslogik für PlotFrame.xaml
    /// </summary>
    public partial class PlotFrame : Page
    {
        public PlotFrame()
        {
            InitializeComponent();
            DataContextChanged += (object sender, DependencyPropertyChangedEventArgs e) =>
            {
                if (e.NewValue is PlotFrameViewModel vm && this.Resources["vcPortGrid"] is WindowPortToGridLines pc)
                {
                    //                    pc.Row = vm.Row;
                    //                  pc.Col = vm.Column;
                    pc.WindowSize = new Size(Width, Height);
                }
            };

            SizeChanged += (object sender, SizeChangedEventArgs e) =>
            {
                if (DataContext is PlotFrameViewModel vm && this.Resources["vcPortGrid"] is WindowPortToGridLines pc)
                {
                    pc.WindowSize = e.NewSize;
                    vm.WindowSize = e.NewSize;
                }
            };

            if (this.Resources["vcPortGrid"] is WindowPortToGridLines pc)
            {
             //   pc.Real2VisP2 = (pt) => new System.Drawing.PointF((float)Math.Sin(pt.X * Math.PI) * pt.Y, (float)Math.Cos(pt.X * Math.PI) * pt.Y);
                pc.Real2VisP2 = (pt) => new System.Drawing.PointF(pt.X * 0.8f + pt.Y*0.6f, -pt.X * 0.6f + pt.Y*0.8f);
            }

        }
    }
}
