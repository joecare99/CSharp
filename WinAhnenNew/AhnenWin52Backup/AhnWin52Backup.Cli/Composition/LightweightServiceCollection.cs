using System;
using System.Collections.Generic;

namespace AhnWin52Backup.Cli.Composition;

internal sealed class LightweightServiceCollection
{
    private readonly Dictionary<Type, Func<IServiceProvider, object>> _factories = new();

    public void AddSingleton<TService>(Func<IServiceProvider, TService> factory)
        where TService : class
    {
        ArgumentNullException.ThrowIfNull(factory);
        _factories.Add(typeof(TService), provider => factory(provider));
    }

    public IServiceProvider BuildServiceProvider() =>
        new LightweightServiceProvider(new Dictionary<Type, Func<IServiceProvider, object>>(_factories));
}
