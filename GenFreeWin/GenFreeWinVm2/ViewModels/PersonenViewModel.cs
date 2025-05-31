using CommunityToolkit.Mvvm.Input;
using Gen_FreeWin;
using GenFree.Data;
using GenFree.Helper;
using GenFree.ViewModels.Interfaces;
using MVVM.ViewModel;
using System.Collections.ObjectModel;

namespace GenFreeWin.ViewModels;

public partial class PersonenViewModel : BaseViewModelCT, IPersonenViewModel
{
    public IFraPersImpQuerryViewModel FraPersImpQuerryViewModel => throw new NotImplementedException();

    public IEventShowEditViewModel BirthEVM => throw new NotImplementedException();

    public IEventShowEditViewModel BaptismEVM => throw new NotImplementedException();

    public IEventShowEditViewModel DeathEVM => throw new NotImplementedException();

    public IEventShowEditViewModel BurialEVM => throw new NotImplementedException();

    public IRelayCommand FormLoadCommand => throw new NotImplementedException();

    public IRelayCommand ResidenceCommand => throw new NotImplementedException();

    public IRelayCommand OccupationCommand => throw new NotImplementedException();

    public IRelayCommand TitleCommand => throw new NotImplementedException();

    public IRelayCommand AdditionalCommand => throw new NotImplementedException();

    public IRelayCommand HometownCommand => throw new NotImplementedException();

    public IRelayCommand LoadFromFileCommand => throw new NotImplementedException();

    public IRelayCommand ReturnCommand => throw new NotImplementedException();

    public IRelayCommand SearchNumberCommand => throw new NotImplementedException();

    public IRelayCommand NewPersonCommand => throw new NotImplementedException();

    public IRelayCommand NextCommand => throw new NotImplementedException();

    public IRelayCommand PreviousCommand => throw new NotImplementedException();

    public IRelayCommand DeleteCommand => throw new NotImplementedException();

    public IRelayCommand GodparentIfNoCommand => throw new NotImplementedException();

    public IRelayCommand NoGodparentsCommand => throw new NotImplementedException();

    public IRelayCommand EndTextInputCommand => throw new NotImplementedException();

    public IRelayCommand SaveNExitCommand => throw new NotImplementedException();

    public IRelayCommand SaveToFamilyCommand => throw new NotImplementedException();

    public IRelayCommand SearchAncestorsCommand => throw new NotImplementedException();

    public IRelayCommand NewFamilyCommand => throw new NotImplementedException();

    public IRelayCommand ShowPersonCommand => throw new NotImplementedException();

    public IRelayCommand NoSourcesCommand => throw new NotImplementedException();

    public IRelayCommand LinkedPersonCommand => throw new NotImplementedException();

    public IRelayCommand LinkToCommand => throw new NotImplementedException();

    public IRelayCommand SearchNameCommand => throw new NotImplementedException();

    public IRelayCommand PrintScreenCommand => throw new NotImplementedException();

    public IRelayCommand NoWitnessesCommand => throw new NotImplementedException();

    public IRelayCommand WitnessIfNoCommand => throw new NotImplementedException();

    public IRelayCommand ResarchCommand => throw new NotImplementedException();

    public EUserText eFamHdr => throw new NotImplementedException();

    public int FamNr => throw new NotImplementedException();

    public int PersonNr { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public ELinkKennz Family_LinkKennz => throw new NotImplementedException();

    public int NachfNr => throw new NotImplementedException();

    public string Surnames_Text { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string Givennames_Text { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string Alias_Text { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string Predicate_Text { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string Nickname_Text { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string Sex_Text { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string Religion_Text { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string Prefix_Text { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string Suffix_Text { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string Clan_Text { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string Status_Text { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public string Marriages_Text => throw new NotImplementedException();

    public string Notes_Text => throw new NotImplementedException();

    public string Search_Text => throw new NotImplementedException();

    public string Search2_Text => throw new NotImplementedException();

    public string Search3_Text => throw new NotImplementedException();

    public ObservableCollection<ListItem<int>> Occupation_Items => throw new NotImplementedException();

    public ListItem<int> Occupation_SelectedItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public ObservableCollection<ListItem<int>> Title_Items => throw new NotImplementedException();

    public ListItem<int> Title_SelectedItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public ObservableCollection<ListItem<int>> Residence_Items => throw new NotImplementedException();

    public ListItem<int> Residence_SelectedItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public ObservableCollection<ListItem<int>> Home_Items => throw new NotImplementedException();

    public ListItem<int> Home_SelectedItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public ObservableCollection<ListItem<int>> Additional_Items => throw new NotImplementedException();

    public ListItem<int> Additional_SelectedItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public bool Givennames_Enabled => throw new NotImplementedException();

    public bool IsNotReadOnly => throw new NotImplementedException();

    public bool Return_Visible => throw new NotImplementedException();

    public bool SearchNumber_Visible => throw new NotImplementedException();

    public bool NewPerson_Visible => throw new NotImplementedException();

    public bool Next_Visible => throw new NotImplementedException();

    public bool Previous_Visible => throw new NotImplementedException();

    public bool Delete_Visible => throw new NotImplementedException();

    public bool SaveNExit_Visible => throw new NotImplementedException();

    public bool SaveToFamily_Visible => throw new NotImplementedException();

    public bool SearchAncestors_Visible => throw new NotImplementedException();

    public bool NewFamily_Visible => throw new NotImplementedException();

    public bool ShowPerson_Visible => throw new NotImplementedException();

    public bool NoSources_Visible => throw new NotImplementedException();

    public bool LinkedPerson_Visible => throw new NotImplementedException();

    public bool LinkTo_Visible => throw new NotImplementedException();

    public bool SearchName_Visible => throw new NotImplementedException();

    public bool PrintScreen_Visible => throw new NotImplementedException();

    public bool NoWitnesses_Visible => throw new NotImplementedException();

    public bool WitnessIfNo_Visible => throw new NotImplementedException();

    public bool Resarch_Visible => throw new NotImplementedException();

    public bool PersImpQuerry1_Visible => throw new NotImplementedException();

    public bool Dublicates_Visible => throw new NotImplementedException();

    public IRelayCommand PropertyCommand => throw new NotImplementedException();

    public IRelayCommand ShowPlacesCommand => throw new NotImplementedException();

    public string CreationDate_Text => throw new NotImplementedException();

    public string Family1_Text => throw new NotImplementedException();

    public string Age_Text => throw new NotImplementedException();

    public string AncesterNr_Text => throw new NotImplementedException();

    public string NoWitnesses_Text => throw new NotImplementedException();

    public string NoGodparents_Text => throw new NotImplementedException();

    public string NoSources_Text => throw new NotImplementedException();

    public string Display_Text => throw new NotImplementedException();

    public string LinkedPerson_Text => throw new NotImplementedException();

    public string LinkTo_Text => throw new NotImplementedException();

    public string WitnessIfNo_Text => throw new NotImplementedException();

    public string Mandant_Text => throw new NotImplementedException();

    public string Property_Text => throw new NotImplementedException();

    public bool GodparentIfNo_Visible => throw new NotImplementedException();

    public bool NoGodparents_Visible => throw new NotImplementedException();

    public bool EndTextInput_Visible => throw new NotImplementedException();

    public int AendPruef(int persInArb, int ubg2)
    {
        throw new NotImplementedException();
    }

    public (EEventArt eArt, int iLfNr) GetEventArtNr(EUserText iText)
    {
        throw new NotImplementedException();
    }

    public void Perzeig(int persInArb)
    {
        throw new NotImplementedException();
    }
}