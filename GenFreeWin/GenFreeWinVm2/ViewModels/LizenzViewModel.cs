using BaseLib.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GenFree.Interfaces.Sys;
using GenFree.ViewModels.Interfaces;
using Microsoft.VisualBasic;
using MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GenFreeWin.ViewModels;

public partial class LizenzViewModel : BaseViewModelCT, ILizenzViewModel
{
    private IPersistence _persistence;
    private ISystem _system;
    private string _A;
    private float _Value;
    private short _Command1_Click_Counter;

    [ObservableProperty]
    private string _licText1;
    [ObservableProperty]
    private string _licText2;
    [ObservableProperty]
    private string _licText3;

    [ObservableProperty]
    private bool _displayHintVisible ;

    public Action DoClose { get; set; }
    public Action DoEndProg { get; set; }

    public IInteraction Interaction { get; set; }

    public LizenzViewModel(IPersistence persistence, ISystem system)
    {
        _persistence = persistence;
        _system = system;
    }

    [RelayCommand]
    private void Cancel()
    {
        DoClose?.Invoke();
    }

    [RelayCommand]
    private void Verify()
    {
        _Command1_Click_Counter++;
        if (_system.SetLicNr(LicText1 + "-GB-" + LicText2 + "-" + LicText3))
        { 
            return;            
        }
        if (_Command1_Click_Counter > 4)
        {
            Interaction.MsgBox("Sie hatten vier Versuche die Lizenz-Nr. einzugeben. Das Programm wird beendet!");
            DoEndProg?.Invoke();
        }
        _Value = (float)Interaction.MsgBox("Die eingegebene Lizenz-Nr. ist falsch", title: "Versuch " + _Command1_Click_Counter.AsString(), mb: MessageBoxButtons.RetryCancel);
        if (_Value == 2f)
        {
            DoEndProg?.Invoke();
        }
    }

    [RelayCommand]
    private void ReqHint()
    {
        DisplayHintVisible = true;
    }
}
