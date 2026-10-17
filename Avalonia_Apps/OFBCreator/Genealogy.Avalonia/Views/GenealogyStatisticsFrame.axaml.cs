using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Genealogy.Avalonia.Views;

/// <summary>
/// Reusable Avalonia view for displaying a genealogy statistics result.
/// </summary>
public partial class GenealogyStatisticsFrame : UserControl
{
    /// <summary>
    /// Initializes the statistics frame.
    /// </summary>
    public GenealogyStatisticsFrame()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
