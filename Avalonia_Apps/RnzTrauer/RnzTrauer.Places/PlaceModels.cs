using System;
using System.Collections.Generic;

namespace RnzTrauer.Places;

public enum PlaceMatchKind
{
    Empty,
    Normalized,
    Known,
    Ambiguous,
    Unknown,
}

public sealed record PlaceMatch(
    string Input,
    string? Normalized,
    PlaceMatchKind Kind,
    IReadOnlyList<string> Candidates);
