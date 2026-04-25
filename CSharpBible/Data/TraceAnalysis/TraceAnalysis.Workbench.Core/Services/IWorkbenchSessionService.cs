using TraceAnalysis.Workbench.Core.Models;

namespace TraceAnalysis.Workbench.Core.Services;

/// <summary>
/// Provides the initial shell-level session state for the trace analysis workbench.
/// </summary>
public interface IWorkbenchSessionService
{
    /// <summary>
    /// Creates the initial shell session state.
    /// </summary>
    /// <returns>The initial workbench session model.</returns>
    WorkbenchSessionModel CreateInitialSession();
}
