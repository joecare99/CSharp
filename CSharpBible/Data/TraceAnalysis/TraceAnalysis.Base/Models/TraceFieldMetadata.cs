using System;
using TraceAnalysis.Base.Models.Interfaces;

namespace TraceAnalysis.Base.Models
{
    /// <summary>
    /// Default implementation of <see cref="ITraceFieldMetadata"/>.
    /// </summary>
    public class TraceFieldMetadata : ITraceFieldMetadata
    {
        /// <inheritdoc/>
        public string sName { get; }

        /// <inheritdoc/>
        public string? sGroup { get; }

        /// <inheritdoc/>
        public string? sFormat { get; }

        /// <inheritdoc/>
        public Type? FieldType { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="TraceFieldMetadata"/>.
        /// The field group is inferred from the field name when not provided explicitly.
        /// </summary>
        /// <param name="_name">Field name as it appears in the source.</param>
        /// <param name="_fieldType">CLR type of the field values, or <c>null</c> if unknown.</param>
        /// <param name="_group">
        /// Optional field group; when <c>null</c> the group is inferred from the field name
        /// using <c>'.'</c> or <c>'_'</c> as structural separators.
        /// </param>
        /// <param name="_format">Optional format string for display or export.</param>
        public TraceFieldMetadata(string _name, Type? _fieldType = null, string? _group = null, string? _format = null)
        {
            sName = _name;
            FieldType = _fieldType;
            sGroup = _group ?? InferGroup(_name);
            sFormat = _format;
        }

        /// <summary>
        /// Infers a field group from the field name using <c>'.'</c> or <c>'_'</c> as structural
        /// separators (e.g. <c>"AGV1"</c> from <c>"AGV1.X"</c>).
        /// Returns <c>null</c> if no separator with a non-empty prefix is found.
        /// </summary>
        private static string? InferGroup(string _name)
        {
            var dotIndex = _name.IndexOf('.');
            if (dotIndex > 0)
                return _name.Substring(0, dotIndex);

            var underscoreIndex = _name.IndexOf('_');
            if (underscoreIndex > 0)
                return _name.Substring(0, underscoreIndex);

            return null;
        }
    }
}
