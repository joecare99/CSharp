using System.Collections.Generic;

namespace AhnWin52Backup.Core.Workflows;

/// <summary>Record counts and compatibility warnings produced by a successful restore.</summary>
public sealed record HejDatabaseRestoreResult(
    string DestinationDirectory,
    IReadOnlyDictionary<string, int> RecordCounts,
    IReadOnlyList<string> Warnings);
