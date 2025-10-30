// ***********************************************************************
// Assembly         : AA06_Converters_4
// Author           : Mir
// Created          : 07-03-2022
//
// Last Modified By : Mir
// Last Modified On : 07-04-2022
// ***********************************************************************
// <copyright file="MainWindow.axaml.cs" company="AA06_Converters_4">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Avalonia.Controls;
using AA06_Converters_4.View;

namespace AA06_Converters_4;

/// <summary>
/// Interaction logic for MainWindow.axaml
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
  /// </summary>
    public MainWindow(VehicleView1 vehicleView, PlotFrame plotFrame)
    {
  InitializeComponent();
        
        // Set up the content via code-behind since Avalonia doesn't support Frame navigation
        var grid = this.FindControl<Grid>("MainGrid");
    if (grid != null)
        {
          grid.Children.Add(vehicleView);
            Grid.SetColumn(vehicleView, 0);
            
            grid.Children.Add(plotFrame);
          Grid.SetColumn(plotFrame, 1);
        }
    }
}
