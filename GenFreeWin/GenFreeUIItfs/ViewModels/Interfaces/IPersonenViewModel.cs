using CommunityToolkit.Mvvm.Input;
using Gen_FreeWin;
using GenFree.Data;
using System.ComponentModel;

namespace GenFree.ViewModels.Interfaces;

public interface IPersonenViewModel : INotifyPropertyChanged
{
    IFraPersImpQuerryViewModel FraPersImpQuerryViewModel { get; }

    IRelayCommand ResidenceCommand { get; }
    IRelayCommand OccupationCommand { get; }
    IRelayCommand TitleCommand { get; }
    IRelayCommand AdditionalCommand { get; }
    IRelayCommand HometownCommand { get; }

    IRelayCommand LoadFromFileCommand { get; }
    IRelayCommand ReturnCommand { get; }

    string Surnames_Text { get; set; }
    string Givennames_Text { get; set; }
    string Alias_Text { get; set; }
    string Predicate_Text { get; set; }
    string Nickname_Text { get; set; }

    (EEventArt eArt, int iLfNr) GetEventArtNr(EUserText iText);
}