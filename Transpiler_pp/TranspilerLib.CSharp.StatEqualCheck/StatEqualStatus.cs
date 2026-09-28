namespace TranspilerLib.CSharp.StatEqualCheck;

/// <summary>Describes whether two inputs were proven equivalent, disproven, or not fully modeled.</summary>
public enum StatEqualStatus
{
    /// <summary>The modeled control-flow graphs are weakly bisimilar and contain no unmodeled findings.</summary>
    Equivalent,
    /// <summary>The modeled control-flow graphs have observably different behavior.</summary>
    NotEquivalent,
    /// <summary>The inputs may match, but at least one construct could not be proven within the modeled subset.</summary>
    Inconclusive
}
