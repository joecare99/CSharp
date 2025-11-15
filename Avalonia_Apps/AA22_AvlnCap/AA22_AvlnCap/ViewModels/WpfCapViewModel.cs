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
namespace AA22_AvlnCap.ViewModels;

/// <summary>
/// Class RowData.
/// Implements the <see cref="INotifyPropertyChanged" />
/// </summary>
/// <seealso cref="INotifyPropertyChanged" />
public class RowData : NotificationObjectCT
{
    /// <summary>
    /// Gets or sets the row identifier.
    /// </summary>
    /// <value>The row identifier.</value>
    public int RowId { get; set; }

    /// <summary>
    /// Gets or sets the color of the tile.
    /// </summary>
    /// <value>The color of the tile.</value>
    public int[] TileColor
    {
        get
        {
            var _result = new int[4];
            for (var i = 0; i < 4; i++)
                _result[i] = Parent?.Model.TileColor(i, RowId - 1)??0;
            return _result;
        }
    }

    /// <summary>
    /// Gets the this.
    /// </summary>
    /// <value>The this.</value>
    public object This => this;
    /// <summary>
    /// The parent
    /// </summary>
    public WpfCapViewModel? Parent;

    public new void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
    }

}

/// <summary>
/// Class ColData.
/// Implements the <see cref="INotifyPropertyChanged" />
/// </summary>
/// <seealso cref="INotifyPropertyChanged" />
public class ColData : NotificationObjectCT
{
    /// <summary>
    /// Gets or sets the col identifier.
    /// </summary>
    /// <value>The col identifier.</value>
    public int ColId { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="System.Int32"/> with the specified ix.
    /// </summary>
    /// <param name="ix">The ix.</param>
    /// <returns>System.Int32.</returns>
    public int this[int ix] => Parent?.Model.TileColor(ix, ColId - 1)??0;
    /// <summary>
    /// Gets the this.
    /// </summary>
    /// <value>The this.</value>
    public object This => this;
    /// <summary>
    /// The parent
    /// </summary>
    public WpfCapViewModel? Parent;

    /// <summary>
    /// The length
    /// </summary>
    public readonly int Length = 4;
}

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
        Rows = new ObservableCollection<RowData>
        {
            new RowData() { RowId = 1, Parent = this },
            new RowData() { RowId = 2, Parent = this },
            new RowData() { RowId = 3, Parent = this },
            new RowData() { RowId = 4, Parent = this }
        };

        Cols = new ObservableCollection<ColData>
        {
            new ColData() { ColId = 1, Parent = this },
            new ColData() { ColId = 2, Parent = this },
            new ColData() { ColId = 3, Parent = this },
            new ColData() { ColId = 4, Parent = this }
        };

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
    private void MoveLeft(RowData rd) => _model.MoveLeft(rd.RowId - 1);

    [RelayCommand]
    private void MoveRight(RowData rd) => _model.MoveRight(rd.RowId - 1);

    [RelayCommand]
    private void MoveUp(ColData cd) => _model.MoveUp(cd.ColId - 1);

    [RelayCommand]
    private void MoveDown(ColData cd) => _model.MoveDown(cd.ColId - 1);

    public ObservableCollection<RowData> Rows { get; set; }
    public ObservableCollection<ColData> Cols { get; set; }
}
