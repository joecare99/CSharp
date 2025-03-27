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
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    DateTime? LastChange { get; }
}