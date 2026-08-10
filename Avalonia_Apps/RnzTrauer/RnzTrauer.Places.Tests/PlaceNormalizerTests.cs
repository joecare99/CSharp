using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RnzTrauer.Places.Tests;

[TestClass]
public sealed class PlaceNormalizerTests
{
    private readonly PlaceNormalizer _normalizer = new();

    private static PlacesPolicyFixture LoadFixture()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Fixtures", "places-policy.json");
        var fixture = JsonSerializer.Deserialize<PlacesPolicyFixture>(
            File.ReadAllText(path),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.IsNotNull(fixture);
        return fixture!;
    }

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
    public void Fixture_ResolvesKnownAndAmbiguousAliasCases()
    {
        var fixture = LoadFixture();

        var known = _normalizer.Resolve(" HD ", fixture.KnownPlaces, fixture.Aliases);
        var ambiguous = _normalizer.Resolve("Frankfurt", fixture.KnownPlaces, fixture.Aliases);

        Assert.AreEqual(PlaceMatchKind.Known, known.Kind);
        Assert.AreEqual("Heidelberg", known.Normalized);
        Assert.AreEqual(PlaceMatchKind.Ambiguous, ambiguous.Kind);
        CollectionAssert.AreEquivalent(
            new[] { "Frankfurt am Main", "Frankfurt (Oder)" },
            ambiguous.Candidates.ToArray());
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
        var diagnostics = adapter.GetDiagnostics();
        Assert.AreEqual(1, diagnostics.CacheMisses);
        Assert.AreEqual(1, diagnostics.CacheHits);
        Assert.AreEqual(1, diagnostics.RemoteRequests);
        Assert.AreEqual(0, diagnostics.RateLimitRejections);
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
        var diagnostics = adapter.GetDiagnostics();
        Assert.AreEqual(2, diagnostics.CacheMisses);
        Assert.AreEqual(1, diagnostics.RemoteRequests);
        Assert.AreEqual(1, diagnostics.RateLimitRejections);
        Assert.AreEqual(TimeSpan.FromSeconds(40), diagnostics.LastRetryAfter);
    }

    [TestMethod]
    public async Task Fixture_ReportsOfflineMissWithoutInventingCoordinates()
    {
        var fixture = LoadFixture();
        var adapter = new CachingGeocodingAdapter(
            new OfflineGeocodingAdapter(fixture.Geocoding),
            TimeSpan.FromMinutes(10),
            TimeSpan.Zero);

        var result = await adapter.ResolveAsync("Mannheim");

        Assert.IsNull(result);
        var diagnostics = adapter.GetDiagnostics();
        Assert.AreEqual(1, diagnostics.CacheMisses);
        Assert.AreEqual(1, diagnostics.RemoteRequests);
    }

    [TestMethod]
    public async Task CachingAdapter_RefreshesExpiredFixtureEntry()
    {
        var fixture = LoadFixture();
        var inner = new CountingAdapter(fixture.Geocoding["Heidelberg"]);
        var clock = new ManualTimeProvider();
        var adapter = new CachingGeocodingAdapter(
            inner, TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(1), clock);

        await adapter.ResolveAsync("Heidelberg");
        clock.Advance(TimeSpan.FromMinutes(11));
        await adapter.ResolveAsync("Heidelberg");

        Assert.AreEqual(2, inner.CallCount);
        var diagnostics = adapter.GetDiagnostics();
        Assert.AreEqual(2, diagnostics.CacheMisses);
        Assert.AreEqual(0, diagnostics.CacheHits);
        Assert.AreEqual(2, diagnostics.RemoteRequests);
    }

    [TestMethod]
    public async Task CoordinateStore_NormalizesKeysAndReturnsSavedCoordinate()
    {
        var store = new InMemoryPlaceCoordinateStore();
        var coordinate = new PlaceCoordinate(
            "  Heidelberg ",
            49.3988,
            8.6724,
            "offline-fixture",
            false);

        await store.SaveAsync(coordinate);
        var result = await store.GetAsync("heidelberg");

        Assert.IsNotNull(result);
        Assert.AreEqual("Heidelberg", result!.Place);
        Assert.AreEqual(49.3988, result.Latitude);
        Assert.AreEqual("offline-fixture", result.Source);
    }

    [TestMethod]
    public async Task CoordinateStore_RejectsInvalidLatitude()
    {
        var store = new InMemoryPlaceCoordinateStore();

        ArgumentOutOfRangeException? thrown = null;
        try
        {
            await store.SaveAsync(new PlaceCoordinate("Heidelberg", 91, 8.6724, null, false));
        }
        catch (ArgumentOutOfRangeException exception)
        {
            thrown = exception;
        }

        Assert.IsNotNull(thrown);
    }

    [TestMethod]
    public void CoordinateSchemaReport_ControlsPersistenceByStatus()
    {
        var available = CoordinateSchemaReport.Create(CoordinateSchemaStatus.Available);
        var missing = CoordinateSchemaReport.Create(CoordinateSchemaStatus.Missing);
        var unverified = CoordinateSchemaReport.CreateUnverified("probe failed");

        Assert.IsTrue(available.CanPersist);
        Assert.IsFalse(missing.CanPersist);
        Assert.IsFalse(unverified.CanPersist);
        Assert.AreEqual("probe failed", unverified.Diagnostic);
        Assert.AreEqual("schema.probe_failed", unverified.DiagnosticCode);
        Assert.AreEqual("schema.available", available.DiagnosticCode);
        Assert.AreEqual("schema.missing_columns", missing.DiagnosticCode);
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

    private sealed record PlacesPolicyFixture(
        IReadOnlyList<string> KnownPlaces,
        IReadOnlyDictionary<string, IReadOnlyList<string>> Aliases,
        IReadOnlyDictionary<string, GeocodingResult> Geocoding);
}
