using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AA98_AvlnCodeStudio.UI.Views;

public partial class AICredentialsDialog : Window
{
    public AICredentialsDialog()
    {
        InitializeComponent();
    }

    private void OnCloseClick(object? sender, RoutedEventArgs eventArgs) => Close();
}
