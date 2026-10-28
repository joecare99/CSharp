using AhnWin52Backup.Core.Paradox;

namespace AhnWin52Backup.Core.Abstractions;

/// <summary>Reads Paradox database tables without opening them for modification.</summary>
public interface IParadoxTableReader
{
    /// <summary>Reads table metadata and all active records from a Paradox .DB file.</summary>
    ParadoxTable Read(string databaseFilePath);
}
