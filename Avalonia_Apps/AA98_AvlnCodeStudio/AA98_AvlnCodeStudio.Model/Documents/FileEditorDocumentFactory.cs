namespace AA98_AvlnCodeStudio.Model.Documents;

/// <summary>
/// Creates the default plain text editor document implementation.
/// </summary>
public sealed class FileEditorDocumentFactory : IFileEditorDocumentFactory
{
    /// <inheritdoc/>
    public IFileEditorDocument Create()
    {
        return new FileEditorDocument();
    }
}