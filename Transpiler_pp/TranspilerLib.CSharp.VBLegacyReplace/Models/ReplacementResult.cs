using System.Collections.Generic;
using TranspilerLib.CSharp.VBLegacyReplace.Models;

namespace TranspilerLib.CSharp.VBLegacyReplace;

/// <summary>Contains transformed source, using directives, and diagnostics.</summary>
public sealed class ReplacementResult
{
    /// <summary>Initializes the immutable result of a replacement operation.</summary>
    /// <param name="source">The transformed source text.</param>
    /// <param name="requiredUsings">Usings required by applied replacements.</param>
    /// <param name="diagnostics">Diagnostics generated while processing the source.</param>
    public ReplacementResult(string source, IReadOnlyList<string> requiredUsings, IReadOnlyList<RuleDiagnostic> diagnostics)
    {
        Source = source;
        RequiredUsings = requiredUsings;
        Diagnostics = diagnostics;
    }

    /// <summary>Gets the transformed source text.</summary>
    public string Source { get; }

    /// <summary>Gets the distinct required namespaces from applied rules.</summary>
    public IReadOnlyList<string> RequiredUsings { get; }

    /// <summary>Gets diagnostics generated from configuration and source processing.</summary>
    public IReadOnlyList<RuleDiagnostic> Diagnostics { get; }
}

