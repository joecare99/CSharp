using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AA98_AvlnCodeStudio.Model.Workspaces;

/// <summary>
/// Prepares and applies explicitly confirmed text changes inside a workspace.
/// </summary>
/// <remarks>
/// Existing files retain UTF-8 encoding and BOM, or BOM-marked UTF-16/UTF-32 encoding.
/// Files without a BOM must be valid UTF-8; new files use UTF-8 without a BOM.
/// Invalid Unicode is rejected before review rather than silently replaced.
/// </remarks>
public sealed class WorkspaceFileChangeService
{
    private const int MaximumHistoryEntries = 20;
    private const long MaximumHistoryBytes = 32 * 1024 * 1024;
    private static readonly UTF8Encoding s_utf8 = new(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);
    private static readonly UTF8Encoding s_utf8WithBom = new(encoderShouldEmitUTF8Identifier: true, throwOnInvalidBytes: true);
    private static readonly UnicodeEncoding s_utf16Le = new(bigEndian: false, byteOrderMark: true, throwOnInvalidBytes: true);
    private static readonly UnicodeEncoding s_utf16Be = new(bigEndian: true, byteOrderMark: true, throwOnInvalidBytes: true);
    private static readonly UTF32Encoding s_utf32Le = new(bigEndian: false, byteOrderMark: true, throwOnInvalidCharacters: true);
    private static readonly UTF32Encoding s_utf32Be = new(bigEndian: true, byteOrderMark: true, throwOnInvalidCharacters: true);
    private static readonly StringComparer s_pathComparer = WorkspacePathIdentity.Comparer;
    private readonly IWorkspaceOpenDocumentProvider _openDocumentProvider;
    private readonly Stack<HistoryEntry> _undoHistory = [];
    private readonly Stack<HistoryEntry> _redoHistory = [];
    private string? _historyWorkspaceRoot;

    public WorkspaceFileChangeService(IWorkspaceOpenDocumentProvider openDocumentProvider)
    {
        _openDocumentProvider = openDocumentProvider
            ?? throw new ArgumentNullException(nameof(openDocumentProvider));
    }

    public bool CanUndo => _undoHistory.Count > 0;

    public bool CanRedo => _redoHistory.Count > 0;

    public string? HistoryWarning { get; private set; }

    public event EventHandler? HistoryChanged;

    public void ResetHistory()
    {
        _undoHistory.Clear();
        _redoHistory.Clear();
        _historyWorkspaceRoot = null;
        HistoryWarning = null;
        HistoryChanged?.Invoke(this, EventArgs.Empty);
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
            Encoding encoding = originalBytes is null ? s_utf8 : DetectEncoding(originalBytes);
            string? originalContent = originalBytes is null ? null : DecodeContent(originalBytes, encoding, relativePath);
            ValidateRequestTarget(request, relativePath, originalContent);
            byte[] proposedBytes = EncodeContent(request.ProposedContent, encoding, relativePath);
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
                request.ExpectedOpenDocumentContent ?? originalContent,
                requiresOpenDocument: request.ExpectedOpenDocumentContent is not null,
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

    private static Encoding DetectEncoding(ReadOnlySpan<byte> bytes)
    {
        if (bytes.StartsWith(s_utf32Le.Preamble)) return s_utf32Le;
        if (bytes.StartsWith(s_utf32Be.Preamble)) return s_utf32Be;
        if (bytes.StartsWith(s_utf8WithBom.Preamble)) return s_utf8WithBom;
        if (bytes.StartsWith(s_utf16Le.Preamble)) return s_utf16Le;
        if (bytes.StartsWith(s_utf16Be.Preamble)) return s_utf16Be;
        return s_utf8;
    }

    private static string DecodeContent(byte[] bytes, Encoding encoding, string relativePath)
    {
        try
        {
            return encoding.GetString(bytes, encoding.Preamble.Length, bytes.Length - encoding.Preamble.Length);
        }
        catch (DecoderFallbackException exception)
        {
            throw new InvalidDataException($"The existing file '{relativePath}' is not valid {encoding.WebName} text.", exception);
        }
    }

    private static byte[] EncodeContent(string content, Encoding encoding, string relativePath)
    {
        try
        {
            byte[] bytes = new byte[encoding.Preamble.Length + encoding.GetByteCount(content)];
            encoding.Preamble.CopyTo(bytes);
            encoding.GetBytes(content.AsSpan(), bytes.AsSpan(encoding.Preamble.Length));
            return bytes;
        }
        catch (EncoderFallbackException exception)
        {
            throw new InvalidDataException($"The proposed content for '{relativePath}' contains invalid Unicode text.", exception);
        }
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
            EnsureParentDirectoriesAreSafe(proposal.WorkspaceRoot, change.TargetPath, createMissing: false, null);
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
                EnsureParentDirectoriesAreSafe(proposal.WorkspaceRoot, change.TargetPath, true, createdDirectories);
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

            if (_openDocumentProvider is IWorkspaceOpenDocumentChangeProvider provider)
            {
                foreach (WorkspaceFileChange change in appliedChanges.Where(change =>
                             _openDocumentProvider.IsDocumentOpen(change.TargetPath)))
                {
                    provider.ApplyChange(
                        change.TargetPath,
                        change.DocumentOriginalContent!,
                        change.OriginalContent!,
                        change.ProposedContent,
                        change.ProposedContent);
                    synchronizedDocuments.Add(change);
                    if (!provider.CanApplyChange(change.TargetPath, change.ProposedContent, change.ProposedContent))
                    {
                        throw new IOException($"The open document '{change.RelativePath}' did not match the confirmed change after synchronization.");
                    }
                }
            }
        }
        catch (Exception applyException)
        {
            List<Exception> rollbackErrors = [];
            if (_openDocumentProvider is IWorkspaceOpenDocumentChangeProvider provider)
            {
                foreach (WorkspaceFileChange change in synchronizedDocuments.AsEnumerable().Reverse())
                {
                    try
                    {
                        provider.ApplyChange(
                            change.TargetPath,
                            change.ProposedContent,
                            change.ProposedContent,
                            change.DocumentOriginalContent!,
                            change.OriginalContent!);
                    }
                    catch (Exception rollbackException)
                    {
                        rollbackErrors.Add(new IOException($"Could not restore the open document '{change.RelativePath}' after applying the change proposal.", rollbackException));
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
                    rollbackErrors.Add(new IOException($"Could not restore '{change.RelativePath}' after applying the change proposal.", rollbackException));
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
                    rollbackErrors.Add(new IOException($"Could not remove temporary file '{temporaryPath}' after applying the change proposal failed.", cleanupException));
                }
            }

            stagedFiles.Clear();
            foreach (string directoryPath in createdDirectories.AsEnumerable().Reverse())
            {
                try
                {
                    if (Directory.Exists(directoryPath)) Directory.Delete(directoryPath);
                }
                catch (Exception cleanupException) when (cleanupException is IOException or UnauthorizedAccessException)
                {
                    rollbackErrors.Add(new IOException($"Could not remove the newly created directory '{directoryPath}' after applying the change proposal failed.", cleanupException));
                }
            }

            if (rollbackErrors.Count > 0)
            {
                rollbackErrors.Insert(0, applyException);
                throw new AggregateException("Applying the workspace changes failed and one or more files could not be restored.", rollbackErrors);
            }

            throw;
        }
        finally
        {
            foreach (string temporaryPath in stagedFiles.Values) File.Delete(temporaryPath);
        }

        RecordSuccessfulChange(proposal, createdDirectories);
    }

    public bool Undo()
    {
        if (_undoHistory.Count == 0) return false;
        HistoryEntry entry = _undoHistory.Peek();
        ApplyHistoryEntry(entry, undo: true);
        _undoHistory.Pop();
        _redoHistory.Push(entry);
        HistoryChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public bool Redo()
    {
        if (_redoHistory.Count == 0) return false;
        HistoryEntry entry = _redoHistory.Peek();
        ApplyHistoryEntry(entry, undo: false);
        _redoHistory.Pop();
        _undoHistory.Push(entry);
        TrimHistory(_undoHistory);
        HistoryChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }

    private void RecordSuccessfulChange(WorkspaceFileChangeProposal proposal, IReadOnlyList<string> createdDirectories)
    {
        HistoryWarning = null;
        if (_historyWorkspaceRoot is not null && !WorkspacePathIdentity.AreEqual(_historyWorkspaceRoot, proposal.WorkspaceRoot))
        {
            _undoHistory.Clear();
            _redoHistory.Clear();
        }

        _historyWorkspaceRoot = proposal.WorkspaceRoot;
        HistoryEntry entry = new(proposal.WorkspaceRoot, proposal.Changes.ToArray(), createdDirectories.ToArray());
        if (entry.Size > MaximumHistoryBytes)
        {
            _undoHistory.Clear();
            _redoHistory.Clear();
            HistoryWarning = "The confirmed workspace change exceeded the 32 MiB undo-history limit. It was applied but was not recorded, and existing undo/redo history was cleared.";
        }
        else
        {
            _undoHistory.Push(entry);
            TrimHistory(_undoHistory);
            _redoHistory.Clear();
        }

        HistoryChanged?.Invoke(this, EventArgs.Empty);
    }

    private static void TrimHistory(Stack<HistoryEntry> history)
    {
        HistoryEntry[] retained = history.Reverse().ToArray();
        long size = retained.Sum(static entry => entry.Size);
        int removeCount = 0;
        while (retained.Length - removeCount > MaximumHistoryEntries || size > MaximumHistoryBytes)
        {
            size -= retained[removeCount++].Size;
        }

        history.Clear();
        for (int index = removeCount; index < retained.Length; index++) history.Push(retained[index]);
    }

    private void ApplyHistoryEntry(HistoryEntry entry, bool undo)
    {
        if (!Directory.Exists(entry.WorkspaceRoot))
            throw new DirectoryNotFoundException($"Workspace directory '{entry.WorkspaceRoot}' no longer exists.");

        foreach (WorkspaceFileChange change in entry.Changes)
        {
            EnsureNoReparsePoints(entry.WorkspaceRoot, change.TargetPath);
            EnsureParentDirectoriesAreSafe(entry.WorkspaceRoot, change.TargetPath, false, null);
            bool expectedExists = undo || !change.IsNewFile;
            byte[]? expectedBytes = undo ? change.ProposedBytes : change.OriginalBytes;
            if (File.Exists(change.TargetPath) != expectedExists || (expectedExists && !File.ReadAllBytes(change.TargetPath).AsSpan().SequenceEqual(expectedBytes)))
                throw new WorkspaceFileChangeConflictException(change.RelativePath, $"'{change.RelativePath}' changed since this action. Undo/redo was cancelled without applying any changes.");

            bool targetExists = !undo || !change.IsNewFile;
            if (_openDocumentProvider.IsDocumentOpen(change.TargetPath)
                && (!expectedExists || !targetExists
                    || _openDocumentProvider is not IWorkspaceOpenDocumentChangeProvider provider
                    || !provider.CanApplyChange(
                        change.TargetPath,
                        undo ? change.ProposedContent : change.DocumentOriginalContent!,
                        undo ? change.ProposedContent : change.OriginalContent!)))
                throw new WorkspaceOpenDocumentException(change.RelativePath);
        }

        List<string> createdDirectories = [];
        List<HistoryFileTransition> applied = [];
        List<WorkspaceFileChange> synchronized = [];
        Dictionary<WorkspaceFileChange, string> stagedFiles = [];
        try
        {
            if (!undo)
            {
                foreach (string directoryPath in entry.CreatedDirectories)
                {
                    if (!Directory.Exists(directoryPath))
                    {
                        EnsureNoReparsePoints(entry.WorkspaceRoot, directoryPath);
                        Directory.CreateDirectory(directoryPath);
                        createdDirectories.Add(directoryPath);
                    }
                }
            }

            foreach (WorkspaceFileChange change in entry.Changes)
            {
                if (!undo || !change.IsNewFile)
                    stagedFiles.Add(change, WriteTemporaryFile(Path.GetDirectoryName(change.TargetPath)!, undo ? change.OriginalBytes! : change.ProposedBytes));
            }

            foreach (WorkspaceFileChange change in entry.Changes)
            {
                bool expectedExists = undo || !change.IsNewFile;
                byte[]? expectedBytes = undo ? change.ProposedBytes : change.OriginalBytes;
                if (File.Exists(change.TargetPath) != expectedExists || (expectedExists && !File.ReadAllBytes(change.TargetPath).AsSpan().SequenceEqual(expectedBytes)))
                    throw new WorkspaceFileChangeConflictException(change.RelativePath);

                bool targetExists = !undo || !change.IsNewFile;
                if (targetExists)
                {
                    if (expectedExists) File.Replace(stagedFiles[change], change.TargetPath, destinationBackupFileName: null);
                    else File.Move(stagedFiles[change], change.TargetPath);
                    stagedFiles.Remove(change);
                }
                else
                {
                    File.Delete(change.TargetPath);
                }

                applied.Add(new HistoryFileTransition(change, expectedExists, expectedBytes, targetExists, undo ? change.OriginalBytes : change.ProposedBytes));
            }

            if (_openDocumentProvider is IWorkspaceOpenDocumentChangeProvider provider)
            {
                foreach (WorkspaceFileChange change in entry.Changes.Where(change => _openDocumentProvider.IsDocumentOpen(change.TargetPath)))
                {
                    string expectedContent = undo ? change.ProposedContent : change.DocumentOriginalContent!;
                    string expectedSavedContent = undo ? change.ProposedContent : change.OriginalContent!;
                    string targetContent = undo ? change.DocumentOriginalContent! : change.ProposedContent;
                    string targetSavedContent = undo ? change.OriginalContent! : change.ProposedContent;
                    provider.ApplyChange(
                        change.TargetPath,
                        expectedContent,
                        expectedSavedContent,
                        targetContent,
                        targetSavedContent);
                    synchronized.Add(change);
                    if (!provider.CanApplyChange(change.TargetPath, targetContent, targetSavedContent))
                        throw new IOException($"The open document '{change.RelativePath}' did not match the undo/redo snapshot.");
                }
            }

            if (undo)
            {
                foreach (string directoryPath in Enumerable.Reverse(entry.CreatedDirectories))
                {
                    if (Directory.Exists(directoryPath) && !Directory.EnumerateFileSystemEntries(directoryPath).Any())
                    {
                        try
                        {
                            Directory.Delete(directoryPath);
                        }
                        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
                        {
                            HistoryWarning = $"Undo completed, but the empty generated directory '{directoryPath}' could not be removed: {exception.Message}";
                        }
                    }
                }
            }
        }
        catch (Exception exception)
        {
            List<Exception> rollbackErrors = [];
            if (_openDocumentProvider is IWorkspaceOpenDocumentChangeProvider provider)
            {
                foreach (WorkspaceFileChange change in synchronized.AsEnumerable().Reverse())
                {
                    try
                    {
                        string expectedContent = undo ? change.ProposedContent : change.DocumentOriginalContent!;
                        string expectedSavedContent = undo ? change.ProposedContent : change.OriginalContent!;
                        string targetContent = undo ? change.DocumentOriginalContent! : change.ProposedContent;
                        string targetSavedContent = undo ? change.OriginalContent! : change.ProposedContent;
                        if (provider.CanApplyChange(change.TargetPath, targetContent, targetSavedContent))
                            provider.ApplyChange(
                                change.TargetPath,
                                targetContent,
                                targetSavedContent,
                                expectedContent,
                                expectedSavedContent);
                        else if (!provider.CanApplyChange(change.TargetPath, expectedContent, expectedSavedContent))
                            throw new IOException($"The open document '{change.RelativePath}' could not be restored after undo/redo failed.");
                    }
                    catch (Exception rollbackException) { rollbackErrors.Add(rollbackException); }
                }
            }

            foreach (HistoryFileTransition transition in applied.AsEnumerable().Reverse())
            {
                try { RestoreHistoryFile(transition); }
                catch (Exception rollbackException) { rollbackErrors.Add(rollbackException); }
            }

            foreach (string directoryPath in createdDirectories.AsEnumerable().Reverse())
            {
                try { if (Directory.Exists(directoryPath) && !Directory.EnumerateFileSystemEntries(directoryPath).Any()) Directory.Delete(directoryPath); }
                catch (Exception rollbackException) when (rollbackException is IOException or UnauthorizedAccessException) { rollbackErrors.Add(rollbackException); }
            }

            foreach (string temporaryPath in stagedFiles.Values) File.Delete(temporaryPath);
            if (rollbackErrors.Count > 0)
            {
                rollbackErrors.Insert(0, exception);
                throw new AggregateException("Undo/redo failed and rollback was incomplete.", rollbackErrors);
            }

            throw;
        }
        finally
        {
            foreach (string temporaryPath in stagedFiles.Values) File.Delete(temporaryPath);
        }
    }

    private static void RestoreHistoryFile(HistoryFileTransition transition)
    {
        WorkspaceFileChange change = transition.Change;
        if (!transition.ExpectedExists)
        {
            if (transition.TargetExistsAfterTransition && File.Exists(change.TargetPath)
                && File.ReadAllBytes(change.TargetPath).AsSpan().SequenceEqual(transition.TargetBytesAfterTransition))
            {
                File.Delete(change.TargetPath);
                return;
            }

            if (File.Exists(change.TargetPath) || Directory.Exists(change.TargetPath))
                throw new IOException($"The undo/redo target '{change.RelativePath}' was created by another process.");
            return;
        }

        if (!File.Exists(change.TargetPath))
        {
            string temporary = WriteTemporaryFile(Path.GetDirectoryName(change.TargetPath)!, transition.ExpectedBytes!);
            try { File.Move(temporary, change.TargetPath); }
            finally { File.Delete(temporary); }
            return;
        }

        if (!File.ReadAllBytes(change.TargetPath).AsSpan().SequenceEqual(transition.TargetBytesAfterTransition))
            throw new IOException($"The undo/redo target '{change.RelativePath}' changed during rollback.");
        string restorePath = WriteTemporaryFile(Path.GetDirectoryName(change.TargetPath)!, transition.ExpectedBytes!);
        try { File.Replace(restorePath, change.TargetPath, destinationBackupFileName: null); }
        finally { File.Delete(restorePath); }
    }

    private sealed class HistoryEntry(string workspaceRoot, WorkspaceFileChange[] changes, string[] createdDirectories)
    {
        public string WorkspaceRoot { get; } = workspaceRoot;
        public WorkspaceFileChange[] Changes { get; } = changes;
        public string[] CreatedDirectories { get; } = createdDirectories;
        public long Size => Changes.Sum(static change => (long)(change.OriginalBytes?.Length ?? 0) + change.ProposedBytes.Length);
    }

    private sealed record HistoryFileTransition(WorkspaceFileChange Change, bool ExpectedExists, byte[]? ExpectedBytes, bool TargetExistsAfterTransition, byte[]? TargetBytesAfterTransition);

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
            if (change.RequiresOpenDocument)
            {
                throw new WorkspaceOpenDocumentException(change.RelativePath);
            }
            return;
        }

        if (change.DocumentOriginalContent is not null
            && _openDocumentProvider is IWorkspaceOpenDocumentChangeProvider changeProvider
            && changeProvider.CanApplyChange(change.TargetPath, change.DocumentOriginalContent, change.OriginalContent!))
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
