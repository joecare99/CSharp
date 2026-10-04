using System;

namespace Genealogy.Models;

/// <summary>
/// A directed relationship from one record to another, with a role and optional detail.
/// </summary>
public sealed class GenealogyAssociation
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public Guid SourceRecordId { get; set; }

    public Guid? TargetRecordId { get; set; }

    public GenealogyIdentifier? TargetIdentifier { get; set; }

    public string Role { get; set; } = string.Empty;

    public string? Detail { get; set; }
}
