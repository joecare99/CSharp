using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gen_FreeWin.ViewModels.Interfaces;
using MVVM.ViewModel;

namespace GenFreeWin.ViewModels;

public partial class MenueViewModel : BaseViewModelCT, IMenu1ViewModel
{
    private IFraStatisticsViewModel _statistics;

    #region Properties
    public bool CreationDateVisible => throw new NotImplementedException();

    public bool MarkedVisible => throw new NotImplementedException();

    public bool NotesVisible => throw new NotImplementedException();

    public bool CodeOfArmsVisible => throw new NotImplementedException();

    public bool ListBox2Visible => throw new NotImplementedException();

    public bool List3Visible => throw new NotImplementedException();

    public bool WarningVisible => throw new NotImplementedException();

    public bool EnterLizenzVisible => throw new NotImplementedException();

    public string Notes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public string Mandant => "Some Mandant";

    public string MandantPath => "";

    public string HdrOwner => "Me";

    public string Owner => "";

    public string Menue18 => "Menue18";

    public string HdrProgName => "";

    public string HdrAdt => "";

    public string HdrCopyright => "(c) by Joe Care 2025";

    public string WarningText => "a Warning !";

    public IInteraction Interaction { get; set; }
    public Action<Enum> SetWindowState { get; set; }
    public Func<Enum> GetWindowState { get; set; }
    public Action<float> Grossaend { get; set; }

    public IFraStatisticsViewModel Statistics => _statistics;

    public float FontSize => throw new NotImplementedException();
    #endregion

    #region Methods
    public MenueViewModel(IFraStatisticsViewModel statistics)
    {
        _statistics = statistics;
        // Initialize other properties or commands if needed
    }
    [RelayCommand]
   private void FormClosing() => throw new NotImplementedException();

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
    private void OpenNotes() => throw new NotImplementedException();

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
    private void OpenCheckUpdate() => throw new NotImplementedException();

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
        // Implement the logic for OpenAddress here
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
