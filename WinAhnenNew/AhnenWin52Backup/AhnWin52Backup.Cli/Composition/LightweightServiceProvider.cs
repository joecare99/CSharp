using System;
using System.Collections.Generic;

namespace AhnWin52Backup.Cli.Composition;

internal sealed class LightweightServiceProvider : IServiceProvider
{
    private readonly IReadOnlyDictionary<Type, Func<IServiceProvider, object>> _factories;
    private readonly Dictionary<Type, object> _instances = new();

    public LightweightServiceProvider(IReadOnlyDictionary<Type, Func<IServiceProvider, object>> factories)
    {
        _factories = factories;
    }

    public object? GetService(Type serviceType)
    {
        if (_instances.TryGetValue(serviceType, out object? instance))
        {
            return instance;
        }

        if (!_factories.TryGetValue(serviceType, out Func<IServiceProvider, object>? factory))
        {
            return null;
        }

        instance = factory(this);
        _instances.Add(serviceType, instance);
        return instance;
    }
}
