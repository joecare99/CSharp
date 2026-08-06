using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

    [TestMethod]
    public async Task OfflineGeocodingAdapter_ReturnsFixtureAndHonorsNormalization()
    {
        var adapter = new OfflineGeocodingAdapter(new Dictionary<string, GeocodingResult>
        {
            ["Heidelberg"] = new("Heidelberg", 49.3988, 8.6724, "Heidelberg, DE", false),
        });

        var result = await adapter.ResolveAsync("  heidelberg ");

        Assert.IsNotNull(result);
        Assert.AreEqual(49.3988, result!.Latitude);
        Assert.AreEqual("Heidelberg, DE", result.DisplayName);
    }

    [TestMethod]
    public async Task OfflineGeocodingAdapter_PropagatesCancellation()
    {
        var adapter = new OfflineGeocodingAdapter(
            new Dictionary<string, GeocodingResult>());
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        OperationCanceledException? thrown = null;
        try
        {
            await adapter.ResolveAsync("Heidelberg", cancellation.Token);
        }
        catch (OperationCanceledException exception)
        {
            thrown = exception;
        }

        Assert.IsNotNull(thrown);
    }

    [TestMethod]
    public async Task CachingAdapter_ReturnsCacheHitWithoutCallingInnerAdapter()
    {
        var inner = new CountingAdapter(new GeocodingResult(
            "Heidelberg", 49.3988, 8.6724, "Heidelberg, DE", false));
        var clock = new ManualTimeProvider();
        var adapter = new CachingGeocodingAdapter(
            inner, TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(1), clock);

        await adapter.ResolveAsync("Heidelberg");
        clock.Advance(TimeSpan.FromSeconds(30));
        var result = await adapter.ResolveAsync(" heidelberg ");

        Assert.AreEqual(1, inner.CallCount);
        Assert.AreEqual("Heidelberg, DE", result!.DisplayName);
    }

    [TestMethod]
    public async Task CachingAdapter_ReportsRateLimitForUncachedRequest()
    {
        var inner = new CountingAdapter(null);
        var clock = new ManualTimeProvider();
        var adapter = new CachingGeocodingAdapter(
            inner, TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(1), clock);

        await adapter.ResolveAsync("Heidelberg");
        clock.Advance(TimeSpan.FromSeconds(20));

        GeocodingRateLimitException? thrown = null;
        try
        {
            await adapter.ResolveAsync("Mannheim");
        }
        catch (GeocodingRateLimitException exception)
        {
            thrown = exception;
        }

        Assert.IsNotNull(thrown);
        Assert.AreEqual(TimeSpan.FromSeconds(40), thrown!.RetryAfter);
        Assert.AreEqual(1, inner.CallCount);
    }

    private sealed class CountingAdapter : IGeocodingAdapter
    {
        private readonly GeocodingResult? _result;

        public CountingAdapter(GeocodingResult? result) => _result = result;
        public int CallCount { get; private set; }

        public Task<GeocodingResult?> ResolveAsync(
            string query,
            CancellationToken cancellationToken = default)
        {
            CallCount++;
            return Task.FromResult(_result);
        }
    }

    private sealed class ManualTimeProvider : TimeProvider
    {
        private DateTimeOffset _now = DateTimeOffset.UtcNow;
        public override DateTimeOffset GetUtcNow() => _now;
        public void Advance(TimeSpan duration) => _now += duration;
    }
}
