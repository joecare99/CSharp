using System.Threading;
using System.Threading.Tasks;
using TileSetAnimator.Persistence;

namespace TileSetAnimator.Services;

/// <summary>
/// Handles persisting and restoring tile set state alongside the source sprite sheet.
/// </summary>
public interface ITileSetPersistence
{
    /// <summary>
    /// Loads the persisted state for the supplied tile sheet path.
    /// </summary>
    Task<TileSetState?> LoadStateAsync(string tileSheetPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists the latest state for the supplied tile sheet.
    /// </summary>
    Task SaveStateAsync(TileSetState state, CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads a tile set structure from a specific state file.
    /// </summary>
    Task<TileSetState?> LoadStateFromFileAsync(string stateFilePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves a tile set structure to a specific state file.
    /// </summary>
    Task SaveStateToFileAsync(TileSetState state, string stateFilePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads a shared metadata definition that can be reused across tile sets.
    /// </summary>
    Task<TileSetClassMetadata?> LoadClassAsync(string classKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists a shared metadata definition for cross-resolution tile sets.
    /// </summary>
    Task SaveClassAsync(TileSetClassMetadata metadata, CancellationToken cancellationToken = default);
}
