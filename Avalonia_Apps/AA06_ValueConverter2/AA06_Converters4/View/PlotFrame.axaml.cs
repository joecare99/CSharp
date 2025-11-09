// ***********************************************************************
// Assembly: AA06_Converters_4
// Author        : Mir
// Created      : 08-28-2022
//
// Last Modified By : Mir
// Last Modified On : 12-20-2024
// ***********************************************************************
// <copyright file="PlotFrame.axaml.cs" company="JC-Soft">
//   (c) by Joe Care 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using AA06_Converters_4.ViewModels;
using Avalonia;
using Avalonia.Controls;

namespace AA06_Converters_4.View;

/// <summary>
/// Interaktionslogik für PlotFrame.axaml
/// </summary>
public partial class PlotFrame : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlotFrame"/> class.
    /// </summary>
    public PlotFrame(PlotFrameViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        // Window Size wird automatisch durch DynamicPlotCanvas behandelt
    }
}
