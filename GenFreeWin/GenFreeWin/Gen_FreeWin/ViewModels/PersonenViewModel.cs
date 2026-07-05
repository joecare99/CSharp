using BaseLib.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gen_FreeWin.Main;
using Gen_FreeWin.Views;
using GenFree;
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Sys;
using GenFree.Interfaces.VB;
using GenFree.ViewModels.Interfaces;
using GenFreeWin.Views;
using Microsoft.VisualBasic;
using MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Gen_FreeWin.ViewModels;

public partial class PersonenViewModel : BaseViewModelCT, IPersonenViewModel
{
    public const string cPerPos = "Per.pos";
    private const int cDefaultErr = -0x7FF5FFEC;
    private const string sSexMustBeAssigned = "Geschlecht muß angegeben werden\rM = Mann; F = Frau; D = Divers; U = unbekannt";

    private IModul1 Modul1=> _Modul1.Instance;
    private IGenPersistence Persistence => Modul1.Persistence;
    private IFraPersImpQueryViewModel _fraPersImpQuerryViewModel;
    private IEventShowEditViewModel _birthEVM;
    private IEventShowEditViewModel _baptismEVM;
    private IEventShowEditViewModel _deathEVM;
    private IEventShowEditViewModel _burialEVM;

    private float Pschalt;
    private bool Kreuz;

    [Obsolete]
    IProjectData ProjectData => Modul1.ProjectData;
    IInteraction Interaction => Menue.Default;
    [Obsolete]
    IVBInformation Information => Modul1.Information;
    [Obsolete]
    IStrings Strings => Modul1.Strings;

    [ObservableProperty]
    public partial string Surnames_Text { get; set; }
    [ObservableProperty]
    public partial string Givennames_Text { get; set; }
    [ObservableProperty]
    public partial string Alias_Text { get; set; }
    [ObservableProperty]
    public partial string Predicate_Text { get; set; }
    [ObservableProperty]
    public partial string Nickname_Text { get; set; }
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Givennames_Enabled))]
    public partial string Sex_Text { get; set; }
    [ObservableProperty]
    public partial string Religion_Text { get; set; }
    [ObservableProperty]
    public partial string Prefix_Text { get; set; }
    [ObservableProperty]
    public partial string Suffix_Text { get; set; }
    [ObservableProperty]
    public partial string Clan_Text { get; set; }
    [ObservableProperty]
    public partial string Status_Text { get; set; }
    [ObservableProperty]
    public partial string Notes_Text { get; set; }
    [ObservableProperty]
    public partial string Search_Text { get; set; }
    [ObservableProperty]
    public partial string Search2_Text { get; set; }
    [ObservableProperty]
    public partial string Search3_Text { get; set; }
    [ObservableProperty]
    public partial string Marriages_Text { get; set; } 
    [ObservableProperty]
    public partial string AncesterNr_Text { get; set; }
    [ObservableProperty]
    public partial string NoWitnesses_Text { get; set; }
    [ObservableProperty]
    public partial string NoGodparents_Text { get; set; }
    [ObservableProperty]
    public partial string NoSources_Text { get; set; }
    [ObservableProperty]
    public partial string Display_Text { get; set; }
    [ObservableProperty]
    public partial string LinkedPerson_Text { get; set; }
    [ObservableProperty]
    public partial string LinkTo_Text { get; set; }
    [ObservableProperty]
    public partial string WitnessIfNo_Text { get; set; }
    [ObservableProperty]
    public partial string Family1_Text { get; set; }
    [ObservableProperty]
    public partial string CreationDate_Text { get; set; } 
    [ObservableProperty]
    public partial string Mandant_Text { get; set; }
    [ObservableProperty]
    public partial string Property_Text { get; set; }


    [ObservableProperty]
    int _nachfNr;
    public string DuplLabel6_Text { get; private set; }
    public string FamPers_Text { get; private set; }

    public IFraPersImpQueryViewModel FraPersImpQuerryViewModel 
        => _fraPersImpQuerryViewModel ??= IoC.GetRequiredService<IFraPersImpQueryViewModel>();
    public IEventShowEditViewModel BirthEVM 
        => _birthEVM ??= IoC.GetKeyedRequiredService<IEventShowEditViewModel>(EEventArt.eA_Birth);
    public IEventShowEditViewModel BaptismEVM
        => _baptismEVM ??= IoC.GetKeyedRequiredService<IEventShowEditViewModel>(EEventArt.eA_Baptism);
    public IEventShowEditViewModel DeathEVM
        => _deathEVM ??= IoC.GetKeyedRequiredService<IEventShowEditViewModel>(EEventArt.eA_Death);
    public IEventShowEditViewModel BurialEVM 
        => _burialEVM ??= IoC.GetKeyedRequiredService<IEventShowEditViewModel>(EEventArt.eA_Burial);

    public bool IsNotReadOnly { get; private set; }
    public bool SaveNExit_Enabled { get; private set; }
    public bool SaveToFamily_Enabled { get; private set; }
    public bool Cancel_Enabled { get; private set; }

    public bool Givennames_Enabled => !string.IsNullOrWhiteSpace(Sex_Text);

    [ObservableProperty]
    public int _personNr;
    public int FamNr { get; private set; }
    public ELinkKennz Family_LinkKennz { get; private set; }
    public EUserText eFamHdr { get; private set; }

    private bool xDestIsFam;
    [ObservableProperty]
    private bool _Return_Visible;
    [ObservableProperty]
    private bool _SearchNumber_Visible;
    [ObservableProperty]
    private bool _NewPerson_Visible;
    [ObservableProperty]
    private bool _Next_Visible;
    [ObservableProperty]
    private bool _Previous_Visible;
    [ObservableProperty]
    private bool _PersImpQuerry1_Visible;
    [ObservableProperty]
    private bool _Dublicates_Visible;
    [ObservableProperty]
    private bool _SaveNExit_Visible;
    [ObservableProperty]
    private bool _SaveToFamily_Visible;
    [ObservableProperty]
    private bool _Delete_Visible;
    [ObservableProperty]
    private bool _Cancel_Visible;
    [ObservableProperty]
    private bool _GodparentIfNo_Visible;
    [ObservableProperty]
    private bool _NoGodparents_Visible;
    [ObservableProperty]
    private bool _SearchAncestors_Visible;
    [ObservableProperty]
    private bool _ShowPerson_Visible;
    [ObservableProperty]
    private bool _NewFamily_Visible;
    [ObservableProperty]
    private bool _SearchName_Visible;
    [ObservableProperty]
    private bool _Resarch_Visible;
    [ObservableProperty]
    private bool _RegisterSearch_Visible;
    [ObservableProperty]
    private bool _ShowPlaces_Visible;
    [ObservableProperty]
    private bool _Sorting_Visible;
    [ObservableProperty]
    private bool _NoSources_Visible;
    [ObservableProperty]
    private bool _LinkedPerson_Visible;
    [ObservableProperty]
    private bool _LinkTo_Visible;
    [ObservableProperty]
    private bool _PrintScreen_Visible;
    [ObservableProperty]
    private bool _NoWitnesses_Visible;
    [ObservableProperty]
    private bool _WitnessIfNo_Visible;
    [ObservableProperty]
    private bool _residence_Visible;

    private Color lblDisp_BackColor;
    private string BiText;
    private string Bitext;
    private bool xWitnessIfNo_Tag;
    private bool xNoWitnesses_Tag;
    private Color FieldsOFB_BackColor;
    private bool internalChange;
    private int Trans;
    private DateTime CreationDate_Tag;
    private DateTime Family1_Tag;
    private string SaveToFamily_Text;
    private string Duplicates_Text;
    private string Kont_3;
    private string NachfNr_Text;
    private string Notes_SelectedItem;

    public Action DoClose { private get; set; }
    public Action<object> SetFocus { private get; set; }
    public bool List3_Visible { get; private set; }
    public bool Picture_Visible { get; private set; }
    public bool Adoption_Visible { get; private set; }
    public bool DuplBttn3_Visible { get; private set; }
    public bool Reenter_Visible { get; private set; }
    [ObservableProperty]
    public partial System.Collections.ObjectModel.ObservableCollection<ListItem<int>> Duplicates_Items { get; private set; } = new();
    public System.Collections.ObjectModel.ObservableCollection<ListItem<int>> Property_Items { get; private set; } = new();
    [ObservableProperty]
    public partial System.Collections.ObjectModel.ObservableCollection<ListItem<int>> Occupation_Items { get; private set; } = new();
    public System.Collections.ObjectModel.ObservableCollection<ListItem<int>> Title_Items { get; private set; } = new();
    public System.Collections.ObjectModel.ObservableCollection<ListItem<int>> Residence_Items { get; private set; } = new();
    public System.Collections.ObjectModel.ObservableCollection<ListItem<int>> Home_Items { get; private set; } = new();
    public System.Collections.ObjectModel.ObservableCollection<ListItem<int>> Additional_Items { get; private set; } = new();
    public System.Collections.ObjectModel.ObservableCollection<ListItem<int>> List1_Items { get; private set; } = new();
    public System.Collections.ObjectModel.ObservableCollection<ListItem<PersonFamily>> List2_Items { get; private set; } = new();
    public System.Collections.ObjectModel.ObservableCollection<ListItem<int>> List3_Items { get; private set; } = new();
    public System.Collections.ObjectModel.ObservableCollection<ListItem<(int, string)>> Witnes_Items { get; private set; } = new();
    public System.Collections.ObjectModel.ObservableCollection<IListItem<(int, DateTime, ELinkKennz)>> Sortlist_Items { get; private set; } = new();
    public ListItem<int> Duplicates_SelectedItem { get; set; }
    public ListItem<int> Property_SelectedItem { get; set; }
    public ListItem<int> Occupation_SelectedItem { get; set; }
    public ListItem<int> Title_SelectedItem { get; set; }
    public ListItem<int> Residence_SelectedItem { get; set; }
    public ListItem<int> Home_SelectedItem { get; set; }
    public ListItem<int> Additional_SelectedItem { get; set; }
    public ListItem<(int, string)> Witnes_SelectedItem { get; set; }
    public ListItem<(int, DateTime, ELinkKennz)> Sorting_SelectedItem { get; set; }
    public ListItem<int> List5_SelectedItem { get; set; }
    public string Witnes_Text { get; private set; }
    public Color Witnes_BackColor { get; private set; }
    public string Label15_Text { get; private set; }

    [Obsolete]
    Personen View => Personen.Default;

    public Color NewPerson_BackColor { get; private set; }
    public Color NoGodparents_BackColor { get; private set; }
    public Color NoSources_BackColor { get; private set; }
    public Color LinkedPerson_BackColor { get; private set; }
    public Color LinkTo_BackColor { get; private set; }
    public Color NoWitnesses_BackColor { get; private set; }
    public Color WitnessIfNo_BackColor { get; private set; }
    public bool FieldsOFB_Visible { get; private set; }
    public bool EndTextInput_Visible { get; private set; }
    public string Age_Text { get; private set; }
    public string Box_Text { get; private set; }
    public string DuplBttn3_Text { get; private set; }
    public IList<ESearchSelection> Suchfeld { get; private set; }
    public bool Witnes_Visible { get; private set; }

    partial void OnGivennames_TextChanged(string newValue)
    {
        if (internalChange) return;
        if (Sex_Text.Trim() == "")
        {
            Givennames_Text = "";
            _ = Interaction.MsgBox("Zuerst Geschlecht eingeben.\nOhne Geschlecht keine Eingabe möglich");
            SetFocus(nameof(Sex_Text));
        }
        else
        {
            _ = MainProject.Forms.Vornam.Text_Renamed[1].Focus();
            MainProject.Forms.Vornam.Visible = false;
            MainProject.Forms.Vornam.Show();
            //=================
        }
    }

    partial void OnSex_TextChanged(string value)
    {
        if (internalChange) return;
        Modul1.Trans = 1;
        switch (value.Trim())
        {
            case "U":
            case "M":
            case "F":
            case "m":
            case "u":
            case "f":
                if (eFamHdr == EUserText.tChild_AS)
                {
                    if (Modul1.Aus[(int)EOutCfg.o24].AsInt() == 1.0)
                    {
                        Dublicates_Visible = true;
                        Zeigfam(Surnames_Text.Trim());
                    }
                    //=================
                }
                SetFocus(nameof(Religion_Text));
                break;
            default:
                if (value.Trim() != "")
                {
                    var text = "Geschlecht nur";
                    text += "\r               U = unbekannt";
                    text += "\r               M = male = männlich";
                    text += "\r               F = female = weiblich";
                    _ = Interaction.MsgBox(text, title: "Eingabefehler", mb: MessageBoxButtons.OK);
                    Sex_Text = "";
                    SetFocus(nameof(Sex_Text));
                    //=================
                }
                break;
        }
    }

    partial void OnPersonNrChanged(int persNr)
    {
        BirthEVM.IPersNr = persNr;
        BaptismEVM.IPersNr = persNr;
        DeathEVM.IPersNr = persNr;
        BurialEVM.IPersNr = persNr;
    }

    [RelayCommand]
    private void FormLoad()
    {
        FraPersImpQuerryViewModel.onCancel = (s, e) => _ = Command2_AnyClick(s, e);
        FraPersImpQuerryViewModel.onFromFile = LoadFromFile;
        FraPersImpQuerryViewModel.onReenter = Reenter;
    }

    private (EEventArt eArt,object obj) Command2_AnyClick(object s, object e)
    {
        throw new NotImplementedException();
    }

    [RelayCommand]
    private void Return()
    {
        Kreuz = true;
        FileSystem.FileClose(99);
        if (IsNotReadOnly)
        {
            Modul1.Letzte = new() { iPerson = PersonNr, iFamily = Modul1.Letzte.iFamily };
            Modul1.Persistence.PutIntMand("Letzter.dat", Modul1.Letzte.iPerson, 1L);
        }

        Menue.Default.Hide();
        Menue.Default.Show();


        DoClose();
    }

    [RelayCommand]
    private void Button5_Click()
    {
        FraPersImpQuerryViewModel.IText = EUserText.tCivilState;
        Perschreib(1);
        FraPersImpQuerryViewModel.IReenter = EUserText.t72;
        PersImpQuerry1_Visible = true;
        if (Home_SelectedItem.ItemString == "")
        {
            LoadFromFile();
        }
    }

    [RelayCommand]
    private void Command1_Click()
    {
        var Pschalt = 1f;
        checked
        {
            Modul1.PersInArb = PersonNr;
            Perloesch(Modul1.PersInArb);
            List3_Visible = false;
            SaveNExit_Visible = false;
            Cancel_Visible = false;
            //==========================================
            Return_Visible = true;
            SearchNumber_Visible = true;
            NewPerson_Visible = true;
            Next_Visible = true;
            Previous_Visible = true;
            Delete_Visible = true;
            GodparentIfNo_Visible = true;
            NoGodparents_Visible = true;
            SearchAncestors_Visible = true;
            ShowPerson_Visible = true;
            SearchAncestors_Visible = true;
            NewFamily_Visible = true;
            ShowPerson_Visible = true;
            SearchName_Visible = true;
            Resarch_Visible = true;
            RegisterSearch_Visible = true;
            ShowPlaces_Visible = true;
            Sorting_Visible = true;

            EUserText left = Rahmen.Default.iHdrText;
            if (left == EUserText.tGodparents
                || left == EUserText.tGodparentOf
                || left == EUserText.t348)
            {
                Rahmen.Default.Close();
                Modul1.PersInArb = Modul1.PersInArbsp;
                Perzeig(Modul1.PersInArb);
            }
            else if (left == EUserText.tMarrWitness
                || left == EUserText.tWitnOfEngage
                || left == EUserText.tWitnOfMarr)
            {
                DoClose();
                Rahmen.Default.Close();
                Familie.Default.Fameinlesen(Modul1.FamInArb, out short Rich);
            }
            else
            {
                _ = Interaction.MsgBox(Rahmen.Default.frmFrame1.Text);
            }
        }
    }


    [RelayCommand]
    private void LoadFromFile()
    {
        var r = Command2_AnyClick(null,null);
        var M1_Iter = 1;
        while (M1_Iter <= 900)
        {
            if (!DataModul.Event.Exists(((EEventArt)Modul1.Ubg, Modul1.PersInArb, (short)M1_Iter)))
            {
                Modul1.LfNR = (short)M1_Iter;
                break;
            }

            M1_Iter++;

        }
        Modul1.ErSchalt = 2;
        MainProject.Forms.Ereignis.Show();
        int ubg = Modul1.Ubg;
        if (ubg == 105)
        {
            _ = MainProject.Forms.Ereignis.TextBox1.Focus();
            //=================
        }
        else if (ubg is 300 or 301)
        {
            _ = MainProject.Forms.Ereignis.TextBox4.Focus();
            //=================
        }
        else if (ubg == 302)
        {
            if (Modul1.Aus[(int)EOutCfg.o26] == "")
            {
                Modul1.Aus[(int)EOutCfg.o26] = true.AsString();
            }
            _ = Modul1.Aus[(int)EOutCfg.o26].AsBool()
                ? MainProject.Forms.Ereignis.TextBox5.Focus()
                : MainProject.Forms.Ereignis.TextBox2.Focus();
            //=================
        }
        else
        {
            _ = MainProject.Forms.Ereignis.TextBox2.Focus();
            //=================
        }

        MainProject.Forms.Ereignis.Hide();
        _ = MainProject.Forms.Ereignis.ShowEventDialog(r.eArt);
        if (Modul1.PersInArb == 0)
        {
            Modul1.PersInArb = PersonNr;
            //=================
        }
        Perzeig(Modul1.PersInArb);

    }

    [RelayCommand]
    private void Reenter()
    {
        PersImpQuerry1_Visible = false;

        var r = GetEventArtNr(FraPersImpQuerryViewModel.IText);
        _ = MainProject.Forms.Ereignis.ShowEventDialog(r.eArt);
        Modul1.PersInArb = PersonNr;
        if (Rahmen.Default.eResult == ERahmenResult.eRR25)
        {
            Modul1.PersInArb = 0;//Modul1_PerZeug;
        }
        Perzeig(Modul1.PersInArb);
    }

    public (EEventArt eArt, int iLfNr) GetEventArtNr(EUserText iText)
    {
        return iText switch
        {
            EUserText.tOccupation => (EEventArt.eA_300, Occupation_SelectedItem.ItemData > -1 ? Occupation_SelectedItem.ItemData : 1),
            EUserText.t70 => (EEventArt.eA_301, Title_SelectedItem.ItemData > -1 ? Title_SelectedItem.ItemData : 1),
            EUserText.tResidence => (EEventArt.eA_302, Residence_SelectedItem.ItemData > -1 ? Residence_SelectedItem.ItemData : 1),
            EUserText.tConfirmation => (EEventArt.eA_105, Additional_SelectedItem.ItemData > -1.0 ? Additional_SelectedItem.ItemData : 1),
            EUserText.tCivilState => (EEventArt.eA_106, Home_SelectedItem.ItemData > -1 ? Home_SelectedItem.ItemData : 1),
            _ => (EEventArt.eA_Unknown, 0),
        };
    }

    [RelayCommand(CanExecute = nameof(IsNotReadOnly))]
    private void NewFamily()
    {
        if (!IsNotReadOnly)
        {
            _ = Interaction.MsgBox(Modul1.Message_sNoChangesOnCD, title: "", icon: MessageBoxIcon.Information);
            return;
        }
        else if (Interaction.MsgBox("Wirklich eine neue Familie anlegen in der diese Person Elternteil ist?", mb: MessageBoxButtons.YesNo) == DialogResult.No)
        {
            return;
        }

        if (DataModul.Person.GetSex(PersonNr) == "M")
        {
            Modul1.eLKennz = ELinkKennz.lkFather;
        }
        else if (DataModul.Person.GetSex(PersonNr) == "F")
        {
            Modul1.eLKennz = ELinkKennz.lkMother;
        }
        else
        {
            _ = Interaction.MsgBox("Bei unbekanntem Geschlecht ist keine Ehe möglich.");
            return;
        }
        int famInArb = Modul1.FamInArb;
        famInArb = 0;
        DataModul.DB_WDTable.MoveFirst();
        if (DataModul.DB_WDTable.Fields[WDFields.Nr].AsInt() == 1)
        {
            if (DataModul.LeerTab_GetEmptyFam(out famInArb))
            {
                DataModul.DB_FamilyTable.Index = nameof(FamilyIndex.Fam);
                DataModul.DB_FamilyTable.Seek("=", famInArb);
                if (!DataModul.DB_FamilyTable.NoMatch)
                {
                    famInArb = 0;
                }
            }
        }
        if (famInArb == 0)
        {
            if (DataModul.DB_FamilyTable.RecordCount == 0)
            {
                famInArb = 1;
            }
            else
            {
                DataModul.DB_FamilyTable.Index = nameof(FamilyIndex.Fam);
                DataModul.DB_FamilyTable.MoveLast();
                famInArb = DataModul.DB_FamilyTable.Fields[FamilyFields.FamNr].AsInt() + 1;
            }
        }
        DoClose();
        if (Modul1.Typ != DriveType.CDRom)
        {
            DataModul.DB_FamilyTable.AddNew();
            DataModul.DB_FamilyTable.Fields[FamilyFields.AnlDatum].Value = DateTime.Today.Year.ToString() + DateTime.Today.Day.ToString() + DateTime.Today.Month.ToString();
            DataModul.DB_FamilyTable.Fields[FamilyFields.EditDat].Value = "0";
            DataModul.DB_FamilyTable.Fields[FamilyFields.Prüfen].Value = "1    ";
            DataModul.DB_FamilyTable.Fields[FamilyFields.Bem1].Value = " ";
            DataModul.DB_FamilyTable.Fields[FamilyFields.FamNr].Value = famInArb;
            DataModul.DB_FamilyTable.Fields[FamilyFields.Name].Value = 0;
            DataModul.DB_FamilyTable.Fields[FamilyFields.Aeb].Value = 0;
            DataModul.DB_FamilyTable.Fields[FamilyFields.Fuid].Value = Guid.NewGuid();
            DataModul.DB_FamilyTable.Update();
        }
        PerSatzLes(Modul1.PersInArb);
        if (DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "M")
        {
            Modul1.eLKennz = ELinkKennz.lkFather;
        }
        else if (DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "F")
        {
            Modul1.eLKennz = ELinkKennz.lkMother;
        }
        else
        {
            _ = Interaction.MsgBox("Bei unbekanntem Geschlecht ist keine Ehe möglich.");
            return;
        }
        if (Modul1.Typ != DriveType.CDRom)
        {
            new CLinkData(Modul1.eLKennz, famInArb, Modul1.PersInArb).AppendDB();
        }
        Modul1.Ubg = famInArb;
        Familie.Default.Family_ShowFamilyDlg(famInArb);
        Modul1.FamInArb = famInArb;
    }



    [RelayCommand]
    private void NoSources()
    {
        float num11;
        Quellen frmQuellen = MainProject.Forms.Quellen;
        num11 = 0f;
        Modul1.Ubg = 0;
        Modul1.Qkenn = 1;

        Modul1.PersInArb = PersonNr;


        frmQuellen.Show();
        frmQuellen.Button3.Enabled = false;

        frmQuellen.SourceLink_RefreshList(Modul1.Qkenn, Modul1.PersInArb);
        if (frmQuellen.ComboBox1.Items.Count > 0)
        {
            frmQuellen.Button3.Enabled = true;
        }

        DataModul.DB_PersonTable.Index = nameof(PersonIndex.PerNr);
        DataModul.DB_PersonTable.Seek("=", Modul1.PersInArb.AsString());
        if ((null != DataModul.DB_PersonTable.Fields[PersonFields.Bem3].Value) && Strings.Trim(DataModul.DB_PersonTable.Fields[PersonFields.Bem3].AsString()) != "")
        {
            frmQuellen.RTB.Text = DataModul.DB_PersonTable.Fields[PersonFields.Bem3].AsString();
        }
        _ = frmQuellen.RTB.Focus();
        frmQuellen.StartPosition = FormStartPosition.CenterScreen;
        frmQuellen.Visible = false;

        _ = frmQuellen.ShowDialog(Modul1.Ubg);
        return;

    }


    [RelayCommand]
    private void SearchNumber()
    {
        var prompt = "Nummer der gesuchten Person\rLeer,0 oder Abbrechen wechselt in die Suche nach Namen";
        var text3 = Interaction.InputBox(prompt, "Personensuche");
        Modul1.PersInArb = text3.AsInt();
        Modul1.Schalt = 0;
        if (Modul1.PersInArb == 0)
        {
            Modul1.Schalt = 2;
            Modul1.Suchfam = 0;
            Modul1.SuchPer = 0;

            Modul1.PersInArb = FrmSuchname_ShowDialog(text3);
        }
        if (Modul1.PersInArb != 0)
        {
            Perzeig(Modul1.PersInArb);
        }

    }

    private int FrmSuchname_ShowDialog(string text3)
    {
        Namensuch namensuch = MainProject.Forms.Namensuch;
        if (text3.Trim() != "")
        {
            if (Strings.Asc(text3) > 64)
            {
                namensuch.ViewModel.Text1_Text = text3;
            }
            else
            {
            }
        }
        if (namensuch.List1.SelectedIndex > 10)
        {
            namensuch.List1.TopIndex = namensuch.List1.SelectedIndex - 5;
        }
        _ = namensuch.ComboBox1.Focus();
        namensuch.ComboBox1.SelectionStart = namensuch.ComboBox1.Text.Length;
        if (!namensuch.ViewModel.Selection_Checked)
        {
            namensuch.ComboBox1.Text = "";
        }
        _ = namensuch.ShowDialog(PersonNr);

        return Modul1.SuchPer;
    }

    [RelayCommand(CanExecute = nameof(IsNotReadOnly))]
    private void NewPerson()
    {
        lblDisp_BackColor = Modul1.Feld1Farb;

        if (!IsNotReadOnly)
        {
            _ = Interaction.MsgBox(Modul1.Message_sNoChangesOnCD, title: "", icon: MessageBoxIcon.Information);
            return;
        }

        if (SaveToFamily_Visible)
        {
            if (FamNr != 0)
            {
                Modul1.eLKennz = Family_LinkKennz;
            }
            if (Modul1.Typ != DriveType.CDRom)
                DataModul.Link.Append(FamNr, PersonNr, Modul1.eLKennz);

            Modul1.Ubg = FamNr;
            Familie.Default.frmAppendPerson.Visible = false;
            Family_Update(FamNr);
            SaveToFamily_Visible = false;
            Delete_Visible = false;
        }
        if (!SaveNExit_Visible)
        {
        }
        if (Modul1.System.xDemo && DataModul.DB_PersonTable.RecordCount > 100)
        {
            _ = Interaction.MsgBox(Modul1.Message_sDemoVerNotPossibl);
        }
        else
        {
            Modul1.Trans = 1;
            int persInArb = Modul1.PersInArb;
            if (Modul1.Schalt != 5)
            {
                persInArb = 0;
                DataModul.DB_WDTable.MoveFirst();
                if (DataModul.DB_WDTable.Fields[WDFields.Nr].AsInt() == 1)
                {
                    if (DataModul.LeerTab_GetEmptyPerson(out persInArb))
                    {
                        DataModul.DB_PersonTable.Index = nameof(PersonIndex.PerNr);
                        DataModul.DB_PersonTable.Seek("=", persInArb);
                        if (DataModul.DB_PersonTable.NoMatch)
                        {
                            DataModul.Person_Sichlöschloesch(persInArb);
                        }
                    }
                }
                if (persInArb == 0)
                {
                    DataModul.DB_PersonTable.Index = nameof(PersonIndex.PerNr);
                    DataModul.DB_PersonTable.MoveLast();
                    persInArb = DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt();
                    persInArb++;
                }
            }
            else if (persInArb == 0)
            {
                persInArb = 1;
            }
            Modul1.PersInArb = persInArb;
            Modul1.Ad = true;
            Clear();
            Marriages_Text = Modul1.IText[EUserText.tMarrCount] + " 0";
            AncesterNr_Text = "";
            View.Show(persInArb, EUserText.t158);
            SetFocus(nameof(Sex_Text));
        }
        //=================


    }

    [RelayCommand]
    private void ShowPerson()
    {
        Namensuch.Default.SetPerson(PersonNr, 0, 9);
    }



    [RelayCommand]
    private void Command1_6()
    {
        List<int> Per1 = [];

        int persInArb = Modul1.PersInArb;
        var kennz = Modul1.eLKennz;
        var num7 = 1;
        foreach (var link in DataModul.Link.ReadAllPers(persInArb, ELinkKennz.lkMarrWitness))
        {
            Per1.Add(link.iFamNr);
            if (num7++ > 99) break;
        }

        foreach (var link in DataModul.Link.ReadAllPers(persInArb, ELinkKennz.lkWitnOfEngage))
        {
            Per1.Add(link.iFamNr);
        }

        num7 = 1;
        foreach (var link in DataModul.Link.ReadAllPers(persInArb, ELinkKennz.lkWitnOfMarr))
        {
            Per1.Add(link.iFamNr);
            if (num7++ > 99) break;
        }

        Rahmen.Default.ShowRahmenDialog(Modul1.IText[EUserText.t302], EUserText.t302, 3, Per1);

    }


    [RelayCommand]
    private void OpenPicture()
    {
        Modul1.Ubg = Modul1.PersInArb;
        Modul1.sPKennz = "P";
        MainProject.Forms.Bilder.Show();
    }

    [RelayCommand]
    private void Resarch()
    {
        Modul1.PersInArb = 0;
        _ = MainProject.Forms.RechText.ShowDialog();
        if (Modul1.PersInArb == 0)
        {
            Modul1.PersInArb = PersonNr;
        }
        Perzeig(Modul1.PersInArb);
    }

    [RelayCommand]
    private void WitnessIfNo()
    {
        string text;
        if (xWitnessIfNo_Tag)
        {
            return;
        }
        text = "";
        Rahmen.Default.List4.Items.Clear();
        Rahmen.Default.RTB.Text = "";
        Modul1.PersInArb = PersonNr;
        DataModul.DB_WitnessTable.Index = nameof(WitnessIndex.ElSu);
        DataModul.DB_WitnessTable.Seek("=", Modul1.PersInArb, "10");
        var aiWitt = new List<(EEventArt, short, int)>();
        int num6 = 0;
        while (!DataModul.DB_WitnessTable.EOF
                && !DataModul.DB_WitnessTable.EOF
                && !DataModul.DB_WitnessTable.NoMatch
                && !((DataModul.DB_WitnessTable.Fields[WitnessFields.PerNr].AsInt() != Modul1.PersInArb) || (DataModul.DB_WitnessTable.Fields[WitnessFields.Kennz].AsString() != "10"))
            )
        {
            num6++;
            EEventArt Witness_eArt = DataModul.DB_WitnessTable.Fields[WitnessFields.Art].AsEnum<EEventArt>();
            int Witness_iLfNr = DataModul.DB_WitnessTable.Fields[WitnessFields.LfNr].AsInt();
            int Witness_iPerNr = DataModul.DB_WitnessTable.Fields[WitnessFields.PerNr].AsInt();
            aiWitt.Add((Witness_eArt, (short)Witness_iLfNr, Witness_iPerNr));
            DataModul.DB_WitnessTable.MoveNext();
        }
        Rahmen.Default.RTB.Visible = false;
        Rahmen.Default.lblAsText.Visible = false;
        Rahmen.Default.btnAppend.Enabled = false;
        Rahmen.Default.btnDelete.Visible = false;
        Rahmen.Default.ShowWittDialog(aiWitt, 11, EUserText.t302);
        if (Rahmen.Default.eResult == ERahmenResult.eRR_OK)
        {
            return;
        }
        Perzeig(Modul1.PersInArb);
        if (Rahmen.Default.eResult == ERahmenResult.eRR_Removed)
        {
            LinkTo();
        }
    }

    [RelayCommand]
    private void NoWitnesses()
    {
        if (xNoWitnesses_Tag)
        {
            return;
        }
        var aiWitt = new List<(EEventArt eArt, short iLfNr, int iPerNr)>();
        Rahmen.Default.List4.Items.Clear();
        Rahmen.Default.RTB.Text = "";
        Modul1.PersInArb = PersonNr;
        foreach (var wtn in DataModul.Witness.ReadAllFams(Modul1.PersInArb, 10))
        {
            if (wtn.eArt < EEventArt.eA_500)
            {
                aiWitt.Add((wtn.eArt, wtn.iLfNr, wtn.iLink));
            }
        }
        Modul1.Ubg = 10;
        Rahmen.Default.btnAppend.Enabled = false;
        Rahmen.Default.btnDelete.Visible = false;
        Rahmen.Default.List4.Items.Clear();
        Rahmen.Default.RTB.Visible = false;
        Rahmen.Default.lblAsText.Visible = false;
        Rahmen.Default.ShowWittDialog(aiWitt, 10, EUserText.t301);
        if (Rahmen.Default.eResult == ERahmenResult.eRR_OK)
        {
            return;
        }
        Perzeig(Modul1.PersInArb);
        if (Rahmen.Default.eResult == ERahmenResult.eRR_Removed)
        {
            LinkTo();

        }
    }

    [RelayCommand]
    private void LinkTo()
    {
        Rahmen.Default.eResult = ERahmenResult.eRR_Removed;
        while (Rahmen.Default.eResult == ERahmenResult.eRR_Removed)
        {
            var aiFam = new List<int>();
            Rahmen.Default.List4.Items.Clear();
            Rahmen.Default.RTB.Text = "";
            Modul1.PersInArb = PersonNr;

            var num7 = 1;
            foreach (var Link in DataModul.Link.ReadAllPers(Modul1.PersInArb, ELinkKennz.lk9))
            {
                if (num7++ > 99) break;
                aiFam.Add(Link.iFamNr);
            }
            Modul1.Ubg = 2;
            Rahmen.Default.RTB.Visible = false;
            Rahmen.Default.lblAsText.Visible = false;
            Rahmen.Default.ShowDialog(Modul1.Ubg, aiFam, EUserText.t304);
            if (Rahmen.Default.eResult == ERahmenResult.eRR_OK)
            {
                break;
            }
            Perzeig(Modul1.PersInArb);
        }
    }

    [RelayCommand]
    private void LinkedPerson()
    {
        do
        {
            var aiPers = new List<int>();
            Rahmen.Default.List4.Items.Clear();
            Rahmen.Default.RTB.Text = "";
            Modul1.PersInArb = PersonNr;

            var num7 = 1;
            foreach (var Link in DataModul.Link.ReadAllFams(Modul1.PersInArb, ELinkKennz.lk9))
            {
                if (num7++ > 99) break;
                aiPers.Add(Link.iPersNr);
            }
            Modul1.Ubg = 1;
            Rahmen.Default.Close();
            Rahmen.Default.RTB.Visible = false;
            Rahmen.Default.lblAsText.Visible = false;
            Rahmen.Default.List4.Items.Clear();
            Rahmen.Default.RTB.Text = "";
            Rahmen.Default.ShowDialog(Modul1.Ubg, aiPers, EUserText.t348);
            if (Rahmen.Default.eResult == ERahmenResult.eRR_OK)
            {
                break;
            }
            Modul1.PersInArb = PersonNr;
            Perzeig(Modul1.PersInArb);
        } while (Rahmen.Default.eResult == ERahmenResult.eRR_Removed);
    }

    [RelayCommand]
    private void SaveToFamily()
    {
        short Rich;
        if (FamNr != 0)
        {
            Modul1.eLKennz = Family_LinkKennz;
        }
        ELinkKennz kennz = Modul1.eLKennz;
        if (IsNotReadOnly)
        {
            switch (kennz)
            {
                case ELinkKennz.lkChild:
                    _ = DataModul.Link.AppendE(FamNr, PersonNr, ELinkKennz.lkChild);
                    break;
                case ELinkKennz.lkFather:
                case ELinkKennz.lkMother:
                case ELinkKennz.lkAdoptedChild:
                    DataModul.Link.Append(FamNr, PersonNr, ELinkKennz.lkChild);
                    break;
                default:
                    break;
            }
            //=================
        }
        Modul1.Ubg = FamNr;
        Modul1.FamInArb = Modul1.Ubg;
        DoClose();
        Familie.Default.frmAppendPerson.Visible = false;
        DataModul.DB_FamilyTable.Index = nameof(FamilyIndex.Fam);
        DataModul.DB_FamilyTable.Seek("=", Modul1.FamInArb);
        if (!DataModul.DB_FamilyTable.NoMatch
            && (DataModul.DB_FamilyTable.Fields[FamilyFields.AnlDatum].AsDate() == DateTime.Today))
        {
            DataModul.DB_FamilyTable.Edit();
            DataModul.DB_FamilyTable.Fields[FamilyFields.EditDat].Value = DateTime.Today.Year.ToString() + DateTime.Today.Day.ToString() + DateTime.Today.Month.ToString();
            if (Modul1.Typ != DriveType.CDRom)
            {
                DataModul.DB_FamilyTable.Update();
            }
            //=================
        }
        FamNr = 0;
        Familie.Default.Show(Modul1.FamInArb);
        _ = Familie.Default.lstChildren.Focus();
    }

    [RelayCommand]
    private void SaveNExit()
    {
        Return_Visible = true;
        SearchNumber_Visible = true;
        NewPerson_Visible = true;
        Next_Visible = true;
        Previous_Visible = true;
        Delete_Visible = true;
        SaveNExit_Visible = false;
        Cancel_Visible = false;
        FamNr = 0;

        GodparentIfNo_Visible = true;
        NoGodparents_Visible = true;
        SearchAncestors_Visible = true;
        NewFamily_Visible = true;
        ShowPerson_Visible = true;
        SearchName_Visible = true;
        Resarch_Visible = true;
        RegisterSearch_Visible = true;
        ShowPlaces_Visible = true;
        Sorting_Visible = true;

        EUserText left = Rahmen.Default.iHdrText;
        switch (left)
        {
            case EUserText.tGodparents:
                Pat(EUserText.tGodparents, PersonNr, Rahmen.Default.DataHolder.iPerfam);
                break;
            case EUserText.tGodparentOf:
                Pat(EUserText.tGodparentOf, PersonNr, Rahmen.Default.DataHolder.iPerfam);
                break;
            case EUserText.tMarrWitness:
                Pat(EUserText.tMarrWitness, PersonNr, Rahmen.Default.DataHolder.iPerfam);
                break;
            case EUserText.tWitnOfEngage:
                Pat(EUserText.tWitnOfEngage, PersonNr, Rahmen.Default.DataHolder.iPerfam);
                break;
            case EUserText.tWitnOfMarr:
                Pat(EUserText.tWitnOfMarr, PersonNr, Rahmen.Default.DataHolder.iPerfam);
                break;
            case EUserText.t303: //Linked person
                Pat(EUserText.t303, PersonNr, Rahmen.Default.DataHolder.iPerfam);
                break;
            case EUserText.t304: //Linked o01_Person to
                Pat(EUserText.t304, PersonNr, Rahmen.Default.DataHolder.iPerfam);
                break;
            default:
                break;
        }
        if (Rahmen.Default.DataHolder.iPerfam == 0)
        {
        }

        if (Modul1.Typ != DriveType.CDRom)
        {
            DataModul.Witness.Add(PersonNr, Rahmen.Default.DataHolder.iPerfam, Rahmen.Default.DataHolder.Art, Rahmen.Default.DataHolder.LfNR, 10);
        }
        if (Rahmen.Default.DataHolder.Art > EEventArt.eA_499)
        {
            Modul1.FamInArb = Rahmen.Default.DataHolder.iPerfam;
            Familie.Default.Show(Modul1.FamInArb);
            DoClose();
            MainProject.Forms.Ereignis.Close();
            Rahmen.Default.DH_Clear();
            _ = MainProject.Forms.Ereignis.ShowEventDialog(Rahmen.Default.DataHolder.Art);
            //=================
        }
        else
        {
            PersonNr = Rahmen.Default.DataHolder.iPerfam;
            Perzeig(PersonNr);
            MainProject.Forms.Ereignis.Close();
            Modul1.LfNR = Rahmen.Default.DataHolder.LfNR;
            Rahmen.Default.DH_Clear();
            _ = MainProject.Forms.Ereignis.ShowEventDialog(Rahmen.Default.DataHolder.Art);
            Rahmen.Default.DH_Clear();
            //=================
        }
    }

    [RelayCommand]
    private void EndTextInput()
    {
        NoSources_Visible = true;

        View.edtNotes.Top = Modul1.Posi[0];
        View.edtNotes.Height = Modul1.Posi[1];

        FieldsOFB_Visible = true;
        if (SaveToFamily_Visible || SaveNExit_Visible)
        {
            Delete_Visible = true;
            SaveNExit_Enabled = true;
            SaveToFamily_Enabled = true;
            Cancel_Enabled = true;
            //=================
        }
        else
        {
            Return_Visible = true;
            SearchNumber_Visible = true;
            NewPerson_Visible = true;
            Next_Visible = true;
            Previous_Visible = true;
            Delete_Visible = true;
            SearchAncestors_Visible = true;
            NewFamily_Visible = true;
            ShowPerson_Visible = true;
            SearchName_Visible = true;
            Resarch_Visible = true;
            RegisterSearch_Visible = true;
            ShowPlaces_Visible = true;
            Sorting_Visible = true;
        }
        EndTextInput_Visible = false;
        Cancel_Visible = false;
        Perzeig(Modul1.PersInArb);
    }

    [RelayCommand]
    private void NoGodparents()
    {
        do
        {
            List<int> Per1 = [];
            Modul1.PersInArb = PersonNr;
            foreach (var link in DataModul.Link.ReadAllFams(Modul1.PersInArb, ELinkKennz.lkGodparent))
            {

                Per1.Add(link.iPersNr);
            }

            Rahmen.Default.ShowRahmenDialog("", EUserText.tGodparents, 1, Per1);
            Modul1.PersInArb = PersonNr;
            if (Rahmen.Default.eResult == ERahmenResult.eRR_OK)
            {
                break;
            }
            Perzeig(Modul1.PersInArb);
            Rahmen.Default.Close();
        }
        while (Rahmen.Default.eResult == ERahmenResult.eRR_Removed);
    }

    [RelayCommand]
    private void GodparentIfNo()
    {
        do
        {
            List<int> Per1 = [];
            Modul1.PersInArb = PersonNr;

            foreach (var link in DataModul.Link.ReadAllPers(Modul1.PersInArb, ELinkKennz.lkGodparent))
            {
                Per1.Add(link.iFamNr);
            }
            Rahmen.Default.ShowRahmenDialog("", EUserText.tGodparentOf, 2, Per1);
            if (Rahmen.Default.eResult == ERahmenResult.eRR_OK)
            {
                break;
            }
            Perzeig(Modul1.PersInArb);
        }
        while (Rahmen.Default.eResult == ERahmenResult.eRR_Removed);
    }

    [RelayCommand]
    private void Previous()
    {
        switch (Modul1.Aus[(int)EOutCfg.o21].AsInt())
        {
            case 1:

                Modul1.PersInArb = Modul1.Suchfeld[0] == ESearchSelection.eManual
                    ? DataModul.Person_DoSearch(PersonIndex.Such1, Search_Text, Modul1.PersInArb, true)
                    : DataModul.Person_DoSearch(PersonIndex.Such4, Search_Text, Modul1.PersInArb, true);
                //=================
                break;
            case 2:

                Modul1.PersInArb = Modul1.Suchfeld[1] == ESearchSelection.eManual
                    ? DataModul.Person_DoSearch(PersonIndex.Such2, Search2_Text, Modul1.PersInArb, true)
                    : DataModul.Person_DoSearch(PersonIndex.Such5, Search2_Text, Modul1.PersInArb, true);
                //=================
                break;
            case 3:

                Modul1.PersInArb = Modul1.Suchfeld[2] == ESearchSelection.eManual
                    ? DataModul.Person_DoSearch(PersonIndex.Such3, Search3_Text, Modul1.PersInArb, true)
                    : DataModul.Person_DoSearch(PersonIndex.Such6, Search3_Text, Modul1.PersInArb, true);
                break;
            case 0 when PersonNr > 1:
                Modul1.PersInArb = PersonNr - 1;
                break;
        }
        Modul1.Schalt = 4;
        Search2_Text = "";
        Perzeig(Modul1.PersInArb);
    }

    [RelayCommand]
    private void Next()
    {
        switch (Modul1.Aus[(int)EOutCfg.o21].AsInt())
        {
            case 1:
                Modul1.PersInArb = Modul1.Suchfeld[0] == ESearchSelection.eManual
                    ? DataModul.Person_DoSearch(PersonIndex.Such1, Search_Text, Modul1.PersInArb)
                    : DataModul.Person_DoSearch(PersonIndex.Such4, Search_Text, Modul1.PersInArb);
                break;
            case 2:
                Modul1.PersInArb = Modul1.Suchfeld[1] == ESearchSelection.eManual
                    ? DataModul.Person_DoSearch(PersonIndex.Such2, Search2_Text, Modul1.PersInArb)
                    : DataModul.Person_DoSearch(PersonIndex.Such5, Search2_Text, Modul1.PersInArb);
                break;
            case 3:
                Modul1.PersInArb = Modul1.Suchfeld[2] == ESearchSelection.eManual
                    ? DataModul.Person_DoSearch(PersonIndex.Such3, Search3_Text, Modul1.PersInArb)
                    : DataModul.Person_DoSearch(PersonIndex.Such6, Search3_Text, Modul1.PersInArb);
                break;
            case 0:
                Modul1.PersInArb = PersonNr + 1;
                break;
        }
        Modul1.Schalt = 3;
        Search2_Text = "";
        Perzeig(Modul1.PersInArb);
    }


    [RelayCommand(CanExecute = nameof(IsNotReadOnly))]
    private void Delete()
    {
        string text2;
        int num11;
        if (!IsNotReadOnly)
        {
            _ = Interaction.MsgBox(Modul1.Message_sNoChangesOnCD, title: "", icon: MessageBoxIcon.Information);
            return;
        }
        if (Interaction.MsgBox("Person " + Givennames_Text.Trim() + " " + Surnames_Text.Trim() + " wirklich löschen?", "Gefahr", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes)
        {
            return;
        }
        int PersInArb = PersonNr;
        Modul1.eLKennz = Sex_Text == "F" ? ELinkKennz.lkMother : ELinkKennz.lkFather;
        var ubgT = Modul1.Link_Famsuch(PersInArb, Modul1.eLKennz);
        if (ubgT == null)
        {
            if (ubgT.Count > 10)
            {
                text2 = Modul1.IText[EUserText.tPersonInFams];
            }
            else
                text2 = Modul1.IText[EUserText.tPersonInFam];
            if (Interaction.MsgBox(text2 + "\r(" + string.Join(", ", ubgT) + ")\r als Elternteil eingebunden", "Wirklich löschen?", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes)
            {
                return;
            }
        }
        if (Modul1.Eltsuch(PersInArb) > 0
            && Interaction.MsgBox(Modul1.IText[EUserText.tPersonInFam] + "\r      " + Modul1.Ubg.AsString() + "\r als Kind eingebunden", "Wirklich löschen?", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes)
        {
            return;
        }
        if (DataModul.Link.ExistE(PersInArb, ELinkKennz.lkAdoptedChild)
            && Interaction.MsgBox(Modul1.IText[EUserText.tPersonInFam] + "\r      " + Modul1.Ubg.AsString() + "\r als Adoptivkind eingebunden", "Wirklich löschen?", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes)
        {
            return;
        }
        if ((DataModul.Link.ExistF(PersInArb, ELinkKennz.lkGodparent)
            || DataModul.Link.ExistE(PersInArb, ELinkKennz.lkGodparent))
            && Interaction.MsgBox("Person ist als Pate/Zeuge eingebunden", "Wirklich löschen?", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
        {
            Perloesch(PersInArb);
        }
        else
        {
            if (DataModul.Witness.ExistE(PersInArb)
                && Interaction.MsgBox("Person ist als Zeuge eingebunden", icon: MessageBoxIcon.Exclamation, mb: MessageBoxButtons.YesNo, title: "Wirklich löschen?") == DialogResult.Yes)
            {
                Perloesch(PersInArb);
            }
        }
    }

    private void Text2_KeyPress(object eventSender, KeyPressEventArgs eventArgs)
    {
        short num = checked((short)Strings.Asc(eventArgs.KeyChar));
        if (num == 13)
        {
            num = 0;
        }
        eventArgs.KeyChar = Strings.Chr(num);
        if (num == 0)
        {
            eventArgs.Handled = true;
        }
    }

    public void Clear()
    {
        internalChange = true;
        var M1_Iter = 0;
        int i;
        int num;
        Occupation_Items.Clear();
        View.cbxOccupation.BackColor = Modul1.Feld1Farb;
        Title_Items.Clear();
        View.cbxTitle.BackColor = Modul1.Feld1Farb;
        Residence_Items.Clear();
        View.cbxResidence.BackColor = Modul1.Feld1Farb;
        Home_Items.Clear();
        View.cbxHome.BackColor = Modul1.Feld1Farb;
        NoGodparents_Text = Modul1.IText[EUserText.tGodparents] + ":" + Modul1.IText[EUserText.tNo];
        Color clBackColor = Color.FromArgb(0xE0E0E0);
        NoGodparents_BackColor = clBackColor;
        NoSources_Text = Modul1.IText[EUserText.t244] + ":" + Modul1.IText[EUserText.tNo];
        NoSources_BackColor = clBackColor;
        LinkedPerson_Text = Modul1.IText[EUserText.t303] + ":" + Modul1.IText[EUserText.tNo];
        LinkedPerson_BackColor = clBackColor;
        LinkTo_Text = Modul1.IText[EUserText.t303] + ":" + Modul1.IText[EUserText.tNo];
        LinkTo_BackColor = clBackColor;
        NoWitnesses_Text = Modul1.IText[EUserText.t263] + ":" + Modul1.IText[EUserText.tNo];
        NoWitnesses_BackColor = clBackColor;
        WitnessIfNo_Text = Modul1.IText[EUserText.t302] + ":" + Modul1.IText[EUserText.tNo];
        WitnessIfNo_BackColor = clBackColor;
        Picture_Visible = false;
        Occupation_Items.Clear();
        Occupation_SelectedItem = null;
        Title_Items.Clear();
        Title_SelectedItem = null;
        Residence_Items.Clear();
        Residence_SelectedItem = null;
        Home_Items.Clear();
        Home_SelectedItem = null;
        Additional_Items.Clear();
        Additional_SelectedItem = null;
        View.cbxAdditional.BackColor = Modul1.Feld1Farb;
        NachfNr = 0;
        View.lblNachfNr.BackColor = View.BackColor;
        Search2_Text = "";
        Sex_Text = "";
        Religion_Text = "";
        Surnames_Text = "";
        Prefix_Text = "";
        Suffix_Text = "";
        Givennames_Text = "";
        Alias_Text = "";
        Predicate_Text = "";
        Clan_Text = "";
        Status_Text = "";
        Family1_Text = "";
        CreationDate_Text = Modul1.IText[EUserText.tCreationDt] + " " + DateTime.Today.Month.ToString() + "." + DateTime.Today.Day.ToString() + "." + DateTime.Today.Year.ToString();
        Notes_Text = "";
        Search_Text = "";
        Search2_Text = "";
        Search3_Text = "";
        List1_Items.Clear();
        List2_Items.Clear();
        MainProject.Forms.OFB.Close();
        FieldsOFB_BackColor = NewPerson_BackColor;
        internalChange = false;
    }

    public void PerSatzLes(int persInArb)
    {

        if (persInArb < 1)
        {
            persInArb = 1;
        }

        var dB_PersonTable = DataModul.DB_PersonTable;
        if (dB_PersonTable.RecordCount == 0)
        {
            persInArb = 0;
            Modul1.Schalt = 5;
            NewPerson();
            goto end_IL_0001_2;
            //=================
        }

        int iMinPersNr;
        int iMaxPersNr;
        Person_GetMinMax(out iMinPersNr, out iMaxPersNr);
        if (persInArb < iMinPersNr)
        {
            persInArb = iMinPersNr;
        }

        dB_PersonTable.Index = nameof(PersonIndex.PerNr);
        dB_PersonTable.Seek("=", persInArb.AsString());
        var xTested = false;
        while (dB_PersonTable.NoMatch || !xTested)
        {
            if (dB_PersonTable.NoMatch)
            {
                switch (Modul1.Schalt)
                {
                    case 0:
                    case 4:
                        if (persInArb > 1 & persInArb < iMaxPersNr)
                        {
                            persInArb--;
                            //=================
                        }
                        else if (persInArb > iMaxPersNr)
                        {
                            persInArb = iMaxPersNr;
                            //=================
                        }

                        dB_PersonTable.Seek("=", persInArb.AsString());
                        continue;
                    case 3:
                        if (persInArb > 1 & persInArb < iMaxPersNr)
                        {
                            persInArb++;
                        }
                        else if (persInArb > iMaxPersNr)
                        {
                            persInArb = iMaxPersNr;
                            //=================
                        }

                        dB_PersonTable.Seek("=", persInArb.AsString());
                        continue;
                    default:
                        break;
                }
                break;
            }
            else
            {
                var Person_fPruefen = dB_PersonTable.Fields[PersonFields.Pruefen];
                if (Person_fPruefen.AsString() == "G")
                {
                    string prompt = "Diese Person ist gelöscht und kann neu belegt werden";
                    if (Interaction.MsgBox(prompt, "Jetzt neu eingeben", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                    {
                        if (Modul1.Typ != DriveType.CDRom)
                        {
                            dB_PersonTable.Edit();
                            Person_fPruefen.Value = "     ";
                            dB_PersonTable.Update();
                            xTested = true;
                        }
                        break;
                    }
                    else
                    {
                        persInArb++;
                        continue;
                    }
                }
                else
                    xTested = true;
            }
        }
        if (!xTested)
            goto end_IL_0001_2;

        DateTime Person_dAnlDatum = dB_PersonTable.Fields[PersonFields.AnlDatum].AsDate();
        DateTime Person_dEditDat = dB_PersonTable.Fields[PersonFields.EditDat].AsDate();
        int Person_iPersNr = dB_PersonTable.Fields[PersonFields.PersNr].AsInt();

        CreationDate_Text = Person_dAnlDatum.AsString();
        CreationDate_Tag = Person_dAnlDatum;
        Family1_Tag = Person_dEditDat;
        if (Person_dEditDat > DateTime.MinValue)
        {
            Family1_Text = Person_dEditDat.ToString();
        }
        Age_Text = Modul1.IText[EUserText.t116];
        PersonNr = Person_iPersNr;

        Modul1.Kont[0] = Person_dAnlDatum.AsString();
        if (Modul1.Kont[0].Length > 0)
        {
            Modul1.UbgT = Modul1.Kont[0];
            Modul1.Kont[0] = Modul1.IText[EUserText.tCreationDt] + " " + Strings.Mid(Modul1.UbgT, 7, 2) + "." + Strings.Mid(Modul1.UbgT, 5, 2) + "." + Modul1.UbgT.Left(4);
            //=================
        }
        else
        {
            Modul1.Kont[0] = Modul1.IText[EUserText.tCreationDt] + " " + DateTime.Today.Month.ToString() + "." + DateTime.Today.Day.ToString() + "." + DateTime.Today.Year.ToString();
            //=================
        }
        if (Person_iPersNr.AsInt() > 0.0)
        {
            Modul1.UbgT = Person_iPersNr.AsString();
            Modul1.Kont[2] = Modul1.IText[EUserText.t114] + ": " + Strings.Mid(Modul1.UbgT, 7, 2) + "." + Strings.Mid(Modul1.UbgT, 5, 2) + "." + Modul1.UbgT.Left(4);
            //=================
        }
        else
        {
            Modul1.Kont[2] = "";
            //=================
        }
        goto end_IL_0001_2;
    //=================
    end_IL_0001_2: // <========== 3
        return;
    }

    private static void Person_GetMinMax(out int iMinPersNr, out int iMaxPersNr)
    {
        DataModul.DB_PersonTable.Index = nameof(PersonIndex.PerNr);
        DataModul.DB_PersonTable.MoveFirst();
        iMinPersNr = DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt();
        DataModul.DB_PersonTable.MoveLast();
        iMaxPersNr = DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt();
    }

    private string SetSuchfeldTxt(bool xCond, string sTrue, string text, string sFalse)
        => (xCond ? sTrue : sFalse) is string s && s != "" ? s : text;

    private void ZeigBeruf(int persInArb, EEventArt Beruf, Label lalBeruf, ComboBox comboBox)
    {
        lalBeruf.Text = "";
        lalBeruf.BackColor = comboBox.BackColor;
        Modul1.Berufles(persInArb, Beruf, comboBox);
        if (comboBox.Items.Count > 0 && comboBox.Text.Trim() == "")
        {
            comboBox.Text = comboBox.Items[0].AsString();
        }

        if (comboBox.Items.Count > 1)
        {
            lalBeruf.BackColor = Color.FromArgb(12648447);
        }
    }

    private short Beruf2Index(int Beruf) => Beruf switch
    {
        >= 300 and < 600 => (short)(Beruf - 300),
        105 => 10,
        106 => 3,
        _ => 0,
    };

    private void Beruf2Label(int persInArb)
    {
        Label label = View.lblOccubation;
        ComboBox comboBox = View.cbxOccupation;

        label.Text = "";
        label.BackColor = comboBox.BackColor;

        Modul1.Berufles(persInArb, EEventArt.eA_300, comboBox);

        if (comboBox.Items.Count > 0
            && comboBox.Text.Trim() == "")
        {
            comboBox.Text = comboBox.Items[0].ToString();
        }
        if (comboBox.Items.Count > 1)
        {
            label.BackColor = Color.FromArgb(0xC0FFFF);
        }
    }

    public void Perloesch(int persInArb)
    {
        ////Discarded unreachable code: IL_15b6
        //int try0001_dispatch = -1;
        //int num3 = default;
        //int num2 = default;
        //int num = default;
        //int lErl = default;
        //while (true)
        //{
        //    try
        //    {
        //        /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
        //        ;
        //        checked
        //        {
        //            int num4;
        //            switch (try0001_dispatch)
        //            {
        //                default:
        //                    ProjectData.ClearProjectError();
        //                    num3 = 2;
        //                    goto IL_0008;
        //                case 6928:
        //                    {
        //                        num2 = num;
        //                        switch (num3 <= -2 ? 1 : num3)
        //                        {
        //                            case 2:
        //                                break;
        //                            case 3:
        //                            case 4:
        //                                goto IL_15e2;
        //                            case 1:
        //                                goto IL_166c;
        //                            //=================
        //                            default:
        //                                goto IL_1b58;
        //                        }
        //                        break;
        //                    }
        //                end_IL_0001: // <========== 8
        //                             // <========== 8
        //                    break;
        //                IL_0008:
        //                    num = 2;
        do
        {
            if (!IsNotReadOnly)
                break;

            DataModul.Namen_RemovePerson(persInArb);
            DataModul.TTable_RemovePerson(persInArb, 1);
            DataModul.TTable_RemovePerson(persInArb, 3);
            var UVar = 1;
            while (UVar <= 9)
            {
                Modul1.eLKennz = UVar.AsEnum<ELinkKennz>();
                _ = DataModul.Link.DeleteAllE(persInArb, Modul1.eLKennz);
                UVar += 1;
            }

            Modul1.eLKennz = ELinkKennz.lkGodparent;
            _ = DataModul.Link.DeleteAllF(persInArb, Modul1.eLKennz);
            //=================
            DataModul.Witness_DeleteAllE(persInArb);
            DataModul.Witness_DeleteAllF(persInArb);
            DataModul.Ancestors_DeleteAll(persInArb);
            DataModul.Descendents_DeleteAll(persInArb);
            DataModul.Events_DeleteAllPersVitEv(persInArb);
            DataModul.Event.DeleteAllNonVitalE(persInArb);
            DataModul.Search_DeletePeson(persInArb);
            DataModul.Pictures_DeletePerson(persInArb);
            if (persInArb == 1)
            {
                DataModul.DB_PersonTable.Seek("=", persInArb);
                if (!DataModul.DB_PersonTable.NoMatch)
                {
                    DataModul.Person_Clear_EntryRaw("C", "G");

                    Clear();
                }
                break;
            }
            else
            {
                DataModul.DB_PersonTable.Index = nameof(PersonIndex.PerNr);
                DataModul.DB_PersonTable.Seek("=", persInArb.AsString());
                if (!DataModul.DB_PersonTable.NoMatch)
                {
                    DataModul.DB_PersonTable.Delete();
                }

                DataModul.LeerTab_AddRaw(persInArb, "P");

                if (View.Visible & !Dublicates_Visible)
                {
                    _ = Interaction.MsgBox("Person wurde gelöscht");
                }

                PersonNr = 0;
                if (Pschalt == 1f || !View.Visible)
                {
                    break;
                }

                Modul1.Trans = 0;
                Previous();
                break;
            }
        } while (false);
        //                //=================
        //                IL_15e2: // <========== 3
        //                         // <========== 3
        //                    num = 285;
        //                    if (Information.Err().Number == 3022)
        //                    {
        //                        ProjectData.ClearProjectError();
        //                        if (num2 == 0)
        //                        {
        //                            throw ProjectData.CreateProjectError(cDefaultErr);
        //                        }
        //                        goto IL_166c;
        //                        //=================
        //                    }
        //                    else
        //                    {
        //                        if (Interaction.MsgBox(Conversions.ErrorToString(), MessageBoxButtons.OKCancel, (Information.Err().Number).AsString()) == DialogResult.Cancel)
        //                        {
        //                            ProjectData.EndApp();
        //                        }
        //                        ProjectData.ClearProjectError();
        //                        if (num2 == 0)
        //                        {
        //                            throw ProjectData.CreateProjectError(cDefaultErr);
        //                        }
        //                        num4 = num2;
        //                        goto IL_166f;
        //                        //=================
        //                    }
        //                IL_166c:
        //                    num4 = unchecked(num2 + 1);
        //                    goto IL_166f;
        //                //=================
        //                IL_166f:
        //                    num2 = 0;
        //                    switch (num4)
        //                    {
        //                        case 1:
        //                            break;
        //                        case 247:
        //                            goto end_IL_0001;
        //                        case 248:
        //                            goto end_IL_0001;
        //                        case 271:
        //                            goto end_IL_0001;
        //                        case 274:
        //                            goto end_IL_0001;
        //                        case 279:
        //                            goto end_IL_0001;
        //                        case 280:
        //                            goto end_IL_0001;
        //                        case 281:
        //                            goto end_IL_0001;
        //                        case 282:
        //                            num = 282;
        //                            goto IL_15e2;
        //                        case 284:
        //                        case 285:
        //                            goto IL_15e2;
        //                        case 293:
        //                            goto end_IL_0001;
        //                            //=================
        //                    }
        //                    goto default;
        //                    //=================
        //            }
        //        }
        //    }
        //    catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
        //    {
        //        ProjectData.SetProjectError(obj, lErl);
        //        try0001_dispatch = 6928;
        //        continue;
        //    }
        //    break;
        //IL_1b58:
        //    throw ProjectData.CreateProjectError(-2146828237);
        //}
        //if (num2 != 0)
        //{
        //    ProjectData.ClearProjectError();
        //}
    }


    [RelayCommand]
    private void SearchAncestors()
    {
        var num5 = 0;
        var num6 = 0;
        var num7 = 0;
        var num8 = 0;
        var nr = 0;
        int M1_Iter = 0;
        Modul1.Frauenkek1 = 0;
        Modul1.Frauenkek2 = 0;

        Modul1.Verz1 = Modul1.Verz.Left(15);
        if (Modul1.System.VerSpecial == 1)
        {
            Modul1.Verz1 = Modul1.Verz.Left(20);
        }

        DataModul.CreateNewNBDatabase(Modul1.Persistence);

        Modul1.Schalt = 0;
        Modul1.PersInArb = PersonNr;
        Modul1.Startpers = PersonNr;
        int Gen1 = 0;
        if (Modul1.PersInArb == 0)
        {
            _ = Interaction.MsgBox("Stop", title: "4", icon: MessageBoxIcon.Error);
            return;
            //=================
        }
        else
        {
            nr = 1;
            num6 = nr - 1;
            Gen1 = 5;
            num7 = 0;
            num8 = 1;
            M1_Iter = 1;
        }
        while (Modul1.PersInArb > 0)
        {
            if (IsNotReadOnly)
            {
                DataModul.NB_Ahnen.Commit(Modul1.PersInArb, Modul1.FamInArb, num5, num8, num7, Modul1.Person.SurName);
            }
            //=================

            //=================

            Modul1.Schalt = 1;
            var aiFams = Modul1.Link_Famsuch(Modul1.PersInArb, Modul1.eLKennz = ELinkKennz.lkChild);
            Modul1.FamInArb = aiFams.FirstOrDefault();
            num5++;
            if (!(num5 == 135 | num5 > Gen1 + 1) && Modul1.FamInArb != 0)
            {
                Modul1.Family.Mann = 0;
                Modul1.Family.Frau = 0;
                Modul1.Ubg = 0;
                DataModul.Link.ReadFamily(Modul1.FamInArb, Modul1.Family);
                Modul1.PersInArb = Modul1.Family.Mann;
                if (Modul1.Schalt < 2)
                {
                    num8 *= 2;
                }
                if (Math.Abs(num8) >= 1e9)
                {
                    int num9 = (int)(num8 / 1e9);
                    num8 -= (int)(num9 * 1e9);
                    num7 += num9;
                }
                if (Modul1.Schalt < 2)
                {
                    Modul1.Frauenkek2 = num7;
                    Modul1.Frauenkek1 = num8 + 1;
                    //=================
                }
                else
                {
                    _ = Interaction.MsgBox("3");
                }

                if (IsNotReadOnly)
                {
                    if (Modul1.Family.Frau > 0)
                    {
                        if (!DataModul.NB_Frau.PersonExists(Modul1.Family.Frau))
                        {
                            DataModul.NB_Frau.AddRow(nr, num5, Modul1.Family.Frau, Modul1.FamInArb, Modul1.Frauenkek1, Modul1.Frauenkek2);
                        }
                        else
                        {
                            DataModul.NB_Frau.AddRow(nr, num5, Modul1.Family.Frau, Modul1.FamInArb, Modul1.Frauenkek1, Modul1.Frauenkek2);
                        }
                        nr++;
                    }
                    //=================
                }
                Modul1.Family.Frau = 0;
                if (Modul1.Family.Mann > 0)
                {
                }
                else
                {
                    M1_Iter++;
                    //=================
                }
                continue;
            }
            num6++;

            if (DataModul.NB_Frau.CReadData(num6, out var value))
            {
                (num5, Modul1.PersInArb, Modul1.FamInArb) = value.Value;
            }
            else
                break;
        }

    }

    public void Perschreib(int ubg2)
    {
        if (!IsNotReadOnly)
        {
            return;
        }
        if (Sex_Text.Trim() == "")
        {
            _ = Interaction.MsgBox(sSexMustBeAssigned, "Eingabefehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            SetFocus(nameof(Sex_Text));
            return;
        }
        Modul1.Person_TextSpeichern(Surnames_Text, PersonNr, ETextKennz.tkName);
        Modul1.Person_TextSpeichern(Prefix_Text, PersonNr, ETextKennz.A_);// Prefix
        Modul1.Person_TextSpeichern(Suffix_Text, PersonNr, ETextKennz.B_);// Suffix
        Modul1.Person_TextSpeichern(Alias_Text, PersonNr, ETextKennz.C_);// Alias
        Modul1.Person_TextSpeichern(Status_Text, PersonNr, ETextKennz.U_);// Status
        Modul1.Person_TextSpeichern(Predicate_Text, PersonNr, ETextKennz.tk2_);// Predicate

        if (Surnames_Text.Trim() == "")
        {
            _ = Interaction.MsgBox("Person hat keinen Namen");
        }

        Modul1.PersInArb = PersonNr;
        var person = new CPersonData(PersonNr);
        person.dEditDat = DateTime.Now;
        person.SetSex(Sex_Text.Trim().ToUpper().Left(1));

        if (Religion_Text.Trim().Length > DataModul.DB_PersonTable.Fields[PersonFields.Konv].Size)
        {
            string text = "Der Text für die Religion darf bei diesem Mandanten max. " + DataModul.DB_PersonTable.Fields[PersonFields.Konv].Size.AsString() + " Zeichen lang sein\n";
            text += "In einem neuen Mandanten kann die Länge bis zu 240 Zeichen betragen!";
            _ = Interaction.MsgBox(text, title: "", icon: MessageBoxIcon.Error);
        }
        DataModul.DB_PersonTable.Fields[PersonFields.Konv].Value = Religion_Text.Trim().Left(DataModul.DB_PersonTable.Fields[PersonFields.Konv].Size);

        string Wort = Religion_Text.Trim();
        int Satz = DataModul.Texte_Schreib(Wort, "", ETextKennz.tk7_);
        person.iReligi = Satz;

        int M1_Iter = 1;
        while (Notes_Text.Length != 0
            && Strings.Asc(Notes_Text.Right(1)) < 14
            && M1_Iter <= 20)
        {
            Notes_Text = Notes_Text.Left(Notes_Text.Length - 1);
            M1_Iter++;
        }
        if (Notes_Text.TrimEnd() == "")
        {
            Notes_Text = " ";
        }
        person.sBem[1] = Notes_Text;
        person.sPruefen = "0";

        if (Modul1.Suchfeld[0] == ESearchSelection.eManual)
        {
            person.sSuch[1] = Search_Text.Trim() + " ";
        }
        if (Modul1.Suchfeld[1] == ESearchSelection.eManual)
        {
            person.sSuch[2] = Search2_Text.Trim() + " ";
        }
        if (Modul1.Suchfeld[2] == ESearchSelection.eManual)
        {
            person.sSuch[3] = Search3_Text.Trim() + " ";
        }
        if (IsNotReadOnly)
        {
            person.Update();
        }
    }

    public void Zeigfam(string Such)
    {

        //if (IsNotReadOnly)
        //{
        //    var dSB_SearchTable1 = DataModul.DSB_SearchTable;
        //    dSB_SearchTable1.Edit();
        //    dSB_SearchTable1.Fields["Kenn"].Value = "9";
        //    dSB_SearchTable1.Commit();
        //}

        var num3 = 2;
        var dSB_SearchTable = DataModul.DSB_SearchTable;

        Duplicates_Items.Clear();
        string value = Sex_Text.Trim();
        Such = Such.ToUpper();
        short num6 = 0;
        if (Such.Right(1) == "Ä"
            || Such.Right(1) == "Ö"
            || Such.Right(1) == "Ü"
            || Strings.InStr(Such, "ß") != 0)
        {
            num6 = 1;
        }

        dSB_SearchTable.Index = "Persuch";
        Such = Such.Uml2Such();
        dSB_SearchTable.MoveFirst();
        if (num6 == 1)
        {
            Such = Such.Left(Such.Length - 1);
        }
        dSB_SearchTable.Seek(">=", Such, 0);
        if (dSB_SearchTable.EOF)
        {
            if (Duplicates_Items.Count > 0)
            {
                Duplicates_Items.Add(new("------------ Ende der Liste-----------"));
            }
            return;
            //=================
        }

        short num4 = 0;
        //int PersInArb = Modul1.PersInArb;
        int recordCount = dSB_SearchTable.RecordCount;
        var M1_Iter = 1;
        while (!dSB_SearchTable.EOF
        && M1_Iter <= recordCount
                && !(Duplicates_Items.Count >= Modul1.Aus[(int)EOutCfg.o12].AsDouble()))
        {
            int SearchTable_iNummer = dSB_SearchTable.Fields["Nummer"].AsInt();
            string SearchTable_sName = dSB_SearchTable.Fields["Name"].AsString().ToUpper();
            string SearchTable_sIKenn = dSB_SearchTable.Fields["iKenn"].AsString();
            DateTime SearchTable_dDatum = dSB_SearchTable.Fields["Datum"].AsDate();
            string SearchTable_sSich = dSB_SearchTable.Fields["Sich"].AsString();

            Modul1.PersInArb = SearchTable_iNummer;

            string Person_sSex = DataModul.Person.GetSex(SearchTable_iNummer);

            if ((value.ToUpper() != "M" || !(Person_sSex == "F"))
                && (value.ToUpper() != "F" || !(Person_sSex == "M")))
            {

                string UbgT = SearchTable_sName.Uml2Such();
                DuplLabel6_Text = "";
                Display_Text = Modul1.IText[EUserText.t399] + " " + Modul1.Aus[(int)EOutCfg.o12] + " " + Modul1.IText[EUserText.t84_Persons];
                if (UbgT.Left(Such.Length) != Such.ToUpper())
                {
                    DuplLabel6_Text = Modul1.IText[EUserText.t400] + ":" + Duplicates_Items.Count.AsString() + " " + Modul1.IText[EUserText.t84_Persons];
                    return;
                    //=================
                }

                string text = " " + SearchTable_sIKenn;
                string text2 = $"{text}{SearchTable_dDatum.Year,4}";
                if ("" != SearchTable_sSich)
                {
                    text2 += SearchTable_sSich;
                }
                text2 = text2.PadRight(7);
                if (SearchTable_dDatum == default)
                {
                    text2 = "       ";
                }

                text2 = DataModul.Link.ExistE(SearchTable_iNummer, ELinkKennz.lkChild) ? "K " + text2 : "  " + text2;

                Modul1.Person_ReadNames(SearchTable_iNummer, Modul1.Person);
                Modul1.Person.SetFullSurname(Modul1.BuildFullSurName(Modul1.Person));
                string text3 = Modul1.Person.FullSurName.TrimEnd() + "," + Modul1.Person.Givennames;

                {
                    var aiFam = Modul1.Ehesuch(SearchTable_iNummer, Person_sSex);
                    Modul1.eLKennz = Person_sSex == "F" ? ELinkKennz.lkFather : ELinkKennz.lkMother;
                    short num14;
                    if (aiFam.Count != 0)
                    {
                        if (!aiFam.Contains(FamNr))
                        {
                            int num12 = aiFam.Count;
                            int num9 = 1;
                            while (num9 <= num12)
                            {
                                int Fam = aiFam[num9];
                                Famzeig(Fam, out var Modul1_LiText, Modul1.eLKennz);
                                num14 = (short)Strings.InStr(text3, ",");
                                if (num14 > 25)
                                {
                                    text3 = text3.Left(25) + Strings.Mid(text3, num14, text3.Length);
                                }
                                Duplicates_Items.Add(new($"{text3:38} {text2} {dSB_SearchTable.Fields["Nummer"].AsString():20} {Modul1_LiText} {Fam}", Fam));
                                num4++;
                                Modul1_LiText = "";
                                num9++;
                                //=================
                            }
                        }
                    }
                    else
                    {
                        num14 = (short)Strings.InStr(text3, ",");
                        if (num14 > 25)
                        {
                            text3 = text3.Left(25) + Strings.Mid(text3, num14, text3.Length);
                        }
                        Duplicates_Items.Add(new($"{text3:38} {text2} {dSB_SearchTable.Fields["Nummer"].AsString():10}"));
                        num4 = (short)(num4 + 1);
                    }
                }
            }
            dSB_SearchTable.MoveNext();
            M1_Iter++;
            //=================

        }
        if (Duplicates_Items.Count > 0)
        {
            Duplicates_Items.Add(new("------------ Ende der Liste-----------"));
            //=================
        }
        DuplLabel6_Text = Modul1.IText[EUserText.t400] + ":" + Duplicates_Items.Count.AsString() + " " + Modul1.IText[EUserText.t84_Persons];
    }
    public void Famzeig(int Fam, out string LiText, ELinkKennz eLKennz)
    {
        EEventArt[] aeConEvent = {
            EEventArt.eA_Marriage,  EEventArt.eA_MarrReligious, EEventArt.eA_504,  EEventArt.eA_505,
            EEventArt.eA_506,  EEventArt.eA_507, EEventArt.eA_500,  EEventArt.eA_501, EEventArt.eA_601
        };

        Modul1.sDatu = "";
        EEventArt num = default; // EEventArt

        foreach (var e in aeConEvent)
            if (GetEvtYear(Fam, num = e, out var sDate))
            {
                Modul1.sDatu = sDate;
                break;
            }
        string text = num switch
        {
            EEventArt.eA_500 => "Prok.",//Proklamation
            EEventArt.eA_501 => "Verl.",//Verlobung
            EEventArt.eA_Marriage => "Heir.",//Heirat
            EEventArt.eA_MarrReligious => "kirH.",//kirchliche Heirat
            EEventArt.eA_504 => "Scheid.",//Scheidung
            EEventArt.eA_505 => "Eheä.",//Eheähnliche Gemeinschaft
            EEventArt.eA_507 => "Dim.",//Dimissoriale
            EEventArt.eA_601 => "FiHr.",//Findbuch Heirat
            _ => "    ",
        };

        LiText = new string(' ', 80);
        if (DataModul.Link.GetFamPerson(Fam, eLKennz, out int iParent))
        {
            var person = new CPersonData();
            Modul1.Person_ReadNames(iParent, person);
            LiText = $"{text,-6} {Modul1.sDatu.Trim(),4} mit {person.SurName.Trim().ToUpper() + ", " + person.Givennames,35} {iParent,10}";
        }
        else
        {
            LiText = $"{text,-6} {Modul1.sDatu.Trim(),4} mit {Modul1.IText[EUserText.tUnknown],35} {0,10}";
        }
        if (Modul1.sDatu.Trim() == "")
        {
            if (!DataModul.Family.Get_Aeb(Fam))
            {
                LiText = $"  {"Ausserehel. ",-13} {"",4}     {"",35} {"",10}";
            }

        }

        static bool GetEvtYear(int iLink, EEventArt num2, out string sDate)
        {
            DateTime dt;
            if ((dt = DataModul.Event.GetDate(num2, iLink)) != default)
            {
                sDate = $"{dt.Year,4}";
                return true;
            }
            else
            {
                sDate = "    ";
            }

            return false;
        }
    }


    public void ParentToList(int Fams, short U, IList<ListItem<PersonFamily>> items)
    {
        if (Fams > 0)
        {
            if (DataModul.Link.GetFamPerson(Fams, ELinkKennz.lkFather, out int iFather))
            {
                Modul1.Person_ReadNames(iFather, Modul1.Person);
                Modul1.Person.SetFullSurname(Modul1.BuildFullSurName(Modul1.Person));
                items.Insert(U + 1, new ListItem<PersonFamily>($"{iFather,10} {Modul1.Person.Givennames} {Modul1.Person.FullSurName}", (iFather, Fams)));
            }
            else
            {
                items.Insert(U + 1, new ListItem<PersonFamily>("         ", (0, 0)));
            }
            if (DataModul.Link.GetFamPerson(Fams, ELinkKennz.lkFather, out int iMother))
            {
                Modul1.Person_ReadNames(iMother, Modul1.Person);
                Modul1.Person.SetFullSurname(Modul1.BuildFullSurName(Modul1.Person));
                items.Insert(U + 2, new ListItem<PersonFamily>($"{iMother,10} {Modul1.Person.Givennames} {Modul1.Person.FullSurName}", (iMother, Fams)));
            }
            else
            {
                items.Insert(U + 2, new ListItem<PersonFamily>("         ", (0, 0)));
            }
        }
    }

    private void Textbox_Alias_KeyUp(object sender, KeyEventArgs e)
    {
        short num = checked((short)e.KeyCode);
        short num2 = num;
        if (num == 13)
        {
            List3_Visible = false;
            Clan_SetFocus();
            List3_Items.Clear();
            Residence_Visible = true;
            GodparentIfNo_Visible = true;
            NoGodparents_Visible = true;
            NoSources_Visible = true;
            _ = Modul1.TextSpeich(Alias_Text.Left(240), "", ETextKennz.C_, num2, Modul1.LfNR);
        }
    }

    private void Clan_SetFocus()
    {
        throw new NotImplementedException();
    }

    private void Button3_Click(object sender, EventArgs e)
    {
        MainProject.Forms.Namensuch.SetPerson(List5_SelectedItem.ItemData<int>(), 0, 9);
    }


    public void Perzeig(int persInArb)
    {
        var num = 1;
        MainProject.Forms.Bilder.Close();

        Cursor.Current = Cursors.WaitCursor;
        DataModul.Convert_OldReligion();
        Cursor.Current = Cursors.Default;


        var dB_PersonTable = DataModul.DB_PersonTable;
        if (dB_PersonTable.RecordCount == 0)
        {
            Modul1.PersInArb = 1;
            Modul1.Schalt = 5;
            NewPerson();
            return;
            //=================
        }

        ShowPlaces_Visible = false;
        Persistence.DeleteTempFile("GenFreeWin.kml");
        Clear();
        PerSatzLes(persInArb);
        FieldsOFB_BackColor = NewPerson_BackColor;
        var ubg = 0;
        if (DataModul.Names.ExistsNK(persInArb, ETextKennz.Y_))
            ubg = 1;

        if (DataModul.OFB.Exists(OFBIndex.InDNr, persInArb, "NN"))
            ubg = 1;

        if (DataModul.OFB.Exists(OFBIndex.InDNr, persInArb, "EE"))
            ubg = 1;

        if (DataModul.OFB.Exists(OFBIndex.InDNr, persInArb, "OO"))
            ubg = 1;

        if (ubg == 1)
        {
            FieldsOFB_BackColor = Color.FromArgb(0x0000FF);
        }

        var Person = new CPersonData(dB_PersonTable);

        if (Person.sOFB == "J")
        {
            FieldsOFB_BackColor = Color.FromArgb(255);
        }

        if (Person.sPruefen.Trim() == "G")
        {
            return;
        }

        Search_Text = SetSuchfeldTxt(Modul1.Suchfeld[0] > ESearchSelection.eManual, Search_Text, Person.sSuch[4], Person.sSuch[1]);
        Search2_Text = SetSuchfeldTxt(Modul1.Suchfeld[1] > ESearchSelection.eManual, Search2_Text, Person.sSuch[5], Person.sSuch[2]);
        Search3_Text = SetSuchfeldTxt(Modul1.Suchfeld[2] > ESearchSelection.eManual, Search3_Text, Person.sSuch[6], Person.sSuch[3]);
        internalChange = true;
        Sex_Text = Person.sSex;
        if (Person.iReligi > 0)
        {
            Religion_Text = DataModul.TextLese1(Person.iReligi.AsInt());
        }

        PersonNr = Person.ID;
        CreationDate_Text = Person.SurName;
        Family1_Text = Person.Suffix;
        View.lblMandant.BackColor = Color.FromArgb(8454143);
        Mandant_Text = Modul1.Verz;
        if (Person.sBem[1].Trim() != "")
        {
            Notes_Text = Person.sBem[1];
        }
        Modul1.Person_ReadNames(persInArb, Person);
        View.cbxOccupation.BackColor = Modul1.Feld1Farb;
        View.cbxTitle.BackColor = Modul1.Feld1Farb;
        View.cbxResidence.BackColor = Modul1.Feld1Farb;
        View.cbxHome.BackColor = Modul1.Feld1Farb;

        Beruf2Label(persInArb);
        ZeigBeruf(persInArb, EEventArt.eA_301, View.lblTitelDisp, View.cbxTitle);
        ZeigBeruf(persInArb, EEventArt.eA_302, View.lblResidenceDisp, View.cbxResidence);
        ZeigBeruf(persInArb, EEventArt.eA_105, View.lblAdditDisp, View.cbxAdditional);
        ZeigBeruf(persInArb, EEventArt.eA_106, View.lblHomeDisp, View.cbxHome);

        AncesterNr_Text = Modul1.Ancesters_GetPersonData(Person.ID, out _, out _);

        Surnames_Text = " " + Person.SurName;
        Prefix_Text = " " + Person.Prefix;
        Suffix_Text = " " + Person.Suffix;
        Givennames_Text = " " + Person.Givennames;
        Alias_Text = " " + Person.Alias;
        Clan_Text = " " + Person.Clan;
        Status_Text = " " + Person.Stat;
        Predicate_Text = " " + Person.Prae;
        internalChange = false;
        Modul1.sPKennz = "P";

        DataModul.DB_PictureTable.Index = nameof(PictureIndex.PerKenn);
        DataModul.DB_PictureTable.Seek("=", Modul1.sPKennz, persInArb);

        Label15_Text = "";
        Picture_Visible = false;
        View.Picture1.Image = null;
        Witnes_Text = "Medien: Nein";
        Witnes_BackColor = Color.FromArgb(14737632);
        Bildliste();
        var ja = Modul1.Bildzeig1("Personenbild", View.Picture1.Width, View.Picture1.Height, "Personen", out BiText, out Bitext);
        if (ja)
        {
            Witnes_Text = "Medien: Ja";
            Witnes_BackColor = Color.FromArgb(12648447);
        }
        if (Strings.InStr(BiText, "Name:") != 0)
        {
            View.frmPicture.Visible = true;
            BiText = Strings.InStr(BiText, "Text:") != 0
                ? Strings.Mid(BiText, 7, Strings.InStr(BiText, "Text:") - 7)
                : Strings.Mid(BiText, 7, BiText.Length - 6);
            View.frmPicture.Text = BiText;
            Label15_Text = Bitext;
        }

        Adoption_Visible = DataModul.Link.ExistE(Modul1.PersInArb, ELinkKennz.lkAdoptedChild);

        View.btnNoGodparents.SetCommandBtn( DataModul.Link.ExistFam(Modul1.PersInArb, new[] { ELinkKennz.lkGodparent })
            || (Person.sBem[2].Length > 1), Modul1.IText[EUserText.tGodparents], Modul1.IText);

        foreach (var wtn in DataModul.Witness.ReadAllFams(PersonNr, 10))
        {
            if (wtn.eArt > EEventArt.eA_499)
            {
                NoWitnesses_Text = Modul1.IText[EUserText.t263] + ":" + Modul1.IText[EUserText.tNo];
                NoWitnesses_BackColor = Color.FromArgb(0xE0E0E0);
                continue;
                //=================
            }
            else
            {
                NoWitnesses_Text = Modul1.IText[EUserText.t263] + ":" + Modul1.IText[EUserText.tYes];
                NoWitnesses_BackColor = Color.FromArgb(0xC0FFFF);
                break;
            }
            //=================
        }

        if (!DataModul.Witness.ExistF(PersonNr, 10))
        {
            //=================

            EEventArt Idx_U = EEventArt.eA_Birth;

            while (Idx_U <= EEventArt.eA_302
            && (!(DataModul.Event.Seek((Idx_U, Modul1.PersInArb, 0)) is IRecordset dB_EventTable)
                || dB_EventTable.Fields[EventFields.Bem4].AsString().Trim() == ""))
            {

                Idx_U += 1;
            }

            View.btnNoWitnesses.SetCommandBtn(!DataModul.Event.Exists((Idx_U, Modul1.PersInArb, 0)), Modul1.IText[EUserText.t263], Modul1.IText);

        }
        View.btnLinkedPerson.SetCommandBtn(DataModul.Link.ExistF(Modul1.PersInArb, ELinkKennz.lk9), Modul1.IText[EUserText.t303], Modul1.IText);

        View.btnLinkTo.SetCommandBtn(DataModul.Link.ExistE(Modul1.PersInArb, ELinkKennz.lk9), Modul1.IText[EUserText.t304], Modul1.IText);

        View.btnWitnessIfNo.SetCommandBtn(!DataModul.Witness.ExistE(PersonNr, 10), Modul1.IText[EUserText.t302], Modul1.IText);

        View.btnNoGodparents.SetCommandBtn(Person.sBem[2].Length > 1, Modul1.IText[EUserText.tGodparents], Modul1.IText);

        DataModul.DB_SourceLinkTable.Index = "Tab";
        DataModul.DB_SourceLinkTable.Seek("=", 1, Modul1.PersInArb);
        View.btnNoSources.SetCommandBtn(!DataModul.DB_SourceLinkTable.NoMatch || Person.sBem[3].Length > 1, Modul1.IText[EUserText.t244], Modul1.IText);

        View.btnGodparentIfNo.SetCommandBtn(DataModul.Link.ExistE(Modul1.PersInArb, ELinkKennz.lkGodparent), Modul1.IText[EUserText.tGodparentOf], Modul1.IText);

        if (DataModul.Link.ExistE(Modul1.PersInArb, ELinkKennz.lkMarrWitness))
        {
        }
        if (DataModul.Link.ExistE(Modul1.PersInArb, ELinkKennz.lkWitnOfEngage))
        {
        }
        if (DataModul.Link.ExistE(Modul1.PersInArb, ELinkKennz.lkWitnOfMarr))
        {
        }
        Personen.Default.FrmPerson_EventUpd(Modul1.PersInArb);
        if (!Modul1.Person.xVChr)
        {
            Modul1.Kont[25] = DateHelper2.CalcAge(Modul1.Person.Death, Modul1.Person.Birthday, Modul1.IText);
            Age_Text = Modul1.Kont[25];
        }
        DataModul.DT_DescendentTable.Index = "PerNr";
        DataModul.DT_DescendentTable.Seek("=", Modul1.PersInArb);
        NachfNr_Text = !DataModul.DT_DescendentTable.NoMatch
            ? (string.Concat(Modul1.IText[EUserText.t239] + " ", DataModul.DT_DescendentTable.Fields["Gen"].AsString()) +  "-" +  DataModul.DT_DescendentTable.Fields["Nr"].Value)
        : "";

        ubg = Modul1.Eltsuch(Modul1.PersInArb);
        List2_Items.Clear();
        var M1_Iter_ = 0;
        while (M1_Iter_ <= 9)
        {
            List2_Items.Add(new ListItem<PersonFamily>(" ", new PersonFamily(0, 0)));
            M1_Iter_++;
        }
        if (ubg > 0)
        {
            List2_Items.Insert(0, new(Modul1.IText[EUserText.t108] + " " + "          " + ubg.AsString().Right(10), new PersonFamily(Modul1.PersInArb, ubg)));

            int Fams = List2_Items[0].ItemData<PersonFamily>().iFamily;
            short Idx_U = 0;
            ParentToList(Fams, Idx_U, List2_Items);

            Modul1.PersInArb = List2_Items[1].ItemData<PersonFamily>().iPerson;
            ubg = Modul1.Eltsuch(Modul1.PersInArb);
            if (ubg > 0)
            {
                List2_Items.Insert(3, new(Modul1.IText[EUserText.t109] + "          " + ubg.AsString().Right(10), new PersonFamily(Modul1.PersInArb, ubg)));
            }
            else
            {
                List2_Items.Insert(3, new(Modul1.IText[EUserText.t110], new PersonFamily(Modul1.PersInArb, -1)));
            }

            Fams = List2_Items[3].ItemData<PersonFamily>().iFamily;
            Idx_U = 3;
            ParentToList(Fams, Idx_U, List2_Items);

            Modul1.PersInArb = List2_Items[2].ItemData<PersonFamily>().iPerson;
            ubg = Modul1.Eltsuch(Modul1.PersInArb);
            if (ubg > 0)
            {
                List2_Items.Insert(6, new(Modul1.IText[EUserText.t133] + "          " + ubg.AsString().Right(10), new PersonFamily(Modul1.PersInArb, ubg)));
            }
            else
            {
                List2_Items.Insert(6, new(Modul1.IText[EUserText.t134], new PersonFamily(Modul1.PersInArb, -1)));
            }
            Fams = List2_Items[6].ItemData<PersonFamily>().iFamily;
            Idx_U = 6;
            ParentToList(Fams, Idx_U, List2_Items);
        }
        else
        {
            List2_Items.Insert(0, new(Modul1.IText[EUserText.t135]));
        }
        //=================
        var M1_Iter = List2_Items.Count - 1;
        while (M1_Iter_ >= 9)
        {
            List2_Items.RemoveAt(M1_Iter_);
            M1_Iter_ += -1;
            //=================
        }
        Modul1.UbgT = "";
        var aiMarr = Modul1.Ehesuch(personNr: PersonNr, Persex: Sex_Text);
        Marriages_Text = Modul1.IText[EUserText.tMarrCount] + ":" + aiMarr.Count.AsString();
        ubg = 0;
        Sortlist_Items.Clear();
        foreach (var iFam in aiMarr)
        {
            foreach (var iPer in Modul1.Family_Kindsuch(iFam))
                Sortlist_Items.Add(iPer);
        }
        List1_Items.Clear();
        if (aiMarr.Count == 0)
        {
            List1_Items.Add(new(Modul1.IText[EUserText.t132]));
        }
        if (Sortlist_Items.Count > 0)
        {
            List1_Items.Add(new(Modul1.IText[EUserText.t130]));
            int num11 = Sortlist_Items.Count - 1;
            M1_Iter = 0;
            while (M1_Iter <= num11)
            {
                Modul1.PersInArb = Sortlist_Items[M1_Iter].ItemData.Item1;
                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                var DDatum = Sortlist_Items[M1_Iter].ItemData.Item2;
                var HT = $"{DDatum:yyyyMMdd}";
                if (Sortlist_Items[M1_Iter].ItemData.Item3 == ELinkKennz.lkAdoptedChild)
                {
                    HT += " (Adop.)";
                }
                var sGivennames = Modul1.Person.Givennames.Trim() == "" ? Modul1.Person.SurName.Trim() : Modul1.Person.Givennames.Trim();
                List1_Items.Add(new($"{HT}   {sGivennames}", Modul1.PersInArb));
                M1_Iter++;
                //=================
            }
        }
        else if (List1_Items.Count < 1)
        {
            List1_Items.Add(new(Modul1.IText[EUserText.t131]));
        }

        HandleSources();
        Persistence.AppendStringsTemp("GenPluswin.kml", ["</Document>", "</kml>"]);
        ShowPlaces_Visible = Persistence.FileLengthTemp("GenPluswin.kml") > 100;
        return;


        //    if (Information.Err().Number == 94)
        //    {
        //        dB_PersonTable1.Edit();
        //        dB_PersonTable1.Fields[PersonFields.Bem2].Value = "";
        //        if (IsNotReadOnly)
        //        {
        //            dB_PersonTable1.Commit();
        //        }
        //        ProjectData.ClearProjectError();
        //        if (num2 == 0)
        //        {
        //            throw ProjectData.CreateProjectError(cDefaultErr);
        //        }
        //    }
        //    else if (Information.Err().Number == 3265)
        //    {
        //        DataModul.MandDB.DoHide();
        //        DataModul.TempDB.DoHide();
        //        DataModul.DOSB.DoHide();
        //        DataModul.DSB.DoHide();
        //        Modul1.Dateienopen();
        //        ProjectData.ClearProjectError();
        //        if (num2 == 0)
        //        {
        //            throw ProjectData.CreateProjectError(cDefaultErr);
        //        }
        //    }
        //    else
        //    {
        //        if (Interaction.MsgBox(Conversions.ErrorToString(), MessageBoxButtons.OKCancel, Information.Err().Number.AsString()) == DialogResult.Cancel)
        //        {
        //            ProjectData.EndApp();
        //        }
        //        ProjectData.ClearProjectError();
        //        if (num2 == 0)
        //        {
        //            throw ProjectData.CreateProjectError(cDefaultErr);
        //        }
        //        //=================
        //    }
        //    goto IL_3b3b;
        //}
        //_ = Interaction.MsgBox(Conversions.ErrorToString() + "\nDie Verbindung kann über Bilder ansehen gelöscht werden", MessageBoxButtons.OK, Information.Err().Number.AsString());
        //if (Information.Err().Number == 53)
        //{
        //    ProjectData.ClearProjectError();
        //    if (num2 == 0)
        //    {
        //        throw ProjectData.CreateProjectError(cDefaultErr);
        //    }
        //    num2 = 0;
        //}
        //else
        //{
        //    ProjectData.ClearProjectError();
        //    if (num2 == 0)
        //    {
        //        throw ProjectData.CreateProjectError(cDefaultErr);
        //    }
        //    num2 = 0;
        //    //=================
        //}



        void HandleSources()
        {
            Property_Items.Clear();
            Property_SelectedItem = null;
            Modul1.PersInArb = PersonNr;
            var dB_PropertyTable = DataModul.DB_PropertyTable;
            var dB_GbeTable = DataModul.DB_GbeTable;
            dB_PropertyTable.Index = nameof(PropertyIndex.Per);
            dB_PropertyTable.Seek("=", Modul1.PersInArb);
            if (!dB_PropertyTable.NoMatch)
            {
                if (dB_PropertyTable.RecordCount > 0)
                {
                    while (!dB_PropertyTable.EOF)
                    {
                        dB_GbeTable.Index = "Nr";
                        dB_GbeTable.Seek("=", dB_PropertyTable.Fields[PropertyFields.Nr].Value);
                        var Gbe_iNr = dB_GbeTable.Fields["nr"].AsInt();
                        var Gbe_sAkte = dB_GbeTable.Fields["Akte"].AsString();
                        object Gbe_sName = dB_GbeTable.Fields["name"].AsString();
                        object Gbe_iJahr = dB_GbeTable.Fields["Jahr"].AsInt();
                        Property_Items.Add(new ListItem<int>($"{Gbe_iJahr,4} {Gbe_sName,-30} Akte: {Gbe_sAkte}", Gbe_iNr));
                        dB_PropertyTable.MoveNext();
                        //=================
                    }
                    if (Property_Items.Count > 0)
                    {
                        Property_SelectedItem = Property_Items[0];
                    }
                }
            }

        }
    }

    public void Pat(EUserText Defi, int personNr, int iPerFam)
    {
        Modul1.Trans = 1;
        if (Defi == EUserText.tGodparents)
        {
            DataModul.Link.Append(iPerFam, personNr, ELinkKennz.lkGodparent);
            Modul1.PersInArb = iPerFam;

            Rahmen.Default.Close();
            Perzeig(Modul1.PersInArb);
            NoGodparents();
        }
        else if (Defi == EUserText.tGodparentOf)
        {
            DataModul.Link.Append(personNr, iPerFam, ELinkKennz.lkGodparent);
            Modul1.PersInArb = iPerFam;

            Rahmen.Default.Close();
            Perzeig(Modul1.PersInArb);
            GodparentIfNo();
        }
        else if (Defi == EUserText.tMarrWitness
        || Defi == EUserText.tWitnOfEngage
        || Defi == EUserText.tWitnOfMarr)
        {
            if (personNr > 0)
            {
                if (IsNotReadOnly)
                {
                    DataModul.Link.Append(personNr, iPerFam, Defi switch
                    {
                        EUserText.tMarrWitness => ELinkKennz.lkMarrWitness,
                        EUserText.tWitnOfEngage => ELinkKennz.lkWitnOfEngage,
                        EUserText.tWitnOfMarr => ELinkKennz.lkWitnOfMarr,
                        _ => ELinkKennz.lkNone,
                    });

                }
                DoClose?.Invoke();
                Rahmen.Default.Close();
                Familie.Default.Fameinlesen(Modul1.FamInArb, out short Rich);
            }
        }
        else if (Defi == EUserText.t304)
        {
            if (Modul1.Typ != DriveType.CDRom)
            {
                DataModul.Link.Append(personNr, iPerFam, ELinkKennz.lk9);
            }
            Modul1.PersInArb = iPerFam;

            Rahmen.Default.Close();
            Perzeig(Modul1.PersInArb);
            LinkTo();
        }
        else if (Defi == EUserText.t303)
        {
            if (Modul1.Typ != DriveType.CDRom)
            {
                DataModul.Link.Append(iPerFam, personNr, ELinkKennz.lk9);
            }
            Modul1.PersInArb = iPerFam;

            Rahmen.Default.Close();
            Perzeig(Modul1.PersInArb);
            LinkedPerson();
        }
        if (Rahmen.Default.eResult != ERahmenResult.eRR_OK)
        {
            Return_Visible = false;
            SearchNumber_Visible = false;
            NewPerson_Visible = false;
            Next_Visible = false;
            Previous_Visible = false;
            Delete_Visible = false;
            SaveNExit_Visible = false;
            Cancel_Visible = false;


            GodparentIfNo_Visible = true;
            NoGodparents_Visible = true;
            SearchAncestors_Visible = true;
            ShowPerson_Visible = true;
            SearchAncestors_Visible = true;
            NewFamily_Visible = true;
            ShowPerson_Visible = true;
            SearchName_Visible = true;
            Resarch_Visible = true;
            RegisterSearch_Visible = true;
            ShowPlaces_Visible = true;
            Sorting_Visible = true;
        }
    }

    private void Picture1()
    {
        if (View.frmPicture.Top == 1)
        {
            View.frmPicture.Width = Modul1.Posi[0];
            View.frmPicture.Height = Modul1.Posi[1];
            View.frmPicture.Left = Modul1.Posi[2];
            View.frmPicture.Top = Modul1.Posi[3];
            string text = Label15_Text;
            FileStream fileStream = new FileStream(text, FileMode.Open);
            Bitmap oBitmap;
            if (Modul1.Typ != DriveType.CDRom)
            {
                oBitmap = new Bitmap(fileStream);
                fileStream.Close();
            }
            else
            {
                oBitmap = new Bitmap(text);
            }
            PictureBox picture = View.Picture1;
            picture.Image = Modul1.AutoSizeImage(oBitmap, picture.ClientRectangle.Width, picture.ClientRectangle.Height);
            fileStream.Close();
            return;
        }
        checked
        {
            Modul1.Posi[0] = (short)View.frmPicture.Width;
            Modul1.Posi[1] = (short)View.frmPicture.Height;
            Modul1.Posi[2] = (short)View.frmPicture.Left;
            Modul1.Posi[3] = (short)View.frmPicture.Top;
            View.frmPicture.Top = 1;
            View.frmPicture.Height = View.Height - 40;
            View.frmPicture.Left = 1;
            View.frmPicture.Width = (int)Math.Round(View.Width / 100.0 * 99.0);
            View.Picture1.Width = View.frmPicture.Width - 10;
            View.Picture1.Height = View.frmPicture.Height - 20;
            View.frmPicture.BringToFront();
            string text2 = Label15_Text;
            Bitmap oBitmap2;
            if (Modul1.Typ != DriveType.CDRom)
            {
                FileStream fileStream2 = new FileStream(text2, FileMode.Open);
                oBitmap2 = new Bitmap(fileStream2);
                fileStream2.Close();
            }
            else
            {
                oBitmap2 = new Bitmap(text2);
            }
            PictureBox picture2 = View.Picture1;
            picture2.Image = Modul1.AutoSizeImage(oBitmap2, picture2.ClientRectangle.Width, picture2.ClientRectangle.Height, bStretch: true);
        }
    }

    //private void btnChangeSexToF_Click(object sender, EventArgs e)
    //{
    //    //Discarded unreachable code: IL_051b, IL_0523
    //    var eText = fraPersImpQuerry1.IText;
    //    EEventArt eArt = default;
    //    int iPerFamNr = default;
    //    iPerFamNr = PersonNr;
    //    int iComb;
    //    (eArt, iComb) = eText switch
    //    {
    //        EUserText.tOccupation => (EEventArt.eA_300, 0),
    //        EUserText.t70 => (EEventArt.eA_301, 1),
    //        EUserText.tResidence => (EEventArt.eA_302, 2),
    //        EUserText.tConfirmation => (EEventArt.eA_105, 10),
    //        EUserText.tCivilState => (EEventArt.eA_106, 3),
    //        _ => (default, default),
    //    };
    //    if (eArt == default)
    //    {
    //        return;
    //    }
    //    Modul1.Modul1.LfNR = Combo1[iComb].Tag.AsInt() > -1.0 ? (short)Combo1[iComb].Tag.AsInt() : (short)1;
    //    if (DataModul.Event.Exists((eArt, iPerFamNr, Modul1.Modul1.LfNR)))
    //    {
    //        DataModul.SourceLink_DeleteAllEvLk(iPerFamNr, eArt, Modul1.Modul1.LfNR);

    //        DataModul.Witness.DeleteAllZ(iPerFamNr, 10, eArt, Modul1.Modul1.LfNR);

    //        DataModul.Event.Delete((eArt, iPerFamNr, Modul1.Modul1.LfNR));
    //        Modul1.Modul1.Art = eArt;
    //        if (Modul1.Modul1.Art > EEventArt.eA_499)
    //        {
    //            Modul1.FamInArb = Familie.Default.iFamNr;
    //            Familie.Default.Fameinlesen(Modul1.FamInArb, out short Rich);
    //        }
    //        else
    //        {
    //            Modul1.PersInArb = PersonNr;
    //            fraPersImpQuerry1.Visible = false;
    //            _viewModel.Perzeig(Modul1.PersInArb);
    //        }
    //    }
    //    else
    //    {
    //        Debugger.Break();
    //    }

    //}
    /*
    private void List3_MouseMove(object sender, MouseEventArgs e)
    {
        View.frmDublicates.Width = checked(View.Width - (View.Width - View.List3.Left));
    }

    private void Frame4_MouseMove(object sender, MouseEventArgs e)
    {
        View.frmDublicates.Width = View.Width - 20;
        View.List5.Width = View.frmDublicates.Width - 10;
    }
    */
    partial void OnAlias_TextChanged(string value)
    {
        Modul1.eNKennz = ETextKennz.C_;
        if (value.Trim() == "")
        {
            return;
        }
        Textzeig1(Modul1.eTKennz, value);
    }

    partial void OnPredicate_TextChanged(string value)
    {
        Modul1.eNKennz = ETextKennz.tk2_;
        if (value.Trim() == "")
        {
            return;
        }
        Textzeig1(Modul1.eTKennz, value);
    }

    private void RichTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
    {
        _ = Interaction.MsgBox(e.LinkText);
        _ = Process.Start(e.LinkText);
    }

    [RelayCommand]
    private void OpenProperty()
    {
        if (Property_Text.Trim() == "")
        {
            Modul1.Ubg = 0;
            _ = MainProject.Forms.Besitz.ShowDialog();
            Perzeig(Modul1.PersInArb);
        }
        else
        {
            Reenter_Visible = true;
        }
    }

    [RelayCommand]
    private void Property()
    {
        Reenter_Visible = false;
        Modul1.Ubg = 0;
        _ = MainProject.Forms.Besitz.ShowDialog(0);
        Perzeig(Modul1.PersInArb);
    }

    private void Button11_Click(object sender, EventArgs e)
    {
        Reenter_Visible = false;
        Modul1.Ubg = 0;
        Modul1.Ubg = Strings.Mid(Property_Text, 201, 10).AsInt();
        _ = MainProject.Forms.Besitz.ShowDialog();
        Perzeig(Modul1.PersInArb);
    }

    private void Button9_Click(object sender, EventArgs e)
    {
        Reenter_Visible = false;
    }

    private void Button8_Click(object sender, EventArgs e)
    {
        View.edtNotes.Top = Modul1.Posi[0];
        View.edtNotes.Height = Modul1.Posi[1];

        FieldsOFB_Visible = true;
        if (SaveToFamily_Visible || SaveNExit_Visible)
        {
            Delete_Visible = true;
            SaveNExit_Enabled = true;
            SaveToFamily_Enabled = true;
            Cancel_Enabled = true;
        }
        else
        {
            Return_Visible = true;
            SearchNumber_Visible = true;
            NewPerson_Visible = true;
            Next_Visible = true;
            Previous_Visible = true;
            Delete_Visible = true;

            SearchAncestors_Visible = true;
            NewFamily_Visible = true;
            ShowPerson_Visible = true;
            SearchName_Visible = true;
            Resarch_Visible = true;
            RegisterSearch_Visible = true;
            ShowPlaces_Visible = true;
            Sorting_Visible = true;
        }
        EndTextInput_Visible = false;
        Cancel_Visible = false;
        Perzeig(Modul1.PersInArb);

    }

    private void RichTextBox1_MouseClick(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            Debugger.Break();
        }
    }

    private void RichTextBox1_MouseUp(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            if (View.edtNotes.Top > View.lblRemark.Top)
            {
                Modul1.Posi[0] = (short)View.edtNotes.Top;
                Modul1.Posi[1] = (short)View.edtNotes.Height;
            }
            View.edtNotes.Top = View.List1.Top;
            View.edtNotes.Height = View.btnEndTextInput.Top - View.List1.Top;
            if (Notes_Text.Trim() == "")
            {
                View.edtNotes.SelectionStart = 0;
            }
            int M1_Iter = 0;
            int i;
            Return_Visible = false;
            SearchNumber_Visible = false;
            NewPerson_Visible = false;
            Next_Visible = false;
            Previous_Visible = false;
            Delete_Visible = false;

            FieldsOFB_Visible = false;
            EndTextInput_Visible = true;
            Cancel_Visible = true;
            SaveToFamily_Enabled = false;
            SaveNExit_Enabled = false;
            Cancel_Enabled = false;
            SearchAncestors_Visible = false;
            NewFamily_Visible = false;
            ShowPerson_Visible = false;
            SearchName_Visible = false;
            Resarch_Visible = false;
            RegisterSearch_Visible = false;
            ShowPlaces_Visible = false;
            Sorting_Visible = false;
        }

    }

    [RelayCommand]
    private void ShowPlaces()
    {
        _ = Process.Start(Modul1.TempPath + "\\GenPluswin.kml");
    }

    private void Bildliste()
    {
        Witnes_Items.Clear();
        Witnes_Items.Add(new("neue Eingabe/Bearbeiten"));
        DataModul.DB_PictureTable.Seek("=", Modul1.sPKennz, Modul1.PersInArb);
        if (DataModul.DB_PictureTable.NoMatch)
        {
            return;
        }
        while (!DataModul.DB_PictureTable.EOF && !(DataModul.DB_PictureTable.Fields[PictureFields.ZuNr].AsInt() != Modul1.PersInArb))
        {
            if (DataModul.DB_PictureTable.Fields[PictureFields.Beschreibung].AsString() == "Quelle")
            {
                Witnes_Text = DataModul.DB_PictureTable.Fields[PictureFields.Beschreibung].AsString();
            }
            string DateiName = DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString() + DataModul.DB_PictureTable.Fields[PictureFields.Datei].AsString();
            if (DateiName.Left(1) == "#")
            {
                DateiName = Modul1.Verz + Strings.Mid(DateiName, 2, DateiName.Length);
            }
            if (File.Exists(DateiName))
            {
                var items = Witnes_Items;
                items.Add(new(
                    DataModul.DB_PictureTable.Fields[PictureFields.Beschreibung].AsString(),
                    (DataModul.DB_PictureTable.Fields[PictureFields.LfNr].AsInt(),
                     DateiName)));
            }
            DataModul.DB_PictureTable.MoveNext();
        }
        Witnes_Text = "Medien Ja";
        if (Witnes_Items.Count > 0)
        {
            Witnes_Visible = true;
        }
    }

    private void ComboBox2_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        if (Witnes_Text == "neue Eingabe")
        {
            Modul1.Ubg = Modul1.PersInArb;
            Modul1.sPKennz = "P";
            _ = MainProject.Forms.Bilder.ShowDialog();
        }
        else
        {
            DataModul.DB_PictureTable.Index = nameof(PictureIndex.Nr);
            _ = Process.Start(Witnes_SelectedItem.ItemData.Item2);
        }
    }

    private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Witnes_Text == "neue Eingabe/Bearbeiten")
        {
            Modul1.Ubg = Modul1.PersInArb;
            Modul1.sPKennz = "P";
            _ = MainProject.Forms.Bilder.ShowDialog();
        }
        else
        {
            DataModul.DB_PictureTable.Index = nameof(PictureIndex.Nr);
            _ = Process.Start(Witnes_SelectedItem.ItemData.Item2);
        }
    }

    public void FrmPerson_SetData(int persInArb, int iFamNr, string lblFamText, EUserText eHdrText, ELinkKennz eKennz)
    {
        Marriages_Text = Modul1.IText[EUserText.tMarrCount] + " 1";
        Age_Text = Modul1.IText[EUserText.t116];
        PersonNr = persInArb;
        eFamHdr = eHdrText;
        xDestIsFam = true;
        FamNr = iFamNr;
        FamPers_Text = lblFamText;
        Family_LinkKennz = eKennz;

        if (eKennz == ELinkKennz.lkFather)
        {
            Sex_Text = "M";
        }
        else if (eKennz == ELinkKennz.lkMother)
        {
            Sex_Text = "F";
        }
        AncesterNr_Text = "";
        Return_Visible = false;
        SearchNumber_Visible = false;
        NewPerson_Visible = false;
        Next_Visible = false;
        Previous_Visible = false;
        Delete_Visible = false;
        SaveToFamily_Text = Modul1.IText[EUserText.tNMSaveToFamily];
        SaveToFamily_Visible = true;
        Cancel_Visible = true;
        PrintScreen_Visible = false;
        Resarch_Visible = false;
        SearchAncestors_Visible = false;
        NewFamily_Visible = false;
        ShowPerson_Visible = false;
        SearchName_Visible = false;
        RegisterSearch_Visible = false;
        Sorting_Visible = false;
        SetFocus(nameof(Religion_Text));
    }


    [RelayCommand]
    private void Additional()
    {
        throw new NotImplementedException();
    }
    [RelayCommand]
    private void Residence()
    {
        throw new NotImplementedException();
    }
    [RelayCommand]
    private void Title()
    {
        throw new NotImplementedException();
    }

    [RelayCommand]
    private void Occupation()
    {
        throw new NotImplementedException();
    }

    [RelayCommand]
    private void Hometown()
    {
        throw new NotImplementedException();
    }

    [RelayCommand]
    private void PrintScreen()
    {
        throw new NotImplementedException();
    }

    private void Sorting_SelectedIndexChanged(object eventSender, EventArgs eventArgs)
    {
        View.cbxSorting.BackColor = Color.FromArgb(0xFFFFFF);
        if (Sorting_SelectedItem.ItemString != "Nummer")
        {
            View.cbxSorting.BackColor = Color.FromArgb(0xFF0000FF.AsInt());
        }
        Modul1.Aus[(int)EOutCfg.o21] = Sorting_SelectedItem.ItemData<int>().AsString();
        if (IsNotReadOnly)
        {
            Modul1.Persistence.WriteStringsInit("Druck_ini.dat", Modul1.Aus);
        }
    }

    public int AendPruef(int PersInArb, int ubg2)
    {
        string inputStr = 0.AsString();
        string value = "";
        if (!IsNotReadOnly)
        {
            return 0;
        }
        if (!DataModul.Person.Exists(PersInArb))
        {
            Perschreib(ubg2);
        }
        if (ubg2 == 0f)
        {
            return 0;
        }
        string sName = (Surnames_Text.Trim() + "," + Givennames_Text.Replace("\"", "").Trim()).Left(50).Trim();
        string sKenn = "";

        Person_UpdateSearchfield(PersInArb, sName, Suchfeld);

        string text2 = " ";
        EEventArt eArt = EEventArt.eA_Birth;
        while (eArt <= EEventArt.eA_Burial)
        {
            if (DataModul.Event.ReadData(eArt, PersInArb, out var cEvt, 0))
            {
                if (cEvt.dDatumV != default)
                {
                    var Datu = cEvt.dDatumV.ToString("yyyyMMdd");
                    switch (eArt)
                    {
                        case EEventArt.eA_Birth:
                        case EEventArt.eA_Baptism:
                            sKenn = "*";
                            if (Datu.Trim() != "")
                            {
                                var suchfeld = Suchfeld;

                                Person_UpdateSearchField2(PersInArb, eArt, sName, cEvt.sReg, suchfeld, Datu);
                            }
                            break;
                        default:
                            break;
                    }
                    inputStr = Datu.Left(4);
                    text2 = Strings.Trim(checked(eArt - 100).AsString());
                    value = cEvt.sDatumV_S;
                    break;
                }
            }
            eArt++;
        }
        eArt = EEventArt.eA_Birth;
        while (eArt <= EEventArt.eA_Burial)
        {
            if (DataModul.Event.ReadData(eArt, PersInArb, out var cEvt, 0))
            {
                Person_UpdateSrchVal3(PersInArb, eArt, Suchfeld, cEvt.sReg.Trim());
            }
            eArt++;
        }
        string sLeitName = "";
        sLeitName = DataModul.Texte_GetLeitName(sName, Surnames_Text, Kont_3);

        string left = text2;
        if (left is "1" or "2")
        {
            sKenn = Modul1.sBaptismMark;
            //=================
        }
        else
        {
            if (left is "3" or "4")
            {
                sKenn = Modul1.sDeathMark;
            }
            //=================
        }
        string sAlias = Alias_Text.Trim() == ""
            ? sName
            : (Alias_Text + "," + Kont_3.Trim()).Left(50);

        DataModul.SearchTable_Update(PersInArb, inputStr, value, sName, Surnames_Text, sKenn, sLeitName, sAlias, Module2.Koelner_Phonetic, Module2.GetSoundEx);

        ProjectData.ClearProjectError();
        var pt = DataModul.Person.Seek(PersInArb);
        if (Suchfeld[0] == ESearchSelection.eManual)
        {
            if (null != pt.Fields[PersonFields.Such1].Value)
            {
                if (Search_Text.Trim() != Strings.Trim(pt.Fields[PersonFields.Such1].AsString()))
                {
                    Trans = 1;
                }
            }
            else if (Search_Text.Trim() != "")
            {
                Trans = 1;
            }
        }
        if (Suchfeld[1] == ESearchSelection.eManual)
        {
            if (null != pt.Fields[PersonFields.Such2].Value)
            {
                if (Search2_Text.Trim() != Strings.Trim(pt.Fields[PersonFields.Such2].AsString()))
                {
                    Trans = 1;
                }
            }
            else if (Search2_Text.Trim() != "")
            {
                Trans = 1;
            }
        }
        if (Suchfeld[2] == ESearchSelection.eManual)
        {
            if (null != pt.Fields[PersonFields.Such3].Value)
            {
                if (Search3_Text.Trim() != Strings.Trim(pt.Fields[PersonFields.Such3].AsString()))
                {
                    Trans = 1;
                }
            }
            else if (Search3_Text.Trim() != "")
            {
                Trans = 1;
            }
        }
        ProjectData.ClearProjectError();
        Perschreib(ubg2);
        ProjectData.ClearProjectError();
        Pschalt = 0f;
        Modul1.DatPruef(0);
        goto end_IL_0001_2;
    end_IL_0001_2: // <========== 9
        return ubg2;
    }

    private void Person_UpdateSrchVal3(int PersInArb, EEventArt eArt, IList<ESearchSelection> suchfeld, string sSrchVal)
    {
        IRecordset dB_PersonTable = DataModul.DB_PersonTable;
        dB_PersonTable.Seek("=", PersInArb.AsString());
        dB_PersonTable.Edit();
        switch (eArt, suchfeld[0])
        {
            default: break;
            case (EEventArt.eA_Birth, ESearchSelection.e6):
            case (EEventArt.eA_Baptism, ESearchSelection.e7):
            case (EEventArt.eA_Death, ESearchSelection.e8):
            case (EEventArt.eA_Burial, ESearchSelection.e9):
                dB_PersonTable.Fields[PersonFields.Such4].Value = sSrchVal; break;
        }
        switch (eArt, suchfeld[1])
        {
            case (EEventArt.eA_Birth,ESearchSelection.e6):
            case (EEventArt.eA_Baptism, ESearchSelection.e7):
            case (EEventArt.eA_Death, ESearchSelection.e8):
            case (EEventArt.eA_Burial, ESearchSelection.e9):
                dB_PersonTable.Fields[PersonFields.Such5].Value = sSrchVal;
                break;
            default:
                break;
        }
        switch (eArt, suchfeld[2])
        {
            case (EEventArt.eA_Birth, ESearchSelection.e6):
            case (EEventArt.eA_Baptism, ESearchSelection.e7):
            case (EEventArt.eA_Death, ESearchSelection.e8):
            case (EEventArt.eA_Burial, ESearchSelection.e9):
                dB_PersonTable.Fields[PersonFields.Such6].Value = sSrchVal;
                break;
            default:
                break;
        }
        dB_PersonTable.Update();
    }

    private void Person_UpdateSearchField2(int PersInArb, EEventArt eArt, string sName, string sReg, IList<ESearchSelection> suchfeld, string Datu)
    {
        IRecordset dB_PersonTable = DataModul.DB_PersonTable;
        dB_PersonTable.Seek("=", PersInArb);
        dB_PersonTable.Edit();
        dB_PersonTable.Fields[PersonFields.Such4].Value = suchfeld[0] switch
        {
            ESearchSelection.e6 when eArt == EEventArt.eA_Birth => sReg.Trim(),
            ESearchSelection.e3 => sName + " " + Datu.AsString(),
            ESearchSelection.e4 => Datu.AsString().Trim() + " " + sName,
            _ => dB_PersonTable.Fields[PersonFields.Such4].Value,
        };
        dB_PersonTable.Fields[PersonFields.Such5].Value = suchfeld[1] switch
        {
            ESearchSelection.e3 => sName + " " + Datu.AsString(),
            ESearchSelection.e4 => Datu.AsString().Trim() + " " + sName,
            _ => dB_PersonTable.Fields[PersonFields.Such5].Value,
        };
        dB_PersonTable.Fields[PersonFields.Such6].Value = suchfeld[2] switch
        {
            ESearchSelection.e3 => sName + " " + Datu.AsString(),
            ESearchSelection.e4 => Datu.AsString().Trim() + " " + sName,
            _ => dB_PersonTable.Fields[PersonFields.Such6].Value,
        };
        dB_PersonTable.Update();
    }

    private void Person_UpdateSearchfield(int iPers, string sName, IList<ESearchSelection> suchfeld)
    {
        IRecordset dB_PersonTable = DataModul.DB_PersonTable;
        dB_PersonTable.Seek("=", iPers);
        dB_PersonTable.Edit();
        if (suchfeld[0] == ESearchSelection.e2)
        {
            dB_PersonTable.Fields[PersonFields.Such4].Value = sName;
        }
        if (suchfeld[1] == ESearchSelection.e2)
        {
            dB_PersonTable.Fields[PersonFields.Such5].Value = sName;
        }
        if (suchfeld[2] == ESearchSelection.e2)
        {
            dB_PersonTable.Fields[PersonFields.Such6].Value = sName;
        }
        dB_PersonTable.Update();
    }

    private static void Rahmen_ShowRamenDialog2(EUserText eHeader, int iUbg, IList<int> aiPer)
    {
        Rahmen frmRahmen = Rahmen.Default;
        frmRahmen.RTB.Visible = false;
        frmRahmen.lblAsText.Visible = false;

        frmRahmen.List4.Items.Clear();
        frmRahmen.RTB.Text = "";

        frmRahmen.btnAppend.Enabled = true;
        frmRahmen.btnAppend.Visible = true;
        frmRahmen.ShowDialog(iUbg, aiPer, eHeader);
    }

    private static void Rahmen_ShowRahmenDialog(EUserText eHeader, int iUbg, IList<int> aiPer)
    {
        Rahmen frmRahmen = Rahmen.Default;
        frmRahmen.RTB.Visible = false;
        frmRahmen.lblAsText.Visible = false;

        frmRahmen.Close();
        frmRahmen.Visible = false;

        frmRahmen.btnAppend.Enabled = true;
        frmRahmen.btnAppend.Visible = true;
        frmRahmen.ShowDialog(iUbg, aiPer, eHeader);
    }

    private bool Handle_Common(object sender)
    {
        var flag = false;
        Modul1.Suchschalt = 0;
        ProjectData.ClearProjectError();
        int persInArb = PersonNr;
        if (Modul1.Typ != DriveType.CDRom)
        {
            var ubg2 = 1;
            if (unchecked(persInArb > 0 && sender != View.btnDelete))
            {
                ubg2 = AendPruef(persInArb, ubg2);
                if (ubg2 == 2)
                {
                    flag = true;
                }
            }
            if (!flag)
            {
                List3_Items.Clear();
                List3_Visible = false;
                if (ubg2 == 0)
                    flag = true;
            }
        }
        Kreuz = false;
        return flag;
    }

    [RelayCommand]
    private void SearchName()
    {
        Rahmen.Default.RTB.Visible = false;
        Rahmen.Default.lblAsText.Visible = false;
        Rahmen.Default.ShowDialog(9, [PersonNr], EUserText.t460);
    }

    private void Personen_KeyUp(object eventSender, KeyEventArgs eventArgs)
    {
        Keys num = eventArgs.KeyCode;
        short num2 = (short)unchecked((int)eventArgs.KeyData / 65536);
        if (num2 == 2)
        {
            switch (num)
            {
                case (Keys)112:
                    Label1_Click(View.fraEventShowEdit1, new EventArgs());
                    break;
                case (Keys)113:
                    Label1_Click(View.fraEventShowEdit2, new EventArgs());
                    break;
                case (Keys)114:
                    Label1_Click(View.fraEventShowEdit3, new EventArgs());
                    break;
                case (Keys)115:
                    Label1_Click(View.fraEventShowEdit4, new EventArgs());
                    break;
                case (Keys)116:
                    Occupation();
                    break;
                case (Keys)117:
                    Label_Click(View.lblOccupation, new EventArgs());
                    break;
                case (Keys)118:
                    Label_Click(View.lblOther, new EventArgs());
                    break;
                case (Keys)119:
                    Label_Click(View.lblResidence, new EventArgs());
                    break;
            }
        }

    }

    private void Label_Click(Label lblOccupation, EventArgs eventArgs)
    {
        throw new NotImplementedException();
    }

    private void Label1_Click(FraEventShowEdit fraEventShowEdit1, EventArgs eventArgs)
    {
        throw new NotImplementedException();
    }

    private void Personen_FormClosing(object eventSender, FormClosingEventArgs eventArgs)
    {
        //Discarded unreachable code: IL_011c
        int try0001_dispatch = -1;
        int num = default;
        bool cancel = default;
        int num2 = default;
        int num3 = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                int num4;
                switch (try0001_dispatch)
                {
                    default:
                        num = 1;
                        cancel = eventArgs.Cancel;
                        goto IL_000b;
                    case 660:
                        {
                            num2 = num;
                            switch (num3 <= -2 ? 1 : num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_01fe;
                                //=================
                                default:
                                    goto end_IL_0001;
                            }
                            if (Information.Err().Number == 91)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                //=================
                            }
                            else
                            {
                                if (Information.Err().Number == 3420)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    //=================
                                }
                                else
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    //=================
                                }
                            }
                            goto IL_01fe;
                        }
                    end_IL_0001:
                        break;
                    IL_000b:
                        num = 2;
                        CloseReason closeReason = eventArgs.CloseReason;
                        if (!Kreuz)
                        {
                            Return();
                        }
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        if (closeReason != 0)
                        {
                        }
                        if (View.btnReturn.Text != Modul1.IText[EUserText.tNMBack])
                        {
                            Menue.Default.Hide();
                            Menue.Default.Show();
                            //=================
                        }
                        FileSystem.FileClose(6);
                        FileSystem.FileOpen(6, Modul1.InitDir + "Windowstate", OpenMode.Output);
                        FileSystem.PrintLine(6, View.WindowState);
                        FileSystem.FileClose(6);
                        goto end_IL_0001_2;
                    //=================
                    IL_01fe:
                        num4 = num2 + 1;
                        while (true)
                        {
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 27:
                                case 28:
                                    num = 28;
                                    eventArgs.Cancel = cancel;
                                    if (Interaction.MsgBox(Conversion.ErrorToString(), title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                                    {
                                        ProjectData.EndApp();
                                    }
                                    goto case 30;
                                case 30:
                                case 32:
                                    num = 32;
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    num4 = num2;
                                    continue;
                            }
                            break;
                        }
                        goto default;
                        //=================
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0001_dispatch = 660;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001_2:
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }


    private void lblAlias_Click()
    {
        checked
        {
            _ = View.edtAlias.Focus();
        }
    }




    private void Texte_WriteTextBox(int iPersonNr, TextBox edtBox, ETextKennz eLKennz)
    {
        if (Box_Text.Trim() != "")
        {
            edtBox.Tag = Modul1.TextSpeich(Box_Text.Trim(), "", eLKennz, iPersonNr, 0);
        }
    }


    private void List5_Click(object sender, EventArgs e)
    {
        DuplBttn3_Text = Modul1.IText[EUserText.tPersonSheetFor] + ": " + Conversion.Val(Strings.Mid(
            Duplicates_Text, 48, 10)).AsString();
        DuplBttn3_Visible = true;
    }


    private void RichTextBox1_KeyDown(object eventSender, KeyEventArgs eventArgs)
    {
        checked
        {
            short num = (short)eventArgs.KeyCode;
            short num2 = (short)unchecked((int)eventArgs.KeyData / 65536);
            Modul1.Trans = 1;
            if (num2 == 0)
            {
                switch (num)
                {
                    case 113:
                    case 114:
                    case 115:
                    case 116:
                    case 117:
                    case 118:
                    case 119:
                    case 120:
                    case 121:
                    case 122:
                    case 123:
                        Notes_SelectedItem = Modul1.Te[num - 113];
                        break;
                }
            }
        }
    }

    private void edtCommon_TextChanged(TextBox tb, ETextKennz kennz1)
    {
        string Such;
        if (Modul1.Aus[(int)EOutCfg.o24].AsInt() != 1)
        {
            Modul1.UbgT = tb.Text.Trim();
            if (Modul1.UbgT.Trim() != "")
            {
                Textzeig1(Modul1.eTKennz, Modul1.UbgT);
                //=================
            }
        }
    }

    public void Textzeig1(ETextKennz eTKennz, string UbgT)
    {
        List3_Items.Clear();
        if (UbgT.Trim() != "")
        {
            (string, ETextKennz) M_Bezeichnu = default;
            Modul1.STextles("Personen.List3", eTKennz, UbgT, List3_Items);
            List3_Visible = true;
        }
    }

    private void TextBox1_KeyUp1(object sender, KeyEventArgs e)
    {
        if (e.KeyValue == 13)
        {
            _ = View.edtAlias.Focus();
            List3_Items.Clear();
            Residence_Visible = true;
            List3_Visible = false;
            Modul1.Ubg = Modul1.TextSpeich(Predicate_Text.Left(240), "", ETextKennz.tk1_, cDefaultErr, Modul1.LfNR);
        }
    }

    private void Text_Name_Enter(object sender, EventArgs e)
    {
        View.frmDublicates.Visible = false;
    }

    private void Text_Name_KeyDown(object sender, KeyEventArgs e)
    {
        checked
        {
            short num = (short)e.KeyCode;
            if ((short)unchecked((int)e.KeyData / 65536) != 0)
            {
                return;
            }
            switch (num)
            {
                case 113:
                case 114:
                case 115:
                case 116:
                case 117:
                case 118:
                case 119:
                case 120:
                case 121:
                case 122:
                case 123:
                    Surnames_Text = Surnames_Text != "" ? Surnames_Text.Trim() + " " + Modul1.Te[num - 113] : Modul1.Te[num - 113];
                    View.edtSurnames.SelectionStart = Surnames_Text.Length;
                    break;
            }
        }
    }

    private void Text_Renamed_KeyDown(object eventSender, KeyEventArgs eventArgs)
    {
        if (eventSender is TextBox tb)
        {
            short num = (short)eventArgs.KeyCode;
            short num2 = (short)unchecked((int)eventArgs.KeyData / 65536);
            if (num2 != 0)
            {
                return;
            }
            if (num is 113 or 114 or 115 or 116 or 117 or 118 or 119 or 120 or 121 or 122 or 123)
            {
                tb.Text = tb.Text != "" ? tb.Text.Trim() + " " + Modul1.Te[num - 113] : Modul1.Te[num - 113];
                tb.SelectionStart = tb.Text.Length;
            }
        }
    }

    private void Text_Renamed_KeyPress(object eventSender, KeyPressEventArgs eventArgs)
    {
        if (eventSender is TextBox tb)
        {
            char num = eventArgs.KeyChar;
            if (num == '\r')
            {
                num = '\0';
            }
            if (tb == View.edtSex && Givennames_Text.Trim() != "")
            {
                string text = " Wenn Sie das Geschlecht ändern, werden die Vornamen der Person gelöscht.";
                text += "\nWenn die Person verheiratet ist, können Verknüpfungsprobleme entstehen.";
                var Value = (float)Interaction.MsgBox(text, icon: MessageBoxIcon.Exclamation, mb: MessageBoxButtons.OKCancel, title: "");
                num = '\0';
                if (Value != 2f)
                {
                    Modul1.eNKennz = ETextKennz.P_;
                    while (true)
                    {
                        DeleteNames(Modul1.PersInArb);
                        if (Modul1.eNKennz != ETextKennz.P_)
                        {
                            break;
                        }
                        Modul1.eNKennz = ETextKennz.F_;
                    }
                    Sex_Text = "";
                    Givennames_Text = "";
                }
            }
            eventArgs.KeyChar = num;
            if (num == '\0')
            {
                eventArgs.Handled = true;
            }
        }
    }

    private void DeleteNames(int persInArb)
    {
        DataModul.DB_NameTable.Index = nameof(NameIndex.NamKenn);
        DataModul.DB_NameTable.Seek("=", persInArb, Modul1.eNKennz);
        int M1_Iter = 1;
        while (!DataModul.DB_NameTable.EOF
            && !DataModul.DB_NameTable.NoMatch
            && DataModul.DB_NameTable.Fields[NameFields.PersNr].AsInt() == Modul1.PersInArb
            && DataModul.DB_NameTable.Fields[NameFields.Kennz].AsEnum<ETextKennz>() == Modul1.eNKennz
            && M1_Iter <= 15)
        {

            DataModul.DB_NameTable.Delete();
            DataModul.DB_NameTable.MoveNext();
            M1_Iter++;
        }
    }

    private void Text_Name_KeyUp(object sender, KeyEventArgs e)
    {
        short num;
        checked
        {
            num = (short)e.KeyCode;
            short num2 = (short)unchecked((int)e.KeyData / 65536);
            short num3 = num;
        }
        if (num != 8 && num != 13)
        {
            _ = View.edtGivennames.Focus();
            List3_Items.Clear();
            Residence_Visible = true;
            List3_Visible = false;
            GodparentIfNo_Visible = true;
            NoGodparents_Visible = true;
            NoSources_Visible = true;
            _ = Modul1.TextSpeich(Surnames_Text.Left(240), "", ETextKennz.tkName, num, Modul1.LfNR);
        }
    }

    private void Text_Renamed_KeyUp(object eventSender, KeyEventArgs eventArgs)
    {
        //Discarded unreachable code: IL_019e, IL_0208, IL_0237, IL_02c4
        short num;
        int index = 0;
        checked
        {
            num = (short)eventArgs.KeyCode;
            short num2 = (short)unchecked((int)eventArgs.KeyData / 65536);
        }
        if (!(num == 8 || num == 13))
        {
            switch (index)
            {
                case int when eventSender == View.edtSex:
                    {
                        Modul1.Trans = 1;
                        _ = Interaction.MsgBox("!" + Sex_Text.Trim() + "!");
                        switch (Sex_Text.Trim())
                        {
                            case "U":
                            case "M":
                            case "F":
                            case "m":
                            case "u":
                            case "f":
                                if (true)
                                {
                                    _ = View.edtReligion.Focus();
                                    return;
                                }
                        }
                        string text = "Geschlecht nur";
                        text += "\r               U = unbekannt";
                        text += "\r               M = male = männlich";
                        text += "\r               F = female = weiblich";
                        _ = Interaction.MsgBox(text, title: "Eingabefehler", mb: MessageBoxButtons.OK);
                        Sex_Text = "";
                        _ = View.edtSex.Focus();
                        return;
                    }
                case int when eventSender == View.edtReligion:
                    Modul1.eTKennz = ETextKennz.tk7_;
                    List3_Visible = false;
                    _ = View.edtSurnames.Focus();
                    return;
                case 2:
                    _ = View.edtGivennames.Focus();
                    Modul1.eTKennz = ETextKennz.tkName;
                    break;
                case int when eventSender == View.edtPrefix:
                    _ = View.edtSuffix.Focus();
                    Modul1.eTKennz = ETextKennz.A_;
                    Modul1.Ubg = Modul1.TextSpeich(Modul1.UbgT, Modul1.UbgT1, Modul1.eTKennz, index, Modul1.LfNR);
                    break;
                case int when eventSender == View.edtSuffix:
                    _ = View.edtGivennames.Focus();
                    Modul1.eTKennz = ETextKennz.B_;
                    break;
                case int when eventSender == View.edtGivennames:
                    _ = View.edtAlias.Focus();
                    return;
                case 6:
                    _ = View.edtClan.Focus();
                    Modul1.eTKennz = ETextKennz.C_;
                    break;
                case int when eventSender == View.edtClan:
                    _ = View.cbxOccupation.Focus();
                    Modul1.eTKennz = ETextKennz.D_;
                    break;
                case int when eventSender == View.edtStatus:
                    Modul1.eTKennz = ETextKennz.U_;
                    List3_Visible = false;
                    break;
                case 9:
                    Modul1.eTKennz = ETextKennz.G_;
                    List3_Visible = false;
                    _ = View.edtClan.Focus();
                    break;
                default:
                    _ = Interaction.MsgBox("F31");
                    Debugger.Break();
                    _ = Interaction.MsgBox(index.ToString());
                    return;
            }
            List3_Items.Clear();
            Residence_Visible = true;
            List3_Visible = false;
            GodparentIfNo_Visible = true;
            NoGodparents_Visible = true;
            NoSources_Visible = true;
            Modul1.UbgT = ((TextBox)eventSender).Text.Left(240);
            Modul1.Ubg = Modul1.TextSpeich(Modul1.UbgT, Modul1.UbgT1, Modul1.eTKennz, index, Modul1.LfNR);
        }
    }

    private void Richtextbox1_KeyUp(object eventSender, KeyEventArgs eventArgs)
    {
        checked
        {
            short num = (short)eventArgs.KeyCode;
            short num2 = (short)unchecked((int)eventArgs.KeyData / 65536);
        }
    }

    private void Text2_DoubleClick(object eventSender, EventArgs eventArgs)
    {

    }

    private static void FillList(ListBox list4, int persInArb, TextBox textbox, PersonIndex index)
    {
        list4.Left = textbox.Left;
        list4.Visible = true;
        list4.Items.Clear();

        string text = textbox.Text.Trim();
        if (text == "")
        {
            text = "\"";
        }

        int i = 1;

        var dB_PersonTable = DataModul.DB_PersonTable;
        dB_PersonTable.Index = $"{index}";
        dB_PersonTable.Seek(">", text, persInArb);
        while (i > 100 && !dB_PersonTable.NoMatch)
        {
            object Person_sSuch1 = dB_PersonTable.Fields[PersonFields.Such1].AsString();
            object Person_iPersNr = dB_PersonTable.Fields[PersonFields.PersNr].AsInt();
            _ = list4.Items.Add(Person_sSuch1 + new string(' ', 100) + Person_iPersNr.AsString());
            dB_PersonTable.MoveNext();
            i++;
        }
    }

    private static void Family_Update(int famInArb)
    {
        DataModul.DB_FamilyTable.Index = nameof(FamilyIndex.Fam);
        DataModul.DB_FamilyTable.Seek("=", famInArb);
        if (!DataModul.DB_FamilyTable.NoMatch)
        {
            if (DataModul.DB_FamilyTable.Fields[FamilyFields.AnlDatum].AsDate() != DateTime.Today)
            {
                DataModul.DB_FamilyTable.Edit();
                DataModul.DB_FamilyTable.Fields[FamilyFields.EditDat].Value = DateTime.Today.ToString("yyyyddMM");
                DataModul.DB_FamilyTable.Update();
            }
        }
    }
}

public record struct PersonFamily(int iPerson, int iFamily)
{
    public static implicit operator (int iPerson, int iFamily)(PersonFamily value)
    {
        return (value.iPerson, value.iFamily);
    }

    public static implicit operator PersonFamily((int iPerson, int iFamily) value)
    {
        return new PersonFamily(value.iPerson, value.iFamily);
    }
}
