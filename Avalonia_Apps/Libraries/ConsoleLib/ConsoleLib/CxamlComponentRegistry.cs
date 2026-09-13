using System;
using System.Collections.Generic;
using ConsoleLib.Interfaces;

namespace ConsoleLib;

public sealed class CxamlComponentRegistry : ICxamlComponentRegistry
{
    private readonly Dictionary<string, Func<IServiceProvider, CxamlLoadContext, IControl>> _factories =
        new(StringComparer.Ordinal);
    private readonly IServiceProvider _services;

    public CxamlComponentRegistry(IServiceProvider? services = null)
        => _services = services ?? EmptyServiceProvider.Instance;

    public void Register(string xmlName, Func<IServiceProvider, CxamlLoadContext, IControl> factory)
    {
        if (string.IsNullOrWhiteSpace(xmlName))
            throw new ArgumentException("An XML component name is required.", nameof(xmlName));
        _factories.Add(xmlName, factory ?? throw new ArgumentNullException(nameof(factory)));
    }

    public bool TryCreate(string xmlName, CxamlLoadContext context, out IControl control)
    {
        if (_factories.TryGetValue(xmlName, out var factory))
        {
            control = factory(_services, context);
            return control is not null;
        }
        control = null!;
        return false;
    }

    private sealed class EmptyServiceProvider : IServiceProvider
    {
        internal static readonly EmptyServiceProvider Instance = new();
        public object? GetService(Type serviceType) => null;
    }
}
