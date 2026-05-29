using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RenderDemo.Pages;

public partial class DrawingPage : UserControl
{
    public DrawingPage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
