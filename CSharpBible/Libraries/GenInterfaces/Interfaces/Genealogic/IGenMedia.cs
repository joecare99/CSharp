// ***********************************************************************
// Assembly         : GenInterfaces
// Author           : Mir
// Created          : 03-21-2025
//
// Last Modified By : Mir
// Last Modified On : 03-21-2025
// ***********************************************************************
// <copyright file="IGenMedia.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using GenInterfaces.Data;
using System;
using System.Collections.Generic;

/// <summary>
/// The Genealogic namespace.
/// </summary>
namespace GenInterfaces.Interfaces.Genealogic;

/// <summary>
/// Interface IGenMedia
/// Extends the <see cref="GenInterfaces.Interfaces.Genealogic.IGenObject" />
/// </summary>
/// <seealso cref="GenInterfaces.Interfaces.Genealogic.IGenObject" />
public interface IGenMedia: IGenObject, IHasOwner<IGenealogy>
{
    /// <summary>
    /// Gets or sets the type of the e media.
    /// </summary>
    /// <value>The type of the e media.</value>
    EMediaType eMediaType { get; set; }
    /// <summary>
    /// Gets or sets the media URI.
    /// </summary>
    /// <value>The media URI.</value>
    Uri? MediaUri { get; set; }
    /// <summary>
    /// Gets or sets the name of the media.
    /// </summary>
    /// <value>The name of the media.</value>
    string? MediaName { get; set; }
    /// <summary>
    /// Gets or sets the media description.
    /// </summary>
    /// <value>The media description.</value>
    string? MediaDescription { get; set; }
    /// <summary>
    /// Gets or sets the pre view data.
    /// </summary>
    /// <value>The pre view data.</value>
    object? PreViewData { get; set; }
    /// <summary>
    /// Gets the usage-list.
    /// </summary>
    /// <value>The usage.</value>
    IList<WeakReference<IGenObject>> Usage { get; }
}
