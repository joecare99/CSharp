using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RenderDemo.Pages;

public partial class LineBoundsPage : UserControl
{
    public LineBoundsPage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}

