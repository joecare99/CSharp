namespace TranspilerLib.CSharp.VBLegacyReplace.Services;

internal sealed class ReplacementSegment
{
    internal ReplacementSegment(string? literal, string? placeholderName)
    {
        Literal = literal;
        PlaceholderName = placeholderName;
    }

    public string? Literal { get; }
    public string? PlaceholderName { get; }

    public static ReplacementSegment ForLiteral(string literal) => new(literal, null);
    public static ReplacementSegment ForPlaceholder(string name) => new(null, name);
}











