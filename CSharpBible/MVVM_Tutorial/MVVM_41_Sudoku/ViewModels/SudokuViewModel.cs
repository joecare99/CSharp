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
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MVVM.View.Extension;
using MVVM.ViewModel;
using MVVM_41_Sudoku.Models;
using Sudoku_Base.Models.Interfaces;
using System;
using System.IO;
using System.Collections.ObjectModel;
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

    [ObservableProperty]
    private ObservableCollection<ISudokuField> _fields;
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
        Fields = new ObservableCollection<ISudokuField>(_model.Fields);
    }

    private void OnMPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(e.PropertyName); 
        if (e.PropertyName == nameof(_model.Fields))
        {
            Fields = new ObservableCollection<ISudokuField>(_model.Fields);
        }
    }

    [RelayCommand]
    private void NewGame()
    {
        _model.Clear();
    }
    [RelayCommand]
    private void LoadGame()
    {
        // Show OpenFileDialog

        using (var fs = new FileStream("Resources\\123.sudoku", FileMode.Open))
        {
            _model.ValuesFromStream(fs);
        }
    }

    [RelayCommand]
    private void SaveGame()
    {
        // Show SaveFileDialog
        _model.ValuesToStream(null);
    }
    [RelayCommand]
    private void Print()
    {
        // Show PrintDialog
        SudokuPrinter.Print("PDF","SuDoKu",_model.Values,SudokuPrinter.DrawSudoku);
    }
    #endregion
}
