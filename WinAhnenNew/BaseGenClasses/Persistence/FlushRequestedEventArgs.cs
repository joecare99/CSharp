using System;
using GenInterfaces.Interfaces.Genealogic;

namespace BaseGenClasses.Persistence;

/// <summary>
/// Provides details about a pending flush request for a genealogy persistence context.
/// </summary>
public sealed class FlushRequestedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FlushRequestedEventArgs"/> class.
    /// </summary>
    /// <param name="genRequestedEntity">The preferred entity focus for the flush request.</param>
    /// <param name="eScope">The requested flush scope.</param>
    /// <param name="xHasPendingChanges">Indicates whether the genealogy is currently dirty.</param>
    public FlushRequestedEventArgs(IGenEntity? genRequestedEntity, GenealogyFlushScope eFlushScope, bool xPendingChanges)
    {
        GenRequestedEntity = genRequestedEntity;
        eScope = eFlushScope;
        xHasPendingChanges = xPendingChanges;
    }

    /// <summary>
    /// Gets the preferred entity focus for the flush request.
    /// </summary>
    public IGenEntity? GenRequestedEntity { get; }

    /// <summary>
    /// Gets the requested flush scope.
    /// </summary>
    public GenealogyFlushScope eScope { get; }

    /// <summary>
    /// Gets a value indicating whether the genealogy currently has pending changes.
    /// </summary>
    public bool xHasPendingChanges { get; }
}
