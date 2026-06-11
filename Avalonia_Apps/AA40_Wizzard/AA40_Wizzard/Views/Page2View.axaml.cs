using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AA40_Wizzard.Views;

/// <summary>
/// Displays the second wizard page.
/// </summary>
public partial class Page2View : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Page2View"/> class.
    /// </summary>
    public Page2View()
        => AvaloniaXamlLoader.Load(this);
}
