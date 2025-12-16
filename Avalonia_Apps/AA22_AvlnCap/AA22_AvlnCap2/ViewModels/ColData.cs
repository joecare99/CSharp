// ***********************************************************************
// Assembly         : AA22_AvlnCap2
// Author           : Mir
// Created          : 08-18-2022
//
// Last Modified By : Mir
// Last Modified On : 08-16-2022
// ***********************************************************************
// <copyright file="ColData.cs" company="JC-Soft">
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
/// Class ColData.
/// Implements the <see cref="INotifyPropertyChanged" />
/// </summary>
/// <seealso cref="INotifyPropertyChanged" />
public partial class ColData : NotificationObjectCT, IHasParent<IWpfCapViewModel>, IColData
{

    /// <summary>
    /// Gets or sets the col identifier.
    /// </summary>
    /// <value>The col identifier.</value>
    public required int ColId { get; set; }
    /// <summary>
    /// Gets or sets the move up.
    /// </summary>
    /// <value>The move up.</value>
    public IRelayCommand<object>? MoveUpCommand => Parent?.MoveUpCommand;
    /// <summary>
    /// Gets or sets the move down.
    /// </summary>
    /// <value>The move down.</value>
    public IRelayCommand<object>? MoveDownCommand => Parent?.MoveDownCommand;
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
    public IColData This => this;
    /// <summary>
    /// The parent
    /// </summary>
    public required IWpfCapViewModel? Parent { get; set; }

    /// <summary>
    /// The length
    /// </summary>
    public int Length => 4;
}
