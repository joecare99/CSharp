using System;
using System.IO;

namespace AA98_AvlnCodeStudio.Model.Workspaces;

/// <summary>
/// Indicates that a file no longer matches the snapshot shown in a change proposal.
/// </summary>
public sealed class WorkspaceFileChangeConflictException : IOException
{
    public WorkspaceFileChangeConflictException(string relativePath)
        : this(
            relativePath,
            $"'{relativePath}' changed after the proposal was prepared. Review the current file and prepare the changes again.")
    {
    }

    public WorkspaceFileChangeConflictException(string relativePath, string message)
        : base(message)
    {
        RelativePath = relativePath;
    }

    public string RelativePath { get; }
}
