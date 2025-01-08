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
using CommonDialogs.Interfaces;
using CommonDialogs;
using Microsoft.Win32;
using System.Reflection;

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

    public Func<IPrintDialog, Action<IPrintDialog>?, bool?> dPrintDialog { get; set; }
    public Func<string, IFileDialog, Action<string, IFileDialog>?, bool?> FileOpenDialog { get; set; }
    public Func<string, IFileDialog, Action<string, IFileDialog>?, bool?> FileSaveAsDialog { get; set; }
    public string SudokuFileName { get; private set; }
    public Action CloseApp { get; set; }

    public string AllImgSource => $"pack://application:,,,/{Assembly.GetExecutingAssembly().GetName().Name};component/Resources/all64_2.png";
    #endregion

    #region Methods
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    public SudokuViewModel():this(IoC.GetRequiredService<ISudokuModel>())
    {
    }

#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
    public SudokuViewModel(ISudokuModel model)
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
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
        IFileDialog foPar = new FileDialogProxy<OpenFileDialog>(new()
        {
            FileName = SudokuFileName,
            Filter = "Sudoku files (*.sudoku)|*.sudoku|All files (*.*)|*.*",
            DefaultExt = ".sudoku"
        });
        FileOpenDialog?.Invoke(SudokuFileName, foPar,
            (s, p) =>
            {
                SudokuFileName = s;
                using (var fs = new FileStream(s, FileMode.Open))
                {
                    _model.ValuesFromStream(fs);
                }
            });

    }

    [RelayCommand]
    private void SaveGame()
    {
        IFileDialog fsPar = new FileDialogProxy<SaveFileDialog>(new()
        {
            FileName = SudokuFileName
        });
        FileSaveAsDialog?.Invoke(SudokuFileName, fsPar, (s, p) =>
        {
            SudokuFileName = s;
            using (var fs = new FileStream(s, FileMode.CreateNew))
            {
                _model.ValuesToStream(fs);
            }
        });
    }

    [RelayCommand]
    private void Print()
    {
        // Show PrintDialog
        IPrintDialog dialog = new PrintDialog()
        {
            PageRangeSelection = System.Windows.Controls.PageRangeSelection.AllPages
        };
        dPrintDialog?.Invoke(dialog, (p) => {
            PagePrinter.Print(p.PrintQueue,p.PrintTicket, $"SuDoKu {Path.GetFileNameWithoutExtension(SudokuFileName)}", _model.Values, _model.DrawSudoku);
        });
    }

    [RelayCommand]
    private void Undo()
    {
        _model.UndoCommand.Execute(null);
    }

    [RelayCommand]
    private void Redo()
    {
        _model.RedoCommand.Execute(null);
    }

    [RelayCommand]
    private void Exit()
    {
        // Handle unsaved Changes
        CloseApp?.Invoke();
    }
    #endregion
}
