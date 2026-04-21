using Document.Base.Factories;
using Document.Docx;
using Document.Html;
using Document.Odf;
using Document.Pdf;
using Document.Xaml;

namespace Document.Demo.Gui.Services;

public static class DocumentProviderRegistry
{
    private static bool _isRegistered;

    public static void EnsureRegistered()
    {
        if (_isRegistered)
        {
            return;
        }

        UserDocumentFactory.AutoScanOnFirstUse = false;
        UserDocumentFactory.Register<HtmlDocument>("html", [".htm", ".html"], "text/html", overwrite: true);
        UserDocumentFactory.Register<XamlDocument>("xaml", [".xaml", ".xml"], "text/xaml", overwrite: true);
        UserDocumentFactory.Register<PdfDocument>("pdf", [".pdf"], "application/pdf", overwrite: true);
        UserDocumentFactory.Register<OdfTextDocument>("odf", [".odf"], "application/vnd.oasis.opendocument.text", overwrite: true);
        UserDocumentFactory.Register<DocxDocument>("docx", [".docx"], "application/vnd.openxmlformats-officedocument.wordprocessingml.document", overwrite: true);

        _isRegistered = true;
    }
}
