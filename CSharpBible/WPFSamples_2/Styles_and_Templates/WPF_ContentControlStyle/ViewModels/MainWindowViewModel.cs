using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace ContentControlStyle.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private string contentText = "This is the content of the content control.";

    [RelayCommand]
    private void CheckContent()
    {
        if (!string.IsNullOrWhiteSpace(ContentText))
        {
            MessageBox.Show("ViewModel reports content present.");
        }
    }
}
