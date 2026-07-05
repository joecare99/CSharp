using Gen_FreeWin.Views;
using MVVM.ViewModel;
using System;
using System.Windows.Forms;
using BaseLib.Helper;
using GenFree;
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces.Sys;
using GenFreeWin.Views;
using GenFree.Interfaces.VB;
using System.Drawing;
using System.Diagnostics;
using GenFree.ViewModels.Interfaces;

namespace Gen_FreeWin.ViewModels;

public class PartnerRechercheViewModel : BaseViewModelCT , IPartnerRechercheViewModel
{
    IContainerControl IPartnerRechercheViewModel.View { get; set; }
    Partnerrecherche View=> (Partnerrecherche)((IPartnerRechercheViewModel)this).View;

    IModul1 Modul1 => _Modul1.Instance;
    [Obsolete]
    IProjectData ProjectData => Modul1.ProjectData;
    IVBInformation Information => Modul1.Information;
    IStrings Strings => Modul1.Strings;

    IVBConversions Conversion => Modul1.Conversions;
    IInteraction Interaction => Menue.Default;

    private float Textindex;

    private string Modul1_LiText;
    private (string, ETextKennz) Modul1_Bezeichnu;

    public void Command1_Click(object eventSender, EventArgs eventArgs)
    {
        //Discarded unreachable code: IL_1786, IL_17ed
        int try0001_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int num9 = default;
        int lErl = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int M1_Iter = default;
                    int num5;
                    int num6;
                    switch (try0001_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0009;
                        case 7127:
                            int num4;
                            {
                                num2 = num;
                                switch ((num3 <= -2) ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_17f1;
                                    default:
                                        goto end_IL_0001;
                                }
                                if (Menue.Default.MsgBox(Conversion.ErrorToString(), mb: MessageBoxButtons.OKCancel, title: Information.Err().Number.AsString()) == DialogResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                Debugger.Break();
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num2 = 0;
                                lErl = 67;
                                num4 = 0;
                                while (num4 != View.List1.Items.Count + 1
                                    && num4 != View.List1.Items.Count)
                                {
                                    View.List1.SelectedIndex = num4;
                                    if (View.List1.Text.Length < 50)
                                    {
                                        View.List1.Items.RemoveAt(num4);
                                    }
                                    else
                                    {
                                        num4++;
                                    }
                                }
                                if (View.List1.Items.Count == 0)
                                {
                                    _ = View.List1.Items.Add("Kein Eintrag gefunden");
                                }
                                lErl = 34;
                                goto end_IL_0001_2;

                            }
                        end_IL_0001:
                            break;
                        IL_0009:
                            num = 2;
                            string text = "";
                            View.List2.Visible = false;
                            View.List2.Items.Clear();
                            View.List1.Items.Clear();
                            if (View._Text1_0.Tag.AsString().Length == 0)
                            {
                                View._Text1_0.Tag = 0;
                            }
                            if (View._Text1_1.Tag.AsString().Length == 0)
                            {
                                View._Text1_1.Tag = 0;
                            }
                            if (View._Text1_2.Tag.AsString().Length == 0)
                            {
                                View._Text1_2.Tag = 0;
                            }
                            if (View._Text1_0.Tag.AsDouble() == 0.0)
                            {
                                _ = Interaction.MsgBox("Name des Mannes muss aus der Liste gewählt werden");
                                View._Text1_0.Text = View._Text1_0.Text + " ";
                                goto end_IL_0001_2;
                            }
                            if ((View._Text1_1.Tag.AsDouble() == 0.0) & (View._Text1_2.Tag.AsDouble() == 0.0))
                            {
                                _ = Interaction.MsgBox("Name der Frau muss aus der Liste gewählt werden");
                                View._Text1_1.Text = "";
                                goto end_IL_0001_2;
                            }


                            int iText = 0;
                            if (View._Text1_0.Text.Trim() != "")
                            {
                                iText = View._Text1_0.Tag.AsInt();
                            }
                            DataModul.DB_NameTable.Index = nameof(NameIndex.TxNr);
                            num4 = 0;
                            DataModul.DB_NameTable.Seek("=", iText);
                            while (num4 < 32700
                                && !DataModul.DB_NameTable.EOF
                                    && !DataModul.DB_NameTable.NoMatch)
                            {
                                int Name_iPersNr = DataModul.DB_NameTable.Fields[NameFields.PersNr].AsInt();
                                ETextKennz Name_eKennz = DataModul.DB_NameTable.Fields[NameFields.Kennz].AsEnum<ETextKennz>();
                                int Name_iText = DataModul.DB_NameTable.Fields[NameFields.Text].AsInt();

                                if (ETextKennz.tkNone != Name_eKennz)
                                {
                                    if (Name_iText > iText)
                                        break;
                                    if ((Name_eKennz == ETextKennz.tkName)
                                        && (Name_iText == iText))
                                    {
                                        num4++;
                                        _ = View.List2.Items.Add(new ListItem("                 " + Name_iPersNr.AsString().Right(10), Name_iPersNr));
                                        num6 = Name_iPersNr;
                                    }
                                }
                                DataModul.DB_NameTable.MoveNext();
                            }

                            int num7 = View.List2.Items.Count - 1;
                            M1_Iter = 0;
                            while (M1_Iter <= num7)
                            {
                                View.List2.SelectedIndex = 0;
                                Modul1.PersInArb = View.List2.Text.AsInt();
                                if (Modul1.PersInArb > 0)
                                {
                                    string left = DataModul.Person.GetSex(Modul1.PersInArb);
                                    if (left == "M")
                                    {
                                        Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                        //var sPrefix = Modul1.o01_Person.Prefix != "" ? Modul1.o01_Person.Prefix + " " : "";
                                        //var sSuffix = Modul1.o01_Person.Suffix != "" ? Modul1.o01_Person.Suffix + " " : "";
                                        var sGivennames = Modul1.Person.Givennames.Replace("\"", "");
                                        string item = $"{Modul1.PersInArb,10}  {(Modul1.Person.SurName + "," + sGivennames).Left(25),25}";
                                        _ = View.List1.Items.Add(new ListItem(item, Modul1.PersInArb));
                                    }
                                }
                                View.List2.Items.RemoveAt(0);
                                M1_Iter++;
                            }

                            short num11;
                            int num15;
                            int num18;
                            string text2;
                            if (View._Text1_1.Text.Trim() != "")
                            {
                                num9 = View._Text1_1.Tag.AsInt();
                                num11 = 0;
                                while (num11 < View.List1.Items.Count)
                                {
                                    Modul1.PersInArb = View.List1.Items[num11].ItemData<int>();
                                    var aiFam = DataModul.Link.GetPersonFams(Modul1.PersInArb, Modul1.eLKennz = ELinkKennz.lkFather);
                                    text = "";
                                    num15 = 0;
                                    while (num15 < aiFam.Count)
                                    {
                                        Modul1.FamInArb = aiFam[num15];
                                        _ = DataModul.Link.GetFamPerson(Modul1.FamInArb, ELinkKennz.lkMother, out var iFamPerson);
                                        Modul1.PersInArb = iFamPerson;
                                        Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                        DataModul.DB_TexteTable.Index = nameof(TexteIndex.STexte);
                                        DataModul.DB_TexteTable.Seek(">=", "N", Modul1.Person.SurName);
                                        num18 = DataModul.DB_TexteTable.Fields[TexteFields.TxNr].AsInt();
                                        if (num18 == num9)
                                        {
                                            DataModul.DB_EventTable.Index = nameof(EventIndex.ArtNr);
                                            text2 = "                ";
                                            M1_Iter = 500;
                                            while (M1_Iter <= 507)
                                            {
                                                DataModul.DB_EventTable.Seek("=", M1_Iter, Modul1.FamInArb.AsString(), "0");
                                                if (!DataModul.DB_EventTable.NoMatch
                                                    && (DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate() != default))
                                                {
                                                    text2 = "   oo " + Strings.Left(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString(), 4);
                                                    break;
                                                }
                                                M1_Iter++;
                                            }
                                            if (num15 > 0)
                                            {
                                                num11 = (short)(num11 + 1);
                                                text2 = text2.Left(10) + new string(' ', 8) + Modul1.PersInArb.AsString().Right(8) + " ";
                                                if (text == "")
                                                {
                                                    Modul1_LiText = $"{Modul1.PersInArb,10}{new string(' ', 17) + text2.Right(17)}{Modul1.Person.SurName},{Modul1.Person.Givennames}";
                                                    text2 = "";
                                                    text = $"{Modul1.PersInArb,10}";
                                                }
                                                else
                                                {
                                                    Modul1_LiText = text + new string(' ', 17) + text2.Right(17) + Modul1.Person.SurName + "," + Modul1.Person.Givennames;
                                                    text2 = "";
                                                }
                                                _ = View.List1.Items.Add(new ListItem(Modul1_LiText, Modul1.FamInArb));
                                            }
                                            else
                                            {
                                                text2 = text2.Left(10) + new string(' ', 8) + Modul1.PersInArb.AsString().Right(8) + " ";
                                                text = View.List1.Text;
                                                Modul1_LiText = View.List1.Text + new string(' ', 17) + text2.Right(17) + Modul1.Person.SurName + "," + Modul1.Person.Givennames;
                                                text2 = "";
                                                _ = View.List1.Items.Add(new ListItem(Modul1_LiText, Modul1.FamInArb));
                                            }
                                        }
                                        num15++;
                                    }
                                    num11 = (short)unchecked(num11 + 1);
                                }

                            }
                            if (View._Text1_2.Text.Trim() != "")
                            {
                                if (View._Text1_2.Text.Trim() != "")
                                {
                                    num9 = View._Text1_2.Tag.AsInt();
                                }

                                short num19 = (short)(View.List1.Items.Count - 1);
                                num11 = 0;
                                while (num11 <= num19)
                                {
                                    View.List1.SelectedIndex = num11;
                                    Modul1.PersInArb = View.List1.Text.AsInt();
                                    var aiFam = DataModul.Link.GetPersonFams(Modul1.PersInArb, Modul1.eLKennz = ELinkKennz.lkFather);
                                    text = "";
                                    num15 = 1;
                                    while (num15 <= aiFam.Count)
                                    {
                                        Modul1.FamInArb = aiFam[num15 - 1];
                                        if (DataModul.Link.GetFamPerson(Modul1.FamInArb, ELinkKennz.lkMother, out int Link_iPerNr))
                                        {
                                            Modul1.PersInArb = Link_iPerNr;
                                            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                            while (true)
                                            {
                                                num18 = 0;
                                                M1_Iter = 1;
                                                while (M1_Iter <= 15
                                                    && Modul1.Person.Givenname[M1_Iter] != "")
                                                {
                                                    DataModul.DB_TexteTable.Index = nameof(TexteIndex.STexte);
                                                    DataModul.DB_TexteTable.Seek(">=", "F", Modul1.Person.Givenname[M1_Iter]);
                                                    num18 = DataModul.DB_TexteTable.Fields[TexteFields.TxNr].AsInt();
                                                    if (num18 == num9)
                                                        break;
                                                    M1_Iter++;
                                                }
                                                if (num18 == num9)
                                                {
                                                    DataModul.DB_EventTable.Index = nameof(EventIndex.ArtNr);
                                                    text2 = "                ";
                                                    M1_Iter = 500;
                                                    while (M1_Iter <= 507)
                                                    {
                                                        DataModul.DB_EventTable.Seek("=", M1_Iter, Modul1.FamInArb.AsString(), "0");
                                                        if (!DataModul.DB_EventTable.NoMatch)
                                                        {
                                                            if (DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate() != default)
                                                            {
                                                                text2 = " oo " + Strings.Left(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString(), 4);
                                                                break;
                                                            }
                                                        }
                                                        M1_Iter++;
                                                    }
                                                    if (num15 > 1.0)
                                                    {
                                                        num11 = (short)(num11 + 1);
                                                        text2 = text2.Left(10) + new string(' ', 8) + Modul1.PersInArb.AsString().Right(8) + " ";
                                                        if (text == "")
                                                        {
                                                            Modul1_LiText = View.List1.Text + new string(' ', 17) + text2.Right(17) + Modul1.Person.SurName + "," + Modul1.Person.Givennames;
                                                            text2 = "";
                                                            text = View.List1.Text;
                                                        }
                                                        else
                                                        {
                                                            Modul1_LiText = text + new string(' ', 17) + text2.Right(17) + Modul1.Person.SurName + "," + Modul1.Person.Givennames;
                                                            text2 = "";
                                                        }
                                                        _ = View.List1.Items.Add(new ListItem(Modul1_LiText, Modul1.FamInArb));
                                                    }
                                                    else
                                                    {
                                                        text2 = text2.Left(10) + new string(' ', 8) + Modul1.PersInArb.AsString().Right(8) + " ";
                                                        text = View.List1.Text;
                                                        Modul1_LiText = View.List1.Text + new string(' ', 17) + text2.Right(17) + Modul1.Person.SurName + "," + Modul1.Person.Givennames;
                                                        text2 = "";
                                                        _ = View.List1.Items.Add(new ListItem(Modul1_LiText, Modul1.FamInArb));
                                                    }
                                                }
                                                lErl = 2;
                                                num15 += 1;
                                                while (num15 <= (double)aiFam.Count)
                                                {
                                                    Modul1.FamInArb = (int)Math.Round(Strings.Mid(Modul1.UbgT, (int)Math.Round(num15 * 10.0 - 9.0), 10).AsDouble());
                                                    if (DataModul.Link.GetFamPerson(Modul1.FamInArb, ELinkKennz.lkMother, out Link_iPerNr))
                                                        break;

                                                    Modul1.PersInArb = 0;
                                                    lErl = 2;
                                                    num15 += 1;

                                                }
                                                if (!DataModul.Link.GetFamPerson(Modul1.FamInArb, ELinkKennz.lkMother, out Link_iPerNr))
                                                {
                                                    num11 = (short)unchecked(num11 + 1);
                                                    break;
                                                }
                                                Modul1.PersInArb = Link_iPerNr;
                                                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                            }
                                            num11 = num19;
                                            break;
                                        }
                                        else
                                        {
                                            Modul1.PersInArb = 0;
                                        }
                                        lErl = 2;
                                        num15 += 1;
                                    }

                                    num11 = (short)unchecked(num11 + 1);
                                }
                            }
                            lErl = 67;
                            num4 = 0;
                            while (num4 != View.List1.Items.Count + 1
                                && num4 != View.List1.Items.Count)
                            {
                                View.List1.SelectedIndex = num4;
                                if (View.List1.Text.Length < 50)
                                {
                                    View.List1.Items.RemoveAt(num4);
                                }
                                else
                                {
                                    num4++;
                                }
                            }
                            if (View.List1.Items.Count == 0)
                            {
                                _ = View.List1.Items.Add("Kein Eintrag gefunden");
                            }
                            lErl = 34;
                            goto end_IL_0001_2;

                        IL_17f1:
                            num5 = unchecked(num2 + 1);
                            num2 = 0;
                            switch (num5)
                            {
                                case 1:
                                    break;
                                case 18:
                                case 23:
                                case 238:
                                case 245:
                                    goto end_IL_0001_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 7127;
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


    public void Command2_Click(object eventSender, EventArgs eventArgs)
    {
        Modul1.UbgT = View._Text1_0.Text;
        Modul1.Ubg = View._Text1_0.Tag.AsInt();
        Modul1.UbgT1 = View._Text1_1.Text;
        int num = View._Text1_1.Tag.AsInt();
        View._Text1_0.Tag = num;
        View._Text1_0.Text = Modul1.UbgT1;
        Modul1.UbgT1 = "";
        View._Text1_1.Tag = Modul1.Ubg;
        View._Text1_1.Text = Modul1.UbgT;
        View.List2.Visible = false;
        View.List1.Items.Clear();
    }

    public void Command3_Click(object eventSender, EventArgs eventArgs)
    {
        checked
        {
            if (View.CheckBox1.CheckState == CheckState.Unchecked)
            {
                View.List1.Items.Clear();
                int M1_Iter = 0;
                int i;
                int num;
                do
                {
                    View.Text1[(short)M1_Iter].Text = "";
                    View.Text1[(short)M1_Iter].Tag = "";
                    M1_Iter++;
                    i = M1_Iter;
                    num = 2;
                }
                while (i <= num);
            }
            View.Close();
        }
    }

    public void Partnerrecherche_Load(object eventSender, EventArgs eventArgs)
    {
        if (Modul1.FontSize > 0f)
        {
            View.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.List1.Font = new Font("courier new", Modul1.FontSize, FontStyle.Regular);
            View.List2.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View._Label2_0.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View._Label2_1.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.Label1.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.Command1.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.Command2.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.Command3.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.CheckBox1.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
        }
        Modul1.Persistence.ReadEnumInit<FormWindowState>("Windowstate", out var WiS);
        View.WindowState = WiS;
        View.DesktopLocation = Familie.Default.DesktopLocation;
        View.BackColor = Modul1.HintFarb;
        _ = View._Text1_0.Focus();
    }

    public void Partnerrecherche_Resize(object eventSender, EventArgs eventArgs)
    {
        _ = View._Text1_0.Focus();
    }

    public void List1_DoubleClick(object eventSender, EventArgs eventArgs)
    {
        Modul1.Suchfam = View.List1.SelectedItem.ItemData<int>();
        checked
        {
            if (View.CheckBox1.CheckState == CheckState.Unchecked)
            {
                View.List1.Items.Clear();
                int M1_Iter = 0;
                int i;
                int num;
                do
                {
                    View.Text1[(short)M1_Iter].Text = "";
                    View.Text1[(short)M1_Iter].Tag = "";
                    M1_Iter++;
                    i = M1_Iter;
                    num = 2;
                }
                while (i <= num);
            }
            View.Close();
        }
    }

    public void List2_DoubleClick(object eventSender, EventArgs eventArgs)
    {
        if (View._Text1_1.Text.Trim() == "")
        {
            View.Command2.Visible = false;
        }
        if (View._Text1_1.Text.Trim() != "")
        {
            View.Command2.Visible = true;
        }
        View.List1.Items.Clear();
        float textindex = Textindex;
        if (textindex == 0f)
        {
            View._Text1_0.Tag = Strings.Mid(View.List2.Text, 241, 10).AsInt();
            View._Text1_0.Text = View.List2.Text.Left(240);
            _ = View._Text1_1.Focus();
        }
        else if (textindex == 1f)
        {
            View._Text1_1.Tag = Strings.Mid(View.List2.Text, 241, 10).AsInt();
            View._Text1_1.Text = View.List2.Text.Left(240);
        }
        else if (textindex == 2f)
        {
            View._Text1_2.Tag = Strings.Mid(View.List2.Text, 241, 10).AsInt();
            View._Text1_2.Text = View.List2.Text.Left(240);
        }
        View.List2.Visible = false;
    }

    public void Text1_TextChanged(object eventSender, EventArgs eventArgs)
    {
        short index = (short)View.Text1.GetIndex((TextBox)eventSender);
        Textindex = index;
        View.List2.Items.Clear();
        View.List2.Visible = true;
        float textindex = Textindex;
        if (textindex == 0f)
        {
            Modul1.STextles("Partnerrecherche.List2", ETextKennz.tkName, View._Text1_0.Text, View.List2.Items);
        }
        else if (textindex == 1f)
        {
            Modul1.STextles("Partnerrecherche.List2", ETextKennz.tkName, View._Text1_1.Text, View.List2.Items);
        }
        else if (textindex == 2f)
        {
            Modul1.STextles("Partnerrecherche.List2", ETextKennz.F_, View._Text1_2.Text, View.List2.Items);
        }
    }

    public void Text1_KeyPress(object eventSender, KeyPressEventArgs eventArgs)
    {
        short num = checked((short)Strings.Asc(eventArgs.KeyChar));
        switch (View.Text1.GetIndex((TextBox)eventSender))
        {
            case 0:
                View._Text1_0.Tag = 0;
                break;
            case 1:
                View._Text1_2.Text = "";
                View._Text1_1.Tag = 0;
                break;
            case 2:
                View._Text1_2.Tag = 0;
                View._Text1_1.Text = "";
                break;
        }
        eventArgs.KeyChar = Strings.Chr(num);
        if (num == 0)
        {
            eventArgs.Handled = true;
        }
    }

    public void _Text1_0_TextChanged(object sender, EventArgs e)
    {
    }
}
