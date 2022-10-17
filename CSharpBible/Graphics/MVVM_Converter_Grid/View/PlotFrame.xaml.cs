﻿// ***********************************************************************
// Assembly         : MVVM_Converter_Grid
// Author           : Mir
// Created          : 07-21-2022
//
// Last Modified By : Mir
// Last Modified On : 08-21-2022
// ***********************************************************************
// <copyright file="PlotFrame.xaml.cs" company="JC-Soft">
//     (c) by Joe Care 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM_Converter_Grid.View.Converter;
using MVVM_Converter_Grid.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace MVVM_Converter_Grid.View
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
                    (DataContext as PlotFrameViewModel).WindowSize = e.NewSize;
                }
            };

        }
    }
}
