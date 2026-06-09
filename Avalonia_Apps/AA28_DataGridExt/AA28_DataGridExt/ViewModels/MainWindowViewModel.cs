using Avalonia.ViewModels;
using BaseLib.Helper;

namespace AA28_DataGridExt.ViewModels;

/// <summary>
/// Provides the root screen state for desktop and browser hosts.
/// </summary>
public class MainWindowViewModel : BaseViewModelCT
{
    /// <summary>
    /// Gets the hosted data grid screen.
    /// </summary>
    public DataGridViewModel DataGrid { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    public MainWindowViewModel()
        : this(IoC.GetRequiredService<DataGridViewModel>())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    /// <param name="dataGrid">The hosted data grid view model.</param>
    public MainWindowViewModel(DataGridViewModel dataGrid)
    {
        DataGrid = dataGrid;
    }
}
