using Avalonia.Controls;
using AA19_FilterLists.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace AA19_FilterLists.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<MainWindowViewModel>();
    }
}
