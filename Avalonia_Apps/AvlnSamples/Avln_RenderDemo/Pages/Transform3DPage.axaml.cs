using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using RenderDemo.ViewModels;

namespace RenderDemo.Pages;

public partial class Transform3DPage : UserControl
{
    public Transform3DPage()
    {
        InitializeComponent();
        this.DataContext = new Transform3DPageViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}

