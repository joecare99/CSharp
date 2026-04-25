using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GenFreeBrowser.Map;
using GenFreeBrowser.Places.Interface;

namespace GenFreeBrowser.Places;

// GOV (Genealogy.net) place authority
// API docs: https://gov.genealogy.net/search/index
public sealed class GovAuthority : IPlaceAuthority
{
    private readonly HttpClient _http;
    public string Name => "GOV";

    public GovAuthority(HttpClient client) => _http = client;

    public async Task<IReadOnlyList<PlaceResult>> SearchAsync(PlaceQuery query, CancellationToken ct = default)
    {
        // Simple search endpoint returns HTML if not specifying JSON. Use format=json
        var url = $"https://gov.genealogy.net/api/searchByNameAndType?placename={Uri.EscapeDataString(query.Text)}";
        using var resp = await _http.GetAsync(url, ct);
        resp.EnsureSuccessStatusCode();
        using var stream = await resp.Content.ReadAsStreamAsync(ct);
        var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);
        var list = new List<PlaceResult>();
        if (doc.RootElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var loc in doc.RootElement.EnumerateArray())
            {
                // structure may vary; defensive parsing
                var id = loc.TryGetProperty("id", out var idProp) ? idProp.GetString() ?? "" : "";
                var name = loc.TryGetProperty("name", out var nameProp) && nameProp.ValueKind == JsonValueKind.Array ? nameProp.EnumerateArray().First().GetProperty("value").GetString() ?? "" : "";
                double lat = 0, lon = 0;
                if (loc.TryGetProperty("position", out var coords) && coords.ValueKind == JsonValueKind.Object)
                {
                    if (coords.TryGetProperty("lat", out var latProp) && latProp.TryGetDouble(out var dlat)) lat = dlat;
                    if (coords.TryGetProperty("lon", out var lonProp) && lonProp.TryGetDouble(out var dlon)) lon = dlon;
                }
                var hierarchy = new List<string>();
                if (loc.TryGetProperty("hierarchy", out var hProp) && hProp.ValueKind == JsonValueKind.Array)
                {
                    foreach (var h in hProp.EnumerateArray())
                    {
                        if (h.TryGetProperty("name", out var hName) && hName.GetString() is { } hStr && !string.IsNullOrWhiteSpace(hStr))
                            hierarchy.Add(hStr);
                    }
                }
                list.Add(new PlaceResult(id, name, new GeoPoint(lon, lat), hierarchy, Name));
            }
        }
        return list;
    }
}
