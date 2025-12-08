// ***********************************************************************
// Assembly         : GenInterfaces
// Author           : Mir
// Created          : 01-28-2025
//
// Last Modified By : Mir
// Last Modified On : 01-10-2025
// ***********************************************************************
// <copyright file="IGenSource.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

/// <summary>
/// The Genealogic namespace.
/// </summary>
namespace GenInterfaces.Interfaces.Genealogic;

/// <summary>
/// Interface IGenSource
/// Extends the <see cref="IGenObject" />
/// </summary>
/// <seealso cref="IGenObject" />
public interface IGenSource : IGenObject, IHasOwner<IGenealogy>
{
    /// <summary>Gets or sets the description.</summary>
    /// <value>a short description of the source.</value>
    string Description { get; set; }

    /// <summary>Gets or sets the URL.</summary>
    /// <value>The URL of the source.</value>
    Uri? Url { get; set; }

    /// <summary>Gets or sets the data.</summary>
    /// <value>The data/text of the source.</value>
    string Data { get; set; }

    /// <summary>Gets the medias.</summary>
    /// <value>The list of media of this source.</value>
    IList<IGenMedia> Medias { get; init; }

    
}