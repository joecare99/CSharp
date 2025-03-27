﻿// ***********************************************************************
// Assembly         : GenInterfaces
// Author           : Mir
// Created          : 01-06-2025
//
// Last Modified By : Mir
// Last Modified On : 01-06-2025
// ***********************************************************************
// <copyright file="IGenBase.cs" company="JC-Soft">
//     Copyright © JC-Soft 2024
// </copyright>
// <summary></summary>
// ***********************************************************************
using GenInterfaces.Data;
using System;
using System.Text.Json.Serialization;

/// <summary>
/// The Interfaces namespace.
/// </summary>
namespace GenInterfaces.Interfaces.Genealogic;

/// <summary>
/// Interface IGenBase
/// </summary>
public interface IGenBase
{
    /// <summary>
    /// Gets the u identifier.
    /// </summary>
    /// <value>The u identifier.</value>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    Guid UId { get; init; }
    /// <summary>
    /// Gets the type of the genealogy-object.
    /// </summary>
    /// <value>The type of the e gen.</value>
    EGenType eGenType { get; }
}