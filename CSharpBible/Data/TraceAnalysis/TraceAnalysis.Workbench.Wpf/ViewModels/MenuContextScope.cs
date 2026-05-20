namespace TraceAnalysis.Workbench.Wpf.ViewModels;

/// <summary>
/// Describes the context scope of a menu command.
/// </summary>
public enum MenuContextScope
{
    /// <summary>
    /// Command is not tied to a specific widget context.
    /// </summary>
    Global,

    /// <summary>
    /// Command is tied to the trace source area.
    /// </summary>
    TraceSource,

    /// <summary>
    /// Command is tied to the channel browser.
    /// </summary>
    ChannelBrowser,

    /// <summary>
    /// Command is tied to the processing-step list.
    /// </summary>
    ProcessingSteps,

    /// <summary>
    /// Command is tied to the current-step editor.
    /// </summary>
    CurrentStep,

    /// <summary>
    /// Command is tied to the preview area.
    /// </summary>
    Preview,

    /// <summary>
    /// Command is tied to the chart area.
    /// </summary>
    Chart,

    /// <summary>
    /// Command is tied to diagnostics.
    /// </summary>
    Diagnostics
}
