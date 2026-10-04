using System;
using System.Collections.Generic;

namespace Genealogy.Models;

/// <summary>
/// An ordered, typed content node belonging to a genealogical record or event.
/// </summary>
public class GenealogyNode
{
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the semantic type code, independent of line-based serialization.
    /// </summary>
    public string TypeCode { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public IList<GenealogyNode> Children { get; } = new List<GenealogyNode>();

    public IList<GenealogyIdentifier> References { get; } = new List<GenealogyIdentifier>();
}
