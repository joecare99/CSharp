using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalonia_App_01.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial string Greeting { get; set; } = "Welcome to Avalonia!";
}
