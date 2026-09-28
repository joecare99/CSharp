using System;
using System.Collections.Generic;
using System.Linq;
using TranspilerLib.Interfaces.Code;
using TranspilerLib.Models.Scanner;

namespace TranspilerLib.CSharp.StatEqualCheck;

/// <summary>Provides conservative static behavioral comparison for C# source and code-block trees.</summary>
public static class StatEqualCheck
{
    /// <summary>Parses and compares two C# sources using deterministic control-flow graphs.</summary>
    /// <param name="leftSource">The first C# source or executable source fragment.</param>
    /// <param name="rightSource">The second C# source or executable source fragment.</param>
    /// <returns>An immutable proof status with source-located diagnostics.</returns>
    public static StatEqualResult Compare(string leftSource, string rightSource)
    {
        ArgumentNullException.ThrowIfNull(leftSource);
        ArgumentNullException.ThrowIfNull(rightSource);
        return CompareGraphs(SourceFlowParser.Parse(leftSource), SourceFlowParser.Parse(rightSource));
    }

    /// <summary>Renders and compares two code-block trees, optionally treating caller-selected blocks as terminals.</summary>
    /// <param name="original">The original code-block tree.</param>
    /// <param name="candidate">The candidate code-block tree.</param>
    /// <param name="originalTerminalBlocks">Optional blocks in <paramref name="original"/> to treat as terminals.</param>
    /// <param name="candidateTerminalBlocks">Optional blocks in <paramref name="candidate"/> to treat as terminals.</param>
    /// <remarks>
    /// Each listed block must be the same object instance as a node in its corresponding tree. It is represented as
    /// a synthetic <c>return;</c> action: its descendants are not traversed and execution cannot proceed to later
    /// siblings along paths entering the block. Each input has an independent list. Unlisted blocks, including
    /// switch, try/catch, or dispatcher code, remain fully subject to comparison. No Resume-pattern detection or
    /// automatic block exclusion occurs. Input trees are not modified; a listed block not found in its tree is rejected.
    /// </remarks>
    /// <returns>An immutable proof status with source-located diagnostics.</returns>
    public static StatEqualResult Compare(ICodeBlock original, ICodeBlock candidate,
        IEnumerable<ICodeBlock>? originalTerminalBlocks = null,
        IEnumerable<ICodeBlock>? candidateTerminalBlocks = null)
    {
        ArgumentNullException.ThrowIfNull(original);
        ArgumentNullException.ThrowIfNull(candidate);
        var originalSource = CodeBlockSourceRenderer.Render(original, originalTerminalBlocks, nameof(originalTerminalBlocks));
        var candidateSource = CodeBlockSourceRenderer.Render(candidate, candidateTerminalBlocks, nameof(candidateTerminalBlocks));
        return Compare(originalSource, candidateSource);
    }

    private static StatEqualResult CompareGraphs(FlowGraph left, FlowGraph right)
    {
        var findings = left.Findings.Concat(right.Findings).ToList();
        if (!WeakBisimulation.AreEquivalent(left, right, out var mismatchLeft, out var mismatchRight))
        {
            findings.Add(new StatEqualFinding(StatEqualFindingSeverity.Error, "BEHAVIOR_MISMATCH",
                "The inputs have observably different modeled control-flow behavior.",
                left.Nodes[mismatchLeft].SourceSpan, right.Nodes[mismatchRight].SourceSpan));
            return new StatEqualResult(StatEqualStatus.NotEquivalent, findings);
        }

        var status = findings.Any(finding => finding.Severity == StatEqualFindingSeverity.Warning)
            ? StatEqualStatus.Inconclusive
            : StatEqualStatus.Equivalent;
        return new StatEqualResult(status, findings);
    }
}
