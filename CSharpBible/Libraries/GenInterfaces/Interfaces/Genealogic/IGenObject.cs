// ***********************************************************************
// Assembly         : GenInterfaces
// Author           : Mir
// Created          : 01-28-2025
//
// Last Modified By : Mir
// Last Modified On : 01-10-2025
// ***********************************************************************
// <copyright file="IGenObject.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Text.Json.Serialization;

/// <summary>
/// The Genealogic namespace.
/// </summary>
namespace GenInterfaces.Interfaces.Genealogic;

/// <summary>
/// Interface IGenObject
/// Extends the <see cref="GenInterfaces.Interfaces.Genealogic.IGenBase" />, It's the interface for all objects that contain  genealogycal data.
/// </summary>
public interface IGenObject : IGenBase 
{
    /// <summary>
    /// Gets the (local) identifier. This ID is only unique within the application, maybe classes and not across different applications.
    /// </summary>
    /// <value>The identifier.</value>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    int ID { get; init; }
    /// <summary>
    /// Gets the (time and) date of the last change.
    /// </summary>
    /// <value>The last change.</value>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    DateTime? LastChange { get; }
}