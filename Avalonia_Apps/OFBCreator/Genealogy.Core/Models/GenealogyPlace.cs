namespace Genealogy.Models;

/// <summary>
/// A place record with structured content and stable identity.
/// </summary>
public sealed class GenealogyPlace : GenealogyRecord
{
    public GenealogyPlace()
    {
        Kind = "Place";
        TypeCode = "PLAC";
    }
}
