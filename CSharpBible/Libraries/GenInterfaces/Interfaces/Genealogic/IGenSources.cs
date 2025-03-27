// ***********************************************************************
// Assembly         : GenInterfaces
// Author           : Mir
// Created          : 01-28-2025
//
// Last Modified By : Mir
// Last Modified On : 01-10-2025
// ***********************************************************************
// <copyright file="IGenSources.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;

/// <summary>
/// The Genealogic namespace.
/// </summary>
namespace GenInterfaces.Interfaces.Genealogic;

/// <summary>
/// Interface IGenSources
/// Extends the <see cref="GenInterfaces.Interfaces.Genealogic.IGenObject" />
/// </summary>
/// <seealso cref="GenInterfaces.Interfaces.Genealogic.IGenObject" />
public interface IGenSources : IGenObject
{
    /// <summary>Gets the medias.</summary>
    /// <value>The list of media of this fact.</value>
    IList<IGenMedia> Medias { get; init; }
}