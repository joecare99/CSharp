using System.Collections.Generic;

namespace ConsoleLib;

/// <summary>Result of optional CXAML source generation.</summary>
public sealed class CxamlGenerationResult
{
    public CxamlGenerationResult(string generatedCode, IReadOnlyList<CxamlDiagnostic> diagnostics)
    {
        GeneratedCode = generatedCode;
        Diagnostics = diagnostics;
    }

    public string GeneratedCode { get; }
    public IReadOnlyList<CxamlDiagnostic> Diagnostics { get; }
    public bool Succeeded => Diagnostics.Count == 0;
}
