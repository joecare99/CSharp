using System;

namespace AA98_AvlnCodeStudio.Model.Workspaces;

/// <summary>
/// Indicates that a proposed file change conflicts with an open editor document.
/// </summary>
public sealed class WorkspaceOpenDocumentException : InvalidOperationException
{
    public WorkspaceOpenDocumentException(string relativePath)
        : base($"The editor for '{relativePath}' is closed, dirty, ambiguous, or no longer matches the reviewed content.")
    {
        RelativePath = relativePath;
    }

    public string RelativePath { get; }
}
