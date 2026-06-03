namespace AA98_AvlnCodeStudio.Model.Documents;

/// <summary>
/// Defines the editable state contract for a single plain text document.
/// </summary>
public interface IFileEditorDocument
{
    /// <summary>
    /// Gets the current file path.
    /// </summary>
    string? FilePath { get; }

    /// <summary>
    /// Gets the current document content.
    /// </summary>
    string Content { get; }

    /// <summary>
    /// Gets a value indicating whether the document has unsaved changes.
    /// </summary>
    bool IsDirty { get; }

    /// <summary>
    /// Gets the display name that should be shown for the document.
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Resets the document to a new empty state.
    /// </summary>
    void Reset();

    /// <summary>
    /// Loads a persisted text document into the current state.
    /// </summary>
    /// <param name="filePath">The source file path.</param>
    /// <param name="content">The loaded text content.</param>
    void Load(string filePath, string content);

    /// <summary>
    /// Updates the current document content.
    /// </summary>
    /// <param name="content">The new document content.</param>
    void UpdateContent(string content);

    /// <summary>
    /// Marks the document as saved.
    /// </summary>
    /// <param name="filePath">The persisted file path.</param>
    void MarkSaved(string filePath);
}