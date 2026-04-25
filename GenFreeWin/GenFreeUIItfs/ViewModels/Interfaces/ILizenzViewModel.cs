using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel;

namespace GenFree.ViewModels.Interfaces;

public interface ILizenzViewModel: INotifyPropertyChanged
{
    string LicText1 { get; set; }
    string LicText2 { get; set; }
    string LicText3 { get; set; }
     
    bool DisplayHintVisible { get; }

    IRelayCommand ReqHintCommand { get; }
    IRelayCommand VerifyCommand { get; }
    IRelayCommand CancelCommand { get; }

    Action DoClose { set; }
    Action DoEndProg { set; }
    IInteraction Interaction { set; }
}