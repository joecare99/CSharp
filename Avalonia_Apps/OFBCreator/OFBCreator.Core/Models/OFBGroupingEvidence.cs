namespace OFBCreator.Core.Models;

/// <summary>One typed observation contributing to a surname-merge candidate score.</summary>
public sealed record OFBGroupingEvidence(string Kind, int? Distance, int ObservationCount, int Score);
