// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
//
// Last Modified By : Mir
// Last Modified On : 2025
// ***********************************************************************
// <copyright file="GBEModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Domain model for HGakte (Grundbuchakte / Land Register Entry)</summary>
// ***********************************************************************

namespace GenFreeWin.Models;

/// <summary>
/// Represents a Grundbucheintrag (land register entry / property use record).
/// </summary>
public class GBEModel
{
    /// <summary>
    /// Unique numeric identifier for this GBE.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Reference to parent Akte number.
    /// </summary>
    public string AkteNumber { get; set; } = string.Empty;

    /// <summary>
    /// Jahr / Year of the entry or registration.
    /// </summary>
    public string Jahr { get; set; } = string.Empty;

    /// <summary>
    /// Name / Description of the property or owner (at time of entry).
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Geb / Birth or acquisition information.
    /// </summary>
    public string Geb { get; set; } = string.Empty;

    /// <summary>
    /// Erb / Inheritance or succession information.
    /// </summary>
    public string Erb { get; set; } = string.Empty;

    /// <summary>
    /// Abg / Settlement, conveyance or other transfer information.
    /// </summary>
    public string Abg { get; set; } = string.Empty;

    /// <summary>
    /// Display text for UI list presentation.
    /// </summary>
    public string DisplayText => $"{Jahr} {Name}".Trim();

    /// <summary>
    /// Validates that the GBE has required minimum information.
    /// </summary>
    /// <returns>True if GBE is valid for persistence, false otherwise.</returns>
    public bool IsValid()
    {
        // GBE must have at least an Akte reference
        return !string.IsNullOrWhiteSpace(AkteNumber) && Id > 0;
    }
}
