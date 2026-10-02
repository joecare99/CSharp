using System.Collections.Generic;

namespace OFBCreator.Core.Models.Gedcom;

internal sealed class GedcomRecord
{
    public GedcomRecord(int level, string? xref, string tag, string value)
    {
        Level = level;
        Xref = xref;
        Tag = tag;
        Value = value;
    }

    public int Level { get; }
    public string? Xref { get; }
    public string Tag { get; }
    public string Value { get; set; }
    public List<GedcomRecord> Children { get; } = new();
}
