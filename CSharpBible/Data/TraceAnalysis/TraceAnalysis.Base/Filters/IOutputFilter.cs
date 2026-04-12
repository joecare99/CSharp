using System.IO;
using TraceAnalysis.Base.Models.Interfaces;

namespace TraceAnalysis.Base.Filters
{
    /// <summary>
    /// Contract for pluggable output filters that serialize the canonical
    /// <see cref="ITraceDataSet"/> structure to a target stream.
    /// </summary>
    public interface IOutputFilter
    {
        /// <summary>
        /// Writes the canonical trace data set to the target stream.
        /// </summary>
        /// <param name="_dataSet">The canonical data set to export.</param>
        /// <param name="_stream">The target stream to write to.</param>
        void Write(ITraceDataSet _dataSet, Stream _stream);
    }
}
