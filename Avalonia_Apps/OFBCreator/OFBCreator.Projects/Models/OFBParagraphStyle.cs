namespace OFBCreator.Projects.Models;

/// <summary>
/// Editable font and paragraph properties for one semantic output role.
/// </summary>
public sealed class OFBParagraphStyle
{
    public string FontFamily { get; set; } = "Times New Roman";

    public double FontSizePt { get; set; } = 11;

    public int IndentPt { get; set; }

    public int HangingIndentPt { get; set; }

    public int SpaceBeforePt { get; set; }

    public int SpaceAfterPt { get; set; }

    public string Alignment { get; set; } = "Left";

    public OFBParagraphStyle Clone() => new()
    {
        FontFamily = FontFamily,
        FontSizePt = FontSizePt,
        IndentPt = IndentPt,
        HangingIndentPt = HangingIndentPt,
        SpaceBeforePt = SpaceBeforePt,
        SpaceAfterPt = SpaceAfterPt,
        Alignment = Alignment
    };

    public static OFBParagraphStyle CreateDefault(
        int indentPt = 0,
        int hangingIndentPt = 0,
        int spaceBeforePt = 0,
        int spaceAfterPt = 0) => new()
    {
        IndentPt = indentPt,
        HangingIndentPt = hangingIndentPt,
        SpaceBeforePt = spaceBeforePt,
        SpaceAfterPt = spaceAfterPt
    };
}
