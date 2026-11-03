using System;

namespace AA98_AvlnCodeStudio.Model.Workspaces;

/// <summary>
/// Describes one proposed UTF-8 text file creation or snapshot-checked replacement.
/// </summary>
public sealed class WorkspaceFileChangeRequest
{
    private WorkspaceFileChangeRequest(
        string relativePath,
        string proposedContent,
        WorkspaceFileChangeTarget target,
        string? expectedContent)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(relativePath);
        ArgumentNullException.ThrowIfNull(proposedContent);
        RelativePath = relativePath;
        ProposedContent = proposedContent;
        Target = target;
        ExpectedContent = expectedContent;
    }

    public string RelativePath { get; }

    public string ProposedContent { get; }

    public WorkspaceFileChangeTarget Target { get; }

    public string? ExpectedContent { get; }

    public static WorkspaceFileChangeRequest CreateNew(string relativePath, string proposedContent)
    {
        return new WorkspaceFileChangeRequest(
            relativePath,
            proposedContent,
            WorkspaceFileChangeTarget.MustNotExist,
            expectedContent: null);
    }

    public static WorkspaceFileChangeRequest ReplaceExisting(
        string relativePath,
        string expectedContent,
        string proposedContent)
    {
        ArgumentNullException.ThrowIfNull(expectedContent);
        return new WorkspaceFileChangeRequest(
            relativePath,
            proposedContent,
            WorkspaceFileChangeTarget.MustExist,
            expectedContent);
    }
}

public enum WorkspaceFileChangeTarget
{
    MustNotExist,
    MustExist,
}
