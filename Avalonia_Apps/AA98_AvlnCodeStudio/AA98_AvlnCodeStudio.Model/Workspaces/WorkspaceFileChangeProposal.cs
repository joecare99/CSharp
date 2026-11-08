using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AA98_AvlnCodeStudio.Model.Workspaces;

/// <summary>
/// Captures a workspace-bounded set of text changes and their original file snapshots.
/// </summary>
public sealed class WorkspaceFileChangeProposal
{
    internal WorkspaceFileChangeProposal(string workspaceRoot, IList<WorkspaceFileChange> changes)
    {
        WorkspaceRoot = workspaceRoot;
        Changes = new ReadOnlyCollection<WorkspaceFileChange>(changes);
    }

    public string WorkspaceRoot { get; }

    public IReadOnlyList<WorkspaceFileChange> Changes { get; }
}

/// <summary>
/// Describes one file in a prepared change proposal.
/// </summary>
public sealed class WorkspaceFileChange
{
    private readonly byte[]? _originalBytes;
    private readonly byte[] _proposedBytes;

    internal WorkspaceFileChange(
        string relativePath,
        string targetPath,
        WorkspaceFileChangeTarget target,
        byte[]? originalBytes,
        string? originalContent,
        string? originalOpenDocumentContent,
        bool requiresOpenDocument,
        string proposedContent,
        byte[] proposedBytes)
    {
        RelativePath = relativePath;
        TargetPath = targetPath;
        Target = target;
        _originalBytes = originalBytes;
        OriginalContent = originalContent;
        OriginalOpenDocumentContent = originalOpenDocumentContent;
        RequiresOpenDocument = requiresOpenDocument;
        ProposedContent = proposedContent;
        _proposedBytes = proposedBytes;
    }

    public string RelativePath { get; }

    public bool IsNewFile => Target == WorkspaceFileChangeTarget.MustNotExist;

    public WorkspaceFileChangeTarget Target { get; }

    public string? OriginalContent { get; }

    /// <summary>
    /// Gets the open editor snapshot that was reviewed, which can differ from the saved file content.
    /// </summary>
    public string? OriginalOpenDocumentContent { get; }

    internal string? DocumentOriginalContent => OriginalOpenDocumentContent ?? OriginalContent;

    internal bool RequiresOpenDocument { get; }

    public string ProposedContent { get; }

    internal string TargetPath { get; }

    internal byte[]? OriginalBytes => _originalBytes;

    internal byte[] ProposedBytes => _proposedBytes;
}
