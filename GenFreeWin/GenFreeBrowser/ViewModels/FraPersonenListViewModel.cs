using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GenFreeBrowser.Model;
using GenFreeBrowser.ViewModels.Interfaces;
using MVVM.ViewModel;

namespace GenFreeBrowser.ViewModels;

public partial class FraPersonenListViewModel : BaseViewModelCT, IPersonenListViewModel
{
    private readonly IPersonenService _service;
    private readonly ObservableCollection<DispPersones> _personenMutable = new();
    public ReadOnlyObservableCollection<DispPersones> Personen { get; }

    [ObservableProperty]
    private bool istLeer;

    [ObservableProperty]
    private bool isBusy;

    // Filter / Paging Felder
    [ObservableProperty]
    private string? filterName;

    [ObservableProperty]
    private int? filterBirthYearFrom;

    [ObservableProperty]
    private int? filterBirthYearTo;

    [ObservableProperty]
    private int pageIndex;

    [ObservableProperty]
    private int pageSize = 50;

    [ObservableProperty]
    private int totalCount;

    public bool HasNextPage => (PageIndex + 1) * PageSize < TotalCount;
    public bool HasPreviousPage => PageIndex > 0;

    public FraPersonenListViewModel(IPersonenService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        Personen = new ReadOnlyObservableCollection<DispPersones>(_personenMutable);
        IstLeer = true;
    }

    partial void OnFilterNameChanged(string? value) => _ = ReloadFirstPageAsync();
    partial void OnFilterBirthYearFromChanged(int? value) => _ = ReloadFirstPageAsync();
    partial void OnFilterBirthYearToChanged(int? value) => _ = ReloadFirstPageAsync();
    partial void OnPageSizeChanged(int value) => _ = ReloadFirstPageAsync();

    partial void OnPageIndexChanged(int value)
    {
        OnPropertyChanged(nameof(HasNextPage));
        OnPropertyChanged(nameof(HasPreviousPage));
    }
    partial void OnTotalCountChanged(int value)
    {
        OnPropertyChanged(nameof(HasNextPage));
        OnPropertyChanged(nameof(HasPreviousPage));
    }

    private Task ReloadFirstPageAsync()
    {
        PageIndex = 0;
        return LadeAsync();
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    public async Task LadeAsync(CancellationToken ct = default)
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            _personenMutable.Clear();
            var (items, total) = await _service.QueryAsync(new PersonenQuery
            {
                NameContains = FilterName,
                BirthYearFrom = FilterBirthYearFrom,
                BirthYearTo = FilterBirthYearTo,
                PageIndex = PageIndex,
                PageSize = PageSize
            }, ct).ConfigureAwait(false);
            foreach (var p in items)
                _personenMutable.Add(p);
            TotalCount = total;
            IstLeer = _personenMutable.Count == 0;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    public async Task RefreshAsync(CancellationToken ct = default)
    {
        await LadeAsync(ct);
    }

    [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanGoNext))]
    public async Task NextPageAsync(CancellationToken ct = default)
    {
        if (!HasNextPage) return;
        PageIndex++;
        await LadeAsync(ct);
    }

    [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanGoPrevious))]
    public async Task PreviousPageAsync(CancellationToken ct = default)
    {
        if (!HasPreviousPage) return;
        PageIndex--;
        await LadeAsync(ct);
    }

    private bool CanGoNext() => HasNextPage && !IsBusy;
    private bool CanGoPrevious() => HasPreviousPage && !IsBusy;
}
