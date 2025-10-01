using System;
using System.Collections.Generic;
using GenFreeBrowser.Map.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GenFreeBrowser.Map;

/// <summary>
/// Factory helpers to create preconfigured map providers (OSM, Bing, Google).
/// NOTE: Usage of some providers (Google, Bing) requires adhering to their Terms of Service.
/// These examples are for development/testing only. Provide your own API keys / parameters where required.
/// </summary>
public static class MapProviders
{
    public static MapProvider CreateOpenStreetMap()
    {
        var mp = new MapProvider("osm", "OpenStreetMap");
        mp.AddUrl(
            template: "https://%serv%.tile.openstreetmap.org/%z%/%x%/%y%.png",
            serverCount: 3,
            minZoom: 0,
            maxZoom: 19,
            serverFormatter: MapProvider.LetterServer
        );
        return mp;
    }

    public static MapProvider CreateBingRoads()
    {
        // Bing Roads layer (no API key appended here - add your key via query string if needed)
        var mp = new MapProvider("bing_roads", "Bing Roads");
        mp.AddUrl(
            template: "https://ecn.t%serv%.tiles.virtualearth.net/tiles/r%x%.png?g=0&n=z",
            serverCount: 4,
            minZoom: 0,
            maxZoom: 20,
            serverFormatter: id => id.ToString(),
            xFormatter: MapProvider.QuadKey,
            yFormatter: null,
            zFormatter: null
        );
        return mp;
    }

    public static MapProvider CreateBingAerial()
    {
        var mp = new MapProvider("bing_aerial", "Bing Aerial");
        mp.AddUrl(
            template: "https://ecn.t%serv%.tiles.virtualearth.net/tiles/a%x%.jpg?g=0&n=z",
            serverCount: 4,
            minZoom: 0,
            maxZoom: 20,
            serverFormatter: id => id.ToString(),
            xFormatter: MapProvider.QuadKey
        );
        return mp;
    }

    public static MapProvider CreateGoogleMaps()
    {
        // Google tile access may require proper usage of their APIs. This is illustrative only.
        var mp = new MapProvider("google_maps", "Google Maps");
        mp.AddUrl(
            template: "https://mt%serv%.google.com/vt/lyrs=m&x=%x%&y=%y%&z=%z%",
            serverCount: 4,
            minZoom: 0,
            maxZoom: 21,
            serverFormatter: id => id.ToString()
        );
        return mp;
    }

    public static MapProvider CreateGoogleSatellite()
    {
        var mp = new MapProvider("google_satellite", "Google Satellite");
        mp.AddUrl(
            template: "https://mt%serv%.google.com/vt/lyrs=s&x=%x%&y=%y%&z=%z%",
            serverCount: 4,
            minZoom: 0,
            maxZoom: 21,
            serverFormatter: id => id.ToString()
        );
        return mp;
    }
}

/// <summary>
/// Simple registry to hold available providers for binding / selection.
/// </summary>
public interface IMapProviderCatalog
{
    IReadOnlyList<IMapProvider> Providers { get; }
}

internal sealed class MapProviderCatalog : IMapProviderCatalog
{
    public IReadOnlyList<IMapProvider> Providers { get; }
    public MapProviderCatalog(IEnumerable<IMapProvider> providers)
    {
        Providers = new List<IMapProvider>(providers);
    }
}

public static class MapProviderServiceCollectionExtensions
{
    /// <summary>
    /// Registers a catalog containing the provided map providers (immutable list).
    /// </summary>
    public static IServiceCollection AddMapProviderCatalog(this IServiceCollection services, params IMapProvider[] providers)
    {
        services.AddSingleton<IMapProviderCatalog>(_ => new MapProviderCatalog(providers));
        return services;
    }
}
