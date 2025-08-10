using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseLib.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces.UI;
using GenFree.ViewModels.Interfaces;
using MVVM.ViewModel;

namespace GenFreeWin.ViewModels;

public partial class MenueViewModel : BaseViewModelCT, IMenu1ViewModel
{
    private IFraStatisticsViewModel _statistics;
    private IMessenger _messenger;

    #region Properties
    [ObservableProperty]
    Color _backColor;
    [ObservableProperty]
    Color _ownerBackColor;
    [ObservableProperty]
    Color _addressBackColor;
    [ObservableProperty]
    Color _trackBar1BackColor;
    [ObservableProperty]
    Color _mandantPathBackColor;
    [ObservableProperty]
    Color _frmWindowSizeBackColor;

    [ObservableProperty]
    string _notes;
    [ObservableProperty]
    string _mandant = "Some Mandant";
    [ObservableProperty]
    string _mandantPath = "";
    [ObservableProperty]
    string _hdrOwner = "Me";
    [ObservableProperty]
    string _owner = "<Owner>";
    [ObservableProperty]
    string _menue18 = "Menue18";
    [ObservableProperty]
    string _hdrProgName = "<HdrProgName>";
    [ObservableProperty]
    string _hdrAdt = "<HdrAdt>";
    [ObservableProperty]
    string _hdrCopyright = "(c) by Joe Care 2025";
    [ObservableProperty]
    string _warningText = "a Warning !";

    public IInteraction Interaction { get; set; }
    public Action<Enum> SetWindowState { get; set; }
    public Func<Enum> GetWindowState { get; set; }
    public Action<float> Grossaend { get; set; }

    public float FontSize { get; set; } = 12f;
    public IFraStatisticsViewModel Statistics => _statistics;

    [ObservableProperty]
    public Type _adresseType;

    [ObservableProperty]
    private bool _creationDateVisible;
    [ObservableProperty]
    private bool _markedVisible;
    [ObservableProperty]
    bool _notesVisible;
    [ObservableProperty]
    bool _codeOfArmsVisible;
    [ObservableProperty]
    bool _listBox2Visible;
    [ObservableProperty]
    bool _list3Visible;
    [ObservableProperty]
    bool _warningVisible;
    [ObservableProperty]
    bool _enterLizenzVisible;
    [ObservableProperty]
    private bool _CheckUpdateVisible;
    [ObservableProperty]
    private bool _UpdateVisible;
    [ObservableProperty]
    private bool _DateTimePickerVisible;
    [ObservableProperty]
    private bool _SetDateVisible;
    [ObservableProperty]
    private bool _FrmWindowSizeVisible;
    [ObservableProperty]
    private bool _PbxLanguage1Visible;
    [ObservableProperty]
    private bool _PbxLanguage2Visible;
    [ObservableProperty]
    private bool _PbxLanguage3Visible;

    [ObservableProperty]
    private string _AutoUpdState;

    [ObservableProperty]
    private string _CreationDate;

    [ObservableProperty]
    private int _TrackBar1Maximum;

    [ObservableProperty]
    private int _TrackBar1Value;

    [ObservableProperty]
    private DateTime _DateTimePicker1Value;

    [ObservableProperty]
    System.Collections.ObjectModel.ObservableCollection<ListItem<(EEventArt, int)>> _ListBox2Items = new();

    [ObservableProperty]
    System.Collections.ObjectModel.ObservableCollection<ListItem<(bool, int)>> _LstList3Items = new();

    [ObservableProperty]
    private string _LstList3Text;

    [ObservableProperty]
    private string _DateLastCheckText;

    [ObservableProperty]
    ListItem<(bool, int)> _LstList3SelectedItem;

    #endregion

    #region Methods
    public MenueViewModel(IFraStatisticsViewModel statistics, IMessenger messenger)
    {
        _statistics = statistics;
        _messenger = messenger;
        // Initialize other properties or commands if needed
    }
    [RelayCommand]
    private void FormClosing()
    {
        // Implement the logic for FormClosing here
    }

    [RelayCommand]
    private void OpenPrint() => throw new NotImplementedException();

    [RelayCommand]
    private void OpenImportExport() => throw new NotImplementedException();

    [RelayCommand]
    private void OpenCalculations() => throw new NotImplementedException();

    [RelayCommand]
    private void OpenFunctionKeys() => throw new NotImplementedException();

    [RelayCommand]
    private void OpenCheckFamilies() => throw new NotImplementedException();

    [RelayCommand]
    private void OpenCheckMissing() => throw new NotImplementedException();

    [RelayCommand]
    private void OpenCheckPersons() => throw new NotImplementedException();

    [RelayCommand]
    private void OpenDuplettes() => throw new NotImplementedException();

    [RelayCommand]
    private void OpenNotes()
    {
        Interaction.MsgBox("Test");
    }

    [RelayCommand]
    private void OpenEnterLizenz() => throw new NotImplementedException();

    [RelayCommand]
    private void OpenReorg() => throw new NotImplementedException();

    [RelayCommand]
    private void OpenBackupRead() => throw new NotImplementedException();

    [RelayCommand]
    private void BackupWrite() => throw new NotImplementedException();

    [RelayCommand]
    private void OpenSendData() => throw new NotImplementedException();

    [RelayCommand]
    private void OpenCheckUpdate()
    {
        UpdateVisible = true;
    }

    [RelayCommand]
    private void UpdateYes()
    {
        UpdateVisible = false;
    }
    [RelayCommand]
    private void UpdateNo()
    {
        UpdateVisible = false;
    }
    [RelayCommand]
    private void OpenRemoteDiag() => throw new NotImplementedException();


    [RelayCommand]
    private void EndProgram()
    {
        Environment.Exit(0);
    }

    [RelayCommand]
    private void FormLoad()
    {
        // Implement the logic for FormLoad here
    }

    [RelayCommand]
    private void OpenFamilies()
    {
        // Implement the logic for OpenFamilies here
    }

    [RelayCommand]
    private void OpenPlaces()
    {
        // Implement the logic for OpenPlaces here
    }

    [RelayCommand]
    private void OpenPersons()
    {
        FontSize = 18f;
        Grossaend?.Invoke(FontSize);
        // Implement the logic for OpenPersons here
    }
    [RelayCommand]
    private void OpenStatistics()
    {
        // Implement the logic for OpenStatistics here
    }
    [RelayCommand]
    private void OpenConfig()
    {
        // Implement the logic for OpenConfig here
    }
    [RelayCommand]
    private void OpenAddress()
    {
        var _msg = IoC.GetRequiredService<IShowDlgMsg>();
        _msg.Dialog = IoC.GetReqSrv(AdresseType);
        _messenger.Send<IShowDlgMsg>(_msg);
    }
    [RelayCommand]
    private void OpenSources()
    {
        // Implement the logic for OpenSources here
    }
    [RelayCommand]
    private void OpenProperty()
    {
        // Implement the logic for OpenSources here
    }
    [RelayCommand]
    private void OpenMandants()
    {
        // Implement the logic for OpenMandants here
    }
    [RelayCommand]
    private void OpenTexts()
    {
        // Implement the logic for OpenPlacesMap here
    }
    #endregion
}
