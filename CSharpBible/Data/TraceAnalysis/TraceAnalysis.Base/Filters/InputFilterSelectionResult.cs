using System.Collections.Generic;
using System.Linq;

namespace TraceAnalysis.Base.Filters
{
    /// <summary>
    /// Contains the selected input filter and the corresponding analysis details.
    /// </summary>
    public sealed class InputFilterSelectionResult
    {
        /// <summary>
        /// Initializes a new instance of <see cref="InputFilterSelectionResult"/>.
        /// </summary>
        /// <param name="selectedFilter">Selected filter or <c>null</c> if no filter matches.</param>
        /// <param name="analyses">All filter analyses produced during selection.</param>
        public InputFilterSelectionResult(
            IAnalyzableInputFilter? selectedFilter,
            IEnumerable<InputFilterAnalysisResult>? analyses)
        {
            SelectedFilter = selectedFilter;
            Analyses = (analyses ?? Enumerable.Empty<InputFilterAnalysisResult>()).ToArray();
        }

        /// <summary>
        /// Selected filter or <c>null</c> if no filter matches.
        /// </summary>
        public IAnalyzableInputFilter? SelectedFilter { get; }

        /// <summary>
        /// All filter analyses produced during selection.
        /// </summary>
        public IReadOnlyList<InputFilterAnalysisResult> Analyses { get; }
    }
}
