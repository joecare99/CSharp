// ***********************************************************************
// Assembly         : MVVM_41_Sudoku
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
using MVVM.View.Extension;
using MVVM.ViewModel;
using Sudoku_Base.Models.Interfaces;
using System;
using System.ComponentModel;

namespace MVVM_41_Sudoku.ViewModels;

/// <summary>
/// Class MainWindowViewModel.
/// Implements the <see cref="BaseViewModel" />
/// </summary>
/// <seealso cref="BaseViewModel" />
public partial class SudokuViewModel : BaseViewModelCT
{
    #region Properties
    private readonly ISudokuModel _model;

    #endregion

    #region Methods
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    public SudokuViewModel():this(IoC.GetRequiredService<ISudokuModel>())
    {
    }

    public SudokuViewModel(ISudokuModel model)
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
