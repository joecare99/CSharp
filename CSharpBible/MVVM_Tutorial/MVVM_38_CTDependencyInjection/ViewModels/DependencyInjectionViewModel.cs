// ***********************************************************************
// Assembly         : MVVM_38_CTDependencyInjection
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
using MVVM.ViewModel;
using MVVM.View.Extension;
using MVVM_38_CTDependencyInjection.Models.Interfaces;
using System;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace MVVM_38_CTDependencyInjection.ViewModels;

/// <summary>
/// Class MainWindowViewModel.
/// Implements the <see cref="BaseViewModel" />
/// </summary>
/// <seealso cref="BaseViewModel" />
public partial class DependencyInjectionViewModel : BaseViewModelCT
{
    #region Properties
    private readonly ITemplateModel _model;

    public DateTime Now => _model.Now;

    public ObservableCollection<string> Usernames { get; private set; } = new();
    #endregion

    #region Methods
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    public DependencyInjectionViewModel():this(IoC.GetRequiredService<ITemplateModel>())
    {
    }

    public DependencyInjectionViewModel(ITemplateModel model)
    {
        _model = model;
        _model.PropertyChanged += OnMPropertyChanged;
        foreach (var item in _model.GetUsers())
        {
            Usernames.Add(item);
        }
    }

    private void OnMPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(e.PropertyName); 
    }

    #endregion
}
