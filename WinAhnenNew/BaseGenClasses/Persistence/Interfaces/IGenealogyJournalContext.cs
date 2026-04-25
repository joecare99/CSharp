using System;
using System.Collections.Generic;
using GenInterfaces.Interfaces;
using GenInterfaces.Interfaces.Genealogic;

namespace BaseGenClasses.Persistence.Interfaces;

/// <summary>
/// Exposes recorded journal entries for a genealogy root context.
/// </summary>
public interface IGenealogyJournalContext
{
    /// <summary>
    /// Occurs when a new journal entry was recorded.
    /// </summary>
    event EventHandler<JournalEntryRecordedEventArgs>? JournalEntryRecorded;

    /// <summary>
    /// Gets the recorded journal entries.
    /// </summary>
    IReadOnlyList<IGenTransaction> JournalEntries { get; }

    /// <summary>
    /// Records a journal entry for a genealogy change.
    /// </summary>
    /// <param name="genClass">The changed aggregate or class.</param>
    /// <param name="genEntry">The changed entry.</param>
    /// <param name="objData">The new value snapshot.</param>
    /// <param name="objOldData">The old value snapshot.</param>
    /// <returns>The recorded transaction.</returns>
    IGenTransaction RecordJournalEntry(IGenBase genClass, IGenBase genEntry, object? objData, object? objOldData);
}
