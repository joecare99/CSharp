using System.Collections.Generic;

namespace ConsoleLib;

/// <summary>Canonical CXAML XML namespaces recognized by the shared loader and validator.</summary>
public static class CxamlNamespaces
{
    /// <summary>Canonical namespace for the core ConsoleLib CXAML controls.</summary>
    public const string Core = "http://schemas.consolelib.dev/cxaml/core";

    /// <summary>Every namespace URI the shared CXAML pipeline recognizes as valid.</summary>
    public static IReadOnlyCollection<string> Recognized { get; } = new[] { Core };
}
