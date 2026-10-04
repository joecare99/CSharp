namespace Genealogy.Models;

/// <summary>
/// A provider-neutral tree node for private, unknown, or context-incompatible content.
/// </summary>
public sealed class UserDefinedEvent : GenealogyNode
{
    public string? Context { get; set; }
}
