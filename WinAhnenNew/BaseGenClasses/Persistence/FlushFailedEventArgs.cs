using System;
using GenInterfaces.Interfaces.Genealogic;

namespace BaseGenClasses.Persistence;

/// <summary>
/// Provides details about a failed genealogy flush operation.
/// </summary>
public sealed class FlushFailedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FlushFailedEventArgs"/> class.
    /// </summary>
    /// <param name="genRequestedEntity">The preferred entity focus for the failed flush.</param>
    /// <param name="eScope">The requested flush scope.</param>
    /// <param name="exException">The exception that caused the flush failure.</param>
    public FlushFailedEventArgs(IGenEntity? genRequestedEntity, GenealogyFlushScope eScope, Exception exException)
    {
        GenRequestedEntity = genRequestedEntity;
        this.eScope = eScope;
        ExException = exException;
    }

    /// <summary>
    /// Gets the preferred entity focus for the failed flush.
    /// </summary>
    public IGenEntity? GenRequestedEntity { get; }

    /// <summary>
    /// Gets the requested flush scope.
    /// </summary>
    public GenealogyFlushScope eScope { get; }

    /// <summary>
    /// Gets the exception that caused the flush failure.
    /// </summary>
    public Exception ExException { get; }
}
