using TraceAnalysis.Workbench.Core.Models;

namespace TraceAnalysis.Workbench.Core.Services;

/// <summary>
/// Loads a trace source and derives its structural data basis.
/// </summary>
public interface ITraceSourceLoader
{
    /// <summary>
    /// Loads the specified trace source.
    /// </summary>
    /// <param name="filePath">The trace file path.</param>
    /// <returns>The resulting trace source state.</returns>
    TraceSourceState Load(string filePath);
}
