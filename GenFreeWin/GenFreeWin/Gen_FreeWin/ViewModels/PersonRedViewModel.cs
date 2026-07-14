using CommunityToolkit.Mvvm.Input;
using GenFree.ViewModels.Interfaces;
using MVVM.ViewModel;

namespace GenFreeWin.ViewModels;

public partial class PersonRedViewModel : BaseViewModelCT, IPersonRedViewModel
{
    public int PersonId { get; set; }
    public string PersonSurname { get; set; }
    public string PersonGivenName { get; set; }
    public string PersonTitle { get; set; }
    public string PersonNotes { get; set; }
    public string PersonAKA { get; set; }
    public string PersonResidence { get; set; }
    public string PersonAdditional { get; set; }
    public string Marriages { get; set; }
    public string PersonBirthData { get; set; }
    public string PersonDeathData { get; set; }
    public string PersonGender { get; set; }
    public string PersonOccupation { get; set; }
    public string Mother_Text { get; set; }
    public string Father_Text { get; set; }

    [RelayCommand]
    private void DeletePerson() { }

    [RelayCommand]
    private void PersonName() => throw new System.NotImplementedException();

    [RelayCommand]
    private void Grandparent() => throw new System.NotImplementedException();

    public void Clear(EUserText eUser)
    {
        PersonSurname = string.Empty;
        PersonGivenName = string.Empty;
        PersonTitle = string.Empty;
        PersonNotes = string.Empty;
        PersonAKA = string.Empty;
        PersonResidence = string.Empty;
        PersonAdditional = string.Empty;
        Marriages = string.Empty;
    }

}