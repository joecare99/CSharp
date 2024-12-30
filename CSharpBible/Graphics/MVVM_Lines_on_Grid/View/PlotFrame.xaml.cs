// ***********************************************************************
// Assembly         : MVVM_Lines_on_Grid
// Author           : Mir
// Created          : 08-28-2022
//
// Last Modified By : Mir
// Last Modified On : 08-28-2022
// ***********************************************************************
// <copyright file="PlotFrame.xaml.cs" company="JC-Soft">
//     (c) by Joe Care 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM_Lines_on_Grid.View.Converter;
using MVVM_Lines_on_Grid.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace MVVM_Lines_on_Grid.View
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
                if (this.Resources["vcPortGrid"] is WindowPortToGridLines pc)
                {
                    pc.WindowSize = e.NewSize;
                    if (DataContext is PlotFrameViewModel vm)
                        vm.WindowSize = e.NewSize;
                }
            };

        }
    }
}
