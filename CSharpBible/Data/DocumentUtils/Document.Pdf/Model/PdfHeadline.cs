using Document.Base.Models.Interfaces;

namespace Document.Pdf.Model;

public sealed class PdfHeadline : PdfContentBase
{
    public int Level { get; }
    public PdfHeadline(int level) => Level = Math.Clamp(level, 1, 6);
    public override IDocStyleStyle GetStyle() => new PdfStyle($"H{Level}");
}