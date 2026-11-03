using System;

namespace AA98_AvlnCodeStudio.Model.Workspaces;

/// <summary>
/// Indicates that a proposed file change conflicts with an open editor document.
/// </summary>
public sealed class WorkspaceOpenDocumentException : InvalidOperationException
{
    public WorkspaceOpenDocumentException(string relativePath)
        : base($"'{relativePath}' is open in an editor and is dirty, ambiguous, or no longer matches the reviewed content.")
    {
        RelativePath = relativePath;
    }

    public string RelativePath { get; }
}
