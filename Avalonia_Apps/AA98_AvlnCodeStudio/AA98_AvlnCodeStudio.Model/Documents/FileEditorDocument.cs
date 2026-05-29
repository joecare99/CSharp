using System;
using System.IO;

namespace AA98_AvlnCodeStudio.Model.Documents;

/// <summary>
/// Represents the editable state of a single plain text document.
/// </summary>
public sealed class FileEditorDocument
{
    private const string DefaultDocumentName = "Untitled.txt";

    /// <summary>
    /// Gets the current file path.
    /// </summary>
    public string? FilePath { get; private set; }

    /// <summary>
    /// Gets the current document content.
    /// </summary>
    public string Content { get; private set; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the document has unsaved changes.
    /// </summary>
    public bool IsDirty { get; private set; }

    /// <summary>
    /// Gets the display name that should be shown in the UI.
    /// </summary>
    public string DisplayName => string.IsNullOrWhiteSpace(FilePath)
        ? DefaultDocumentName
        : Path.GetFileName(FilePath);

    /// <summary>
    /// Resets the document to a new empty state.
    /// </summary>
    public void Reset()
    {
        FilePath = null;
        Content = string.Empty;
        IsDirty = false;
    }

    /// <summary>
    /// Loads a persisted text document into the current state.
    /// </summary>
    /// <param name="filePath">The source file path.</param>
    /// <param name="content">The loaded text content.</param>
    public void Load(string filePath, string content)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("A file path is required to load a document.", nameof(filePath));
        }

        FilePath = filePath;
        Content = content ?? string.Empty;
        IsDirty = false;
    }

    /// <summary>
    /// Updates the current document content and marks the document as modified when needed.
    /// </summary>
    /// <param name="content">The new document content.</param>
    public void UpdateContent(string content)
    {
        content ??= string.Empty;

        if (string.Equals(Content, content, StringComparison.Ordinal))
        {
            return;
        }

        Content = content;
        IsDirty = true;
    }

    /// <summary>
    /// Marks the document as successfully saved.
    /// </summary>
    /// <param name="filePath">The persisted file path.</param>
    public void MarkSaved(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("A file path is required to save a document.", nameof(filePath));
        }

        FilePath = filePath;
        IsDirty = false;
    }
}
