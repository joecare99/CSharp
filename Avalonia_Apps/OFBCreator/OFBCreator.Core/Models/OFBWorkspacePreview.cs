using System.Collections.Generic;

namespace OFBCreator.Core.Models;

/// <summary>Combines privacy effects and grouping evidence from the same filtered export view.</summary>
/// <param name="Grouping">The grouping evidence and decisions for the filtered families.</param>
/// <param name="People">People retained in the filtered families and their changed fields.</param>
public sealed record OFBWorkspacePreview(
    OFBFamilyGroupingResult Grouping,
    IReadOnlyList<OFBPersonPrivacyPreview> People);
