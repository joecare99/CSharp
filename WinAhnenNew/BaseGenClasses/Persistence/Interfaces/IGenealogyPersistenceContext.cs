using System;
using System.Threading;
using System.Threading.Tasks;
using GenInterfaces.Interfaces.Genealogic;

namespace BaseGenClasses.Persistence.Interfaces;

/// <summary>
/// Exposes dirty tracking and flush orchestration for a genealogy root context.
/// </summary>
public interface IGenealogyPersistenceContext
{
    /// <summary>
    /// Occurs when the dirty-state of the genealogy changes.
    /// </summary>
    event EventHandler<DirtyStateChangedEventArgs>? DirtyStateChanged;

    /// <summary>
    /// Occurs when a flush is requested.
    /// </summary>
    event EventHandler<FlushRequestedEventArgs>? FlushRequested;

    /// <summary>
    /// Occurs when a flush completed successfully.
    /// </summary>
    event EventHandler<FlushCompletedEventArgs>? Flushed;

    /// <summary>
    /// Occurs when a flush fails.
    /// </summary>
    event EventHandler<FlushFailedEventArgs>? FlushFailed;

    /// <summary>
    /// Gets a value indicating whether the genealogy currently has unsaved changes.
    /// </summary>
    bool xDirty { get; }

    /// <summary>
    /// Attaches a persistence provider that will receive future flush requests.
    /// </summary>
    /// <param name="persistenceProvider">The persistence provider to attach.</param>
    void AttachPersistenceProvider(IGenealogyPersistenceProvider persistenceProvider);

    /// <summary>
    /// Marks the genealogy as dirty because one of its entities changed.
    /// </summary>
    /// <param name="genChangedEntity">The entity that changed, if known.</param>
    /// <param name="sReason">A descriptive reason for the change.</param>
    void MarkDirty(IGenEntity? genChangedEntity = null, string? sReason = null);

    /// <summary>
    /// Flushes pending changes through the attached persistence provider.
    /// </summary>
    /// <param name="genRequestedEntity">The entity focus for the flush request, if known.</param>
    /// <param name="eScope">The requested persistence scope.</param>
    /// <param name="cancellationToken">A token that can cancel the flush.</param>
    Task FlushAsync(
        IGenEntity? genRequestedEntity = null,
        GenealogyFlushScope eScope = GenealogyFlushScope.Auto,
        CancellationToken cancellationToken = default);
}
