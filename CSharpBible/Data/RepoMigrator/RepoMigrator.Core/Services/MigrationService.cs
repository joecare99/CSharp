// RepoMigrator.Core/Services/MigrationService.cs
using RepoMigrator.Core.Abstractions;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RepoMigrator.Core;

public interface IMigrationService
{
    Task MigrateAsync(
        RepositoryEndpoint source,
        RepositoryEndpoint target,
        ChangeSetQuery query,
        IMigrationProgress progress,
        CancellationToken ct);
}

public interface IMigrationProgress
{
    void Report(string message);
    void ReportStep(string changeSetId, int index, int total);
}

public sealed class MigrationService : IMigrationService
{
    private readonly IProviderFactory _factory;

    public MigrationService(IProviderFactory factory) => _factory = factory;

    public async Task MigrateAsync(
        RepositoryEndpoint source,
        RepositoryEndpoint target,
        ChangeSetQuery query,
        IMigrationProgress progress,
        CancellationToken ct)
    {
        await using var src = _factory.Create(source.Type);
        await using var dst = _factory.Create(target.Type);

        progress.Report($"Öffne Quelle ({src.Name}) …");
        await src.OpenAsync(source, ct);

        progress.Report($"Lese Changesets …");
        var changes = await src.GetChangeSetsAsync(query, ct);
        if (changes.Count == 0)
        {
            progress.Report("Keine Changesets gefunden.");
            return;
        }

        progress.Report($"Initialisiere Ziel ({dst.Name}) …");
        await dst.OpenAsync(target, ct);
        await dst.InitializeTargetAsync(target.UrlOrPath, emptyInit: true, ct);

        var total = changes.Count;
        for (int i = 0; i < total; i++)
        {
            ct.ThrowIfCancellationRequested();
            var c = changes[i];
            progress.ReportStep(c.Id, i + 1, total);

            // 1) Snapshot aus Quelle exportieren
            var tempDir = Path.Combine(Path.GetTempPath(), "RepoMigrator", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            try
            {
                await src.MaterializeSnapshotAsync(tempDir, c.Id, ct);

                // 2) Snapshot in Ziel committen
                var meta = new CommitMetadata
                {
                    Message = c.Message,
                    AuthorName = c.AuthorName,
                    AuthorEmail = c.AuthorEmail,
                    Timestamp = c.Timestamp
                };
                await dst.CommitSnapshotAsync(tempDir, meta, ct);

                progress.Report($"Commit {i + 1}/{total} übertragen: {Short(c.Id)}");
            }
            finally
            {
                try
                { Directory.Delete(tempDir, recursive: true); }
                catch { /* ignore */ }
            }
        }

        progress.Report("Migration abgeschlossen.");
    }

    private static string Short(string id) => id.Length > 8 ? id.Substring(0, 8) : id;
}
