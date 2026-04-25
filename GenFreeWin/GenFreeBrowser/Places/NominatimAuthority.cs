using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GenFreeBrowser.Map;
using GenFreeBrowser.Places.Interface;

namespace GenFreeBrowser.Places;

/// <summary>
/// Nominatim (OpenStreetMap) implementation of <see cref="IPlaceAuthority"/>.
/// Usage policy: https://operations.osmfoundation.org/policies/nominatim/
/// Provide a descriptive User-Agent and (optional) contact e-mail.
/// </summary>
public sealed class NominatimAuthority : IPlaceAuthority
{
    private readonly HttpClient _http;
    private readonly string _userAgent;
    private readonly string? _email;
    public string Name => "Nominatim";

    public NominatimAuthority(HttpClient httpClient, string? userAgent = null, string? email = null)
    {
        _http = httpClient;
        _userAgent = string.IsNullOrWhiteSpace(userAgent) ? "GenFreeBrowser/1.0 (+https://example.invalid)" : userAgent.Trim();
        _email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();

        // Ensure headers (Nominatim policy requires valid UA)
        if (!_http.DefaultRequestHeaders.UserAgent.Any())
        {
            try { _http.DefaultRequestHeaders.UserAgent.ParseAdd(_userAgent); } catch { /* ignore */ }
        }
        if (_email is not null && !_http.DefaultRequestHeaders.Contains("From"))
        {
            try { _http.DefaultRequestHeaders.Add("From", _email); } catch { /* ignore */ }
        }
    }

    public async Task<IReadOnlyList<PlaceResult>> SearchAsync(PlaceQuery query, CancellationToken ct = default)
    {
        // Build request URL
        var url = $"https://nominatim.openstreetmap.org/search?format=jsonv2&addressdetails=1&limit={query.MaxResults}&q={Uri.EscapeDataString(query.Text)}";
        if (!string.IsNullOrWhiteSpace(query.Country))
        {
            // Nominatim expects 2-letter country codes (lowercase) in countrycodes parameter
            url += "&countrycodes=" + Uri.EscapeDataString(query.Country.ToLowerInvariant());
        }
        // Accept-language (best effort)
        var lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        url += "&accept-language=" + Uri.EscapeDataString(lang);

        try
        {
            using var resp = await _http.GetAsync(url, ct);
            if (!resp.IsSuccessStatusCode)
                return Array.Empty<PlaceResult>();

            await using var stream = await resp.Content.ReadAsStreamAsync(ct);
            var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);
            if (doc.RootElement.ValueKind != JsonValueKind.Array)
                return Array.Empty<PlaceResult>();

            var list = new List<PlaceResult>();
            foreach (var el in doc.RootElement.EnumerateArray())
            {
                var id = SafeGetString(el, "place_id");
                var name = FirstNonEmpty(
                    SafeGetString(el, "name", true),
                    SafeGetString(el, "display_name", true),
                    "(unknown)"
                );
                var lat = SafeGetDouble(el, "lat");
                var lon = SafeGetDouble(el, "lon");

                var hierarchy = BuildHierarchy(el);
                list.Add(new PlaceResult(id, name, new GeoPoint(lon, lat), hierarchy, Name));
            }
            return list;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            return Array.Empty<PlaceResult>();
        }
    }

    private static IReadOnlyList<string> BuildHierarchy(JsonElement el)
    {
        var list = new List<string>();
        if (el.TryGetProperty("address", out var addr) && addr.ValueKind == JsonValueKind.Object)
        {
            // Ordered keys (coarse -> fine). Add distinct non-empty values.
            string[] keys =
            [
                "country",
                "state",
                "region",
                "province",
                "county",
                "municipality",
                "city",
                "town",
                "village",
                "hamlet",
                "suburb",
                "neighbourhood"
            ];
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var k in keys)
            {
                if (addr.TryGetProperty(k, out var v) && v.GetString() is { } s && !string.IsNullOrWhiteSpace(s))
                {
                    if (seen.Add(s)) list.Add(s);
                }
            }
        }
        return list;
    }

    private static string FirstNonEmpty(params string?[] values)
    {
        foreach (var v in values)
            if (!string.IsNullOrWhiteSpace(v)) return v!;
        return string.Empty;
    }

    private static string SafeGetString(JsonElement el, string prop, bool optional = false)
    {
        if (el.TryGetProperty(prop, out var p))
        {
            return p.ValueKind switch
            {
                JsonValueKind.String => p.GetString() ?? string.Empty,
                JsonValueKind.Number => p.TryGetInt64(out var i) ? i.ToString(CultureInfo.InvariantCulture) : p.GetRawText(),
                _ => string.Empty
            };
        }
        return optional ? string.Empty : string.Empty;
    }

    private static double SafeGetDouble(JsonElement el, string prop)
    {
        if (el.TryGetProperty(prop, out var p))
        {
            if (p.ValueKind == JsonValueKind.Number && p.TryGetDouble(out var d)) return d;
            if (p.ValueKind == JsonValueKind.String && double.TryParse(p.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var ds)) return ds;
        }
        return 0d;
    }
}
