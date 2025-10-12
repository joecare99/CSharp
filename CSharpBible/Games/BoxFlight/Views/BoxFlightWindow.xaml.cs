using System.Windows;
using BoxFlight.ViewModels;

namespace BoxFlight.Views;

public partial class BoxFlightWindow : Window
{
    public BoxFlightWindow(IBoxFlightViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}