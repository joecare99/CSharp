using System.Collections.Generic;

namespace AhnWin52Backup.Core.Paradox;

/// <summary>Header-declared metadata; no records or memo content are read.</summary>
public sealed record ParadoxTableSchema(
    string Name,
    IReadOnlyList<ParadoxField> Fields,
    int DeclaredRecordCount,
    int DeclaredBlockCount,
    byte Version,
    bool Encrypted);
