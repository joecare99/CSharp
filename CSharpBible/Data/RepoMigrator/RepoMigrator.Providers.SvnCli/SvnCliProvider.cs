// RepoMigrator.Providers.SvnCli/SvnCliProvider.cs
using RepoMigrator.Core;
using RepoMigrator.Core.Abstractions;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
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

    public Task<RepositoryCapabilities> GetCapabilitiesAsync(RepositoryEndpoint endpoint, CancellationToken ct)
        => Task.FromResult(new RepositoryCapabilities
        {
            SupportsRevisionSelection = true
        });

    public async Task<RepositorySelectionData> GetSelectionDataAsync(RepositoryEndpoint endpoint, CancellationToken ct)
    {
        _endpoint = endpoint;
        _wcPath = IsRemote(endpoint.UrlOrPath) ? null : Path.GetFullPath(endpoint.UrlOrPath);

        var sPathUrl = await GetRepositoryUrlAsync(ct);
        var lstRevisions = await LoadRevisionInfosAsync(sPathUrl, ct);
        return new RepositorySelectionData
        {
            SuggestedFromRevisionId = SvnRevisionRangeResolver.GetSuggestedFromRevisionId(lstRevisions),
            SuggestedToRevisionId = null,
            Revisions = lstRevisions
        };
    }

    public async Task<RepositoryProbeResult> ProbeAsync(RepositoryEndpoint endpoint, RepositoryAccessMode accessMode, CancellationToken ct)
    {
        try
        {
            _endpoint = endpoint;
            _wcPath = IsRemote(endpoint.UrlOrPath) ? null : Path.GetFullPath(endpoint.UrlOrPath);

            var target = IsRemote(endpoint.UrlOrPath) ? ResolveRemoteEndpointPath(endpoint) : _wcPath!;
            var infoXml = await RunSvnAsync($"info --xml \"{target}\"", workingDir: null, ct);
            var doc = XDocument.Parse(infoXml);
            var entry = doc.Root?.Element("entry");
            var url = entry?.Element("url")?.Value;
            var root = entry?.Element("repository")?.Element("root")?.Value;
            var revision = entry?.Attribute("revision")?.Value;

            var details = new List<string>();
            if (!string.IsNullOrWhiteSpace(url))
                details.Add($"URL: {url}");
            if (!string.IsNullOrWhiteSpace(root))
                details.Add($"Repository Root: {root}");
            if (!string.IsNullOrWhiteSpace(revision))
                details.Add($"Revision: {revision}");
            if (accessMode == RepositoryAccessMode.Write)
                details.Add("Hinweis: Der Test bestätigt Erreichbarkeit. Schreibrechte werden endgültig erst beim Commit geprüft.");

            return new RepositoryProbeResult
            {
                Success = true,
                Summary = "SVN-Repository ist erreichbar.",
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

    public Task TransferAsync(RepositoryEndpoint source, RepositoryEndpoint target, MigrationOptions options, IMigrationProgress progress, CancellationToken ct)
        => throw new NotSupportedException("Native history transfer is not supported for Subversion endpoints.");

    public async Task<IReadOnlyList<ChangeSetInfo>> GetChangeSetsAsync(ChangeSetQuery query, CancellationToken ct)
    {
        var sPathUrl = await GetRepositoryUrlAsync(ct);
        var list = (await LoadRevisionInfosAsync(sPathUrl, ct))
            .Select(svnRevision => new ChangeSetInfo
            {
                Id = svnRevision.Id,
                Message = svnRevision.Message,
                AuthorName = svnRevision.AuthorName,
                AuthorEmail = null,
                Timestamp = svnRevision.Timestamp
            })
            .ToList();

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

    public async Task InitializeTargetAsync(RepositoryEndpoint endpoint, bool emptyInit, CancellationToken ct)
    {
        _endpoint = endpoint;

        _wcPath = IsRemote(endpoint.UrlOrPath)
            ? Path.Combine(Path.GetTempPath(), "RepoMigrator", "svn-target", Guid.NewGuid().ToString("N"))
            : Path.GetFullPath(endpoint.UrlOrPath);

        Directory.CreateDirectory(_wcPath);

        if (IsRemote(endpoint.UrlOrPath))
        {
            // Falls targetPath noch keine WC ist: checkout
            if (!await IsWorkingCopyAsync(_wcPath, ct))
            {
                await RunSvnAsync($"checkout \"{ResolveRemoteEndpointPath(endpoint)}\" \"{_wcPath}\"", workingDir: null, ct);
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
            .ToList();

        if (toDelete.Count > 0)
        {
            var targetsFile = Path.Combine(Path.GetTempPath(), $"svndelete_{Guid.NewGuid():N}.txt");
            await File.WriteAllLinesAsync(targetsFile, toDelete, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false), ct);
            try
            {
                await RunSvnAsync($"delete --force --targets \"{targetsFile}\"", workingDir: _wcPath, ct);
            }
            finally
            {
                TryDeleteFile(targetsFile);
            }
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

            await RunSvnAsync("update", workingDir: _wcPath, ct);
        }
        finally
        {
            TryDeleteFile(messageFile);
        }
    }

    public Task FlushAsync(CancellationToken ct) => Task.CompletedTask;

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    // -------- Helpers --------

    private async Task<string> GetRepositoryUrlAsync(CancellationToken ct)
    {
        if (_endpoint is null)
            throw new InvalidOperationException("Endpoint nicht gesetzt.");

        if (IsRemote(_endpoint.UrlOrPath))
            return ResolveRemoteEndpointPath(_endpoint);

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

    private static string ResolveRemoteEndpointPath(RepositoryEndpoint endpoint)
    {
        var sBaseUrl = endpoint.UrlOrPath.Trim();
        if (string.IsNullOrWhiteSpace(endpoint.BranchOrTrunk))
            return sBaseUrl;

        var sBranchOrTrunk = endpoint.BranchOrTrunk
            .Replace('\\', '/')
            .Trim();

        if (string.IsNullOrWhiteSpace(sBranchOrTrunk))
            return sBaseUrl;

        return $"{sBaseUrl.TrimEnd('/')}/{sBranchOrTrunk.TrimStart('/')}";
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

    private async Task<IReadOnlyList<RepositoryRevisionInfo>> LoadRevisionInfosAsync(string sPathUrl, CancellationToken ct)
    {
        var sXml = await RunSvnAsync($"log -r 0:HEAD --xml \"{sPathUrl}\"", workingDir: null, ct);
        var xDoc = XDocument.Parse(sXml);
        var lstEntries = xDoc.Root?.Elements("logentry") ?? Enumerable.Empty<XElement>();
        var lstRevisions = new List<RepositoryRevisionInfo>();

        foreach (var xEntry in lstEntries)
        {
            var sRevision = xEntry.Attribute("revision")?.Value ?? "0";
            var sAuthor = xEntry.Element("author")?.Value ?? "unknown";
            var sDate = xEntry.Element("date")?.Value ?? "";
            var sMessage = xEntry.Element("msg")?.Value ?? "";

            if (!DateTimeOffset.TryParse(sDate, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dtWhen))
                dtWhen = DateTimeOffset.UtcNow;

            lstRevisions.Add(new RepositoryRevisionInfo
            {
                Id = sRevision,
                Message = sMessage,
                AuthorName = string.IsNullOrWhiteSpace(sAuthor) ? "unknown" : sAuthor,
                Timestamp = dtWhen
            });
        }

        lstRevisions.Sort((svnLeft, svnRight) => int.Parse(svnLeft.Id, CultureInfo.InvariantCulture).CompareTo(int.Parse(svnRight.Id, CultureInfo.InvariantCulture)));
        return lstRevisions;
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
            .Where(src => !IsSvnAdministrativePath(source, src))
            .Select(p => p[(source.Length)..].TrimStart(Path.DirectorySeparatorChar))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var file in Directory.EnumerateFiles(dest, "*", SearchOption.AllDirectories))
        {
            if (IsSvnAdministrativePath(dest, file))
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
            if (IsSvnAdministrativePath(dest, dir))
                continue;
            if (!Directory.EnumerateFileSystemEntries(dir).Any())
                Directory.Delete(dir, false);
        }

        // Kopieren/Aktualisieren
        foreach (var src in Directory.EnumerateFiles(source, "*", SearchOption.AllDirectories))
        {
            if (IsSvnAdministrativePath(source, src))
                continue;

            var rel = src[(source.Length)..].TrimStart(Path.DirectorySeparatorChar);
            var dst = Path.Combine(dest, rel);
            Directory.CreateDirectory(Path.GetDirectoryName(dst)!);
            File.Copy(src, dst, true);
        }
    }

    private static bool IsSvnAdministrativePath(string sRootPath, string sCandidatePath)
    {
        var sRelativePath = Path.GetRelativePath(sRootPath, sCandidatePath);
        if (string.IsNullOrWhiteSpace(sRelativePath) || string.Equals(sRelativePath, ".", StringComparison.Ordinal))
            return false;

        var arrSegments = sRelativePath
            .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
            .Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);

        return arrSegments.Any(sSegment => string.Equals(sSegment, ".svn", StringComparison.OrdinalIgnoreCase));
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
        // Wir suchen die Revisionsnummer gezielt in der Abschlusszeile statt alle Ziffern aus dem gesamten Output zu sammeln.
        var arrLines = commitOutput.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        for (var iLineIndex = arrLines.Length - 1; iLineIndex >= 0; iLineIndex--)
        {
            var sLine = arrLines[iLineIndex];
            var match = Regex.Match(sLine, @"\brevision\s+(?<revision>\d+)\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            if (match.Success)
                return match.Groups["revision"].Value;
        }

        return null;
    }

    private static string EscapeProp(string s) => s.Replace("\"", "\\\"");
    private static void TryDeleteFile(string path) { try { if (File.Exists(path)) File.Delete(path); } catch { } }
}
