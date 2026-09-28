using System.Collections.Generic;
using TranspilerLib.CSharp.VBLegacyReplace.Models;

namespace TranspilerLib.CSharp.VBLegacyReplace.Services;

internal sealed class CompiledRule
{
    public CompiledRule(ReplacementRule definition, List<TemplatePart> sourceParts, List<ReplacementSegment> replacementSegments, int fixedTokenCount)
    {
        Definition = definition;
        SourceParts = sourceParts;
        ReplacementSegments = replacementSegments;
        FixedTokenCount = fixedTokenCount;
    }

    public ReplacementRule Definition { get; }
    public List<TemplatePart> SourceParts { get; }
    public List<ReplacementSegment> ReplacementSegments { get; }
    public int FixedTokenCount { get; }
}











