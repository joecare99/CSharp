// ***********************************************************************
// Assembly         : AA22_AvlnCap
// Author           : Mir
// Created          : 08-18-2022
//
// Last Modified By : Mir
// Last Modified On : 08-16-2022
// ***********************************************************************
// <copyright file="WpfCapViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BaseLib.Helper;
using AA22_AvlnCap.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia.ViewModels;
using AA22_AvlnCap.ViewModels.Interfaces;
namespace AA22_AvlnCap.ViewModels;

/// <summary>
/// ViewModel using CommunityToolkit
/// </summary>
public partial class WpfCapViewModel : BaseViewModelCT, IWpfCapViewModel
{
    public WpfCapViewModel() : this(IoC.GetRequiredService<IWpfCapModel>())
    {}

    public WpfCapViewModel(IWpfCapModel model)
    {
        _model = model;
        Rows =
        [
            new RowData() { RowId = 1, Parent = this },
            new RowData() { RowId = 2, Parent = this },
            new RowData() { RowId = 3, Parent = this },
            new RowData() { RowId = 4, Parent = this }
        ];

        Cols =
        [
            new ColData() { ColId = 1, Parent = this },
            new ColData() { ColId = 2, Parent = this },
            new ColData() { ColId = 3, Parent = this },
            new ColData() { ColId = 4, Parent = this }
        ];

        model.Init();
        model.Shuffle();
        model.TileColorChanged += Model_TileColorChanged;
    }

    private void Model_TileColorChanged(
#if NET5_0_OR_GREATER
        object?
#else
        object
# endif
        sender, EventArgs e)
    {
        for (int i = 0; i < _model.Height; i++)
        {
            var rd = Rows[i];
            rd.OnPropertyChanged(nameof(rd.TileColor));
        }
    }

    private readonly IWpfCapModel _model;
    public IWpfCapModel Model => _model;

    [RelayCommand]
    private void Shuffle() => _model?.Shuffle();

    [RelayCommand]
    private void MoveLeft(object o) 
        =>  _model.MoveLeft(o is IRowData rd ? rd.RowId - 1 : throw new Exception());

    [RelayCommand]
    private void MoveRight(object o) 
        =>  _model.MoveRight(o is IRowData rd ? rd.RowId - 1 : throw new Exception());

    [RelayCommand]
    private void MoveUp(object o) 
        => _model.MoveUp(o is IColData cd? cd.ColId - 1:throw new Exception());

    [RelayCommand]
    private void MoveDown(object o) 
        => _model.MoveDown(o is IColData cd ? cd.ColId - 1 : throw new Exception());

    public ObservableCollection<IRowData> Rows { get; set; }
    public ObservableCollection<IColData> Cols { get; set; }
}
