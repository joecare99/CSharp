using Document.Base.Models.Interfaces;

namespace Document.Html.Model;

public sealed class HtmlLineBreak : HtmlContentBase
{
    public override IDocStyleStyle GetStyle() => new HtmlStyle();
}

public sealed class HtmlNbSpace : HtmlContentBase
{
    public override IDocStyleStyle GetStyle() => new HtmlStyle();
    public override string GetTextContent(bool xRecursive = true) => "\u00A0";
}

public sealed class HtmlTab : HtmlContentBase
{
    public override IDocStyleStyle GetStyle() => new HtmlStyle();
    public override string GetTextContent(bool xRecursive = true) => "\t";
}
