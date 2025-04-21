using CommunityToolkit.Mvvm.Input;
using GenFree.ViewModels.Interfaces;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;

namespace Gen_FreeWin.ViewModels.Interfaces;

public interface IMenu1ViewModel : INotifyPropertyChanged
{
    IRelayCommand FormClosingCommand { get; }
    IRelayCommand FormLoadCommand { get; }
    IRelayCommand OpenFamiliesCommand { get; }
    IRelayCommand OpenPlacesCommand { get; }
    IRelayCommand OpenPersonsCommand { get; }
    IRelayCommand OpenSourcesCommand { get; }
    IRelayCommand OpenMandantsCommand { get; }
    IRelayCommand OpenPropertyCommand { get; }
    IRelayCommand OpenConfigCommand { get; }
    IRelayCommand OpenTextsCommand { get; }
    IRelayCommand OpenPrintCommand { get; }
    IRelayCommand OpenImportExportCommand { get; }
    IRelayCommand OpenAddressCommand { get; }
    IRelayCommand EndProgramCommand { get; }
    IRelayCommand OpenCalculationsCommand { get; }
    IRelayCommand OpenFunctionKeysCommand { get; }
    IRelayCommand OpenCheckFamiliesCommand { get; }
    IRelayCommand OpenCheckMissingCommand { get; }
    IRelayCommand OpenCheckPersonsCommand { get; }
    IRelayCommand OpenDuplettesCommand { get; }
    IRelayCommand OpenNotesCommand { get; }
    IRelayCommand OpenEnterLizenzCommand { get; }
    IRelayCommand OpenReorgCommand { get; }
    IRelayCommand OpenBackupReadCommand { get; }
    IRelayCommand BackupWriteCommand { get; }
    IRelayCommand OpenSendDataCommand { get; }
    IRelayCommand OpenCheckUpdateCommand { get; }
    IRelayCommand OpenRemoteDiagCommand { get; }

    bool CreationDateVisible { get;  }
    bool MarkedVisible { get;  }
    bool NotesVisible { get; }
    bool CodeOfArmsVisible { get; }
    bool ListBox2Visible { get; }
    bool List3Visible { get; }
    bool WarningVisible { get; }
    bool EnterLizenzVisible { get; }

    string Notes { get; set; }
    string Mandant { get; }
    string MandantPath { get; }
    string HdrOwner { get; }
    string Owner { get; }
    string Menue18 { get; }
    string HdrProgName { get; }
    string HdrAdt { get; }
    string HdrCopyright { get; }
    string WarningText { get; }
    IInteraction Interaction { get; set; }
    public Action<Enum> SetWindowState { set; }
    public Func<Enum> GetWindowState { set; }
    Action<float> Grossaend { set; }
    IFraStatisticsViewModel Statistics { get; }
    float FontSize { get; }
}