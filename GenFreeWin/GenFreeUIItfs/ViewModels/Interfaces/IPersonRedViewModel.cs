using CommunityToolkit.Mvvm.Input;
using Gen_FreeWin;
using System.ComponentModel;

namespace GenFree.ViewModels.Interfaces;

public interface IPersonRedViewModel: INotifyPropertyChanged
{
    int PersonId { get; set; }
    string PersonSurname { get; set; }
    string PersonGivenName { get; set; }
    string PersonBirthData { get; set; }
    string PersonDeathData { get; set; }
    string PersonGender { get; set; }
    string PersonOccupation { get; set; }
    string PersonAKA { get; set; }

    string Marriages { get; set; }
    string PersonTitle { get; set; }
    string PersonAdditional { get; set; }
    string PersonResidence { get; set; }

    string Mother_Text { get; set; }
    string Father_Text { get; set; }

    IRelayCommand DeletePersonCommand { get; }
    IRelayCommand PersonNameCommand { get; }
    IRelayCommand GrandparentCommand { get; }
    string PersonNotes { get; set; }

    void Clear(EUserText eUser);
}