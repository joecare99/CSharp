using System;
using RnzTrauer.Core.Domain;

namespace RnzTrauer.Core.Services;

/// <summary>Non-destructive parser output; the view model decides which facts to apply.</summary>
public sealed record ParsedNoticeFacts(
    DateTime? BirthDate,
    DateTime? DeathDate,
    DateTime? BurialDate,
    string? MaidenName,
    string? Place,
    int? Age,
    AdvertisementCategory? AdjustedCategory);
