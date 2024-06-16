﻿// ***********************************************************************
// Assembly         : MVVM_40_Wizzard
// Author           : Mir
// Created          : 06-13-2024
//
// Last Modified By : Mir
// Last Modified On : 06-13-2024
// ***********************************************************************
// <copyright file="Page4ViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.ComponentModel;
using MVVM.View.Extension;
using MVVM.ViewModel;
using MVVM_40_Wizzard.Models;
using BaseLib.Helper;
using CommunityToolkit.Mvvm.Input;
using System.Linq;
using MVVM_40_Wizzard.Properties;


/// <summary>
/// The ViewModels namespace.
/// </summary>
namespace MVVM_40_Wizzard.ViewModels;

/// <summary>
/// Class Page4ViewModel.
/// Implements the <see cref="BaseViewModelCT" />
/// </summary>
/// <seealso cref="BaseViewModelCT" />
public partial class Page4ViewModel : BaseViewModelCT
{
    /// <summary>
    /// The model
    /// </summary>
    private IWizzardModel _model;

    /// <summary>
    /// Gets the main-selection.
    /// </summary>
    /// <value>The sub selection.</value>
    public string MainSelection => Resources.ResourceManager.GetString($"MainSelection{_model.MainSelection}");

    /// <summary>
    /// Gets the sub selection.
    /// </summary>
    /// <value>The sub selection.</value>
    public string SubSelection => Resources.ResourceManager.GetString($"SubSelection{_model.SubSelection}");

    /// <summary>
    /// Gets the additional selection.
    /// </summary>
    /// <value>The sub selection.</value>
    public string Additional1 => Resources.ResourceManager.GetString($"AdditSelection{_model.Additional1}");

    /// <summary>
    /// Gets the additional selection.
    /// </summary>
    /// <value>The sub selection.</value>
    public string Additional2 => Resources.ResourceManager.GetString($"AdditSelection{_model.Additional2}");

    /// <summary>
    /// Gets the additional selection.
    /// </summary>
    /// <value>The sub selection.</value>
    public string Additional3 => Resources.ResourceManager.GetString($"AdditSelection{_model.Additional3}");

    /// <summary>
    /// Initializes a new instance of the <see cref="Page4ViewModel"/> class.
    /// </summary>
    public Page4ViewModel():this(IoC.GetRequiredService<IWizzardModel>())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Page4ViewModel"/> class.
    /// </summary>
    /// <param name="model">The model.</param>
    public Page4ViewModel(IWizzardModel model)
    {
        _model = model;
        _model.PropertyChanged += OnMPropertyChanged;
    }

    /// <summary>
    /// Handles the <see cref="E:MPropertyChanged" /> event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
    private void OnMPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (this.IsProperty(e.PropertyName!))
            OnPropertyChanged(e.PropertyName);
    }
}
