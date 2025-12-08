using CommunityToolkit.Mvvm.Input;
using Gen_FreeWin;
using GenFree.Data;
using System;
using System.ComponentModel;

namespace GenFree.ViewModels.Interfaces;

public interface IEventShowEditViewModel: INotifyPropertyChanged
{
    IRelayCommand ClickCommand { get; }
    string Display_Text { get; }
    EUserText Display_Hdr { get; }

    EEventArt EEvtArt { get; set; }
    int IPersNr { get; set; }
    Action<int> DataSchreib { get; set; }
    Action<int> DataZeig { get; set; }
    Action DoClick { get; set; }
}