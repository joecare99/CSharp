using Avalonia.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using Avln_Hello_World.ViewModels;

namespace Avln_Hello_World.Views;

/// <summary>
/// Interaction logic for HelloWorldView.axaml
/// </summary>
public partial class HelloWorldView : UserControl
{
    public HelloWorldView()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetService<HelloWorldViewModel>();
    }
}
