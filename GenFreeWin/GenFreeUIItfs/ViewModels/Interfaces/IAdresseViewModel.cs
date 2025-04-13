using CommunityToolkit.Mvvm.Input;

namespace Gen_FreeWin.ViewModels.Interfaces;

public interface IAdresseViewModel
{
    string Title { get; set; }
    string Givenname { get; set; }
    string Surname { get; set; }
    string Street { get; set; }
    string Zip { get; set; }
    string Place { get; set; }
    string Phone { get; set; }
    string EMail { get; set; }
    string Special { get; set; }
    IRelayCommand SaveCommand { get; }
    IRelayCommand FormLoadCommand { get; }
}