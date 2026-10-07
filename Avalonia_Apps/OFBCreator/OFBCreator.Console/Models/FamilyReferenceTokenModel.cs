namespace OFBCreator.Console.Models;

/// <summary>
/// One display token in a compact family-reference list.
/// </summary>
public sealed class FamilyReferenceTokenModel
{
    /// <summary>Gets the token text.</summary>
    public required string Text { get; init; }

    /// <summary>Gets the internal family bookmark target, or an empty value for punctuation.</summary>
    public string Anchor { get; init; } = string.Empty;
}
