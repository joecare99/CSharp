namespace Genealogy.Models;

/// <summary>
/// An identifier assigned to an object by an external provider.
/// </summary>
public sealed class GenealogyIdentifier
{
    public string Provider { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;
}
