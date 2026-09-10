namespace AA98_AvlnCodeStudio.Editor.Navigation;

/// <summary>
/// Represents a document that is available in the editor host.
/// </summary>
public interface IEditorDocument
{
    /// <summary>
    /// Gets the normalized source path represented by the document.
    /// </summary>
    string FilePath { get; }
}