using System.Threading;
using System.Threading.Tasks;
using GenInterfaces.Interfaces.Genealogic;

namespace BaseGenClasses.Persistence;

/// <summary>
/// Persists a genealogy aggregate when a flush is requested by the genealogy root context.
/// </summary>
public interface IGenealogyPersistenceProvider
{
    /// <summary>
    /// Persists the provided genealogy according to the requested scope.
    /// </summary>
    /// <param name="genGenealogy">The genealogy aggregate to persist.</param>
    /// <param name="genRequestedEntity">The entity that initiated the flush, if known.</param>
    /// <param name="eScope">The requested persistence scope.</param>
    /// <param name="cancellationToken">A token that can cancel the persistence operation.</param>
    Task FlushAsync(
        IGenealogy genGenealogy,
        IGenEntity? genRequestedEntity,
        GenealogyFlushScope eScope,
        CancellationToken cancellationToken = default);
}
