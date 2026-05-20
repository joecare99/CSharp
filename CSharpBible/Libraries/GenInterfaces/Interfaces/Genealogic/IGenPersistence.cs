using System;
using System.Threading;
using System.Threading.Tasks;
using GenInterfaces.Data;

namespace GenInterfaces.Interfaces.Genealogic;

/// <summary>
/// Provides unified genealogy persistence operations for loading and saving genealogy data.
/// </summary>
public interface IGenPersistence
{
    /// <summary>
    /// Loads a genealogy aggregate into the provided root object.
    /// </summary>
    /// <param name="genealogy">The genealogy root to populate.</param>
    /// <param name="cancellationToken">A token that can cancel the load operation.</param>
    Task LoadAsync(IGenealogy genealogy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads a single entity for the provided genealogy context.
    /// </summary>
    /// <param name="genealogy">The owning genealogy context.</param>
    /// <param name="gUid">The unique identifier of the entity to load.</param>
    /// <param name="eGenType">The optional entity type hint.</param>
    /// <param name="cancellationToken">A token that can cancel the load operation.</param>
    /// <returns>The loaded entity, or <see langword="null"/> if no matching entity exists.</returns>
    Task<IGenEntity?> LoadEntityAsync(
        IGenealogy genealogy,
        Guid gUid,
        EGenType? eGenType = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves the complete genealogy aggregate.
    /// </summary>
    /// <param name="genealogy">The genealogy aggregate to save.</param>
    /// <param name="cancellationToken">A token that can cancel the save operation.</param>
    Task SaveAsync(IGenealogy genealogy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists the provided genealogy according to the requested scope.
    /// </summary>
    /// <param name="genealogy">The genealogy aggregate to persist.</param>
    /// <param name="genRequestedEntity">The entity that initiated the flush, if known.</param>
    /// <param name="eScope">The requested persistence scope.</param>
    /// <param name="cancellationToken">A token that can cancel the persistence operation.</param>
    Task FlushAsync(
        IGenealogy genealogy,
        IGenEntity? genRequestedEntity,
        GenealogyPersistenceScope eScope = GenealogyPersistenceScope.Auto,
        CancellationToken cancellationToken = default);
}
