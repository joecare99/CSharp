// ***********************************************************************
// Assembly         : MVVM_17_1_CSV_Laden
// Author           : Mir
// Created          : 07-03-2022
//
// Last Modified By : Mir
// Last Modified On : 07-04-2022
// ***********************************************************************
// <copyright file="DataPointsView.xaml.cs" company="MVVM_17_1_CSV_Laden">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM_17_1_CSV_Laden.ViewModels;
using MVVM_17_1_CSV_Laden.Views.Converter;
using System.Windows;
using System.Windows.Controls;

namespace MVVM_17_1_CSV_Laden.Views;

/// <summary>
/// Interaktionslogik für DataPointsView.xaml
/// </summary>
public partial class DataPointsView : Page
{
    /// <summary>
    /// Finalizes an instance of the <see cref="DataPointsView"/> class.
    /// </summary>
    public DataPointsView()
    {
        InitializeComponent();
        DataContextChanged += (object sender, DependencyPropertyChangedEventArgs e) =>
        {
            if (e.NewValue is DataPointsViewModel vm && this.Resources["vcPortGrid"] is WindowPortToGridLines pc)
            {
                //                    pc.Row = vm.Row;
                //                  pc.Col = vm.Column;
                pc.WindowSize = new Size(Width, Height);
            }
        };

        var f = this.FindName("ViewPort") as FrameworkElement;
        f.SizeChanged += (object sender, SizeChangedEventArgs e) =>
        {
            if (this.Resources["vcPortGrid"] is WindowPortToGridLines pc)
            {
                pc.WindowSize = e.NewSize;
                if (DataContext is DataPointsViewModel vm)
                    vm.WindowSize = e.NewSize;
            }
        };

    }
}
