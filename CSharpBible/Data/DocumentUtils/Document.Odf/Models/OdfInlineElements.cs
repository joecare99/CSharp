using Document.Base.Models.Interfaces;

namespace Document.Odf.Models;

public sealed class OdfLineBreak : OdfContentBase
{
    public override IDocStyleStyle GetStyle() => new OdfStyle();
}

public sealed class OdfNbSpace : OdfContentBase
{
    public override IDocStyleStyle GetStyle() => new OdfStyle();
    public override string GetTextContent(bool xRecursive = true) => "\u00A0";
}

public sealed class OdfTab : OdfContentBase
{
    public override IDocStyleStyle GetStyle() => new OdfStyle();
    public override string GetTextContent(bool xRecursive = true) => "\t";
}
