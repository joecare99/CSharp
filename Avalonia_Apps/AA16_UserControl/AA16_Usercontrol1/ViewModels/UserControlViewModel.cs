using AA16_UserControl1.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AA16_UserControl1.ViewModels;

public partial class UserControlViewModel : ViewModelBase, IUserControlViewModel
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(Do1Command))]
    private string text = "<Ein Motto>";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(Do2Command))]
    private string daten = "<Daten>";

    public UserControlViewModel()
    {
    }

    bool CanDo1() => !string.IsNullOrEmpty(Daten);

    bool CanDo2() => !string.IsNullOrEmpty(Text);

    [RelayCommand(CanExecute = nameof(CanDo1))]
    private void Do1()
    {
        Text = "<Motto>";
        Daten = string.Empty;
    }

    [RelayCommand(CanExecute = nameof(CanDo2))]
    private void Do2()
    {
        Daten = "<Daten>";
        Text = string.Empty;
    }
}
