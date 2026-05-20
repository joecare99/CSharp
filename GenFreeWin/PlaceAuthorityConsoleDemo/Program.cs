using System;
using System.Threading.Tasks;
using BaseLib.Interfaces;
using BaseLib.Models;
using ConsoleLib;
using ConsoleLib.Interfaces;
using GenFreeBrowser.Places;
using GenFreeBrowser.Places.Interface;
using GenInterfaces.Interfaces.Authorities;
using Microsoft.Extensions.DependencyInjection;

namespace PlaceAuthorityConsoleDemo;

class Program
{
    private static ServiceProvider _provider = null!;

    static async Task Main(string[] args)
    {
        ConfigureServices();
        var view = _provider.GetRequiredService<ConsoleSearchView>();
        view.Run();
    }

    private static void ConfigureServices()
    {
        var services = new ServiceCollection();
        services.AddHttpClient();
        services.AddSingleton<IConsole, ConsoleProxy>();
        services.AddSingleton<IExtendedConsole, ExtendedConsole>();
        services.AddSingleton<ISearchHistoryService, SearchHistoryService>();
        services.AddSingleton<IGenPlaceAuthority>(sp => new NominatimAuthority(sp.GetRequiredService<IHttpClientFactory>().CreateClient()));
        services.AddSingleton<IGenPlaceAuthority>(sp => new GovAuthority(sp.GetRequiredService<IHttpClientFactory>().CreateClient()));
        services.AddSingleton<IPlaceSearchService, PlaceSearchService>();
        services.AddSingleton<SearchViewModel>();
        services.AddTransient<ConsoleAppViewModel>();
        services.AddSingleton<ConsoleSearchView>();
        _provider = services.BuildServiceProvider();
    }
}
