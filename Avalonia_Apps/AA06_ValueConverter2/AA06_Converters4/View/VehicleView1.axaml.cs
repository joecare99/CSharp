// ***********************************************************************
// Assembly         : AA06_Converters_4
// Author    : Mir
// Created          : 07-03-2022
//
// Last Modified By : Mir
// Last Modified On : 07-04-2022
// ***********************************************************************
// <copyright file="VehicleView1.axaml.cs" company="AA06_Converters_4">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Avalonia.Controls;
using AA06_Converters_4.ViewModels;

namespace AA06_Converters_4.View;

/// <summary>
/// Interaktionslogik für CurrencyView.axaml
/// </summary>
public partial class VehicleView1 : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VehicleView1"/> class.
    /// </summary>
    public VehicleView1(VehicleViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
