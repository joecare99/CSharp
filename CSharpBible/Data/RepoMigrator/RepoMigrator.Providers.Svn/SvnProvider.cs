// RepoMigrator.Providers.Svn/SvnProvider.cs
using RepoMigrator.Core.Abstractions;
using RepoMigrator.Core;

namespace RepoMigrator.Providers.Svn;

public sealed class SvnProvider : IVersionControlProvider
{
    private string? _wcPath; // Working copy path (source or target)
    public string Name => "Subversion";
    public bool SupportsRead => true;
    public bool SupportsWrite => true;

    public Task OpenAsync(RepositoryEndpoint endpoint, CancellationToken ct)
    {
        _wcPath = PrepareWorkingCopy(endpoint);

        if (LooksLikeRemote(endpoint.UrlOrPath))
        {
            using var client = new SvnClient();
            ApplyCredentials(client, endpoint);

            if (!Directory.Exists(_wcPath))
                Directory.CreateDirectory(_wcPath!);
            // Checkout falls leer
            if (!Directory.EnumerateFileSystemEntries(_wcPath!).Any())
            {
                var targetUrl = new Uri(endpoint.UrlOrPath);
                client.CheckOut(targetUrl, _wcPath!);
            }
        }
        else
        {
            Directory.CreateDirectory(_wcPath!);
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<ChangeSetInfo>> GetChangeSetsAsync(ChangeSetQuery query, CancellationToken ct)
    {
        using var client = new SvnClient();
        var list = new List<ChangeSetInfo>();
        client.Log(new Uri(GetRepoRoot(_wcPath!)), new SvnLogArgs
        {
            Range = new SvnRevisionRange(SvnRevision.Head, SvnRevision.Zero)
        }, (sender, e) =>
        {
            list.Add(new ChangeSetInfo
            {
                Id = e.Revision.ToString(),
                Message = e.LogMessage ?? "",
                AuthorName = e.Author ?? "unknown",
                AuthorEmail = null,
                Timestamp = e.Time
            });
        });

        // Reverse to oldest-first
        list.Reverse();

        if (!string.IsNullOrEmpty(query.FromExclusiveId))
            list = list.SkipWhile(c => c.Id != query.FromExclusiveId).Skip(1).ToList();

        if (!string.IsNullOrEmpty(query.ToInclusiveId))
            list = list.TakeWhile(c => c.Id != query.ToInclusiveId).Concat(list.Where(c => c.Id == query.ToInclusiveId).Take(1)).ToList();

        if (query.MaxCount is int max)
            list = list.Take(max).ToList();

        return Task.FromResult<IReadOnlyList<ChangeSetInfo>>(list.AsReadOnly());
    }

    public Task MaterializeSnapshotAsync(string workDir, string changeSetId, CancellationToken ct)
    {
        using var client = new SvnClient();
        Directory.CreateDirectory(workDir);
        var rev = new SvnRevision(long.Parse(changeSetId));
        var exportArgs = new SvnExportArgs { Revision = rev, Depth = SvnDepth.Infinity, IgnoreExternals = false, Overwrite = true };
        client.Export(new Uri(GetRepoRoot(_wcPath!)), workDir, exportArgs);
        return Task.CompletedTask;
    }

    public Task InitializeTargetAsync(string targetPath, bool emptyInit, CancellationToken ct)
    {
        // Für Ziel: Erwartet Working Copy. Falls leer, optional checkout/initialisieren.
        Directory.CreateDirectory(targetPath);
        // Hier könnte man einen "svn checkout" auf eine existierende Repo-URL machen (aus Endpoint.UrlOrPath),
        // oder bei neuem Repo: "svn mkdir/commit" extern vorbereiten.
        _wcPath = targetPath;
        return Task.CompletedTask;
    }

    public Task CommitSnapshotAsync(string workDir, CommitMetadata metadata, CancellationToken ct)
    {
        using var client = new SvnClient();

        // 1) Sync Dateien (ähnlich wie Git-Provider)
        SyncDirectory(workDir, _wcPath!);

        // 2) SVN-Adds/Deletes erkennen
        client.Status(_wcPath!, out var statuses);
        foreach (var s in statuses)
        {
            if (s.LocalNodeStatus == SvnStatus.Missing)
                client.Delete(s.Path);
            else if (s.LocalNodeStatus == SvnStatus.NotVersioned)
                client.Add(s.Path);
        }

        // 3) Commit
        var args = new SvnCommitArgs { LogMessage = metadata.Message };
        client.Commit(_wcPath!, args, out _);

        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    private static bool LooksLikeRemote(string s) => s.StartsWith("http", StringComparison.OrdinalIgnoreCase) || s.StartsWith("svn", StringComparison.OrdinalIgnoreCase);

    private static string PrepareWorkingCopy(RepositoryEndpoint ep)
        => LooksLikeRemote(ep.UrlOrPath) ? Path.Combine(Path.GetTempPath(), "RepoMigrator", "svn", Guid.NewGuid().ToString("N"))
                                         : Path.GetFullPath(ep.UrlOrPath);

    private static void ApplyCredentials(SvnClient client, RepositoryEndpoint ep)
    {
        if (!string.IsNullOrEmpty(ep.Credentials?.Username))
        {
            client.Authentication.DefaultCredentials = new System.Net.NetworkCredential(
                ep.Credentials!.Username!, ep.Credentials!.Password ?? "");
        }
    }

    private static string GetRepoRoot(string wc) => Directory.Exists(wc) ? wc : throw new DirectoryNotFoundException(wc);

    private static void SyncDirectory(string source, string dest)
    {
        foreach (var file in Directory.EnumerateFiles(dest, "*", SearchOption.AllDirectories))
        {
            if (file.Contains(Path.DirectorySeparatorChar + ".svn" + Path.DirectorySeparatorChar))
                continue;
            var rel = file.Substring(dest.Length).TrimStart(Path.DirectorySeparatorChar);
            var src = Path.Combine(source, rel);
            if (!File.Exists(src))
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }
        }

        foreach (var srcFile in Directory.EnumerateFiles(source, "*", SearchOption.AllDirectories))
        {
            var rel = srcFile.Substring(source.Length).TrimStart(Path.DirectorySeparatorChar);
            var dst = Path.Combine(dest, rel);
            Directory.CreateDirectory(Path.GetDirectoryName(dst)!);
            File.Copy(srcFile, dst, overwrite: true);
        }
    }
}
