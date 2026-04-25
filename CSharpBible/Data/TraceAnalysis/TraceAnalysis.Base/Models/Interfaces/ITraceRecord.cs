using System.Collections.Generic;

namespace TraceAnalysis.Base.Models.Interfaces;

/// <summary>
/// Represents a single canonical trace row.
/// The timestamp is the only mandatory canonical field;
/// all other field values are optional and are passed through unchanged.
/// </summary>
public interface ITraceRecord
{
    /// <summary>
    /// Mandatory canonical timestamp or row key.
    /// All other values are optional.
    /// </summary>
    object Timestamp { get; }

    /// <summary>
    /// Field values keyed by field name; <c>null</c> when a value is absent
    /// for that field in this record.
    /// </summary>
    IReadOnlyDictionary<string, object?> Values { get; }
}
