using System.Threading;
using System.Threading.Tasks;

namespace Config.Service;

/// <summary>
/// Loads and saves explicit overrides for one configuration layer.
/// </summary>
public interface IConfigLayerStore
{
    ConfigLayer Layer { get; }

    Task<ConfigLayerSnapshot> LoadAsync(CancellationToken cancellationToken = default);

    Task<ConfigLayerSnapshot> SaveAsync(ConfigLayerSnapshot snapshot, CancellationToken cancellationToken = default);
}
