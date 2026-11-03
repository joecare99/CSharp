namespace AA98_AvlnCodeStudio.Model.Workspaces;

/// <summary>
/// Provides a safe, snapshot-checked way to synchronize an open editor after a confirmed file change.
/// </summary>
public interface IWorkspaceOpenDocumentChangeProvider : IWorkspaceOpenDocumentProvider
{
    /// <summary>
    /// Returns whether exactly one open document can transition from the expected snapshot.
    /// </summary>
    bool CanApplyChange(string fullPath, string expectedContent);

    /// <summary>
    /// Applies a change only if the open document still matches the expected snapshot.
    /// A successful call must leave the document clean and matching the proposed snapshot.
    /// </summary>
    void ApplyChange(string fullPath, string expectedContent, string proposedContent);
}
