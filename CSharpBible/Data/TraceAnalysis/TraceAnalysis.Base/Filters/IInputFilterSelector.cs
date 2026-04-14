using System.Collections.Generic;
using System.IO;

namespace TraceAnalysis.Base.Filters
{
    /// <summary>
    /// Selects the most suitable <see cref="IAnalyzableInputFilter"/> for a given source
    /// using deterministic ranking rules.
    /// </summary>
    public interface IInputFilterSelector
    {
        /// <summary>
        /// Selects the best matching filter and returns full analysis details.
        /// </summary>
        /// <param name="filters">Registered analyzable input filters.</param>
        /// <param name="stream">Source stream used for inspection.</param>
        /// <param name="sourceDescriptor">Logical source descriptor and hints.</param>
        /// <returns>Selection result including all analysis outputs.</returns>
        InputFilterSelectionResult Select(
            IEnumerable<IAnalyzableInputFilter> filters,
            Stream stream,
            FilterSourceDescriptor sourceDescriptor);
    }
}
