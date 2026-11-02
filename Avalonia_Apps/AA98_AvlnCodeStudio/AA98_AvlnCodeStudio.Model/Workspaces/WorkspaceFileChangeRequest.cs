using System;

namespace AA98_AvlnCodeStudio.Model.Workspaces;

/// <summary>
/// Describes one proposed UTF-8 text file addition or replacement.
/// </summary>
public sealed class WorkspaceFileChangeRequest
{
    public WorkspaceFileChangeRequest(string relativePath, string proposedContent)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(relativePath);
        ArgumentNullException.ThrowIfNull(proposedContent);
        RelativePath = relativePath;
        ProposedContent = proposedContent;
    }

    public string RelativePath { get; }

    public string ProposedContent { get; }
}
