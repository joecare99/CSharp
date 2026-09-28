using System.Collections.Generic;

namespace TranspilerLib.CSharp.StatEqualCheck;

internal sealed class FlowGraph(IReadOnlyList<FlowNode> nodes, int entry, int exit,
    IReadOnlyList<StatEqualFinding> findings)
{
    public IReadOnlyList<FlowNode> Nodes { get; } = nodes;
    public int Entry { get; } = entry;
    public int Exit { get; } = exit;
    public IReadOnlyList<StatEqualFinding> Findings { get; } = findings;
}
