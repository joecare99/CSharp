using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AA98_AvlnCodeStudio.Planning.UI.Controls;

/// <summary>
/// Hosts a generic properties list for objects exposing IHasProperties.
/// </summary>
public partial class PropertiesPanel : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PropertiesPanel"/> class.
    /// </summary>
    public PropertiesPanel()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
