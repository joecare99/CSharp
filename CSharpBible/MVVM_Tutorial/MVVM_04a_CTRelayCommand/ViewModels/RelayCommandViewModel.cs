using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MVVM.ViewModel;

namespace MVVM_04a_CTRelayCommand.ViewModels;

public partial class RelayCommandViewModel : BaseViewModelCT
{
    #region Properties
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Fullname))]
    [NotifyCanExecuteChangedFor(nameof(ClearCommand))]
    private string _firstname="";
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Fullname))]
    [NotifyCanExecuteChangedFor(nameof(ClearCommand))]
    private string _lastname="";

    private bool _CanClear 
        => !string.IsNullOrEmpty(Firstname) 
        || !string.IsNullOrEmpty(Lastname);

    public string Fullname => $"{Lastname}, {Firstname}";
    #endregion

    #region Methods
    public RelayCommandViewModel()
    {
        _firstname = "Dave";
        _lastname = "Dev";
    }


    [RelayCommand(CanExecute = nameof(_CanClear))]
    private void Clear() 
        => (Firstname, Lastname) = ("", "");
    #endregion

}
