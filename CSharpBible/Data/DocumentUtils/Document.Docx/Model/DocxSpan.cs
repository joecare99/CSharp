using Document.Base.Models.Interfaces;

namespace Document.Docx.Model;

public sealed class DocxSpan : DocxContentBase, IDocSpan
{
    public DocxSpan(IDocFontStyle style)
    {
        Style = style;
    }

    public IDocFontStyle Style { get; private set; }
    public bool IsLink { get; set; }
    public string? Href { get; set; }
    public string? Id { get; set; }

    public override IDocStyleStyle GetStyle() => new DocxStyle(Style.Name ?? "Span");

    public void SetStyle(object fs)
    {
        if (fs is IDocFontStyle idfs) Style = idfs;
    }

    public void SetStyle(IDocFontStyle fs) => Style = fs;
    public void SetStyle(IUserDocument doc, object aFont) => SetStyle(aFont);
    public void SetStyle(IUserDocument doc, IDocFontStyle aFont) => Style = aFont;
    public void SetStyle(string aStyleName) => Style = new DocxFontStyle { Name = aStyleName };
}
