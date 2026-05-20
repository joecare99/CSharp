namespace RepoMigrator.Core;

/// <summary>
/// Provides a migration progress sink that ignores all messages.
/// </summary>
public sealed class NullMigrationProgress : IMigrationProgress
{
    public static NullMigrationProgress Instance { get; } = new();

    private NullMigrationProgress()
    {
    }

    /// <inheritdoc />
    public void Report(MigrationReportSeverity severity, MigrationReportMessage message, params object?[] arrAdditional)
    {
    }
}
