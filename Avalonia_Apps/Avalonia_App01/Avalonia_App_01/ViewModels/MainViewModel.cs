using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalonia_App_01.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _greeting = "Welcome to Avalonia!";
}
