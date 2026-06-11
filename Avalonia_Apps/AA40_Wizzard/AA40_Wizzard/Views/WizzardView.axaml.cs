using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AA40_Wizzard.Views;

/// <summary>
/// Displays the interactive wizard content.
/// </summary>
public partial class WizzardView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WizzardView"/> class.
    /// </summary>
    public WizzardView()
        => AvaloniaXamlLoader.Load(this);
}
