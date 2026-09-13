using AA98_AvlnCodeStudio.Editor.Navigation;

namespace AA98_AvlnCodeStudio.UI.Navigation;

/// <summary>
/// Represents the document currently hosted by the CodeStudio workbench.
/// </summary>
public sealed class WorkbenchEditorDocument : IEditorDocument
{
    /// <summary>
    /// Initializes a workbench editor document.
    /// </summary>
    /// <param name="filePath">The normalized document path.</param>
    public WorkbenchEditorDocument(string filePath)
    {
        FilePath = filePath;
    }

    /// <inheritdoc/>
    public string FilePath { get; }
}
