// ***********************************************************************
// Assembly         : MVVM_40_Wizzard
// Author           : Mir
// Created          : 06-13-2024
//
// Last Modified By : Mir
// Last Modified On : 06-13-2024
// ***********************************************************************
// <copyright file="Page3ViewModel.cs" company="JC-Soft">
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


/// <summary>
/// The ViewModels namespace.
/// </summary>
namespace MVVM_40_Wizzard.ViewModels;

/// <summary>
/// Class Page3ViewModel.
/// Implements the <see cref="BaseViewModelCT" />
/// </summary>
/// <seealso cref="BaseViewModelCT" />
public partial class Page3ViewModel : BaseViewModelCT
{
    /// <summary>
    /// The model
    /// </summary>
    private IWizzardModel _model;

    /// <summary>
    /// Gets or sets the sub selection.
    /// </summary>
    /// <value>The sub selection.</value>
    public ListEntry? Additional1
    {
        get => AdditOptions.FirstOrDefault((e)=>e.ID== _model.Additional1);
        set => _model.Additional1 = value?.ID ?? -1;
    }

    /// <summary>
    /// Gets or sets the sub selection.
    /// </summary>
    /// <value>The sub selection.</value>
    public ListEntry? Additional2
    {
        get => AdditOptions.FirstOrDefault((e) => e.ID == _model.Additional2);
        set => _model.Additional2 = value?.ID ?? -1;
    }

    /// <summary>
    /// Gets or sets the sub selection.
    /// </summary>
    /// <value>The sub selection.</value>
    public ListEntry? Additional3
    {
        get => AdditOptions.FirstOrDefault((e) => e.ID == _model.Additional3);
        set => _model.Additional3 = value?.ID ?? -1;
    }

    /// <summary>
    /// Gets the sub options.
    /// </summary>
    /// <value>The sub options.</value>
    public IList<ListEntry> AdditOptions 
        => _model.AdditOptions.Select((i) => new ListEntry(i, Properties.Resources.ResourceManager.GetString($"AdditSelection{i}"))).ToList();


    /// <summary>
    /// Initializes a new instance of the <see cref="Page3ViewModel"/> class.
    /// </summary>
    public Page3ViewModel():this(IoC.GetRequiredService<IWizzardModel>())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Page3ViewModel"/> class.
    /// </summary>
    /// <param name="model">The model.</param>
    public Page3ViewModel(IWizzardModel model)
    {
        _model = model;
        _model.PropertyChanged += OnMPropertyChanged;
    }

    /// <summary>
    /// Clears this instance.
    /// </summary>
    [RelayCommand]
    private void Clear()
    {
        Additional1 = null;
        Additional2 = null;
        Additional3 = null;
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
