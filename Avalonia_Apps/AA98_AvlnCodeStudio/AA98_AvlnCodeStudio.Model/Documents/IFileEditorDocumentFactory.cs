namespace AA98_AvlnCodeStudio.Model.Documents;

/// <summary>
/// Creates editor document instances for a component workflow.
/// </summary>
public interface IFileEditorDocumentFactory
{
    /// <summary>
    /// Creates a new editor document instance.
    /// </summary>
    /// <returns>A new document instance.</returns>
    IFileEditorDocument Create();
}