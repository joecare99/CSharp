using System.Collections.ObjectModel;
using System.Windows.Input;

namespace TraceAnalysis.Workbench.Wpf.ViewModels;

/// <summary>
/// Represents one menu item within the workbench main menu.
/// </summary>
public sealed class MenuItemViewModel : ViewModelBase
{
    private bool _isVisible;
    private bool _isEnabled;
    private MenuCommandKind _commandKind;
    private MenuContextScope _contextScope;

    public MenuItemViewModel(
        string header,
        ICommand? command = null,
        MenuCommandKind commandKind = MenuCommandKind.General,
        MenuContextScope contextScope = MenuContextScope.Global)
    {
        Header = header;
        Command = command;
        _commandKind = commandKind;
        _contextScope = contextScope;
        _isVisible = true;
        _isEnabled = true;
        Items = new ObservableCollection<MenuItemViewModel>();
    }

    /// <summary>
    /// Gets the menu header text.
    /// </summary>
    public string Header { get; }

    /// <summary>
    /// Gets the bound command.
    /// </summary>
    public ICommand? Command { get; }

    /// <summary>
    /// Gets the child menu items.
    /// </summary>
    public ObservableCollection<MenuItemViewModel> Items { get; }

    /// <summary>
    /// Gets the command kind.
    /// </summary>
    public MenuCommandKind CommandKind
    {
        get => _commandKind;
        set => SetProperty(ref _commandKind, value);
    }

    /// <summary>
    /// Gets the menu context scope.
    /// </summary>
    public MenuContextScope ContextScope
    {
        get => _contextScope;
        set => SetProperty(ref _contextScope, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the item is visible.
    /// </summary>
    public bool IsVisible
    {
        get => _isVisible;
        set => SetProperty(ref _isVisible, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the item is enabled.
    /// </summary>
    public bool IsEnabled
    {
        get => _isEnabled;
        set => SetProperty(ref _isEnabled, value);
    }
}
