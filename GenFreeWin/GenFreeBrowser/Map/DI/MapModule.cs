using GenFreeBrowser.Map.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GenFreeBrowser.Map.DI;

public static class MapModule
{
    public static IServiceCollection AddMapViewer(this IServiceCollection services, string? cacheRoot = null)
    {
        cacheRoot ??= System.IO.Path.Combine(System.IO.Path.GetTempPath(), "MapTileCache");
        services.AddSingleton<ViewportState>();
        services.AddSingleton<ITileCache>(_ => new FileTileCache(cacheRoot));
        services.AddHttpClient();
        // Provider and tilesource are app specific, register factories
        return services;
    }
}
