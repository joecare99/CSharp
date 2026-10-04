namespace Genealogy.Models;

/// <summary>
/// A person record with provider-independent identity, names, facts, and relationships.
/// </summary>
public sealed class GenealogyPerson : GenealogyRecord
{
    public GenealogyPerson()
    {
        Kind = "Person";
        TypeCode = "INDI";
    }
}
