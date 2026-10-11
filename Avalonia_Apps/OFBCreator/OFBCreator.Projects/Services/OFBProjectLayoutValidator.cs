using System;
using System.IO;
using OFBCreator.Projects.Models;

namespace OFBCreator.Projects.Services;

/// <summary>
/// Validates the persisted OFB page and paragraph layout.
/// </summary>
public static class OFBProjectLayoutValidator
{
    public static void Validate(OFBProjectLayout layout)
    {
        ArgumentNullException.ThrowIfNull(layout);

        ValidateMargin(layout.TopMarginMm, nameof(layout.TopMarginMm));
        ValidateMargin(layout.BottomMarginMm, nameof(layout.BottomMarginMm));
        ValidateMargin(layout.LeftMarginMm, nameof(layout.LeftMarginMm));
        ValidateMargin(layout.RightMarginMm, nameof(layout.RightMarginMm));
        if (layout.TopMarginMm + layout.BottomMarginMm >= 297)
            throw new InvalidDataException("Top and bottom margins must leave printable space on A4 paper.");
        if (layout.LeftMarginMm + layout.RightMarginMm >= 210)
            throw new InvalidDataException("Left and right margins must leave printable space on A4 paper.");

        ValidateColumns(layout.FamilyDirectoryColumns, nameof(layout.FamilyDirectoryColumns));
        ValidateColumns(layout.PersonIndexColumns, nameof(layout.PersonIndexColumns));
        ValidateColumns(layout.OccupationIndexColumns, nameof(layout.OccupationIndexColumns));
        ValidateColumns(layout.PropertyIndexColumns, nameof(layout.PropertyIndexColumns));
        ValidateColumns(layout.HierarchicalPlaceIndexColumns, nameof(layout.HierarchicalPlaceIndexColumns));
        ValidateColumns(layout.AlphabeticalPlaceIndexColumns, nameof(layout.AlphabeticalPlaceIndexColumns));

        ValidateStyle(layout.FamilyEntry, nameof(layout.FamilyEntry));
        ValidateStyle(layout.ParentEntry, nameof(layout.ParentEntry));
        ValidateStyle(layout.ParentLifeEvents, nameof(layout.ParentLifeEvents));
        ValidateStyle(layout.ChildHeader, nameof(layout.ChildHeader));
        ValidateStyle(layout.ChildEntry, nameof(layout.ChildEntry));
        ValidateStyle(layout.ChildLifeEvents, nameof(layout.ChildLifeEvents));
        ValidateStyle(layout.IndexEntry, nameof(layout.IndexEntry));
    }

    private static void ValidateMargin(double margin, string name)
    {
        if (!double.IsFinite(margin) || margin is < 5 or > 100)
            throw new InvalidDataException($"{name} must be between 5 and 100 millimeters.");
    }

    private static void ValidateColumns(int columns, string name)
    {
        if (columns is < 1 or > 4)
            throw new InvalidDataException($"{name} must be between 1 and 4.");
    }

    private static void ValidateStyle(OFBParagraphStyle? style, string name)
    {
        if (style is null)
            throw new InvalidDataException($"{name} cannot be null.");
        if (string.IsNullOrWhiteSpace(style.FontFamily) || style.FontFamily.Length > 100)
            throw new InvalidDataException($"{name}.FontFamily must contain between 1 and 100 characters.");
        if (!double.IsFinite(style.FontSizePt) || style.FontSizePt is < 4 or > 144)
            throw new InvalidDataException($"{name}.FontSizePt must be between 4 and 144 points.");
        if (style.IndentPt is < 0 or > 1440 || style.HangingIndentPt is < 0 or > 1440
            || style.SpaceBeforePt is < 0 or > 1440 || style.SpaceAfterPt is < 0 or > 1440)
            throw new InvalidDataException($"{name} paragraph measurements must be between 0 and 1440 points.");
        if (style.Alignment is not ("Left" or "Center" or "Right" or "Justified"))
            throw new InvalidDataException($"{name}.Alignment must be Left, Center, Right, or Justified.");
    }
}
