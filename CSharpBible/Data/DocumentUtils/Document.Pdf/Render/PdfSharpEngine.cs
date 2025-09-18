using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
namespace Document.Pdf.Render;

public sealed class PdfSharpEngine : IPdfEngine
{
    private PdfSharp.Pdf.PdfDocument? _doc;
    private PdfPage? _page;
    private XGraphics? _gfx;
    private XFont _font;
    private readonly XSolidBrush _brush = XBrushes.Black;
    private double _y;
    private double _left = 40;
    private double _top = 40;
    private double _right = 40;

    public PdfSharpEngine()
    {
        GlobalFontSettings.UseWindowsFontsUnderWindows = true;
        _font = new("Arial", 12, XFontStyleEx.Regular);
    }

    private double ContentWidth => (_page?.Width ?? 595) - _left - _right;
    private double LineHeight => _font.GetHeight();

    public void BeginDocument()
    {

        _doc = new PdfSharp.Pdf.PdfDocument();
        AddPage();
        SetFont("Arial", bold: false, italic: false, sizePt: 12);
    }

    public void AddPage()
    {
        _page = (_doc ?? throw new InvalidOperationException()).AddPage();
        _gfx?.Dispose();
        _gfx = XGraphics.FromPdfPage(_page);
        _y = _top;
    }

    public void SetFont(string family, bool bold, bool italic, double sizePt)
    {
        var style = XFontStyleEx.Regular;
        if (bold) style |= XFontStyleEx.Bold;
        if (italic) style |= XFontStyleEx.Italic;
        _font = new XFont(string.IsNullOrWhiteSpace(family) ? "Arial" : family, sizePt <= 0 ? 12 : sizePt, style);
    }

    public void WriteText(string text)
    {
        if (string.IsNullOrEmpty(text)) return;
        var lines = text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        foreach (var line in lines)
        {
            EnsurePageSpace();
            _gfx!.DrawString(line, _font, _brush, new XRect(_left, _y, ContentWidth, LineHeight), XStringFormats.TopLeft);
            _y += LineHeight;
        }
    }

    public void WriteLine(string text)
    {
        WriteText(text);
    }

    public void AddHeadline(int level, string text)
    {
        var size = level switch
        {
            1 => 20,
            2 => 18,
            3 => 16,
            4 => 14,
            5 => 12,
            _ => 11
        };
        var prev = _font;
        SetFont(prev.Name2, bold: true, italic: false, sizePt: size);
        WriteText(text);
        _y += LineHeight * 0.5;
        _font = prev;
    }

    public void SaveToFile(string path)
    {
        _doc?.Save(path);
    }

    public void SaveToStream(Stream stream)
    {
        _doc?.Save(stream, closeStream: false);
    }

    private void EnsurePageSpace()
    {
        if (_page is null || _gfx is null) return;
        if (_y + LineHeight > _page.Height - _top)
            AddPage();
    }

    public void Dispose()
    {
        _gfx?.Dispose();
        _doc?.Dispose();
    }
}