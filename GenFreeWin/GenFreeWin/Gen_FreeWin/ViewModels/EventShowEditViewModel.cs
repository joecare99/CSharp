using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GenFree.Data;
using GenFree.ViewModels.Interfaces;
using MVVM.ViewModel;
using System;

namespace GenFreeWin.ViewModels;

public partial class EventShowEditViewModel : BaseViewModelCT, IEventShowEditViewModel
{
    [ObservableProperty]
    private EEventArt _eEvtArt;

    [ObservableProperty]
    private int _iPersNr;

    [ObservableProperty]
    public EHdrSize _eHdrSize = EHdrSize.eShort;

    [ObservableProperty]
    private string _display_Text;

    [ObservableProperty]
    private EUserText _display_Hdr;

    public Action<int> DataSchreib { get; set; }
    public Action<int> DataZeig { get; set; }
    public Action DoClick { get; set; }

    partial void OnEEvtArtChanged(EEventArt eEventArt)
    {
        Display_Hdr = eEventArt switch
        {
            EEventArt.eA_Birth => EUserText.tBirth,
            EEventArt.eA_Baptism => EUserText.tBaptism,
            EEventArt.eA_Death => EUserText.tDeath,
            EEventArt.eA_Burial => EUserText.tBurial,
            _ => throw new NotImplementedException()
        };
    }

    partial void OnIPersNrChanged(int newVal)
    {
        Display_Text = DataModul.Event_GetLabelText(newVal, EEvtArt, Event_PreDisplay);
    }

    public string Event_PreDisplay(bool xCitation = false, bool xWitness = false, bool xAnnotation = false, bool xBC = false, bool xReg = false)
    {
        string text = "";
        if (xCitation)
            text = "§ ";
        if (xWitness)
            text += "Z ";
        if (xAnnotation)
            text += "B ";
        if (xBC)
            text += "< ";
        if (xReg)
            text += "U ";
        return text;
    }

    [RelayCommand]
    private void Click()
    {
        DataSchreib?.Invoke(1);

        _ = MainProject.Forms.Ereignis.ShowEventDialog(EEvtArt);
        DataZeig?.Invoke(IPersNr);
    }
}
