using Document.Base.Models.Interfaces;

namespace Document.Odf.Models;

public sealed class OdfHeadline : OdfContentBase, IDocHeadline
{
    public int Level { get; }

    public string Id { get; }

    public OdfHeadline(int level, string id)
    {
        Level = Math.Clamp(level, 1, 6);
        Id = id;
    }

    public override IDocStyleStyle GetStyle() => new OdfStyle($"H{Level}");
}
