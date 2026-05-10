using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GenInterfaces.Data;

namespace GenInterfaces.Interfaces.Authorities;

/// <summary>
/// Provides access to general name reference data such as given-name and sex suggestions.
/// </summary>
public interface IGenNameAuthority
{
    /// <summary>
    /// Gets the authority display name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Searches the authority for matching names.
    /// </summary>
    /// <param name="query">The name query text.</param>
    /// <param name="cancellationToken">A token that can cancel the lookup.</param>
    /// <returns>The matching names.</returns>
    Task<IReadOnlyList<GenNameMatch>> SearchNamesAsync(string query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Suggests a sex classification for the supplied given name.
    /// </summary>
    /// <param name="givenName">The given name to classify.</param>
    /// <param name="cancellationToken">A token that can cancel the lookup.</param>
    /// <returns>A suggestion when available; otherwise <see langword="null"/>.</returns>
    Task<GenSexSuggestion?> SuggestSexAsync(string givenName, CancellationToken cancellationToken = default);
}
