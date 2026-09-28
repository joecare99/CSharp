using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TranspilerLib.CSharp.StatEqualCheck;

/// <summary>The immutable outcome of a static behavioral comparison.</summary>
public sealed class StatEqualResult
{
    /// <summary>Creates a result and takes a defensive copy of its findings.</summary>
    public StatEqualResult(StatEqualStatus status, IEnumerable<StatEqualFinding> findings)
    {
        ArgumentNullException.ThrowIfNull(findings);
        Status = status;
        Findings = new ReadOnlyCollection<StatEqualFinding>(findings.ToArray());
    }

    /// <summary>Gets the proof status.</summary>
    public StatEqualStatus Status { get; }
    /// <summary>Gets all parser, modeling, and comparison findings.</summary>
    public IReadOnlyList<StatEqualFinding> Findings { get; }
    /// <summary>Gets whether equivalence was proven with no unmodeled constructs.</summary>
    public bool IsEquivalent => Status == StatEqualStatus.Equivalent;
}



