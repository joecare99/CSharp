// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
//
// Last Modified By : Mir
// Last Modified On : 2025
// ***********************************************************************
// <copyright file="PropertyUsageModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Domain model for HGakte (Grundbuchakte / Land Register Entry)</summary>
// ***********************************************************************

namespace GenFreeWin.Models;

/// <summary>
/// Represents the usage of a property (person or entity linked to a property).
/// </summary>
public class PropertyUsageModel
{
    /// <summary>
    /// Reference to the Akte number.
    /// </summary>
    public string AkteNumber { get; set; } = string.Empty;

    /// <summary>
    /// Person ID linked to this property.
    /// </summary>
    public int PersonId { get; set; }

    /// <summary>
    /// Person display name (surname, givennames).
    /// </summary>
    public string PersonName { get; set; } = string.Empty;
}
