using System;
using System.Collections.Generic;
using ConsoleLib.Interfaces;

namespace ConsoleLib;

/// <summary>Materialized CXAML root and its explicitly named controls.</summary>
public sealed class CxamlLoadResult
{
    public CxamlLoadResult(IControl root, IReadOnlyDictionary<string, IControl> namedControls)
    {
        Root = root ?? throw new ArgumentNullException(nameof(root));
        NamedControls = namedControls ?? throw new ArgumentNullException(nameof(namedControls));
    }

    public IControl Root { get; }
    public IReadOnlyDictionary<string, IControl> NamedControls { get; }
}
