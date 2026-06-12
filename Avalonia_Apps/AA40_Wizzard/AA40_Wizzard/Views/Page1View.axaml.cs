using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AA40_Wizzard.Views;

/// <summary>
/// Displays the first wizard page.
/// </summary>
public partial class Page1View : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Page1View"/> class.
    /// </summary>
    public Page1View()
        => AvaloniaXamlLoader.Load(this);
}
