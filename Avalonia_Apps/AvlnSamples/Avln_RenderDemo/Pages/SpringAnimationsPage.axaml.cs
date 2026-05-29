using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RenderDemo.Pages;

public partial class SpringAnimationsPage : UserControl
{
    public SpringAnimationsPage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
