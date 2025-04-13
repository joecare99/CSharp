// ***********************************************************************
// Assembly         : GenInterfaces
// Author           : Mir
// Created          : 01-28-2025
//
// Last Modified By : Mir
// Last Modified On : 04-04-2025
// ***********************************************************************
// <copyright file="IGenRepository.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

/// <summary>
/// The Genealogic namespace.
/// </summary>
namespace GenInterfaces.Interfaces.Genealogic;

/// <summary>
/// Interface IGenRepository
/// Extends the <see cref="GenInterfaces.Interfaces.Genealogic.IGenObject" />
/// </summary>
/// <seealso cref="GenInterfaces.Interfaces.Genealogic.IGenObject" />
public interface IGenRepository: IGenObject, IHasOwner<IGenealogy>
{
    /// <summary>
    /// Gets the name of the repository.
    /// </summary>
    /// <value>The name.</value>
    string Name { get; init; }
    /// <summary>
    /// Gets or sets asdditional information.
    /// </summary>
    /// <value>The information.</value>
    string Info { get; set; }
    /// <summary>
    /// Gets or sets the URI.
    /// </summary>
    /// <value>The URI.</value>
    Uri Uri { get; set; }
    /// <summary>
    /// Gets the connected sources.
    /// </summary>
    /// <value>The gen sources.</value>
    IIndexedList<IGenSources> GenSources { get; }
}