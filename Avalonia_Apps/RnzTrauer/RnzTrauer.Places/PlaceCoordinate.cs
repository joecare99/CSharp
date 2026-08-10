using System;

namespace RnzTrauer.Places;

public sealed record PlaceCoordinate(
    string Place,
    double Latitude,
    double Longitude,
    string? Source,
    bool IsApproximate);
