using Avalonia.Controls;
using ConsoleLib.Cxaml.Designer.ViewModels;

namespace ConsoleLib.Cxaml.Designer;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new DesignerViewModel();
    }
}
