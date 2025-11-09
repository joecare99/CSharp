using CommunityToolkit.Mvvm.ComponentModel;

namespace AA16_UserControl1.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel(UserControlViewModel content)
    {
        CurrentViewModel = content;
    }

    public ViewModelBase CurrentViewModel { get; }
}
