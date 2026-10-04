namespace Genealogy.Models;

/// <summary>
/// A family record with ordered facts and directed partner/child associations.
/// </summary>
public sealed class GenealogyFamily : GenealogyRecord
{
    public GenealogyFamily()
    {
        Kind = "Family";
        TypeCode = "FAM";
    }
}
