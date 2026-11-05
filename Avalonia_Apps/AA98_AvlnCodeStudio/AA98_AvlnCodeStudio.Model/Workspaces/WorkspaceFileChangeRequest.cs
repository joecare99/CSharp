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
        string? expectedContent,
        string? expectedOpenDocumentContent)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(relativePath);
        ArgumentNullException.ThrowIfNull(proposedContent);
        RelativePath = relativePath;
        ProposedContent = proposedContent;
        Target = target;
        ExpectedContent = expectedContent;
        ExpectedOpenDocumentContent = expectedOpenDocumentContent;
    }

    public string RelativePath { get; }

    public string ProposedContent { get; }

    public WorkspaceFileChangeTarget Target { get; }

    public string? ExpectedContent { get; }

    /// <summary>
    /// Gets the expected in-memory snapshot for an open document, when it differs from the file snapshot.
    /// </summary>
    public string? ExpectedOpenDocumentContent { get; }

    public static WorkspaceFileChangeRequest CreateNew(string relativePath, string proposedContent)
    {
        return new WorkspaceFileChangeRequest(
            relativePath,
            proposedContent,
            WorkspaceFileChangeTarget.MustNotExist,
            expectedContent: null,
            expectedOpenDocumentContent: null);
    }

    public static WorkspaceFileChangeRequest ReplaceExisting(
        string relativePath,
        string expectedContent,
        string proposedContent,
        string? expectedOpenDocumentContent = null)
    {
        ArgumentNullException.ThrowIfNull(expectedContent);
        return new WorkspaceFileChangeRequest(
            relativePath,
            proposedContent,
            WorkspaceFileChangeTarget.MustExist,
            expectedContent,
            expectedOpenDocumentContent);
    }
}

public enum WorkspaceFileChangeTarget
{
    MustNotExist,
    MustExist,
}
