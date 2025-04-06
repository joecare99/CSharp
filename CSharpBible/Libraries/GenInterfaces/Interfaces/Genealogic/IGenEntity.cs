// ***********************************************************************
// Assembly         : GenInterfaces
// Author           : Mir
// Created          : 01-28-2025
//
// Last Modified By : Mir
// Last Modified On : 03-13-2025
// ***********************************************************************
// <copyright file="IGenEntity.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using GenInterfaces.Data;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

/// <summary>
/// The Genealogic namespace.
/// </summary>
namespace GenInterfaces.Interfaces.Genealogic;

/// <summary>
/// Interface IGenEntity
/// Extends the <see cref="T:GenInterfaces.Interfaces.Genealogic.IGenObject" />, This interface is the base for Person and Families.
/// </summary>
[JsonDerivedType(typeof(IGenEntity))]
[JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)]
public interface IGenEntity : IGenObject, IHasOwner<IGenealogy>
{
    /// <summary>
    /// Gets the facts.
    /// </summary>
    /// <value>The facts.</value>
    IList<IGenFact> Facts { get; init; }
    /// <summary>
    /// Gets the connects.
    /// </summary>
    /// <value>The connects.</value>
    IList<IGenConnects> Connects { get; init; }
    /// <summary>
    /// Gets the start.
    /// </summary>
    /// <value>The start.</value>
    [JsonIgnore]

    IGenFact? Start { get; }

    /// <summary>
    /// Gets the end.
    /// </summary>
    /// <value>The end.</value>
    [JsonIgnore]
    IGenFact? End { get; }

    /// <summary>
    /// Gets the sources.
    /// </summary>
    /// <value>The sources.</value>
    [JsonIgnore]
    IList<IGenSources> Sources { get; init; }

    /// <summary>
    /// Gets the media.
    /// </summary>
    /// <value>The media.</value>
    [JsonIgnore]
    IList<IGenMedia> Media { get; init; }

}