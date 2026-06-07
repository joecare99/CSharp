using System.Windows;
using System.Windows.Controls;
using AppWithPluginWpf.ViewModels;

namespace AppWithPluginWpf.Views;
/// <summary>
/// Interaktionslogik für TerminalView.xaml
/// </summary>
public partial class TerminalView : Page
{
    public TerminalView()
    {
        InitializeComponent();
        DataContext = (App.Current as App)!.GetService<TerminalViewModel>();
        if (DataContext is TerminalViewModel vm)
        {
            vm.DoShowMessage = message =>
            {
                MessageBox.Show(message, "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            };
        }
    }
}
