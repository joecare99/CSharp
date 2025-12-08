using Document.Base.Models.Interfaces;

namespace Document.Xaml.Model;

public sealed class XamlParagraph : XamlContentBase, IDocParagraph
{
    public string? StyleName { get; }

    public XamlParagraph(string? styleName = null)
    {
        StyleName = styleName;
    }

    public IDocSpan AddBookmark(IDocFontStyle docFontStyle)
    {
        var span = new XamlSpan(docFontStyle) { Id = Guid.NewGuid().ToString("N") };
        return (IDocSpan)AddChild(span);
    }

    public override IDocStyleStyle GetStyle() => new XamlStyle(StyleName);

    public IDocSpan AddBookmark(string Id, IDocFontStyle docFontStyle)
    {
        var span = new XamlSpan(docFontStyle) { Id = Id };
        return (IDocSpan)AddChild(span);
    }
}