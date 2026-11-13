using System.Collections.Generic;

namespace AA98_AvlnCodeStudio.Base.Testing.Models;

/// <summary>
/// The test cases and warnings reported by an adapter discovery operation.
/// </summary>
public sealed record TestDiscoveryResult(
    IReadOnlyList<DiscoveredTestCase> Cases,
    IReadOnlyList<string> Warnings);
