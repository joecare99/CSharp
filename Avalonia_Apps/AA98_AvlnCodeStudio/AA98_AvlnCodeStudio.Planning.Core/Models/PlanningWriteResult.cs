using AppKomponentBaseLib.Diagnostics;
using System.Collections.Generic;

namespace AA98_AvlnCodeStudio.Planning.Core.Models;

/// <summary>
/// Represents the provider-neutral result of persisting planning documents.
/// </summary>
public sealed class PlanningWriteResult
{
    public IList<string> WrittenSourcePaths { get; } = new List<string>();

    public IList<Diagnostic> Diagnostics { get; } = new List<Diagnostic>();
}
