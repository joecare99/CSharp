using AhnWin52Backup.Core.Hej;

namespace AhnWin52Backup.Core.Abstractions;

/// <summary>Builds complete HEJ documents from AhnWin Paradox database directories.</summary>
public interface IHejDatabaseExportService
{
    /// <summary>Reads and validates all Paradox tables required for a complete HEJ backup.</summary>
    HejDocument Export(string databaseDirectory);
}
