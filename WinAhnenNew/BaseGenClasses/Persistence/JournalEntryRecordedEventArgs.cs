using System;
using GenInterfaces.Interfaces;

namespace BaseGenClasses.Persistence;

/// <summary>
/// Provides details about a recorded genealogy journal entry.
/// </summary>
public sealed class JournalEntryRecordedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JournalEntryRecordedEventArgs"/> class.
    /// </summary>
    /// <param name="genTransaction">The recorded journal transaction.</param>
    public JournalEntryRecordedEventArgs(IGenTransaction genTransaction)
    {
        GenTransaction = genTransaction ?? throw new ArgumentNullException(nameof(genTransaction));
    }

    /// <summary>
    /// Gets the recorded journal transaction.
    /// </summary>
    public IGenTransaction GenTransaction { get; }
}
