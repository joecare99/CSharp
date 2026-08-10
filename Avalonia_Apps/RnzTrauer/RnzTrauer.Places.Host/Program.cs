using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using RnzTrauer.Places;

namespace RnzTrauer.Places.Host;

internal static class Program
{
    private static int Main(string[] args) => MainAsync(args).GetAwaiter().GetResult();

    private static async Task<int> MainAsync(string[] args)
    {
        if (args.Length == 0 || HasOption(args, "--help"))
        {
            Console.WriteLine("RNZ places host");
            Console.WriteLine("Usage: RnzTrauer.Places.Host --place <name> [--known <file>] [--geocode <json>] [--output <file>]");
            Console.WriteLine("The report includes geocoding cache and rate-limit diagnostics when --geocode is supplied.");
            return args.Length == 0 ? 2 : 0;
        }

        var place = GetOption(args, "--place");
        if (place is null)
        {
            Console.Error.WriteLine("Missing required option: --place");
            return 2;
        }

        var knownPath = GetOption(args, "--known");
        var known = knownPath is null
            ? Array.Empty<string>()
            : File.ReadAllLines(knownPath).Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();
        var result = new PlaceNormalizer().Resolve(place, known);
        var geocodePath = GetOption(args, "--geocode");
        GeocodingResult? geocode = null;
        GeocodingPolicyDiagnostics? policy = null;
        PlaceCoordinate? storedCoordinate = null;
        if (geocodePath is not null)
        {
            var entries = JsonSerializer.Deserialize<Dictionary<string, GeocodingResult>>(
                File.ReadAllText(geocodePath)) ?? new Dictionary<string, GeocodingResult>();
            var adapter = new CachingGeocodingAdapter(
                new OfflineGeocodingAdapter(entries),
                TimeSpan.FromMinutes(10),
                TimeSpan.FromMinutes(1));
            geocode = await adapter.ResolveAsync(place, CancellationToken.None);
            policy = adapter.GetDiagnostics();
            if (geocode?.Latitude is double latitude
                && geocode.Longitude is double longitude)
            {
                var coordinateStore = new InMemoryPlaceCoordinateStore();
                await coordinateStore.SaveAsync(new PlaceCoordinate(
                    geocode.Query,
                    latitude,
                    longitude,
                    "offline-geocoding",
                    geocode.IsApproximate));
                storedCoordinate = await coordinateStore.GetAsync(place);
            }
        }
        var report = new
        {
            Match = result,
            Geocode = geocode,
            Policy = policy,
            StoredCoordinate = storedCoordinate,
        };
        var json = JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true });
        var output = GetOption(args, "--output");
        if (output is null)
            Console.WriteLine(json);
        else
            File.WriteAllText(output, json);
        return 0;
    }

    private static bool HasOption(string[] args, string name) =>
        args.Any(argument => string.Equals(argument, name, StringComparison.OrdinalIgnoreCase));

    private static string? GetOption(string[] args, string name)
    {
        for (var index = 0; index < args.Length - 1; index++)
            if (string.Equals(args[index], name, StringComparison.OrdinalIgnoreCase))
                return args[index + 1];
        return null;
    }
}
