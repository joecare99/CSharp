namespace Document.Pdf.Render;

public interface IPdfEngine : IDisposable
{
    void BeginDocument();
    void AddPage();
    void SetFont(string family, bool bold, bool italic, double sizePt);
    void WriteText(string text);
    void WriteLine(string text);
    void AddHeadline(int level, string text);
    void SaveToFile(string path);
    void SaveToStream(Stream stream);
}