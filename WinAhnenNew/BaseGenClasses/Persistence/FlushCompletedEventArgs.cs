using System;
using GenInterfaces.Interfaces.Genealogic;

namespace BaseGenClasses.Persistence;

/// <summary>
/// Provides details about a completed genealogy flush operation.
/// </summary>
public sealed class FlushCompletedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FlushCompletedEventArgs"/> class.
    /// </summary>
    /// <param name="genRequestedEntity">The preferred entity focus that was flushed.</param>
    /// <param name="eScope">The flush scope used for the completed operation.</param>
    public FlushCompletedEventArgs(IGenEntity? genRequestedEntity, GenealogyFlushScope eScope)
    {
        GenRequestedEntity = genRequestedEntity;
        this.eScope = eScope;
    }

    /// <summary>
    /// Gets the preferred entity focus that was flushed.
    /// </summary>
    public IGenEntity? GenRequestedEntity { get; }

    /// <summary>
    /// Gets the flush scope used for the completed operation.
    /// </summary>
    public GenealogyFlushScope eScope { get; }
}
