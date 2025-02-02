// ***********************************************************************
// Assembly         : MVVM_40_Wizzard
// Author           : Mir
// Created          : 06-13-2024
//
// Last Modified By : Mir
// Last Modified On : 06-13-2024
// ***********************************************************************
// <copyright file="Page2ViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.ComponentModel;
using BaseLib.Helper;
using MVVM.ViewModel;
using MVVM_40_Wizzard.Models;
using BaseLib.Helper;
using CommunityToolkit.Mvvm.Input;
using System.Linq;
using MVVM_40_Wizzard.Models.Interfaces;


/// <summary>
/// The ViewModels namespace.
/// </summary>
namespace MVVM_40_Wizzard.ViewModels;

/// <summary>
/// Class Page2ViewModel.
/// Implements the <see cref="BaseViewModelCT" />
/// </summary>
/// <seealso cref="BaseViewModelCT" />
public partial class Page2ViewModel : BaseViewModelCT
{
    /// <summary>
    /// The model
    /// </summary>
    private IWizzardModel _model;

    /// <summary>
    /// Gets or sets the sub selection.
    /// </summary>
    /// <value>The sub selection.</value>
    public ListEntry? SubSelection
    {
        get => SubOptions.FirstOrDefault((e)=>e.ID== _model.SubSelection);
        set => _model.SubSelection = value?.ID ?? -1;
    }

    /// <summary>
    /// Gets the sub options.
    /// </summary>
    /// <value>The sub options.</value>
    public IList<ListEntry> SubOptions => _model.SubOptions.Select((i) => new ListEntry(i, Properties.Resources.ResourceManager.GetString($"SubSelection{i}"))).ToList();


    /// <summary>
    /// Initializes a new instance of the <see cref="Page2ViewModel"/> class.
    /// </summary>
    public Page2ViewModel():this(IoC.GetRequiredService<IWizzardModel>())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Page2ViewModel"/> class.
    /// </summary>
    /// <param name="model">The model.</param>
    public Page2ViewModel(IWizzardModel model)
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
        SubSelection = null;
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
