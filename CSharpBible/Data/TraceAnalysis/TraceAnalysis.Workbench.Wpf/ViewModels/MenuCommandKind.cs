namespace TraceAnalysis.Workbench.Wpf.ViewModels;

/// <summary>
/// Distinguishes general and conditional menu commands.
/// </summary>
public enum MenuCommandKind
{
    /// <summary>
    /// Command is generally available.
    /// </summary>
    General,

    /// <summary>
    /// Command depends on an active widget or context.
    /// </summary>
    Conditional
}
