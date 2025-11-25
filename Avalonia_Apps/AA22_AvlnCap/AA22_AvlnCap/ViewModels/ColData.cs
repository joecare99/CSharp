// ***********************************************************************
// Assembly         : AA22_AvlnCap
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
using AA22_AvlnCap.ViewModels.Interfaces;
using Avalonia.ViewModels;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
namespace AA22_AvlnCap.ViewModels;

/// <summary>
/// Class ColData.
/// Implements the <see cref="INotifyPropertyChanged" />
/// </summary>
/// <seealso cref="INotifyPropertyChanged" />
public class ColData : NotificationObjectCT, IColData
{
    /// <summary>
    /// Gets or sets the col identifier.
    /// </summary>
    /// <value>The col identifier.</value>
    public int ColId { get; set; }

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
    public int this[int ix] => Parent?.Model.TileColor(ix, ColId - 1) ?? 0;
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
