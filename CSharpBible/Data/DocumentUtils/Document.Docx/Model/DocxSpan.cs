using System.Collections.Generic;
using Document.Base.Models;
using Document.Base.Models.Interfaces;

namespace Document.Docx.Model;

public sealed class DocxSpan : DocxContentBase, IDocSpan
{
    public new IList<IDocAttributes> DocAttributes { get; } = new List<IDocAttributes>();

    public DocxSpan(IDocFontStyle style)
    {
        Style = style;
        CopyStyleToAttributes(style);
    }

    public IDocFontStyle Style { get; private set; }
    public bool IsLink { get; set; }
    public string? Href { get; set; }
    public string? Id { get; set; }

    public override IDocStyleStyle GetStyle() => new DocxStyle(Style.Name ?? "Span");

    public void SetStyle(object fs)
    {
        if (fs is IDocFontStyle idfs)
            SetStyle(idfs);
    }

    public void SetStyle(IDocFontStyle fs)
    {
        Style = fs;
        CopyStyleToAttributes(fs);
    }

    private void CopyStyleToAttributes(IDocFontStyle fs)
    {
        SetAttribute(DocAttributeNames.Bold, fs.Bold);
        SetAttribute(DocAttributeNames.Italic, fs.Italic);
        SetAttribute(DocAttributeNames.Underline, fs.Underline);
        SetAttribute(DocAttributeNames.Strikeout, fs.Strikeout);
        SetAttribute(DocAttributeNames.Color, fs.Color);
        SetAttribute(DocAttributeNames.FontFamily, fs.FontFamily);
        SetAttribute(DocAttributeNames.FontSizePt, fs.FontSizePt);
    }
    public void SetStyle(IUserDocument doc, object aFont) => SetStyle(aFont);
    public void SetStyle(IUserDocument doc, IDocFontStyle aFont) => SetStyle(aFont);
    public void SetStyle(string aStyleName)
    {
        Style = new DocxFontStyle { Name = aStyleName };
        SetAttribute("character.styleName", aStyleName);
    }

    private void SetAttribute(string name, object? value)
    {
        for (var index = DocAttributes.Count - 1; index >= 0; index--)
        {
            if (DocAttributes[index].Name == name)
                DocAttributes.RemoveAt(index);
        }

        if (value is not null)
            DocAttributes.Add(new DocAttribute(name, value));
    }
}
