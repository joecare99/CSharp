using System;

namespace ConsoleLib;

/// <summary>Explicit data context used to resolve CXAML bindings and commands.</summary>
public sealed class CxamlLoadContext
{
    public CxamlLoadContext(object dataContext, bool allowUnresolvedBindings = false, IServiceProvider? services = null)
    {
        DataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        AllowUnresolvedBindings = allowUnresolvedBindings;
        Services = services ?? EmptyServiceProvider.Instance;
    }

    public object DataContext { get; }
    public bool AllowUnresolvedBindings { get; }
    public IServiceProvider Services { get; }

    private sealed class EmptyServiceProvider : IServiceProvider
    {
        internal static readonly EmptyServiceProvider Instance = new();
        public object? GetService(Type serviceType) => null;
    }
}
