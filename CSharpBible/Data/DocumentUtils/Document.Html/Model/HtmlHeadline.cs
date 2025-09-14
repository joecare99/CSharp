using Document.Base.Models.Interfaces;

namespace Document.Html.Model;

public sealed class HtmlHeadline : HtmlContentBase
{
    public int Level { get; }

    public HtmlHeadline(int level)
    {
        Level = Math.Clamp(level, 1, 6);
    }

    public override IDocStyleStyle GetStyle() => new HtmlStyle($"H{Level}");
}
