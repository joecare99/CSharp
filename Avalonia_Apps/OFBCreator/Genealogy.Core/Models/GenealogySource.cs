namespace Genealogy.Models;

/// <summary>
/// A source record that can be cited by genealogical facts.
/// </summary>
public sealed class GenealogySource : GenealogyRecord
{
    public GenealogySource()
    {
        Kind = "Source";
        TypeCode = "SOUR";
    }
}
