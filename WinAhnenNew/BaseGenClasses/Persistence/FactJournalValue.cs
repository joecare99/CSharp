using System;
using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;

namespace BaseGenClasses.Persistence;

/// <summary>
/// Represents a serializable fact snapshot for genealogy journal entries.
/// </summary>
public sealed class FactJournalValue
{
    /// <summary>
    /// Gets or sets the fact type.
    /// </summary>
    public EFactType eFactType { get; init; }

    /// <summary>
    /// Gets or sets the fact data text.
    /// </summary>
    public string? Data { get; init; }

    /// <summary>
    /// Gets or sets the primary event date.
    /// </summary>
    public DateTime? Date1 { get; init; }

    /// <summary>
    /// Gets or sets the date modifier.
    /// </summary>
    public EDateModifier? eDateModifier { get; init; }

    /// <summary>
    /// Gets or sets the display date text.
    /// </summary>
    public string? DateText { get; init; }

    /// <summary>
    /// Gets or sets the place name.
    /// </summary>
    public string? PlaceName { get; init; }

    /// <summary>
    /// Creates a journal snapshot from a genealogy fact.
    /// </summary>
    /// <param name="genFact">The fact to snapshot.</param>
    /// <returns>The snapshot value, or <see langword="null"/> when no fact is available.</returns>
    public static FactJournalValue? FromFact(IGenFact? genFact)
    {
        if (genFact is null)
        {
            return null;
        }

        return new FactJournalValue
        {
            eFactType = genFact.eFactType,
            Data = genFact.Data,
            Date1 = genFact.Date?.Date1 == DateTime.MinValue ? null : genFact.Date?.Date1,
            eDateModifier = genFact.Date?.eDateModifier,
            DateText = genFact.Date?.DateText,
            PlaceName = genFact.Place?.Name
        };
    }
}
