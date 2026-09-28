using System.Collections.Generic;

namespace TranspilerLib.CSharp.StatEqualCheck;

internal sealed class FlowNode(int id, StatEqualSourceSpan? sourceSpan)
{
    public int Id { get; } = id;
    public StatEqualSourceSpan? SourceSpan { get; } = sourceSpan;
    public List<FlowEdge> Edges { get; } = [];
}
