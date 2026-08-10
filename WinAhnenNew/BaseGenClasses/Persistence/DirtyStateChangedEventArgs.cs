using GenInterfaces.Interfaces.Genealogic;
using System;

namespace BaseGenClasses.Persistence;

/// <summary>
/// Provides details about a dirty-state transition of a genealogy persistence context.
/// </summary>
public sealed class DirtyStateChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DirtyStateChangedEventArgs"/> class.
    /// </summary>
    /// <param name="isDirty">The new dirty-state value.</param>
    /// <param name="changedEntity">The entity that caused the state update, if known.</param>
    /// <param name="reason">A descriptive reason for the state change.</param>
    public DirtyStateChangedEventArgs(bool isDirty, IGenEntity? changedEntity, string? reason)
    {
        IsDirty = isDirty;
        ChangedEntity = changedEntity;
        Reason = reason;
    }

    /// <summary>
    /// Gets a value indicating whether the genealogy contains unsaved changes.
    /// </summary>
    public bool IsDirty { get; }

    /// <summary>
    /// Gets the legacy dirty-state value.
    /// </summary>
    [Obsolete("Use IsDirty instead.")]
    public bool xIsDirty => IsDirty;

    /// <summary>
    /// Gets the entity that triggered the dirty-state update, if known.
    /// </summary>
    public IGenEntity? ChangedEntity { get; }

    /// <summary>
    /// Gets the legacy entity reference for the dirty-state update.
    /// </summary>
    [Obsolete("Use ChangedEntity instead.")]
    public IGenEntity? GenChangedEntity => ChangedEntity;

    /// <summary>
    /// Gets an optional descriptive reason for the dirty-state update.
    /// </summary>
    public string? Reason { get; }
}
