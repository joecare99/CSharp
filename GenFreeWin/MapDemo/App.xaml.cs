using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using GenFreeBrowser.Map;
using GenFreeBrowser.Map.DI;
using GenFreeBrowser.Map.Interfaces;

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

        sc.AddTransient<MainViewModel>();
        Services = sc.BuildServiceProvider();
    }
}
