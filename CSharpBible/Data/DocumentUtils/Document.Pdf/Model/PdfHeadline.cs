using Document.Base.Models.Interfaces;

namespace Document.Pdf.Model;

public sealed class PdfHeadline : PdfContentBase, IDocHeadline
{
    public int Level { get; }
    public object Page { get; internal set; }

    public PdfHeadline(int level) => Level = Math.Clamp(level, 1, 6);
    public override IDocStyleStyle GetStyle() => new PdfStyle($"H{Level}");
}