namespace AA98_AvlnCodeStudio.Model.Workspaces;

/// <summary>
/// Provides a safe, snapshot-checked way to synchronize an open editor after a confirmed file change.
/// </summary>
public interface IWorkspaceOpenDocumentChangeProvider : IWorkspaceOpenDocumentProvider
{
    bool CanApplyChange(string fullPath, string expectedContent);

    void ApplyChange(string fullPath, string expectedContent, string proposedContent);
}
