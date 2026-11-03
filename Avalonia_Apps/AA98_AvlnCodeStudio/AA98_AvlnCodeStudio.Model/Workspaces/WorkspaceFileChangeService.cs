using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AA98_AvlnCodeStudio.Model.Workspaces;

/// <summary>
/// Prepares and applies explicitly confirmed text changes inside a workspace.
/// </summary>
public sealed class WorkspaceFileChangeService
{
    private static readonly UTF8Encoding s_utf8 = new(encoderShouldEmitUTF8Identifier: false);
    private static readonly StringComparer s_pathComparer = OperatingSystem.IsWindows()
        ? StringComparer.OrdinalIgnoreCase
        : StringComparer.Ordinal;
    private readonly IWorkspaceOpenDocumentProvider _openDocumentProvider;

    public WorkspaceFileChangeService(IWorkspaceOpenDocumentProvider openDocumentProvider)
    {
        _openDocumentProvider = openDocumentProvider
            ?? throw new ArgumentNullException(nameof(openDocumentProvider));
    }

    /// <summary>
    /// Captures the current contents of every target so callers can review changes before applying them.
    /// </summary>
    public WorkspaceFileChangeProposal Prepare(
        string workspaceRoot,
        IEnumerable<WorkspaceFileChangeRequest> requests)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspaceRoot);
        ArgumentNullException.ThrowIfNull(requests);

        string normalizedRoot = Path.GetFullPath(workspaceRoot);
        if (!Directory.Exists(normalizedRoot))
        {
            throw new DirectoryNotFoundException($"Workspace directory '{normalizedRoot}' does not exist.");
        }

        List<WorkspaceFileChange> changes = [];
        HashSet<string> targets = new(s_pathComparer);
        foreach (WorkspaceFileChangeRequest request in requests)
        {
            ArgumentNullException.ThrowIfNull(request);
            string relativePath = NormalizeRelativePath(request.RelativePath);
            string targetPath = ResolveTargetPath(normalizedRoot, relativePath);
            if (!targets.Add(targetPath))
            {
                throw new InvalidDataException($"The change proposal contains '{relativePath}' more than once.");
            }

            EnsureNoReparsePoints(normalizedRoot, targetPath);
            EnsureParentDirectoriesAreSafe(normalizedRoot, targetPath, createMissing: false, null);

            if (Directory.Exists(targetPath))
            {
                throw new IOException($"The change target '{relativePath}' is a directory.");
            }

            byte[]? originalBytes = File.Exists(targetPath) ? File.ReadAllBytes(targetPath) : null;
            string? originalContent = originalBytes is null
                ? null
                : File.ReadAllText(targetPath);
            ValidateRequestTarget(request, relativePath, originalContent);
            byte[] proposedBytes = s_utf8.GetBytes(request.ProposedContent);
            if (originalBytes is not null
                && (originalBytes.AsSpan().SequenceEqual(proposedBytes)
                    || string.Equals(originalContent, request.ProposedContent, StringComparison.Ordinal)))
            {
                continue;
            }

            WorkspaceFileChange change = new(
                relativePath,
                targetPath,
                request.Target,
                originalBytes,
                originalContent,
                request.ProposedContent,
                proposedBytes);
            EnsureDocumentCanBeChanged(change);
            changes.Add(change);
        }

        if (changes.Count == 0)
        {
            throw new InvalidDataException("The change proposal contains no file changes.");
        }

        return new WorkspaceFileChangeProposal(normalizedRoot, changes);
    }

    /// <summary>
    /// Applies a prepared proposal only when the caller explicitly confirms it.
    /// </summary>
    public void Apply(WorkspaceFileChangeProposal proposal, bool confirmed)
    {
        ArgumentNullException.ThrowIfNull(proposal);
        if (!confirmed)
        {
            throw new InvalidOperationException("The workspace file changes were not confirmed.");
        }

        if (!Directory.Exists(proposal.WorkspaceRoot))
        {
            throw new DirectoryNotFoundException($"Workspace directory '{proposal.WorkspaceRoot}' no longer exists.");
        }

        foreach (WorkspaceFileChange change in proposal.Changes)
        {
            EnsureDocumentCanBeChanged(change);
            EnsureParentDirectoriesAreSafe(
                proposal.WorkspaceRoot,
                change.TargetPath,
                createMissing: false,
                null);
            EnsureNoReparsePoints(proposal.WorkspaceRoot, change.TargetPath);
            EnsureUnchanged(change);
        }

        Dictionary<WorkspaceFileChange, string> stagedFiles = [];
        List<string> createdDirectories = [];
        List<WorkspaceFileChange> appliedChanges = [];
        List<WorkspaceFileChange> synchronizedDocuments = [];
        try
        {
            foreach (WorkspaceFileChange change in proposal.Changes)
            {
                string parentPath = Path.GetDirectoryName(change.TargetPath)!;
                EnsureParentDirectoriesAreSafe(
                    proposal.WorkspaceRoot,
                    change.TargetPath,
                    createMissing: true,
                    createdDirectories);
                stagedFiles.Add(change, WriteTemporaryFile(parentPath, change.ProposedBytes));
            }

            foreach (WorkspaceFileChange change in proposal.Changes)
            {
                EnsureDocumentCanBeChanged(change);
                EnsureNoReparsePoints(proposal.WorkspaceRoot, change.TargetPath);
                EnsureUnchanged(change);
            }

            foreach (WorkspaceFileChange change in proposal.Changes)
            {
                EnsureDocumentCanBeChanged(change);
                MoveStagedFile(stagedFiles[change], change);
                stagedFiles.Remove(change);
                appliedChanges.Add(change);
            }

            if (_openDocumentProvider is IWorkspaceOpenDocumentChangeProvider changeProvider)
            {
                foreach (WorkspaceFileChange change in appliedChanges.Where(change =>
                             _openDocumentProvider.IsDocumentOpen(change.TargetPath)))
                {
                    changeProvider.ApplyChange(
                        change.TargetPath,
                        change.OriginalContent!,
                        change.ProposedContent);
                    synchronizedDocuments.Add(change);
                }
            }
        }
        catch (Exception applyException)
        {
            List<Exception> rollbackErrors = [];
            if (_openDocumentProvider is IWorkspaceOpenDocumentChangeProvider changeProvider)
            {
                foreach (WorkspaceFileChange change in synchronizedDocuments.AsEnumerable().Reverse())
                {
                    try
                    {
                        changeProvider.ApplyChange(
                            change.TargetPath,
                            change.ProposedContent,
                            change.OriginalContent!);
                    }
                    catch (Exception rollbackException)
                    {
                        rollbackErrors.Add(new IOException(
                            $"Could not restore the open document '{change.RelativePath}' after applying the change proposal.",
                            rollbackException));
                    }
                }
            }

            foreach (WorkspaceFileChange change in appliedChanges.AsEnumerable().Reverse())
            {
                try
                {
                    RollBack(change);
                }
                catch (Exception rollbackException)
                {
                    rollbackErrors.Add(new IOException(
                        $"Could not restore '{change.RelativePath}' after applying the change proposal.",
                        rollbackException));
                }
            }

            foreach (string temporaryPath in stagedFiles.Values)
            {
                try
                {
                    File.Delete(temporaryPath);
                }
                catch (Exception cleanupException) when (cleanupException is IOException or UnauthorizedAccessException)
                {
                    rollbackErrors.Add(new IOException(
                        $"Could not remove temporary file '{temporaryPath}' after applying the change proposal failed.",
                        cleanupException));
                }
            }

            stagedFiles.Clear();
            foreach (string directoryPath in createdDirectories.AsEnumerable().Reverse())
            {
                try
                {
                    if (Directory.Exists(directoryPath))
                    {
                        Directory.Delete(directoryPath);
                    }
                }
                catch (Exception cleanupException) when (cleanupException is IOException or UnauthorizedAccessException)
                {
                    rollbackErrors.Add(new IOException(
                        $"Could not remove the newly created directory '{directoryPath}' after applying the change proposal failed.",
                        cleanupException));
                }
            }

            if (rollbackErrors.Count > 0)
            {
                rollbackErrors.Insert(0, applyException);
                throw new AggregateException(
                    "Applying the workspace changes failed and one or more files could not be restored.",
                    rollbackErrors);
            }

            throw;
        }
        finally
        {
            foreach (string temporaryPath in stagedFiles.Values)
            {
                File.Delete(temporaryPath);
            }
        }
    }

    private static void EnsureUnchanged(WorkspaceFileChange change)
    {
        bool exists = File.Exists(change.TargetPath);
        if (change.Target == WorkspaceFileChangeTarget.MustNotExist)
        {
            if (exists || Directory.Exists(change.TargetPath))
            {
                throw new WorkspaceFileChangeConflictException(change.RelativePath);
            }

            return;
        }

        if (!exists || !File.ReadAllBytes(change.TargetPath).AsSpan().SequenceEqual(change.OriginalBytes))
        {
            throw new WorkspaceFileChangeConflictException(change.RelativePath);
        }
    }

    private static void ValidateRequestTarget(
        WorkspaceFileChangeRequest request,
        string relativePath,
        string? originalContent)
    {
        if (request.Target == WorkspaceFileChangeTarget.MustNotExist)
        {
            if (originalContent is not null)
            {
                throw new WorkspaceFileChangeConflictException(
                    relativePath,
                    $"'{relativePath}' already exists and this proposal only allows creating a new file.");
            }

            return;
        }

        if (originalContent is null)
        {
            throw new WorkspaceFileChangeConflictException(
                relativePath,
                $"'{relativePath}' does not exist and this proposal requires replacing an existing file.");
        }

        if (!string.Equals(originalContent, request.ExpectedContent, StringComparison.Ordinal))
        {
            throw new WorkspaceFileChangeConflictException(
                relativePath,
                $"'{relativePath}' no longer matches the content this proposal was based on. Review the current file and prepare the changes again.");
        }
    }

    private static void MoveStagedFile(string stagedPath, WorkspaceFileChange change)
    {
        if (change.Target == WorkspaceFileChangeTarget.MustExist)
        {
            File.Replace(stagedPath, change.TargetPath, destinationBackupFileName: null);
            return;
        }

        try
        {
            File.Move(stagedPath, change.TargetPath);
        }
        catch (IOException) when (File.Exists(change.TargetPath) || Directory.Exists(change.TargetPath))
        {
            throw new WorkspaceFileChangeConflictException(
                change.RelativePath,
                $"'{change.RelativePath}' was created by another process while applying the proposal. The existing target was preserved.");
        }
    }

    private void EnsureDocumentCanBeChanged(WorkspaceFileChange change)
    {
        if (!_openDocumentProvider.IsDocumentOpen(change.TargetPath))
        {
            return;
        }

        if (change.OriginalContent is not null
            && _openDocumentProvider is IWorkspaceOpenDocumentChangeProvider changeProvider
            && changeProvider.CanApplyChange(change.TargetPath, change.OriginalContent))
        {
            return;
        }

        throw new WorkspaceOpenDocumentException(change.RelativePath);
    }

    private static void RollBack(WorkspaceFileChange change)
    {
        if (!File.Exists(change.TargetPath)
            || !File.ReadAllBytes(change.TargetPath).AsSpan().SequenceEqual(change.ProposedBytes))
        {
            throw new IOException("The file no longer contains the content written by this proposal.");
        }

        if (change.IsNewFile)
        {
            File.Delete(change.TargetPath);
            return;
        }

        string parentPath = Path.GetDirectoryName(change.TargetPath)!;
        string rollbackPath = WriteTemporaryFile(parentPath, change.OriginalBytes!);
        try
        {
            File.Move(rollbackPath, change.TargetPath, overwrite: true);
        }
        finally
        {
            File.Delete(rollbackPath);
        }
    }

    private static string WriteTemporaryFile(string directory, byte[] contents)
    {
        string temporaryPath = Path.Combine(directory, $".avln-change-{Guid.NewGuid():N}.tmp");
        try
        {
            using FileStream stream = new(
                temporaryPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 4096,
                FileOptions.WriteThrough);
            stream.Write(contents);
            stream.Flush(flushToDisk: true);
            return temporaryPath;
        }
        catch
        {
            File.Delete(temporaryPath);
            throw;
        }
    }

    private static string NormalizeRelativePath(string relativePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(relativePath);
        string platformPath = relativePath
            .Replace('\\', Path.DirectorySeparatorChar)
            .Replace('/', Path.DirectorySeparatorChar);
        if (Path.IsPathRooted(platformPath)
            || (platformPath.Length >= 2 && char.IsAsciiLetter(platformPath[0]) && platformPath[1] == ':')
            || platformPath.Split(Path.DirectorySeparatorChar).Any(static segment => segment is "." or ".."))
        {
            throw new ArgumentException("Change paths must be relative paths inside the workspace.", nameof(relativePath));
        }

        return platformPath;
    }

    private static string ResolveTargetPath(string root, string relativePath)
    {
        string targetPath = Path.GetFullPath(Path.Combine(root, relativePath));
        string rootPrefix = Path.EndsInDirectorySeparator(root)
            ? root
            : root + Path.DirectorySeparatorChar;
        if (!targetPath.StartsWith(rootPrefix, OperatingSystem.IsWindows()
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal))
        {
            throw new ArgumentException("Change paths must remain inside the workspace.");
        }

        return targetPath;
    }

    private static void EnsureNoReparsePoints(string root, string targetPath)
    {
        string relativePath = Path.GetRelativePath(root, targetPath);
        string currentPath = root;
        string[] segments = relativePath.Split(
            [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar],
            StringSplitOptions.RemoveEmptyEntries);
        foreach (string segment in segments)
        {
            currentPath = Path.Combine(currentPath, segment);
            if (!File.Exists(currentPath) && !Directory.Exists(currentPath))
            {
                continue;
            }

            if ((File.GetAttributes(currentPath) & FileAttributes.ReparsePoint) != 0)
            {
                throw new IOException($"Change path '{relativePath}' crosses a symbolic link or reparse point.");
            }
        }
    }

    private static void EnsureParentDirectoriesAreSafe(
        string root,
        string targetPath,
        bool createMissing,
        ICollection<string>? createdDirectories)
    {
        string? parentPath = Path.GetDirectoryName(targetPath);
        if (parentPath is null)
        {
            throw new IOException("The change target does not have a parent directory.");
        }

        string relativeParent = Path.GetRelativePath(root, parentPath);
        string currentPath = root;
        foreach (string segment in relativeParent.Split(
                     [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar],
                     StringSplitOptions.RemoveEmptyEntries))
        {
            currentPath = Path.Combine(currentPath, segment);
            if (File.Exists(currentPath))
            {
                throw new IOException($"Destination path '{currentPath}' is a file, not a directory.");
            }

            if (!Directory.Exists(currentPath))
            {
                if (!createMissing)
                {
                    continue;
                }

                Directory.CreateDirectory(currentPath);
                createdDirectories?.Add(currentPath);
            }

            if ((File.GetAttributes(currentPath) & FileAttributes.ReparsePoint) != 0)
            {
                throw new IOException($"Change path '{currentPath}' crosses a symbolic link or reparse point.");
            }
        }
    }
}
