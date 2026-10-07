using AhnWin52Backup.Core.Workflows;

namespace AhnWin52Backup.Core.Abstractions;

/// <summary>Restores a validated HEJ document into a Paradox database directory.</summary>
public interface IHejDatabaseRestoreService
{
    /// <summary>Stages, validates, and publishes a restore without changing the HEJ input.</summary>
    HejDatabaseRestoreResult Restore(
        string hejFilePath,
        string destinationDirectory,
        HejDatabaseRestoreMode mode,
        bool force);
}
