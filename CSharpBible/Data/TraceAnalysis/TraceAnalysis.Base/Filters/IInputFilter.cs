using System.IO;
using TraceAnalysis.Base.Models.Interfaces;

namespace TraceAnalysis.Base.Filters
{
    /// <summary>
    /// Contract for pluggable input filters that convert a source stream
    /// into the canonical <see cref="ITraceDataSet"/> structure.
    /// </summary>
    public interface IInputFilter
    {
        /// <summary>
        /// Determines whether this filter can handle the given stream.
        /// Implementations should inspect the stream without permanently consuming its content.
        /// </summary>
        /// <param name="_stream">The source stream to inspect.</param>
        /// <param name="_sourceId">An identifier for the source, e.g. a file path or label.</param>
        /// <returns><c>true</c> if this filter can process the stream; otherwise <c>false</c>.</returns>
        bool CanHandle(Stream _stream, string _sourceId);

        /// <summary>
        /// Reads the source stream and returns the canonical trace data set.
        /// Parse errors are collected in <see cref="ITraceDataSet.ParseErrors"/> rather than thrown.
        /// </summary>
        /// <param name="_stream">The source stream to read from.</param>
        /// <param name="_sourceId">An identifier for the source, e.g. a file path or label.</param>
        /// <returns>
        /// A canonical <see cref="ITraceDataSet"/> with all imported records and any parse errors.
        /// </returns>
        ITraceDataSet Read(Stream _stream, string _sourceId);
    }
}
