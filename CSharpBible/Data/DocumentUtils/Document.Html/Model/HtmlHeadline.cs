using Document.Base.Models.Interfaces;

namespace Document.Html.Model;

public sealed class HtmlHeadline : HtmlContentBase, IDocHeadline
{
    public int Level { get; }

    public string Id { get; }

    public HtmlHeadline(int level, string id)
    {
        Level = Math.Clamp(level, 1, 6);
        Id = id;
    }

    public override IDocStyleStyle GetStyle() => new HtmlStyle($"H{Level}");
}
