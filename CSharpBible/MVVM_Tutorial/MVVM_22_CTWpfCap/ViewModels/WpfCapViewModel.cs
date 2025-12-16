// ***********************************************************************
// Assembly         : MVVM_22_CTWpfCap
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
using BaseLib.Helper;
using CommunityToolkit.Mvvm.Input;
using MVVM.ViewModel;
using MVVM_22_CTWpfCap.Model;
using MVVM_22_CTWpfCap.ViewModels.Factories;
using MVVM_22_CTWpfCap.ViewModels.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MVVM_22_CTWpfCap.ViewModels;

/// <summary>
/// Class WpfCapViewModel.
/// Implements the <see cref="BaseViewModel" />
/// </summary>
/// <seealso cref="BaseViewModel" />
public partial class WpfCapViewModel : BaseViewModelCT, IWpfCapViewModel
{
    public WpfCapViewModel() : this(IoC.GetRequiredService<IWpfCapModel>(), IoC.GetRequiredService<IRowDataFactory>(), IoC.GetRequiredService<IColDataFactory>())
    {}
    /// <summary>
    /// Initializes a new instance of the <see cref="WpfCapViewModel"/> class.
    /// </summary>
    public WpfCapViewModel(IWpfCapModel model,
                           IRowDataFactory rowFactory,
                           IColDataFactory colFactory)
    {
        _model = model;

        Rows = new ObservableCollection<IRowData>();
        for (int r = 1; r <= model.Height; r++)
            Rows.Add(rowFactory.Create(r, this));

        Cols = new ObservableCollection<IColData>();
        for (int c = 1; c <= model.Width; c++)
            Cols.Add(colFactory.Create(c, this));

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
    public IWpfCapModel Model 
        => _model;

    /// <summary>
    /// Shuffles the specified object.
    /// </summary>
    /// <param name="obj">The object.</param>
    [RelayCommand]
    private void Shuffle() 
        => _model?.Shuffle();

    /// <summary>
    /// Does the move left.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <exception cref="System.NotImplementedException"></exception>
    [RelayCommand]
    private void MoveLeft(object obj) 
        => _model.MoveLeft((obj is IRowData rd) ? rd.RowId - 1 : throw new NotImplementedException());

    /// <summary>
    /// Does the move right.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <exception cref="System.NotImplementedException"></exception>
    [RelayCommand]
    private void MoveRight(object obj) 
        => _model.MoveRight((obj is IRowData rd) ? rd.RowId - 1 : throw new NotImplementedException());

    /// <summary>
    /// Does the move up.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <exception cref="System.NotImplementedException"></exception>
    [RelayCommand]
    private void MoveUp(object obj)
        => _model.MoveUp((obj is IColData cd) ? cd.ColId - 1 : throw new NotImplementedException());

    /// <summary>
    /// Does the move down.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <exception cref="System.NotImplementedException"></exception>
    [RelayCommand]
    private void MoveDown(object obj)
        => _model.MoveDown((obj is IColData cd) ? cd.ColId - 1 : throw new NotImplementedException());

    /// <summary>
    /// Gets or sets the rows.
    /// </summary>
    /// <value>The rows.</value>
    public ObservableCollection<IRowData> Rows { get; set; }
    /// <summary>
    /// Gets or sets the cols.
    /// </summary>
    /// <value>The cols.</value>
    public ObservableCollection<IColData> Cols { get; set; }
}
