namespace AA98_AvlnCodeStudio.Model.Workspaces;

/// <summary>
/// Reports whether a workspace file currently has an open editor document.
/// </summary>
public interface IWorkspaceOpenDocumentProvider
{
    bool IsDocumentOpen(string fullPath);
}
