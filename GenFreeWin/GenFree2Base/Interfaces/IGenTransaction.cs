// ***********************************************************************
// Assembly         : 
// Author           : Mir
// Created          : 09-22-2024
//
// Last Modified By : Mir
// Last Modified On : 09-22-2024
// ***********************************************************************
// <copyright file="IGenTransaction.cs" company="HP Inc.">
//     Copyright (c) HP Inc.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// The Interfaces namespace.
/// </summary>
namespace GenFree2Base.Interfaces;

/// <summary>
/// Interface IGenTransaction
/// Extends the <see cref="GenFree2Base.Interfaces.IGenBase" />
/// </summary>
/// <seealso cref="GenFree2Base.Interfaces.IGenBase" />
public interface IGenTransaction : IGenBase
{
    /// <summary>
    /// Gets the class.
    /// </summary>
    /// <value>The class.</value>
    IGenBase Class { get; init; }
    /// <summary>
    /// Gets the entry.
    /// </summary>
    /// <value>The entry.</value>
    IGenBase Entry { get; init; }
    /// <summary>
    /// Gets the data.
    /// </summary>
    /// <value>The data.</value>
    object? Data { get; init; }
    /// <summary>
    /// Gets the old data.
    /// </summary>
    /// <value>The old data.</value>
    object? OldData { get; init; }
    /// <summary>
    /// Gets the timestamp.
    /// </summary>
    /// <value>The timestamp.</value>
    DateTime Timestamp { get; init; }
    /// <summary>
    /// Gets the previous.
    /// </summary>
    /// <value>The previous.</value>
    IGenTransaction? Prev { get; init; }
}