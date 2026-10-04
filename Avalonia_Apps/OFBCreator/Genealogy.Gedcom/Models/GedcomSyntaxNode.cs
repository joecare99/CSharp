using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Genealogy.Gedcom.Models;

internal sealed class GedcomSyntaxNode
{
    public GedcomSyntaxNode()
    {
    }

    public int Level { get; set; }

    [JsonIgnore]
    public int SourceLineNumber { get; set; }

    public string? CrossReference { get; set; }

    public string Tag { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public List<GedcomSyntaxNode> Children { get; set; } = new();
}
