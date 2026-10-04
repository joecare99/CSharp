using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace OFBCreator.Avalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow() => AvaloniaXamlLoader.Load(this);
}
