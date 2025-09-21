using Document.Base.Models.Interfaces;

namespace Document.Pdf.Model;

public sealed class PdfHeadline : PdfContentBase, IDocHeadline
{
    public int Level { get; }
    public object Page { get; internal set; }

    public string Id { get; }

    public PdfHeadline(int level, string Id) => (Level,Id) = (Math.Clamp(level, 1, 6),Id);
    public override IDocStyleStyle GetStyle() => new PdfStyle($"H{Level}");
}