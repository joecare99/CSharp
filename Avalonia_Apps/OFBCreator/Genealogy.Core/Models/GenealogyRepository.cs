namespace Genealogy.Models;

/// <summary>
/// A repository record that identifies a source repository or archive.
/// </summary>
public sealed class GenealogyRepository : GenealogyRecord
{
    public GenealogyRepository()
    {
        Kind = "Repository";
        TypeCode = "REPO";
    }
}
