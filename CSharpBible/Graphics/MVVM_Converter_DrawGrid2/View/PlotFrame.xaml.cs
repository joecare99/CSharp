// ***********************************************************************
// Assembly         : MVVM_Converter_DrawGrid2
// Author           : Mir
// Created          : 08-21-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="PlotFrame.xaml.cs" company="JC-Soft">
//     (c) by Joe Care 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM_Converter_DrawGrid2.View.Converter;
using MVVM_Converter_DrawGrid2.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace MVVM_Converter_DrawGrid2.View
{
    /// <summary>
    /// Interaktionslogik für PlotFrame.xaml
    /// </summary>
    public partial class PlotFrame : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlotFrame" /> class.
        /// </summary>
        public PlotFrame()
        {
            InitializeComponent();
            SizeChanged += (object sender, SizeChangedEventArgs e) =>
            {
                if (this.Resources["vcPortGrid"] is WindowPortToTileDisplay pc)
                {
                    pc.WindowSize = e.NewSize;
                    if (DataContext is PlotFrameViewModel vm)
                        vm.WindowSize = e.NewSize;
                }
            };

        }
    }
}
