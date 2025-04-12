using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gen_FreeWin.ViewModels.Interfaces;
using MVVM.ViewModel;

namespace GenFreeWin.ViewModels;

public partial class AdresseViewModel : BaseViewModelCT, IAdresseViewModel
{
    [ObservableProperty]
    private string _title;
    [ObservableProperty]
    private string _givenname;
    [ObservableProperty]
    private string _surname;
    [ObservableProperty]
    private string _street;
    [ObservableProperty]
    private string _zip;
    [ObservableProperty]
    private string _place;
    [ObservableProperty]
    private string _phone;
    [ObservableProperty]
    private string _eMail;
    [ObservableProperty]
    private string _special;

    [RelayCommand]
    private void Save() { 
    }
    [RelayCommand] 
    private void FormLoad() { 
    }
}
