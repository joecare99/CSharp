using System;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Model.Documents;

/// <summary>
/// Owns the content and persisted state of a document independently of its views.
/// </summary>
public interface IDocumentEditorSession : IDisposable
{
    string? FilePath { get; }

    string DisplayName { get; }

    bool IsDirty { get; }

    /// <summary>Raised before a user edit changes the document content.</summary>
    event EventHandler? ContentChanging;

    /// <summary>Raised after the path, display name, or dirty state changes.</summary>
    event EventHandler? StateChanged;

    /// <summary>Loads the specified document and sets its path and saved state.</summary>
    Task OpenAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves to the specified path and updates the document path and saved snapshot
    /// after a successful write. Edits made during the write remain dirty.
    /// </summary>
    Task SaveAsync(string filePath, CancellationToken cancellationToken = default);
}
