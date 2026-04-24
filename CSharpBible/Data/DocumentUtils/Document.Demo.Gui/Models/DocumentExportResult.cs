namespace Document.Demo.Gui.Models;

public sealed class DocumentExportResult
{
    public DocumentExportResult(DocumentExportStatus status, Exception? exception = null)
    {
        Status = status;
        Exception = exception;
    }

    public DocumentExportStatus Status { get; }

    public Exception? Exception { get; }
}
