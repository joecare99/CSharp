// ***********************************************************************
// Assembly         : MVVM_99_SomeIssue
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
using MVVM.ViewModel;
using MVVM_99_SomeIssue.Models;
using System;
using System.ComponentModel;

namespace MVVM_99_SomeIssue.ViewModels;

/// <summary>
/// Class MainWindowViewModel.
/// Implements the <see cref="BaseViewModel" />
/// </summary>
/// <seealso cref="BaseViewModel" />
public partial class SomeIssueViewModel : BaseViewModelCT
{
    #region Properties
    private readonly ISomeIssueModel _model;

    public DateTime Now => _model.Now;
    #endregion

    #region Methods
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    public SomeIssueViewModel():this(IoC.GetRequiredService<ISomeIssueModel>())
    {
    }

    public SomeIssueViewModel(ISomeIssueModel model)
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
