// ***********************************************************************
// Assembly         : 
// Author           : Mir
// Created          : 09-22-2024
//
// Last Modified By : Mir
// Last Modified On : 09-22-2024
// ***********************************************************************
// <copyright file="IGenConnects.cs" company="JC-Soft">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
/// <summary>
/// The Interfaces namespace.
/// </summary>
namespace GenInterfaces.Interfaces.Genealogic;

/// <summary>
/// Interface IGenConnects
/// Extends the <see cref="IGenBase" />
/// </summary>
/// <seealso cref="IGenBase" />
public interface IGenConnects : IGenBase
{
    /// <summary>
    /// Gets the entity.
    /// </summary>
    /// <value>The entity.</value>
    IGenEntity Entity { get; init; }
}