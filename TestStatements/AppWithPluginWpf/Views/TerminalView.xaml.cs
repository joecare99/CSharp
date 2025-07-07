using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
        DataContext = (App.Current as App).GetService<TerminalViewModel>();
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
