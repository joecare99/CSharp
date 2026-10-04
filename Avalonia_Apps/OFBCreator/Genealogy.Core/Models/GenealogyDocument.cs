using System;
using System.Collections.Generic;

namespace Genealogy.Models;

/// <summary>
/// Provider-independent root document for genealogical data.
/// </summary>
public sealed class GenealogyDocument
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public IList<GenealogyRecord> Records { get; } = new List<GenealogyRecord>();

    public IList<GenealogyNode> OtherContent { get; } = new List<GenealogyNode>();

    public IList<GenealogyDiagnostic> Diagnostics { get; } = new List<GenealogyDiagnostic>();

    public GenealogyProviderData? ProviderData { get; set; }

    public bool HasRecoveryIssues { get; set; }
}
