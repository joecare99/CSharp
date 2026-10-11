namespace OFBCreator.Projects.Models;

/// <summary>
/// Project-local page, section, and semantic paragraph formatting.
/// </summary>
public sealed class OFBProjectLayout
{
    public double TopMarginMm { get; set; } = 20;

    public double BottomMarginMm { get; set; } = 20;

    public double LeftMarginMm { get; set; } = 20;

    public double RightMarginMm { get; set; } = 20;

    public int FamilyDirectoryColumns { get; set; } = 2;

    public int PersonIndexColumns { get; set; } = 3;

    public int OccupationIndexColumns { get; set; } = 3;

    public int PropertyIndexColumns { get; set; } = 3;

    public int HierarchicalPlaceIndexColumns { get; set; } = 3;

    public int AlphabeticalPlaceIndexColumns { get; set; } = 3;

    public OFBParagraphStyle FamilyEntry { get; set; } = OFBParagraphStyle.CreateDefault(spaceBeforePt: 6);

    public OFBParagraphStyle ParentEntry { get; set; } = OFBParagraphStyle.CreateDefault(indentPt: 36, hangingIndentPt: 18);

    public OFBParagraphStyle ParentLifeEvents { get; set; } = OFBParagraphStyle.CreateDefault(indentPt: 36);

    public OFBParagraphStyle ChildHeader { get; set; } = OFBParagraphStyle.CreateDefault(indentPt: 18);

    public OFBParagraphStyle ChildEntry { get; set; } = OFBParagraphStyle.CreateDefault(indentPt: 54, hangingIndentPt: 18);

    public OFBParagraphStyle ChildLifeEvents { get; set; } = OFBParagraphStyle.CreateDefault(indentPt: 72);

    public OFBParagraphStyle IndexEntry { get; set; } = OFBParagraphStyle.CreateDefault(hangingIndentPt: 9);

    public OFBProjectLayout Clone() => new()
    {
        TopMarginMm = TopMarginMm,
        BottomMarginMm = BottomMarginMm,
        LeftMarginMm = LeftMarginMm,
        RightMarginMm = RightMarginMm,
        FamilyDirectoryColumns = FamilyDirectoryColumns,
        PersonIndexColumns = PersonIndexColumns,
        OccupationIndexColumns = OccupationIndexColumns,
        PropertyIndexColumns = PropertyIndexColumns,
        HierarchicalPlaceIndexColumns = HierarchicalPlaceIndexColumns,
        AlphabeticalPlaceIndexColumns = AlphabeticalPlaceIndexColumns,
        FamilyEntry = FamilyEntry.Clone(),
        ParentEntry = ParentEntry.Clone(),
        ParentLifeEvents = ParentLifeEvents.Clone(),
        ChildHeader = ChildHeader.Clone(),
        ChildEntry = ChildEntry.Clone(),
        ChildLifeEvents = ChildLifeEvents.Clone(),
        IndexEntry = IndexEntry.Clone()
    };
}
