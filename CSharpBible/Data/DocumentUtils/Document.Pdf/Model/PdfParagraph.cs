using Document.Base.Models.Interfaces;

namespace Document.Pdf.Model;

public sealed class PdfParagraph : PdfContentBase, IDocParagraph
{
    public string? StyleName { get; }
    public PdfParagraph(string? styleName = null) => StyleName = styleName;

    public IDocSpan AddBookmark(IDocFontStyle docFontStyle) => AddSpan(docFontStyle);

    public override IDocStyleStyle GetStyle() => new PdfStyle(StyleName);
}