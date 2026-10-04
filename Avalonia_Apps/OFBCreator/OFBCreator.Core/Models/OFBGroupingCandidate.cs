using System.Collections.Generic;

namespace OFBCreator.Core.Models;

/// <summary>A reviewable, evidence-scored proposal to combine two exact surname groups.</summary>
public sealed record OFBGroupingCandidate(
    string Id,
    string LeftSurname,
    string RightSurname,
    string? LeftFamilyTargetId,
    string? RightFamilyTargetId,
    int Score,
    string Status,
    IReadOnlyList<OFBGroupingEvidence> Evidence);
