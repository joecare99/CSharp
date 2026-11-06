using Avalonia.Controls;
using Avalonia;

namespace Config.UI.Avalonia.Views;

/// <summary>
/// Displays reusable configuration section navigation and editing.
/// </summary>
public partial class ConfigUiView : UserControl
{
    public static readonly StyledProperty<bool> ShowSectionActionsProperty =
        AvaloniaProperty.Register<ConfigUiView, bool>(nameof(ShowSectionActions), true);

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigUiView"/> class.
    /// </summary>
    public ConfigUiView()
    {
        InitializeComponent();
        SectionActionsPanel.IsVisible = ShowSectionActions;
    }

    public bool ShowSectionActions
    {
        get => GetValue(ShowSectionActionsProperty);
        set
        {
            SetValue(ShowSectionActionsProperty, value);
            if (SectionActionsPanel is not null)
            {
                SectionActionsPanel.IsVisible = value;
            }
        }
    }
}
