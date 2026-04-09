using System;
using GenInterfaces.Interfaces.Genealogic;

namespace BaseGenClasses.Persistence;

/// <summary>
/// Provides details about a dirty-state transition of a genealogy persistence context.
/// </summary>
public sealed class DirtyStateChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DirtyStateChangedEventArgs"/> class.
    /// </summary>
    /// <param name="xIsDirty">The new dirty-state value.</param>
    /// <param name="genChangedEntity">The entity that caused the state update, if known.</param>
    /// <param name="sReason">A descriptive reason for the state change.</param>
    public DirtyStateChangedEventArgs(bool xIsDirty, IGenEntity? genChangedEntity, string? sReason)
    {
        this.xIsDirty = xIsDirty;
        GenChangedEntity = genChangedEntity;
        Reason = sReason;
    }

    /// <summary>
    /// Gets a value indicating whether the genealogy contains unsaved changes.
    /// </summary>
    public bool xIsDirty { get; }

    /// <summary>
    /// Gets the entity that triggered the dirty-state update, if known.
    /// </summary>
    public IGenEntity? GenChangedEntity { get; }

    /// <summary>
    /// Gets an optional descriptive reason for the dirty-state update.
    /// </summary>
    public string? Reason { get; }
}
