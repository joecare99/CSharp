// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
//
// Last Modified By : Mir
// Last Modified On : 2025
// ***********************************************************************
// <copyright file="HGAkteModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Domain model for HGakte (Grundbuchakte / Land Register Entry)</summary>
// ***********************************************************************

using System.Collections.Generic;

namespace Gen_FreeWin.Models;

/// <summary>
/// Represents a single Grundbuchakte (land register entry / property record).
/// Contains basic property information and a collection of associated Grundbucheinträge (property uses).
/// </summary>
public class HGAkteModel
{
    /// <summary>
    /// Unique numeric identifier for this Akte.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Akte number (primary identifier, user-facing).
    /// </summary>
    public string AkteNumber { get; set; } = string.Empty;

    /// <summary>
    /// Kirchspiel (parish or administrative district).
    /// </summary>
    public string Kirchspiel { get; set; } = string.Empty;

    /// <summary>
    /// Beschreibung / Description of the property or district.
    /// </summary>
    public string Beschreibung { get; set; } = string.Empty;

    /// <summary>
    /// Hof / Farm designation or class.
    /// </summary>
    public string Hof { get; set; } = string.Empty;

    /// <summary>
    /// Brandkasse (fire insurance class or related classification).
    /// </summary>
    public string Brandkasse { get; set; } = string.Empty;

    /// <summary>
    /// Bemerkungen / Additional remarks or notes.
    /// </summary>
    public string Bemerkungen { get; set; } = string.Empty;

    /// <summary>
    /// Flur / Field or plot designation.
    /// </summary>
    public string Flur { get; set; } = string.Empty;

    /// <summary>
    /// Parzelle / Parcel number or designation.
    /// </summary>
    public string Parzelle { get; set; } = string.Empty;

    /// <summary>
    /// Display text combining key fields for UI presentation.
    /// </summary>
    public string DisplayText => $"{AkteNumber} {Kirchspiel}".Trim();

    /// <summary>
    /// Associated Grundbucheinträge (land register entries) for this Akte.
    /// </summary>
    public List<GBEModel> Grundbucheintraege { get; set; } = new();

    /// <summary>
    /// Collection of persons/entities using or referenced in this property.
    /// </summary>
    public List<PropertyUsageModel> PropertyUsages { get; set; } = new();

    /// <summary>
    /// Validates that the Akte has required minimum information.
    /// </summary>
    /// <returns>True if Akte is valid for persistence, false otherwise.</returns>
    public bool IsValid()
    {
        // Akte must have at least an Akte number
        return !string.IsNullOrWhiteSpace(AkteNumber);
    }
}
