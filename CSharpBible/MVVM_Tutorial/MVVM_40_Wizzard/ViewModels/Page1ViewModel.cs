// ***********************************************************************
// Assembly         : MVVM_40_Wizzard
// Author           : Mir
// Created          : 06-13-2024
//
// Last Modified By : Mir
// Last Modified On : 06-13-2024
// ***********************************************************************
// <copyright file="Page1ViewModel.cs" company="JC-Soft">
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
using System.Windows.Documents;
using System.Linq;


/// <summary>
/// The ViewModels namespace.
/// </summary>
namespace MVVM_40_Wizzard.ViewModels;

/// <summary>
/// Class Page1ViewModel.
/// Implements the <see cref="BaseViewModelCT" />
/// </summary>
/// <seealso cref="BaseViewModelCT" />
public partial class Page1ViewModel : BaseViewModelCT
{
    /// <summary>
    /// The model
    /// </summary>
    private IWizzardModel _model;

    /// <summary>
    /// Gets or sets the main selection.
    /// </summary>
    /// <value>The main selection.</value>
    public ListEntry? MainSelection
    {
        get => MainOptions.FirstOrDefault((e)=>e.ID==_model.MainSelection);
        set => _model.MainSelection = value?.ID ?? -1;
    }

    /// <summary>
    /// Gets the main options.
    /// </summary>
    /// <value>The main options.</value>
    public IList<ListEntry> MainOptions 
        => _model.MainOptions.Select((i)=>new ListEntry(i, Properties.Resources.ResourceManager.GetString($"MainSelection{i}"))).ToList();


    /// <summary>
    /// Initializes a new instance of the <see cref="Page1ViewModel"/> class.
    /// </summary>
    public Page1ViewModel():this(IoC.GetRequiredService<IWizzardModel>())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Page1ViewModel"/> class.
    /// </summary>
    /// <param name="model">The model.</param>
    public Page1ViewModel(IWizzardModel model)
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
        MainSelection = null;
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
