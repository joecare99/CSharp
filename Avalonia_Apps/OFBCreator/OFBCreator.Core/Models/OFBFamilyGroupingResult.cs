using System.Collections.Generic;
using GenInterfaces.Interfaces.Genealogic;

namespace OFBCreator.Core.Models;

/// <summary>Contains applied family groups, review candidates, and grouping diagnostics.</summary>
public sealed record OFBFamilyGroupingResult(
    IReadOnlyDictionary<string, IReadOnlyList<IGenFamily>> Groups,
    IReadOnlyList<OFBGroupingCandidate> Candidates,
    IReadOnlyList<OFBGroupingDiagnostic> Diagnostics);
