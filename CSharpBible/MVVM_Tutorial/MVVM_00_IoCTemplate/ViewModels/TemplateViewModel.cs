// ***********************************************************************
// Assembly         : MVVM_00_IoCTemplate
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
using BaseLib.Helper;
using BaseLib.Helper;
using MVVM.ViewModel;
using MVVM_00_IoCTemplate.Models;
using System;
using System.ComponentModel;

namespace MVVM_00_IoCTemplate.ViewModels;

/// <summary>
/// Class MainWindowViewModel.
/// Implements the <see cref="BaseViewModel" />
/// </summary>
/// <seealso cref="BaseViewModel" />
public partial class TemplateViewModel : BaseViewModelCT
{
    #region Properties
    private readonly ITemplateModel _model;

    public DateTime Now => _model.Now;
    #endregion

    #region Methods
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    public TemplateViewModel():this(IoC.GetRequiredService<ITemplateModel>())
    {
    }

    public TemplateViewModel(ITemplateModel model)
    {
        _model = model;
        _model.PropertyChanged += OnMPropertyChanged;
    }

    private void OnMPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(e.PropertyName); 
    }

    #endregion
}
