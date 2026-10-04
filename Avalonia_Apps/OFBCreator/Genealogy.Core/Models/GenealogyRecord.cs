using System;
using System.Collections.Generic;

namespace Genealogy.Models;

/// <summary>
/// A genealogical record such as a person, family, source, place, or media item.
/// </summary>
public class GenealogyRecord
{
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the provider-neutral record kind, for example Person or Family.
    /// </summary>
    public string Kind { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the original standardized record type code when one exists.
    /// </summary>
    public string? TypeCode { get; set; }

    public IList<GenealogyIdentifier> Identifiers { get; } = new List<GenealogyIdentifier>();

    public IList<GenealogyNode> Content { get; } = new List<GenealogyNode>();

    public IList<GenealogyAssociation> Associations { get; } = new List<GenealogyAssociation>();

    public string? DisplayName { get; set; }

    public string? GivenName { get; set; }

    public string? Surname { get; set; }

    public string? Sex { get; set; }
}
