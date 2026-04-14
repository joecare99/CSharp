using System;

namespace TraceAnalysis.Base.Filters
{
    /// <summary>
    /// Describes a logical input source for stream-based filter selection without
    /// forcing file-system specific behavior.
    /// </summary>
    public sealed class FilterSourceDescriptor
    {
        /// <summary>
        /// Initializes a new instance of <see cref="FilterSourceDescriptor"/>.
        /// </summary>
        /// <param name="sourceId">Stable source identifier, for example a file path or logical stream id.</param>
        /// <param name="suggestedExtension">Optional extension hint (e.g. <c>".csv"</c>).</param>
        /// <param name="contentTypeHint">Optional content type hint.</param>
        /// <param name="displayName">Optional display name for diagnostics.</param>
        /// <param name="manualFilterId">Optional manual filter override identifier.</param>
        public FilterSourceDescriptor(
            string sourceId,
            string? suggestedExtension = null,
            string? contentTypeHint = null,
            string? displayName = null,
            string? manualFilterId = null)
        {
            if (string.IsNullOrWhiteSpace(sourceId))
                throw new ArgumentException("A source identifier is required.", nameof(sourceId));

            SourceId = sourceId;
            SuggestedExtension = suggestedExtension;
            ContentTypeHint = contentTypeHint;
            DisplayName = displayName;
            ManualFilterId = manualFilterId;
        }

        /// <summary>
        /// Stable source identifier, for example a file path or logical stream id.
        /// </summary>
        public string SourceId { get; }

        /// <summary>
        /// Optional extension hint used for prefiltering candidates.
        /// </summary>
        public string? SuggestedExtension { get; }

        /// <summary>
        /// Optional content type hint.
        /// </summary>
        public string? ContentTypeHint { get; }

        /// <summary>
        /// Optional display label used in diagnostics.
        /// </summary>
        public string? DisplayName { get; }

        /// <summary>
        /// Optional manual filter override identifier.
        /// </summary>
        public string? ManualFilterId { get; }
    }
}
