using CommunityToolkit.Mvvm.Input;
using GenFree.Helper;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Forms;

namespace GenFree.ViewModels.Interfaces;

public interface INamenSuchViewModel: INotifyPropertyChanged
{
    IContainerControl View { get; set; }
    int PersNr { get; set; }
    int FamNr { get; set; }
    int FamPerschalt { get; set; }
    string Text1_Text { get; set; }
    string Text2_Text { get; set; }
    bool Male_Checked { get; set; }
    bool Females_Checked { get; set; }
    bool FamOnly_Checked { get; set; }
    bool Selection_Checked { get; set; }
    bool Male2_Checked { get; set; }
    bool Female2_Checked { get; set; }
    bool OmitSpouse_Checked { get; set; }

    IRelayCommand FormLoadCommand { get; }
    IRelayCommand FormClosedCommand { get; }
    IRelayCommand CloseCommand { get; }
    IRelayCommand PersonSheetCommand { get; }
    IRelayCommand FamilySheetCommand { get; }
    IRelayCommand StartSearchCommand { get; }
    IRelayCommand PrintListCommand { get; }
    IRelayCommand ReqHintCommand { get; }
    IRelayCommand<int> Label6_DoubleClickCommand { get; }
    IRelayCommand<int> Label5_DoubleClickCommand { get; }

    bool xComboBox2AddT308 { get; }
    bool xComboBox2AddT309 { get; }
    ObservableCollection<ListItem<int>> Label1_Text { get; }
    ObservableCollection<ListItem<int>> Label5_Text { get; }
    ObservableCollection<ListItem<int>> Label7_Text { get; }
    Action DoHide { get; set; }

    void SetPerson(int value, int an, short schalt);
    void ShowNamensuchDlg(string title, int personNr, int iFamNr);
}