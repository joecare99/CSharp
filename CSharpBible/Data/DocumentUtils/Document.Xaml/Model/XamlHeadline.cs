using Document.Base.Models.Interfaces;

namespace Document.Xaml.Model;

public sealed class XamlHeadline : XamlContentBase, IDocHeadline
{
    public int Level { get; }

    public string Id { get; }

    public XamlHeadline(int level, string id)
    {
        Level = Math.Clamp(level, 1, 6);
        Id = id;
    }

    public override IDocStyleStyle GetStyle() => new XamlStyle($"H{Level}");
}