using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using GenFreeBrowser.Map;
using GenFreeBrowser.Map.DI;
using GenFreeBrowser.Map.Interfaces;
using GenFreeBrowser.Places;
using GenFreeBrowser.Places.Interface;

namespace MapDemo;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var sc = new ServiceCollection();
        sc.AddMapViewer();

        // Register providers
        var osm = MapProviders.CreateOpenStreetMap();
        var bing = MapProviders.CreateBingRoads();
        var bingareal = MapProviders.CreateBingAerial();
        var google = MapProviders.CreateGoogleMaps();
        sc.AddMapProviderCatalog(bing, bingareal, osm, google);

        // Active provider state + tilesource factory
        sc.AddSingleton<IMapProvider>(bingareal); // default
        sc.AddSingleton<ITileSource>(sp => new HttpTileSource(sp.GetRequiredService<IMapProvider>(), sp.GetRequiredService<ITileCache>()));

        // Place Authorities
        sc.AddHttpClient<GeoNamesAuthority>();
        sc.AddHttpClient<GovAuthority>();
        sc.AddSingleton<IPlaceAuthority>(sp => new GeoNamesAuthority(sp.GetRequiredService<System.Net.Http.IHttpClientFactory>().CreateClient(nameof(GeoNamesAuthority)))); // TODO: username config
        sc.AddSingleton<IPlaceAuthority>(sp => new GovAuthority(sp.GetRequiredService<System.Net.Http.IHttpClientFactory>().CreateClient(nameof(GovAuthority))));
        sc.AddSingleton<IPlaceSearchService, PlaceSearchService>();

        sc.AddTransient<MainViewModel>();
        Services = sc.BuildServiceProvider();
    }
}
