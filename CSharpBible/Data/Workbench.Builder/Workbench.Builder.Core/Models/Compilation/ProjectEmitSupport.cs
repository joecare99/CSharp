namespace Workbench.Builder.Core.Models.Compilation;

/// <summary>
/// Represents the emit decision derived from the inspected project shape.
/// </summary>
public sealed class ProjectEmitSupport
{
    /// <summary>
    /// Initializes a new instance of <see cref="ProjectEmitSupport"/>.
    /// </summary>
    /// <param name="canEmit">A value indicating whether the project can be emitted in the current slice.</param>
    /// <param name="emitKind">The primary emit kind selected for the project.</param>
    /// <param name="reason">The optional reason why the project cannot be emitted or why a specific emit kind was chosen.</param>
    public ProjectEmitSupport(bool canEmit, ProjectEmitKind emitKind, string? reason = null)
    {
        CanEmit = canEmit;
        EmitKind = emitKind;
        Reason = reason;
    }

    /// <summary>
    /// Gets a value indicating whether the project can be emitted in the current slice.
    /// </summary>
    public bool CanEmit { get; }

    /// <summary>
    /// Gets the primary emit kind selected for the project.
    /// </summary>
    public ProjectEmitKind EmitKind { get; }

    /// <summary>
    /// Gets the optional reason why the project cannot be emitted or why a specific emit kind was chosen.
    /// </summary>
    public string? Reason { get; }
}
