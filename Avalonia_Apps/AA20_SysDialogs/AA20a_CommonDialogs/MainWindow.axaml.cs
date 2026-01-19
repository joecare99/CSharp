using Avalonia.Controls;

namespace AA20a_CommonDialogs;

public sealed partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel vm, TopLevelAccessor tla)
    {
        InitializeComponent();
        DataContext = vm;
        tla.Current = this;
    }
}
