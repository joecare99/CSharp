using CommunityToolkit.Mvvm.ComponentModel;

namespace AA16_UserControl1.ViewModels;

public partial class CurrencyViewViewModel : ViewModelBase
{
    [ObservableProperty]
    private decimal value = 10m;
}
