using System;

namespace ConsoleLib;

/// <summary>Explicit data context used to resolve CXAML bindings and commands.</summary>
public sealed class CxamlLoadContext
{
    public CxamlLoadContext(object dataContext, bool allowUnresolvedBindings = false)
    {
        DataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        AllowUnresolvedBindings = allowUnresolvedBindings;
    }

    public object DataContext { get; }
    public bool AllowUnresolvedBindings { get; }
}
