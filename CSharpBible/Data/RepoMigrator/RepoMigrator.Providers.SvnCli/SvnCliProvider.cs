// RepoMigrator.Providers.SvnCli/SvnCliProvider.cs
using RepoMigrator.Core;
using RepoMigrator.Core.Abstractions;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Xml.Linq;

namespace RepoMigrator.Providers.SvnCli;

public sealed class SvnCliProvider : IVersionControlProvider
{
    private RepositoryEndpoint? _endpoint;
    private string? _wcPath;

    public string Name => "Subversion (CLI)";
    public bool SupportsRead => true;
    public bool SupportsWrite => true;

    public Task OpenAsync(RepositoryEndpoint endpoint, CancellationToken ct)
    {
        _endpoint = endpoint;
        // Für reine Lese-Operationen genügt die URL; für Ziel-Operationen setzen wir _wcPath in InitializeTargetAsync.
        if (!IsRemote(endpoint.UrlOrPath))
        {
            // Falls lokale Working Copy angegeben ist (Quelle), merken wir sie.
            _wcPath = Path.GetFullPath(endpoint.UrlOrPath);
        }
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<ChangeSetInfo>> GetChangeSetsAsync(ChangeSetQuery query, CancellationToken ct)
    {
        var repoUrl = await GetRepositoryUrlAsync(ct);
        var xml = await RunSvnAsync($"log -r 0:HEAD --xml \"{repoUrl}\"", workingDir: null, ct);

        var doc = XDocument.Parse(xml);
        var entries = doc.Root?.Elements("logentry") ?? Enumerable.Empty<XElement>();

        var list = new List<ChangeSetInfo>();
        foreach (var e in entries)
        {
            var rev = e.Attribute("revision")?.Value ?? "0";
            var author = e.Element("author")?.Value ?? "unknown";
            var dateStr = e.Element("date")?.Value ?? "";
            var msg = e.Element("msg")?.Value ?? "";

            if (!DateTimeOffset.TryParse(dateStr, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var when))
                when = DateTimeOffset.UtcNow;

            list.Add(new ChangeSetInfo
            {
                Id = rev,
                Message = msg,
                AuthorName = string.IsNullOrWhiteSpace(author) ? "unknown" : author,
                AuthorEmail = null,
                Timestamp = when
            });
        }

        // Älteste zuerst
        list.Sort((a, b) => int.Parse(a.Id, CultureInfo.InvariantCulture).CompareTo(int.Parse(b.Id, CultureInfo.InvariantCulture)));

        if (!string.IsNullOrEmpty(query.FromExclusiveId))
            list = list.SkipWhile(c => c.Id != query.FromExclusiveId).Skip(1).ToList();

        if (!string.IsNullOrEmpty(query.ToInclusiveId))
            list = list.TakeWhile(c => c.Id != query.ToInclusiveId)
                       .Concat(list.Where(c => c.Id == query.ToInclusiveId).Take(1))
                       .ToList();

        if (query.MaxCount is int max)
            list = list.Take(max).ToList();
        if (!query.OldestFirst)
            list.Reverse();

        return list.AsReadOnly();
    }

    public async Task MaterializeSnapshotAsync(string workDir, string changeSetId, CancellationToken ct)
    {
        Directory.CreateDirectory(workDir);
        // Export direkt aus der Repo-URL (kein Checkout nötig)
        var repoUrl = await GetRepositoryUrlAsync(ct);
        await RunSvnAsync($"export -r {changeSetId} \"{repoUrl}\" \"{workDir}\" --force", workingDir: null, ct);
    }

    public async Task InitializeTargetAsync(string targetPath, bool emptyInit, CancellationToken ct)
    {
        if (_endpoint is null)
            throw new InvalidOperationException("Endpoint nicht gesetzt. OpenAsync zuerst aufrufen.");

        Directory.CreateDirectory(targetPath);
        _wcPath = Path.GetFullPath(targetPath);

        if (IsRemote(_endpoint.UrlOrPath))
        {
            // Falls targetPath noch keine WC ist: checkout
            if (!await IsWorkingCopyAsync(_wcPath, ct))
            {
                await RunSvnAsync($"checkout \"{_endpoint.UrlOrPath}\" \"{_wcPath}\"", workingDir: null, ct);
            }
        }
        else
        {
            // Lokale Working Copy als Ziel: sicherstellen, dass es eine WC ist
            if (!await IsWorkingCopyAsync(_wcPath, ct))
                throw new InvalidOperationException($"'{_wcPath}' ist keine SVN-Working-Copy.");
        }
    }

    public async Task CommitSnapshotAsync(string workDir, CommitMetadata metadata, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(_wcPath))
            throw new InvalidOperationException("Working Copy Pfad nicht initialisiert.");

        // Inhalte synchronisieren
        SyncDirectory(workDir, _wcPath);

        // Unversionierte hinzufügen (rekursiv)
        await RunSvnAsync("add --force --parents .", workingDir: _wcPath, ct);

        // Fehlende löschen (Status analysieren)
        var statusOutput = await RunSvnAsync("status", workingDir: _wcPath, ct);
        var toDelete = ParseMissingFromStatus(statusOutput)
            .Select(rel => Path.Combine(_wcPath, rel))
            .ToList();

        if (toDelete.Count > 0)
        {
            // Pfade quoten
            var args = new StringBuilder("delete --force");
            foreach (var p in toDelete)
                args.Append(' ').Append('"').Append(p).Append('"');
            await RunSvnAsync(args.ToString(), workingDir: null, ct);
        }

        // Commit
        var messageFile = Path.Combine(Path.GetTempPath(), $"svnmsg_{Guid.NewGuid():N}.txt");
        await File.WriteAllTextAsync(messageFile, metadata.Message ?? string.Empty, ct);
        try
        {
            var commitArgs = $"commit \"{_wcPath}\" --file \"{messageFile}\"";
            var commitOutput = await RunSvnAsync(commitArgs, workingDir: null, ct);

            // Versuch: Revision aus Output extrahieren, z. B. "Committed revision 123."
            var rev = ExtractCommittedRevision(commitOutput);

            // Optional: revprops setzen (falls Server erlaubt)
            if (rev is not null)
            {
                var repoUrl = await GetRepositoryUrlAsync(ct);
                var author = string.IsNullOrWhiteSpace(metadata.AuthorName) ? "unknown" : metadata.AuthorName;
                var date = metadata.Timestamp.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ", CultureInfo.InvariantCulture);

                // Autor setzen (ignorieren, wenn Server es nicht erlaubt)
                await TryRunSvnAsync($"propset --revprop -r {rev} svn:author \"{EscapeProp(author)}\" \"{repoUrl}\"", workingDir: null, ct);

                // Datum setzen
                await TryRunSvnAsync($"propset --revprop -r {rev} svn:date \"{date}\" \"{repoUrl}\"", workingDir: null, ct);
            }
        }
        finally
        {
            TryDeleteFile(messageFile);
        }
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    // -------- Helpers --------

    private async Task<string> GetRepositoryUrlAsync(CancellationToken ct)
    {
        if (_endpoint is null)
            throw new InvalidOperationException("Endpoint nicht gesetzt.");

        if (IsRemote(_endpoint.UrlOrPath))
            return _endpoint.UrlOrPath;

        // Lokale WC: URL aus 'svn info' ermitteln
        var info = await RunSvnAsync("info --show-item url", workingDir: _wcPath ?? _endpoint.UrlOrPath, ct);
        var url = info.Trim();
        if (string.IsNullOrEmpty(url))
            throw new InvalidOperationException("Konnte Repo-URL über 'svn info' nicht ermitteln.");
        return url;
    }

    private static bool IsRemote(string pathOrUrl)
    {
        var s = pathOrUrl.Trim().ToLowerInvariant();
        return s.StartsWith("http://") || s.StartsWith("https://") || s.StartsWith("svn://") || s.StartsWith("svn+ssh://") || s.StartsWith("file://");
    }

    private async Task<bool> IsWorkingCopyAsync(string path, CancellationToken ct)
    {
        try
        {
            var output = await RunSvnAsync("info --show-item wc-root", workingDir: path, ct);
            return !string.IsNullOrWhiteSpace(output);
        }
        catch
        {
            return false;
        }
    }

    private async Task<string> RunSvnAsync(string args, string? workingDir, CancellationToken ct)
    {
        // Credentials anhängen, falls vorhanden
        if (_endpoint?.Credentials is { Username: not null } cred)
        {
            args += $" --username \"{cred.Username}\" --password \"{cred.Password ?? ""}\" --non-interactive --trust-server-cert";
        }

        var psi = new ProcessStartInfo
        {
            FileName = "svn",
            Arguments = args,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };
        if (!string.IsNullOrEmpty(workingDir))
            psi.WorkingDirectory = workingDir;

        using var proc = new Process { StartInfo = psi, EnableRaisingEvents = true };
        var stdout = new StringBuilder();
        var stderr = new StringBuilder();

        proc.OutputDataReceived += (_, e) => { if (e.Data is not null) stdout.AppendLine(e.Data); };
        proc.ErrorDataReceived += (_, e) => { if (e.Data is not null) stderr.AppendLine(e.Data); };

        if (!proc.Start())
            throw new InvalidOperationException("Konnte 'svn' Prozess nicht starten.");

        proc.BeginOutputReadLine();
        proc.BeginErrorReadLine();

        await proc.WaitForExitAsync(ct);

        if (proc.ExitCode != 0)
        {
            var msg = $"svn {args}\nExitCode: {proc.ExitCode}\n{stderr}";
            throw new InvalidOperationException(msg);
        }

        return stdout.ToString();
    }

    private async Task TryRunSvnAsync(string args, string? workingDir, CancellationToken ct)
    {
        try
        { await RunSvnAsync(args, workingDir, ct); }
        catch { /* revprop setting may be disallowed; ignore */ }
    }

    private static IEnumerable<string> ParseMissingFromStatus(string statusOutput)
    {
        // '!' in erster Spalte => missing (Datei/Ordner entfernt)
        // Beispielzeile: "!       src/old.cs"
        using var reader = new StringReader(statusOutput);
        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            if (line.Length < 2)
                continue;
            var code = line[0];
            if (code == '!' || code == '~')
            {
                var rel = line.Length > 8 ? line[8..].Trim() : null;
                if (!string.IsNullOrWhiteSpace(rel))
                    yield return rel!;
            }
        }
    }

    private static void SyncDirectory(string source, string dest)
    {
        Directory.CreateDirectory(dest);

        // Löschen von Dateien/Ordnern, die es im Source nicht mehr gibt (ausgenommen .svn)
        var srcFiles = Directory.EnumerateFiles(source, "*", SearchOption.AllDirectories)
            .Select(p => p[(source.Length)..].TrimStart(Path.DirectorySeparatorChar))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var file in Directory.EnumerateFiles(dest, "*", SearchOption.AllDirectories))
        {
            if (file.Contains($"{Path.DirectorySeparatorChar}.svn{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
                continue;
            var rel = file[(dest.Length)..].TrimStart(Path.DirectorySeparatorChar);
            if (!srcFiles.Contains(rel))
            {
                TryMakeWritable(file);
                File.Delete(file);
            }
        }

        // Leere Verzeichnisse ggf. entfernen (ohne .svn)
        foreach (var dir in Directory.EnumerateDirectories(dest, "*", SearchOption.AllDirectories).OrderByDescending(d => d.Length))
        {
            if (dir.EndsWith($"{Path.DirectorySeparatorChar}.svn", StringComparison.OrdinalIgnoreCase))
                continue;
            if (!Directory.EnumerateFileSystemEntries(dir).Any())
                Directory.Delete(dir, false);
        }

        // Kopieren/Aktualisieren
        foreach (var src in Directory.EnumerateFiles(source, "*", SearchOption.AllDirectories))
        {
            var rel = src[(source.Length)..].TrimStart(Path.DirectorySeparatorChar);
            var dst = Path.Combine(dest, rel);
            Directory.CreateDirectory(Path.GetDirectoryName(dst)!);
            File.Copy(src, dst, true);
        }
    }

    private static void TryMakeWritable(string path)
    {
        try
        {
            var attr = File.GetAttributes(path);
            if (attr.HasFlag(FileAttributes.ReadOnly))
                File.SetAttributes(path, attr & ~FileAttributes.ReadOnly);
        }
        catch { /* ignore */ }
    }

    private static string? ExtractCommittedRevision(string commitOutput)
    {
        // gängige Ausgaben:
        // "Committed revision 123."
        // "Übertragen wurden Revision 123." (lokalisierte Clients)
        // Wir suchen die letzte Zahl im Text.
        var digits = new string(commitOutput.Where(char.IsDigit).ToArray());
        if (int.TryParse(digits, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n))
            return n.ToString(CultureInfo.InvariantCulture);
        return null;
    }

    private static string EscapeProp(string s) => s.Replace("\"", "\\\"");
    private static void TryDeleteFile(string path) { try { if (File.Exists(path)) File.Delete(path); } catch { } }
}
