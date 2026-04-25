using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GenFree.Data;
using GenFree.Helper;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;

namespace GenFree.ViewModels.Interfaces;

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
    IRelayCommand UpdateYesCommand { get; }
    IRelayCommand UpdateNoCommand { get; }
    IRelayCommand OpenRemoteDiagCommand { get; }

    Color BackColor { get; }
    Color OwnerBackColor { get; }
    Color AddressBackColor { get; }
    Color TrackBar1BackColor { get; }
    Color MandantPathBackColor { get; }
    Color FrmWindowSizeBackColor { get; }

    bool CreationDateVisible { get;  }
    bool MarkedVisible { get;  }
    bool NotesVisible { get; }
    bool CodeOfArmsVisible { get; }
    bool ListBox2Visible { get; }
    bool List3Visible { get; }
    bool WarningVisible { get; }
    bool CheckUpdateVisible { get; }
    bool UpdateVisible { get; }
    bool DateTimePickerVisible { get; }
    bool SetDateVisible { get; }
    bool FrmWindowSizeVisible { get; }
    bool PbxLanguage1Visible { get; }
    bool PbxLanguage2Visible { get; }
    bool PbxLanguage3Visible { get; }


    string Notes { get; }
    string Mandant { get; }
    string MandantPath { get; }
    string HdrOwner { get; }
    string Owner { get; }
    string Menue18 { get; }
    string HdrProgName { get; }
    string HdrAdt { get; }
    string HdrCopyright { get; }
    string WarningText { get; }
    string AutoUpdState { get; }
    string CreationDate { get; }
    int TrackBar1Maximum { get; }
    int TrackBar1Value { get; set; }
    DateTime DateTimePicker1Value { get; }

    ObservableCollection<ListItem<(EEventArt, int)>> ListBox2Items { get; }
    ObservableCollection<ListItem<(bool, int)>> LstList3Items { get; }
    string LstList3Text { get; }
    string DateLastCheckText { get; }
    ListItem<(bool, int)> LstList3SelectedItem { get; }

    IInteraction Interaction { get; set; }
    public Action<Enum> SetWindowState { set; }
    public Func<Enum> GetWindowState { set; }

    Action<float> Grossaend { set; }
    IFraStatisticsViewModel Statistics { get; }
    float FontSize { get; }
    Type AdresseType { get; set; }
}