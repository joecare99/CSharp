using GenFreeBrowser.Model;
using System.Collections.ObjectModel;

namespace GenFreeBrowser.ViewModels.Interfaces;

public interface IPersonenListViewModel
{
    ReadOnlyObservableCollection<DispPersones> Personen { get; }
    Task LadeAsync(CancellationToken ct = default);
    Task RefreshAsync(CancellationToken ct = default);
    bool IstLeer { get; }
    string? FilterName { get; set; }
    int? FilterBirthYearFrom { get; set; }
    int? FilterBirthYearTo { get; set; }
    int PageIndex { get; }
    int PageSize { get; set; }
    int TotalCount { get; }
    bool HasNextPage { get; }
    bool HasPreviousPage { get; }
    Task NextPageAsync(CancellationToken ct = default);
    Task PreviousPageAsync(CancellationToken ct = default);
}