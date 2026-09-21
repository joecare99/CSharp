using System;
using System.Collections.Generic;
using GenInterfaces.Interfaces;
using GenInterfaces.Interfaces.Genealogic;

namespace OFBCreator.Abstractions.Models;

/// <summary>
/// Diagnostic record explaining why an entity was excluded from OFB selection.
/// Contains actionable information for the user or automated processing.
/// </summary>
public sealed class OFBExclusionDiagnostic
{
    /// <summary>
    /// Unique identity of the excluded entity (GEDCOM pointer like I1, F1, P1).
    /// </summary>
    public string EntityId { get; init; } = default!;

    /// <summary>
    /// The entity type (Person, Family, Source, etc.).
    /// </summary>
    public string EntityType { get; init; } = default!;

    /// <summary>
    /// Display name or description of the excluded entity.
    /// May be empty if no name was found in source data.
    /// </summary>
    public string DisplayName { get; init; } = default!;

    /// <summary>
    /// Actionable reason code explaining why this entity was excluded.
    /// Examples: "PlaceNotInScope", "MissingBirthDate", "ExclusionRuleMatched".
    /// </summary>
    public string ReasonCode { get; init; } = default!;

    /// <summary>
    /// Human-readable explanation of the exclusion reason.
    /// May include suggestions for resolution.
    /// </summary>
    public string Explanation { get; init; } = default!;

    /// <summary>
    /// Creates a new exclusion diagnostic with validation.
    /// </summary>
    public static OFBExclusionDiagnostic Create(
        string entityId,
        string entityType,
        string reasonCode,
        string explanation )
    {
        return new()
        {
            EntityId = entityId ?? throw new ArgumentNullException( nameof( entityId ) ),
            EntityType = entityType ?? throw new ArgumentNullException( nameof( entityType ) ),
            ReasonCode = reasonCode ?? throw new ArgumentNullException( nameof( reasonCode ) ),
            Explanation = explanation,
        };
    }

    /// <summary>
    /// Creates a diagnostic from a GEDCOM entity with automatic identity extraction.
    /// </summary>
    public static OFBExclusionDiagnostic FromEntity( IGenEntity entity, string reasonCode, string explanation )
    {
        return new()
        {
            EntityId = GetEntityId( entity ),
            EntityType = GetEntityType( entity ),
            ReasonCode = reasonCode,
            Explanation = explanation,
        };
    }

    private static string GetEntityId( IGenEntity entity ) =>
        (entity as GenInterfaces.Interfaces.IHasOwner<object>)?.Owner?.ToString() ?? "Unknown";

    private static string GetEntityType( IGenEntity entity ) =>
        entity switch
        {
            IGenPerson => "Person",
            IGenFamily => "Family",
            IGenSource => "Source",
            IGenPlace => "Place",
            _ => "Entity",
        };
}

/// <summary>
/// Immutable snapshot of family/person selection derived from a GEDCOM import.
/// Carries all downstream input without requiring raw genealogy traversal.
/// </summary>
public sealed class OFBSourceSelection
{
    /// <summary>
    /// Selected families that form the basis of this OFB output.
    /// Already filtered, grouped and numbered by prior pipeline stages.
    /// </summary>
    public IReadOnlyList<IGenFamily> SelectedFamilies { get; init; } = new List<IGenFamily>().AsReadOnly();

    /// <summary>
    /// Persons referenced but not forming a primary family unit (virtual persons, orphan children).
    /// </summary>
    public IReadOnlyList<IGenPerson> VirtualPersons { get; init; } = new List<IGenPerson>().AsReadOnly();

    /// <summary>
    /// Exclusions captured during selection with actionable diagnostic information.
    /// </summary>
    public IReadOnlyList<OFBExclusionDiagnostic> Exclusions { get; init; } = new List<OFBExclusionDiagnostic>().AsReadOnly();

    /// <summary>
    /// Global identity counter — the last assigned family number used for display formatting.
    /// </summary>
    public int LastGlobalNumber { get; init; }

    /// <summary>
    /// Test data source for unit test scenarios. Contains raw object arrays (e.g., OFBFamilyModel)
    /// that support duck-typed property access (SourceRefId, Husband, Wife, Children, etc.).
    /// When populated, DocumentComposer uses this instead of SelectedFamilies to enable
    /// testing without full IGenFamily implementations. Should be null in production.
    /// </summary>
    public object? TestFamilyData { get; set; }

    /// <summary>
    /// Creates an empty selection with no data and a zero global counter.
    /// </summary>
    public static OFBSourceSelection Empty => new() { LastGlobalNumber = 0 };

    /// <summary>
    /// Returns true if any entities were selected during this operation.
    /// </summary>
    public bool HasData => SelectedFamilies.Count > 0 || VirtualPersons.Count > 0;
}
