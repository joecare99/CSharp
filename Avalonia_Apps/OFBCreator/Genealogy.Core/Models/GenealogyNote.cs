namespace Genealogy.Models;

/// <summary>
/// A standalone note record.
/// </summary>
public sealed class GenealogyNote : GenealogyRecord
{
    public GenealogyNote()
    {
        Kind = "Note";
        TypeCode = "NOTE";
    }
}
