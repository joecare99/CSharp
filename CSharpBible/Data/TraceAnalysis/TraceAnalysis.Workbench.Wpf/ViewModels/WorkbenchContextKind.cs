namespace TraceAnalysis.Workbench.Wpf.ViewModels;

/// <summary>
/// Defines the active shell context used for menu adaptation.
/// </summary>
public enum WorkbenchContextKind
{
    /// <summary>
    /// No specific context is active.
    /// </summary>
    None,

    /// <summary>
    /// Trace source summary is active.
    /// </summary>
    TraceSource,

    /// <summary>
    /// Channel browser is active.
    /// </summary>
    ChannelBrowser,

    /// <summary>
    /// Processing step list is active.
    /// </summary>
    ProcessingSteps,

    /// <summary>
    /// Current step editor is active.
    /// </summary>
    CurrentStep,

    /// <summary>
    /// Preview area is active.
    /// </summary>
    Preview,

    /// <summary>
    /// Chart area is active.
    /// </summary>
    Chart,

    /// <summary>
    /// Diagnostics area is active.
    /// </summary>
    Diagnostics
}
