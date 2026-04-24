using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace MapDemo;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<MainViewModel>();
    }
}
