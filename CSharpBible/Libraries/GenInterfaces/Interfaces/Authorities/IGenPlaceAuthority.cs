using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GenInterfaces.Data;

namespace GenInterfaces.Interfaces.Authorities;

/// <summary>
/// Provides access to place reference data and place search services.
/// </summary>
public interface IGenPlaceAuthority
{
    /// <summary>
    /// Gets the authority display name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Searches the authority for matching places.
    /// </summary>
    /// <param name="query">The place query.</param>
    /// <param name="cancellationToken">A token that can cancel the lookup.</param>
    /// <returns>The matching places.</returns>
    Task<IReadOnlyList<GenPlaceMatch>> SearchPlacesAsync(GenPlaceQuery query, CancellationToken cancellationToken = default);
}
