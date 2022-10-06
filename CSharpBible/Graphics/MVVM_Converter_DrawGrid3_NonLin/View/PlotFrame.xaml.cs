// ***********************************************************************
// Assembly         : MVVM_Converter_DrawGrid3_NonLin
// Author           : Mir
// Created          : 09-02-2022
//
// Last Modified By : Mir
// Last Modified On : 09-04-2022
// ***********************************************************************
// <copyright file="PlotFrame.xaml.cs" company="JC-Soft">
//     (c) by Joe Care 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM_Converter_DrawGrid3_NonLin.View.Converter;
using MVVM_Converter_DrawGrid3_NonLin.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MVVM_Converter_DrawGrid3_NonLin.View
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
                if (this.Resources["vcPortGrid"] is WindowPortToTileDisplay pc)
                {
                    pc.WindowSize = e.NewSize;
                    (DataContext as PlotFrameViewModel).WindowSize = e.NewSize;
                }
            };

        }
    }
}
