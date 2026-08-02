using BaseLib.Helper;
using GenFreeWin.Main;
using GenFreeWin.Views;
using GenFree;
using GenFree.Data;
using GenFree.Helper;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using GenFree.Interfaces.Sys;
using GenFree.ViewModels.Interfaces;
using GenFreeWin.Views;

namespace GenFreeWin;

[DesignerGenerated]
public partial class Quellen : Form
{
    private static List<WeakReference> __ENCList = new List<WeakReference>();
    IModul1 Modul1 => _Modul1.Instance;
    IInteraction Interaction => Menue.Default;

    public int Nr2;
    private string Q;
    private EEventArt eEventArt;
    private int Modul1_Nr1;


    // Eventhandler direkt im Konstruktor zuweisen
    [DebuggerNonUserCode]
    public Quellen()
    {
        Load += Quellen_Load;
        lock (__ENCList)
        {
            __ENCList.Add(new WeakReference(this));
        }
        InitializeComponent();
        // Eventhandler zuweisen
    }

    [DebuggerNonUserCode]
    protected override void Dispose(bool disposing)
    {
        try
        {

        }
        finally
        {
            base.Dispose(disposing);
        }
    }


    public DialogResult ShowDialog(int eArt)
    {
        eEventArt = (EEventArt)eArt;
        return base.ShowDialog(null);
    }

    private void Quellen_Load(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_03f2
        int try0001_dispatch = -1;
        int num = default;
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
                        Modul1.Art = eEventArt;
                        goto IL_000f;
                    case 1472:
                        {
                            num2 = num;
                            switch ((num3 <= -2) ? 1 : num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_04aa;
                                default:
                                    goto end_IL_0001;
                            }
                            goto IL_03f4;
                        }
                    IL_0334:
                        num = 45;
                        DataModul.DB_SourceLinkTable.MoveFirst();
                        goto IL_0343;
                    IL_0343:
                        num = 46;
                        if (DataModul.CitationData.iQuNr == 0)
                        {
                            goto end_IL_0001_2;
                        }
                        goto IL_0368;
                    IL_0323:
                        num = 44;
                        ListBox1.Visible = false;
                        goto IL_0334;
                    IL_03f4:
                        num = 53;
                        if (Information.Err().Number == 3022)
                        {
                            goto IL_040f;
                        }
                        else
                        {
                            goto IL_0439;
                        }
                    IL_0439:
                        num = 58;
                        if (Information.Err().Number == 3021)
                        {
                            goto end_IL_0001_2;
                        }
                        goto IL_045a;
                    IL_045a:
                        num = 61;
                        if (Interaction.MsgBox(Conversion.ErrorToString(), title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                        {
                            ProjectData.EndApp();
                        }
                        goto IL_0487;
                    IL_0379:
                        num = 48;
                        Button7.Visible = true;
                        goto IL_038a;
                    IL_0487:
                        num = 64;
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        num4 = num2;
                        goto IL_04ae;
                    IL_038a:
                        num = 49;
                        Button9.Visible = true;
                        goto IL_039b;
                    IL_039b:
                        num = 50;
                        Label3.Text = DataModul.CitationData.sSourceTitle + " " + DataModul.CitationData.sPage + " " + DataModul.CitationData.sEntry;
                        goto end_IL_0001_2;
                    IL_04ae:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 2:
                                goto IL_000f;
                            case 3:
                                goto IL_0016;
                            case 4:
                                goto IL_002a;
                            case 5:
                                goto IL_0042;
                            case 6:
                                goto IL_005c;
                            case 7:
                                goto IL_007b;
                            case 8:
                                goto IL_009a;
                            case 9:
                                goto IL_00b9;
                            case 10:
                                goto IL_00d9;
                            case 11:
                                goto IL_00f9;
                            case 12:
                                goto IL_0119;
                            case 13:
                                goto IL_0139;
                            case 14:
                                goto IL_0159;
                            case 15:
                            case 16:
                                goto IL_017a;
                            case 17:
                                goto IL_0191;
                            case 18:
                                goto IL_01a3;
                            case 19:
                                goto IL_01b4;
                            case 21:
                                goto IL_01ca;
                            case 22:
                                goto IL_01cf;
                            case 23:
                                goto IL_01e4;
                            case 24:
                                goto IL_01f9;
                            case 25:
                                goto IL_0210;
                            case 26:
                                goto IL_022e;
                            case 27:
                            case 28:
                                goto IL_023d;
                            case 29:
                                goto IL_025b;
                            case 30:
                                goto IL_026c;
                            case 32:
                                goto IL_027f;
                            case 33:
                                goto IL_0284;
                            case 34:
                                goto IL_0295;
                            case 20:
                            case 31:
                            case 35:
                            case 36:
                            case 37:
                                goto IL_02a8;
                            case 38:
                                goto IL_02bd;
                            case 39:
                                goto IL_02ce;
                            case 40:
                                goto IL_02df;
                            case 41:
                                goto IL_02f0;
                            case 42:
                                goto IL_0301;
                            case 43:
                                goto IL_0312;
                            case 44:
                                goto IL_0323;
                            case 45:
                                goto IL_0334;
                            case 46:
                                goto IL_0343;
                            case 47:
                                goto IL_0368;
                            case 48:
                                goto IL_0379;
                            case 49:
                                goto IL_038a;
                            case 50:
                                goto IL_039b;
                            case 53:
                                goto IL_03f4;
                            case 54:
                                goto IL_040f;
                            case 55:
                                goto IL_041e;
                            case 56:
                            case 57:
                            case 58:
                                goto IL_0439;
                            case 60:
                            case 61:
                                goto IL_045a;
                            case 62:
                            case 64:
                                goto IL_0487;
                            default:
                                goto end_IL_0001;
                            case 51:
                            case 52:
                            case 59:
                            case 65:
                                goto end_IL_0001_2;
                        }
                        goto default;
                    IL_040f:
                        num = 54;
                        DataModul.DB_SourceLinkTable.Delete();
                        goto IL_041e;
                    IL_041e:
                        num = 55;
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        goto IL_04aa;
                    IL_0368:
                        num = 47;
                        Label3.Visible = true;
                        goto IL_0379;
                    IL_04aa:
                        num4 = num2 + 1;
                        goto IL_04ae;
                    IL_000f:
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        goto IL_0016;
                    IL_0016:
                        num = 3;
                        BackColor = Modul1.HintFarb;
                        goto IL_002a;
                    IL_002a:
                        num = 4;
                        if (Modul1.FontSize > 0f)
                        {
                            goto IL_0042;
                        }
                        else
                        {
                            goto IL_017a;
                        }
                    IL_0042:
                        num = 5;
                        Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                        goto IL_005c;
                    IL_005c:
                        num = 6;
                        Button1.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                        goto IL_007b;
                    IL_007b:
                        num = 7;
                        Button2.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                        goto IL_009a;
                    IL_009a:
                        num = 8;
                        Button3.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                        goto IL_00b9;
                    IL_00b9:
                        num = 9;
                        Button4.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                        goto IL_00d9;
                    IL_00d9:
                        num = 10;
                        Button5.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                        goto IL_00f9;
                    IL_00f9:
                        num = 11;
                        ListBox1.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                        goto IL_0119;
                    IL_0119:
                        num = 12;
                        RTB.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                        goto IL_0139;
                    IL_0139:
                        num = 13;
                        Option1.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                        goto IL_0159;
                    IL_0159:
                        num = 14;
                        Option2.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                        goto IL_017a;
                    IL_017a:
                        num = 16;
                        FileSystem.FileClose(99);
                        goto IL_0191;
                    IL_0191:
                        num = 17;
                        if (Modul1.Typ == DriveType.CDRom)
                        {
                            goto IL_01a3;
                        }
                        else
                        {
                            goto IL_01ca;
                        }
                    IL_01a3:
                        num = 18;
                        Option1.Checked = false;
                        goto IL_01b4;
                    IL_01b4:
                        num = 19;
                        Option2.Checked = true;
                        goto IL_02a8;
                    IL_01ca:
                        num = 21;
                        goto IL_01cf;
                    IL_01cf:
                        num = 22;
                        FileSystem.FileOpen(99, Modul1.InitDir + "Qu.dat", OpenMode.Random);
                        goto IL_01e4;
                    IL_01e4:
                        num = 23;
                        FileSystem.FileGet(99, ref Q, 1L);
                        goto IL_01f9;
                    IL_01f9:
                        num = 24;
                        FileSystem.FileClose(99);
                        goto IL_0210;
                    IL_0210:
                        num = 25;
                        if (Q == "")
                        {
                            goto IL_022e;
                        }
                        else
                        {
                            goto IL_023d;
                        }
                    IL_022e:
                        num = 26;
                        Q = "1";
                        goto IL_023d;
                    IL_023d:
                        num = 28;
                        if (Q == "0")
                        {
                            goto IL_025b;
                        }
                        else
                        {
                            goto IL_027f;
                        }
                    IL_025b:
                        num = 29;
                        Option1.Checked = false;
                        goto IL_026c;
                    IL_026c:
                        num = 30;
                        Option2.Checked = true;
                        goto IL_02a8;
                    IL_027f:
                        num = 32;
                        goto IL_0284;
                    IL_0284:
                        num = 33;
                        Option1.Checked = true;
                        goto IL_0295;
                    IL_0295:
                        num = 34;
                        Option2.Checked = false;
                        goto IL_02a8;
                    IL_02a8:
                        num = 37;
                        Label3.Text = "";
                        goto IL_02bd;
                    IL_02bd:
                        num = 38;
                        Label3.Visible = false;
                        goto IL_02ce;
                    IL_02ce:
                        num = 39;
                        Button7.Visible = false;
                        goto IL_02df;
                    IL_02df:
                        num = 40;
                        Button9.Visible = false;
                        goto IL_02f0;
                    IL_02f0:
                        num = 41;
                        TextBox1.Visible = false;
                        goto IL_0301;
                    IL_0301:
                        num = 42;
                        Option1.Visible = false;
                        goto IL_0312;
                    IL_0312:
                        num = 43;
                        Option2.Visible = false;
                        goto IL_0323;
                    end_IL_0001:
                        break;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0001_dispatch = 1472;
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

    private void Button2_Click(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_068d
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
                checked
                {
                    int num4;
                    switch (try0001_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0009;
                        case 2169:
                            {
                                num2 = num;
                                switch ((num3 <= -2) ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_0717;
                                    default:
                                        goto end_IL_0001;
                                }
                                if (Information.Err().Number == 3027)
                                {
                                    _ = Interaction.MsgBox("Änderungen sind nicht möglich");
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
                                goto IL_071b;
                            }
                        end_IL_0001:
                            break;
                        IL_0009:
                            num = 2;
                            //Todo: nummer aus SelectedItem extrahieren
                            Modul1.Nr = ComboBox1.SelectedItem.ItemData<int>();
                            if (Modul1.System.VerSpecial == 0)
                            {
                                switch (Modul1.Qkenn)
                                {
                                    case 1:
                                        if (Modul1.Typ != DriveType.CDRom)
                                        {
                                            if (Modul1.System.VerSpecial == 0)
                                            {
                                                Modul1.PersInArb = Personen.Default.PersonNr;
                                                string sBem3 = RTB.Text.Trim();
                                                Person_SetVal_Bem3(Modul1.PersInArb, sBem3);
                                                RTB.Text = "";
                                            }
                                        }
                                        Personen.Default.Perzeig(Modul1.PersInArb);
                                        break;
                                    case 2:
                                        num = 21;
                                        Modul1.FamInArb = Familie.Default.iFamNr;
                                        DataModul.DB_FamilyTable.Seek("=", Modul1.FamInArb.AsString());
                                        DataModul.DB_FamilyTable.Edit();
                                        DataModul.DB_FamilyTable.Fields[FamilyFields.Bem3].Value = RTB.Text.Trim();
                                        DataModul.DB_FamilyTable.Update();
                                        RTB.Text = "";
                                        Modul1.Famsatzles(Modul1.FamInArb, 2, Modul1.Family);
                                        break;
                                    case 3:
                                        num = 30;
                                        if (Modul1.Typ != DriveType.CDRom)
                                        {
                                            int num5;
                                            if (Operators.ConditionalCompareObjectLess(
                                                DataModul.DB_EventTable.Fields[EventFields.Art].AsInt(), 499, TextCompare: false))
                                            {
                                                num5 = Personen.Default.PersonNr;
                                            }
                                            else
                                            {
                                                Modul1.FamInArb = Familie.Default.iFamNr;
                                                num5 = Modul1.FamInArb;
                                            }
                                            DataModul.DB_EventTable.Edit();
                                            DataModul.DB_EventTable.Fields[EventFields.Bem3].Value = RTB.Text.Trim();
                                            RTB.Text = "";
                                            DataModul.DB_EventTable.Update();
                                            MainProject.Forms.Ereignis.Button1.Text = "&Quellen: Nein";
                                            MainProject.Forms.Ereignis.Button1.BackColor = MainProject.Forms.Ereignis.Button13.BackColor;
                                            if (Strings.Trim(DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString()) != "")
                                            {
                                                MainProject.Forms.Ereignis.Button1.Text = "&Quellen: Ja";
                                                MainProject.Forms.Ereignis.Button1.BackColor = ColorTranslator.FromOle(65535);
                                            }
                                            DataModul.DB_SourceLinkTable.Index = "Tab22";
                                            DataModul.DB_SourceLinkTable.Seek("=", 3, num5, Modul1.Art, Modul1.LfNR);
                                            if (!DataModul.DB_SourceLinkTable.NoMatch)
                                            {
                                                MainProject.Forms.Ereignis.Button1.Text = "&Quellen: Ja";
                                                MainProject.Forms.Ereignis.Button1.BackColor = ColorTranslator.FromOle(65535);
                                            }
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                            Q = "0";
                            if (Option1.Visible)
                            {
                                Q = !Option1.Checked ? "0" : "1";
                                if (Modul1.Typ != DriveType.CDRom)
                                {
                                    Persistence_WriteStringInit(Q, "Qu.dat", 1L);
                                }
                            }
                            ComboBox1.Text = "";
                            Close();
                            goto end_IL_0001_2;
                        IL_0717:
                            num4 = unchecked(num2 + 1);
                            goto IL_071b;
                        IL_071b:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;

                                case 82:
                                case 83:
                                    num = 83;
                                    Close();
                                    goto end_IL_0001_2;
                                case 74:
                                case 84:
                                    goto end_IL_0001_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0001_dispatch = 2169;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001_2: // <========== 3
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    private static void Person_SetVal_Bem3(int persInArb, string sBem3)
    {
        GenFree.Interfaces.DB.IRecordset dB_PersonTable = DataModul.DB_PersonTable;
        dB_PersonTable.Seek("=", persInArb);
        dB_PersonTable.Edit();
        dB_PersonTable.Fields[PersonFields.Bem3].Value = sBem3;
        dB_PersonTable.Update();
    }

    private void Persistence_WriteStringInit(string sValue, string sSection, long lPos)
    {
        FileSystem.FileClose(99);
        FileSystem.FileOpen(99, Modul1.InitDir + sSection, OpenMode.Random);
        FileSystem.FilePut(99, sValue, lPos);
        FileSystem.FileClose(99);
    }

    private void Button4_Click(object sender, EventArgs e)
    {
        if (Modul1.Typ == DriveType.CDRom)
        {
            _ = Interaction.MsgBox(Modul1.Message_sNoChangesOnCD, title: "", icon: MessageBoxIcon.Information);
            return;
        }
        Quellspeich();
        TextBox1.Text = "";
        TextBox2.Text = "";
        ListBox1.Items.Clear();
        TextBox1.Visible = true;
        TextBox2.Visible = true;
        Option1.Visible = true;
        Option2.Visible = true;
        ListBox1.Visible = true;
        Label2.Visible = true;
        Label4.Visible = true;
        _ = TextBox1.Focus();
    }

    private void TextBox1_KeyDown(object sender, KeyEventArgs e)
    {
        TextBox2.Text = "";
        Button6.Visible = false;
    }

    private void TextBox1_TextChanged(object sender, EventArgs e)
    {
        if (TextBox1.Text == "")
        {
            return;
        }
        if (Option1.Checked)
        {
            ListBox1.Items.Clear();
            DataModul.DB_QuTable.Index = "Nam";
            DataModul.DB_QuTable.Seek(">=", TextBox1.Text);
            while (!DataModul.DB_QuTable.EOF && !DataModul.DB_QuTable.NoMatch)
            {
                _ = ListBox1.Items.Add(Strings.Left(((DataModul.DB_QuTable.Fields[QuFields._2].Value) + (new string(' ', 240))).AsString(), 240) + DataModul.DB_QuTable.Fields[QuFields._1].AsString());
                DataModul.DB_QuTable.MoveNext();
            }
        }
        else
        {
            ListBox1.Items.Clear();
            DataModul.DB_QuTable.Index = "Zitat";
            DataModul.DB_QuTable.Seek(">=", TextBox1.Text);
            while (!DataModul.DB_QuTable.EOF && !DataModul.DB_QuTable.NoMatch)
            {
                _ = ListBox1.Items.Add(Strings.Left(((DataModul.DB_QuTable.Fields[QuFields._4].Value) + (new string(' ', 240))).AsString(), 240) + DataModul.DB_QuTable.Fields[QuFields._1].AsString());
                DataModul.DB_QuTable.MoveNext();
            }
        }
    }

    private void TextBox2_TextChanged(object sender, EventArgs e)
    {
        TextBox1.Text = "";
        Button6.Visible = true;
    }

    private void Button6_Click(object sender, EventArgs e)
    {
        ListBox1.Items.Clear();
        if (TextBox2.Text == "")
        {
            return;
        }
        DataModul.DB_QuTable.Index = "Nam";
        string @string = TextBox2.Text.ToUpper().Trim();
        DataModul.DB_QuTable.MoveFirst();
        while (!DataModul.DB_QuTable.EOF && !DataModul.DB_QuTable.NoMatch)
        {
            if (Option1.Checked)
            {
                Type typeFromHandle = typeof(Strings);
                object[] array = new object[1];
                object[] array2 = array;
                var field = DataModul.DB_QuTable.Fields[QuFields._2];
                array2[0] = field.Value;
                object[] array3 = array;
                bool[] array4 = new bool[1] { true };
                object obj = field.AsString().ToUpper();
                if (array4[0])
                {
                    field.Value = array3[0];
                }
                if (Strings.InStr(obj.AsString(), @string) > 0)
                {
                    _ = ListBox1.Items.Add(Strings.Left(((DataModul.DB_QuTable.Fields[QuFields._2].Value) + (new string(' ', 240))).AsString(), 240) + DataModul.DB_QuTable.Fields[QuFields._1].AsString());
                }
            }
            else
            {
                Type typeFromHandle2 = typeof(Strings);
                object[] array3 = new object[1];
                object[] array5 = array3;
                var field = DataModul.DB_QuTable.Fields[QuFields._4];
                array5[0] = field.Value;
                object[] array = array3;
                bool[] array4 = new bool[1] { true };
                object obj2 = field.AsString().ToUpper();
                if (array4[0])
                {
                    field.Value = array[0];
                }
                if (Strings.InStr(obj2.AsString(), @string) > 0)
                {
                    _ = ListBox1.Items.Add(Strings.Left(((DataModul.DB_QuTable.Fields[QuFields._4].Value) + (new string(' ', 240))).AsString(), 240) + DataModul.DB_QuTable.Fields[QuFields._1].AsString());
                }
            }
            DataModul.DB_QuTable.MoveNext();
        }
    }

    private void Option2_CheckedChanged(object sender, EventArgs e)
    {
        TextBox1.Text = "";
        ListBox1.Items.Clear();
        _ = TextBox1.Focus();
    }

    private void ListBox1_DoubleClick(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_05d2
        int try0001_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        string text = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num4;
                    EinzelQuelle einzelQuelle = MainProject.Forms.EinzelQuelle;
                    GenFree.Interfaces.DB.IRecordset dB_SourceLinkTable = DataModul.DB_SourceLinkTable;
                    switch (try0001_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0008;
                        case 3468:
                            {
                                num2 = num;
                                switch ((num3 <= -2) ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_0b94;
                                    default:
                                        goto end_IL_0001;
                                }
                                if (Information.Err().Number == 3021)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_0b94;
                                }
                                else
                                {
                                    goto IL_060d;
                                }
                            }
                        end_IL_0001_2:
                            break;
                        IL_0008:
                            num = 2;
                            Modul1.Nr = ComboBox1.SelectedItem.ItemData<int>();
                            Nr2 = ListBox1.SelectedItem.ItemData<int>();
                            if (Modul1.Qkenn == 3)
                            {
                                if (MainProject.Forms.Ereignis.Visible)
                                {
                                    if (Operators.ConditionalCompareObjectLess(DataModul.DB_EventTable.Fields[EventFields.Art].AsInt(), 499, TextCompare: false))
                                    {
                                        Modul1_Nr1 = Personen.Default.PersonNr;
                                        Modul1.Qkenn = 3;
                                        dB_SourceLinkTable.Index = "Tab23";
                                        dB_SourceLinkTable.Seek("=", 3, Modul1_Nr1, Nr2, Modul1.Art, Modul1.LfNR);
                                        if (!dB_SourceLinkTable.NoMatch)
                                        {
                                            Ein1(Modul1.Qkenn, Modul1_Nr1);
                                        }
                                        else
                                        {
                                            Schreib1(Modul1.Qkenn, Modul1_Nr1, Nr2, Modul1.Art, Modul1.LfNR);
                                        }
                                    }
                                    else
                                    {
                                        Modul1_Nr1 = Familie.Default.iFamNr;
                                        Modul1.Qkenn = 3;
                                        dB_SourceLinkTable.Index = "Tab23";
                                        dB_SourceLinkTable.Seek("=", 3, Modul1_Nr1, Nr2, Modul1.Art, Modul1.LfNR);
                                        if (!dB_SourceLinkTable.NoMatch)
                                        {
                                            Ein1(Modul1.Qkenn, Modul1_Nr1);
                                        }
                                        else
                                        {
                                            Schreib1(Modul1.Qkenn, Modul1_Nr1, Nr2, Modul1.Art, Modul1.LfNR);
                                        }
                                    }
                                    goto IL_060d;
                                }
                            }
                            if (Modul1.Qkenn == 1)
                            {
                                Modul1_Nr1 = Personen.Default.PersonNr;
                                Modul1.Qkenn = 1;
                                dB_SourceLinkTable.Index = "Tab21";
                                dB_SourceLinkTable.Seek("=", Modul1.Qkenn, Modul1_Nr1, Nr2);
                                if (!dB_SourceLinkTable.NoMatch)
                                {
                                    Ein1(Modul1.Qkenn, Modul1_Nr1);
                                }
                                else
                                {
                                    Schreib(Modul1.Qkenn, Modul1_Nr1, Nr2, Modul1.Art, Modul1.LfNR);
                                    if (DataModul.DB_PersonTable.Fields[PersonFields.Bem3].AsString().Length > 1)
                                    {
                                        RTB.Text = DataModul.DB_PersonTable.Fields[PersonFields.Bem3].AsString();
                                    }
                                    _ = RTB.Focus();
                                }
                            }
                            else
                            {
                                if (Modul1.Qkenn != 2)
                                {
                                    break;
                                }
                                Modul1_Nr1 = Familie.Default.iFamNr;
                                Modul1.Qkenn = 2;
                                dB_SourceLinkTable.Index = "Tab21";
                                dB_SourceLinkTable.Seek("=", Modul1.Qkenn, Modul1_Nr1, Nr2);
                                if (!dB_SourceLinkTable.NoMatch)
                                {
                                    Ein1(Modul1.Qkenn, Modul1_Nr1);
                                }
                                else
                                {
                                    Schreib(Modul1.Qkenn, Modul1_Nr1, Nr2, Modul1.Art, Modul1.LfNR);
                                    if (DataModul.DB_FamilyTable.Fields[FamilyFields.Bem3].AsString().Length > 1)
                                    {
                                        RTB.Text = DataModul.DB_FamilyTable.Fields[FamilyFields.Bem3].AsString();
                                        _ = RTB.Focus();
                                    }
                                    _ = RTB.Focus();
                                }
                            }
                            goto IL_060d;
                        IL_060d: // <========== 4
                            num = 69;
                            if (!Option1.Checked)
                            {
                                text = "0";
                            }
                            else
                            {
                                text = "1";
                            }
                            FileSystem.FileClose(99);
                            if (Modul1.Typ != DriveType.CDRom)
                            {
                                if (text != "")
                                {
                                    FileSystem.FileOpen(99, Modul1.InitDir + "Qu.dat", OpenMode.Random);
                                    FileSystem.FilePut(99, text, 1L);
                                }
                            }
                            FileSystem.FileClose(99);
                            Option1.Visible = false;
                            Option2.Visible = false;
                            Option1.Visible = false;
                            if (Modul1.Qkenn < 3)
                            {
                                dB_SourceLinkTable.Index = "Tab21";
                                dB_SourceLinkTable.Seek("=", Modul1.Qkenn, Modul1_Nr1, Nr2);
                            }
                            else
                            {
                                dB_SourceLinkTable.Index = "Tab23";
                                dB_SourceLinkTable.Seek("=", 3, Modul1_Nr1, Nr2, Modul1.Art, Modul1.LfNR);
                            }

                            string sSourceTitle = DataModul_QuTable_GetSourceTitle(Nr2);

                            string sCitationEntry = dB_SourceLinkTable.Fields[SourceLinkFields._4].AsString();
                            string sCitytion_OrgText = dB_SourceLinkTable.Fields[SourceLinkFields.Orig].AsString();
                            string sCitation_Comment = dB_SourceLinkTable.Fields[SourceLinkFields.Kom].AsString();
                            string sCitation_Aus = dB_SourceLinkTable.Fields[SourceLinkFields.Aus].AsString();

                            einzelQuelle.SetData(Modul1.Qkenn, iPerFamNr: Modul1_Nr1, iSourceNr: Nr2, sSourceTitle);
                            einzelQuelle.edtEntry.Text = sCitationEntry.AsString();
                            einzelQuelle.edtOriginalText.SelectedText = sCitytion_OrgText.AsString();
                            einzelQuelle.edtComment.SelectedText = sCitation_Comment.AsString();
                            einzelQuelle.edtAus.Text = sCitation_Aus.AsString();
                            if ("" == sCitation_Aus)
                            {
                                einzelQuelle.edtAus.Text = "Seite:";
                            }
                            einzelQuelle.Show();
                            _ = einzelQuelle.edtEntry.Focus();
                            einzelQuelle.Visible = false;

                            break;
                        IL_0b94:
                            num4 = unchecked(num2 + 1);
                            while (true)
                            {
                                num2 = 0;
                                switch (num4)
                                {
                                    case 1:
                                        break;
                                    case 13:
                                    case 16:
                                    case 24:
                                    case 27:
                                    case 28:
                                    case 38:
                                    case 45:
                                    case 54:
                                    case 62:
                                    case 67:
                                    case 69:
                                        goto IL_060d;
                                    case 119:
                                        goto end_IL_0001_2;
                                    case 121:
                                        goto IL_0b73;
                                }
                                break;
                            IL_0b73:
                                num = 121;
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                            }
                            goto default;
                    }
                    num = 119;
                    _ = einzelQuelle.ShowDialog();
                    break;
                }
            end_IL_0001:
                ;
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0001_dispatch = 3468;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    private string DataModul_QuTable_GetSourceTitle(int nr2)
    {
        GenFree.Interfaces.DB.IRecordset dB_QuTable = DataModul.DB_QuTable;
        dB_QuTable.Index = "NR";
        dB_QuTable.Seek("=", nr2);
        return dB_QuTable.Fields[QuFields._2.AsFld()].AsString();
    }

    public void Ein1(short qkenn, int iPerFamNr)
    {
        ListBox1.Visible = false;
        TextBox1.Visible = false;
        RTB.Visible = true;
        SourceLink_RefreshList(qkenn, iPerFamNr);
    }

    public void Schreib1(short qkenn, int iPerFamNr, int iNr2, EEventArt eArt, int lfNR)
    {
        DataModul.SourceLink_AppendRaw(qkenn, iPerFamNr, iNr2, eArt, lfNR);
        ListBox1.Visible = false;
        TextBox1.Visible = false;
        RTB.Visible = true;
        SourceLink_RefreshList(qkenn, iPerFamNr);
    }

    public void Schreib(short qkenn, int iPerFam, int nr2, EEventArt art, short lfNR)
    {
        DataModul.SourceLink_AppendRaw(qkenn, iPerFam, nr2, art, lfNR);
        // Buttons
        ListBox1.Visible = false;
        TextBox1.Visible = false;
        TextBox2.Visible = false;
        Option1.Visible = false;
        Option2.Visible = false;
        Label2.Visible = false;
        Label4.Visible = false;
        Button6.Visible = false;
        RTB.Visible = true;
        // Fill List
        SourceLink_RefreshList(qkenn, iPerFam);
    }

    public void SourceLink_RefreshList(short qkenn, int iLink)
    {
        ComboBox1.Items.Clear();
        DataModul.DB_SourceLinkTable.Index = "Tab";
        DataModul.DB_SourceLinkTable.Seek("=", qkenn, iLink);
        while (!DataModul.DB_SourceLinkTable.EOF
            && DataModul.DB_SourceLinkTable.Fields[0].AsInt() == qkenn
               && DataModul.DB_SourceLinkTable.Fields[1].AsInt() <= iLink)
        {
            DataModul.DB_QuTable.Index = "Nr";
            DataModul.DB_QuTable.Seek("=", DataModul.DB_SourceLinkTable.Fields[2]);
            if (!DataModul.DB_QuTable.NoMatch)
                _ = ComboBox1.Items.Add(new ListItem<int>((DataModul.DB_QuTable.Fields[QuFields._2].AsString() + new string(' ', 240)).Left(240) + DataModul.DB_QuTable.Fields[QuFields._1].AsString(), DataModul.DB_QuTable.Fields[QuFields._1].AsInt()));
            DataModul.DB_SourceLinkTable.MoveNext();
        }
        if (ComboBox1.Items.Count > 0)
            ComboBox1.Text = ComboBox1.Items[0].AsString();
    }

    private void Button3_Click(object sender, EventArgs e)
    {
        if (Modul1.Typ != DriveType.CDRom)
        {
            Quellspeich();
            MainProject.Forms.Quellverw.Close();
        }
        checked
        {
            int ubg = ComboBox1.SelectedItem.ItemData<int>();
            if (ubg == 0)
            {
                _ = Interaction.MsgBox("Keine Quelle ausgewählt, Neueingabe nur über die Quellenverwaltung");
                return;
            }
            int M1_Iter = 0;
            int i;
            int num;
            do
            {
                MainProject.Forms.Quellverw.ACommand1[(short)M1_Iter].Visible = false;
                M1_Iter++;
                i = M1_Iter;
                num = 9;
            }
            while (i <= num);
            MainProject.Forms.Quellverw._Command1_12.Visible = false;
            MainProject.Forms.Quellverw._Command1_6.Visible = true;
            _ = MainProject.Forms.Quellverw.ShowDialog(ubg);
            Show();
        }
    }

    private void Button1_Click(object sender, EventArgs e)
    {
        if (ComboBox1.Text == "")
        {
            return;
        }
        Modul1.Art = DataModul.DB_EventTable.Fields[EventFields.Art].AsEnum<EEventArt>();
        Modul1.Nr = Module2.ZuPerFamNummer(Modul1.Art, Modul1.Qkenn);

        if (Modul1.Nr != 0)
        {
            GenFree.Interfaces.DB.IRecordset dB_SourceLinkTable = DataModul.DB_SourceLinkTable;
            if (Modul1.Qkenn < 3)
            {
                dB_SourceLinkTable.Index = "Tab21";
                dB_SourceLinkTable.Seek("=", Modul1.Qkenn, Modul1_Nr1, Modul1.Nr);
            }
            else
            {
                dB_SourceLinkTable.Index = "Tab23";
                dB_SourceLinkTable.Seek("=", 3, Modul1_Nr1, Modul1.Nr, Modul1.Art, Modul1.LfNR);
            }

            string sSourceTitle = DataModul_QuTable_GetSourceTitle(Nr2);

            EinzelQuelle einzelQuelle = MainProject.Forms.EinzelQuelle;
            string sCitationEntry = dB_SourceLinkTable.Fields[SourceLinkFields._4.AsFld()].AsString();
            string sCitytion_OrgText = dB_SourceLinkTable.Fields[SourceLinkFields.Orig].AsString();
            string sCitation_Comment = dB_SourceLinkTable.Fields[SourceLinkFields.Kom].AsString();
            string sCitation_Aus = dB_SourceLinkTable.Fields[SourceLinkFields.Aus].AsString();

            einzelQuelle.edtEntry.Text = sCitationEntry;
            einzelQuelle.edtOriginalText.Text = sCitytion_OrgText;
            einzelQuelle.edtComment.Text = sCitation_Comment;
            einzelQuelle.edtAus.Text = sCitation_Aus;
            if ("" == sCitation_Aus && Modul1.Aus[(int)EOutCfg.o46] == "Y")
            {
                einzelQuelle.edtAus.Text = "Seite:";
            }
            einzelQuelle.Top = Top;
            einzelQuelle.Left = Left;
            einzelQuelle.Show();
            _ = einzelQuelle.edtEntry.Focus();
            einzelQuelle.Visible = false;
            _ = einzelQuelle.ShowDialog(Modul1.Qkenn, Modul1_Nr1, Modul1.Nr, sSourceTitle);
        }
    }

    private void Button5_Click(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_04fc
        if (Modul1.Typ == DriveType.CDRom)
        {
            _ = Interaction.MsgBox(Modul1.Message_sNoChangesOnCD, title: "", icon: MessageBoxIcon.Information);
            return;
        }
        Quellspeich();
        checked
        {
            Modul1.Nr = ComboBox1.SelectedItem.ItemData<int>();
            int num = Module2.ZuPerFamNummer(Modul1.Art, Modul1.Qkenn);
            if (Modul1.Qkenn != 3)
            {
                DataModul.DB_SourceLinkTable.Index = "Tab21";
                DataModul.DB_SourceLinkTable.Seek("=", Modul1.Qkenn, num, Modul1.Nr);
                if (!DataModul.DB_SourceLinkTable.NoMatch)
                {
                    DataModul.DB_SourceLinkTable.Delete();

                    DataModul.DB_SourceLinkTable.Index = "Tab";
                    DataModul.DB_SourceLinkTable.Seek("=", Modul1.Qkenn, num);

                    SourceLink_RefreshList(Modul1.Qkenn, Modul1_Nr1);
                    if (Modul1.Qkenn == 1)
                    {
                        if (DataModul.DB_PersonTable.Fields[PersonFields.Bem3].AsString().Length > 1)
                        {
                            RTB.Text = DataModul.DB_PersonTable.Fields[PersonFields.Bem3].AsString();
                        }
                    }
                    else if (Modul1.Qkenn == 2
                        && DataModul.DB_FamilyTable.Fields[FamilyFields.Bem3].AsString().Length > 1)
                    {
                        RTB.Text = DataModul.DB_FamilyTable.Fields[FamilyFields.Bem3].AsString();
                    }
                }
                ComboBox1.Text = "";
                if (ComboBox1.Items.Count > 0)
                {
                    ComboBox1.Text = ComboBox1.Items[0].AsString();
                }
                return;
            }
            DataModul.DB_SourceLinkTable.Index = "Tab22";
            DataModul.DB_SourceLinkTable.Seek("=", 3, num, Modul1.Art, Modul1.LfNR);
            if (!DataModul.DB_SourceLinkTable.NoMatch)
            {
                while (!DataModul.DB_SourceLinkTable.EOF)
                {
                    if (DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._3].AsInt() == ComboBox1.SelectedItem.ItemData<int>())
                    {
                        DataModul.DB_SourceLinkTable.Delete();
                        break;
                    }
                    DataModul.DB_SourceLinkTable.MoveNext();
                }

                SourceLink_RefreshListEvt(num, Modul1.Art, Modul1.LfNR);
                if (null != DataModul.DB_EventTable.Fields[EventFields.Bem3].Value)
                {
                    RTB.Text = DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString();
                }
            }

            ComboBox1.Text = "";
            if (ComboBox1.Items.Count > 0)
            {
                ComboBox1.Text = ComboBox1.Items[0].AsString();
            }
        }
    }

    public void SourceLink_RefreshListEvt(int persInArb, EEventArt eArt, short iLfNr)
    {
        ComboBox1.Items.Clear();
        DataModul.DB_SourceLinkTable.Index = "Tab22";
        DataModul.DB_SourceLinkTable.Seek("=", 3, persInArb, eArt, iLfNr);
        if (!DataModul.DB_SourceLinkTable.NoMatch)
        {
            while (!DataModul.DB_SourceLinkTable.EOF
                && !DataModul.DB_SourceLinkTable.NoMatch
                && DataModul.DB_SourceLinkTable.Fields[0].AsInt() == Modul1.Qkenn
                && DataModul.DB_SourceLinkTable.Fields[1].AsInt() == persInArb
                && DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Art].AsInt() == DataModul.DB_EventTable.Fields[EventFields.Art].AsInt()
                && DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.LfNr].AsInt() == iLfNr)
            {
                Button3.Enabled = true;
                DataModul.DB_QuTable.Index = "Nr";
                DataModul.DB_QuTable.Seek("=", DataModul.DB_SourceLinkTable.Fields[2]);
                if (!DataModul.DB_QuTable.NoMatch)
                {
                    _ = ComboBox1.Items.Add(new ListItem<int>((DataModul.DB_QuTable.Fields[QuFields._2].AsString() + new string(' ', 240)).Left(240) + DataModul.DB_QuTable.Fields[QuFields._1].AsString(), DataModul.DB_QuTable.Fields[QuFields._1].AsInt()));
                }
                DataModul.DB_SourceLinkTable.MoveNext();
            }
        }
        if (ComboBox1.Items.Count > 0)
        {
            ComboBox1.Text = ComboBox1.Items[0].AsString();
        }
    }

    private void RTB_KeyDown(object sender, KeyEventArgs e)
    {
        checked
        {
            short num = (short)e.KeyCode;
            short num2 = (short)unchecked((int)e.KeyData / 65536);
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
                        RTB.SelectedText = Modul1.Te[num - 113];
                        break;
                }
            }
        }
    }

    public void Quellspeich()
    {
        checked
        {
            switch (Modul1.Qkenn)
            {
                case 1:
                    if (Modul1.System.VerSpecial == 0)
                    {
                        Modul1.PersInArb = Personen.Default.PersonNr;
                        Person_SetVal_Bem3(Modul1.PersInArb, RTB.Text.Trim());
                    }
                    break;
                case 2:
                    {
                        Modul1.FamInArb = Familie.Default.iFamNr;
                        Family_SetVal_Bem3(Modul1.FamInArb, RTB.Text.Trim());
                        Modul1.Famsatzles(Modul1.FamInArb, 2, Modul1.Family);
                        break;
                    }
                case 3:
                    if (Operators.ConditionalCompareObjectLess(DataModul.DB_EventTable.Fields[EventFields.Art].AsInt(), 499, TextCompare: false))
                    {
                        int num = Personen.Default.PersonNr;
                    }
                    else
                    {
                        Modul1.FamInArb = Familie.Default.iFamNr;
                    }
                    DataModul.DB_EventTable.Edit();
                    DataModul.DB_EventTable.Fields[EventFields.Bem3].Value = RTB.Text.Trim();
                    DataModul.DB_EventTable.Update();
                    break;
            }
        }
    }

    private void Family_SetVal_Bem3(object famInArb, string sBem3)
    {
        GenFree.Interfaces.DB.IRecordset dB_FamilyTable = DataModul.DB_FamilyTable;
        dB_FamilyTable.Seek("=", famInArb);
        dB_FamilyTable.Edit();
        dB_FamilyTable.Fields[FamilyFields.Bem3].Value = sBem3;
        dB_FamilyTable.Update();
    }

    private void Button7_Click(object sender, EventArgs e)
    {
        if (DataModul.CitationData.iQuNr != 0)
        {
            var iPerFam = Module2.ZuPerFamNummer(DataModul.DB_EventTable.Fields[EventFields.Art].AsEnum<EEventArt>(), Modul1.Qkenn);
            EinzelQuelle einzelQuelle = MainProject.Forms.EinzelQuelle;
            DataModul.CitationData.iLinkType = (short)Modul1.Qkenn;
            DataModul.CitationData.Commit(iPerFam, Modul1.Art, Modul1.LfNR);
            Button2.PerformClick();
        }
    }

    private void ComboBox1_TextChanged(object sender, EventArgs e)
    {
        _ = Button1.Focus();
    }

    private void Button8_Click(object sender, EventArgs e)
    {
        if (Modul1.Typ != DriveType.CDRom)
        {
            DataModul.DB_QuTable.Index = "Nr";
            DataModul.DB_QuTable.MoveLast();
            var Modul1_Satznr = DataModul.DB_QuTable.Fields[QuFields._1].AsInt() + 1;

            MainProject.Forms.Quellverw.ALabel1[13].Text = Modul1_Satznr.AsString();
            for (var i = 0; i < 11; i++)
                if (i != 4)
                    MainProject.Forms.Quellverw.AText1[i].Text = "";
            MainProject.Forms.Quellverw.RTB1.Text = "";
            MainProject.Forms.Quellverw.ComboBox1.Items.Clear();
            MainProject.Forms.Quellverw.ComboBox1.Text = "";
            int M1_Iter = 0;
            while (M1_Iter++ <= 9)
            {
                MainProject.Forms.Quellverw.ACommand1[(short)M1_Iter].Visible = false;
            }
            MainProject.Forms.Quellverw._Command1_12.Visible = false;
            MainProject.Forms.Quellverw.btnHometown.Visible = false;
            MainProject.Forms.Quellverw.btnClose2.Visible = true;
            _ = MainProject.Forms.Quellverw.ShowDialog(Modul1_Satznr);
            Show();
        }

    }

    private void Button9_Click(object sender, EventArgs e)
    {
        if (DataModul.CitationData.iQuNr != 0)
        {
            var iPerFam = Module2.ZuPerFamNummer(Modul1.Art, Modul1.Qkenn);
            Schreib(Modul1.Qkenn, Modul1_Nr1, Nr2, Modul1.Art, Modul1.LfNR);

            DataModul.CitationData.sEntry = "";
            DataModul.CitationData.sOriginalText = "";
            DataModul.CitationData.sComment = "";
            DataModul.CitationData.Commit(iPerFam, Modul1.Art, Modul1.LfNR);
        }
    }

    private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
    }
}
