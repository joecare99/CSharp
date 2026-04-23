using System;
using GenFreeBrowser.Map;
using GenFreeBrowser.Map.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MapDemo;

public sealed partial class MainViewModel : ObservableObject
{
    private readonly IServiceProvider _sp;

    [ObservableProperty]
    private IMapProvider _selectedProvider;

    private ITileSource _tileSource;

    public ViewportState Viewport { get; }
    public IMapProviderCatalog ProviderCatalog { get; }

    public ITileSource TileSource => _tileSource;

    public MainViewModel(IServiceProvider sp, ViewportState viewport, IMapProviderCatalog catalog, IMapProvider initialProvider, ITileCache cache)
    {
        _sp = sp;
        Viewport = viewport;
        ProviderCatalog = catalog;
        _selectedProvider = initialProvider;
        _tileSource = new HttpTileSource(initialProvider, cache);
        Viewport.Center = new GeoPoint(9, 50);
        Viewport.Zoom = 6;
    }

    partial void OnSelectedProviderChanged(IMapProvider value)
    {
        _tileSource = new HttpTileSource(value, _sp.GetRequiredService<ITileCache>());
        OnPropertyChanged(nameof(TileSource));
    }

    [RelayCommand]
    private void CenterWorld()
    {
        Viewport.Center = new GeoPoint(9, 50);
    }
}
