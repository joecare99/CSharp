using System;
using System.IO;
using System.Linq;
using System.Text;

namespace PdfMarkdownDeterministic.ConsoleApp;

internal static class PdfInspectorSelfCheck
{
    public static void Run()
    {
        string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".pdf");
        File.WriteAllBytes(path, Encoding.ASCII.GetBytes(CreateSyntheticPdf()));

        try
        {
            PdfDocumentInspection inspection = PdfStructureInspector.Inspect(path);

            if (!inspection.HasTextOperators || !inspection.HasVectorDrawingHints || !inspection.HasToUnicodeMap || inspection.ImageObjectCount < 1 || inspection.XObjectReferenceCount < 1 || inspection.InlineImageMarkerCount < 1 || !inspection.Fonts.Contains("Helvetica") || !inspection.XObjects.Contains("Im0") || !inspection.ContentHints.Contains("Do operator present") || !inspection.ContentHints.Contains("ToUnicode map present"))
            {
                throw new InvalidOperationException("Synthetic PDF inspection did not produce the expected heuristics.");
            }
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }

    private static string CreateSyntheticPdf()
    {
        return "%PDF-1.4\n" +
            "1 0 obj\n<< /Type /Catalog /Pages 2 0 R >>\nendobj\n" +
            "2 0 obj\n<< /Type /Pages /Kids [3 0 R] /Count 1 >>\nendobj\n" +
            "3 0 obj\n<< /Type /Page /Parent 2 0 R /Resources << /Font << /F1 4 0 R >> /XObject << /Im0 5 0 R >> >> /Contents 6 0 R >>\nendobj\n" +
            "4 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica /Encoding /WinAnsiEncoding /ToUnicode 7 0 R >>\nendobj\n" +
            "5 0 obj\n<< /Type /XObject /Subtype /Image /Width 10 /Height 10 >>\nendobj\n" +
            "6 0 obj\n<< /Length 44 >>\nstream\nBT /F1 12 Tf 0 0 m 10 10 l S /Im0 Do BI ID abc EI\nendstream\nendobj\n" +
            "7 0 obj\n<< /Length 12 >>\nstream\n/CIDInit /ProcSet findresource begin end\nendstream\nendobj\n";
    }
}
