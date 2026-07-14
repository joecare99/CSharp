using BaseLib.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using Gen_FreeWin.Views;
using GenFree;
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Sys;
using GenFree.Interfaces.VB;
using GenFree.ViewModels.Interfaces;
using GenFreeWin.Views;
using MVVM.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Gen_FreeWin.ViewModels;

public partial class QuellVerwViewModel : BaseViewModelCT, IQuellVerwViewModel
{
    IContainerControl IQuellVerwViewModel.View { get; set; }
    [Obsolete]
    Quellverw View => (Quellverw)((IQuellVerwViewModel)this).View;

    [ObservableProperty]
    public partial bool Frame1_Visible { get; set; }

    IModul1 Modul1 => _Modul1.Instance;

    [Obsolete]
    IProjectData ProjectData => Modul1.ProjectData;
    public IInteraction Interaction => Menue.Default;
    [Obsolete]
    IVBInformation Information => Modul1.Information;
    [Obsolete]
    IVBConversions Conversions => Modul1.Conversions;
    IVBConversions Conversion => Modul1.Conversions;
    IStrings Strings => Modul1.Strings;

    public IFrmQuellSrchViewModel frmSrch { get; }

    private int Maxsatz;
    private int AltPer;
    private string _Command1_Click_Seit;
    private int Command1_Click_Count;
    private int _Speichern_Count;

    public QuellVerwViewModel()
    {
        frmSrch = IoC.GetRequiredService<IFrmQuellSrchViewModel>();
    }

    public void Command1_Click(object eventSender, EventArgs eventArgs)
    {
        int try0001_dispatch = -1;
        int num = default;
        int index = default;
        int num2 = default;
        int num3 = default;
        bool Rück = default;
        string source = default;
        string destination = default;
        int num7 = default;
        int num8 = default;
        int num9 = default;
        int num10 = default;
        string text = default;
        int num11 = default;
        int num12 = default;
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
                    int i4;
                    int i6;
                    int num6;
                    long Satznr;
                    int Modul1_Satznr;
                    switch (try0001_dispatch)
                    {
                        default:
                            num = 1;
                            goto IL_0016;
                        case 12420:
                            {
                                num2 = num;
                                switch ((num3 <= -2) ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_296a;
                                    default:
                                        goto end_IL_0001;
                                }
                                switch (Information.Err().Number)
                                {
                                    case 53:
                                    case 91:
                                    case 75:
                                    case 340:
                                    case 3420:
                                        ProjectData.ClearProjectError();
                                        if (num2 == 0)
                                        {
                                            throw ProjectData.CreateProjectError(-2146828268);
                                        }
                                        goto IL_296a;
                                    case 3027:
                                        _ = Interaction.MsgBox(Modul1.Message_sNoChangesOnCD, title: "", icon:
                                            MessageBoxIcon.Information);
                                        goto end_IL_0001_2;
                                    case 3022:
                                        _ = Interaction.MsgBox("Die Felder >Titel< und >Zitiert als< müssen eindeutig sein. Die gemachten Eingaben widersprechen diesem Grundsatz. Bitte korrigieren Sie diese Engaben, ein Speichern ist sonst nicht möglich.");
                                        goto end_IL_0001_2;
                                    case 3021:
                                        goto end_IL_0001_2;
                                }
                                if (Interaction.MsgBox(Conversion.ErrorToString(), title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                                goto IL_296e;
                            }
                        end_IL_0001:
                            break;
                        IL_0016:
                            ProjectData.ClearProjectError();
                            index = View.ACommand1.GetIndex((Button)eventSender);
                            num3 = 2;
                            Frame1_Visible = false;
                            Rück = true;
                            if (index != 7)
                            {
                                speichern(ref Rück);
                            }
                            if (!Rück)
                            {
                                goto end_IL_0001_2;
                            }
                            switch (index)
                            {
                                case 0:
                                    Command1_0_Click();
                                    break;
                                case 1:
                                    Modul1_Satznr = Command1_1_Click(Modul1_Satznr = 0);
                                    break;
                                case 2:
                                    Command1_2_Click();
                                    break;
                                case 3:
                                    Command1_3_Click();
                                    break;
                                case 4:
                                    Command1_4_Click(out Satznr, out Modul1_Satznr);
                                    break;
                                case 5:
                                    Command1_5_Click(out Satznr, out Modul1_Satznr);
                                    break;
                                case 6:
                                    View.Close();
                                    MainProject.Forms.Quellen.Show();
                                    break;
                                case 7:
                                    M1_Iter = 1;
                                    while (M1_Iter <= 11)
                                    {
                                        View.AText1[(short)(M1_Iter - 1)].Text = Modul1.Kont[M1_Iter];
                                        M1_Iter++;
                                    }
                                    View._Command1_7.Enabled = false;
                                    break;
                                case 8:
                                    M1_Iter = 1;
                                    while (M1_Iter <= 11)
                                    {
                                        Modul1.Kont[M1_Iter] = "";
                                        M1_Iter++;
                                    }
                                    M1_Iter = 1;
                                    while (M1_Iter <= 11)
                                    {
                                        Modul1.Kont[M1_Iter] = View.AText1[(short)(M1_Iter - 1)].Text;
                                        M1_Iter++;
                                    }
                                    View._Command1_7.Enabled = true;
                                    break;
                                case 9:
                                    goto IL_0b5c;
                                case 10:
                                    goto IL_1fbf;
                                case 11:
                                    Command1_11_Click();
                                    break;
                                case 12:
                                    goto IL_21e6;
                                case 14:
                                    Command1_14_Click(out source, out destination);
                                    break;
                                default:
                                    break;
                            }
                            goto end_IL_0001_2;
                        IL_0b5c:
                            num = 141;
                            View.List2.Items.Clear();
                            View.List3.Items.Clear();
                            View.List4.Items.Clear();
                            View.List5.Items.Clear();
                            View.Frame2.Left = 3;
                            View.Frame2.Top = 0;
                            View.Frame2.Height = View.Height - 15;
                            View.Frame2.Width = View.Width - 10;
                            View.Frame2.Visible = true;
                            View.Frame2.Text = DataModul.DB_QuTable.Fields[QuFields._2].AsString();
                            View.ProgressBar1.Minimum = 0;
                            View.ProgressBar1.Maximum = 0;
                            View.List2.Visible = false;
                            M1_Iter = 1;
                            while (M1_Iter <= 3)
                            {
                                View.ProgressBar1.PerformStep();
                                Modul1.Nr = View._Label1_13.Tag.AsInt();
                                DataModul.DB_SourceLinkTable.Index = "Verw";
                                DataModul.DB_SourceLinkTable.Seek("=", Modul1.Nr, M1_Iter);
                                Monitor.Enter(Command1_Click_Count);
                                try
                                {
                                    if (Command1_Click_Count == 0)
                                    {
                                        Command1_Click_Count = 2;
                                        _Command1_Click_Seit = "";
                                    }
                                    else if (Command1_Click_Count == 2)
                                    {
                                    }
                                }
                                finally
                                {
                                    Command1_Click_Count = 1;
                                    Monitor.Exit(Command1_Click_Count);
                                }
                                while (!DataModul.DB_SourceLinkTable.EOF
                                    && !DataModul.DB_SourceLinkTable.NoMatch
                                    && DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._1].AsInt() == M1_Iter
                                    && DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._3].AsInt() == Modul1.Nr)
                                {
                                    _Command1_Click_Seit = "";
                                    if (DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._4].AsString().Trim() != "")
                                    {
                                        _Command1_Click_Seit = DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].AsString() + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._4].AsString();
                                        if (Modul1.Aus[(int)EOutCfg.o46] == "Y")
                                        {
                                            if (null == DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].Value)
                                            {
                                                _Command1_Click_Seit = "Seite: " + _Command1_Click_Seit;
                                            }
                                            else
                                            {
                                                if (DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].AsString().Trim() == "")
                                                {
                                                    _Command1_Click_Seit = "Seite: " + _Command1_Click_Seit;
                                                }
                                            }
                                        }
                                    }
                                    switch (M1_Iter)
                                    {
                                        case 1:
                                            _ = View.List3.Items.Add("A" + "          " + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._2].AsString().Right(10) + _Command1_Click_Seit);
                                            break;
                                        case 3:
                                            if (0 != DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Art].AsInt() && (DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Art].AsInt() < 500))
                                            {
                                                _ = View.List3.Items.Add("A" + "          " + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._2].AsString().Right(10) + _Command1_Click_Seit);
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    View.Label2.Text = "Fundstellen: " + View.List3.Items.Count.AsString();
                                    DataModul.DB_SourceLinkTable.MoveNext();
                                }
                                M1_Iter += 2;
                                i4 = M1_Iter;
                            }
                            View.ProgressBar1.Minimum = 0;
                            View.ProgressBar1.Maximum = 0;
                            View.ProgressBar1.Maximum = View.List3.Items.Count;
                            View.ProgressBar1.Step = 1;
                            View.List4.Items.Clear();
                            AltPer = 0;
                            num9 = View.List3.Items.Count - 1;
                            M1_Iter = 0;
                            while (M1_Iter <= num9)
                            {
                                View.ProgressBar1.PerformStep();
                                Application.DoEvents();
                                Modul1.PersInArb = 0;
                                View.List3.SelectedIndex = M1_Iter;
                                Modul1.PersInArb = (int)Math.Round(Strings.Mid(View.List3.Text, 2, 10).AsDouble());
                                if (AltPer != Modul1.PersInArb)
                                {
                                    AltPer = Modul1.PersInArb;
                                    if (Modul1.PersInArb > 0)
                                    {
                                        Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                        text = Strings.Left(Modul1.Person.SurName.Trim() + Modul1.Person.Givennames.Trim() + new string(' ', 50).Left(50) + " " + Strings.Trim(Strings.Mid(View.List3.Text, 12, View.List3.Text.Length)) + new string(' ', 200), 250) + Modul1.PersInArb.AsString();
                                        _ = View.List4.Items.Add(text);
                                        text = "";
                                    }
                                }
                                M1_Iter++;
                            }
                            View.List3.Items.Clear();
                            DataModul.DB_SourceLinkTable.Index = "Verw";
                            M1_Iter = 2;

                            goto IL_145c;
                        IL_145c: // <========== 3
                                 // <========== 3
                                 // <========== 3
                            num = 228;
                            DataModul.DB_SourceLinkTable.Seek("=", Modul1.Nr, M1_Iter);
                            goto IL_1779;
                        IL_1582:
                            num = 242;
                            _ = View.List3.Items.Add(("F" + "          " + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._2].AsString().Right(10) + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].Value + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._4].Value));
                            goto IL_171e;
                        IL_161a:
                            num = 245;
                            if (0 != DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Art].AsInt())
                            {
                                if (DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Art].AsInt() > 499)
                                {
                                    _ = View.List3.Items.Add(("F" + "          " + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._2].AsString().Right(10) + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].Value + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._4].Value));
                                }
                            }
                            goto IL_171e;
                        IL_171e: // <========== 4
                                 // <========== 4
                                 // <========== 4
                            num = 251;
                            View.Label2.Text = "Fundstellen: " + View.List3.Items.Count + View.List4.Items.Count.AsString();
                            DataModul.DB_SourceLinkTable.MoveNext();
                            goto IL_1779;
                        IL_1779: // <========== 3
                                 // <========== 3
                                 // <========== 3
                            num = 230;
                            if (!DataModul.DB_SourceLinkTable.EOF && !DataModul.DB_SourceLinkTable.NoMatch && !((DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._1].AsInt() != M1_Iter || DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._3].AsInt() != Modul1.Nr)))
                            {
                                Application.DoEvents();
                                switch (M1_Iter)
                                {
                                    case 2:
                                        goto IL_1582;
                                    case 3:
                                        goto IL_161a;
                                    default:
                                        break;
                                }
                                goto IL_171e;
                            }
                            else
                            {
                                M1_Iter++;
                                i6 = M1_Iter;
                                num6 = 3;
                                if (i6 <= num6)
                                {
                                    goto IL_145c;
                                }
                                else
                                {
                                    View.List5.Items.Clear();
                                    num10 = View.List3.Items.Count - 1;
                                    M1_Iter = 0;
                                    while (M1_Iter <= num10)
                                    {
                                        Application.DoEvents();
                                        View.List3.SelectedIndex = M1_Iter;
                                        Modul1.FamInArb = View.List3.Items[M1_Iter].ItemData<int>();
                                        if (Modul1.FamInArb > 0)
                                        {
                                            DataModul.Link.ReadFamily(Modul1.FamInArb, Modul1.Family);
                                            if (Modul1.Family.Mann > 0)
                                            {
                                                Modul1.PersInArb = Modul1.Family.Mann;
                                                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                                text = Strings.Left(Modul1.Person.SurName.Trim() + Modul1.Person.Givennames.Trim() + new string(' ', 50).Left(50)
                                                        + " " + Strings.Trim(Strings.Mid(View.List3.Text, 12, View.List3.Text.Length)) + new string(' ', 200), 200) + Modul1.FamInArb.AsString();
                                                _ = View.List5.Items.Add(new ListItem(text, (Modul1.PersInArb, Modul1.FamInArb)));
                                                text = "";
                                            }
                                            else
                                            {
                                                text = "unbekannt" + new string(' ', 200).Left(200) + Modul1.FamInArb.AsString();
                                                _ = View.List5.Items.Add(new ListItem(text, (0, Modul1.FamInArb)));
                                                text = "";
                                            }
                                        }
                                        M1_Iter++;
                                    }
                                    View.List2.Visible = false;
                                    View.ProgressBar1.Minimum = 0;
                                    View.ProgressBar1.Maximum = 0;
                                    View.ProgressBar1.Maximum = View.List4.Items.Count + View.List3.Items.Count;
                                    View.ProgressBar1.Step = 1;
                                    text = "";
                                    AltPer = 0;
                                    num11 = View.List4.Items.Count - 1;
                                    M1_Iter = 0;
                                    while (M1_Iter <= num11)
                                    {
                                        Application.DoEvents();
                                        View.ProgressBar1.PerformStep();
                                        View.List4.SelectedIndex = M1_Iter;
                                        Modul1.PersInArb = View.List4.Items[M1_Iter].ItemData<int>();
                                        if (AltPer != Modul1.PersInArb)
                                        {
                                            AltPer = Modul1.PersInArb;
                                            if (Modul1.PersInArb > 0)
                                            {
                                                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                                Modul1.Person.SetFullSurname(Modul1.BuildFullSurName(Modul1.Person));
                                                text = " " + (Modul1.Person.FullSurName + " " + Modul1.Person.Givennames).Trim() + " " + Strings.Mid(View.List4.Text, 51, 150).Trim();
                                                text = text + new string(' ', 200).Left(63) + "           " + Modul1.PersInArb.AsString().Right(10);
                                            }
                                            _ = View.List2.Items.Add(new ListItem(text, Modul1.PersInArb));
                                            text = "";
                                        }
                                        M1_Iter++;
                                    }
                                    num12 = View.List5.Items.Count - 1;
                                    M1_Iter = 0;
                                    while (M1_Iter <= num12)
                                    {
                                        Application.DoEvents();
                                        View.ProgressBar1.PerformStep();
                                        View.List5.SelectedIndex = M1_Iter;
                                        Modul1.FamInArb = View.List5.Items[M1_Iter].ItemData<int>();
                                        if (Modul1.FamInArb > 0)
                                        {
                                            DataModul.Link.ReadFamily(Modul1.FamInArb, Modul1.Family);
                                            if (Modul1.Family.Mann > 0)
                                            {
                                                Modul1.PersInArb = Modul1.Family.Mann;
                                                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                                Modul1.Person.SetFullSurname(Modul1.BuildFullSurName(Modul1.Person));
                                                text = "F" + text.TrimEnd() + View.List5.Items[M1_Iter].AsString().Right(10) + " " + Modul1.Person.Givennames + Modul1.Person.SurName + " / ";
                                            }
                                            else
                                            {
                                                text = "F" + text.TrimEnd() + View.List5.Items[M1_Iter].AsString().Right(10) + " " + Modul1.IText[EUserText.tUnknown] + " / ";
                                            }
                                            if (Modul1.Family.Frau > 0)
                                            {
                                                Modul1.PersInArb = Modul1.Family.Frau;
                                                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                                Modul1.Person.SetFullSurname(Modul1.BuildFullSurName(Modul1.Person));
                                                text = text.TrimEnd() + Modul1.Person.Givennames + Modul1.Person.FullSurName;
                                            }
                                            else
                                            {
                                                text = text.TrimEnd() + Modul1.IText[EUserText.tUnknown];
                                            }
                                            text = text + " " + Strings.Mid(View.List5.Text, 51, 150).Trim();
                                            _ = View.List2.Items.Add(text);
                                            text = "";
                                        }
                                        M1_Iter++;
                                    }
                                    View.List2.Visible = true;
                                    View.Label2.Text = "Fundstellen: " + View.List2.Items.Count.AsString();
                                }
                                goto end_IL_0001_2;
                            }
                        IL_1fbf:
                            num = 334;
                            View.Frame2.Visible = false;
                            goto end_IL_0001_2;
                        IL_21e6:
                            num = 357;
                            if (unchecked((0 - (View._Option2_0.Checked ? 1 : 0) == 0) & (0 - (View._Option2_2.Checked ? 1 : 0) == 0)))
                            {
                                View._Option2_0.Checked = true;
                            }
                            View.Frame3.Height = View.Height - 10;
                            View.Frame3.Width = View.Width - 10;
                            View.Frame3.Visible = true;
                            goto end_IL_0001_2;
                        IL_296a:
                            // <========== 6
                            num4 = unchecked(num2 + 1);
                            goto IL_296e;
                        IL_296e:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 70:
                                case 71:
                                case 77:
                                case 73:
                                case 78:
                                case 79:
                                case 82:
                                case 83:
                                case 89:
                                case 94:
                                case 100:
                                case 155:
                                case 183:
                                case 187:
                                case 188:
                                case 189:
                                case 194:
                                case 196:
                                case 197:
                                case 198:
                                case 199:
                                case 160:
                                case 161:
                                case 201:
                                case 228:
                                    goto IL_145c;
                                case 239:
                                case 243:
                                case 248:
                                case 249:
                                case 250:
                                case 251:
                                    goto IL_171e;
                                case 229:
                                case 230:
                                case 253:
                                    goto IL_1779;
                                case 9:
                                case 12:
                                case 18:
                                case 22:
                                case 41:
                                case 47:
                                case 58:
                                case 59:
                                case 60:
                                case 96:
                                case 101:
                                case 102:
                                case 103:
                                case 104:
                                case 113:
                                case 120:
                                case 124:
                                case 130:
                                case 139:
                                case 332:
                                case 335:
                                case 355:
                                case 363:
                                case 411:
                                case 412:
                                case 413:
                                case 436:
                                case 437:
                                case 441:
                                case 444:
                                case 450:
                                    goto end_IL_0001_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0001_dispatch = 12420;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001_2: // <========== 26
            // <========== 26
            // <========== 26
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    private void Command1_5_Click(out long Satznr, out int Modul1_Satznr)
    {
        Modul1_Satznr = View._Label1_13.Tag.AsInt() - 1;
        if (Modul1_Satznr <= 0)
        {
            Modul1_Satznr = 1;
        }
        Satznr = Modul1_Satznr;
        Les1(Satznr, Rich: false);
        Modul1_Satznr = (int)Satznr;
    }

    private void Command1_11_Click()
    {
        IList<string> items = View.List2.Items.Cast<string>().ToList();

        if (Modul1.Typ != DriveType.CDRom)
        {
            _Modul1.Instance.Persistence.WriteStringsMand("Temp\\View.Text2.TXT", items);
        }
        _Modul1.Instance.Persistence.WriteStringsTemp("View.Text2.TXT", items);
        Modul1.Ausdruck("\\View.Text2.txt");
    }

    private void Command1_14_Click(out string source, out string destination)
    {
        destination = Modul1.Verz + "Quell\\GEDAUS.mdb";
        bool xReplace = true;
        if (File.Exists(destination))
            xReplace = DialogResult.No != Interaction.MsgBox("Bestehende Auswahl löschen?\n Anderenfalls werden diese Daten zu der bestehenden Auswahl zugefügt.", title: "", mb: MessageBoxButtons.YesNo, icon: MessageBoxIcon.Question);
        source = Modul1.InitDir + "GedAUS.mdb";
        DataModul.OpenQuellData(destination, source, xReplace);
        ForEachFamilyPerson(View.List2.Items, DataModul_NB_Frau_AddPerson);
        DataModul.NB.Close();
        _ = Interaction.MsgBox("Gespeichert");
    }

    private void Command1_4_Click(out long Satznr, out int Modul1_Satznr)
    {
        DataModul.DB_QuTable.MoveLast();
        Maxsatz = DataModul.DB_QuTable.Fields[QuFields._1].AsInt();
        Modul1_Satznr = View._Label1_13.Tag.AsInt();
        if (Modul1_Satznr > Maxsatz)
        {
            Modul1_Satznr = Maxsatz;
        }
        Satznr = Modul1_Satznr;
        Les1(Satznr, Rich: false);
        Modul1_Satznr = (int)Satznr;
    }

    private void Command1_3_Click()
    {
        View.Frame1.Top = 3;
        View.Frame1.Left = View._Command1_12.Left;
        Frame1_Visible = true;
        _ = View.Text2.Focus();
        IList items = View.List1.Items;
        if (items.Count == 0 & (View.Text2.Text.Trim() != ""))
        {
            if (View._Option1_0.Checked)
            {
                DataModul_QuTable_ForEachGt(QuIndex.Nam, View.Text2.Text, QuFields._2, (i, s) => AddData(items, i, s));
            }
            if (View._Option1_1.Checked)
            {
                DataModul_QuTable_ForEachGt(QuIndex.Zitat, View.Text2.Text, QuFields._4, (i, s) => AddData(items, i, s));
            }
            if (View.RadioButton1.Checked)
            {
                DataModul_QuTable_ForEachGt(QuIndex.Autor, View.Text2.Text, QuFields._2, (i, s) => AddData(items, i, s));
            }
        }
    }


    private void DataModul_QuTable_ForEachGt(QuIndex aIdx, object text, QuFields eField, Action<int, string> AddData)
    {
        var dB_QuTable = DataModul.DB_QuTable;
        dB_QuTable.Index = aIdx.AsFld();
        dB_QuTable.Seek(">=", text);
        while (!dB_QuTable.EOF && !dB_QuTable.NoMatch)
        {
            int iQuRable_ID = dB_QuTable.Fields[QuFields._1].AsInt();
            string value = dB_QuTable.Fields[eField].AsString();

            AddData(iQuRable_ID, value);
            dB_QuTable.MoveNext();
        }
    }

    private static void AddData(IList items, int iId, string sText)
    {
        _ = items.Add(new ListItem<int>($"{sText,-240}{iId}", iId));
    }

    private void Command1_0_Click()
    {
        View.Close();
        Menue.Default.Hide();
        Menue.Default.Show();
    }

    private void Command1_2_Click()
    {
        long Satznr;
        int Modul1_Satznr = View._Label1_13.Tag.AsInt();
        DataModul.DB_QuTable.Index = "NR";
        if (Modul1_Satznr == 1)
        {
            _ = Interaction.MsgBox("Quelle 1 kann nicht gelöscht werden");
        }
        else
        {
            DataModul.DB_QuTable.Seek("=", Modul1_Satznr);
            if (!DataModul.DB_QuTable.NoMatch
                && DataModul.DB_QuTable.Fields[QuFields._1].AsInt() == Modul1_Satznr)
            {
                DataModul.DB_QuTable.Delete();
                Modul1_Satznr--;
                if (Modul1_Satznr <= 0)
                {
                    Modul1_Satznr = 1;
                }
                Satznr = Modul1_Satznr;
                Les1(Satznr, Rich: true);
                Modul1_Satznr = (int)Satznr;
            }
        }
    }

    private int Command1_1_Click(int Modul1_Satznr)
    {
        if (Modul1.Typ == DriveType.CDRom)
        {
            _ = Interaction.MsgBox(Modul1.Message_sNoChangesOnCD, title: "", icon: MessageBoxIcon.Information);
        }
        else
        {
            DataModul.DB_QuTable.Index = "Nr";
            DataModul.DB_QuTable.MoveLast();
            Modul1_Satznr = DataModul.DB_QuTable.Fields[QuFields._1].AsInt() + 1;
            View._Label1_13.Text = Modul1_Satznr.AsString();
            View._Label1_13.Tag = Modul1_Satznr;
            View._Text1_0.Text = "";
            View._Text1_1.Text = "";
            View._Text1_2.Text = "";
            View._Text1_3.Text = "";
            View._Text1_5.Text = "";
            View._Text1_6.Text = "";
            View._Text1_7.Text = "";
            View._Text1_8.Text = "";
            View._Text1_9.Text = "";
            View._Text1_10.Text = "";
            View.RTB1.Text = "";
            View.ComboBox1.Items.Clear();
            View.ComboBox1.Text = "";
        }

        return Modul1_Satznr;
    }


    private void ForEachFamilyPerson(IList items, Action<int> aAction)
    {
        var family = Modul1.Family;
        foreach (IListItem<int> Item in items)
        {
            var iFamInArb = Item.ItemData;
            if (Item.ItemString.Left(1) == "F")
            {
                DataModul.Link.ReadFamily(iFamInArb, family);
                if (family.Mann > 0)
                {
                    aAction(family.Mann);
                }
                if (family.Frau > 0)
                {
                    aAction(family.Frau);
                }
            }
            else
            {
                aAction(iFamInArb);
            }
        }
    }

    private void DataModul_NB_Frau_AddPerson(int iPerNr)
    {
        if (DataModul.NB_FrauTable is IRecordset nB_FrauTable)
        {
            nB_FrauTable.AddNew();
            nB_FrauTable.Fields[NB_Frau1Fields.Nr].Value = iPerNr;
            nB_FrauTable.Update();
        }
    }

    public void Command3_Click(object eventSender, EventArgs eventArgs)
    {
        //Discarded unreachable code: IL_010e, IL_0140, IL_017a
        switch (View.ACommand3.GetIndex((Button)eventSender))
        {
            case 0:
                if (Modul1.Typ == DriveType.CDRom)
                {
                    View.RTB2.SaveFile(Modul1.TempPath + "\\View.Text2.RTF", RichTextBoxStreamType.RichText);
                }
                else
                {
                    View.RTB1.SaveFile(Modul1.TempPath + "\\View.Text2.RTF", RichTextBoxStreamType.RichText);
                    View.RTB2.SaveFile(Modul1.GenFreeDir + "\\Temp\\View.Text2.RTF", RichTextBoxStreamType.RichText);
                }
                Modul1.Ausdruck("\\View.Text2.RTF");
                break;
            case 1:
                View.Frame3.Visible = false;
                View.RTB2.Text = "";
                break;
            case 2:
                View.RTB2.Text = "";
                if (0 - (View._Option2_0.Checked ? 1 : 0) == -1)
                {
                    DataModul.DB_QuTable.Index = "Nam";
                }
                else if (0 - (View._Option2_1.Checked ? 1 : 0) == -1)
                {
                    DataModul.DB_QuTable.Index = "Autor";
                }
                else
                {
                    if (0 - (View._Option2_2.Checked ? 1 : 0) != -1)
                    {
                        break;
                    }
                    DataModul.DB_QuTable.Index = "Zitat";
                }
                DataModul.DB_QuTable.MoveFirst();
                View.RTB2.Font = new Font("Arial", 11.01f, FontStyle.Regular);
                while (!DataModul.DB_QuTable.EOF)
                {
                    View.RTB2.SelectedText = "\n";
                    View.RTB2.SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                    View.RTB2.SelectionIndent = 0;
                    if (0 - (View._Option2_0.Checked ? 1 : 0) == -1)
                    {
                        View.RTB2.SelectionFont = new Font("Arial", 11.01f, FontStyle.Bold);
                    }
                    View.RTB2.SelectedText = "Titel: " + DataModul.DB_QuTable.Fields[QuFields._2].AsString() + '\n';
                    View.RTB2.SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                    View.RTB2.SelectionIndent = 30;
                    if (DataModul.DB_QuTable.Fields[QuFields._4].AsString().Trim() != "")
                    {
                        View.RTB2.SelectionFont = 0 - (View._Option2_2.Checked ? 1 : 0) == -1
                            ? new Font("Arial", 11.01f, FontStyle.Bold)
                            : new Font("Arial", 11.01f, FontStyle.Regular);
                        View.RTB2.SelectedText = ("Zitiert als: " + DataModul.DB_QuTable.Fields[QuFields._4].Value + '\n').AsString();
                    }
                    View.RTB2.SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                    if (DataModul.DB_QuTable.Fields[QuFields._5].AsString().Trim() != "")
                    {
                        if (0 - (View._Option2_1.Checked ? 1 : 0) == -1)
                        {
                            View.RTB2.SelectionFont = new Font("Arial", 11.01f, FontStyle.Bold);
                        }
                        View.RTB2.SelectedText = ("Autor: " + DataModul.DB_QuTable.Fields[QuFields._5].Value + '\n').AsString();
                    }
                    View.RTB2.SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                    string text = "";
                    if (DataModul.DB_RepoTab.RecordCount > 0)
                    {
                        DataModul.DB_RepoTab.Index = "Nr";
                        DataModul.DB_RepoTab.Seek("=", DataModul.DB_QuTable.Fields[QuFields._1].Value);
                        while (!DataModul.DB_RepoTab.NoMatch && !DataModul.DB_RepoTab.EOF)
                        {
                            DataModul.DB_RepoTable.Index = "Nr";
                            DataModul.DB_RepoTable.Seek("=", DataModul.DB_RepoTab.Fields["Repo"].Value);
                            if (DataModul.DB_RepoTable.NoMatch || (DataModul.DB_RepoTab.Fields["Quelle"].Value != DataModul.DB_QuTable.Fields[QuFields._1].Value))
                            {
                                break;
                            }
                            text = Strings.Trim(Conversions.ToString(text + " " + DataModul.DB_RepoTable.Fields[RepoFields.Name].Value + " " + DataModul.DB_RepoTable.Fields[RepoFields.Ort].Value));
                            DataModul.DB_RepoTab.MoveNext();
                        }
                    }
                    if (text.Trim() != "")
                    {
                        View.RTB2.SelectedText = "Standort: " + text + "\n";
                    }
                    View.RTB2.SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                    if (DataModul.DB_QuTable.Fields[QuFields._7].AsString().Trim() != "")
                    {
                        View.RTB2.SelectedText = ("Herausgeber: " + DataModul.DB_QuTable.Fields[QuFields._7].Value + '\n').AsString();
                    }
                    if (DataModul.DB_QuTable.Fields[QuFields._8].AsString().Trim() != "")
                    {
                        View.RTB2.SelectedText = ("Erscheinungsort: " + DataModul.DB_QuTable.Fields[QuFields._8].Value + '\n').AsString();
                    }
                    if (DataModul.DB_QuTable.Fields[QuFields._9].AsString().Trim() != "")
                    {
                        View.RTB2.SelectedText = ("Erscheinungsdatum: " + DataModul.DB_QuTable.Fields[QuFields._9].Value + '\n').AsString();
                    }
                    if (DataModul.DB_QuTable.Fields[QuFields._10].AsString().Trim() != "")
                    {
                        View.RTB2.SelectedText = ("Erschienen in: " + DataModul.DB_QuTable.Fields[QuFields._10].Value + '\n').AsString();
                    }
                    if (DataModul.DB_QuTable.Fields[QuFields._11].AsString().Trim() != "")
                    {
                        View.RTB2.SelectedText = ("Jahrgang: " + DataModul.DB_QuTable.Fields[QuFields._11].Value + '\n').AsString();
                    }
                    if (DataModul.DB_QuTable.Fields[QuFields._12].AsString().Trim() != "")
                    {
                        View.RTB2.SelectedText = ("Band: " + DataModul.DB_QuTable.Fields[QuFields._12].Value + '\n').AsString();
                    }
                    if (DataModul.DB_QuTable.Fields[QuFields._13].AsString().Trim() != "")
                    {
                        View.RTB2.SelectedText = ("Bemerkung: " + DataModul.DB_QuTable.Fields[QuFields._13].Value + '\n').AsString();
                    }
                    DataModul.DB_QuTable.MoveNext();
                }
                View.Frame3.Visible = true;
                View.RTB2.Visible = true;
                if (View.RTB2.Text != "")
                {
                    View._Command3_0.Enabled = true;
                }
                break;
        }
    }

    private void List2_DoubleClick(object eventSender, EventArgs eventArgs)
    {
        Modul1.Ad = false;
        if (!View.CheckBox1.Checked)
        {
            View.Hide();
        }
        if (View.List2.Items[View.List2.SelectedIndex].AsString().Left(1) == "F")
        {
            Modul1.FamInArb = Conversions.ToInteger(Strings.Mid(View.List2.Items[View.List2.SelectedIndex].AsString(), 2, 10));
            if (View.CheckBox1.Checked)
            {
                Familie.Default.btnMainmenue.Text = Modul1.IText[EUserText.tNMBack];
            }
            short Rich;
            Familie.Default.Show(Modul1.FamInArb);
            return;
        }
        Modul1.PersInArb = View.List2.Items[View.List2.SelectedIndex].AsString().Right(10).AsInt();
        Personen.Default.lblSearch2.Text = "";
        Modul1.Aend = 0f;
        Personen.Default.Close();
        Personen.Default.Show(Modul1.PersInArb, EUserText.tNMBack);
    }

    public void Command2_Click(object eventSender, EventArgs eventArgs)
    {
        Frame1_Visible = false;
    }

    public void Quellverw_Load(object eventSender, EventArgs eventArgs)
    {
        //Discarded unreachable code: IL_0d18
        int try0001_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
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
                    int num4;
                    switch (try0001_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0009;
                        case 4456:
                            {
                                num2 = num;
                                switch ((num3 <= -2) ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_0ece;
                                    default:
                                        goto end_IL_0001;
                                }
                                int number = Information.Err().Number;
                                if (number == 401)
                                {
                                    _ = View.ShowDialog(Modul1.Ubg);
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_0ece;
                                }
                                else if (number == 3010 || number == 3380 || number == 3375)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_0ece;
                                }
                                else if (number == 3211)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    num2 = 0;
                                    goto IL_0009;
                                }
                                else
                                {
                                    if (Interaction.MsgBox(Conversion.ErrorToString(), title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                                    {
                                        ProjectData.EndApp();
                                    }
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    num4 = num2;
                                    goto IL_0ed2;
                                }
                            }
                        end_IL_0001:
                            break;
                        IL_0009:
                            num = 2;
                            Command1_Click_Count = 0;
                            _Speichern_Count = 0;

                            View.RTB1.AddContextMenu();
                            View.RTB2.AddContextMenu();
                            if (Modul1.FontSize > 0f)
                            {
                                View.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                View._Text1_0.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                            }
                            View.RTB1.RightMargin = View.RTB1.Width - 20;
                            View._Command1_0.Text = Modul1.IText[EUserText.t158];
                            View._Command1_6.Text = Modul1.IText[EUserText.tNMBack];
                            View._Command1_10.Text = Modul1.IText[EUserText.tNMBack];
                            View._Command3_1.Text = Modul1.IText[EUserText.tNMBack];
                            View.Text = $"{Modul1.AppName} {Modul1.IText[EUserText.t246]} für Mandant: {Modul1.Mandant}";
                            Modul1.Persistence.ReadEnumInit<FormWindowState>("Windowstate", out var Modul1_WiS);
                            View.WindowState = Modul1_WiS;
                            var aiInts = Modul1.Persistence.ReadIntsInit("maspos.dat", 2);
                            View.Left = aiInts[0];
                            View.Top = aiInts[1];
                            View.BackColor = Modul1.HintFarb;
                            View.PictureBox1.BackColor = View.BackColor;
                            lErl = 1;
                            var flag = true;
                            while (flag)
                            {
                                if (Modul1.System.VerSpecial == 1)
                                {
                                    View._Command1_1.Enabled = false;
                                    View._Command1_2.Enabled = false;
                                    View._Command1_7.Enabled = false;
                                    View._Command1_8.Enabled = false;
                                }
                                MainProject.Forms.Quellen.Hide();
                                DataModul.DB_QuTable.Index = "NR";
                                if ((Modul1.Ubg == 0) | (DataModul.DB_QuTable.RecordCount == 0))
                                {
                                    Modul1.Ubg = 1;
                                }
                                DataModul.DB_QuTable.Seek("=", Modul1.Ubg);
                                if (flag = DataModul.DB_QuTable.NoMatch)
                                {
                                    var Modul1_Satznr = 1;
                                    Quelle_AppendRaw(Modul1_Satznr, "leer2", "leer2");
                                }
                            }
                            View._Label1_13.Text = DataModul.DB_QuTable.Fields[QuFields._1].AsString();
                            View._Text1_0.Text = DataModul.DB_QuTable.Fields[QuFields._2].AsString();
                            View._Text1_2.Text = DataModul.DB_QuTable.Fields[QuFields._4].AsString();
                            View._Text1_3.Text = DataModul.DB_QuTable.Fields[QuFields._5].AsString();
                            View._Text1_5.Text = DataModul.DB_QuTable.Fields[QuFields._7].AsString();
                            View._Text1_6.Text = DataModul.DB_QuTable.Fields[QuFields._8].AsString();
                            View._Text1_7.Text = DataModul.DB_QuTable.Fields[QuFields._9].AsString();
                            View._Text1_8.Text = DataModul.DB_QuTable.Fields[QuFields._10].AsString();
                            View._Text1_9.Text = DataModul.DB_QuTable.Fields[QuFields._11].AsString();
                            View._Text1_10.Text = DataModul.DB_QuTable.Fields[QuFields._12].AsString();
                            View.RTB1.Text = DataModul.DB_QuTable.Fields[QuFields._13].AsString();
                            if (DataModul.DB_RepoTab.RecordCount > 0)
                            {
                                DataModul.DB_RepoTab.Index = "Nr";
                                DataModul.DB_RepoTab.Seek("=", DataModul.DB_QuTable.Fields[QuFields._1].AsInt());
                                View.ComboBox1.BackColor = ColorTranslator.FromOle(-2147483643);
                                View.ComboBox1.Items.Clear();
                                View.ComboBox1.Text = "";
                                while (!DataModul.DB_RepoTab.NoMatch
                                    && !DataModul.DB_RepoTab.EOF)
                                {
                                    DataModul.DB_RepoTable.Index = "Nr";
                                    DataModul.DB_RepoTable.Seek("=", DataModul.DB_RepoTab.Fields["Repo"].Value);
                                    if (!DataModul.DB_RepoTable.NoMatch)
                                    {
                                        if (!(DataModul.DB_RepoTab.Fields["Quelle"].Value != DataModul.DB_QuTable.Fields[QuFields._1].Value))
                                        {
                                            ComboBox comboBox = View.ComboBox1;
                                            _ = comboBox.Items.Add(new MyListItem((DataModul.DB_RepoTable.Fields[RepoFields.Name].Value + " " + DataModul.DB_RepoTable.Fields[RepoFields.Ort].Value).AsString(), DataModul.DB_RepoTab.Fields["Repo"].AsInt()));
                                            comboBox = null;
                                        }
                                        else
                                            break;
                                    }

                                    MyListItem myListItem = (MyListItem)View.ComboBox1.Items[0];
                                    View.ComboBox1.Text = myListItem.ItemString + "\r\n";
                                    View.ComboBox1.Tag = myListItem.ItemData.ToString();
                                    if (View.ComboBox1.Items.Count > 1)
                                    {
                                        View.ComboBox1.BackColor = ColorTranslator.FromOle(12648447);
                                    }
                                    DataModul.DB_RepoTab.MoveNext();
                                }
                                if (DataModul.DB_RepoTab.EOF)
                                {
                                    goto end_IL_0001_2;
                                }
                            }

                            View.Frame2.Top = 1;
                            View.Frame2.Left = 1;
                            View.Frame3.Top = 1;
                            View.Frame3.Left = 1;
                            Bildzeig();
                            Bildliste();
                            View._Command1_2.Visible = true;
                            M1_Iter = 1;
                            while (M1_Iter <= 3)
                            {
                                Modul1.Nr = View._Label1_13.Tag.AsInt();
                                DataModul.DB_SourceLinkTable.Index = "Verw";
                                DataModul.DB_SourceLinkTable.Seek("=", Modul1.Nr, M1_Iter);
                                while (!DataModul.DB_SourceLinkTable.EOF
                                    && !DataModul.DB_SourceLinkTable.NoMatch
                                    && !((DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._1].AsInt() != M1_Iter || DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._3].AsInt() != Modul1.Nr)))
                                {
                                    switch (M1_Iter)
                                    {
                                        case 1:
                                        case 2:
                                        case 3:
                                            View._Command1_2.Visible = false;
                                            goto end_IL_0001_2;
                                        default:
                                            break;
                                    }
                                    DataModul.DB_SourceLinkTable.MoveNext();
                                }
                                M1_Iter++;
                            }
                            goto end_IL_0001_2;

                        IL_0ece: // <========== 5
                            num4 = unchecked(num2 + 1);
                            goto IL_0ed2;
                        IL_0ed2:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 131:
                                case 136:
                                case 137:
                                case 140:
                                case 141:
                                case 144:
                                case 145:
                                case 148:
                                case 149:
                                case 153:
                                case 159:
                                case 160:
                                case 161:
                                    num = 161;
                                    _ = View.ShowDialog(Modul1.Ubg);
                                    goto end_IL_0001_2;
                                case 79:
                                case 124:
                                case 129:
                                case 162:
                                    goto end_IL_0001_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 4456;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001_2: // <========== 5
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    private static void Quelle_AppendRaw(int iSatzNr, string sPar2, string sPar3)
    {
        DataModul.DB_QuTable.AddNew();
        DataModul.DB_QuTable.Fields[QuFields._1].Value = iSatzNr;
        DataModul.DB_QuTable.Fields[QuFields._2].Value = sPar2;
        DataModul.DB_QuTable.Fields[QuFields._3].Value = sPar3;
        DataModul.DB_QuTable.Fields[QuFields._4].Value = "";
        DataModul.DB_QuTable.Fields[QuFields._5].Value = "";
        DataModul.DB_QuTable.Fields[5].Value = "";
        DataModul.DB_QuTable.Fields[QuFields._7].Value = "";
        DataModul.DB_QuTable.Fields[QuFields._8].Value = "";
        DataModul.DB_QuTable.Fields[QuFields._9].Value = "";
        DataModul.DB_QuTable.Fields[QuFields._10].Value = "";
        DataModul.DB_QuTable.Fields[QuFields._11].Value = "";
        DataModul.DB_QuTable.Fields[QuFields._12].Value = "";
        DataModul.DB_QuTable.Fields[QuFields._13].Value = "";
        DataModul.DB_QuTable.Update();
    }

    public void List1_DoubleClick(object eventSender, EventArgs eventArgs)
    {
        var Modul1_Satznr = Conversions.ToInteger(Strings.Mid(View.List1.Items[View.List1.SelectedIndex].AsString(), 240, 10));
        long Satznr = Modul1_Satznr;
        Les1(Satznr, Rich: true);
        Modul1_Satznr = checked((int)Satznr);
    }

    public void Option2_CheckedChanged(object eventSender, EventArgs eventArgs)
    {
        if ((eventSender as CheckBox)?.Checked ?? false)
        {
            View.RTB2.Text = "";
        }
    }

    public void Text1_KeyDown(object eventSender, KeyEventArgs eventArgs)
    {
        short num;
        int index;
        checked
        {
            num = (short)eventArgs.KeyCode;
            short num2 = (short)unchecked((int)eventArgs.KeyData / 65536);
            index = View.AText1.GetIndex((TextBox)eventSender);
            Frame1_Visible = false;
            if (unchecked(index < 10 && num == 40))
            {
                _ = View.AText1[(short)(index + 1)].Focus();
            }
        }
        if (index == 10 && num == 40)
        {
            _ = View._Text1_0.Focus();
        }
        if (index > 0 && num == 38)
        {
            _ = View.AText1[checked((short)(index - 1))].Focus();
        }
        if (index == 0 && num == 38)
        {
            _ = View._Text1_10.Focus();
        }
    }

    public void Text2_KeyDown(object sender, KeyEventArgs e)
    {
        View.List1.Items.Clear();
        View.TextBox1.Text = "";
    }

    public void TextBox1_KeyDown(object sender, KeyEventArgs e)
    {
        View.List1.Items.Clear();
        View.Text2.Text = "";
    }


    public void Button1_Click(object sender, EventArgs e)
    {
        if (View.TextBox1.Text != "")
        {
            if (View._Option1_0.Checked)
            {
                DataModul.DB_QuTable.Index = "Nam";
                string @string = View.TextBox1.Text.ToUpper().Trim();
                DataModul.DB_QuTable.MoveFirst();
                while (!DataModul.DB_QuTable.EOF && !DataModul.DB_QuTable.NoMatch)
                {
                    object[] array = new object[1];
                    object[] array2 = array;
                    var field = DataModul.DB_QuTable.Fields[QuFields._2];
                    array2[0] = field.Value;
                    object[] array3 = array;
                    bool[] array4 = new bool[1] { true };
                    var obj = field.AsString().ToUpper();
                    if (array4[0])
                    {
                        field.Value = array3[0];
                    }
                    if (obj.Contains(@string))
                    {
                        _ = View.List1.Items.Add(
                            new ListItem(
                            DataModul.DB_QuTable.Fields[QuFields._2].AsString().PadRight(240, ' ')
                            + DataModul.DB_QuTable.Fields[QuFields._1].AsString(), DataModul.DB_QuTable.Fields[QuFields._1].Value));
                    }
                    DataModul.DB_QuTable.MoveNext();
                }
            }
            if (View._Option1_1.Checked)
            {
                DataModul.DB_QuTable.Index = "Zitat";
                string string2 = View.TextBox1.Text.ToUpper().Trim();
                DataModul.DB_QuTable.MoveFirst();
                while (!DataModul.DB_QuTable.EOF && !DataModul.DB_QuTable.NoMatch)
                {
                    object[] array3 = new object[1];
                    object[] array5 = array3;
                    var field = DataModul.DB_QuTable.Fields[QuFields._4];
                    array5[0] = field.Value;

                    var obj2 = field.AsString().ToUpper();
                    field.Value = array3[0];
                    if (obj2.Contains(string2))
                    {
                        _ = View.List1.Items.Add(
                            new ListItem(
                                DataModul.DB_QuTable.Fields[QuFields._4].AsString().PadRight(240, ' ')
                                + DataModul.DB_QuTable.Fields[QuFields._1].AsString(),
                                DataModul.DB_QuTable.Fields[QuFields._1].Value));
                    }
                    DataModul.DB_QuTable.MoveNext();
                }
            }
            if (View.RadioButton1.Checked)
            {
                DataModul.DB_QuTable.Index = "Autor";
                string string3 = View.TextBox1.Text.ToUpper().Trim();
                DataModul.DB_QuTable.MoveFirst();
                while (!DataModul.DB_QuTable.EOF && !DataModul.DB_QuTable.NoMatch)
                {
                    var field = DataModul.DB_QuTable.Fields[QuFields._5];
                    var obj3 = field.AsString().ToUpper();
                    if (obj3.Contains(string3))
                    {
                        _ = View.List1.Items.Add(new ListItem(
                                                (DataModul.DB_QuTable.Fields[QuFields._5].AsString() + "; "
                                                + DataModul.DB_QuTable.Fields[QuFields._4].AsString()).PadRight(240, ' ')
                                 + DataModul.DB_QuTable.Fields[QuFields._1].AsString(), DataModul.DB_QuTable.Fields[QuFields._1].Value));
                    }
                    DataModul.DB_QuTable.MoveNext();
                }
            }
        }
        if (View.Text2.Text == "")
        {
            return;
        }
        View.List1.Items.Clear();
        if (View._Option1_0.Checked)
        {
            DataModul.DB_QuTable.Index = "Nam";
            DataModul.DB_QuTable.Seek(">=", View.Text2.Text);
            while (!DataModul.DB_QuTable.EOF && !DataModul.DB_QuTable.NoMatch)
            {
                _ = View.List1.Items.Add(Strings.Left((DataModul.DB_QuTable.Fields[QuFields._2].Value + new string(' ', 240)).AsString(), 240) + DataModul.DB_QuTable.Fields[QuFields._1].AsString());
                DataModul.DB_QuTable.MoveNext();
            }
        }
        if (View._Option1_1.Checked)
        {
            DataModul.DB_QuTable.Index = "Zitat";
            DataModul.DB_QuTable.Seek(">=", View.Text2.Text);
            while (!DataModul.DB_QuTable.EOF && !DataModul.DB_QuTable.NoMatch)
            {
                _ = View.List1.Items.Add(Strings.Left((DataModul.DB_QuTable.Fields[QuFields._4].Value + new string(' ', 240)).AsString(), 240) + DataModul.DB_QuTable.Fields[QuFields._1].AsString());
                DataModul.DB_QuTable.MoveNext();
            }
        }
        if (View.RadioButton1.Checked)
        {
            DataModul.DB_QuTable.Index = "Autor";
            DataModul.DB_QuTable.Seek(">=", View.Text2.Text);
            while (!DataModul.DB_QuTable.EOF && !DataModul.DB_QuTable.NoMatch)
            {
                _ = View.List1.Items.Add(Strings.Left(Conversions.ToString(DataModul.DB_QuTable.Fields[QuFields._5].Value + "; " + DataModul.DB_QuTable.Fields[QuFields._4].Value + new string(' ', 240)), 240) + DataModul.DB_QuTable.Fields[QuFields._1].AsString());
                DataModul.DB_QuTable.MoveNext();
            }
        }
    }

    public void Les1(long Satznr, bool Rich)
    {
        //Discarded unreachable code: IL_08e0
        int try0001_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int lErl = default;
        int M1_Iter = default;
        int num4;
        switch (try0001_dispatch)
        {
            default:
                ProjectData.ClearProjectError();
                num3 = 2;
                goto IL_0009;
            case 2844:
                {
                    num2 = num;
                    switch ((num3 <= -2) ? 1 : num3)
                    {
                        case 2:
                            break;
                        case 1:
                            goto IL_0996;
                        default:
                            goto end_IL_0001;
                    }
                    if (Interaction.MsgBox("Fehler # " + Information.Err().Number.AsString() + " wurde ausgelöst von " + Information.Err().Source + "\r" + Information.Err().Description, title: "Fehler", mb: MessageBoxButtons.OKCancel, icon: MessageBoxIcon.Question) == DialogResult.Cancel)
                    {
                        ProjectData.EndApp();
                    }
                    ProjectData.ClearProjectError();
                    if (num2 == 0)
                    {
                        throw ProjectData.CreateProjectError(-2146828268);
                    }
                    num4 = num2;
                    goto IL_099a;
                }
            end_IL_0001:
                break;
            IL_0009:
                num = 2;
                MainProject.Forms.Bilder.Close();
                DataModul.DB_QuTable.Index = "NR";
                if (Rich)
                {
                    DataModul.DB_QuTable.Seek("=", Satznr.AsInt());
                }
                else
                {
                    DataModul.DB_QuTable.Seek("=", View._Label1_13.Tag.AsInt());
                    if (View._Label1_13.Tag.AsInt() != Satznr)
                    {
                        if (View._Label1_13.Tag.AsInt() < Satznr)
                        {
                            DataModul.DB_QuTable.MoveNext();
                        }
                        else
                        {
                            DataModul.DB_QuTable.MovePrevious();
                        }
                    }
                }
                lErl = 2;
                View.ComboBox1.Items.Clear();
                View.ComboBox1.Text = "";
                View._Label1_13.Text = DataModul.DB_QuTable.Fields[QuFields._1].AsString();
                View._Label1_13.Tag = DataModul.DB_QuTable.Fields[QuFields._1].AsInt();
                View._Text1_0.Text = DataModul.DB_QuTable.Fields[QuFields._2].AsString();
                View._Text1_2.Text = DataModul.DB_QuTable.Fields[QuFields._4].AsString();
                View._Text1_3.Text = DataModul.DB_QuTable.Fields[QuFields._5].AsString();
                View._Text1_5.Text = DataModul.DB_QuTable.Fields[QuFields._7].AsString();
                View._Text1_6.Text = DataModul.DB_QuTable.Fields[QuFields._8].AsString();
                View._Text1_7.Text = DataModul.DB_QuTable.Fields[QuFields._9].AsString();
                View._Text1_8.Text = DataModul.DB_QuTable.Fields[QuFields._10].AsString();
                View._Text1_9.Text = DataModul.DB_QuTable.Fields[QuFields._11].AsString();
                View._Text1_10.Text = DataModul.DB_QuTable.Fields[QuFields._12].AsString();
                View.RTB1.Text = DataModul.DB_QuTable.Fields[QuFields._13].AsString();
                DataModul.DB_RepoTab.Index = "Nr";
                DataModul.DB_RepoTab.Seek("=", DataModul.DB_QuTable.Fields[QuFields._1].Value);
                View.ComboBox1.BackColor = ColorTranslator.FromOle(-2147483643);
                View.ComboBox1.Items.Clear();
                View.ComboBox1.Text = "";
                goto IL_06dc;
            IL_06dc: // <========== 3
                num = 39;
                if (!DataModul.DB_RepoTab.NoMatch)
                {
                    if (DataModul.DB_RepoTab.EOF)
                    {
                        goto end_IL_0001_2;
                    }
                    DataModul.DB_RepoTable.Index = "Nr";
                    DataModul.DB_RepoTable.Seek("=", DataModul.DB_RepoTab.Fields["Repo"].Value);
                    if (!DataModul.DB_RepoTable.NoMatch)
                    {
                        if (!(DataModul.DB_RepoTab.Fields["Quelle"].Value != DataModul.DB_QuTable.Fields[QuFields._1].Value))
                        {
                            ComboBox comboBox = View.ComboBox1;
                            _ = comboBox.Items.Add(new MyListItem((DataModul.DB_RepoTable.Fields[RepoFields.Name].Value + " " + DataModul.DB_RepoTable.Fields[RepoFields.Ort].Value).AsString(), DataModul.DB_RepoTab.Fields["Repo"].AsInt()));
                            comboBox = null;
                            MyListItem myListItem = (MyListItem)View.ComboBox1.Items[0];
                            View.ComboBox1.Text = myListItem.ItemString + "\r\n";
                            View.ComboBox1.Tag = myListItem.ItemData.ToString();
                            if (View.ComboBox1.Items.Count > 1)
                            {
                                View.ComboBox1.BackColor = ColorTranslator.FromOle(12648447);
                            }
                            DataModul.DB_RepoTab.MoveNext();
                            goto IL_06dc;
                        }
                    }
                }
                Bildzeig();
                Bildliste();
                View._Command1_2.Visible = true;
                M1_Iter = 1;
                while (M1_Iter <= 3)
                {
                    Modul1.Nr = View._Label1_13.Tag.AsInt();
                    DataModul.DB_SourceLinkTable.Index = "Verw";
                    DataModul.DB_SourceLinkTable.Seek("=", Modul1.Nr, M1_Iter);
                    while (!DataModul.DB_SourceLinkTable.EOF
                        && !DataModul.DB_SourceLinkTable.NoMatch
                        && DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._1].AsInt() == M1_Iter
                            && DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._3].AsInt() == Modul1.Nr)
                    {
                        switch (M1_Iter)
                        {
                            case 1:
                            case 2:
                            case 3:
                                View._Command1_2.Visible = false;
                                goto end_IL_0001_2;
                            default:
                                break;
                        }
                        DataModul.DB_SourceLinkTable.MoveNext();
                    }

                    M1_Iter++;
                }
                goto end_IL_0001_2;
            IL_0996:
                num4 = unchecked(num2 + 1);
                goto IL_099a;
            IL_099a:
                num2 = 0;
                switch (num4)
                {
                    case 1:
                        break;
                    case 38:
                    case 39:
                    case 61:
                        goto IL_06dc;
                    case 41:
                    case 82:
                    case 87:
                    case 93:
                        goto end_IL_0001_2;
                }
                goto default;
        }
    end_IL_0001_2: // <========== 4
        return;
    }
    public void _Option1_0_CheckedChanged(object sender, EventArgs e)
    {
        View.List1.Items.Clear();
    }

    public void speichern(ref bool Rück)
    {
        //Discarded unreachable code: IL_0974
        int try0001_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        int lErl = default;
        int M1_Iter = default;
        int num4;
        int i;
        int num6;
        int iQuellNr = 0;
        switch (try0001_dispatch)
        {
            default:
                num = 1;
                if (Modul1.Typ == DriveType.CDRom)
                {
                    goto end_IL_0001;
                }
                iQuellNr = View._Label1_13.Tag.AsInt();
                goto IL_0018;
            case 7427:
                int num5 = default;
                {
                    num2 = num;
                    switch ((num3 <= -2) ? 1 : num3)
                    {
                        case 2:
                            break;
                        case 1:
                            goto IL_19b9;
                        default:
                            goto end_IL_0001_2;
                    }
                    if (Information.Err().Number == 3022)
                    {
                        num5 = iQuellNr;
                        DataModul.DB_QuTable.Index = "dopp";
                        DataModul.DB_QuTable.Seek("=", View._Text1_0.Text, View._Text1_2.Text);
                        bool flag = false;
                        string text;
                        if (!DataModul.DB_QuTable.NoMatch)
                        {
                            Modul1.Nr = DataModul.DB_QuTable.Fields[QuFields._1].AsInt();
                            string QuTable_sField2 = DataModul.DB_QuTable.Fields[QuFields._2].AsString();
                            if (QuTable_sField2 != View._Text1_0.Text)
                            {
                                _ = Interaction.MsgBox(
                                    $"Quelle Nr:{Modul1.Nr}   {QuTable_sField2}\nQuelle Nr:{num5}    {View._Text1_0.Text}", title: "Differenz in Zeile " + View._Label1_1.Text, mb: MessageBoxButtons.OK);
                                flag = true;
                            }
                            if (DataModul.DB_QuTable.Fields[QuFields._4].AsString() != View._Text1_2.Text)
                            {
                                _ = Interaction.MsgBox(
                                    $"Quelle Nr:{Modul1.Nr}   {DataModul.DB_QuTable.Fields[QuFields._4].AsString()}\nQuelle Nr:{num5}    {View._Text1_2.Text}",
                                    title: "Differenz in Zeile " + View._Label1_12.Text, mb: MessageBoxButtons.OK);
                                flag = true;
                            }
                            if (DataModul.DB_QuTable.Fields[QuFields._5].AsString() != View._Text1_3.Text)
                            {
                                _ = Interaction.MsgBox(
                                    $"Quelle Nr:{Modul1.Nr}   {DataModul.DB_QuTable.Fields[QuFields._5].AsString()}\nQuelle Nr:{num5}    {View._Text1_3.Text}",
                                    title: "Differenz in Zeile " + View._Label1_5.Text, mb: MessageBoxButtons.OK);
                                flag = true;
                            }
                            if (DataModul.DB_QuTable.Fields[QuFields._7].AsString() != View._Text1_5.Text)
                            {
                                _ = Interaction.MsgBox(
                                    $"Quelle Nr:{Modul1.Nr}   {DataModul.DB_QuTable.Fields[QuFields._7].AsString()}\nQuelle Nr:{num5}    {View._Text1_5.Text}",
                                    title: "Differenz in Zeile " + View._Label1_4.Text, mb: MessageBoxButtons.OK);
                                flag = true;
                            }
                            if (DataModul.DB_QuTable.Fields[QuFields._8].AsString() != View._Text1_6.Text)
                            {
                                _ = Interaction.MsgBox($"Quelle Nr:{Modul1.Nr}   {DataModul.DB_QuTable.Fields[QuFields._8].AsString()}\nQuelle Nr:{num5}    {View._Text1_6.Text}", title: "Differenz in Zeile " + View._Label1_3.Text, mb: MessageBoxButtons.OK);
                                flag = true;
                            }
                            if (DataModul.DB_QuTable.Fields[QuFields._9].AsString() != View._Text1_7.Text)
                            {
                                _ = Interaction.MsgBox($"Quelle Nr:{Modul1.Nr}   {DataModul.DB_QuTable.Fields[QuFields._9].Value}\nQuelle Nr:{num5}    {View._Text1_7.Text}", title: "Differenz in Zeile " + View._Label1_2.Text, mb: MessageBoxButtons.OK);
                                flag = true;
                            }
                            if (DataModul.DB_QuTable.Fields[QuFields._10].AsString() != View._Text1_8.Text)
                            {
                                _ = Interaction.MsgBox($"Quelle Nr:{Modul1.Nr}   {DataModul.DB_QuTable.Fields[QuFields._10].Value}\nQuelle Nr:{num5}    {View._Text1_8.Text}", title: "Differenz in Zeile " + View._Label1_9.Text, mb: MessageBoxButtons.OK);
                                flag = true;
                            }
                            if (DataModul.DB_QuTable.Fields[QuFields._11].AsString() != View._Text1_9.Text)
                            {
                                _ = Interaction.MsgBox($"Quelle Nr:{Modul1.Nr}   {DataModul.DB_QuTable.Fields[QuFields._11].AsString()}\nQuelle Nr:{num5}    {View._Text1_9.Text}", title: "Differenz in Zeile " + View._Label1_8.Text, mb: MessageBoxButtons.OK);
                                flag = true;
                            }
                            if (DataModul.DB_QuTable.Fields[QuFields._12].AsString() != View._Text1_10.Text)
                            {
                                _ = Interaction.MsgBox($"Quelle Nr:{Modul1.Nr}   {DataModul.DB_QuTable.Fields[QuFields._12].AsString()}\nQuelle Nr:{num5}    {View._Text1_10.Text}", title: "Differenz in Zeile " + View._Label1_7.Text, mb: MessageBoxButtons.OK);
                                flag = true;
                            }
                            if (DataModul.DB_QuTable.Fields[QuFields._13].AsString() != View.RTB1.Text)
                            {
                                _ = Interaction.MsgBox("Die Bemerkungen sind unterschiedlich");
                                flag = true;
                            }
                            if (flag)
                            {
                                text = "Die Felder >Titel< und >Zitiert als< müssen eindeutig sein. Die gemachten Eingaben widersprechen diesem Grundsatz. Bitte korrigieren Sie diese Eingaben, ein Speichern ist sonst nicht möglich.";
                                text += "\n\nWenn Sie Quellen verschmelzen wollen, müssen alle Felder die gleichen Einträge haben.";
                                Rück = Interaction.MsgBox(text, title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OKCancel) == DialogResult.Cancel;
                                goto end_IL_0001;
                            }
                            else
                            {
                                DataModul.wrkDefault.Begin();
                                Modul1.Nr = DataModul.DB_QuTable.Fields[QuFields._1].AsInt();
                                M1_Iter = 1;
                            }
                            goto IL_1434;
                        }
                        else
                        {
                            DataModul.DB_QuTable.Index = "Nam";
                            DataModul.DB_QuTable.Seek("=", View._Text1_0.Text);
                            if (!DataModul.DB_QuTable.NoMatch)
                            {
                                _ = Interaction.MsgBox("Text >" + View._Label1_1.Text + " " + View._Text1_0.Text + "< wird bereits bei Quelle " + DataModul.DB_QuTable.Fields[QuFields._1].AsString() + " verwendet");
                            }
                            else
                            {
                                DataModul.DB_QuTable.Index = "Zitat";
                                DataModul.DB_QuTable.Seek("=", View._Text1_2.Text);
                                if (!DataModul.DB_QuTable.NoMatch)
                                {
                                    _ = Interaction.MsgBox("Text >" + View._Label1_12.Text + " " + View._Text1_2.Text + "< wird bereits bei Quelle " + DataModul.DB_QuTable.Fields[QuFields._1].AsString() + " verwendet");
                                }
                            }
                            text = "Die Felder >Titel< und >Zitiert als< müssen eindeutig sein. Die gemachten Eingaben widersprechen diesem Grundsatz. Bitte korrigieren Sie diese Eingaben, ein Speichern ist sonst nicht möglich.";
                            text += "\n\nWenn Sie Quellen verschmelzen wollen, müssen alle Felder die gleichen Einträge haben.";
                            Rück = Interaction.MsgBox(text, title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OKCancel) == DialogResult.Cancel;
                        }
                        goto end_IL_0001;
                    }
                    else
                    {
                        if (Interaction.MsgBox(Conversion.ErrorToString(), title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                        {
                            ProjectData.EndApp();
                        }
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        num4 = num2;
                        goto IL_19bd;
                    }
                }
            end_IL_0001_2:
                break;
            IL_0018:
                ProjectData.ClearProjectError();
                num3 = 2;
                goto IL_0021;
            IL_0021: // <========== 3
                num = 5;
                lErl = 4;
                if (View._Text1_0.Text.Trim() + View._Text1_1.Text.Trim() + View._Text1_5.Text.Trim() == "")
                {
                    goto end_IL_0001;
                }
                if (View._Text1_0.Text.Trim() == "")
                {
                    if (Interaction.MsgBox("Feld >Titel< muß einen Eintrag enthalten", title: "", mb: MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    {
                        Rück = false;
                        goto end_IL_0001;
                    }
                }
                if (View._Text1_2.Text.Trim() == "")
                {
                    if (Interaction.MsgBox("Feld >Zitiert als< muß einen Eintrag enthalten", title: "", mb: MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    {
                        Rück = false;
                        goto end_IL_0001;
                    }
                }
                bool xChange = DataModul_Quelle_SaveEntry(iQuellNr, View_GetQuellData);
                if (xChange)
                    View.List1.Items.Clear();

                goto end_IL_0001;
            IL_1434: // <========== 3
                num = 156;
                DataModul.DB_SourceLinkTable.Index = "Verw";
                DataModul.DB_SourceLinkTable.Seek("=", Modul1.Nr, M1_Iter);
                Monitor.Enter(_Speichern_Count);
                try
                {
                    if (_Speichern_Count == 0)
                    {
                        _Speichern_Count = 2;
                    }
                    else if (_Speichern_Count == 2)
                    {
                    }
                }
                finally
                {
                    _Speichern_Count = 1;
                    Monitor.Exit(_Speichern_Count);
                }
                goto IL_1620;
            IL_1620: // <========== 3
                num = 160;
                if (!DataModul.DB_SourceLinkTable.EOF)
                {
                    Application.DoEvents();
                    if (!DataModul.DB_SourceLinkTable.NoMatch)
                    {
                        if (!((DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._1].AsInt() != M1_Iter ||
                     DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._3].AsInt() != Modul1.Nr)))
                        {
                            DataModul.DB_SourceLinkTable.Edit();
                            DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._3].Value = num5;
                            DataModul.DB_SourceLinkTable.Update();
                            DataModul.DB_SourceLinkTable.MoveNext();
                            goto IL_1620;
                        }
                    }
                }
                M1_Iter++;
                i = M1_Iter;
                num6 = 3;
                if (i <= num6)
                {
                    goto IL_1434;
                }
                else
                {
                    DataModul.DB_QuTable.Delete();
                    DataModul.wrkDefault.Commit();
                }
                goto IL_0021;
            IL_19b9:
                num4 = unchecked(num2 + 1);
                goto IL_19bd;
            IL_19bd:
                num2 = 0;
                switch (num4)
                {
                    case 1:
                        break;
                    case 5:
                    case 176:
                        goto IL_0021;
                    case 156:
                        goto IL_1434;
                    case 159:
                    case 160:
                    case 172:
                        goto IL_1620;
                    case 2:
                    case 7:
                    case 12:
                    case 18:
                    case 78:
                    case 93:
                    case 94:
                    case 147:
                    case 150:
                    case 197:
                    case 200:
                    case 206:
                        goto end_IL_0001;
                }
                goto default;
        }
    end_IL_0001: // <========== 8
        return;
    }

    private bool DataModul_Quelle_SaveEntry(int iQuellNr, string[] sText)
    {
        var xChange = false;
        DataModul.DB_QuTable.Index = "NR";
        DataModul.DB_QuTable.Seek("=", iQuellNr);
        if (!DataModul.DB_QuTable.NoMatch)
        {
            if (DataModul.DB_QuTable.Fields[QuFields._2].AsString() != sText[0]
                || DataModul.DB_QuTable.Fields[QuFields._4].AsString() != sText[2]
                || DataModul.DB_QuTable.Fields[QuFields._5].AsString() != sText[3]
                || DataModul.DB_QuTable.Fields[QuFields._7].AsString() != sText[5]
                || DataModul.DB_QuTable.Fields[QuFields._8].AsString() != sText[6]
                || DataModul.DB_QuTable.Fields[QuFields._9].AsString() != sText[7]
                || DataModul.DB_QuTable.Fields[QuFields._10].AsString() != sText[8]
                || DataModul.DB_QuTable.Fields[QuFields._11].AsString() != sText[9]
                || DataModul.DB_QuTable.Fields[QuFields._12].AsString() != sText[10]
                || DataModul.DB_QuTable.Fields[QuFields._13].AsString() != sText[11])
            {
                xChange = true;
            }
            DataModul.DB_QuTable.Edit();
            DataModul.DB_QuTable.Fields[QuFields._1].Value = iQuellNr;
            DataModul.DB_QuTable.Fields[QuFields._2].Value = sText[0];
            DataModul.DB_QuTable.Fields[QuFields._4].Value = sText[2];
            DataModul.DB_QuTable.Fields[QuFields._5].Value = sText[3];
            DataModul.DB_QuTable.Fields[QuFields._7].Value = sText[5]; // Updated from sText_5 to sText[5]
            DataModul.DB_QuTable.Fields[QuFields._8].Value = sText[6]; // Updated from sText_6 to sText[6]
            DataModul.DB_QuTable.Fields[QuFields._9].Value = sText[7]; // Updated from sTExt_7 to sText[7]
            DataModul.DB_QuTable.Fields[QuFields._10].Value = sText[8];
            DataModul.DB_QuTable.Fields[QuFields._11].Value = sText[9];
            DataModul.DB_QuTable.Fields[QuFields._12].Value = sText[10];
            DataModul.DB_QuTable.Fields[QuFields._13].Value = sText[11]; // Updated from sTExt_11 to sText[11]
            DataModul.DB_QuTable.Update();
        }
        else
        {
            DataModul.DB_QuTable.AddNew();
            DataModul.DB_QuTable.Fields[QuFields._1].Value = iQuellNr;
            DataModul.DB_QuTable.Fields[QuFields._2].Value = sText[0];
            DataModul.DB_QuTable.Fields[QuFields._4].Value = sText[2];
            DataModul.DB_QuTable.Fields[QuFields._5].Value = sText[3];
            DataModul.DB_QuTable.Fields[QuFields._7].Value = sText[5];
            DataModul.DB_QuTable.Fields[QuFields._8].Value = sText[6]; // Updated from sText_6 to sText[6]
            DataModul.DB_QuTable.Fields[QuFields._9].Value = sText[7]; // Updated from sTExt_7 to sText[7]
            DataModul.DB_QuTable.Fields[QuFields._10].Value = sText[8];
            DataModul.DB_QuTable.Fields[QuFields._11].Value = sText[9];
            DataModul.DB_QuTable.Fields[QuFields._12].Value = sText[10];
            DataModul.DB_QuTable.Fields[QuFields._13].Value = sText[11]; // Updated from sTExt_11 to sText[11]
            DataModul.DB_QuTable.Update();
        }

        return xChange;
    }

    private string[] View_GetQuellData => [
            View._Text1_0.Text,
                View._Text1_1.Text, // Added missing assignment for sText[1]
                View._Text1_2.Text,
                View._Text1_3.Text,
                "", // Added empty initialization for sText[4]
                View._Text1_5.Text,
                View._Text1_6.Text,
                View._Text1_7.Text,
                View._Text1_8.Text,
                View._Text1_9.Text,
                View._Text1_10.Text,
                View.RTB1.Text,
            ];

    public void RTB1_GotFocus(object sender, EventArgs e)
    {
        Frame1_Visible = false;
    }

    public void ComboBox1_DoubleClick(object sender, EventArgs e)
    {
        if (View.ComboBox1.Text == "")
        {
        }
    }

    public void ComboBox2_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        if (View.ComboBox2.Text == "neue Eingabe")
        {
            _ = MainProject.Forms.Bilder.ShowDialog(View._Label1_13.Tag.AsInt(), "Q");
        }
        else
        {
            DataModul.DB_PictureTable.Index = nameof(PictureIndex.Nr);
            _ = Process.Start(((MyComboItem)View.ComboBox2.SelectedItem).WasAnderes);
        }
    }

    public void ComboBox1_KeyUp(object sender, KeyEventArgs e)
    {
        bool Rück = true;
        if (View._Text1_0.Text.Trim() == "")
        {
            _ = Interaction.MsgBox("Feld >Titel< muß einen Eintrag enthalten");
            return;
        }
        if (View._Text1_2.Text.Trim() == "")
        {
            _ = Interaction.MsgBox("Feld >Zitiert als< muß einen Eintrag enthalten");
            return;
        }
        speichern(ref Rück);
        if (View.ComboBox1.Items.Count > 0)
        {
            View.frmSrch_btnDeleteEntry.Enabled = true;
            View.frmSrch_Label5.Text = View.ComboBox1.Text;
        }
        else
        {
            View.frmSrch_btnDeleteEntry.Visible = false;
            View.frmSrch_Label5.Visible = false;
            View.frmSrch_edtSearch.Text = View.ComboBox1.Text;
        }
        View.LagerFrame.Top = checked((int)Math.Round(View._Label1_7.Top + 2.5 * View._Label1_7.Height));
        View.LagerFrame.Visible = true;
    }


    public void btnNewEntry_Click(object sender, EventArgs e)
    {
        MainProject.Forms.Repo.Top = View.LagerFrame.Top;
        MainProject.Forms.Repo.btnSave.Visible = true;
        MainProject.Forms.Repo.btnSave2.Visible = false;
        MainProject.Forms.Repo.btnClose.Visible = true;
        MainProject.Forms.Repo.btnNewEntry.Visible = false;
        Modul1.KontM[1] = 1.AsString();
        _ = MainProject.Forms.Repo.ShowDialog();
        View.LagerFrame.Hide();
        long Satznr = checked(View._Label1_13.Tag.AsInt());
        Les1(Satznr, Rich: false);
    }

    public void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
        MyListItem myListItem = (MyListItem)View.ComboBox1.SelectedItem;
        View.ComboBox1.Text = myListItem.ItemString + "\r\n";
        View.ComboBox1.Tag = myListItem.ItemData.ToString();
    }

    public void Label7_Click(object sender, EventArgs e)
    {
        bool Rück = true;
        if (View._Text1_0.Text.Trim() == "")
        {
            _ = Interaction.MsgBox("Feld >Titel< muß einen Eintrag enthalten");
            return;
        }
        if (View._Text1_2.Text.Trim() == "")
        {
            _ = Interaction.MsgBox("Feld >Zitiert als< muß einen Eintrag enthalten");
            return;
        }
        speichern(ref Rück);
        if (View.ComboBox1.Items.Count > 0)
        {
            View.frmSrch_btnDeleteEntry.Visible = true;
            View.frmSrch_Label5.Visible = true;
            View.frmSrch_Label5.Text = View.ComboBox1.Text;
        }
        else
        {
            View.frmSrch_btnDeleteEntry.Visible = false;
            View.frmSrch_Label5.Visible = false;
        }
        View.LagerFrame.Top = checked((int)Math.Round(View._Label1_7.Top + 2.5 * View._Label1_7.Height + 10.0));
        View.LagerFrame.Left = 20;
        View.LagerFrame.Visible = true;
    }

    public void btnHometown_Click(object sender, EventArgs e)
    {
        if (View.btnHometown.Text != Modul1.IText[EUserText.tNMBack])
        {
            MainProject.Forms.Repo.btnSave.Visible = false;
            MainProject.Forms.Repo.btnSave2.Visible = true;
            MainProject.Forms.Repo.btnClose.Visible = true;
            MainProject.Forms.Repo.btnNewEntry.Visible = true;
            MainProject.Forms.Repo.Width = View.Width;
            MainProject.Forms.Repo.Height = View.Height;
            MainProject.Forms.Repo.Top = View.Top;
            Modul1.KontM[1] = 0.AsString();

            _ = MainProject.Forms.Repo.ShowDialog();
        }
        else
        {
            MainProject.Forms.Repo.Visible = true;
        }
    }

    public void Button6_Click(object sender, EventArgs e)
    {
        bool Rück = default;
        int num5 = default;
        string text = default;
        speichern(ref Rück);
        ProjectData.ClearProjectError();
        num5 = View._Label1_13.Tag.AsInt(); // Updated from Text.AsInt() to Tag.AsInt()
        int Modul1_Nr1 = 0;
        View.Close();
        IRecordset dB_SourceLinkTable = DataModul.DB_SourceLinkTable;

        switch (Modul1.Qkenn)
        {
            case 3:
                if (MainProject.Forms.Ereignis.Visible)
                {
                    if ((DataModul.DB_EventTable.Fields[EventFields.Art].AsInt() < 499))
                    {
                        dB_SourceLinkTable.Index = "Tab23";
                        dB_SourceLinkTable.Seek("=", 3, Modul1_Nr1 = Personen.Default.PersonNr, num5, Modul1.Art, Modul1.LfNR);
                    }
                    else
                    {
                        dB_SourceLinkTable.Index = "Tab23";
                        dB_SourceLinkTable.Seek("=", 3, Modul1_Nr1 = Familie.Default.iFamNr, num5, Modul1.Art, Modul1.LfNR);
                    }
                }
                break;
            case 1:
                dB_SourceLinkTable.Index = "Tab21";
                dB_SourceLinkTable.Seek("=", Modul1.Qkenn, Modul1_Nr1 = Personen.Default.PersonNr, num5);
                if (dB_SourceLinkTable.NoMatch)
                {
                    MainProject.Forms.Quellen.Schreib(Modul1.Qkenn, Modul1_Nr1, MainProject.Forms.Quellen.Nr2, Modul1.Art, Modul1.LfNR);
                    _ = MainProject.Forms.Quellen.RTB.Focus();
                }
                break;
            case 2:
                Modul1.Qkenn = 2;
                dB_SourceLinkTable.Index = "Tab21";
                dB_SourceLinkTable.Seek("=", Modul1.Qkenn, Modul1_Nr1 = Familie.Default.iFamNr, num5);
                if (dB_SourceLinkTable.NoMatch)
                {
                    _ = MainProject.Forms.Quellen.RTB.Focus();
                }
                break;
            default:
                View.Close();
                MainProject.Forms.Quellen.Show();
                return;
        }

        // never ?
        //if (Modul1.Typ != DriveType.CDRom)
        //{
        //    if (sText[0] != "")
        //    {
        //        FileSystem.FileOpen(99, Modul1.InitDir + "Qu.dat", OpenMode.Random);
        //        FileSystem.FilePut(99, sText[0], 1L);
        //    }
        //}

        if (Modul1.Qkenn < 3)
        {
            dB_SourceLinkTable.Index = "Tab21";
            dB_SourceLinkTable.Seek("=", Modul1.Qkenn, Modul1_Nr1, num5);
        }
        else
        {
            dB_SourceLinkTable.Index = "Tab23";
            dB_SourceLinkTable.Seek("=", 3, Modul1_Nr1, num5, Modul1.Art, Modul1.LfNR);
        }

        DataModul.DB_QuTable.Index = "NR";
        DataModul.DB_QuTable.Seek("=", num5);
        string sSourceTitle = DataModul.DB_QuTable.Fields[QuFields._2].AsString();

        string sSrc_OrginalText = dB_SourceLinkTable.Fields[SourceLinkFields._4].AsString();
        string sOrigText = dB_SourceLinkTable.Fields[SourceLinkFields.Orig].AsString();
        string sComment = dB_SourceLinkTable.Fields[SourceLinkFields.Kom].AsString();
        string sSourceAus = dB_SourceLinkTable.Fields[SourceLinkFields.Aus].AsString();

        EinzelQuelle einzelQuelle = MainProject.Forms.EinzelQuelle;
        einzelQuelle.SetData(Modul1.Qkenn, Modul1_Nr1, num5, sSourceTitle);
        einzelQuelle.edtEntry.Text = sSrc_OrginalText;
        einzelQuelle.edtComment.Text = sComment;
        einzelQuelle.edtOriginalText.Text = sOrigText;
        einzelQuelle.edtAus.Text = sSourceAus;
        if ("" == sSourceAus && Modul1.Aus[(int)EOutCfg.o46] == "Y")
        {
            einzelQuelle.edtAus.Text = "Seite:";
        }

        einzelQuelle.Show();
        _ = einzelQuelle.edtEntry.Focus();
        einzelQuelle.Visible = false;
    }

    public void Bildzeig()
    {
        View.PictureBox1.Image = null;
        string BiText = default;
        string Bitext = default;
        bool ja = default;
        ja = Modul1.Bildzeig1("Quelle", View.PictureBox1.Width, View.PictureBox1.Height, "Quellverw", out BiText, out Bitext);
        View.Label6.Text = BiText;
        View.Label8.Text = Bitext;
    }

    private void Bildliste()
    {
        View.ComboBox2.Text = "";
        View.ComboBox2.Items.Clear();
        _ = View.ComboBox2.Items.Add("neue Eingabe");
        if (!(DataModul.DB_QuTable.Fields[QuFields._1].AsInt() > 0))
        {
            return;
        }
        DataModul.DB_PictureTable.Seek("=", "Q", DataModul.DB_QuTable.Fields[QuFields._1].Value);
        if (!DataModul.DB_PictureTable.NoMatch)
        {
            while (!DataModul.DB_PictureTable.EOF
                && !(DataModul.DB_PictureTable.Fields[PictureFields.ZuNr].AsInt() != DataModul.DB_QuTable.Fields[QuFields._1].AsInt()))
            {
                if (DataModul.DB_PictureTable.Fields[PictureFields.Beschreibung].AsString() == "Quelle")
                {
                    View.ComboBox2.Text = DataModul.DB_PictureTable.Fields[PictureFields.Beschreibung].AsString();
                }
                string DateiName = DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString() + DataModul.DB_PictureTable.Fields[PictureFields.Datei].AsString();
                if (DateiName.Left(1) == "#")
                {
                    DateiName = Modul1.Verz + Strings.Mid(DateiName, 2, DateiName.Length);
                }
                if (File.Exists(DateiName))
                {
                    ComboBox.ObjectCollection items = View.ComboBox2.Items;
                    _ = items.Add(new MyComboItem
                    {
                        Text = DataModul.DB_PictureTable.Fields[PictureFields.Beschreibung].AsString(),
                        WasAnderes = DateiName,
                        SatzNr = DataModul.DB_PictureTable.Fields[PictureFields.LfNr].AsInt()
                    });
                }
                DataModul.DB_PictureTable.MoveNext();
            }
        }
        View.ComboBox2.Text = "Alle Medien";
        if (View.ComboBox2.Items.Count > 1)
        {
            View.ComboBox2.Text = "Medien: Ja";
            View.ComboBox2.BackColor = ColorTranslator.FromOle(12648447);
        }
        else
        {
            View.ComboBox2.Text = "Medien: Nein";
            View.ComboBox2.BackColor = ColorTranslator.FromOle(14737632);
        }
    }

    public void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (View.ComboBox2.Text == "neue Eingabe")
        {
            _ = MainProject.Forms.Bilder.ShowDialog(View._Label1_13.Tag.AsInt(), "Q");
        }
        else
        {
            DataModul.DB_PictureTable.Index = nameof(PictureIndex.Nr);
            _ = Process.Start(((MyComboItem)View.ComboBox2.SelectedItem).WasAnderes);
        }
    }

    public void PictureBox1_Click(object sender, EventArgs e)
    {
        _ = Process.Start(View.Label8.Text);
    }
}
