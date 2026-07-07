using System.Collections.ObjectModel;

namespace AA98_AvlnCodeStudio.Planning.UI.ViewModels;

/// <summary>
/// Represents a category group in the planning explorer category view.
/// </summary>
public sealed class PlanningCategoryGroupViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlanningCategoryGroupViewModel"/> class.
    /// </summary>
    /// <param name="name">The displayed category name.</param>
    /// <param name="items">The grouped items.</param>
    public PlanningCategoryGroupViewModel(string name, ObservableCollection<PlanningTreeItemViewModel> items)
    {
        Name = name;
        Items = items;
    }

    /// <summary>
    /// Gets the category display name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the items of the category.
    /// </summary>
    public ObservableCollection<PlanningTreeItemViewModel> Items { get; }
}
