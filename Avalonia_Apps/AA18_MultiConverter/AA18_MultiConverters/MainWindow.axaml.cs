using Avalonia.Controls;

namespace AA18_MultiConverter;

public partial class MainWindow : Window
{
    public MainWindow(Views.DateDifView view)
    {
        InitializeComponent();
        Content = view;
    }
}
