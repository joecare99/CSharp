using System;
using System.IO;

namespace AA98_AvlnCodeStudio.Model.Workspaces;

/// <summary>
/// Indicates that a file no longer matches the snapshot shown in a change proposal.
/// </summary>
public sealed class WorkspaceFileChangeConflictException : IOException
{
    public WorkspaceFileChangeConflictException(string relativePath)
        : base($"'{relativePath}' changed after the proposal was prepared. Review the current file and prepare the changes again.")
    {
        RelativePath = relativePath;
    }

    public string RelativePath { get; }
}
