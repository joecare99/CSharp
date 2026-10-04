using System.Collections.Generic;

namespace Genealogy.Gedcom.Models;

internal sealed class GedcomSyntaxDocument
{
    public GedcomSyntaxDocument()
    {
    }

    public string? Version { get; set; }

    public bool HadRecoveryIssues { get; set; }

    public List<GedcomSyntaxNode> Records { get; set; } = new();
}
