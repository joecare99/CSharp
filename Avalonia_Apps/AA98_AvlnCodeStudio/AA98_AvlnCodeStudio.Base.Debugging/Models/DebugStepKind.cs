namespace AA98_AvlnCodeStudio.Base.Debugging.Models;

/// <summary>
/// Describes the execution behavior for a debugger step operation.
/// </summary>
public enum DebugStepKind
{
    /// <summary>
    /// Steps into the next function call.
    /// </summary>
    Into,

    /// <summary>
    /// Executes the current source line and stops at the next line.
    /// </summary>
    Over,

    /// <summary>
    /// Runs until the current function returns.
    /// </summary>
    Out
}
