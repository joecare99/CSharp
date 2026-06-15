namespace Document.Axaml;

internal sealed record InlineStyle(
    string? FontWeight = null,
    string? FontStyle = null,
    string? FontSize = null,
    string? TextDecorations = null)
{
    public static InlineStyle Empty { get; } = new();

    public InlineStyle ApplyAttributes(string? fontWeight, string? fontStyle, string? fontSize, string? textDecorations)
        => this with
        {
            FontWeight = fontWeight ?? FontWeight,
            FontStyle = fontStyle ?? FontStyle,
            FontSize = fontSize ?? FontSize,
            TextDecorations = textDecorations ?? TextDecorations,
        };
}
