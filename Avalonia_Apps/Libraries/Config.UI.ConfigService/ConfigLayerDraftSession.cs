using Config.Service;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Config.UI.ConfigService;

internal sealed class ConfigLayerDraftSession
{
    private readonly IConfigLayerStore _store;
    private readonly SemaphoreSlim _operationLock = new(1, 1);
    private readonly object _snapshotLock = new();
    private ConfigLayerSnapshot? _snapshot;

    public ConfigLayerDraftSession(IConfigLayerStore store)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
    }

    public async Task<ConfigLayerSnapshot> LoadAsync(CancellationToken cancellationToken)
    {
        await _operationLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            lock (_snapshotLock)
            {
                if (_snapshot is not null)
                {
                    return _snapshot;
                }
            }

            ConfigLayerSnapshot loaded = await _store.LoadAsync(cancellationToken).ConfigureAwait(false);
            lock (_snapshotLock)
            {
                return _snapshot ??= loaded;
            }
        }
        finally
        {
            _operationLock.Release();
        }
    }

    public async Task<ConfigLayerSnapshot> ReloadAsync(CancellationToken cancellationToken)
    {
        await _operationLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ConfigLayerSnapshot loaded = await _store.LoadAsync(cancellationToken).ConfigureAwait(false);
            lock (_snapshotLock)
            {
                _snapshot = loaded;
                return loaded;
            }
        }
        finally
        {
            _operationLock.Release();
        }
    }

    public ConfigLayerSnapshot GetDraft()
    {
        lock (_snapshotLock)
        {
            return _snapshot
                ?? throw new InvalidOperationException("The configuration layer draft has not been loaded.");
        }
    }

    public ConfigLayerSnapshot SetValue(ConfigPropertyKey key, ConfigLayerValue value)
    {
        lock (_snapshotLock)
        {
            _snapshot = (_snapshot
                ?? throw new InvalidOperationException("The configuration layer draft has not been loaded."))
                .WithValue(key, value);
            return _snapshot;
        }
    }

    public ConfigLayerSnapshot RemoveValue(ConfigPropertyKey key)
    {
        lock (_snapshotLock)
        {
            _snapshot = (_snapshot
                ?? throw new InvalidOperationException("The configuration layer draft has not been loaded."))
                .WithoutValue(key);
            return _snapshot;
        }
    }

    public async Task<ConfigLayerSnapshot> SaveAsync(CancellationToken cancellationToken)
    {
        await _operationLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ConfigLayerSnapshot saving;
            lock (_snapshotLock)
            {
                saving = _snapshot
                    ?? throw new InvalidOperationException("The configuration layer draft has not been loaded.");
            }

            ConfigLayerSnapshot saved = await _store.SaveAsync(saving, cancellationToken).ConfigureAwait(false);
            lock (_snapshotLock)
            {
                if (ReferenceEquals(_snapshot, saving))
                {
                    _snapshot = saved;
                }
                else
                {
                    ConfigLayerSnapshot current = _snapshot
                        ?? throw new InvalidOperationException("The configuration layer draft was cleared while saving.");
                    _snapshot = new ConfigLayerSnapshot(current.Layer, current.Values, saved.StorageVersion);
                }

                return _snapshot;
            }
        }
        finally
        {
            _operationLock.Release();
        }
    }
}
