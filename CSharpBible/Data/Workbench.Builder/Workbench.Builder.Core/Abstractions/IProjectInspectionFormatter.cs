using Workbench.Builder.Core.Models.Inspection;

namespace Workbench.Builder.Core.Abstractions;

/// <summary>
/// Formats a structured project inspection result for host-facing output.
/// </summary>
public interface IProjectInspectionFormatter
{
    /// <summary>
    /// Formats the specified inspection result.
    /// </summary>
    /// <param name="result">The inspection result to format.</param>
    /// <param name="format">The requested output format.</param>
    /// <returns>The formatted output text.</returns>
    string Format(ProjectInspectionResult result, ProjectInspectionOutputFormat format);
}
