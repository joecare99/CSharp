using CommunityToolkit.Mvvm.Input;
using GenFree.Helper;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace GenFree.ViewModels.Interfaces;

public interface INamenSuchViewModel: INotifyPropertyChanged
{
    bool Male_Checked { get; set; }
    bool Females_Checked { get; set; }
    bool FamOnly_Checked { get; set; }
    bool Selection_Checked { get; set; }
    bool Male2_Checked { get; set; }
    bool Female2_Checked { get; set; }
    bool OmitSpouse_Checked { get; set; }

    IRelayCommand CloseCommand { get; }
    IRelayCommand PersonSheetCommand { get; }
    IRelayCommand FamilySheetCommand { get; }
    IRelayCommand StartSearchCommand { get; }
    IRelayCommand PrintListCommand { get; }
    IRelayCommand ReqHintCommand { get; }
    int PersNr { get; set; }
    int FamNr { get; set; }

    bool xComboBox2AddT308 { get; }
    bool xComboBox2AddT309 { get; }
    ObservableCollection<ListItem<int>> Label1_Text { get; }
    ObservableCollection<ListItem<int>> Label5_Text { get; }
    ObservableCollection<ListItem<int>> Label7_Text { get; }
    Action DoHide { get; set; }
}