using System;
using System.Linq;
using Document.Base.Models;
using Document.Base.Models.Interfaces;
using OFBCreator.Projects.Models;

namespace OFBCreator.Publishing.Services;

/// <summary>
/// Applies project layout through provider-neutral document capabilities.
/// </summary>
internal static class OFBDocumentLayout
{
    public static void ApplyPageMargins(IUserDocument document, OFBProjectLayout layout)
    {
        if (document is not IDocPageLayoutProvider pageLayoutProvider)
            throw new NotSupportedException("The selected document provider does not support page margins.");

        pageLayoutProvider.SetPageMarginsMm(
            layout.TopMarginMm,
            layout.BottomMarginMm,
            layout.LeftMarginMm,
            layout.RightMarginMm);
    }

    public static void AddSection(IUserDocument document, int columns)
    {
        if (document is not IDocSectionProvider sectionProvider)
            throw new NotSupportedException("The selected document provider does not support section layouts.");

        sectionProvider.AddSection(columns);
    }

    public static void ApplyStyle(IDocParagraph paragraph, OFBParagraphStyle style)
    {
        ArgumentNullException.ThrowIfNull(paragraph);
        ArgumentNullException.ThrowIfNull(style);
        SetAttribute(DocAttributeNames.FontFamily, style.FontFamily);
        SetAttribute(DocAttributeNames.FontSizePt, style.FontSizePt);
        SetIndentation(DocAttributeNames.IndentationBefore, style.IndentPt);
        SetIndentation(DocAttributeNames.IndentationHanging, style.HangingIndentPt);
        SetAttribute(DocAttributeNames.SpacingBeforePt, style.SpaceBeforePt);
        SetAttribute(DocAttributeNames.SpacingAfterPt, style.SpaceAfterPt);
        SetAttribute(DocAttributeNames.Alignment, style.Alignment);

        void SetIndentation(string name, int baseValue)
        {
            var additionalValue = paragraph.DocAttributes
                .LastOrDefault(attribute => attribute.Name == name)?.Value as int? ?? 0;
            var totalValue = baseValue + additionalValue;
            if (totalValue == 0)
            {
                RemoveAttribute(name);
                return;
            }
            SetAttribute(name, totalValue);
        }

        void SetAttribute(string name, object value)
        {
            RemoveAttribute(name);
            paragraph.DocAttributes.Add(new DocAttribute(name, value));
        }

        void RemoveAttribute(string name)
        {
            for (var index = paragraph.DocAttributes.Count - 1; index >= 0; index--)
                if (paragraph.DocAttributes[index].Name == name)
                    paragraph.DocAttributes.RemoveAt(index);
        }
    }
}
