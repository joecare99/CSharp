using CommunityToolkit.Mvvm.Input;
using Gen_FreeWin;
using GenFree.Data;
using GenFree.Helper;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace GenFree.ViewModels.Interfaces;

public interface IPersonenViewModel : INotifyPropertyChanged
{
    IFraPersImpQuerryViewModel FraPersImpQuerryViewModel { get; }
    IEventShowEditViewModel BirthEVM { get; }
    IEventShowEditViewModel BaptismEVM { get; }
    IEventShowEditViewModel DeathEVM { get; }
    IEventShowEditViewModel BurialEVM { get; }

    IRelayCommand FormLoadCommand { get; }
    IRelayCommand ResidenceCommand { get; }
    IRelayCommand OccupationCommand { get; }
    IRelayCommand TitleCommand { get; }
    IRelayCommand AdditionalCommand { get; }
    IRelayCommand LoadFromFileCommand { get; }

    IRelayCommand ReturnCommand { get; }
    IRelayCommand SearchNumberCommand { get; }
    IRelayCommand NewPersonCommand { get; }
    IRelayCommand NextCommand { get; }
    IRelayCommand PreviousCommand { get; }
    IRelayCommand DeleteCommand { get; }
    IRelayCommand GodparentIfNoCommand { get; }
    IRelayCommand NoGodparentsCommand { get; }
    IRelayCommand EndTextInputCommand { get; }
    IRelayCommand SaveNExitCommand { get; }
    IRelayCommand SaveToFamilyCommand { get; }
    IRelayCommand SearchAncestorsCommand { get; }
    IRelayCommand NewFamilyCommand { get; }
    IRelayCommand ShowPersonCommand { get; }
    IRelayCommand NoSourcesCommand { get; }
    IRelayCommand LinkedPersonCommand { get; }
    IRelayCommand LinkToCommand { get; }
    IRelayCommand SearchNameCommand { get; }
    IRelayCommand PrintScreenCommand { get; }
    IRelayCommand NoWitnessesCommand { get; }
    IRelayCommand WitnessIfNoCommand { get; }
    IRelayCommand ResarchCommand { get; }

    EUserText eFamHdr { get; }

    int FamNr { get; }
    int PersonNr { get; set; }
    public ELinkKennz Family_LinkKennz { get; }
    int NachfNr { get; }


    string Surnames_Text { get; set; }
    string Givennames_Text { get; set; }
    string Alias_Text { get; set; }
    string Predicate_Text { get; set; }
    string Nickname_Text { get; set; }
    string Sex_Text { get; set; }
    string Religion_Text { get; set; }
    string Prefix_Text { get; set; }
    string Suffix_Text { get; set; }
    string Clan_Text { get; set; }
    string Status_Text { get; set; }
    string Marriages_Text { get; }
    string Notes_Text { get; }
    string Search_Text { get; }
    string Search2_Text { get; }
    string Search3_Text { get; }

    ObservableCollection<ListItem<int>> Occupation_Items { get; }
    ListItem<int> Occupation_SelectedItem { get; set; }
    ObservableCollection<ListItem<int>> Title_Items { get; }
    ListItem<int> Title_SelectedItem { get; set; }
    ObservableCollection<ListItem<int>> Residence_Items { get; }
    ListItem<int> Residence_SelectedItem { get; set; }
    ObservableCollection<ListItem<int>> Home_Items { get; }
    ListItem<int> Home_SelectedItem { get; set; }
    ObservableCollection<ListItem<int>> Additional_Items { get; }
    ListItem<int> Additional_SelectedItem { get; set; }

    // Enabeling Stuff
    bool Givennames_Enabled { get; }
    bool IsNotReadOnly { get; }

    // Visibility
    bool Return_Visible { get; }
    bool SearchNumber_Visible { get; }
    bool NewPerson_Visible { get; }
    bool Next_Visible { get; }
    bool Previous_Visible { get; }
    bool Delete_Visible { get; }
    bool SaveNExit_Visible { get; }
    bool SaveToFamily_Visible { get; }
    bool SearchAncestors_Visible { get; }
    bool NewFamily_Visible { get; }
    bool ShowPerson_Visible { get; }
    bool NoSources_Visible { get; }
    bool LinkedPerson_Visible { get; }
    bool LinkTo_Visible { get; }
    bool SearchName_Visible { get; }
    bool PrintScreen_Visible { get; }
    bool NoWitnesses_Visible { get; }
    bool WitnessIfNo_Visible { get; }
    bool Resarch_Visible { get; }

    bool PersImpQuerry1_Visible { get; }
    bool Dublicates_Visible { get; }

    int AendPruef(int persInArb, int ubg2);
    (EEventArt eArt, int iLfNr) GetEventArtNr(EUserText iText);
    void Perzeig(int persInArb);
}