// ***********************************************************************
// Assembly         : MVVM_36_ComToolKtSavesWork_netTests
// Author           : Mir
// Created          : 05-14-2023
//
// Last Modified By : Mir
// Last Modified On : 01-16-2025
// ***********************************************************************
// <copyright file="IGetResult.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Runtime.CompilerServices;

/// <summary>
/// The Interfaces namespace.
/// </summary>
namespace Avalonia.ViewModels.Interfaces;

/// <summary>
/// Interface IGetResult
/// </summary>
public interface IGetResult
{
    /// <summary>
    /// Gets the specified objects.
    /// </summary>
    /// <param name="objects">The objects.</param>
    /// <param name="proc">The proc.</param>
    /// <returns>System.Nullable&lt;System.Object&gt;.</returns>
    object? Get(object[] objects, [CallerMemberName] string proc = "");
    /// <summary>
    /// Registers the specified proc.
    /// </summary>
    /// <param name="proc">The proc.</param>
    /// <param name="fesultFct">The fesult FCT.</param>
    void Register(string proc, Func<object[], object?> fesultFct);
}