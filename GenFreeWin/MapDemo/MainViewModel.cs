using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using GenFreeBrowser.Map;
using Microsoft.Extensions.DependencyInjection;

namespace MapDemo;

public sealed class MainViewModel : INotifyPropertyChanged
{
    private readonly IServiceProvider _sp;
    private IMapProvider _selectedProvider;
    private ITileSource _tileSource;

    public ViewportState Viewport { get; }
    public IMapProviderCatalog ProviderCatalog { get; }

    public IMapProvider SelectedProvider
    {
        get => _selectedProvider;
        set
        {
            if (_selectedProvider != value)
            {
                _selectedProvider = value;
                // recreate tile source for new provider
                _tileSource = new HttpTileSource(_selectedProvider, _sp.GetRequiredService<ITileCache>());
                OnPropertyChanged();
                OnPropertyChanged(nameof(TileSource));
            }
        }
    }

    public ITileSource TileSource => _tileSource;

    public ICommand CenterWorldCommand { get; }

    public MainViewModel(IServiceProvider sp, ViewportState viewport, IMapProviderCatalog catalog, IMapProvider initialProvider, ITileCache cache)
    {
        _sp = sp;
        Viewport = viewport;
        ProviderCatalog = catalog;
        _selectedProvider = initialProvider;
        _tileSource = new HttpTileSource(initialProvider, cache);
        Viewport.Center = new GeoPoint(9, 50);
        Viewport.Zoom = 6;
        CenterWorldCommand = new RelayCommand(_ => { Viewport.Center = new GeoPoint(9, 50); });
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public sealed class RelayCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Func<object?, bool>? _canExecute;

    public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

    public void Execute(object? parameter) => _execute(parameter);

    public event EventHandler? CanExecuteChanged;
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
