// ***********************************************************************
// Assembly         : MVVM_AllExamples
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 08-24-2022
// ***********************************************************************
// <copyright file="BindingGroupViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.ComponentModel;
using MVVM.ViewModel;
using MVVM_AllExamples.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MVVM_AllExamples.ViewModels;

/// <summary>
/// Class BindingGroupViewModel.
/// Implements the <see cref="BaseViewModel" />
/// </summary>
/// <seealso cref="BaseViewModel" />
public partial class AllExamplesViewModel : BaseViewModelCT
{
    #region Properties
    public static Func<IAllExampleModel> GetModel { get; set; } = () => new AllExampleModel();

    private readonly IAllExampleModel _model;

    public DateTime Now => _model.Now;

    [ObservableProperty]
    private ObservableCollection<ExItem> _examples = new();

    [ObservableProperty]
    string _exFilter;
    #endregion

    #region Methods
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    public AllExamplesViewModel():this(GetModel())
    {
    }

    public AllExamplesViewModel(IAllExampleModel model)
    {
        _model = model;
        _model.PropertyChanged += OnMPropertyChanged;

        OnExFilterChanged(ExFilter);
    }

    private void OnMPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(e.PropertyName); 
    }

    partial void OnExFilterChanged(string value)
    {
        Examples.Clear();
        foreach (var ex in _model.Examples)
        {
            if (string.IsNullOrEmpty(value) || ex.Description.Contains(value))
                Examples.Add(ex);
        }
    }
    #endregion
}
