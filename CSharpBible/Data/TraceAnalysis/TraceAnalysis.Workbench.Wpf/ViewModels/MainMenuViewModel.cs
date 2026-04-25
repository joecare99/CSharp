using System.Collections.ObjectModel;

namespace TraceAnalysis.Workbench.Wpf.ViewModels;

/// <summary>
/// Represents the shell-level main menu state.
/// </summary>
public sealed class MainMenuViewModel
{
    public MainMenuViewModel()
    {
        Items = new ObservableCollection<MenuItemViewModel>();
    }

    /// <summary>
    /// Gets the top-level menu items.
    /// </summary>
    public ObservableCollection<MenuItemViewModel> Items { get; }
}
