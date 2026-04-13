using System;

namespace TraceAnalysis.Base.Models.Interfaces
{
    /// <summary>
    /// Describes the metadata of a single field within a trace data set.
    /// </summary>
    public interface ITraceFieldMetadata
    {
        /// <summary>Field name as it appears in the source.</summary>
        string sName { get; }

        /// <summary>
        /// Inferred field group derived from a shared name prefix and separator,
        /// e.g. <c>"AGV1"</c> from <c>"AGV1.X"</c>, or <c>null</c> if no group is applicable.
        /// </summary>
        string? sGroup { get; }

        /// <summary>Optional format string for display or export purposes.</summary>
        string? sFormat { get; }

        /// <summary>CLR type of the field values, or <c>null</c> if unknown.</summary>
        Type? FieldType { get; }
    }
}
