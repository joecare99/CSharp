using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Forms;

namespace Gen_FreeWin.ViewModels.Interfaces;

public interface IHGAkteViewModel
{
    IContainerControl View { get; set; }
    Action<FormWindowState, float> View_InitView { get; set; }
    Action DoClose { get; set; }

    IRelayCommand MainMenueCommand { get; }
    IRelayCommand BackCommand { get; }
    IRelayCommand ShowUsageCommand { get; }
    IRelayCommand NextEntryCommand { get; }
    IRelayCommand PrevEntryCommand { get; }
    IRelayCommand SearchCommand { get; }
    IRelayCommand EnterNew2Command { get; }
    IRelayCommand CloseUsageCommand { get; }
    IRelayCommand EditEntryCommand { get; }
    IRelayCommand NewEntryCommand { get; }
    IRelayCommand CancelEntryCommand { get; }

    bool Usage_Visible { get; }
    bool Frame1_Visible { get; }

    string Number_Text { get; set; }
    string Place_Text { get; set; }
    string Union_Text { get; set; }
    string Class_Text { get; set; }
    string FireInsurance_Text { get; set; }
    string Additional_Text { get; set; }
    string Flurstueck_Text { get; set; }
    string Parzelle_Text { get; set; }
    void Form_Load(object sender, EventArgs e);
}