// RepoMigrator.Providers.Git/GitProvider.cs
using LibGit2Sharp;
using RepoMigrator.Core;
using RepoMigrator.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace RepoMigrator.Providers.Git;

public sealed class GitProvider : IVersionControlProvider
{
    private string? _localPath; // Lokales Arbeitsrepo für Quelle und/oder Ziel
    private Repository? _repo;
    public string Name => "Git";
    public bool SupportsRead => true;
    public bool SupportsWrite => true;

    public async Task OpenAsync(RepositoryEndpoint endpoint, CancellationToken ct)
    {
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
                if (endpoint.Credentials?.Username is not null)
                {
                    cloneOpts.FetchOptions.CredentialsProvider = (_url, _user, _cred) =>
                        new UsernamePasswordCredentials
                        {
                            Username = endpoint.Credentials!.Username!,
                            Password = endpoint.Credentials!.Password ?? ""
                        };
                }
                Repository.Clone(endpoint.UrlOrPath, _localPath!, cloneOpts);
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

    public Task InitializeTargetAsync(string targetPath, bool emptyInit, CancellationToken ct)
    {
        if (!Repository.IsValid(targetPath))
        {
            Repository.Init(targetPath);
        }
        return Task.CompletedTask;
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
        _repo!.Commit(metadata.Message, author, committer);

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
