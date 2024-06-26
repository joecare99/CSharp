﻿using MVVM.ViewModel;

namespace MVVM_03_NotifyChange.ViewModels;

public class NotifyChangeViewModel : BaseViewModel
{
    #region Properties
    private string _firstname;
    private string _lastname;

    public string Firstname { get => _firstname; set => SetProperty(ref _firstname, value); }
    public string Lastname { get => _lastname; set => SetProperty(ref _lastname , value); }
    public string Fullname => $"{Lastname}, {Firstname}";
    #endregion

    #region Methods
    public NotifyChangeViewModel()
    {
        _firstname = "Dave";
        _lastname = "Dev";
        AddPropertyDependency(nameof(Fullname), nameof(Lastname));
        AddPropertyDependency(nameof(Fullname), nameof(Firstname));
    }
    #endregion

}
