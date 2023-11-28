using System;
using System.Drawing;

namespace GenFree.Interfaces
{
    public interface IDocument
    {
        void AppendImage(Image image);
        void AppendText(string text);
        bool AppendTextIfNd(string sText = "\n");
        void ClearDocument();
        int GetIndent();
        void SetAlignment<T>(T eTextAlign) where T :Enum;
        void SetFont(Font font);
        void SetIndent(int iIndent);
        void SetHangingIndent(int iHIndent);
        bool TrimEnd();
        bool TrimEnd(string sText);
        void ReplaceLast(string v1, string v2);

        bool IsEmpty { get; }
    }
}