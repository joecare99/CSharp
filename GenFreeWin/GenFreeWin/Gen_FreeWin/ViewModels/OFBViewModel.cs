using BaseLib.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gen_FreeWin.Views;
using GenFree;
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces.Sys;
using GenFree.Interfaces.VB;
using MVVM.ViewModel;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Forms;

namespace Gen_FreeWin.ViewModels;

public partial class OFBViewModel : BaseViewModelCT, IOFBViewModel
{
    IContainerControl IOFBViewModel.View { get; set; }
    OFB View => (OFB)((IOFBViewModel)this).View;

    [ObservableProperty]
    public partial string Text1_Text { get; set; }

    [ObservableProperty]
    public partial string Text2_0_Text { get; set; }

    [ObservableProperty]
    public partial string Text2_1_Text { get; set; }

    [ObservableProperty]
    public partial string Text2_2_Text { get; set; }

    [ObservableProperty]
    public partial bool List1_Visible { get; set; }
    [ObservableProperty]
    public partial ObservableCollection<string> List1_Items { get; set; } = new();
    [ObservableProperty]
    public partial string List1_SelectedItem { get; set; }
    [ObservableProperty]
    public partial string List50_SelectedItem { get; set; }
    [ObservableProperty]
    public partial string List51_SelectedItem { get; set; }
    [ObservableProperty]
    public partial IListItem<int> List52_SelectedItem { get; set; }

    [ObservableProperty]
    public partial bool List2_Visible { get; set; }
    [ObservableProperty]
    public partial ObservableCollection<string> List2_Items { get; set; } = new();
    [ObservableProperty]
    public partial string List2_SelectedItem { get; set; }
    [ObservableProperty]
    public partial bool List3_Visible { get; set; }
    [ObservableProperty]
    public partial ObservableCollection<string> List3_Items { get; set; } = new();
    [ObservableProperty]
    public partial string List3_SelectedItem { get; set; }
    [ObservableProperty]
    public partial bool List4_Visible { get; set; }
    [ObservableProperty]
    public partial ObservableCollection<IListItem<int>> List4_Items { get; set; } = new();
    [ObservableProperty]
    public partial IListItem<int> List4_SelectedItem { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<string> List50_Items { get; set; } = new();
    [ObservableProperty]
    public partial ObservableCollection<string> List51_Items { get; set; } = new();
    [ObservableProperty]
    public partial ObservableCollection<IListItem<int>> List52_Items { get; set; } = new();

    [ObservableProperty]
    public partial bool Check1_Checked { get; set; }


    IModul1 Modul1 => _Modul1.Instance;

    public int PersInArb { get; private set; }
    public Action DoClose { get; set; }
    public Action<float> InitView { get; set; }
    public Action<string> SetFocus { get; set; } = (i) => { };

    IOperators Operators;
    IStrings Strings;

    private (string, ETextKennz) Modul1_Bezeichnu;

    [RelayCommand]
    private void Apply()
    {
        if (Modul1.Typ == DriveType.CDRom)
        {
            DoClose();
            return;
        }
        PersInArb = Personen.Default.PersonNr;

        DataModul.Person_UpdateOFB(PersInArb, Check1_Checked);

        if (Text1_Text != "")
        {
            var UbgT = Text1_Text.Trim();
            var Ubg = 0;
            if (UbgT != "")
            {
                Ubg = DataModul.Texte_Schreib(UbgT, Modul1.UbgT1, ETextKennz.tkName);
            }
            if (UbgT != "")
            {
                DataModul.Names.Update(PersInArb, Ubg, ETextKennz.Y_);
            }
        }
        else
        {
            _ = Names_Delete(PersInArb, ETextKennz.Y_);
        }

        while (DataModul.OFB.DeleteIndNr(PersInArb, "NN"))
            ;

        int M1_Iter = 0;
        while (M1_Iter <= List50_Items.Count - 1)
        {
            var UbgT = List50_Items[M1_Iter].AsString();
            OFB_FullAppend(PersInArb, "NN", DataModul.Texte_Schreib(UbgT, "", ETextKennz.tkName));
            M1_Iter++;
        }

        while (DataModul.OFB.DeleteIndNr(PersInArb, "EE"))
            ;

        M1_Iter = 0;
        while (M1_Iter <= List51_Items.Count - 1)
        {
            var UbgT = List51_Items[M1_Iter].AsString();
            OFB_Append(PersInArb, "EE", DataModul.Texte_Schreib(UbgT, "", ETextKennz.E_));
            M1_Iter++;
        }

        while (DataModul.OFB.DeleteIndNr(PersInArb, "OO"))
            ;

        M1_Iter = 0;
        while (M1_Iter <= List52_Items.Count - 1)
        {
            OFB_Append(PersInArb, "OO", List52_Items[M1_Iter].AsString().Right(10).AsInt());
            M1_Iter++;
        }

        List50_Items.Clear();
        List51_Items.Clear();
        List52_Items.Clear();

        DoClose();
    }

    private static bool Names_Delete(object persInArb, ETextKennz eTKennz)
    {
        DataModul.DB_NameTable.Index = nameof(NameIndex.NamKenn);
        DataModul.DB_NameTable.Seek("=", persInArb, eTKennz);
        bool result;
        if (result = !DataModul.DB_NameTable.NoMatch)
        {
            DataModul.DB_NameTable.Delete();
            DataModul.DB_NameTable.Index = nameof(NameIndex.PNamen);
        }
        return result;
    }

    public static void OFB_FullAppend(int persInArb, string sKennz, int iSatz)
    {
        var oFBTable = DataModul.DB_OFBTable;
        oFBTable.Index = "Indn";
        oFBTable.Seek("=", persInArb, sKennz, iSatz);
        if (oFBTable.NoMatch)
        {
            OFB_Append(persInArb, sKennz, iSatz);
        }
    }

    public static void OFB_Append(int persInArb, string sKennz, int iSatz)
    {
        var oFBTable = DataModul.DB_OFBTable;
        oFBTable.AddNew();
        oFBTable.Fields["PerNr"].Value = persInArb;
        oFBTable.Fields["Kennz"].Value = sKennz;
        oFBTable.Fields["TextNr"].Value = iSatz;
        oFBTable.Update();
    }

    public void OFB_Load(object eventSender, EventArgs eventArgs)
    {
        InitView?.Invoke(Modul1.FontSize);

        checked
        {
            PersInArb = Personen.Default.PersonNr;

            var pt = DataModul.Person.Seek(Personen.Default.PersonNr);
            Check1_Checked = pt?.Fields[PersonFields.OFB].AsString() == "J";

            DataModul.DB_NameTable.Index = nameof(NameIndex.NamKenn);
            DataModul.DB_NameTable.Seek("=", Personen.Default.PersonNr, ETextKennz.Y_);
            float num = 1f;
            if (!DataModul.DB_NameTable.NoMatch && Operators.ConditionalCompareObjectEqual(DataModul.DB_NameTable.Fields[NameFields.PersNr].AsInt(), PersInArb, TextCompare: false) && !DataModul.DB_NameTable.NoMatch)
            {
                int AAA = DataModul.DB_NameTable.Fields[NameFields.Text].AsInt();
                string bBB;
                bBB = DataModul.TextLese1(AAA);
                Text1_Text = bBB;
                List1_Visible = false;
            }

            DataModul.DB_OFBTable.Index = "InDNr";
            DataModul.DB_OFBTable.Seek("=", PersInArb, "NN");
            while (!DataModul.DB_OFBTable.EOF && !DataModul.DB_OFBTable.NoMatch
                && !(DataModul.DB_OFBTable.Fields[OFBFields.Kennz].AsString() != "NN")
                && !(DataModul.DB_OFBTable.Fields[OFBFields.PerNr].AsInt() > PersInArb))
            {
                num = 1f;
                string bBB;
                int AAA = DataModul.DB_OFBTable.Fields[OFBFields.TextNr].AsInt();
                bBB = DataModul.TextLese1(AAA);
                Modul1.Person.SetFullSurname(bBB);
                if (Modul1.Person.FullSurName != "")
                {
                    List50_Items.Add(Modul1.Person.FullSurName);
                }
                DataModul.DB_OFBTable.MoveNext();
            }

            DataModul.DB_OFBTable.Index = "InDNr";
            DataModul.DB_OFBTable.Seek("=", PersInArb, "EE");
            while (!DataModul.DB_OFBTable.EOF && !DataModul.DB_OFBTable.NoMatch
                && !(DataModul.DB_OFBTable.Fields[OFBFields.Kennz].AsString() != "EE")
                && !(DataModul.DB_OFBTable.Fields[OFBFields.PerNr].AsInt() > PersInArb))
            {
                num = 1f;
                int AAA = DataModul.DB_OFBTable.Fields[OFBFields.TextNr].AsInt();
                string bBB3;
                bBB3 = DataModul.TextLese1(AAA);
                Modul1.Person.SetFullSurname(bBB3);
                if (Modul1.Person.FullSurName != "")
                {
                    List51_Items.Add(Modul1.Person.FullSurName);
                }
                DataModul.DB_OFBTable.MoveNext();
            }

            DataModul.DB_OFBTable.Index = "InDNr";
            DataModul.DB_OFBTable.Seek("=", PersInArb, "OO");
            while (!DataModul.DB_OFBTable.EOF && !DataModul.DB_OFBTable.NoMatch
                && !(DataModul.DB_OFBTable.Fields[OFBFields.Kennz].AsString() != "OO")
                && !(DataModul.DB_OFBTable.Fields[OFBFields.PerNr].AsInt() > PersInArb))
            {
                num = 1f;
                int OFB_iPlace = DataModul.DB_OFBTable.Fields[OFBFields.TextNr].AsInt();
                if (DataModul.Place.ReadData(OFB_iPlace, out var cPlace))
                    Modul1.UbgT = Modul1.Ortles(cPlace)[0];
                List52_Items.Add(new ListItem<int>(Modul1.UbgT + new string(' ', 50) + OFB_iPlace.AsString(), OFB_iPlace));
                DataModul.DB_OFBTable.MoveNext();
            }

            Modul1.Ubg = (int)Math.Round(num);
        }
    }

    [RelayCommand]
    private void List1_DblClick()
    {
        Text1_Text = List1_SelectedItem;
        List1_Visible = false;
    }

    [RelayCommand]
    private void List2_DblClick()
    {
        List50_Items.Add(List2_SelectedItem.Trim());
        Text2_0_Text = "";
        List2_Items.Clear();
        List2_Visible = false;
        SetFocus(nameof(IOFBViewModel.Text2_0_Text));
    }

    [RelayCommand]
    private void List3_DblClick()
    {
        List51_Items.Add(List3_SelectedItem);
        Text2_1_Text = "";
        List3_Items.Clear();
        List3_Visible = false;
        SetFocus(nameof(IOFBViewModel.Text2_1_Text));
    }

    [RelayCommand]
    private void List4_DblClick()
    {
        List52_Items.Add(List4_SelectedItem);
        Text2_2_Text = "";
        List4_Items.Clear();
        List4_Visible = false;
        SetFocus(nameof(IOFBViewModel.Text2_2_Text));
    }

    [RelayCommand]
    private void List5_2_DblClick()
    {
        if (List52_Items.Count >= 1)
        {

            var Ubg = List52_SelectedItem.ItemData<int>();

            DataModul.DB_OFBTable.Index = "InDN";
            DataModul.DB_OFBTable.Seek("=", PersInArb.AsString(), "OO", Ubg);
            if (!DataModul.DB_OFBTable.NoMatch)
            {
                DataModul.DB_OFBTable.Delete();
            }

            List52_Items.Remove(List52_SelectedItem);
        }
    }

    [RelayCommand]
    private void List5_1_DblClick()
    {
        if (List51_Items.Count >= 1)
        {

            var UbgT = List51_SelectedItem;

            var iSatz = DataModul.Texte_Schreib(UbgT, "", ETextKennz.E_);

            DataModul.DB_OFBTable.Index = "InDN";
            DataModul.DB_OFBTable.Seek("=", PersInArb.AsString(), "EE", iSatz);
            if (!DataModul.DB_OFBTable.NoMatch)
            {
                DataModul.DB_OFBTable.Delete();
            }

            List51_Items.Remove(List51_SelectedItem);
        }
    }

    [RelayCommand]
    private void List5_0_DblClick()
    {
        if (List50_Items.Count >= 1)
        {
            var UbgT = List50_SelectedItem;

            int iSatz = DataModul.Texte_Schreib(UbgT, "", ETextKennz.tkName);

            DataModul.DB_OFBTable.Index = "InDN";
            DataModul.DB_OFBTable.Seek("=", PersInArb.AsString(), "NN", iSatz);
            if (!DataModul.DB_OFBTable.NoMatch)
            {
                DataModul.DB_OFBTable.Delete();
            }

            List50_Items.Remove(List50_SelectedItem);
        }
    }

    partial void OnText1_TextChanged(string value)
    {
        List1_Items.Clear();
        if (value.Trim() != "")
        {
            Modul1.STextles("OFBL1", ETextKennz.tkName, value, List1_Items);
            List1_Visible = true;
        }
    }

    [RelayCommand]
    private void Text1_KeyEnter()
    {
        List1_Visible = false;
    }

    partial void OnText2_2_TextChanged(string value)
    {
        if (value.Trim() != "")
        {
            List4_Items.Clear();
            DataModul.DOSB_OrtSTable.Index = "Ortsu";
            DataModul.DOSB_OrtSTable.Seek(">=", value);
            int num = 0;
            while (!DataModul.DOSB_OrtSTable.EOF && !DataModul.DOSB_OrtSTable.NoMatch
                && num < 200)
            {
                int iOrtNr = DataModul.DOSB_OrtSTable.Fields["Nr"].AsInt();
                string sName = DataModul.DOSB_OrtSTable.Fields["Name"].AsString();
                List4_Items.Add(new ListItem<int>(Strings.Left((sName + (new string(' ', 50))).AsString(), 50)
                    + Strings.Right("          " + iOrtNr.AsString().TrimEnd(), 10), iOrtNr));
                DataModul.DOSB_OrtSTable.MoveNext();
                num++;
            }
            List4_Visible = true;

        }
    }

    partial void OnText2_1_TextChanged(string value)
    {
        List3_Items.Clear();
        if (value.Trim() != "")
        {
            Modul1.STextles("OFBL3", ETextKennz.E_, value, List3_Items);
            List3_Visible = true;
        }
    }

    partial void OnText2_0_TextChanged(string value)
    {
        List2_Items.Clear();
        if (value.Trim() != "")
        {
            Modul1.STextles("OFBL2", ETextKennz.tkName, value, List2_Items);
            List2_Visible = true;
        }
    }

    [RelayCommand]
    private void List52_Add()
    {
        if (Text2_2_Text.Trim() != "")
        {
            DataModul_Text_SavePlacename(ETextKennz.H_, Text2_2_Text, (string sPlaceName, int iPlace) =>
            {
                DataModul_DOSB_OrtSTable_Append(sPlaceName, iPlace);
                ListAppendPlace(sPlaceName, iPlace);
            });
        }
        Text2_2_Text = "";
        List4_Visible = false;
    }

    [RelayCommand]
    private void List51_Add()
    {
        List51_Items.Add(Text2_1_Text);
        Text2_1_Text = "";
        List3_Visible = false;

    }

    [RelayCommand]
    private void List50_Add()
    {
        List50_Items.Add(Text2_0_Text);
        Text2_0_Text = "";
        List2_Visible = false;
    }

    private void DataModul_Text_SavePlacename(ETextKennz eTKennz, string sPlaceName, Action<string, int> OnSuccess)
    {
        int iSatz = DataModul.Texte_Schreib(sPlaceName, "", eTKennz);
        DataModul_Place_CreateMinimalNew(sPlaceName, iSatz, OnSuccess);
    }

    private void DataModul_Place_CreateMinimalNew(string sPlaceName, int iSatz, Action<string, int> OnSuccess)
    {

        GenFree.Interfaces.DB.IRecordset dB_PlaceTable = DataModul.DB_PlaceTable;
        dB_PlaceTable.Index = nameof(PlaceIndex.Orte);
        dB_PlaceTable.Seek("=", iSatz, 0, 0, 0, 0);
        if (dB_PlaceTable.NoMatch)
        {
            dB_PlaceTable.Index = nameof(PlaceIndex.OrtNr);
            dB_PlaceTable.MoveLast();
            int num3 = dB_PlaceTable.Fields[PlaceFields.OrtNr].AsInt() + 1;
            dB_PlaceTable.AddNew();
            dB_PlaceTable.Fields[PlaceFields.Ort].Value = iSatz;
            dB_PlaceTable.Fields[PlaceFields.Ortsteil].Value = 0;
            dB_PlaceTable.Fields[PlaceFields.Kreis].Value = 0;
            dB_PlaceTable.Fields[PlaceFields.Land].Value = 0;
            dB_PlaceTable.Fields[PlaceFields.Staat].Value = 0;
            dB_PlaceTable.Fields[PlaceFields.Loc].Value = "";
            dB_PlaceTable.Fields[PlaceFields.L].Value = "";
            dB_PlaceTable.Fields[PlaceFields.B].Value = "";
            dB_PlaceTable.Fields[PlaceFields.Terr].Value = "";
            dB_PlaceTable.Fields[PlaceFields.Staatk].Value = "";
            dB_PlaceTable.Fields[PlaceFields.PLZ].Value = "0";
            dB_PlaceTable.Fields[PlaceFields.Bem].Value = " ";
            dB_PlaceTable.Fields[PlaceFields.OrtNr].Value = num3;
            dB_PlaceTable.Update();
            OnSuccess?.Invoke(sPlaceName, num3);
        }

    }

    private void ListAppendPlace(string sPlaceName, int num3)
    {
        IList items = List52_Items;
        _ = items.Add(new ListItem(sPlaceName + new string(' ', 50).Left(50) + num3.AsString(), num3));
    }

    private static void DataModul_DOSB_OrtSTable_Append(string sPlaceName, int num3)
    {
        DataModul.DOSB_OrtSTable.AddNew();
        DataModul.DOSB_OrtSTable.Fields["Name"].Value = sPlaceName.Trim();
        DataModul.DOSB_OrtSTable.Fields["Nr"].Value = num3;
        DataModul.DOSB_OrtSTable.Update();
    }
}
