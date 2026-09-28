using TranspilerLib.Data;

namespace TranspilerLib.CSharp.VBLegacyReplace.Services;

internal sealed class TemplatePart
{
    private TemplatePart(TokenData? literal, string? placeholderName, bool isBoolean)
    {
        Literal = literal;
        PlaceholderName = placeholderName;
        IsBoolean = isBoolean;
    }

    public TokenData? Literal { get; }
    public string? PlaceholderName { get; }
    public bool IsBoolean { get; }
    public bool IsPlaceholder => PlaceholderName is not null;

    public static TemplatePart ForLiteral(TokenData literal) => new(literal, null, false);
    public static TemplatePart ForPlaceholder(string name, bool isBoolean) => new(null, name, isBoolean);
}











