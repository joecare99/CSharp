using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TileSetAnimator.Models;
using TileSetAnimator.Persistence;

namespace TileSetAnimator.Services;

/// <summary>
/// Converts tile definitions to and from strongly typed C# enums.
/// </summary>
public interface ITileEnumSerializer
{
    /// <summary>
    /// Exports the supplied tiles to a C# enum file.
    /// </summary>
    Task ExportAsync(IEnumerable<TileDefinition> tiles, string filePath, string enumName, string? namespaceName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Imports tile metadata from a C# enum definition file.
    /// </summary>
    Task<IReadOnlyList<TileMetadataSnapshot>> ImportAsync(string filePath, CancellationToken cancellationToken = default);
}
