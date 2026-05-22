// RepoMigrator.Providers.Git/GitProvider.cs
using LibGit2Sharp;
using RepoMigrator.Core;
using RepoMigrator.Core.Abstractions;
using RepoMigrator.Core.Diagnostics;
using System.Diagnostics;
using System.Text;

namespace RepoMigrator.Providers.Git;

public sealed class GitProvider : IVersionControlProvider, IGitTargetRefOperations
{
    public const string ProviderKey = "git";
    private static Func<string, string?, string, CancellationToken, Task<string>> s_runGitCommandAsync = RunGitProcessAsync;
    private static Func<GitProvider, string, CancellationToken, Task> s_pushBranchAsync = static (provider, branchName, ct)
        => provider.PushBranchAsync(branchName, ct);
    private RepositoryEndpoint? _endpoint;
    private string? _localPath; // Lokales Arbeitsrepo für Quelle und/oder Ziel
    private string? _pushTargetUrl;
    private Repository? _repo;
    private readonly HashSet<string> _hsPendingPushBranchNames = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Task> _dctPendingPushTasksByBranchName = new(StringComparer.OrdinalIgnoreCase);
    private readonly object _pushSyncRoot = new();
    public string Name => "Git";
    public bool SupportsRead => true;
    public bool SupportsWrite => true;

    public async Task OpenAsync(RepositoryEndpoint endpoint, CancellationToken ct)
    {
        _endpoint = endpoint;

        // Für lokale Pfade: direkt öffnen; für Remote: temporär klonen
        _localPath = PrepareLocalPath(endpoint);
        if (!Repository.IsValid(_localPath))
        {
            CloneOptions? cloneOpts = null;
            if (!Directory.Exists(_localPath))
                Directory.CreateDirectory(_localPath!);
            if (LooksLikeRemote(endpoint.UrlOrPath))
            {
                cloneOpts = new CloneOptions();
                if (HasHttpCredentials(endpoint))
                {
                    var password = endpoint.Credentials?.Password ?? "";
                    cloneOpts.FetchOptions.CredentialsProvider = (_url, _user, _types) => new UsernamePasswordCredentials
                    {
                        Username = GetEffectiveHttpUsername(endpoint),
                        Password = password
                    };
                }
                Repository.Clone(GetAccessUrl(endpoint), _localPath!, cloneOpts);
            }
            else
            {
                // Initialisieren, falls als Ziel gedacht
                Repository.Init(_localPath!, isBare: false);
            }
        }

        _repo = new Repository(_localPath!);

        // Branch wechseln (falls Quelle)
        if (!string.IsNullOrWhiteSpace(endpoint.BranchOrTrunk) && _repo.Head.FriendlyName != endpoint.BranchOrTrunk)
        {
            var branch = _repo.Branches[endpoint.BranchOrTrunk] ??
                         _repo.Branches.Add(endpoint.BranchOrTrunk, _repo.Head.Tip);
            Commands.Checkout(_repo, branch);
        }

        await Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<string?> GetHeadCommitIdAsync(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        EnsureRepositoryIsOpen();
        return Task.FromResult(_repo!.Head.Tip?.Sha);
    }

    /// <inheritdoc/>
    public Task<bool> BranchExistsAsync(string branchName, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        ArgumentException.ThrowIfNullOrWhiteSpace(branchName);
        EnsureRepositoryIsOpen();
        return Task.FromResult(_repo!.Branches[NormalizeBranchName(branchName)!] is not null);
    }

    /// <inheritdoc/>
    public Task<bool> TagExistsAsync(string tagName, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        ArgumentException.ThrowIfNullOrWhiteSpace(tagName);
        EnsureRepositoryIsOpen();
        return Task.FromResult(_repo!.Tags[tagName] is not null);
    }

    /// <inheritdoc/>
    public async Task EnsureBranchAsync(string branchName, string commitId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        ArgumentException.ThrowIfNullOrWhiteSpace(branchName);
        ArgumentException.ThrowIfNullOrWhiteSpace(commitId);
        EnsureRepositoryIsOpen();

        var normalizedBranchName = NormalizeBranchName(branchName)!;
        var commit = FindCommit(commitId);
        var existingBranch = _repo!.Branches[normalizedBranchName];
        if (existingBranch is not null)
        {
            if (string.Equals(existingBranch.Tip?.Sha, commit.Sha, StringComparison.OrdinalIgnoreCase))
                return;

            throw new InvalidOperationException($"The Git branch '{normalizedBranchName}' already exists on a different commit.");
        }

        _repo.Branches.Add(normalizedBranchName, commit);
        if (_pushTargetUrl is not null)
        {
            await WaitForPendingBranchPushesAsync(excludeBranchName: normalizedBranchName).ConfigureAwait(false);
            await PushBranchAsync(normalizedBranchName, ct);
        }
    }

    /// <inheritdoc/>
    public async Task EnsureTagAsync(string tagName, string commitId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        ArgumentException.ThrowIfNullOrWhiteSpace(tagName);
        ArgumentException.ThrowIfNullOrWhiteSpace(commitId);
        EnsureRepositoryIsOpen();

        var commit = FindCommit(commitId);
        var existingTag = _repo!.Tags[tagName];
        if (existingTag is not null)
        {
            if (string.Equals(GetTagCommitId(existingTag), commit.Sha, StringComparison.OrdinalIgnoreCase))
                return;

            throw new InvalidOperationException($"The Git tag '{tagName}' already exists on a different commit.");
        }

        _repo.ApplyTag(tagName, commit.Sha);
        if (_pushTargetUrl is not null)
        {
            await WaitForPendingBranchPushesAsync().ConfigureAwait(false);
            await RunExclusiveGitRemoteOperationAsync(
                () => RunGitAsync($"push \"{_pushTargetUrl}\" \"refs/tags/{tagName}:refs/tags/{tagName}\"", _localPath, "Git-Tag-Push", ct));
        }
    }

    /// <summary>
    /// Returns Git provider capabilities for the supplied endpoint.
    /// </summary>
    public Task<RepositoryCapabilities> GetCapabilitiesAsync(RepositoryEndpoint endpoint, CancellationToken ct)
        => Task.FromResult(new RepositoryCapabilities
        {
            SupportsNativeHistoryTransfer = !LooksLikeRemote(endpoint.UrlOrPath),
            SupportsBranchSelection = !LooksLikeRemote(endpoint.UrlOrPath),
            SupportsTagSelection = !LooksLikeRemote(endpoint.UrlOrPath),
            SupportsMergeTopology = true
        });

    /// <summary>
    /// Returns selectable local Git branches and tags for UI-driven migrations.
    /// </summary>
    public async Task<RepositorySelectionData> GetSelectionDataAsync(RepositoryEndpoint endpoint, CancellationToken ct)
    {
        if (LooksLikeRemote(endpoint.UrlOrPath))
        {
            var sDefaultBranch = await GetRemoteHeadBranchAsync(endpoint, ct);
            var lstRemoteBranches = await GetRemoteReferenceInfosAsync(endpoint, referenceNamespace: "heads", ct);
            var lstRemoteTags = await GetRemoteReferenceInfosAsync(endpoint, referenceNamespace: "tags", ct);

            return new RepositorySelectionData
            {
                DefaultBranch = sDefaultBranch,
                Branches = lstRemoteBranches,
                Tags = lstRemoteTags
            };
        }

        var sLocalPath = Path.GetFullPath(endpoint.UrlOrPath);
        if (!Repository.IsValid(sLocalPath))
            throw new InvalidOperationException($"'{sLocalPath}' is not a valid Git repository.");

        using var gitRepository = new Repository(sLocalPath);
        var lstBranches = gitRepository.Branches
            .Where(gitBranch => !gitBranch.IsRemote)
            .Select(gitBranch => new RepositoryReferenceInfo
            {
                Name = gitBranch.FriendlyName,
                CommitId = gitBranch.Tip?.Sha
            })
            .OrderBy(gitBranch => gitBranch.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var lstTags = gitRepository.Tags
            .Select(gitTag => new RepositoryReferenceInfo
            {
                Name = gitTag.FriendlyName,
                CommitId = gitTag.Target is Commit gitCommit ? gitCommit.Sha : null
            })
            .OrderBy(gitTag => gitTag.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();

        return new RepositorySelectionData
        {
            DefaultBranch = gitRepository.Head.FriendlyName,
            Branches = lstBranches,
            Tags = lstTags
        };
    }

    public async Task<RepositoryProbeResult> ProbeAsync(RepositoryEndpoint endpoint, RepositoryAccessMode accessMode, CancellationToken ct)
    {
        try
        {
            if (!LooksLikeRemote(endpoint.UrlOrPath))
            {
                var localPath = Path.GetFullPath(endpoint.UrlOrPath);
                if (!Repository.IsValid(localPath))
                {
                    return new RepositoryProbeResult
                    {
                        Success = false,
                        Summary = "Lokales Git-Repository wurde nicht gefunden.",
                        Details = [$"Pfad: {localPath}"]
                    };
                }

                using var repo = new Repository(localPath);
                return new RepositoryProbeResult
                {
                    Success = true,
                    Summary = "Lokales Git-Repository ist erreichbar.",
                    Details =
                    [
                        $"Pfad: {localPath}",
                        $"Branch: {repo.Head.FriendlyName}",
                        $"Letzter Commit: {repo.Head.Tip?.Sha[..Math.Min(8, repo.Head.Tip.Sha.Length)] ?? "keiner"}"
                    ]
                };
            }

            var output = await RunGitAsync(
                $"ls-remote --symref \"{GetAccessUrl(endpoint)}\" HEAD",
                workingDir: null,
                operationName: "Git-Remote-Test",
                ct);

            var refs = output
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Take(3)
                .ToArray();

            var details = new List<string> { $"Remote: {endpoint.UrlOrPath}" };
            if (!string.IsNullOrWhiteSpace(endpoint.BranchOrTrunk))
                details.Add($"Ziel-Branch: {endpoint.BranchOrTrunk}");
            if (refs.Length > 0)
                details.AddRange(refs.Select(r => $"Ref: {r}"));
            else
                details.Add("Keine Referenzen gefunden. Das Remote ist möglicherweise leer.");

            if (accessMode == RepositoryAccessMode.Write)
                details.Add("Hinweis: Der Test bestätigt Erreichbarkeit und Authentifizierung. Schreibrechte werden endgültig erst beim Push geprüft.");

            return new RepositoryProbeResult
            {
                Success = true,
                Summary = "Git-Remote ist erreichbar.",
                Details = details
            };
        }
        catch (Exception ex)
        {
            return new RepositoryProbeResult
            {
                Success = false,
                Summary = ex.Message
            };
        }
    }

    /// <summary>
    /// Transfers Git history natively by pushing selected branches and optional tags from the source repository.
    /// </summary>
    public async Task TransferAsync(RepositoryEndpoint source, RepositoryEndpoint target, MigrationOptions options, IMigrationProgress progress, CancellationToken ct)
    {
        if (!string.Equals(source.ProviderKey, ProviderKey, StringComparison.OrdinalIgnoreCase)
            || !string.Equals(target.ProviderKey, ProviderKey, StringComparison.OrdinalIgnoreCase))
            throw new NotSupportedException("Native history transfer is only supported for Git to Git migrations.");

        if (LooksLikeRemote(source.UrlOrPath))
            throw new NotSupportedException("Native Git history transfer currently requires a local source repository.");

        var sSourcePath = Path.GetFullPath(source.UrlOrPath);
        if (!Repository.IsValid(sSourcePath))
            throw new InvalidOperationException($"'{sSourcePath}' is not a valid Git repository.");

        EnsureLocalTargetRepository(target);

        using var gitRepository = new Repository(sSourcePath);
        var lstSourceBranches = ResolveSourceBranches(gitRepository, source, options);
        var lstSourceTags = ResolveSourceTags(gitRepository, options);
        var hsExistingBranchNames = await GetExistingReferenceNamesAsync(target, referenceNamespace: "heads", ct);
        var hsExistingTagNames = await GetExistingReferenceNamesAsync(target, referenceNamespace: "tags", ct);
        var dtToday = DateOnly.FromDateTime(DateTime.UtcNow);
        var sTargetAccessUrl = GetAccessUrl(target);

        foreach (var sSourceBranchName in lstSourceBranches)
        {
            ct.ThrowIfCancellationRequested();

            var sRequestedTargetBranchName = options.TransferBranches
                ? sSourceBranchName
                : NormalizeBranchName(target.BranchOrTrunk) ?? sSourceBranchName;

            var sResolvedTargetBranchName = GitReferenceNameResolver.ResolveAvailableName(sRequestedTargetBranchName, hsExistingBranchNames, dtToday);
            progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.GitBranchTransferStarting, sSourceBranchName, sResolvedTargetBranchName);

            await RunGitAsync(
                $"push \"{sTargetAccessUrl}\" \"refs/heads/{sSourceBranchName}:refs/heads/{sResolvedTargetBranchName}\"",
                sSourcePath,
                "Git-Branch-Push",
                ct);

            hsExistingBranchNames.Add(sResolvedTargetBranchName);
        }

        foreach (var sSourceTagName in lstSourceTags)
        {
            ct.ThrowIfCancellationRequested();

            var sResolvedTargetTagName = GitReferenceNameResolver.ResolveAvailableName(sSourceTagName, hsExistingTagNames, dtToday);
            progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.GitTagTransferStarting, sSourceTagName, sResolvedTargetTagName);

            await RunGitAsync(
                $"push \"{sTargetAccessUrl}\" \"refs/tags/{sSourceTagName}:refs/tags/{sResolvedTargetTagName}\"",
                sSourcePath,
                "Git-Tag-Push",
                ct);

            hsExistingTagNames.Add(sResolvedTargetTagName);
        }
    }

    public Task<IReadOnlyList<ChangeSetInfo>> GetChangeSetsAsync(ChangeSetQuery query, CancellationToken ct)
    {
        var commits = _repo!.Commits.QueryBy(new CommitFilter
        {
            SortBy = query.OldestFirst ? CommitSortStrategies.Topological | CommitSortStrategies.Time : CommitSortStrategies.Time | CommitSortStrategies.Topological
        }).ToList();

        if (!string.IsNullOrEmpty(query.FromExclusiveId))
        {
            // überspringe bis nach FromExclusiveId
            var idx = commits.FindIndex(c => c.Sha.StartsWith(query.FromExclusiveId!, StringComparison.OrdinalIgnoreCase));
            if (idx >= 0)
                commits = commits.Skip(idx + 1).ToList();
        }

        if (!string.IsNullOrEmpty(query.ToInclusiveId))
        {
            var idx = commits.FindIndex(c => c.Sha.StartsWith(query.ToInclusiveId!, StringComparison.OrdinalIgnoreCase));
            if (idx >= 0)
                commits = commits.Take(idx + 1).ToList();
        }

        if (query.OldestFirst == false)
            commits.Reverse();

        if (query.MaxCount is int max)
            commits = commits.Take(max).ToList();

        var list = commits.Select(c => new ChangeSetInfo
        {
            Id = c.Sha,
            Message = c.Message,
            AuthorName = c.Author.Name,
            AuthorEmail = c.Author.Email,
            Timestamp = c.Author.When
        }).ToList().AsReadOnly();

        return Task.FromResult<IReadOnlyList<ChangeSetInfo>>(list);
    }

    public Task MaterializeSnapshotAsync(string workDir, string changeSetId, CancellationToken ct)
    {
        var commit = _repo!.Commits.First(c => c.Sha.StartsWith(changeSetId, StringComparison.OrdinalIgnoreCase));
        // Leeren
        CleanDirectory(workDir);

        // Tree exportieren
        ExportTree(commit.Tree, workDir);
        return Task.CompletedTask;
    }

    public async Task InitializeTargetAsync(RepositoryEndpoint endpoint, bool emptyInit, CancellationToken ct)
    {
        _endpoint = endpoint;
        EnsureLocalPushTargetRepository(endpoint);
        _pushTargetUrl = GetPushTargetUrl(endpoint);
        _localPath = _pushTargetUrl is not null
            ? Path.Combine(Path.GetTempPath(), "RepoMigrator", "git-target", Guid.NewGuid().ToString("N"))
            : Path.GetFullPath(endpoint.UrlOrPath);

        Directory.CreateDirectory(_localPath);

        if (!Repository.IsValid(_localPath))
            Repository.Init(_localPath);

        _repo?.Dispose();
        _repo = new Repository(_localPath);

        if (_pushTargetUrl is not null && _repo.Network.Remotes["origin"] is null)
        {
            _repo.Network.Remotes.Add("origin", _pushTargetUrl);

            var accessUrl = _pushTargetUrl;
            var targetBranch = NormalizeBranchName(_endpoint.BranchOrTrunk);

            await RunGitAsync($"fetch \"{accessUrl}\" \"+refs/heads/*:refs/remotes/origin/*\"", _localPath, "Git-Fetch", ct);

            var defaultRemoteBranch = await GetRemoteHeadBranchAsync(_endpoint, ct);
            if (!string.IsNullOrWhiteSpace(targetBranch) && await RemoteBranchExistsAsync(_endpoint, targetBranch, ct))
            {
                await RunGitAsync($"checkout -B \"{targetBranch}\" \"origin/{targetBranch}\"", _localPath, "Git-Checkout", ct);
            }
            else if (!string.IsNullOrWhiteSpace(targetBranch) && !string.IsNullOrWhiteSpace(defaultRemoteBranch))
            {
                await RunGitAsync($"checkout -B \"{targetBranch}\" \"origin/{defaultRemoteBranch}\"", _localPath, "Git-Checkout", ct);
            }
            else if (!string.IsNullOrWhiteSpace(targetBranch))
            {
                await RunGitAsync($"checkout -B \"{targetBranch}\"", _localPath, "Git-Checkout", ct);
            }
            else if (!string.IsNullOrWhiteSpace(defaultRemoteBranch))
            {
                await RunGitAsync($"checkout -B \"{defaultRemoteBranch}\" \"origin/{defaultRemoteBranch}\"", _localPath, "Git-Checkout", ct);
            }

            _repo.Dispose();
            _repo = new Repository(_localPath);
        }

        return;
    }

    public async Task CommitSnapshotAsync(string workDir, CommitMetadata metadata, CancellationToken ct)
        => await CommitSnapshotAsync(workDir, metadata, NullMigrationProgress.Instance, ct);

    public async Task CommitSnapshotAsync(string workDir, CommitMetadata metadata, IMigrationProgress progress, CancellationToken ct)
    {
        await EnsureWorkingBranchAsync(metadata.TargetBranch, ct);

        // Zielrepo vorausgesetzt: _repo zeigt auf target
        // Synchronisiere Inhalte aus workDir in _localPath
        SyncDirectoryContents(workDir, _localPath!);

        // Stage + Commit
        Commands.Stage(_repo!, "*");
        VerifyGitChangedPathCount(metadata, progress);
        var author = new Signature(metadata.AuthorName, metadata.AuthorEmail ?? "unknown@example.com", metadata.Timestamp);
        var committer = author;
        try
        {
            _repo!.Commit(metadata.Message, author, committer);
        }
        catch (EmptyCommitException)
        {
            return;
        }

        if (_pushTargetUrl is not null)
        {
            var endpoint = _endpoint!;
            var branchName = string.IsNullOrWhiteSpace(NormalizeBranchName(metadata.TargetBranch ?? endpoint.BranchOrTrunk))
                ? _repo!.Head.FriendlyName
                : NormalizeBranchName(metadata.TargetBranch ?? endpoint.BranchOrTrunk)!;
            lock (_pushSyncRoot)
            {
                _hsPendingPushBranchNames.Add(branchName);
            }

            var pendingPushTask = Task.Run(async () => await PushPendingBranchAsync(branchName).ConfigureAwait(false));
            lock (_pushSyncRoot)
            {
                _dctPendingPushTasksByBranchName[branchName] = pendingPushTask;
            }
        }
    }

    public async Task FlushAsync(CancellationToken ct)
    {
        if (_pushTargetUrl is null)
            return;

        List<string> lstPendingBranches;
        lock (_pushSyncRoot)
        {
            if (_hsPendingPushBranchNames.Count == 0)
                return;

            lstPendingBranches = _hsPendingPushBranchNames
                .OrderBy(static sValue => sValue, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        foreach (var sBranchName in lstPendingBranches)
            await PushPendingBranchAsync(sBranchName, ct);
    }

    private async Task PushBranchAsync(string sBranchName, CancellationToken ct)
    {
        if (_pushTargetUrl is null)
            return;

        var sPushArgs = $"push --set-upstream \"{_pushTargetUrl}\" \"refs/heads/{sBranchName}:refs/heads/{sBranchName}\"";
        await RunExclusiveGitRemoteOperationAsync(() => RunGitAsync(sPushArgs, _localPath, "Git-Push", ct));
    }

    private async Task PushPendingBranchAsync(string branchName, CancellationToken ct = default)
    {
        try
        {
            await s_pushBranchAsync(this, branchName, ct).ConfigureAwait(false);
        }
        catch
        {
            // FlushAsync übernimmt ggf. den erneuten Versuch und surfacet Fehler synchron am Ende.
        }
        finally
        {
            lock (_pushSyncRoot)
            {
                _hsPendingPushBranchNames.Remove(branchName);
                _dctPendingPushTasksByBranchName.Remove(branchName);
            }
        }
    }

    private async Task WaitForPendingBranchPushesAsync(string? excludeBranchName = null)
    {
        Task[] pendingTasks;
        lock (_pushSyncRoot)
        {
            pendingTasks = _dctPendingPushTasksByBranchName
                .Where(entry => string.IsNullOrWhiteSpace(excludeBranchName) || !string.Equals(entry.Key, excludeBranchName, StringComparison.OrdinalIgnoreCase))
                .Select(entry => entry.Value)
                .ToArray();
        }

        if (pendingTasks.Length == 0)
            return;

        await Task.WhenAll(pendingTasks).ConfigureAwait(false);
    }

    private static readonly SemaphoreSlim s_gitRemoteOperationLock = new(1, 1);

    private static async Task RunExclusiveGitRemoteOperationAsync(Func<Task> operation)
    {
        await s_gitRemoteOperationLock.WaitAsync().ConfigureAwait(false);
        try
        {
            await operation().ConfigureAwait(false);
        }
        finally
        {
            s_gitRemoteOperationLock.Release();
        }
    }

    private void VerifyGitChangedPathCount(CommitMetadata metadata, IMigrationProgress progress)
    {
        if (metadata.ExpectedChangedFilePathCount is not { } iExpectedChangedFilePathCount && metadata.ExpectedChangedPathCount is not { } iExpectedChangedPathCount)
            return;

        var summary = BuildGitStatusSummary();
        var iExpected = metadata.ExpectedChangedFilePathCount ?? metadata.ExpectedChangedPathCount!.Value;
        progress.Report(
            MigrationReportSeverity.Information,
            MigrationReportMessage.GitChangeCountVerification,
            summary.Total,
            summary.Modified,
            summary.Added,
            summary.Deleted,
            summary.Renamed,
            iExpected,
            metadata.ExpectedChangedPathCount ?? iExpected,
            summary.SamplePaths);

        if (!metadata.VerifyChangedPathCount || summary.Total <= iExpected)
            return;

        throw new InvalidOperationException($"Git target has {summary.Total} changed file paths before commit, but source revision reported {iExpected} changed file paths ({metadata.ExpectedChangedPathCount ?? iExpected} total SVN paths). Sample changed paths: {summary.SamplePaths}");
    }

    private GitStatusSummary BuildGitStatusSummary()
    {
        var lstChangedEntries = _repo!.RetrieveStatus(new StatusOptions { IncludeIgnored = false })
            .Where(statusEntry => statusEntry.State != FileStatus.Unaltered && statusEntry.State != FileStatus.Ignored)
            .GroupBy(statusEntry => statusEntry.FilePath, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .ToList();

        return new GitStatusSummary(
            lstChangedEntries.Count,
            lstChangedEntries.Count(static entry => entry.State.HasFlag(FileStatus.ModifiedInIndex) || entry.State.HasFlag(FileStatus.ModifiedInWorkdir)),
            lstChangedEntries.Count(static entry => entry.State.HasFlag(FileStatus.NewInIndex) || entry.State.HasFlag(FileStatus.NewInWorkdir)),
            lstChangedEntries.Count(static entry => entry.State.HasFlag(FileStatus.DeletedFromIndex) || entry.State.HasFlag(FileStatus.DeletedFromWorkdir)),
            lstChangedEntries.Count(static entry => entry.State.HasFlag(FileStatus.RenamedInIndex) || entry.State.HasFlag(FileStatus.RenamedInWorkdir)),
            string.Join(", ", lstChangedEntries.Select(static entry => entry.FilePath).OrderBy(static sPath => sPath, StringComparer.OrdinalIgnoreCase).Take(20)));
    }

    private sealed record GitStatusSummary(int Total, int Modified, int Added, int Deleted, int Renamed, string SamplePaths);

    public ValueTask DisposeAsync()
    {
        _repo?.Dispose();
        return ValueTask.CompletedTask;
    }

    private static bool LooksLikeRemote(string sUrlOrPath)
    {
        if (string.IsNullOrWhiteSpace(sUrlOrPath))
            return false;

        if (Path.IsPathRooted(sUrlOrPath) || sUrlOrPath.StartsWith("\\\\", StringComparison.OrdinalIgnoreCase))
            return false;

        if (sUrlOrPath.StartsWith("git@", StringComparison.OrdinalIgnoreCase))
            return true;

        if (!Uri.TryCreate(sUrlOrPath, UriKind.Absolute, out var uriEndpoint))
            return false;

        if (uriEndpoint.IsFile)
            return sUrlOrPath.StartsWith("file://", StringComparison.OrdinalIgnoreCase);

        return uriEndpoint.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase)
            || uriEndpoint.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase)
            || uriEndpoint.Scheme.Equals("ssh", StringComparison.OrdinalIgnoreCase)
            || uriEndpoint.Scheme.Equals("git", StringComparison.OrdinalIgnoreCase);
    }

    private async Task EnsureWorkingBranchAsync(string? sRequestedBranchName, CancellationToken ct)
    {
        var sBranchName = NormalizeBranchName(sRequestedBranchName ?? _endpoint?.BranchOrTrunk);
        if (string.IsNullOrWhiteSpace(sBranchName))
            return;

        if (string.Equals(_repo!.Head.FriendlyName, sBranchName, StringComparison.OrdinalIgnoreCase))
            return;

        var gitLocalBranch = _repo.Branches[sBranchName];
        if (gitLocalBranch is not null)
        {
            Commands.Checkout(_repo, gitLocalBranch);
            return;
        }

        if (_pushTargetUrl is not null)
        {
            var sRemoteBranchName = $"origin/{sBranchName}";
            var gitRemoteBranch = _repo.Branches[sRemoteBranchName];
            if (gitRemoteBranch is not null)
            {
                await RunGitAsync($"checkout -B \"{sBranchName}\" \"{sRemoteBranchName}\"", _localPath, "Git-Checkout", ct);
                _repo.Dispose();
                _repo = new Repository(_localPath!);
                return;
            }
        }

        await RunGitAsync($"checkout --orphan \"{sBranchName}\"", _localPath, "Git-Orphan-Checkout", ct);
        _repo.Dispose();
        _repo = new Repository(_localPath!);
    }

    private static string PrepareLocalPath(RepositoryEndpoint ep)
        => LooksLikeRemote(ep.UrlOrPath) ? Path.Combine(Path.GetTempPath(), "RepoMigrator", "git", Guid.NewGuid().ToString("N"))
                                         : GetRequiredLocalPath(ep.UrlOrPath, "Git repository");

    private static bool IsHttpRemote(string urlOrPath)
        => urlOrPath.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
           || urlOrPath.StartsWith("https://", StringComparison.OrdinalIgnoreCase);

    private static string? GetPushTargetUrl(RepositoryEndpoint endpoint)
    {
        if (LooksLikeRemote(endpoint.UrlOrPath))
            return GetAccessUrl(endpoint);

        var sLocalPath = GetRequiredLocalPath(endpoint.UrlOrPath, "Git target path");
        if (!Repository.IsValid(sLocalPath))
            return ShouldCreateBareLocalTargetRepository(sLocalPath) ? sLocalPath : null;

        using var gitRepository = new Repository(sLocalPath);
        return gitRepository.Info.IsBare ? sLocalPath : null;
    }

    private static string GetAccessUrl(RepositoryEndpoint endpoint)
    {
        if (!IsHttpRemote(endpoint.UrlOrPath) || !HasHttpCredentials(endpoint))
            return endpoint.UrlOrPath;

        var uri = new Uri(endpoint.UrlOrPath, UriKind.Absolute);
        var builder = new UriBuilder(uri)
        {
            UserName = GetEffectiveHttpUsername(endpoint),
            Password = endpoint.Credentials?.Password ?? string.Empty
        };

        return builder.Uri.AbsoluteUri;
    }

    private static void EnsureLocalTargetRepository(RepositoryEndpoint target)
    {
        if (LooksLikeRemote(target.UrlOrPath))
            return;

        var sLocalPath = GetRequiredLocalPath(target.UrlOrPath, "Git target path");
        EnsureDirectoryCanBeInitializedAsRepository(sLocalPath);
        Directory.CreateDirectory(sLocalPath);
        if (!Repository.IsValid(sLocalPath))
            Repository.Init(sLocalPath);
    }

    private static void EnsureLocalPushTargetRepository(RepositoryEndpoint endpoint)
    {
        if (LooksLikeRemote(endpoint.UrlOrPath))
            return;

        var sLocalPath = GetRequiredLocalPath(endpoint.UrlOrPath, "Git target path");
        EnsureDirectoryCanBeInitializedAsRepository(sLocalPath);
        if (Repository.IsValid(sLocalPath) || !ShouldCreateBareLocalTargetRepository(sLocalPath))
            return;

        Directory.CreateDirectory(sLocalPath);
        Repository.Init(sLocalPath, isBare: true);
    }

    private static bool ShouldCreateBareLocalTargetRepository(string sLocalPath)
    {
        if (!Directory.Exists(sLocalPath))
            return true;

        return !Directory.EnumerateFileSystemEntries(sLocalPath).Any();
    }

    private static string GetRequiredLocalPath(string sUrlOrPath, string sDescription)
    {
        if (string.IsNullOrWhiteSpace(sUrlOrPath))
            throw new InvalidOperationException($"{sDescription} must not be empty.");

        return Path.GetFullPath(sUrlOrPath.Trim());
    }

    private static void EnsureDirectoryCanBeInitializedAsRepository(string sLocalPath)
    {
        if (!Directory.Exists(sLocalPath) || Repository.IsValid(sLocalPath))
            return;

        if (Directory.EnumerateFileSystemEntries(sLocalPath).Any())
            throw new InvalidOperationException($"The local Git target path '{sLocalPath}' must be empty or already contain a Git repository.");
    }

    private static async Task<string?> GetRemoteHeadBranchAsync(RepositoryEndpoint endpoint, CancellationToken ct)
    {
        var output = await RunGitAsync(
            $"ls-remote --symref \"{GetAccessUrl(endpoint)}\" HEAD",
            workingDir: null,
            operationName: "Git-Remote-HEAD",
            ct);

        foreach (var line in output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
        {
            const string prefix = "ref: refs/heads/";
            if (line.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                var headIndex = line.IndexOf("\tHEAD", StringComparison.OrdinalIgnoreCase);
                if (headIndex > prefix.Length)
                    return line[prefix.Length..headIndex];
            }
        }

        return null;
    }

    private static async Task<bool> RemoteBranchExistsAsync(RepositoryEndpoint endpoint, string branchName, CancellationToken ct)
    {
        var output = await RunGitAsync(
            $"ls-remote --heads \"{GetAccessUrl(endpoint)}\" \"refs/heads/{branchName}\"",
            workingDir: null,
            operationName: "Git-Remote-Branch-Test",
            ct);

        return output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length > 0;
    }

    private static IReadOnlyList<string> ResolveSourceBranches(Repository gitRepository, RepositoryEndpoint source, MigrationOptions options)
    {
        if (!options.TransferBranches)
        {
            var sBranchName = NormalizeBranchName(source.BranchOrTrunk) ?? gitRepository.Head.FriendlyName;
            return [sBranchName];
        }

        var lstSelectedBranches = options.SelectedBranches.Count > 0
            ? options.SelectedBranches
            : gitRepository.Branches.Where(gitBranch => !gitBranch.IsRemote).Select(gitBranch => gitBranch.FriendlyName).ToList();

        foreach (var sSelectedBranch in lstSelectedBranches)
        {
            if (gitRepository.Branches[sSelectedBranch] is null)
                throw new InvalidOperationException($"The source branch '{sSelectedBranch}' does not exist.");
        }

        return lstSelectedBranches;
    }

    private static IReadOnlyList<string> ResolveSourceTags(Repository gitRepository, MigrationOptions options)
    {
        if (!options.TransferTags)
            return Array.Empty<string>();

        var lstSelectedTags = options.SelectedTags.Count > 0
            ? options.SelectedTags
            : gitRepository.Tags.Select(gitTag => gitTag.FriendlyName).ToList();

        foreach (var sSelectedTag in lstSelectedTags)
        {
            if (gitRepository.Tags[sSelectedTag] is null)
                throw new InvalidOperationException($"The source tag '{sSelectedTag}' does not exist.");
        }

        return lstSelectedTags;
    }

    private static async Task<HashSet<string>> GetExistingReferenceNamesAsync(RepositoryEndpoint endpoint, string referenceNamespace, CancellationToken ct)
    {
        if (!LooksLikeRemote(endpoint.UrlOrPath))
        {
            var sLocalPath = GetRequiredLocalPath(endpoint.UrlOrPath, "Git repository");
            if (!Repository.IsValid(sLocalPath))
                return new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            using var gitRepository = new Repository(sLocalPath);
            return referenceNamespace switch
            {
                "heads" => gitRepository.Branches.Where(gitBranch => !gitBranch.IsRemote).Select(gitBranch => gitBranch.FriendlyName).ToHashSet(StringComparer.OrdinalIgnoreCase),
                "tags" => gitRepository.Tags.Select(gitTag => gitTag.FriendlyName).ToHashSet(StringComparer.OrdinalIgnoreCase),
                _ => new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            };
        }

        var sOutput = await RunGitAsync(
            $"ls-remote --{referenceNamespace} \"{GetAccessUrl(endpoint)}\"",
            workingDir: null,
            operationName: "Git-Reference-List",
            ct);

        var hsReferenceNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var sLine in sOutput.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
        {
            var arrParts = sLine.Split('\t', StringSplitOptions.RemoveEmptyEntries);
            if (arrParts.Length != 2)
                continue;

            var sPrefix = $"refs/{referenceNamespace}/";
            if (arrParts[1].StartsWith(sPrefix, StringComparison.OrdinalIgnoreCase))
                hsReferenceNames.Add(arrParts[1][sPrefix.Length..]);
        }

        return hsReferenceNames;
    }

    private static async Task<IReadOnlyList<RepositoryReferenceInfo>> GetRemoteReferenceInfosAsync(RepositoryEndpoint endpoint, string referenceNamespace, CancellationToken ct)
    {
        var sOutput = await RunGitAsync(
            $"ls-remote --{referenceNamespace} \"{GetAccessUrl(endpoint)}\"",
            workingDir: null,
            operationName: "Git-Reference-Info-List",
            ct);

        var lstReferenceInfos = new List<RepositoryReferenceInfo>();
        foreach (var sLine in sOutput.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
        {
            var arrParts = sLine.Split('\t', StringSplitOptions.RemoveEmptyEntries);
            if (arrParts.Length != 2)
                continue;

            var sPrefix = $"refs/{referenceNamespace}/";
            if (!arrParts[1].StartsWith(sPrefix, StringComparison.OrdinalIgnoreCase))
                continue;

            lstReferenceInfos.Add(new RepositoryReferenceInfo
            {
                Name = arrParts[1][sPrefix.Length..],
                CommitId = arrParts[0]
            });
        }

        return lstReferenceInfos
            .OrderBy(referenceInfo => referenceInfo.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static bool HasHttpCredentials(RepositoryEndpoint endpoint)
        => !string.IsNullOrWhiteSpace(endpoint.Credentials?.Username)
           || !string.IsNullOrWhiteSpace(endpoint.Credentials?.Password);

    private static string? NormalizeBranchName(string? branchName)
        => string.IsNullOrWhiteSpace(branchName) ? null : branchName.Trim();

    private void EnsureRepositoryIsOpen()
    {
        if (_repo is null)
            throw new InvalidOperationException("The Git repository is not open.");
    }

    private Commit FindCommit(string commitId)
    {
        var commit = _repo!.Commits.FirstOrDefault(c => c.Sha.StartsWith(commitId, StringComparison.OrdinalIgnoreCase));
        return commit ?? throw new InvalidOperationException($"The Git commit '{commitId}' does not exist in the current repository.");
    }

    private static string? GetTagCommitId(Tag tag)
        => tag.PeeledTarget is Commit peeledCommit
            ? peeledCommit.Sha
            : tag.Target is Commit directCommit
                ? directCommit.Sha
                : null;

    private static string GetEffectiveHttpUsername(RepositoryEndpoint endpoint)
        => string.IsNullOrWhiteSpace(endpoint.Credentials?.Username)
            ? "git"
            : endpoint.Credentials.Username!;

    private static async Task<string> RunGitAsync(string arguments, string? workingDir, string operationName, CancellationToken ct)
        => await s_runGitCommandAsync(arguments, workingDir, operationName, ct);

    private static async Task<string> RunGitProcessAsync(string arguments, string? workingDir, string operationName, CancellationToken ct)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "git",
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };

        psi.Environment["GIT_TERMINAL_PROMPT"] = "0";
        psi.Environment["GCM_INTERACTIVE"] = "Never";

        if (!string.IsNullOrWhiteSpace(workingDir))
            psi.WorkingDirectory = workingDir;

        using var proc = new Process { StartInfo = psi, EnableRaisingEvents = true };
        var stdout = new StringBuilder();
        var stderr = new StringBuilder();

        proc.OutputDataReceived += (_, e) => { if (e.Data is not null) stdout.AppendLine(e.Data); };
        proc.ErrorDataReceived += (_, e) => { if (e.Data is not null) stderr.AppendLine(e.Data); };

        if (!proc.Start())
            throw new InvalidOperationException($"{operationName} konnte nicht gestartet werden.");

        proc.BeginOutputReadLine();
        proc.BeginErrorReadLine();

        await proc.WaitForExitAsync(ct);

        if (proc.ExitCode != 0)
        {
            var message = stderr.Length > 0 ? stderr.ToString().Trim() : stdout.ToString().Trim();
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(message) ? $"{operationName} ist fehlgeschlagen." : message);
        }

        return stdout.ToString();
    }

    private static void CleanDirectory(string dir)
    {
        foreach (var file in Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories))
            File.SetAttributes(file, FileAttributes.Normal);
        Directory.Delete(dir, true);
        Directory.CreateDirectory(dir);
    }

    private static void ExportTree(Tree tree, string dest)
    {
        foreach (var entry in tree)
        {
            switch (entry.TargetType)
            {
                case TreeEntryTargetType.Blob:
                    var blob = (LibGit2Sharp.Blob)entry.Target;
                    var path = Path.Combine(dest, entry.Path.Replace('/', Path.DirectorySeparatorChar));
                    Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                    using (var stream = blob.GetContentStream())
                    using (var fs = File.Create(path))
                        stream.CopyTo(fs);
                    break;
                case TreeEntryTargetType.Tree:
                    ExportTree((Tree)entry.Target, dest);
                    break;
                default:
                    // Submodule / Symlinks: optional behandeln
                    break;
            }
        }
    }

    private static void SyncDirectoryContents(string sourceDir, string destDir)
    {
        var sSnapshotDirectory = DifferentialSnapshotStore.StartOperation("Git");
        var dctSourceFiles = Directory.EnumerateFiles(sourceDir, "*", SearchOption.AllDirectories)
            .Select(sFilePath => new
            {
                RelativePath = Path.GetRelativePath(sourceDir, sFilePath),
                FullPath = sFilePath,
                Info = new FileInfo(sFilePath)
            })
            .ToDictionary(file => file.RelativePath, file => (file.FullPath, file.Info), StringComparer.OrdinalIgnoreCase);

        var dctDestFiles = Directory.EnumerateFiles(destDir, "*", SearchOption.AllDirectories)
            .Select(sFilePath => new
            {
                RelativePath = Path.GetRelativePath(destDir, sFilePath),
                FullPath = sFilePath,
                Info = new FileInfo(sFilePath)
            })
            .Where(file => !IsGitAdministrativePath(file.RelativePath))
            .ToDictionary(file => file.RelativePath, file => (file.FullPath, file.Info), StringComparer.OrdinalIgnoreCase);

        foreach (var (sRelativePath, destinationFile) in dctDestFiles)
        {
            if (dctSourceFiles.ContainsKey(sRelativePath))
                continue;

            destinationFile.Info.Attributes = FileAttributes.Normal;
            DifferentialSnapshotStore.SaveRemovedFile(sSnapshotDirectory, sRelativePath, destinationFile.FullPath);
            destinationFile.Info.Delete();
        }

        foreach (var (sRelativePath, sourceFile) in dctSourceFiles)
        {
            var sDestinationPath = Path.Combine(destDir, sRelativePath);
            if (dctDestFiles.TryGetValue(sRelativePath, out var destinationFile)
                && AreFilesContentEqual(sourceFile.FullPath, destinationFile.FullPath, sourceFile.Info.Length, destinationFile.Info.Length))
            {
                continue;
            }

            if (File.Exists(sDestinationPath))
                DifferentialSnapshotStore.SaveChangedFile(sSnapshotDirectory, sRelativePath, sourceFile.FullPath, sDestinationPath);
            else
                DifferentialSnapshotStore.SaveAddedFile(sSnapshotDirectory, sRelativePath, sourceFile.FullPath);

            Directory.CreateDirectory(Path.GetDirectoryName(sDestinationPath)!);
            VerifiedFileCopy.CopyAndVerify(sourceFile.FullPath, sDestinationPath);
        }

        RemoveEmptyDirectories(destDir);
    }

    private static bool AreFilesContentEqual(string sSourcePath, string sDestinationPath, long iSourceLength, long iDestinationLength)
    {
        if (iSourceLength != iDestinationLength)
            return false;

        if (iSourceLength == 0)
            return true;

        const int iBufferSize = 128 * 1024;
        var arrSourceBuffer = new byte[iBufferSize];
        var arrDestinationBuffer = new byte[iBufferSize];

        using var sourceStream = new FileStream(sSourcePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var destinationStream = new FileStream(sDestinationPath, FileMode.Open, FileAccess.Read, FileShare.Read);

        while (true)
        {
            var iReadSource = sourceStream.Read(arrSourceBuffer, 0, arrSourceBuffer.Length);
            var iReadDestination = destinationStream.Read(arrDestinationBuffer, 0, arrDestinationBuffer.Length);

            if (iReadSource != iReadDestination)
                return false;

            if (iReadSource == 0)
                return true;

            if (!arrSourceBuffer.AsSpan(0, iReadSource).SequenceEqual(arrDestinationBuffer.AsSpan(0, iReadDestination)))
                return false;
        }
    }

    private static bool IsGitAdministrativePath(string sRelativePath)
    {
        var sNormalizedPath = sRelativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        if (string.Equals(sNormalizedPath, ".git", StringComparison.OrdinalIgnoreCase))
            return true;

        return sNormalizedPath.StartsWith($".git{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase);
    }

    private static void RemoveEmptyDirectories(string sRootDirectory)
    {
        foreach (var sDirectory in Directory.EnumerateDirectories(sRootDirectory, "*", SearchOption.AllDirectories)
            .OrderByDescending(static sValue => sValue.Length))
        {
            var sRelativePath = Path.GetRelativePath(sRootDirectory, sDirectory);
            if (IsGitAdministrativePath(sRelativePath))
                continue;

            if (!Directory.EnumerateFileSystemEntries(sDirectory).Any())
                Directory.Delete(sDirectory, recursive: false);
        }
    }
}
