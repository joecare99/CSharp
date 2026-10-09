using System;
using System.Linq;

namespace OFBCreator.Core.Models.Gedcom;

/// <summary>Represents a person-name event imported from a GEDCOM NAME structure.</summary>
public sealed class GedcomNameEvent
{
    /// <summary>Gets or sets the given name recorded for this event.</summary>
    public string GivenName { get; set; } = string.Empty;

    /// <summary>Gets or sets the surname recorded for this event.</summary>
    public string Surname { get; set; } = string.Empty;

    /// <summary>Gets or sets the GEDCOM name type, if specified.</summary>
    public string? Type { get; set; }

    /// <summary>Gets or sets the original GEDCOM event date text, if specified.</summary>
    public string? DateText { get; set; }

    /// <summary>Gets or sets the parsed event date used for chronology.</summary>
    public DateTime? Date { get; set; }

    /// <summary>Gets the full name represented by this event.</summary>
    public string FullName => string.Join(" ", new[] { GivenName, Surname }.Where(value => !string.IsNullOrWhiteSpace(value)));
}
