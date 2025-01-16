// ***********************************************************************
// Assembly         : MVVM_39_MultiModelTest
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 08-24-2022
// ***********************************************************************
// <copyright file="MainWindowViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.Input;
using MVVM.View.Extension;
using MVVM.ViewModel;
using MVVM_39_MultiModelTest.Models;
using System;
using System.ComponentModel;

namespace MVVM_39_MultiModelTest.ViewModels;

/// <summary>
/// Class MainWindowViewModel.
/// Implements the <see cref="BaseViewModel" />
/// </summary>
/// <seealso cref="BaseViewModel" />
public partial class MultiModelMainViewModel : BaseViewModelCT
{
    #region Delegtes
    public delegate void ShowModelDelegate(IScopedModel model);
    #endregion

    #region Properties
    public static Func<ISystemModel> GetModel { get; set; } = () => IoC.GetRequiredService<ISystemModel>();

    private readonly ISystemModel _model;
    private IScopedModel? scopedModel1;
    private IScopedModel? scopedModel2;

    public ShowModelDelegate? showModel { get; set; }

    public DateTime Now => _model.Now;
    #endregion

    #region Methods
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    public MultiModelMainViewModel():this(GetModel())
    {
    }

    public MultiModelMainViewModel(ISystemModel model)
    {
        _model = model;
        _model.PropertyChanged += OnMPropertyChanged;
    }

    [RelayCommand]
    private void OpenScopedModel1()
    {
        scopedModel1 ??= _model.GetNewScopedModel();
        showModel?.Invoke(scopedModel1);
    }   
    
    [RelayCommand]
    private void OpenScopedModel2()
    {
        scopedModel2 ??= _model.GetNewScopedModel();
        showModel?.Invoke(scopedModel2);
    }

    private void OnMPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(e.PropertyName); 
    }

    #endregion
}
