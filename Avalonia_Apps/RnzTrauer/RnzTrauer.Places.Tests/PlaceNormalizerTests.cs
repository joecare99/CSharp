using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RnzTrauer.Places.Tests;

[TestClass]
public sealed class PlaceNormalizerTests
{
    private readonly PlaceNormalizer _normalizer = new();

    [TestMethod]
    public void Normalize_CollapsesWhitespaceAndNonBreakingSpaces()
    {
        Assert.AreEqual("Bad Mergentheim", PlaceNormalizer.Normalize("  Bad\u00a0  Mergentheim "));
    }

    [TestMethod]
    public void Resolve_ReturnsCanonicalKnownPlace()
    {
        var result = _normalizer.Resolve("  Heidelberg ", ["Heidelberg", "Mannheim"]);

        Assert.AreEqual(PlaceMatchKind.Known, result.Kind);
        Assert.AreEqual("Heidelberg", result.Normalized);
    }

    [TestMethod]
    public void Resolve_ReportsUnknownPlaceWithoutInventingCoordinates()
    {
        var result = _normalizer.Resolve("Neustadt", ["Heidelberg"]);

        Assert.AreEqual(PlaceMatchKind.Unknown, result.Kind);
        Assert.AreEqual("Neustadt", result.Normalized);
        Assert.AreEqual(0, result.Candidates.Count);
    }

    [TestMethod]
    public void Resolve_ReportsEmptyInput()
    {
        var result = _normalizer.Resolve("  ", Array.Empty<string>());

        Assert.AreEqual(PlaceMatchKind.Empty, result.Kind);
        Assert.IsNull(result.Normalized);
    }

    [TestMethod]
    public void Resolve_UsesAliasToReturnCanonicalKnownPlace()
    {
        var aliases = new Dictionary<string, IReadOnlyList<string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["HD"] = ["Heidelberg"],
        };

        var result = _normalizer.Resolve(" HD ", ["Heidelberg"], aliases);

        Assert.AreEqual(PlaceMatchKind.Known, result.Kind);
        Assert.AreEqual("Heidelberg", result.Normalized);
    }

    [TestMethod]
    public void Resolve_ReportsAmbiguousAliasWithoutChoosingCandidate()
    {
        var aliases = new Dictionary<string, IReadOnlyList<string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["Frankfurt"] = ["Frankfurt am Main", "Frankfurt (Oder)"],
        };

        var result = _normalizer.Resolve(
            "Frankfurt",
            ["Frankfurt am Main", "Frankfurt (Oder)"],
            aliases);

        Assert.AreEqual(PlaceMatchKind.Ambiguous, result.Kind);
        CollectionAssert.AreEquivalent(
            new[] { "Frankfurt am Main", "Frankfurt (Oder)" },
            result.Candidates.ToArray());
    }
}
