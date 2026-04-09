namespace RepoMigrator.Core;

/// <summary>
/// Receives migration report messages.
/// </summary>
public interface IMigrationProgress
{
    /// <summary>
    /// Reports a migration message.
    /// </summary>
    void Report(MigrationReportSeverity severity, MigrationReportMessage message, params object?[] arrAdditional);
}
