using BaseLib.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GenFreeWin.Views;
using GenFree;
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces.Data;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Sys;
using GenFree.Interfaces.VB;
using GenFree.ViewModels.Interfaces;
using GenFreeWin.Views;
using Microsoft.VisualBasic;
using MVVM.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GenFreeWin.ViewModels;

public partial class RegSuchViewModel : BaseViewModelCT, IRegSuchViewModel
{
    IContainerControl IRegSuchViewModel.View { get; set; }
    private Regsuch View => (Regsuch)(this as IRegSuchViewModel).View;
    IModul1 Modul1 => _Modul1.Instance;

    [Obsolete]
    IProjectData ProjectData => Modul1.ProjectData;
    IInteraction Interaction => Menue.Default;
    [Obsolete]
    IVBInformation Information => Modul1.Information;
    [Obsolete]
    IVBConversions Conversions => Modul1.Conversions;
    [Obsolete]
    IStrings Strings => Modul1.Strings;
    [Obsolete]
    IStrings StringType => Modul1.Strings;

    public IList LB1_Items => View.ListBox1.Items;

    public IList<ListItem<PersonFamily>> List6_Items => [];
    public IList<ListItem<int>> List4_Items => [];


    [ObservableProperty]
    public partial string Text1_Text { get; set; }


    private string[] KontSP;
    private string[] KontSP1;

    private int S;
    private object Modul1_LiText;
    private int W;
    private int Perc;

    private int Famc;

    private Keys Modul1_EingCode;
    private string Modul1_Kont20;
    private string List6_Text;

    public RegSuchViewModel()
    {
        KontSP = new string[101];
        KontSP1 = new string[101];
    }


    public void Form_Load(object eventSender, EventArgs eventArgs)
    {
        Modul1.Suchfam = 0;
        Modul1.SuchPer = 0;
        Modul1.Persistence.ReadEnumInit<FormWindowState>("Windowstate", out var WiS);
        View.WindowState = WiS;
        View.BackColor = Modul1.HintFarb;
        View._Label1_1.Visible = false;
        if (Modul1.FontSize > 0f)
        {
            View.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.ListBox1.Font = new Font("Courier New", Modul1.FontSize, FontStyle.Regular);
            View.Label3.Font = new Font("Courier New", Modul1.FontSize, FontStyle.Regular);
            View.lblChld05Info.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.lblChild05.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.lblChld04Info.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.lblChild04.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.lblChld03Info.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.lblChild03.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.lblChld02Info.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.lblChild02.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.lblChild01.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.Label69.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.Label71.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.Label72.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.lblChld01Info.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.Label74.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.Label75.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.Label2.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
        }

        View.DesktopLocation = Personen.Default.DesktopLocation;
        FileSystem.FileClose(99);
        if (Modul1.Typ != DriveType.CDRom)
        {
            View.Combo1.Items.Clear();
            View.Combo1.Text = "";
            View.Combo1.Items.AddRange(Modul1.Persistence.ReadStringsMand("RSUCH.DAT", 20).ToArray());

            var eArt = Modul1.Persistence.ReadEnumMand<EEventArt>("FRSUCH.DAT");
            View_SetOption(eArt);
        }
        goto end_IL_0001_2;
    end_IL_0001_2: // <========== 4
        return;
    }

    private void View_SetOption(EEventArt eArt)
    {
        switch (eArt)
        {
            case EEventArt.eA_105:
                View._Option1_0.Checked = true;
                break;
            case EEventArt.eA_100:
                // Dummy: All Person registers
                View._Option1_1.Checked = true;
                break;
            case EEventArt.eA_Burial:
                View._Option1_2.Checked = true;
                break;
            case EEventArt.eA_Death:
                View._Option1_3.Checked = true;
                break;
            case EEventArt.eA_Baptism:
                View._Option1_4.Checked = true;
                break;
            case EEventArt.eA_Birth:
                View._Option1_5.Checked = true;
                break;
            case EEventArt.eA_500:
                View._Option1_6.Checked = true;
                break;
            case EEventArt.eA_501:
                View._Option1_7.Checked = true;
                break;
            case EEventArt.eA_603:
                View._Option1_8.Checked = true;
                break;
            case EEventArt.eA_900:
                // Dummy: All Family registers
                View._Option1_9.Checked = true;
                break;
            case EEventArt.eA_Marriage:
                View._Option1_10.Checked = true;
                break;
            case EEventArt.eA_MarrReligious:
                View._Option1_11.Checked = true;
                break;
            case EEventArt.eA_504:
                View._Option1_12.Checked = true;
                break;
            case EEventArt.eA_507:
                View._Option1_13.Checked = true;
                break;
            default:
                // Handle unknown or unsupported event art
                View._Option1_11.Checked = true;
                break;
        }
    }

    public void Regsuch_FormClosed(object eventSender, FormClosedEventArgs eventArgs)
    {
        if (Modul1.Typ != DriveType.CDRom)
        {
            Suchspeich();
        }
    }

    public void Command1_Click(object eventSender, EventArgs eventArgs)
    {
        short index = (short)View.ACommand1.GetIndex((Button)eventSender);
        switch (index)
        {
            case 0:
                Modul1.Ubg = 0;
                Suchspeich();
                View.Close();
                break;
            case 3:
                Command1_3_Click();
                break;
            case 7:
                Modul1.Listbox3Clip(View.ListBox1.Items, 1);
                break;
            default:
                break;
        }
    end_IL_0001_2: // <========== 6
        return;
    }

    private void Command1_3_Click()
    {
        Text1_Text = View.Combo1.Text.Trim();
        if (Text1_Text == "")
        {
            Text1_Text = "1";
        }
        if (View.Combo1.Text != "")
        {
            if (View.Combo1.Items.Count == 0 || View.Combo1.Items[0].AsString().Trim() != Text1_Text.Trim())
            {
                View.Combo1.Items.Insert(0, Text1_Text);
            }
            View.Combo1.SelectedIndex = 0;
        }

        Listfuell(LB1_Items);
    }

    public void Label5_DoubleClick(object eventSender, EventArgs eventArgs)
    {
        //Discarded unreachable code: IL_00f6
        int try0001_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
        ;
        int num4;
        short index;
        switch (try0001_dispatch)
        {
            default:
                num = 1;
                index = (short)View.ALabel5.GetIndex((Label)eventSender);
                goto IL_0015;
            case 514:
                {
                    num2 = num;
                    switch ((num3 <= -2) ? 1 : num3)
                    {
                        case 2:
                            break;
                        case 1:
                            goto IL_0176;
                        default:
                            goto end_IL_0001;
                    }
                    if (Information.Err().Number == 402)
                    {
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        goto IL_0176;
                    }
                    else
                    {
                        if (Interaction.MsgBox(Conversions.ErrorToString(), title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                        {
                            ProjectData.EndApp();
                        }
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        num4 = num2;
                        goto IL_0179;
                    }
                }
            end_IL_0001:
                break;
            IL_0015:
                ProjectData.ClearProjectError();
                num3 = 2;
                if (Modul1.Suchschalt == 2)
                {
                    goto end_IL_0001_2;
                }
                if (Modul1.Suchschalt == 10)
                {
                    goto end_IL_0001_2;
                }
                Modul1.PersInArb = View.ALabel7[index].Tag.AsInt();
                if (Modul1.PersInArb <= 0)
                {
                    goto end_IL_0001_2;
                }
                if (Modul1.Suchschalt == 1)
                {
                    Familie.Default.Hide();
                    Personen.Default.Show(Modul1.PersInArb, EUserText.t158);
                    Personen.Default.Perzeig(Modul1.PersInArb);
                    Modul1.Ubg = Modul1.PersInArb;
                }
                else
                {
                    Modul1.SuchPer = Modul1.PersInArb;
                }
                goto end_IL_0001_2;
            IL_0176:
                num4 = num2 + 1;
                goto IL_0179;
            IL_0179:
                num2 = 0;
                switch (num4)
                {
                    case 1:
                        break;
                    case 4:
                    case 7:
                    case 16:
                    case 19:
                    case 20:
                    case 21:
                    case 22:
                    case 31:
                        goto end_IL_0001_2;
                }
                goto default;
        }
    end_IL_0001_2: // <========== 5
        return;
    }
    private void List1_SelectedIndexChanged(object eventSender, EventArgs eventArgs)
    {
        int famInArb1 = View.ListBox1.SelectedItem.ItemData<PersonFamily>().iFamily;
        int persInArb = View.ListBox1.SelectedItem.ItemData<PersonFamily>().iPerson;

        View_UpdateTree(famInArb1, persInArb);
        return;
    }

    private void View_UpdateTree(int famInArb1, int persInArb)
    {
        View_Baumleer();
        if (famInArb1 > 0)
        {
            DataModul.Family.ReadData(famInArb1, out var cFamily);
            DataModul.Link.ReadFamily(famInArb1, cFamily);
            View_SetLabel(SetPersonLI(cFamily.Mann), View.Label70);
            View_SetLabel(SetPersonLI(cFamily.Frau), View.Label69);

            View.Label11.Tag = famInArb1;

            if (cFamily.Mann > 0)
            {
                var aiFam = Modul1.Link_Famsuch(cFamily.Mann, ELinkKennz.lkChild);
                if (aiFam.Count > 0)
                {
                    famInArb1 = aiFam[0];
                    View.Label9.Tag = famInArb1;

                    DataModul.Family.ReadData(famInArb1, out var cFamilyM);
                    DataModul.Link.ReadFamily(famInArb1, cFamilyM);
                    View_SetLabel(SetPersonLI(cFamilyM.Mann), View.Label75);
                    View_SetLabel(SetPersonLI(cFamilyM.Frau), View.Label74);
                }
            }
            if (cFamily.Frau > 0)
            {
                var aiFams = Modul1.Link_Famsuch(cFamily.Frau, ELinkKennz.lkChild);
                if (aiFams.Count > 0)
                {
                    famInArb1 = aiFams[0];
                    View.Label10.Tag = famInArb1;

                    DataModul.Family.ReadData(famInArb1, out var cFamilyF);
                    DataModul.Link.ReadFamily(famInArb1, cFamilyF);
                    View_SetLabel(SetPersonLI(cFamilyF.Mann), View.Label72);
                    View_SetLabel(SetPersonLI(cFamilyF.Frau), View.Label71);
                }
            }

            if (cFamily.Kind.Count() > 0)
            {
                View.Label31.Tag = Famc;
                var M1_Iter = 0;
                while (M1_Iter < cFamily.Kind.Count()
                    && M1_Iter < 5)
                {
                    persInArb = cFamily.Kind[M1_Iter];
                    switch (M1_Iter)
                    {
                        case 0:
                            SetChildLabels(persInArb, View.lblChild01, View.lblChld01Info, List4_Items[M1_Iter].ItemString.Left(4).AsInt());
                            break;
                        case 1:
                            SetChildLabels(persInArb, View.lblChild02, View.lblChld02Info, List4_Items[M1_Iter].ItemString.Left(4).AsInt());
                            break;
                        case 2:
                            SetChildLabels(persInArb, View.lblChild03, View.lblChld03Info, List4_Items[M1_Iter].ItemString.Left(4).AsInt());
                            break;
                        case 3:
                            SetChildLabels(persInArb, View.lblChild04, View.lblChld04Info, List4_Items[M1_Iter].ItemString.Left(4).AsInt());
                            break;
                        case 4:
                            SetChildLabels(persInArb, View.lblChild05, View.lblChld05Info, List4_Items[M1_Iter].ItemString.Left(4).AsInt());
                            break;
                        default:
                            break;
                    }
                    M1_Iter++;
                }
            }
        }
        else
        {
            View_SetLabel(SetPersonLI(persInArb), View.Label70);

            var aiFams = Modul1.Link_Famsuch(persInArb, Modul1.eLKennz = ELinkKennz.lkChild);
            famInArb1 = aiFams.FirstOrDefault();
            //!!
            View.Label9.Tag = famInArb1;

            DataModul.Family.ReadData(famInArb1, out var cFamily);
            DataModul.Link.ReadFamily(famInArb1, cFamily);
            View_SetLabel(SetPersonLI(cFamily.Mann), View.Label75);
            View_SetLabel(SetPersonLI(cFamily.Frau), View.Label74);
        }
    }

    private void View_SetLabel(IListItem<int> listItem, Label label70)
    {
        label70.Text = listItem.ItemString;
        label70.Tag = listItem.ItemData;
        ;
    }

    private IListItem<int> SetPersonLI(int iPers) => new ListItem<int>(iPers > 0 ? PersonLabelText(iPers) : "", iPers);

    private string PersonLabelText(int iPerson)
    {
        var Person_iFather = iPerson;
        Modul1.Person_ReadNames(Person_iFather, Modul1.Person);
        if (Modul1.Person.Suffix.Trim() != "")
        {
            Modul1.Person.SetFullSurname(Modul1.Person.SurName + " " + Modul1.Person.Suffix);
        }
        if (Modul1.Person.Prefix != "")
        {
            Modul1.Person.SetFullSurname(Modul1.Person.Prefix + " " + Modul1.Person.SurName);
        }
        ;
        var text = Modul1.Person.Givennames + " " + Modul1.Person.FullSurName;
        var (sDat_Birth, sDat_Death) = Datr(Person_iFather);
        return sDat_Birth + " " + text + sDat_Death;
    }

    private void SetChildLabels(int persInArb, Label lblChldName, Label lblChldInfo, int iNumber)
    {
        IPersonData person = Modul1.Person;
        Modul1.Person_ReadNames(persInArb, person);
        lblChldName.Tag = persInArb;
        lblChldName.Text = $"{iNumber} {person.Givennames} {person.SurName}";
        lblChldInfo.Tag = Perc;
        lblChldInfo.Text = Kindfamsuch(persInArb);
    }

    private void List1_DoubleClick(object eventSender, EventArgs eventArgs)
    {
        //Discarded unreachable code: IL_018d
        int try0001_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int num4;
        switch (try0001_dispatch)
        {
            default:
                ProjectData.ClearProjectError();
                num3 = 2;
                goto IL_0008;
            case 615:
                {
                    num2 = num;
                    switch ((num3 <= -2) ? 1 : num3)
                    {
                        case 2:
                            break;
                        case 1:
                            goto IL_01db;
                        default:
                            goto end_IL_0001;
                    }
                    if (Interaction.MsgBox(Conversions.ErrorToString(), title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    {
                        ProjectData.EndApp();
                    }
                    ProjectData.ClearProjectError();
                    if (num2 == 0)
                    {
                        throw ProjectData.CreateProjectError(-2146828268);
                    }
                    num4 = num2;
                    goto IL_01de;
                }
            end_IL_0001:
                break;
            IL_0008:
                num = 2;
                Modul1.Suchfam = View.ListBox1.SelectedItem.ItemData<PersonFamily>().iFamily;
                Modul1.SuchPer = View.ListBox1.SelectedItem.ItemData<PersonFamily>().iPerson;
                Modul1.Ubg = 0;
                if (Modul1.Schalt == 1)
                {
                    Modul1.Ubg = View.ListBox1.SelectedItem.ItemData<int>();
                    if (Modul1.Ubg == 0)
                    {
                        if (View.ACheck2[3].Checked)
                        {
                            Suchspeich();
                        }
                        Familie.Default.Hide();
                        Personen.Default.Close();
                        Modul1.Suchschalt = 2;
                        Personen.Default.Show(Modul1.PersInArb, EUserText.t158);
                        Modul1.Ubg = Modul1.PersInArb;
                    }
                }
                else
                {
                    Modul1.Ubg = Strings.Mid(View.ListBox1.Text, 48, 10).AsInt();
                    //!!
                    if (View.ACheck2[3].Checked)
                    {
                        Suchspeich();
                        View.Hide();
                    }
                }
                Modul1.Schalt = 0;
                goto end_IL_0001_2;
            IL_01db:
                num4 = unchecked(num2 + 1);
                goto IL_01de;
            IL_01de:
                num2 = 0;
                switch (num4)
                {
                    case 1:
                        break;
                }
                goto default;
        }
    end_IL_0001_2:
        return;
    }
    public void Option1_CheckedChanged(object eventSender, EventArgs eventArgs)
    {

        if (((CheckBox)eventSender).Checked)
        {
            var index = (short)View.AOption1.GetIndex((RadioButton)eventSender);
            View.ListBox1.Items.Clear();
            View_Baumleer();
            ProjectData.ClearProjectError();
            View.Label3.Text = "Register-Nr.   Name,Vorname           Datum JJJJ  Personennr. Heirat       Partner";
            _ = View.Combo1.Focus();
        }
    }

    public void Perles1(int persInArb, CPersonData person)
    {
        var dB_NameTable = DataModul.DB_NameTable;
        person.Clear();
        _ = DataModul.Names.ReadPersonNames(persInArb, out var iName, out var atVorn);
        person.SetPersonNames(iName, atVorn, Modul1.Aus[20] == "Y");
    }

    public IList<int> Ehesuch(int persInArb)
    {

        Modul1.eLKennz = DataModul.Person.GetSex(persInArb) switch
        {
            "M" => ELinkKennz.lkFather,
            "F" => ELinkKennz.lkMother,
            _ => ELinkKennz.lkNone
        };

        return DataModul.Link.GetPersonFams(persInArb, Modul1.eLKennz);

    }

    public void Datwand1(int Beruf, ref string Datu, string Ds)
    {
        int M1_Iter;
        var num = 1;
        var num5 = default(short);
        if (Datu.Length < 8)
        {
            Datu = Datu.PadRight(8, ' ');
        }

        if (Strings.Mid(Datu, 7, 2).AsDouble() == 0.0)
        {
            StringType.MidStmtStr(ref Datu, 7, 2, "  ");
        }
        if (Strings.Mid(Datu, 5, 2).AsDouble() == 0.0)
        {
            StringType.MidStmtStr(ref Datu, 5, 2, "  ");
        }
        Datu = Strings.Mid(Datu, 7, 2) + " " + Strings.Mid(Datu, 5, 2) + " " + Datu.Left(4);
        Datu = Datu.TrimStart().TrimEnd();
        if (Datu.Length > 0)
        {
            M1_Iter = 1;
            while (M1_Iter <= 2)
            {
                num5 = (short)Strings.InStr(Datu, " ");
                if (num5 > 0)
                {
                    StringType.MidStmtStr(ref Datu, num5, 1, ".");
                }
                M1_Iter++;
            }
        }
        var left = Ds;
        switch (left)
        {
            case "U" or "u":
                Datu = "um " + Datu;
                break;
            case "V" or "v":
                Datu = "vor " + Datu;
                break;
            case "N" or "n":
                Datu = "nach " + Datu;
                break;
            case "?":
                Datu += " ?";
                break;
            case "R" or "r":
                Datu = "errech. " + Datu;
                break;
            default:
                if (Beruf == 0 && Datu.Length == 10)
                    Datu = "am " + Datu;
                break;
        }
    }

    public string AhnnenTable_Read(int persInArb)
    {
        //Discarded unreachable code: IL_038d
        int try0001_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int lErl = default;
        string result = "";
        int num4;
        switch (try0001_dispatch)
        {
            default:
                ProjectData.ClearProjectError();
                num3 = 2;
                goto IL_0008;
            case 1204:
                {
                    num2 = num;
                    switch ((num3 <= -2) ? 1 : num3)
                    {
                        case 2:
                            break;
                        case 1:
                            goto IL_03f8;
                        default:
                            goto end_IL_0001;
                    }
                    if (Information.Err().Number == 91)
                    {
                        goto end_IL_0001_2;
                    }
                    if (Interaction.MsgBox(Conversions.ErrorToString(), title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    {
                        ProjectData.EndApp();
                    }
                    ProjectData.ClearProjectError();
                    if (num2 == 0)
                    {
                        throw ProjectData.CreateProjectError(-2146828268);
                    }
                    num4 = num2;
                    goto IL_03fb;
                }
            end_IL_0001:
                break;
            IL_0008:
                num = 2;
                IRecordset dT_AncesterTable = DataModul.DT_AncesterTable;
                IRecordset dT_DescendentTable = DataModul.DT_DescendentTable;
                Modul1_Kont20 = "";
                Modul1.Kont[10] = "";
                string birthday = "";
                lErl = 1;
                dT_AncesterTable.Index = "PerNr";
                dT_AncesterTable.Seek("=", persInArb);
                while (!dT_AncesterTable.NoMatch
                    && dT_AncesterTable.Fields["Ahn"].AsInt() > 0
                    && !(dT_AncesterTable.Fields["PerNr"].AsInt() != persInArb) && dT_AncesterTable.Fields["Weiter"].AsInt() != 0)
                {
                    if (dT_AncesterTable.Fields["Weiter"].AsInt() != 0)
                    {
                        Modul1_Kont20 = ">>";
                    }
                    if (birthday != "")
                    {
                        birthday = birthday + "; " + dT_AncesterTable.Fields["Ahn"].AsString().Trim();
                    }
                    else
                    {
                        birthday = dT_AncesterTable.Fields["Ahn"].AsString();
                    }
                    Modul1.Kont[10] = "Generation " + dT_AncesterTable.Fields["Gen"].AsString() + " Ahn-Nr.: " + birthday.Trim();
                    dT_AncesterTable.MoveNext();
                }
                lErl = 2;
                dT_DescendentTable.Index = "PerNr";
                dT_DescendentTable.Seek("=", persInArb);

                if (!dT_DescendentTable.NoMatch)
                    result = $"{Modul1.IText[EUserText.t239]} {dT_DescendentTable.Fields["Gen"].Value}-{dT_DescendentTable.Fields["Nr"].Value}";
                goto end_IL_0001_2;
            IL_03f8:
                num4 = num2 + 1;
                goto IL_03fb;
            IL_03fb:
                num2 = 0;
                switch (num4)
                {
                    case 1:
                        break;
                    case 34:
                    case 35:
                    case 37:
                    case 43:
                        goto end_IL_0001_2;
                }
                goto default;
        }
    end_IL_0001_2: // <========== 4
        return result;
    }

    public void kindsuch(int famInArb, IList<ListItem<int>> list4_Items)
    {
        int M1_Iter = 1;
        foreach (var link in DataModul.Link.ReadAllFams(famInArb, ELinkKennz.lkChild))
        {
            var dt1 = DataModul.Event.GetDate(EEventArt.eA_Birth, link.iPersNr);
            if (dt1 == default)
                dt1 = DataModul.Event.GetDate(EEventArt.eA_Baptism, link.iPersNr);

            string text = dt1 == default ? "00000000" : dt1.ToString("yyyyMMdd");

            list4_Items.Add(new(text + link.iPersNr.AsString(), Modul1.PersInArb));

            if (M1_Iter++ > 99)
                break;
        }
    }

    public void Listfuell(IList LB1_items)
    {
        int try0001_dispatch = -1;
        int num = default;
        num = 1;
        if (Modul1.Aus[12] == "")
        {
            Modul1.Aus[12] = "200";
        }
        num = 4;
        if (Modul1.Aus[13] == "")
        {
            Modul1.Aus[13] = "100";
        }
        S = 0;
        View.Combo1.Text = Text1_Text;
        if (View.Combo1.Items.Count > 0 && View.Combo1.Text == "")
        {
            View.Combo1.Text = View.Combo1.Text[0].AsString();
            Text1_Text = View.Combo1.Text[0].AsString();
        }
        if (Text1_Text == "")
        {
            Text1_Text = "1";
        }
        if (View.Combo1.Text == "")
        {
            _ = LB1_items.Add("Ende der Liste");
            goto end_IL_0001_2;
        }


        View.Combo1.Text = Text1_Text;
        ProjectData.ClearProjectError();
        LB1_items.Clear();
        int M1_Iter = default;
        if (Text1_Text != "")
        {
            if (View._Option1_1.Checked)
            {
                // All Personregisters
                List6_Items.Clear();
                DataModul.DB_EventTable.Index = nameof(EventIndex.Reg);
                DataModul.DB_EventTable.Seek(">=", Text1_Text);
                M1_Iter = 0;
                while (M1_Iter < Modul1.Aus[13].AsInt()
                   && !DataModul.DB_EventTable.EOF
                   && (DataModul.DB_EventTable.Fields[EventFields.Art].AsInt() <= 299)
                   && !DataModul.DB_EventTable.NoMatch
                   && string.Compare(DataModul.DB_EventTable.Fields[EventFields.Reg].AsString(), Text1_Text.ToUpper()) >= 0)
                {
                    if (!DataModul.DB_EventTable.NoMatch)
                    {
                        M1_Iter++;
                        string sReg = DataModul.DB_EventTable.Fields[EventFields.Reg].AsString();
                        int iEntity = DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt();
                        List6_Items.Add(DataToPFListItem(sReg, iEntity, 0));
                    }

                    DataModul.DB_EventTable.MoveNext();
                }
            }
            else if (View._Option1_9.Checked)
            {
                // All Familyregisters
                List6_Items.Clear();
                DataModul.DB_EventTable.Index = nameof(EventIndex.Reg);
                DataModul.DB_EventTable.Seek(">=", Text1_Text);
                M1_Iter = 0;
                while (!DataModul.DB_EventTable.EOF
                    && !DataModul.DB_EventTable.NoMatch
                    && (DataModul.DB_EventTable.Fields[EventFields.Art].AsInt() > 499)
                   && (M1_Iter < Modul1.Aus[13].AsInt())
                    && string.Compare(DataModul.DB_EventTable.Fields[EventFields.Reg].AsString(), Text1_Text.ToUpper()) >= 0)
                {
                    EEventArt Event_eArt = DataModul.DB_EventTable.Fields[EventFields.Art].AsEnum<EEventArt>();
                    int Event_iEntity = DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt();
                    string Event_sReg = DataModul.DB_EventTable.Fields[EventFields.Reg].AsString();

                    if (Event_eArt >= EEventArt.eA_500) // Is Family-Event
                    {
                        DataModul.Link.ReadFamily(Event_iEntity, Modul1.Family);
                        int persInArb = 0;
                        if (Modul1.Family.Mann > 0)
                        {
                            persInArb = Modul1.Family.Mann;
                        }
                        else
                        {
                            persInArb = Modul1.Family.Frau;
                        }
                        M1_Iter++;
                        List6_Items.Add(DataToPFListItem(Event_sReg, persInArb, Event_iEntity));
                    }

                    DataModul.DB_EventTable.MoveNext();

                }
            }
            else
            {
                var eArt = View_SelOption2EventArt();
                if (eArt < EEventArt.eA_500)
                {
                    List6_Items.Clear();
                    DataModul.DB_EventTable.Index = nameof(EventIndex.Reg1);
                    DataModul.DB_EventTable.Seek(">=", (int)eArt, Text1_Text);
                    if ((int)eArt != 0)
                    {
                        M1_Iter = 0;
                        while (!(M1_Iter >= Modul1.Aus[13].AsInt())
                            && !DataModul.DB_EventTable.EOF
                            && !DataModul.DB_EventTable.NoMatch
                            && DataModul.DB_EventTable.Fields[EventFields.Art].AsInt() == (int)eArt
                            && string.Compare(DataModul.DB_EventTable.Fields[EventFields.Reg].AsString(), Text1_Text.ToUpper()) >= 0)
                        {
                            string sReg = DataModul.DB_EventTable.Fields[EventFields.Reg].AsString();
                            int iEntity = DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt();
                            List6_Items.Add(DataToPFListItem(sReg, iEntity, 0));

                            M1_Iter++;
                            DataModul.DB_EventTable.MoveNext();
                        }
                    }
                }
                else
                {
                    List6_Items.Clear();
                    IRecordset dB_EventTable = DataModul.DB_EventTable;
                    dB_EventTable.Index = nameof(EventIndex.Reg1);
                    dB_EventTable.Seek(">=", (int)eArt, Text1_Text);
                    M1_Iter = 0;
                    while (M1_Iter < Modul1.Aus[13].AsInt()
                        && !dB_EventTable.EOF
                        && !dB_EventTable.NoMatch
                        && dB_EventTable.Fields[EventFields.Art].AsEnum<EEventArt>() == eArt
                        && string.Compare(dB_EventTable.Fields[EventFields.Reg].AsString(), Text1_Text.ToUpper()) >= 0
                        )
                    {
                        Modul1.FamInArb = dB_EventTable.Fields[EventFields.PerFamNr].AsInt();
                        DataModul.Link.ReadFamily(Modul1.FamInArb, Modul1.Family);
                        if (Modul1.Family.Mann > 0)
                        {
                            Modul1.PersInArb = Modul1.Family.Mann;
                        }
                        else
                        {
                            Modul1.PersInArb = Modul1.Family.Frau;
                        }
                        M1_Iter++;
                        List6_Items.Add(DataToPFListItem(dB_EventTable.Fields[EventFields.Reg].AsString(), Modul1.FamInArb, Modul1.PersInArb));
                        dB_EventTable.MoveNext();
                    }
                }
            }
        }

        Zeigfam(List6_Items, LB1_Items);

    end_IL_0001_2:
        return;
    }

    private EEventArt View_SelOption2EventArt()
    {
        // Computes which (other) radio button is selected and returns the corresponding event code.
        EEventArt eArt = (View) switch
        {
            _ when View._Option1_0.Checked => EEventArt.eA_105,
            _ when View._Option1_1.Checked => EEventArt.eA_100,
            _ when View._Option1_2.Checked => EEventArt.eA_Burial,
            _ when View._Option1_3.Checked => EEventArt.eA_Death,
            _ when View._Option1_4.Checked => EEventArt.eA_Baptism,
            _ when View._Option1_5.Checked => EEventArt.eA_Birth,
            _ when View._Option1_6.Checked => EEventArt.eA_500,
            _ when View._Option1_7.Checked => EEventArt.eA_501,
            _ when View._Option1_8.Checked => EEventArt.eA_603,
            _ when View._Option1_9.Checked => EEventArt.eA_900,
            _ when View._Option1_10.Checked => EEventArt.eA_Marriage,
            _ when View._Option1_11.Checked => EEventArt.eA_MarrReligious,
            _ when View._Option1_12.Checked => EEventArt.eA_504,
            _ when View._Option1_13.Checked => EEventArt.eA_507,
            _ => EEventArt.eA_Unknown
        };
        return eArt;
    }

    private static ListItem<PersonFamily> DataToPFListItem(string Event_sReg, int persInArb, int Event_iEntity)
    {
        return new($"{Event_sReg,20}{persInArb,10}{Event_iEntity,10}", new(persInArb, Event_iEntity));
    }

    private static void DataModul_DSB_SearchTbl_EditiKennRaw(string sKenn)
    {
        DataModul.DSB_SearchTable.Edit();
        DataModul.DSB_SearchTable.Fields["iKenn"].Value = sKenn;
        DataModul.DSB_SearchTable.Update();
    }

    public (string, string) Datr(int persInArb)
    {
        var (sDat_Birth, sDat_Death) = Modul1.Datles(persInArb, Modul1.Person, true);
        sDat_Death = $"{Modul1.sDeathMark}{Modul1.Person.dDeath:yyyy}";
        sDat_Birth = $"{Modul1.sBirthMark}{Modul1.Person.dBirth:yyyy}";
        return (sDat_Birth, sDat_Death);

    }

    public void Zeig()
    {
        //Discarded unreachable code: IL_0677
        int try0001_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        string text = default;
        int lErl = default;
        string text2 = default;
        string text3 = default;
        int num5 = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int M1_Iter = default;
                    int num4;
                    int i;
                    int num6;
                    switch (try0001_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0009;
                        case 2076:
                            {
                                num2 = num;
                                switch ((num3 <= -2) ? 1 : num3)
                                {
                                    case 2:
                                    case 3:
                                        break;
                                    case 1:
                                        goto IL_0702;
                                    default:
                                        goto end_IL_0001;
                                }
                                lErl = 94;
                                if (Information.Err().Number == 94)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_0702;
                                }
                                else
                                {
                                    if (Interaction.MsgBox(Conversions.ErrorToString(), title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                                    {
                                        ProjectData.EndApp();
                                    }
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    num4 = num2;
                                    goto IL_0706;
                                }
                            }
                        end_IL_0001:
                            break;
                        IL_0009:
                            num = 2;
                            text3 = ((" ") + (DataModul.DSB_SearchTable.Fields["iKenn"].Value)).AsString();
                            text2 = text3 + "      " + DataModul.DSB_SearchTable.Fields["Datum"].AsString().Right(4) + " ";
                            if (text2.Right(4).AsDouble() == 0.0)
                            {
                                text2 = "       ";
                            }
                            if (Strings.Mid(Modul1.Mandant, 16, 10) == "GPDATENBAN")
                            {
                                Modul1.PersInArb = DataModul.DSB_SearchTable.Fields["Nummer"].AsInt();
                                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                _ = View.ListBox1.Items.Add(((DataModul.DSB_SearchTable.Fields["Name"].Value) + ("                                                ")).AsString().Left(30) + text2 + "           " + DataModul.DSB_SearchTable.Fields["Nummer"].AsString().Right(10) + "        " + Modul1.Person.Clan);
                            }
                            else
                            {
                                text = Strings.Left(((DataModul.DSB_SearchTable.Fields["Name"].Value) + (new string(' ', 40))).AsString(), 40);
                                W = Strings.InStr(text, ",");
                                if (W > 25f)
                                {
                                    text = text.Left(25) + Strings.Mid(text, W, text.Length);
                                }
                                _ = View.ListBox1.Items.Add(text + new string(' ', 40).Left(40) + text2 + "           " + DataModul.DSB_SearchTable.Fields["Nummer"].AsString().Right(10));
                            }
                            ProjectData.ClearProjectError();
                            num3 = 3;
                            if (Modul1.Aus[12] == "")
                            {
                                Modul1.Aus[12] = "200";
                            }
                            num5 = Modul1.Aus[12].AsInt();
                            M1_Iter = 1;
                            goto IL_0661;
                        IL_0661:
                            i = M1_Iter;
                            num6 = num5;
                            if (i > num6)
                            {
                                goto end_IL_0001_2;
                            }
                            if (DataModul.DSB_SearchTable.EOF)
                            {
                                goto end_IL_0001_2;
                            }
                            DataModul.DSB_SearchTable.MoveNext();
                            if (DataModul.DSB_SearchTable.EOF)
                            {
                                goto end_IL_0001_2;
                            }
                            if (DataModul.DSB_SearchTable.Fields["iKenn"].AsString() == "9")
                            {
                                DataModul.DSB_SearchTable.Edit();
                                DataModul.DSB_SearchTable.Fields["iKenn"].Value = " ";
                                DataModul.DSB_SearchTable.Update();
                            }
                            text3 = ((" ") + (DataModul.DSB_SearchTable.Fields["iKenn"].Value)).AsString();
                            text2 = text3 + "      " + DataModul.DSB_SearchTable.Fields["Datum"].AsString().Right(4) + " ";
                            if (Strings.Mid(text2, 4).AsDouble() == 0.0)
                            {
                                text2 = "       ";
                            }
                            if (Strings.Mid(Modul1.Mandant, 16, 10) == "GPDATENBAN")
                            {
                                Modul1.PersInArb = DataModul.DSB_SearchTable.Fields["Nummer"].AsInt();
                                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                _ = View.ListBox1.Items.Add(((DataModul.DSB_SearchTable.Fields["Name"].Value) + ("                                                ")).AsString().Left(30) + text2 + "           " + DataModul.DSB_SearchTable.Fields["Nummer"].Value.AsString().Right(10) + "        " + Modul1.Person.Clan);
                            }
                            else
                            {
                                text = Strings.Left(((DataModul.DSB_SearchTable.Fields["Name"].Value) + (new string(' ', 40))).AsString(), 40);
                                W = Strings.InStr(text, ",");
                                if (W > 25f)
                                {
                                    text = text.Left(25) + Strings.Mid(text, W, text.Length);
                                }
                                _ = View.ListBox1.Items.Add(text + new string(' ', 40).Left(40) + text2 + "           " + DataModul.DSB_SearchTable.Fields["Nummer"].AsString().Right(10));
                            }
                            M1_Iter++;
                            goto IL_0661;
                        IL_0702:
                            num4 = unchecked(num2 + 1);
                            goto IL_0706;
                        IL_0706:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 26:
                                case 30:
                                case 56:
                                case 66:
                                    goto end_IL_0001_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 2076;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001_2: // <========== 4
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }


    public void Zeigfam(IList<ListItem<PersonFamily>> items, IList LB1_Items)
    {
        int try0001_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int lErl = default;
        int num4;
        ProjectData.ClearProjectError();
        num3 = 2;
        num = 2;
        if (items.Count == 0)
        {
            _ = LB1_Items.Add("------------ Keine passenden Einträge-----------");
            goto end_IL_0001_2;
        }
        else
        {
            foreach (var Item in items)
            {
                int iPersNr = Item.ItemData.iPerson;

                var (sDat_Birth, sDat_Death) = Modul1.Datles(iPersNr, Modul1.Person);
                string text2 = "       ";
                if (sDat_Death.Trim() != "")
                {
                    text2 = " + " + sDat_Death.Trim().Right(4);
                }
                if (sDat_Birth.Trim() != "")
                {
                    text2 = " * " + sDat_Birth.Trim().Right(4);
                }
                Modul1.Person_ReadNames(iPersNr, Modul1.Person);
                if (Modul1.Person.Prefix.Trim() != "")
                {
                    Modul1.Person.SetFullSurname(Modul1.Person.Prefix.Trim() + " " + Modul1.Person.SurName);
                }
                string text = Modul1.Person.FullSurName.TrimEnd() + "," + Modul1.Person.Givennames;
                IList<int> aiFams;
                int Fam;
                ELinkKennz Kenn;
                int num5;
                if (Item.ItemData.iFamily > 0)
                {
                    Fam = Item.ItemData.iFamily;
                    var Persex = DataModul.Person.GetSex(iPersNr);
                    aiFams = Ehesuch(iPersNr);
                    Kenn = Persex == "F" ? ELinkKennz.lkFather : ELinkKennz.lkMother;
                    Modul1_LiText = Modul1.Famzeig(Fam, Kenn);
                    W = Strings.InStr(text, ",");
                    if (W > 25f)
                    {
                        text = text.Left(25) + Strings.Mid(text, W, text.Length);
                    }
                    num5 = Strings.Mid(List6_Text, 20, 10).AsInt();
                    _ = LB1_Items.Add(Strings.Left(List6_Text, 19) + text + new string(' ', 40).Left(40) + text2 + "              " + num5.AsString().Right(10) + Modul1_LiText + Fam.AsString());
                    S++;
                    Modul1_LiText = "";
                }
                else
                {
                    iPersNr = Item.ItemData<PersonFamily>().iPerson;
                    var Persex = DataModul.Person.GetSex(iPersNr);
                    aiFams = Ehesuch(iPersNr);
                    Kenn = Persex == "F" ? ELinkKennz.lkFather : ELinkKennz.lkMother;
                    if (aiFams.Count != 0)
                    {
                        foreach (var iFam in aiFams)
                        {
                            Fam = iFam;
                            Modul1_LiText = Modul1.Famzeig(Fam, Kenn);
                            W = Strings.InStr(text, ",");
                            if (W > 25f)
                            {
                                text = text.Left(25) + Strings.Mid(text, W, text.Length);
                            }
                            num5 = Strings.Mid(List6_Text, 20, 10).AsInt();
                            _ = LB1_Items.Add(new ListItem(Strings.Left(List6_Text, 19) + text + new string(' ', 40).Left(40) + text2 + "              " + num5.AsString().Right(10) + Modul1_LiText + Fam.AsString(), Fam));
                            S++;
                        }
                    }
                    else
                    {
                        W = Strings.InStr(text, ",");
                        if (W > 25f)
                        {
                            text = text.Left(25) + Strings.Mid(text, W, text.Length);
                        }
                        _ = LB1_Items.Add(new ListItem(Strings.Left(List6_Text, 19) + text + new string(' ', 40).Left(40) + text2 + "              " + Strings.Mid(List6_Text, 16, 10).Right(10) + Modul1_LiText, 0));
                        S++;
                        Modul1_LiText = "";
                    }
                }
            }
        }
        lErl = 2;
        _ = LB1_Items.Add("------------ Ende der Liste-----------");
        goto end_IL_0001_2;
    end_IL_0001_2: // <========== 4
        return;
    }

    [RelayCommand]
    public void Label70_DoubleClick(object tag)
    {
        //Discarded unreachable code: IL_0509
        Modul1.PersInArb = tag.AsInt();
        if (Modul1.Suchschalt != 10
            && Modul1.PersInArb > 0)
        {
            if (Modul1.Suchschalt == 1)
            {
                Familie.Default.Hide();
                Personen.Default.Close();
                Personen.Default.Show(Modul1.PersInArb, EUserText.t158);
                Modul1.Ubg = Modul1.PersInArb;
                Modul1.Suchschalt = 0;
            }
            else
            {
                Modul1.SuchPer = Modul1.PersInArb;
                if (View.ACheck2[3].CheckState == CheckState.Checked)
                {
                    Suchspeich();
                    View.Hide();
                }
            }
        }
    }
    private string Kindfamsuch(int persInArb)
    {

        Modul1.eLKennz = DataModul.Person.GetSex(persInArb) == "F" ? ELinkKennz.lkMother : ELinkKennz.lkFather;
        var aiFams = Modul1.Link_Famsuch(persInArb, Modul1.eLKennz);
        string ubgT = "";
        if (aiFams.Count > 0)
        {
            if (aiFams.Count > 1)
            {
                ubgT = "mehrere Ehen";
            }
            else
            {
                Modul1.FamInArb = aiFams.FirstOrDefault();
                Famc = Modul1.FamInArb;
                Modul1.Family.Mann = 0;
                Modul1.Family.Frau = 0;
                DataModul.Link.ReadFamily(Modul1.FamInArb, Modul1.Family);
                persInArb = Modul1.eLKennz == ELinkKennz.lkFather ? Modul1.Family.Frau : Modul1.Family.Mann;
                Perc = persInArb;
                if (persInArb > 0)
                {
                    Modul1.Person_ReadNames(persInArb, Modul1.Person);
                    ubgT = Modul1.Person.Givennames + " " + Modul1.Person.SurName;
                }
                else
                {
                    ubgT = " unbekannter Partner";
                }
            }
        }
        else
            ubgT = "Keine Ehe";
        return ubgT;
    }

    [RelayCommand]
    public void Label9_DoubleClick(object oTag)
    {
        //Discarded unreachable code: IL_02a7
        int try0001_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        switch (try0001_dispatch)
        {
            default:
                num = 1;
                goto IL_0011;
            case 963:
                {
                    num2 = num;
                    switch ((num3 <= -2) ? 1 : num3)
                    {
                        case 1:
                            break;
                        default:
                            goto end_IL_0001;
                    }
                    int num4 = num2 + 1;
                    while (true)
                    {
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 40:
                                goto end_IL_0001_2;
                            case 43:
                                num = 43;
                                if (Interaction.MsgBox(Conversions.ErrorToString(), title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                goto case 44;
                            case 44:
                            case 46:
                                num = 46;
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                                continue;
                            case 29:
                            case 41:
                            case 42:
                            case 47:
                                goto end_IL_0001_3;
                        }
                        break;
                    }
                    goto default;
                }
            end_IL_0001_2:
                break;
            IL_0011:
                num = 4;
                Modul1.Ubg = oTag.AsInt();

                if (Modul1.Suchschalt == 0)
                {
                    goto end_IL_0001_3;
                }
                Modul1.Schalt = 0;
                if (Modul1.Ubg <= 0)
                {
                    goto end_IL_0001_3;
                }
                if (View.ACheck2[3].Checked)
                {
                    Suchspeich();
                    View.Hide();
                }
                Personen.Default.Hide();
                Familie.Default.Show();
                Modul1.FamInArb = Modul1.Ubg;
                break;
        }
        num = 40;
        short Rich = 3;
        Familie.Default.Show(Modul1.FamInArb);

    end_IL_0001:
        ;
    end_IL_0001_3: // <========== 3
        return;
    }

    public void Combo1_KeyUp(object sender, KeyEventArgs e)
    {
        //Discarded unreachable code: IL_003a
        int try0001_dispatch = -1;
        int num = default;
        Keys num5 = default;
        int num2 = default;
        int num3 = default;
        switch (try0001_dispatch)
        {
            default:
                num = 1;
                num5 = e.KeyCode;
                goto IL_000b;
            case 195:
                {
                    num2 = num;
                    switch ((num3 <= -2) ? 1 : num3)
                    {
                        case 1:
                            break;
                        default:
                            goto end_IL_0001;
                    }
                    int num4 = num2 + 1;
                    while (true)
                    {
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 2:
                                goto IL_000b;
                            case 3:
                                goto IL_0013;
                            case 4:
                                goto end_IL_0001_2;
                            case 7:
                                num = 7;
                                if (Interaction.MsgBox(Conversions.ErrorToString(), title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                goto case 8;
                            case 8:
                            case 10:
                                num = 10;
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                                continue;
                            default:
                                goto end_IL_0001;
                            case 5:
                            case 6:
                            case 11:
                                goto end_IL_0001_3;
                        }
                        break;
                    }
                    goto default;
                }
            IL_0013:
                num = 3;
                if (num5 != Keys.Enter)
                {
                    goto end_IL_0001_3;
                }
                break;
            IL_000b:
                num = 2;
                Modul1_EingCode = num5;
                goto IL_0013;
            end_IL_0001_2:
                break;
        }
        num = 4;
        View.ACommand1[3].PerformClick();

    end_IL_0001:
        ;
    end_IL_0001_3:
        return;
    }

    private void Suchspeich()
    {
        if (Modul1.Typ != DriveType.CDRom)
        {
            Modul1.Persistence.WriteStringsMand("RSUCH.DAT", View.Combo1.Items.Cast<string>().ToList());
            Modul1.Persistence.WriteEnumMand("FRSUCH.DAT", View_SelOption2EventArt());
        }
    }
    public void View_Baumleer()
    {
        View.Label28.Text = "";
        View.lblChld01Info.Text = "";
        View.lblChld02Info.Text = "";
        View.lblChld03Info.Text = "";
        View.lblChld04Info.Text = "";
        View.lblChld05Info.Text = "";
        View.lblChild01.Text = "";
        View.lblChild02.Text = "";
        View.lblChild03.Text = "";
        View.lblChild04.Text = "";
        View.lblChild05.Text = "";
        View.Label69.Text = "";
        View.Label70.Text = "";
        View.Label71.Text = "";
        View.Label72.Text = "";
        View.Label74.Text = "";
        View.Label75.Text = "";
        View.lblChld01Info.Tag = 0;
        View.lblChld02Info.Tag = 0;
        View.lblChld03Info.Tag = 0;
        View.lblChld05Info.Tag = 0;
        View.lblChld04Info.Tag = 0;
        View.lblChild01.Tag = 0;
        View.lblChild02.Tag = 0;
        View.lblChild03.Tag = 0;
        View.lblChild04.Tag = 0;
        View.lblChild05.Tag = 0;
        View.Label69.Tag = 0;
        View.Label70.Tag = 0;
        View.Label71.Tag = 0;
        View.Label72.Tag = 0;
        View.Label74.Tag = 0;
        View.Label75.Tag = 0;
        View.Label9.Tag = 0;
        View.Label10.Tag = 0;
        View.Label11.Tag = 0;
        View.Label30.Tag = 0;
        View.Label31.Tag = 0;
        View.Label32.Tag = 0;
        View.Label33.Tag = 0;
        View.Label43.Tag = 0;
    }
    public void ListBox1_Click(object sender, EventArgs e)
    {

        var num = 2;
        var lErl = 10;
        Modul1.FamInArb = 0;
        Modul1.PersInArb = 0;
        // Todo: lstUsageList get Data directly
        Modul1.FamInArb = Strings.Mid(View.ListBox1.Text, 135, 10).AsInt();
        Modul1.PersInArb = Strings.Mid(View.ListBox1.Text, 48, 10).AsInt();
        View_UpdateTree(Modul1.FamInArb, Modul1.PersInArb);
        View_Baumleer();
        if (Modul1.FamInArb > 0)
        {
            DataModul.Link.ReadFamily(Modul1.FamInArb, Modul1.Family);
            View.Label11.Tag = Modul1.FamInArb;
            var famInArb = Modul1.FamInArb;
            var frau = Modul1.Family.Frau;
            if (Modul1.Family.Mann > 0)
            {
                View.Label70.Text = PersonLabelText(Modul1.Family.Mann);
                View.Label70.Tag = Modul1.Family.Mann;

                var aiFams = Modul1.Link_Famsuch(Modul1.Family.Mann, Modul1.eLKennz = ELinkKennz.lkChild);
                Modul1.FamInArb = aiFams.FirstOrDefault();
                View.Label9.Tag = Modul1.FamInArb;
                Modul1.Family.Mann = 0;
                frau = Modul1.Family.Frau;
                Modul1.Family.Frau = 0;
                DataModul.Link.ReadFamily(Modul1.FamInArb, Modul1.Family);
                if (Modul1.Family.Mann > 0)
                {
                    View.Label75.Text = PersonLabelText(Modul1.Family.Mann);
                    View.Label75.Tag = Modul1.Family.Mann;
                }
                if (Modul1.Family.Frau > 0)
                {
                    View.Label74.Text = PersonLabelText(Modul1.Family.Frau);
                    View.Label74.Tag = Modul1.Family.Frau;
                }
            }
            if (frau > 0)
            {
                View.Label69.Text = PersonLabelText(frau);
                View.Label69.Tag = frau;

                var aiFams = Modul1.Link_Famsuch(frau, Modul1.eLKennz = ELinkKennz.lkChild);
                Modul1.FamInArb = aiFams.FirstOrDefault();
                View.Label10.Tag = Modul1.FamInArb;

                Modul1.Family.Mann = 0;
                Modul1.Family.Frau = 0;
                DataModul.Link.ReadFamily(Modul1.FamInArb, Modul1.Family);
                if (Modul1.Family.Mann > 0)
                {
                    Modul1.PersInArb = Modul1.Family.Mann;
                    View.Label72.Text = PersonLabelText(Modul1.Family.Mann);
                    View.Label72.Tag = Modul1.Family.Mann;
                }
                if (Modul1.Family.Frau > 0)
                {
                    View.Label71.Text = PersonLabelText(Modul1.Family.Frau);
                    View.Label71.Tag = Modul1.Family.Frau;
                }
            }
            List4_Items.Clear();
            Modul1.FamInArb = famInArb;
            kindsuch(Modul1.FamInArb, List4_Items);
            if (List4_Items.Count <= 0)
            {
                goto end_IL_0001_2;
            }
            View.Label31.Tag = Famc;
            var num5 = List4_Items.Count;
            var M1_Iter = 0;
            while (M1_Iter < num5 && M1_Iter < 5)
            {
                Modul1.PersInArb = List4_Items[M1_Iter].ItemData;
                Perc = 0;
                Famc = 0;
                Modul1.UbgT = "";
                switch (M1_Iter)
                {
                    case 0:
                        SetChildLabels(Modul1.PersInArb, View.lblChild01, View.lblChld01Info, List4_Items[M1_Iter].ItemString.Left(4).AsInt());
                        break;
                    case 1:
                        SetChildLabels(Modul1.PersInArb, View.lblChild02, View.lblChld02Info, List4_Items[M1_Iter].ItemString.Left(4).AsInt());
                        break;
                    case 2:
                        SetChildLabels(Modul1.PersInArb, View.lblChild03, View.lblChld03Info, List4_Items[M1_Iter].ItemString.Left(4).AsInt());
                        break;
                    case 3:
                        SetChildLabels(Modul1.PersInArb, View.lblChild04, View.lblChld04Info, List4_Items[M1_Iter].ItemString.Left(4).AsInt());
                        break;
                    case 4:
                        SetChildLabels(Modul1.PersInArb, View.lblChild05, View.lblChld05Info, List4_Items[M1_Iter].ItemString.Left(4).AsInt());
                        break;
                    default:
                        break;
                }
                M1_Iter++;
            }
        }
        else
        {
            Person2Label(Modul1.PersInArb, View.Label70);
            var aiFams = Modul1.Link_Famsuch(Modul1.PersInArb, Modul1.eLKennz = ELinkKennz.lkChild);
            Modul1.FamInArb = aiFams.FirstOrDefault();
            View.Label9.Tag = Modul1.FamInArb;
            Modul1.Family.Mann = 0;
            var frau = Modul1.Family.Frau;
            Modul1.Family.Frau = 0;
            DataModul.Link.ReadFamily(Modul1.FamInArb, Modul1.Family);
            if (Modul1.Family.Mann > 0)
            {
                Modul1.PersInArb = Modul1.Family.Mann;
                Person2Label(Modul1.Family.Mann, View.Label75);
            }
            if (Modul1.Family.Frau > 0)
            {
                Modul1.PersInArb = Modul1.Family.Frau;
                Person2Label(Modul1.Family.Frau, View.Label74);
            }
        }
        goto end_IL_0001_2;
    end_IL_0001_2:
        return;
    }


    private void Person2Label(int persInArb, Label lbl)
    {
        lbl.Tag = persInArb;
        Modul1.Person_ReadNames(persInArb, Modul1.Person);
        if (Modul1.Person.Suffix.Trim() != "")
        {
            Modul1.Person.SetFullSurname(Modul1.Person.SurName + " " + Modul1.Person.Suffix);
        }
        if (Modul1.Person.Prefix != "")
        {
            Modul1.Person.SetFullSurname(Modul1.Person.Prefix + " " + Modul1.Person.SurName);
        }
        lbl.Text = "";
        lbl.Text = Modul1.Person.Givennames + " " + Modul1.Person.FullSurName;
        var (sDat_Birth, sDat_Death) = Datr(persInArb);
        lbl.Text = sDat_Birth + " " + lbl.Text + sDat_Death;
    }

    public void ListBox1_DoubleClick(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_0241
        int try0001_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
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
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        goto IL_0008;
                    case 863:
                        {
                            num2 = num;
                            switch ((num3 <= -2) ? 1 : num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_028f;
                                default:
                                    goto end_IL_0001;
                            }
                            if (Interaction.MsgBox(Conversions.ErrorToString(), title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                            {
                                ProjectData.EndApp();
                            }
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            num4 = num2;
                            goto IL_0292;
                        }
                    end_IL_0001:
                        break;
                    IL_0008:
                        num = 2;
                        Modul1.Suchfam = checked(Strings.Mid(View.ListBox1.Text, 135, 10).AsInt());
                        Modul1.SuchPer = checked(Strings.Mid(View.ListBox1.Text, 48, 10).AsInt());
                        Modul1.Ubg = 0;
                        Suchspeich();
                        if (Modul1.Schalt == 1)
                        {
                            if (View.ListBox1.Text.Length < 130)
                            {
                                Familie.Default.Hide();
                                Modul1.Suchschalt = 2;
                                Modul1.Schalt = 2;
                                Modul1.Suchfam = 0;
                                if (View.ACheck2[3].Checked)
                                {
                                    View.Hide();
                                }
                                if (View.ACheck2[3].CheckState == CheckState.Unchecked)
                                {
                                    View.ListBox1.Items.Clear();
                                    View.Hide();
                                }
                                Personen.Default.Show(Modul1.PersInArb, EUserText.t158);
                            }
                            else
                            {
                                if (View.ACheck2[3].Checked)
                                {
                                    View.Hide();
                                }
                                if (View.ACheck2[3].CheckState == CheckState.Unchecked)
                                {
                                    View.ListBox1.Items.Clear();
                                    View.Hide();
                                }
                                Familie.Default.Show();
                            }
                        }
                        else
                        {
                            if (View.ACheck2[3].Checked)
                            {
                                View.Hide();
                            }
                            if (View.ACheck2[3].CheckState == CheckState.Unchecked)
                            {
                                View.ListBox1.Items.Clear();
                                View.Hide();
                            }
                            Personen.Default.Show(Modul1.PersInArb, EUserText.t158);
                        }
                        goto end_IL_0001_2;
                    IL_028f:
                        num4 = num2 + 1;
                        goto IL_0292;
                    IL_0292:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0001_dispatch = 863;
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
}
