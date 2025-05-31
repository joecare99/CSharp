using CommunityToolkit.Mvvm.Input;
using GenFree.Data;
using GenFree.Helper;
using GenFree.ViewModels.Interfaces;
using MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenFreeWpf.ViewModels;

public partial class MenueControlViewModel : BaseViewModelCT, IMenu1ViewModel
{
    // Beispiel-Implementierungen für Commands (können durch echte Logik ersetzt werden)
    public IRelayCommand FormClosingCommand { get; } = default!;
    public IRelayCommand FormLoadCommand { get; } = default!;
    public IRelayCommand OpenFamiliesCommand { get; } = default!;
    public IRelayCommand OpenPlacesCommand { get; } = default!;
    public IRelayCommand OpenPersonsCommand { get; } = default!;
    public IRelayCommand OpenSourcesCommand { get; } = default!;
    public IRelayCommand OpenMandantsCommand { get; } = default!;
    public IRelayCommand OpenPropertyCommand { get; } = default!;
    public IRelayCommand OpenConfigCommand { get; } = default!;
    public IRelayCommand OpenTextsCommand { get; } = default!;
    public IRelayCommand OpenPrintCommand { get; } = default!;
    public IRelayCommand OpenImportExportCommand { get; } = default!;
    public IRelayCommand OpenAddressCommand { get; } = default!;
    public IRelayCommand EndProgramCommand { get; } = default!;
    public IRelayCommand OpenCalculationsCommand { get; } = default!;
    public IRelayCommand OpenFunctionKeysCommand { get; } = default!;
    public IRelayCommand OpenCheckFamiliesCommand { get; } = default!;
    public IRelayCommand OpenCheckMissingCommand { get; } = default!;
    public IRelayCommand OpenCheckPersonsCommand { get; } = default!;
    public IRelayCommand OpenDuplettesCommand { get; } = default!;
    public IRelayCommand OpenNotesCommand { get; } = default!;
    public IRelayCommand OpenEnterLizenzCommand { get; } = default!;
    public IRelayCommand OpenReorgCommand { get; } = default!;
    public IRelayCommand OpenBackupReadCommand { get; } = default!;
    public IRelayCommand BackupWriteCommand { get; } = default!;
    public IRelayCommand OpenSendDataCommand { get; } = default!;
    public IRelayCommand OpenCheckUpdateCommand { get; } = default!;
    public IRelayCommand UpdateYesCommand { get; } = default!;
    public IRelayCommand UpdateNoCommand { get; } = default!;
    public IRelayCommand OpenRemoteDiagCommand { get; } = default!;

    // Beispiel-Implementierungen für Properties (können durch echte Logik ersetzt werden)
    public Color BackColor { get; } = default!;
    public Color OwnerBackColor { get; } = default!;
    public Color AddressBackColor { get; } = default!;
    public Color TrackBar1BackColor { get; } = default!;
    public Color MandantPathBackColor { get; } = default!;
    public Color FrmWindowSizeBackColor { get; } = default!;
    public bool CreationDateVisible { get; } = false;
    public bool MarkedVisible { get; } = false;
    public bool NotesVisible { get; } = false;
    public bool CodeOfArmsVisible { get; } = false;
    public bool ListBox2Visible { get; } = false;
    public bool List3Visible { get; } = false;
    public bool WarningVisible { get; } = false;
    public bool CheckUpdateVisible { get; } = false;
    public bool UpdateVisible { get; } = false;
    public bool DateTimePickerVisible { get; } = false;
    public bool SetDateVisible { get; } = false;
    public bool FrmWindowSizeVisible { get; } = false;
    public bool PbxLanguage1Visible { get; } = false;
    public bool PbxLanguage2Visible { get; } = false;
    public bool PbxLanguage3Visible { get; } = false;
    public string Notes { get; } = string.Empty;
    public string Mandant { get; } = string.Empty;
    public string MandantPath { get; } = string.Empty;
    public string HdrOwner { get; } = string.Empty;
    public string Owner { get; } = string.Empty;
    public string Menue18 { get; } = string.Empty;
    public string HdrProgName { get; } = string.Empty;
    public string HdrAdt { get; } = string.Empty;
    public string HdrCopyright { get; } = string.Empty;
    public string WarningText { get; } = string.Empty;
    public string AutoUpdState { get; } = string.Empty;
    public string CreationDate { get; } = string.Empty;
    public int TrackBar1Maximum { get; } = 0;
    public int TrackBar1Value { get; set; }
    public DateTime DateTimePicker1Value { get; } = DateTime.Now;
    public ObservableCollection<ListItem<(EEventArt, int)>> ListBox2Items { get; } = new();
    public ObservableCollection<ListItem<(bool, int)>> LstList3Items { get; } = new();
    public string LstList3Text { get; } = string.Empty;
    public string DateLastCheckText { get; } = string.Empty;
    public ListItem<(bool, int)> LstList3SelectedItem { get; } = default!;
    public IInteraction Interaction { get; set; } = default!;
    public Action<Enum> SetWindowState { set { } }
    public Func<Enum> GetWindowState { set { } }
    public Action<float> Grossaend { set { } }
    public IFraStatisticsViewModel Statistics { get; } = default!;
    public float FontSize { get; } = 12f;
    public Type AdresseType { get; set; } = typeof(object);

    // INotifyPropertyChanged wird bereits durch BaseViewModelCT implementiert
}
