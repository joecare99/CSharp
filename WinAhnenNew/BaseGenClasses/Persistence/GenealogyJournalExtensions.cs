using BaseGenClasses.Persistence.Interfaces;
using GenInterfaces.Interfaces.Genealogic;

namespace BaseGenClasses.Persistence;

/// <summary>
/// Provides helper methods for recording typed genealogy journal entries.
/// </summary>
public static class GenealogyJournalExtensions
{
    /// <summary>
    /// Records a journal entry for a source change.
    /// </summary>
    /// <param name="journalContext">The genealogy journal context.</param>
    /// <param name="genSource">The changed source.</param>
    /// <param name="srcNewValue">The new source snapshot.</param>
    /// <param name="srcOldValue">The old source snapshot.</param>
    public static void RecordSourceChange(
        this IGenealogyJournalContext journalContext,
        IGenSource genSource,
        SourceJournalValue? srcNewValue,
        SourceJournalValue? srcOldValue)
    {
        journalContext.RecordJournalEntry(genSource, genSource, srcNewValue, srcOldValue);
    }

    /// <summary>
    /// Records a journal entry for a media change.
    /// </summary>
    /// <param name="journalContext">The genealogy journal context.</param>
    /// <param name="genMedia">The changed media item.</param>
    /// <param name="medNewValue">The new media snapshot.</param>
    /// <param name="medOldValue">The old media snapshot.</param>
    public static void RecordMediaChange(
        this IGenealogyJournalContext journalContext,
        IGenMedia genMedia,
        MediaJournalValue? medNewValue,
        MediaJournalValue? medOldValue)
    {
        journalContext.RecordJournalEntry(genMedia, genMedia, medNewValue, medOldValue);
    }

    /// <summary>
    /// Records a journal entry for a repository change.
    /// </summary>
    /// <param name="journalContext">The genealogy journal context.</param>
    /// <param name="genRepository">The changed repository.</param>
    /// <param name="repNewValue">The new repository snapshot.</param>
    /// <param name="repOldValue">The old repository snapshot.</param>
    public static void RecordRepositoryChange(
        this IGenealogyJournalContext journalContext,
        IGenRepository genRepository,
        RepositoryJournalValue? repNewValue,
        RepositoryJournalValue? repOldValue)
    {
        journalContext.RecordJournalEntry(genRepository, genRepository, repNewValue, repOldValue);
    }
}
