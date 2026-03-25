// RepoMigrator.Providers.Git/GitProvider.cs
using LibGit2Sharp;
using RepoMigrator.Core;
using RepoMigrator.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RepoMigrator.Providers.Git;

public sealed class GitProvider : IVersionControlProvider
{
    private RepositoryEndpoint? _endpoint;
    private string? _localPath; // Lokales Arbeitsrepo für Quelle und/oder Ziel
    private Repository? _repo;
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
        _localPath = LooksLikeRemote(endpoint.UrlOrPath)
            ? Path.Combine(Path.GetTempPath(), "RepoMigrator", "git-target", Guid.NewGuid().ToString("N"))
            : Path.GetFullPath(endpoint.UrlOrPath);

        Directory.CreateDirectory(_localPath);

        if (!Repository.IsValid(_localPath))
            Repository.Init(_localPath);

        _repo?.Dispose();
        _repo = new Repository(_localPath);

        if (_endpoint is not null
            && LooksLikeRemote(_endpoint.UrlOrPath)
            && _repo.Network.Remotes["origin"] is null)
        {
            _repo.Network.Remotes.Add("origin", _endpoint.UrlOrPath);

            var accessUrl = GetAccessUrl(_endpoint);
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

    public Task CommitSnapshotAsync(string workDir, CommitMetadata metadata, CancellationToken ct)
    {
        // Zielrepo vorausgesetzt: _repo zeigt auf target
        // Synchronisiere Inhalte aus workDir in _localPath
        SyncDirectoryContents(workDir, _localPath!);

        // Stage + Commit
        Commands.Stage(_repo!, "*");
        var author = new Signature(metadata.AuthorName, metadata.AuthorEmail ?? "unknown@example.com", metadata.Timestamp);
        var committer = author;
        try
        {
            _repo!.Commit(metadata.Message, author, committer);
        }
        catch (EmptyCommitException)
        {
            return Task.CompletedTask;
        }

        if (_endpoint is not null && LooksLikeRemote(_endpoint.UrlOrPath))
        {
            var branchName = string.IsNullOrWhiteSpace(NormalizeBranchName(_endpoint.BranchOrTrunk))
                ? _repo!.Head.FriendlyName
                : NormalizeBranchName(_endpoint.BranchOrTrunk)!;
            var args = $"push --set-upstream \"{GetAccessUrl(_endpoint)}\" \"HEAD:refs/heads/{branchName}\"";
            return RunGitAsync(args, _localPath, "Git-Push", ct);
        }

        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        _repo?.Dispose();
        return ValueTask.CompletedTask;
    }

    private static bool LooksLikeRemote(string urlOrPath)
        => urlOrPath.StartsWith("http", StringComparison.OrdinalIgnoreCase)
           || urlOrPath.StartsWith("ssh", StringComparison.OrdinalIgnoreCase)
           || urlOrPath.EndsWith(".git", StringComparison.OrdinalIgnoreCase);

    private static string PrepareLocalPath(RepositoryEndpoint ep)
        => LooksLikeRemote(ep.UrlOrPath) ? Path.Combine(Path.GetTempPath(), "RepoMigrator", "git", Guid.NewGuid().ToString("N"))
                                         : Path.GetFullPath(ep.UrlOrPath);

    private static bool IsHttpRemote(string urlOrPath)
        => urlOrPath.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
           || urlOrPath.StartsWith("https://", StringComparison.OrdinalIgnoreCase);

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

    private static bool HasHttpCredentials(RepositoryEndpoint endpoint)
        => !string.IsNullOrWhiteSpace(endpoint.Credentials?.Username)
           || !string.IsNullOrWhiteSpace(endpoint.Credentials?.Password);

    private static string? NormalizeBranchName(string? branchName)
        => string.IsNullOrWhiteSpace(branchName) ? null : branchName.Trim();

    private static string GetEffectiveHttpUsername(RepositoryEndpoint endpoint)
        => string.IsNullOrWhiteSpace(endpoint.Credentials?.Username)
            ? "git"
            : endpoint.Credentials.Username!;

    private static async Task<string> RunGitAsync(string arguments, string? workingDir, string operationName, CancellationToken ct)
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
        // 1) Dateien löschen, die nicht mehr existieren
        var sourceAll = Directory.EnumerateFiles(sourceDir, "*", SearchOption.AllDirectories)
            .Select(p => p.Substring(sourceDir.Length).TrimStart(Path.DirectorySeparatorChar))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var dstFile in Directory.EnumerateFiles(destDir, "*", SearchOption.AllDirectories))
        {
            var rel = dstFile.Substring(destDir.Length).TrimStart(Path.DirectorySeparatorChar);
            if (rel.StartsWith(".git" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                continue;
            if (!sourceAll.Contains(rel))
            {
                File.SetAttributes(dstFile, FileAttributes.Normal);
                File.Delete(dstFile);
            }
        }

        // 2) Dateien kopieren/überschreiben
        foreach (var srcFile in Directory.EnumerateFiles(sourceDir, "*", SearchOption.AllDirectories))
        {
            var rel = srcFile.Substring(sourceDir.Length).TrimStart(Path.DirectorySeparatorChar);
            var dst = Path.Combine(destDir, rel);
            Directory.CreateDirectory(Path.GetDirectoryName(dst)!);
            File.Copy(srcFile, dst, overwrite: true);
        }
    }
}
