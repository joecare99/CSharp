using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GenFreeBrowser.Map;
using GenFreeBrowser.Places.Interface;

namespace GenFreeBrowser.Places;

public sealed class GeoNamesAuthority : IPlaceAuthority
{
    private readonly HttpClient _http;
    public string Name => "GeoNames";

    // Ermöglicht optionalen Benutzernamen; bei leer -> Fallback ("demo") oder lokaler Modus
    public GeoNamesAuthority(HttpClient httpClient)
    {
        _http = httpClient;
    }

    public async Task<IReadOnlyList<PlaceResult>> SearchAsync(PlaceQuery query, CancellationToken ct = default)
    {
        // Wenn gar kein echter Benutzername vorhanden ist und demo benutzt wird, geoNames limitiert stark.
        // Optional könnte hier ein alternativer Dienst (z.B. Nominatim) verwendet werden, wenn Limits greifen.
        var url = $"https://secure.geonames.org/searchJSON?q={Uri.EscapeDataString(query.Text)}&maxRows={query.MaxResults}&style=FULL";
        if (!string.IsNullOrWhiteSpace(query.Country))
            url += "&country=" + Uri.EscapeDataString(query.Country);

        try
        {
            using var resp = await _http.GetAsync(url, ct);
            if (!resp.IsSuccessStatusCode)
            {
                // Fallback: Bei Fehler (Quota / 401 / 403) keine Exception werfen, sondern leere Liste
                return Array.Empty<PlaceResult>();
            }
            using var stream = await resp.Content.ReadAsStreamAsync(ct);
            var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);
            var list = new List<PlaceResult>();
            if (doc.RootElement.TryGetProperty("geonames", out var arr))
            {
                foreach (var el in arr.EnumerateArray())
                {
                    var id = SafeGetInt(el, "geonameId");
                    var name = SafeGetString(el, "toponymName");
                    var lat = SafeGetDouble(el, "lat");
                    var lon = SafeGetDouble(el, "lng");
                    var countryName = SafeGetString(el, "countryName", true);
                    var admin1 = SafeGetString(el, "adminName1", true);
                    var admin2 = SafeGetString(el, "adminName2", true);
                    var admin3 = SafeGetString(el, "adminName3", true);
                    var hierarchy = new List<string>();
                    void Add(string? s) { if (!string.IsNullOrWhiteSpace(s)) hierarchy.Add(s!); }
                    Add(countryName); Add(admin1); Add(admin2); Add(admin3);
                    list.Add(new PlaceResult(id, name, new GeoPoint(lon, lat), hierarchy, Name));
                }
            }
            return list;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            // Netzwerkproblem -> leere Liste zurück
            return Array.Empty<PlaceResult>();
        }
    }

    private static string SafeGetString(JsonElement el, string prop, bool optional = false)
    {
        return el.TryGetProperty(prop, out var p) ? (p.GetString() ?? "") : (optional ? "" : "");
    }
    private static string SafeGetInt(JsonElement el, string prop)
    {
        if (el.TryGetProperty(prop, out var p))
        {
            if (p.ValueKind == JsonValueKind.Number && p.TryGetInt32(out var i))
                return i.ToString(CultureInfo.InvariantCulture);
            if (p.ValueKind == JsonValueKind.String && int.TryParse(p.GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var si))
                return si.ToString(CultureInfo.InvariantCulture);
        }
        return "0";
    }
    private static double SafeGetDouble(JsonElement el, string prop)
    {
        if (el.TryGetProperty(prop, out var p))
        {
            if (p.ValueKind == JsonValueKind.Number && p.TryGetDouble(out var d))
                return d;
            if (p.ValueKind == JsonValueKind.String && double.TryParse(p.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var sd))
                return sd;
        }
        return 0d;
    }
}
