// ***********************************************************************
// Assembly         : AA22_AvlnCap2
// Author           : Mir
// Created          : 08-18-2022
//
// Last Modified By : Mir
// Last Modified On : 08-16-2022
// ***********************************************************************
// <copyright file="RowData.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.Input;
using Avalonia.ViewModels;
using AA22_AvlnCap2.ViewModels.Interfaces;
using System.ComponentModel;

namespace AA22_AvlnCap2.ViewModels;

/// <summary>
/// Class RowData.
/// Implements the <see cref="INotifyPropertyChanged" />
/// </summary>
/// <seealso cref="INotifyPropertyChanged" />
public class RowData : NotificationObjectCT, IRowData, IHasParent<IWpfCapViewModel>
{
    /// <summary>
    /// Gets or sets the row identifier.
    /// </summary>
    /// <value>The row identifier.</value>
    public required int RowId { get; set; }
    /// <summary>
    /// Gets or sets the move left.
    /// </summary>
    /// <value>The move left.</value>
    public IRelayCommand<object>? MoveLeftCommand => Parent?.MoveLeftCommand;
    /// <summary>
    /// Gets or sets the move right.
    /// </summary>
    /// <value>The move right.</value>
    public IRelayCommand<object>? MoveRightCommand => Parent?.MoveRightCommand;
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
    public IRowData This => this;
    /// <summary>
    /// The parent
    /// </summary>
    public required IWpfCapViewModel? Parent { get; set; }

    public int Length 
        => 4;

    void IRowData.OnPropertyChanged(string propertyName)
    {
        OnPropertyChanged(propertyName);
    }
}
