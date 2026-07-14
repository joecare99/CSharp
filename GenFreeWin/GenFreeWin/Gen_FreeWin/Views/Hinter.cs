using BaseLib.Helper;
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
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace GenFreeWin.Views;

public partial class Hinter : Form
{
    private static List<WeakReference> __ENCList = new List<WeakReference>();

    List<(Control, EOutCfg, string)> _OutCfg;

    IModul1 Modul1 => _Modul1.Instance;
    public int[] Modul1_Diff = new int[5];
    [Obsolete]
    IProjectData ProjectData;
    IInteraction Interaction;
    [Obsolete]
    IVBInformation Information;
    [Obsolete]
    IOperators Operators;
    [Obsolete]
    IVBConversions Conversions;
    [Obsolete]
    IStrings Strings;

    public bool reorga { get; internal set; }
    public string FileListBox1_Pattern { get; private set; }
    public string DriveListBox1_Drive { get; private set; }
    public string FileListBox1_Path { get; private set; }
    public string DirListBox1_Path { get; private set; }
    public string FileListBox1_FileName { get; private set; }
    public bool IsNotReadOnly => Modul1.Typ != DriveType.CDRom;

    public Hinter()
    {
        Load += Hinter_Load;
        lock (__ENCList)
        {
            __ENCList.Add(new WeakReference(this));
        }
        CommonDialog1Save = new SaveFileDialog();
        InitializeComponent();

        _OutCfg = new()
        {
            (TextBox1, EOutCfg.o08, "20"),
            (TextBox2, EOutCfg.o10_EmitIDs, "18"),
            (TextBox3, EOutCfg.o09, "40"),
            (TextBox4, EOutCfg.o11, "50"),
            (TextBox5, EOutCfg.o23, "0"),
            (TextBox6, EOutCfg.o12, "500"),
            (TextBox6, EOutCfg.o13, "500"),
            (TextBox7, EOutCfg.o15, "1800"),
            (CheckBox8, EOutCfg.o45, "Y"),
            (CheckBox8, EOutCfg.o20, "Y"),
            (CheckBox7, EOutCfg.o24, "1"),
            (CheckBox9, EOutCfg.o17, "Y"),
            (Option1, EOutCfg.o26, true.AsString()),
            (CheckBox2, EOutCfg.o44_PictOrginalSize, "Y"),
            (CheckBox3, EOutCfg.o45, "Y"),
            (CheckBox4, EOutCfg.o46, "Y"),
            (CheckBox5, EOutCfg.o48, "Y"),
            (RadioButton32, EOutCfg.o47, "Y"),
        };
    }


    public void AppendText(string sText, int iFamInArb, int iIndent = 0)
    {
        RTB.SelectedText = sText + "\n";
        _ = List1.Items.Add(new ListItem(new string(' ', iIndent) + sText, iFamInArb));
    }

    public void ReadFamily(int famInArb, IFamilyData family)
    {
        //Discarded unreachable code: IL_03fd
        ListBox1.Items.Clear();
        family.Clear();
        var M1_Iter = 1;
        int num2 = default;
        foreach (var link in DataModul.Link.ReadAllFams(famInArb))
        {
            if (link.eKennz == ELinkKennz.lkFather)
            {
                family.Mann = link.iPersNr;
            }
            else if (link.eKennz == ELinkKennz.lkMother)
            {
                family.Frau = link.iPersNr;
            }
            else if (link.eKennz == ELinkKennz.lkChild)
            {
                num2++;
                family.Kinder.Add((link.iPersNr, ""));

                var Event_dDatumV = DataModul.Event.GetPersonBirthOrBapt(link.iPersNr);
                if (Event_dDatumV == default)
                    _ = ListBox1.Items.Add(new ListItem("00000000", link.iPersNr));
                else
                    _ = ListBox1.Items.Add(new ListItem($"{Event_dDatumV.AsString(),8}", link.iPersNr));
            }
            if (M1_Iter++ > 99)
                break;
        }
    }

    public void AppenTitle(string UbgT, int iFamNr)
    {
        RTB.SelectionFont = new Font("Arial", 11.01f, FontStyle.Bold);
        AppendText(UbgT, iFamNr, 0);
        RTB.SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
    }

    public void Show1(int iType)
    {
        Button9.Text = Modul1.IText[EUserText.t158];
        Button7.Text = Modul1.IText[EUserText.tNMPrint];
        Button6.Text = Modul1.IText[EUserText.t204];
        Button7.Visible = iType == 0;
        Button8.Text = Modul1.IText[EUserText.tNMSave];
        Frame2.Visible = iType == 0;
        Label53.Text = Modul1.IText[198 + iType];
        Label53.Tag = iType;
    }

    public void Show0(string sTitle)
    {
        Text = MainProject.Forms.Hinter.Text + sTitle;
        Button7.Visible = false;
        Button8.Visible = false;
        GroupBox1.Visible = true;
        Show();
        Refresh();
    }

    private void Button3_Click_1(object sender, EventArgs e)
    {
        DriveListBox1.Visible = true;
        DirListBox1.Visible = true;
        FileListBox1_Pattern = "*.EXE";
        FileListBox1.Visible = true;
        DriveListBox1_Drive = "C:";
        DirListBox1_Path = "C:\\Program Files\\Google";
        Modul1.Ubg = 1;
    }

    private void Button12_Click_1(object sender, EventArgs e)
    {
        ColorDialog colorDialog = new ColorDialog();
        colorDialog.AllowFullOpen = true;
        colorDialog.ShowHelp = true;
        colorDialog.Color = BackColor;
        if (colorDialog.ShowDialog() == DialogResult.OK)
        {
            BackColor = colorDialog.Color;
            Modul1.HintFarb = colorDialog.Color;
        }
        Modul1.Persistence.PutColorInit("Farb.dat", Modul1.HintFarb, 1);
        Frame4.BackColor = Modul1.HintFarb;
    }

    private void Button14_Click(object sender, EventArgs e)
    {
        ColorDialog colorDialog = new ColorDialog();
        colorDialog.AllowFullOpen = true;
        colorDialog.ShowHelp = true;
        colorDialog.Color = BackColor;
        if (colorDialog.ShowDialog() == DialogResult.OK)
        {
            BackColor = colorDialog.Color;
            Modul1.Feld1Farb = colorDialog.Color;
        }
        Modul1.Persistence.PutColorInit("Farb.dat", Modul1.Feld1Farb, 2);
    }

    private void Button1_Click_1(object sender, EventArgs e)
    {
        FileListBox1_Path = "";
        DirListBox1_Path = "C:\\";
        FileListBox1_Pattern = "*.EXE";
        DriveListBox1_Drive = "C:\\";
        DirListBox1_Path = "C:\\Programme\\Microsoft Office";
        Frame2.Visible = true;
        Modul1.Ubg = 2;
    }
    private void DriveListBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
        DirListBox1_Path = DriveListBox1_Drive.Left(2) + "\\";
    }

    private void DirListBox1_Change(object sender, EventArgs e)
    {
        FileListBox1_Path = DirListBox1_Path;
    }

    private void DirListBox1_DoubleClick1(object sender, EventArgs e)
    {
        FileListBox1_Path = DirListBox1_Path;
    }

    private void FileListBox1_DoubleClick1(object sender, EventArgs e)
    {
        Modul1.UbgT = FileListBox1_Path + "\\" + FileListBox1_FileName;
        if (Modul1.Ubg == 2)
        {
            Label2.Text = "Textverarbeitung: " + FileListBox1_FileName;
            Modul1.Aus[(int)EOutCfg.o07_KeepFormat] = Modul1.UbgT;
        }
        Frame2.Visible = false;
    }

    private void Button4_Click(object sender, EventArgs e)
    {
        Frame2.Visible = false;
    }

    private void Button3_Click(object sender, EventArgs e)
    {
        if (Modul1.OfficeAppInstalled(IModul1.MSOfficeComponent.MSWord))
            // do as a loop
            foreach (IModul1.MSOfficeVersion version in Enum.GetValues(typeof(IModul1.MSOfficeVersion)))
            {
                string text;
                if ((text = Modul1.OfficeInstallPath(version)).Length > 0)
                {
                    Modul1.Aus[(int)EOutCfg.o07_KeepFormat] = text + "Winword.exe";
                    break;
                }
            }

        Modul1.UbgT = Modul1.Aus[(int)EOutCfg.o07_KeepFormat];
        //Discarded unreachable code: IL_021f
        int M1_Iter = 1;
        while (M1_Iter <= 10)
        {
            byte b = (byte)Strings.InStr(Modul1.UbgT, "\\");
            if (b == 0)
            {
                Label2.Text = Modul1.IText[EUserText.t217] + " = " + Modul1.UbgT;
                break;
            }

            Modul1.UbgT = Strings.Trim(Strings.Mid(Modul1.UbgT, unchecked(b) + 1, Modul1.UbgT.Length));
            M1_Iter++;
        }

    }

    private void Hinter_Load(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_156d, IL_15d4, IL_169e
        int try0001_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        short num4 = default;
        int lErl = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    string DateiName;
                    string[] array;
                    short num8;
                    short num6;
                    switch (try0001_dispatch)
                    {
                        default:
                            num = 1;
                            Label3.Text = "";
                            goto IL_0015;
                        case 6864:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 3:
                                        break;
                                    case 2:
                                        goto IL_15d6;
                                    case 4:
                                        goto IL_1647;
                                    case 1:
                                        goto IL_16a2;
                                    default:
                                        goto end_IL_0001;
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_16a2;
                            }
                        end_IL_0001:
                            break;
                        IL_0015:
                            num = 2;
                            Label18.Text = "";
                            FileSystem.FileClose(6);
                            FileSystem.FileClose(99);
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            Modul1.Persistence.ReadEnumInit<FormWindowState>("Windowstate", out var WiS);
                            WindowState = WiS;
                            Refresh();
                            Button1.Text = Modul1.IText[EUserText.t366];
                            Button10.Text = Modul1.IText[EUserText.t375];
                            Button12.Text = Modul1.IText[EUserText.t370];
                            Button13.Text = Modul1.IText[EUserText.t371];
                            Button14.Text = Modul1.IText[EUserText.t372];
                            Button15.Text = Modul1.IText[EUserText.t373];
                            CheckBox1.Text = Modul1.IText[EUserText.t367];
                            CheckBox2.Text = Modul1.IText[EUserText.t388];
                            CheckBox3.Text = Modul1.IText[EUserText.t431];
                            CheckBox7.Text = Modul1.IText[EUserText.t383];
                            CheckBox8.Text = Modul1.IText[EUserText.t384];
                            CheckBox9.Text = Modul1.IText[EUserText.t385];
                            CheckBox10.Text = Modul1.IText[EUserText.t391];
                            Frame1.Text = Modul1.IText[EUserText.t368];
                            Frame3.Text = Modul1.IText[EUserText.t369];
                            Label7.Text = Modul1.IText[EUserText.t376];
                            Label8.Text = Modul1.IText[EUserText.t377];
                            Label9.Text = Modul1.IText[EUserText.t378];
                            Label10.Text = Modul1.IText[EUserText.t379];
                            Label11.Text = Modul1.IText[EUserText.t380];
                            Label12.Text = Modul1.IText[EUserText.t382];
                            Label13.Text = Modul1.IText[EUserText.t381];
                            Label14.Text = Modul1.IText[EUserText.t386];
                            Label15.Text = Modul1.IText[EUserText.t387];
                            Label19.Text = Modul1.IText[EUserText.t411];
                            Option1.Text = Modul1.IText[EUserText.t389];
                            Option2.Text = Modul1.IText[EUserText.t390];
                            Frame4.BackColor = Modul1.HintFarb;
                            if (Modul1.FontSize > 0f)
                            {
                                Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                List1.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                Label6.Font = new Font("Arial", Modul1.FontSize, FontStyle.Bold);
                                Label4.Font = new Font("Arial", Modul1.FontSize, FontStyle.Bold);
                                Label5.Font = new Font("Arial", Modul1.FontSize, FontStyle.Bold);
                                Button1.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                Button2.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                Button3.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                Button4.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                Button5.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                Button10.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                Button12.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                Button13.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                Button14.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                Button15.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                Label1.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                Label2.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                Label3.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                Label7.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                Label8.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                Label9.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                Label19.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                Label10.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                Label11.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                Label12.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                Label13.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                Label14.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                Label15.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                Label16.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                Label17.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                Label18.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                Label53.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                TextBox1.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                TextBox2.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                TextBox3.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                TextBox4.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                TextBox5.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                TextBox6.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                TextBox7.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                Frame3.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                CheckBox2.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                CheckBox7.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                CheckBox8.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                CheckBox9.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                CheckBox10.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                Option1.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                Option2.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                CheckBox1.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                CheckBox4.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                CheckBox5.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                RadioButton31.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                RadioButton32.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                            }
                            Button5.Text = Modul1.IText[EUserText.t158];
                            var pos = Modul1.Persistence.ReadIntsProg("maspos.dat", 2);
                            Left = pos[0];
                            Top = pos[1];
                            Modul1.AutoupD = Modul1.Persistence.ReadStringInit("Update_ini.dat");
                            if (string.IsNullOrWhiteSpace(Modul1.AutoupD))
                                Modul1.AutoupD = 0.AsString();
                            Button2.Visible = true;
                            CheckBox1.CheckState = unchecked((CheckState)Modul1.AutoupD.AsInt());
                            Label4.Width = Width;
                            Label5.Width = Width;
                            Label6.Width = Width;
                            ProjectData.ClearProjectError();
                            num3 = 3;
                            BackColor = Modul1.HintFarb;
                            array = new string[11];
                            Label4.Text = Modul1.VersionT;
                            Label5.Text = Modul1.Version1;
                            Label6.Text = Modul1.Version;
                            if (Modul1.System.VerSpecial == 1)
                            {
                                Label6.Text = "Eingeschränkte Sonderversion";
                            }
                            num4 = 0;
                            FileSystem.FileClose(6);
                            FileSystem.FileClose(99);
                            Modul1.cMandDrive = new DriveInfo(Modul1.Lw);
                            if (IsNotReadOnly)
                            {
                                FileSystem.FileOpen(99, Modul1.InitDir + "Druck_ini.dat", OpenMode.Append);
                                FileSystem.FileClose(99);
                                Modul1.Persistence.ReadStringsInit("Druck_ini.dat", Modul1.Aus, true);
                            }
                            goto IL_0d9d;
                        IL_0d9d: // <========== 3
                            num = 140;
                            FileSystem.FileClose(99);
                            lErl = 10;
                            if (Modul1.Aus[(int)EOutCfg.o45] == "Y")
                            {
                                CheckBox3.CheckState = CheckState.Checked;
                            }
                            if (Modul1.Aus[(int)EOutCfg.o46] == "Y")
                            {
                                CheckBox4.CheckState = CheckState.Checked;
                            }
                            if (Modul1.Aus[(int)EOutCfg.o24] == "1")
                            {
                                CheckBox7.CheckState = CheckState.Checked;
                            }
                            if (Modul1.Aus[(int)EOutCfg.o20] == "Y")
                            {
                                CheckBox8.CheckState = CheckState.Checked;
                            }
                            if (Modul1.Aus[(int)EOutCfg.o17] == "Y")
                            {
                                CheckBox9.CheckState = CheckState.Checked;
                            }
                            if (Modul1.Aus[(int)EOutCfg.o44_PictOrginalSize] == "Y")
                            {
                                CheckBox2.CheckState = CheckState.Checked;
                            }
                            if (Modul1.Aus[(int)EOutCfg.o47] == "Y")
                            {
                                RadioButton32.Checked = true;
                                RadioButton31.Checked = false;
                            }
                            if (Modul1.Aus[(int)EOutCfg.o47] == " ")
                            {
                                RadioButton31.Checked = true;
                                RadioButton32.Checked = false;
                            }
                            if (Modul1.Aus[(int)EOutCfg.o48] == "Y")
                            {
                                CheckBox5.CheckState = CheckState.Checked;
                            }
                            DataModul.DB_WDTable.MoveFirst();
                            if (DataModul.DB_WDTable.Fields[WDFields.Nr].AsInt() == 1)
                            {
                                CheckBox10.CheckState = CheckState.Checked;
                            }
                            Button3.Visible = false;
                            if (Strings.InStr(Modul1.Aus[(int)EOutCfg.o07_KeepFormat].ToUpper(), "WINWORD.EXE") == 0)
                            {
                                if (Modul1.OfficeAppInstalled(IModul1.MSOfficeComponent.MSWord))
                                {
                                    Button3.Visible = true;
                                }
                            }
                            Modul1.UbgT = Modul1.Aus[(int)EOutCfg.o07_KeepFormat];
                            num4 = 1;
                            goto IL_10c3;
                        IL_10c3: // <========== 3
                            num = 185;
                            byte b = (byte)Strings.InStr(Modul1.UbgT, "\\");
                            if (b > 0)
                            {
                                Modul1.UbgT = Strings.Trim(Strings.Mid(Modul1.UbgT, unchecked(b) + 1, Modul1.UbgT.Length));
                                num4 = (short)unchecked(num4 + 1);
                                num8 = num4;
                                num6 = 10;
                                if (num8 <= num6)
                                {
                                    goto IL_10c3;
                                }
                                else
                                {
                                    goto IL_1169;
                                }
                            }
                            else
                            {
                                Label2.Text = Modul1.IText[EUserText.t217] + " = " + Modul1.UbgT;
                                goto IL_1169;
                            }
                        IL_1169: // <========== 3
                            num = 194;
                            foreach (var (control, eOutCfg, defaultValue) in _OutCfg)
                            {
                                if (control is TextBox && Modul1.Aus[(int)eOutCfg] == "")
                                {
                                    Modul1.Aus[(int)eOutCfg] = defaultValue;
                                }
                                if (control is TextBox tb)
                                {
                                    tb.Text = Modul1.Aus[(int)eOutCfg];
                                }
                                else if (control is CheckBox cb)
                                {
                                    cb.CheckState = Modul1.Aus[(int)eOutCfg] == defaultValue ? CheckState.Checked : CheckState.Unchecked;
                                }
                                else if (control is RadioButton rb)
                                {
                                    rb.Checked = Modul1.Aus[(int)eOutCfg] == defaultValue;
                                }
                            }

                            if (Modul1.Aus[(int)EOutCfg.o24] != "1")
                            {
                                Modul1.Aus[(int)EOutCfg.o24] = "0";
                            }
                            if (Modul1.Aus[(int)EOutCfg.o26] == "")
                            {
                                Modul1.Aus[(int)EOutCfg.o26] = true.AsString();
                            }
                            if (Modul1.Aus[(int)EOutCfg.o26].AsBool())
                            {
                                Option1.Checked = true;
                            }
                            else
                            {
                                Option2.Checked = true;
                            }
                            FileSystem.FileClose(99);
                            if (IsNotReadOnly)
                            {
                                FileSystem.FileOpen(99, Modul1.InitDir + "Druck_ini.dat", OpenMode.Output);
                                num4 = 1;
                                while (num4 <= 50)
                                {
                                    FileSystem.PrintLine(99, Modul1.Aus[num4]);
                                    num4 = (short)unchecked(num4 + 1);
                                }
                                FileSystem.FileClose(99);
                            }
                            goto IL_1546;
                        IL_1546:
                            ProjectData.ClearProjectError();
                            num3 = 4;
                            FileSystem.FileClose(6);
                            goto end_IL_0001_2;
                        IL_15d6:
                            num = 252;
                            if (Information.Err().Number == 62)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_16a2;
                            }
                            else
                            {
                                _ = Interaction.MsgBox(Conversion.ErrorToString(), title: Information.Err().Number.ToString(), mb: MessageBoxButtons.OK);
                            }
                            goto IL_1647;
                        IL_1647: // <========== 3
                            num = 258;
                            Modul1.Kont1[0] = "";
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_16a2;
                        IL_16a2: // <========== 4
                            while (true)
                            {
                                int num7 = unchecked(num2 + 1);
                                num2 = 0;
                                switch (num7)
                                {
                                    case 1:
                                        break;
                                    case 131:
                                    case 132:
                                    case 138:
                                    case 135:
                                    case 139:
                                    case 140:
                                        goto IL_0d9d;
                                    case 185:
                                        goto IL_10c3;
                                    case 191:
                                    case 194:
                                        goto IL_1169;
                                    case 242:
                                    case 243:
                                        goto IL_1546;
                                    case 248:
                                    case 249:
                                        num = 249;
                                        _ = Interaction.MsgBox(Conversion.ErrorToString());
                                        ProjectData.ClearProjectError();
                                        if (num2 == 0)
                                        {
                                            throw ProjectData.CreateProjectError(-2146828268);
                                        }
                                        continue;
                                    case 258:
                                        goto IL_1647;
                                    case 260:
                                    case 261:
                                        num = 261;
                                        _ = Interaction.MsgBox(Conversion.ErrorToString());
                                        Debugger.Break();
                                        goto end_IL_0001_2;
                                    case 245:
                                    case 263:
                                        goto end_IL_0001_2;
                                }
                                break;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 6864;
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
    private void Button13_Click(object sender, EventArgs e)
    {
        ColorDialog colorDialog = new ColorDialog();
        colorDialog.AllowFullOpen = true;
        colorDialog.ShowHelp = true;
        colorDialog.Color = BackColor;
        if (colorDialog.ShowDialog() == DialogResult.OK)
        {
            Modul1.ErFarb = colorDialog.Color;
        }
        Modul1.Persistence.PutColorInit("Farb.dat", Modul1.ErFarb, 3);
    }

    private void Button15_Click(object sender, EventArgs e)
    {
        Modul1.HintFarb = ColorTranslator.FromOle(0xC0C0C9);
        Modul1.ErFarb = ColorTranslator.FromOle(0xC0C0C9);
        Modul1.Feld1Farb = ColorTranslator.FromOle(0xFFFFFF);
        Modul1.Persistence.PutColorsInit("Farb.dat", [Modul1.HintFarb, Modul1.Feld1Farb, Modul1.ErFarb]);
        Frame4.BackColor = Modul1.HintFarb;
        BackColor = Modul1.HintFarb;
        TextBox1.BackColor = Modul1.Feld1Farb;
    }

    private void Button5_Click(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_0544
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
                    int M1_Iter = default;
                    int num4;
                    switch (try0001_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0008;
                        case 1848:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_05c8;
                                    default:
                                        goto end_IL_0001;
                                }
                                if (Information.Err().Number == 3010)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_05c8;
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
                                    goto IL_05cb;
                                }
                            }
                        end_IL_0001:
                            break;
                        IL_0008:
                            num = 2;
                            if (IsNotReadOnly)
                            {
                                DataModul.DB_WDTable.MoveFirst();
                                DataModul.DB_WDTable.Edit();
                                DataModul.DB_WDTable.Fields[WDFields.Nr].Value = CheckBox10.Checked ? 1 : (object)0;
                                DataModul.DB_WDTable.Update();
                            }

                            foreach (var (c, eO, d) in _OutCfg)
                                if (c is TextBox tb)
                                    Modul1.Aus[(int)eO] = tb.Text;
                                else if (c is CheckBox cb)
                                    Modul1.Aus[(int)eO] = cb.Checked ? d : " ";
                                else if (c is RadioButton rb)
                                    Modul1.Aus[(int)eO] = rb.Checked ? d : " ";

                            FileSystem.FileClose(99);
                            if (IsNotReadOnly)
                            {
                                FileSystem.FileOpen(99, Modul1.InitDir + "Druck_ini.dat", OpenMode.Output);
                                M1_Iter = 1;
                                if (M1_Iter <= 50)
                                {
                                    FileSystem.PrintLine(99, Modul1.Aus[M1_Iter]);
                                    M1_Iter++;
                                }
                                FileSystem.FileClose(99);
                                FileSystem.FileOpen(99, Modul1.InitDir + "Update_ini.dat", OpenMode.Output);
                                FileSystem.PrintLine(99, CheckBox1.CheckState);
                            }
                            goto IL_04f7;
                        IL_04f7:
                            num = 75;
                            FileSystem.FileClose(99);
                            Frame4.Visible = false;
                            Close();
                            Menue.Default.Show();
                            goto end_IL_0001_2;
                        IL_05c8:
                            num4 = unchecked(num2 + 1);
                            goto IL_05cb;
                        IL_05cb:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 74:
                                case 75:
                                    goto IL_04f7;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0001_dispatch = 1848;
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
    private void Button9_Click(object sender, EventArgs e)
    {
        Close();
        Menue.Default.Show();
    }

    private void Button6_Click(object sender, EventArgs e)
    {
        Button7.Visible = false;
        Button8.Visible = false;
        Button7.Refresh();
        Button8.Refresh();
        Label53.Refresh();
        checked
        {
            if (Label53.Tag.AsInt() == 0)
            {
                List1.Items.Clear();
                List1.Refresh();
                RTB.Text = "";
                Modul1.LfNR = 0;
                DataModul.DB_PersonTable.Index = nameof(PersonIndex.PerNr);
                DataModul.DB_PersonTable.MoveLast();
                long num = DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt().AsLong();
                DataModul.DB_PersonTable.MoveFirst();
                Label16.Text = "Fehler: " + Modul1.LfNR.AsString();
                Label16.Refresh();
                int num2 = (int)num;
                int M1_Iter = 1;
                while (true)
                {
                    int i = M1_Iter;
                    int num3 = num2;
                    if (i > num3)
                    {
                        break;
                    }
                    Modul1.PersInArb = M1_Iter;
                    Application.DoEvents();
                    Label17.Text = $"{Modul1.IText[EUserText.t201]}{M1_Iter} von{num.AsString()}";
                    Label17.Refresh();
                    Modul1.DatPruef(1);
                    Modul1.Ubg = 0;
                    string text = "";


                    foreach (var link in DataModul.Link.ReadAllPers(Modul1.PersInArb, ELinkKennz.lkChild))
                    {
                        text += $"{link.iFamNr,10}";
                    }

                    if (text.Length > 10)
                    {
                        Modul1.LfNR++;
                        Button7.Visible = true;
                        Button8.Visible = true;
                        RTB.SelectedText = "Verknüpfungsfehler bei Person:" + Modul1.PersInArb.AsString() + "\n";
                        _ = List1.Items.Add(new ListItem("Verknüpfungsfehler bei Person:" + Modul1.PersInArb.AsString(), Modul1.PersInArb));
                        Label16.Text = "Fehler: " + Modul1.LfNR.AsString();
                        Label16.Refresh();
                    }

                    foreach (var link in DataModul.Link.ReadAllPers(Modul1.PersInArb, ELinkKennz.lkGodparent))
                    {
                        DataModul.Event.PersonDat(Modul1.PersInArb, out var down, out var up);
                        int num4 = link.iFamNr;
                        DateTime num5 = DataModul.Event.GetDate(EEventArt.eA_Baptism, num4);
                        if (num5 == default)
                            num5 = DataModul.Event.GetDate(EEventArt.eA_Birth, num4);

                        if (num5 != default && down != default && num5 < down)
                        {
                            Modul1.LfNR++;
                            Label16.Text = "Fehler: " + Modul1.LfNR.AsString();
                            RTB.SelectedText = "Patenschaftsproblem (Geb.) bei Person:" + Modul1.PersInArb.AsString() + " zu " + num4.AsString() + "\n";
                            _ = List1.Items.Add(new ListItem("Patenschaftsproblem (Geb.) bei Person:" + Modul1.PersInArb.AsString() + " zu " + num4.AsString(), Modul1.PersInArb));
                        }
                        if (num5 != default && up != default && num5 > up)
                        {
                            Modul1.LfNR++;
                            Label16.Text = "Fehler: " + Modul1.LfNR.AsString();
                            RTB.SelectedText = "Patenschaftsproblem (Tod) bei Person:" + Modul1.PersInArb.AsString() + " zu " + num4.AsString() + "\n";
                            _ = List1.Items.Add(new ListItem("Patenschaftsproblem (Tod) bei Person:" + Modul1.PersInArb.AsString() + " zu " + num4.AsString(), Modul1.PersInArb));
                        }
                    }
                    if (DataModul.DB_WitnessTable.RecordCount > 0)
                    {
                        DataModul.DB_WitnessTable.MoveFirst();
                        DataModul.DB_WitnessTable.Index = nameof(WitnessIndex.ElSu);
                        DataModul.DB_WitnessTable.Seek("=", Modul1.PersInArb.AsString(), "10");
                        for (; !DataModul.DB_WitnessTable.EOF
                            && !DataModul.DB_WitnessTable.NoMatch
                            && DataModul.DB_WitnessTable.Fields[WitnessFields.PerNr].AsInt() == Modul1.PersInArb
                            && DataModul.DB_WitnessTable.Fields[WitnessFields.Kennz].AsString() == "10";
                            DataModul.DB_WitnessTable.MoveNext())
                        {
                            EEventArt Witness_eArt = DataModul.DB_WitnessTable.Fields[WitnessFields.Art].AsEnum<EEventArt>();
                            int Witness_iFamNr = DataModul.DB_WitnessTable.Fields[WitnessFields.FamNr].AsInt();
                            int Witness_iLfNr = DataModul.DB_WitnessTable.Fields[WitnessFields.LfNr].AsInt();
                            DateTime down2;
                            DateTime up2;
                            DataModul.Event.PersonDat(Modul1.PersInArb, out down2, out up2);
                            DateTime num6 = DataModul.Event.GetDate(Witness_eArt, Witness_iFamNr);
                            if (num6 == default)
                            {
                                continue;
                            }
                            int num7 = Witness_iFamNr;
                            if (num6 != default
                                && down2 != default
                                && num6 < down2
                                && DataModul.Event.GetDateB(Witness_eArt, Witness_iFamNr) is DateTime d
                                && (d == default || (num6 = d) <= down2))
                            {
                                Modul1.LfNR++;
                                Label16.Text = "Fehler: " + Modul1.LfNR.AsString();
                                if (Witness_eArt > EEventArt.eA_499)
                                {
                                    RTB.SelectedText = "Zeugenproblem (Geb.) bei Person:" + Modul1.PersInArb.AsString() + " zu F" + num7.AsString() + "\n";
                                    _ = List1.Items.Add(new ListItem("Zeugenproblem (Geb.) bei Person:" + Modul1.PersInArb.AsString() + " zu F" + num7.AsString(), Modul1.PersInArb));
                                }
                                else
                                {
                                    RTB.SelectedText = "Zeugenproblem (Geb.) bei Person:" + Modul1.PersInArb.AsString() + " zu " + num7.AsString() + "\n";
                                    _ = List1.Items.Add(new ListItem("Zeugenproblem (Geb.) bei Person:" + Modul1.PersInArb.AsString() + " zu " + num7.AsString(), Modul1.PersInArb));
                                }
                            }
                            if (unchecked(num6 != default && up2 != default) && num6 > up2)
                            {
                                Modul1.LfNR++;
                                Label16.Text = "Fehler: " + Modul1.LfNR.AsString();
                                if (Witness_eArt > EEventArt.eA_499)
                                {
                                    RTB.SelectedText = "Zeugenproblem (Tod) bei Person:" + Modul1.PersInArb.AsString() + " zu F" + num7.AsString() + "\n";
                                    _ = List1.Items.Add(new ListItem("Zeugenproblem (Tod) bei Person:" + Modul1.PersInArb.AsString() + " zu F" + num7.AsString(), Modul1.PersInArb));
                                }
                                else
                                {
                                    RTB.SelectedText = "Zeugenproblem (Tod) bei Person:" + Modul1.PersInArb.AsString() + " zu " + num7.AsString() + "\n";
                                    _ = List1.Items.Add(new ListItem("Zeugenproblem (Tod) bei Person:" + Modul1.PersInArb.AsString() + " zu " + num7.AsString(), Modul1.PersInArb));
                                }
                            }
                        }
                    }
                    M1_Iter++;
                }
            }
            else
            {
                List1.Items.Clear();
                RTB.Text = "";
                CheckFamDates();
            }
            if (List1.Items.Count > 0)
            {
                Button7.Visible = true;
                Button8.Visible = true;
            }
        }
    }

    public void CheckFamDates()
    {
        //Discarded unreachable code: IL_3158, IL_3202, IL_32bb
        int try0001_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        int lErl = default;
        short Ja = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    var M1_Iter = 0;
                    int num5;
                    string[] array3;
                    switch (try0001_dispatch)
                    {
                        default:
                            num = 1;
                            array3 = new string[13];
                            goto IL_000d;
                        case 15327:
                            {
                                num2 = num;
                                switch ((num3 <= -2) ? 1 : num3)
                                {
                                    case 3:
                                    case 4:
                                        break;
                                    case 2:
                                        goto IL_3204;
                                    case 5:
                                        goto IL_32bd;
                                    case 1:
                                        goto IL_33b5;
                                    default:
                                        goto end_IL_0001;
                                }
                                lErl = 100;
                                //   MainProject.Forms.Hinter.RTB.SelectedText = "         Datumsfehler" + Modul1.FamInArb.AsString() + "\n";
                                //   _ = MainProject.Forms.Hinter.List1.List6_Items.Add(new ListItem("         Datumsfehler bei Familie" + Modul1.FamInArb.AsString(), Modul1.FamInArb));
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_33b5;
                            }
                        end_IL_0001:
                            break;
                        IL_000d:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            Modul1_Diff[1] = 10;
                            if (Modul1.Aus[(int)EOutCfg.o09] != "")
                            {
                                Modul1_Diff[1] = Modul1.Aus[(int)EOutCfg.o09].AsInt();
                            }
                            Modul1_Diff[1] = Modul1_Diff[1] * 10000;
                            Modul1_Diff[2] = 20;
                            if (Modul1.Aus[(int)EOutCfg.o08] != "")
                            {
                                Modul1_Diff[2] = Modul1.Aus[(int)EOutCfg.o08].AsInt();
                            }
                            Modul1_Diff[2] = Modul1_Diff[2] * 10000;
                            Modul1_Diff[3] = 20;
                            if (Modul1.Aus[(int)EOutCfg.o10_EmitIDs] != "")
                            {
                                Modul1_Diff[3] = Modul1.Aus[(int)EOutCfg.o10_EmitIDs].AsInt();
                            }
                            Modul1_Diff[3] = Modul1_Diff[3] * 10000;
                            Modul1_Diff[4] = 20;
                            if (Modul1.Aus[(int)EOutCfg.o11] != "")
                            {
                                Modul1_Diff[4] = Modul1.Aus[(int)EOutCfg.o11].AsInt();
                            }
                            Modul1_Diff[4] = Modul1_Diff[4] * 10000;
                            int[] aiFather = new int[5];
                            int[] aiMother = new int[5];
                            int[] array = new int[5];
                            int num7 = 0;
                            Label16.Text = Modul1.IText[EUserText.t205] + " " + num7.AsString();
                            DataModul.DB_FamilyTable.MoveLast();
                            int recordCount = DataModul.DB_FamilyTable.RecordCount;
                            int num8 = 1;
                            while (num8 <= recordCount)
                            {
                                Label17.Text = $"{Modul1.IText[EUserText.t200]} {num8} von {recordCount}";
                                M1_Iter = 0;
                                while (M1_Iter <= 10)
                                {
                                    array3[M1_Iter] = "";
                                    M1_Iter++;
                                }
                                if (Ja == 1)
                                {
                                    num7++;
                                    Label16.Text = Modul1.IText[EUserText.t205] + " " + num7.AsString();
                                    Label16.Refresh();
                                    Button7.Visible = true;
                                    Button8.Visible = true;
                                }
                                Ja = 0;
                                Modul1.FamInArb = num8;
                                IRecordset dB_FamilyTable = DataModul.DB_FamilyTable;
                                dB_FamilyTable.Index = nameof(FamilyIndex.Fam);
                                dB_FamilyTable.Seek("=", Modul1.FamInArb);
                                if (!dB_FamilyTable.NoMatch)
                                    CheckFamily(dB_FamilyTable);
                                lErl = 23;
                                Application.DoEvents();
                                num8++;
                            }
                            goto end_IL_0001_2;
                        IL_3204:
                            num = 503;
                            if (Interaction.MsgBox((string?)("Fehler # " + Information.Err().Number.AsString() + " wurde ausgelöst von " + Information.Err().Source + "\r" + Information.Err().Description), title: "Fehler", mb: MessageBoxButtons.OKCancel, icon: MessageBoxIcon.Question) == DialogResult.Cancel)
                            {
                                ProjectData.EndApp();
                            }
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_33b5;
                        IL_32bd:
                            num = 509;
                            if (Information.Err().Number == 13)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_33b5;
                            }
                            else
                            {
                                if (Interaction.MsgBox((string?)("Fehler # " + Information.Err().Number.AsString() + " wurde ausgelöst von " + Information.Err().Source + "\r" + Information.Err().Description), title: "Fehler", mb: MessageBoxButtons.OKCancel, icon: MessageBoxIcon.Question) == DialogResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num5 = num2;
                                goto IL_33b9;
                            }
                        IL_33b5: // <========== 4
                                 // <========== 4
                            num5 = unchecked(num2 + 1);
                            goto IL_33b9;
                        IL_33b9:
                            num2 = 0;
                            switch (num5)
                            {
                                case 1:
                                    break;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is Exception exception && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(exception, lErl);
                try0001_dispatch = 15327;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        }
    end_IL_0001_2:
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    private void CheckFamily(IRecordset dB_FamilyTable)
    {
        checked
        {
            var array3 = new string[13];
            int[] aiFather = new int[5];
            int[] aiMother = new int[5];
            int[] array = new int[5];
            int Ja = default;

            var M1_Iter = 500;
            int famInArb = Modul1.FamInArb;
            while (M1_Iter <= 507)
            {
                if (DataModul.Event.ReadData((EEventArt)M1_Iter, famInArb, out var cEvt)
                        && cEvt.sVChr == "0"
                        && cEvt.dDatumV != default)
                {
                    if (cEvt.dDatumV.ToString().Left(2).AsInt() > 0)
                    {
                        ; // 0 = kein Datum
                    }
                }
                M1_Iter++;
            }
            Label17.Refresh();

            ReadFamily(famInArb, Modul1.Family);
            array3[1] = $"{Modul1.IText[EUserText.t174]} {famInArb}/{"Mann"} {Modul1.Family.Mann}/{"Frau"} {Modul1.Family.Frau}";

            if (dB_FamilyTable.Fields[FamilyFields.Prüfen].AsString() == "N")
            {
                M1_Iter = 500;
                while (M1_Iter <= 507
                    )
                {
                    DataModul.Event.DeleteEmptyFam(famInArb, (EEventArt)M1_Iter);
                    M1_Iter++;
                }
                if (Modul1.Aus[(int)EOutCfg.o45] != "Y")
                {
                    AppendText($"{array3[1]} {Modul1.IText[EUserText.t208]}", famInArb);
                }
            }
            else
            {
                void AppendTextExt(string sText)
                {
                    if (Ja == 0)
                    {
                        AppenTitle(array3[1], famInArb);
                        Ja = 1;
                    }
                    AppendText(sText, famInArb, 4);
                }

                DataModul.Family_CheckParent(Modul1.Family.Mann, Modul1.IText[EUserText.t162], "M", dB_FamilyTable, AppendTextExt);

                Person_Dates(Modul1.Family.Mann, aiFather);

                DataModul.Family_CheckParent(Modul1.Family.Frau, Modul1.IText[EUserText.t163], "F", dB_FamilyTable, AppendTextExt);

                Person_Dates(Modul1.Family.Frau, aiFather);

                float num12 = aiFather[1] - aiMother[1];
                if ((aiMother[1] > 0) & (aiFather[1] > 0))
                {
                    if ((num12 > Modul1_Diff[1]) | (num12 < -Modul1_Diff[1]))
                    {
                        array3[2] = Modul1.IText[EUserText.t209] + " " + Strings.Mid(Conversion.Int(num12 / 10000f).AsString(), 2, 10) + Modul1.IText[EUserText.t216] + " !";
                    }
                }
                var num13 = 0;
                while (num13 <= 7)
                {
                    Modul1.Kont1[num13] = "";
                    num13 += 1;
                }

                M1_Iter = 500;
                while (M1_Iter <= 507)
                {
                    DateTime dt;
                    if (IsNotReadOnly)
                    {
                        if (!DataModul.Event.DeleteEmptyFam(famInArb, (EEventArt)M1_Iter))
                        {
                            if (DataModul.Family.Get_Aeb(famInArb))
                            {
                                dB_FamilyTable.Edit();
                                dB_FamilyTable.Fields[FamilyFields.Aeb].Value = 0;
                                dB_FamilyTable.Update();
                            }
                        }
                    }
                    else if ((dt = DataModul.Event.GetDate((EEventArt)M1_Iter, famInArb)) != default)
                    {
                        Modul1.Kont1[M1_Iter - 500] = dt.ToString("yyyyMMdd");
                    }
                    M1_Iter++;
                }
                var num6 = Modul1.Kont1[2].AsInt();
                if (num6 == 0)
                {
                    num6 = Modul1.Kont1[3].AsInt();
                }
                if (num6 == 0)
                {
                    num6 = Modul1.Kont1[1].AsInt();
                }
                if (num6 == 0)
                {
                    num6 = Modul1.Kont1[0].AsInt();
                }
                if (num6 == 0)
                {
                    num6 = Modul1.Kont1[5].AsInt();
                }
                if ((num6 > 0) & (Modul1.Kont1[3].AsInt() > 0.0))
                {
                    if (num6 > Modul1.Kont1[3].AsInt())
                    {
                        num6 = Modul1.Kont1[3].AsInt();
                    }
                }
                if (num6 > 0)
                {
                    if (num6 - Modul1_Diff[2] < aiFather[1])
                    {
                        var num16 = (float)Conversion.Int(num6 / 10000.0);
                        array3[3] = Modul1.IText[EUserText.t162] + Modul1.IText[EUserText.t210] + (Conversion.Int(num6 / 10000.0) - Conversion.Int(aiFather[1] / 10000.0)).AsString() + Modul1.IText[EUserText.t216] + " !";
                    }
                    if (num6 - Modul1_Diff[3] < aiMother[1])
                    {
                        array3[4] = Modul1.IText[EUserText.t163] + Modul1.IText[EUserText.t210] + (Conversion.Int(num6 / 10000.0) - Conversion.Int(aiMother[1] / 10000.0)).AsString() + Modul1.IText[EUserText.t216] + " !";
                    }
                    if ((num6 > aiFather[3]) & (aiFather[3] > 0))
                    {
                        array3[5] = Modul1.IText[EUserText.t162] + Modul1.IText[EUserText.t211];
                    }
                    if ((num6 > aiMother[3]) & (aiMother[3] > 0))
                    {
                        array3[6] = Modul1.IText[EUserText.t163] + Modul1.IText[EUserText.t211];
                    }
                }
                var DDatum = "";
                var num17 = ListBox1.Items.Count - 1;
                var num4 = 0;
                while (num4 <= num17)
                {
                    Modul1.PersInArb = ListBox1.Items.ItemData(num4).AsInt();
                    if (Modul1.PersInArb != 0)
                    {
                        _ = DataModul.Event.GetPersonDates(Modul1.PersInArb, out var VCHR).IntoString(Modul1.Kont1);
                        if (!VCHR)
                        {
                            var num13i = 1;
                            while (num13i <= 4)
                            {
                                array[num13i] = Modul1.Kont1[num13i].AsInt();
                                num13i += 1;
                            }
                            if (array[1] == 0)
                            {
                                array[1] = array[2];
                            }
                            if (array[1] == 0)
                            {
                                array[1] = array[2];
                            }
                            var HT = "";
                            if (num4 > 0f)
                            {
                                HT = DDatum.Date2DotDateStr2();
                                DDatum = HT;
                                var DDatum2 = array[1].AsString().Trim();
                                HT = DDatum2.Date2DotDateStr2();
                                if (HT.Left(2) == "00")
                                {
                                    StringType.MidStmtStr(ref HT, 1, 2, "01");
                                }
                                if (Strings.Mid(HT, 4, 2) == "00")
                                {
                                    StringType.MidStmtStr(ref HT, 4, 2, "01");
                                }
                                DDatum2 = array[1].AsString().Trim();
                                HT = DDatum2.Date2DotDateStr2();
                                if (HT.Left(2) == "00")
                                {
                                    StringType.MidStmtStr(ref HT, 1, 2, "01");
                                }
                                if (Strings.Mid(HT, 4, 2) == "00")
                                {
                                    StringType.MidStmtStr(ref HT, 4, 2, "01");
                                }
                                if ((DDatum.AsInt() > 0.0) & (HT.AsInt() > 0.0))
                                {
                                    ProjectData.ClearProjectError();
                                    if (DDatum.Left(2) == "00")
                                    {
                                        StringType.MidStmtStr(ref DDatum, 1, 2, "01");
                                    }
                                    if (Strings.Mid(DDatum, 4, 2) == "00")
                                    {
                                        StringType.MidStmtStr(ref DDatum, 4, 2, "01");
                                    }
                                    if (((DDatum.AsDate() - HT.AsDate()).TotalDays > 3)
                                        & ((DDatum.AsDate() - HT.AsDate()).TotalDays < 8 * 30.25))
                                    {
                                        if (Ja == 0)
                                        {
                                            AppenTitle(array3[1], famInArb);
                                            Ja = 1;
                                        }
                                        AppendText($"Kind {num4} nur {(DDatum.AsDate() - HT.AsDate()).TotalDays} Tage vor Kind {num4 + 1} geboren.\n", famInArb, 4);
                                    }
                                }
                            }
                            if (array[3] == 0)
                            {
                                array[3] = array[4];
                            }
                            if (unchecked(array[1] > 0 && num6 > 0))
                            {
                                if (array[1] < num6 - (Modul1.Aus[(int)EOutCfg.o23].AsDouble() + 1.0) * 10000.0)
                                {
                                    var num16 = array[1].AsString().Length == 7
                                        ? (float)Conversion.Val(array[1].AsString().Left(4))
                                        : (float)Conversion.Val(array[1].AsString().Left(5));
                                    var num22 = num6 < 10000000
                                        ? (float)Conversion.Val(Conversion.Str(num6.AsString().Left(3).AsDouble()))
                                        : (float)Conversion.Val(Conversion.Str(num6.AsString().Left(4).AsDouble()));
                                    if (Ja == 0)
                                    {
                                        AppenTitle(array3[1], famInArb);
                                        Ja = 1;
                                    }

                                    AppendText($"   {Modul1.IText[EUserText.tChild_AS]}{num22 - num16}{Modul1.IText[EUserText.t214]}", famInArb, 4);
                                }
                            }
                            if ((array[1] > 0) && (aiFather[1] > 0) && array[1] - 10000 < aiFather[1])
                            {
                                if (Ja == 0)
                                {
                                    AppenTitle(array3[1], famInArb);
                                    Ja = 1;
                                }
                                AppendText($"Kind {num4 + 1} vor {Modul1.IText[EUserText.t162]} geboren !", famInArb, 4);
                            }
                            if ((array[1] > 0) && (aiFather[3] > 0) && array[1] - 10000 > aiFather[3])
                            {
                                if (Ja == 0)
                                {
                                    AppenTitle(array3[1], famInArb);
                                    Ja = 1;
                                }
                                AppendText(Modul1.IText[EUserText.t212], famInArb, 4);
                            }
                            if ((array[1] > 0) && (aiMother[1] > 0) && array[1] - 10000 < aiMother[1])
                            {
                                if (Ja == 0)
                                {
                                    AppenTitle(array3[1], famInArb);
                                    Ja = 1;
                                }
                                AppendText($"Kind {num4 + 1} vor {Modul1.IText[EUserText.t163]} geboren ", famInArb, 4);
                            }
                            if ((array[1] > 0) && (aiMother[3] > 0) && array[1] > aiMother[3])
                            {
                                if (Ja == 0)
                                {
                                    AppenTitle(array3[1], famInArb);
                                    Ja = 1;
                                }
                                AppendText(Modul1.IText[EUserText.t213], famInArb, 4);
                            }
                            if ((array[1] > 0) && (aiMother[1] > 0) && array[1] > aiMother[1] + Modul1_Diff[4])
                            {
                                if (Ja == 0)
                                {
                                    AppenTitle(array3[1], famInArb);
                                    Ja = 1;
                                }
                                AppendText($"{Modul1.IText[EUserText.t215]}{(array[1] / 10000.0).AsInt() - (aiMother[1] / 10000.0).AsInt()}{Modul1.IText[EUserText.t216]} ", famInArb, 4);
                            }
                            DDatum = HT;
                            DDatum = array[1].AsString().Trim();

                        }
                        else
                            break;
                    }
                    num4++;
                }
                if (num4 <= num17)
                {
                    if (Ja == 0)
                    {
                        var num19 = 2;
                        while (num19 <= 10 && array3[num19] == "")
                        {
                            num19++;
                        }
                        if (num19 <= 10)
                        {
                            AppenTitle(array3[1], famInArb);
                            Ja = 1;
                        }
                    }
                    if (array3[2] != "")
                    {
                        RTB.SelectedText = "   " + array3[2] + "\n";
                        _ = List1.Items.Add(new ListItem("   " + array3[2], dB_FamilyTable.Fields[FamilyFields.FamNr].AsInt()));
                    }
                    if (array3[3] != "")
                    {
                        RTB.SelectedText = "   " + array3[3] + "\n";
                        _ = List1.Items.Add(new ListItem("   " + array3[3], dB_FamilyTable.Fields[FamilyFields.FamNr].AsInt()));
                    }
                    if (array3[4] != "")
                    {
                        RTB.SelectedText = "   " + array3[4] + "\n";
                        _ = List1.Items.Add(new ListItem("   " + array3[4], dB_FamilyTable.Fields[FamilyFields.FamNr].AsInt()));
                    }
                    if (array3[5] != "")
                    {
                        RTB.SelectedText = "   " + array3[5] + "\n";
                        _ = List1.Items.Add(new ListItem("   " + array3[5], dB_FamilyTable.Fields[FamilyFields.FamNr].AsInt()));
                    }
                    if (array3[6] != "")
                    {
                        RTB.SelectedText = "   " + array3[6] + "\n";
                        _ = List1.Items.Add(new ListItem("   " + array3[6], dB_FamilyTable.Fields[FamilyFields.FamNr].AsInt()));
                    }

                }
            }
            Modul1.FamInArb = famInArb;
        }
    }

    private static void Person_Dates(int iPerson, int[] aiFather)
    {
        var ad = DataModul.Event.GetPersonDates(iPerson, out var xVCHR);
        if (!xVCHR)
        {
            var M1_Iter = 1;
            while (M1_Iter <= 4)
            {
                aiFather[M1_Iter] = ad[M1_Iter].AsInt();
                if (M1_Iter > 2)
                {
                    if (ad[M1_Iter].Year == 0)
                    {
                        aiFather[M1_Iter] = aiFather[M1_Iter] + 10000;
                    }
                }
                M1_Iter++;
            }
            if (aiFather[1] == 0)
            {
                aiFather[1] = aiFather[2];
            }
            if (aiFather[3] == 0)
            {
                aiFather[3] = aiFather[4];
            }
        }
    }


    private void Button7_Click(object sender, EventArgs e)
    {
        if (IsNotReadOnly)
        {
            RTB.SaveFile(Modul1.TempPath + "\\Text3.RTF", RichTextBoxStreamType.RichText);
            RTB.LoadFile(Modul1.TempPath + "\\Text3.RTF", RichTextBoxStreamType.RichText);
        }
        else
        {
            RTB.SaveFile(Modul1.TempPath + "\\Text3.RTF", RichTextBoxStreamType.RichText);
            RTB.LoadFile(Modul1.TempPath + "\\Text3.RTF", RichTextBoxStreamType.RichText);
            RTB.SaveFile(Modul1.GenFreeDir + "\\Temp\\Text3.RTF", RichTextBoxStreamType.RichText);
            RTB.LoadFile(Modul1.GenFreeDir + "\\Temp\\Text3.RTF", RichTextBoxStreamType.RichText);
        }
        Modul1.Ausdruck("\\Text3.RTF");
    }

    private void Button8_Click(object sender, EventArgs e)
    {
        Modul1.Lw = FileSystem.CurDir().Left(1);
        CommonDialog1Save.Filter = "Text (*.txt)|*.txt|Formartierter Text (*.RTF)|*.RTF";
        CommonDialog1Save.InitialDirectory = Modul1.GenFreeDir + "\\list\\";
        CommonDialog1Save.FilterIndex = 2;
        _ = CommonDialog1Save.ShowDialog();
        if (CommonDialog1Save.FileName != "")
        {
            switch (CommonDialog1Save.FilterIndex)
            {
                case 1:
                    RTB.SaveFile(CommonDialog1Save.FileName, RichTextBoxStreamType.PlainText);
                    break;
                case 2:
                    RTB.SaveFile(CommonDialog1Save.FileName, RichTextBoxStreamType.RichText);
                    break;
            }
        }
        FileSystem.ChDrive(Modul1.Lw);
    }

    private void List1_DoubleClick(object sender, EventArgs e)
    {
        if (Label53.Text == Modul1.IText[EUserText.t198])
        {
            Familie.Default.btnMainmenue.Text = Modul1.IText[EUserText.tNMBack];
            Modul1.Ad = true;
            Modul1.PersInArb = List1.Items.ItemData<int>(List1.SelectedIndex);
            Personen.Default.Show(Modul1.PersInArb, EUserText.tNMBack);
            Modul1.Aend = 0f;
        }
        else
        {
            Modul1.FamInArb = List1.Items.ItemData<int>(List1.SelectedIndex);
            Familie.Default.Show();
            Familie.Default.btnMainmenue.Text = Modul1.IText[EUserText.tNMBack];
            short Rich;
            Familie.Default.Fameinlesen(Modul1.FamInArb, out Rich);
        }
    }

    private void Button16_Click(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_002a
        string text = Modul1.GoogleInstallPath();
        _ = text.Length > 0
            ? Interaction.MsgBox("Google Earth ist installiert: Pfad " + text)
            : Interaction.MsgBox("Google Earth ist leider nicht installiert");
    }

    private void Button10_Click_1(object sender, EventArgs e)
    {
        int num = 0;
        checked
        {
            int num2;
            if (DataModul.DB_PersonTable.RecordCount > 0)
            {
                DataModul.DB_PersonTable.Index = nameof(PersonIndex.PerNr);
                DataModul.DB_PersonTable.MoveLast();
                num2 = DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt();
                DataModul.DB_PersonTable.MoveFirst();

                DataModul.DB_LeerTable.Close();
                DataModul.MandDB.TryExecute("DROP Table Leer1");
                ProgressBar1.Minimum = 0;
                ProgressBar1.Maximum = 0;
                Modul1.Dateienopen();
                ProgressBar1.Step = 1;
                ProgressBar1.Maximum = DataModul.DB_PersonTable.RecordCount;
                int num4 = 1;
                while (num4 >= num2)
                {
                    ProgressBar1.PerformStep();
                    Modul1.PersInArb = num4;
                    if (DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt() != num4)
                    {
                        DataModul.LeerTab_AddRaw(num4, "P");
                        num++;
                    }
                    else
                    {
                        DataModul.DB_PersonTable.MoveNext();
                    }
                    num4++;
                }
                Label3.Visible = true;
                Label3.Text = num.AsString() + " Personen";
            }
            DataModul.DB_FamilyTable.Index = nameof(FamilyIndex.Fam);
            DataModul.DB_FamilyTable.MoveLast();
            num2 = DataModul.DB_FamilyTable.Fields[FamilyFields.FamNr].AsInt();
            num = 0;
            if (DataModul.DB_FamilyTable.RecordCount > 0)
            {
                DataModul.DB_FamilyTable.MoveFirst();
                ProgressBar1.Minimum = 0;
                ProgressBar1.Maximum = 0;
                ProgressBar1.Step = 1;
                ProgressBar1.Maximum = DataModul.DB_FamilyTable.RecordCount;
                string inputStr = "";
                int num7 = (int)num2;
                int num4 = 1;
                while (true)
                {
                    int num8 = num4;
                    int num6 = num7;
                    if (num8 > num6)
                    {
                        break;
                    }
                    byte b = 0;
                    ProgressBar1.PerformStep();
                    Modul1.FamInArb = num4;
                    if (DataModul.DB_FamilyTable.Fields[FamilyFields.FamNr].AsInt() != num4)
                    {
                        if (!((DataModul.DB_FamilyTable.Fields[FamilyFields.FamNr].AsInt() < num4)
                            & DataModul.DB_FamilyTable.Fields[FamilyFields.FamNr].AsInt() == inputStr.AsInt()))
                        {
                            if (Modul1.FamInArb == 277)
                            {
                                Debugger.Break();
                            }
                            ELinkKennz b2 = ELinkKennz.lkFather;
                            do
                            {
                                if (DataModul.Link.ExistF(Modul1.FamInArb, b2))
                                {
                                    b = (byte)(unchecked(b) + 1);
                                    if (b == 3)
                                    {
                                        DataModul.LeerTab_AddRaw(num4, "F");
                                        num++;
                                        EEventArt num9 = EEventArt.eA_500;
                                        while (num9 <= EEventArt.eA_510)
                                        {
                                            DataModul.Event.DeleteBeSu(num9, Modul1.FamInArb);
                                            num9++;
                                        }
                                        num9 = EEventArt.eA_601;
                                        while (num9 <= EEventArt.eA_603)
                                        {
                                            DataModul.Event.DeleteBeSu(num9, Modul1.FamInArb);
                                            num9++;
                                        }


                                        int num9a = 1;
                                        foreach (var link in DataModul.Link.ReadAllFams(Modul1.FamInArb))
                                        {
                                            if (link.eKennz != ELinkKennz.lkGodparent && link.eKennz != ELinkKennz.lk9)
                                            {
                                                link.Delete();
                                            }
                                            if (num9a++ > 100)
                                            {
                                                break;
                                            }
                                        }
                                        while (!DataModul.DB_SourceLinkTable.EOF)
                                        {
                                            DataModul.DB_SourceLinkTable.Index = "Tab";
                                            DataModul.DB_SourceLinkTable.Seek("=", "2", Modul1.FamInArb);
                                            if (DataModul.DB_SourceLinkTable.NoMatch || Conversions.ToBoolean(Operators.OrObject(DataModul.DB_SourceLinkTable.Fields[0].AsInt() != 2,
                                                DataModul.DB_SourceLinkTable.Fields[1].AsInt() > Modul1.FamInArb)))
                                            {
                                                break;
                                            }
                                            DataModul.DB_SourceLinkTable.Delete();
                                        }
                                        DataModul.DB_SourceLinkTable.Index = "Tab";
                                        DataModul.DB_SourceLinkTable.Seek("=", "3", Modul1.FamInArb);
                                        while (!DataModul.DB_SourceLinkTable.EOF && !DataModul.DB_SourceLinkTable.NoMatch && !Conversions.ToBoolean(Operators.OrObject(
                                            DataModul.DB_SourceLinkTable.Fields[0].AsInt() != 3,
                                            DataModul.DB_SourceLinkTable.Fields[1].AsInt() > Modul1.FamInArb)))
                                        {
                                            if (DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Art].AsInt() > 499)
                                            {
                                                DataModul.DB_SourceLinkTable.Delete();
                                            }
                                            DataModul.DB_SourceLinkTable.MoveNext();
                                        }
                                        DataModul.DB_WitnessTable.Index = nameof(WitnessIndex.FamSu);
                                        DataModul.DB_WitnessTable.Seek("=", Modul1.FamInArb, "10");
                                        if (!DataModul.DB_WitnessTable.NoMatch)
                                        {
                                            num9a = 1;
                                            while (!DataModul.DB_WitnessTable.EOF)
                                            {
                                                if (DataModul.DB_WitnessTable.NoMatch)
                                                {
                                                    _ = Interaction.MsgBox("F35");
                                                    Debugger.Break();
                                                }
                                                if (Conversions.ToBoolean(Operators.OrObject(DataModul.DB_WitnessTable.Fields[WitnessFields.FamNr].AsInt() != Modul1.FamInArb, DataModul.DB_WitnessTable.Fields[WitnessFields.Kennz].AsString() != "10")))
                                                {
                                                    break;
                                                }
                                                DataModul.DB_WitnessTable.Delete();
                                                DataModul.DB_WitnessTable.MoveNext();
                                                if (num9a++ > 99)
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    b2++;
                                    continue;
                                }

                                DataModul_Family_AddEmpty(Modul1.FamInArb);
                                break;
                            }
                            while (b2 <= ELinkKennz.lkChild);
                            goto IL_0c92;
                        }
                        num4--;
                        DataModul.DB_FamilyTable.Delete();
                    }
                    else
                    {
                        inputStr = DataModul.DB_FamilyTable.Fields[FamilyFields.FamNr].AsInt().AsString();
                    }
                    DataModul.DB_FamilyTable.MoveNext();
                    goto IL_0c92;
                IL_0c92:
                    num4++;
                }
            }
            Label18.Visible = true;
            Label18.Text = "und " + num.AsString() + " Familien können neu belegt werden";
            _ = Interaction.MsgBox("Fertig");
            ProgressBar1.Minimum = 0;
            ProgressBar1.Maximum = 0;
        }
    }

    private static void DataModul_Family_AddEmpty(int famInArb)
    {
        DataModul.DB_FamilyTable.AddNew();
        DataModul.DB_FamilyTable.Fields[FamilyFields.AnlDatum].Value = DateTime.Today.Year.ToString() + DateTime.Today.Day.ToString() + DateTime.Today.Month.ToString();
        DataModul.DB_FamilyTable.Fields[FamilyFields.EditDat].Value = "0";
        DataModul.DB_FamilyTable.Fields[FamilyFields.Prüfen].Value = "1    ";
        DataModul.DB_FamilyTable.Fields[FamilyFields.FamNr].Value = famInArb;
        DataModul.DB_FamilyTable.Fields[FamilyFields.Name].Value = 0;
        DataModul.DB_FamilyTable.Fields[FamilyFields.Aeb].Value = 0;
        DataModul.DB_FamilyTable.Fields[FamilyFields.Fuid].Value = Guid.NewGuid();
        DataModul.DB_FamilyTable.Fields[FamilyFields.Bem1].Value = " ";
        DataModul.DB_FamilyTable.Update();
    }

    private void CheckBox10_Click(object sender, EventArgs e)
    {
        if (CheckBox10.Checked)
        {
            Button10.PerformClick();
        }
    }

    private void Button2_Click(object sender, EventArgs e)
    {
        if (IsNotReadOnly)
        {
            Modul1.Persistence.ReadEnumsMand("suchfeld.dat", Modul1.Suchfeld);
        }
        fraHntSearchFields1.eSearchSelection1 = Modul1.Suchfeld[0];
        fraHntSearchFields1.eSearchSelection2 = Modul1.Suchfeld[1];
        fraHntSearchFields1.eSearchSelection3 = Modul1.Suchfeld[2];
        fraHntSearchFields1.Top = 0;
        fraHntSearchFields1.Left = 0;
        fraHntSearchFields1.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
        fraHntSearchFields1.Visible = true;
    }

    private void Button18_Click(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_03fa, IL_043c, IL_0444, IL_0445
        ESearchSelection[] array = new ESearchSelection[3];

        Modul1.Persistence.ReadEnumsMand("suchfeld.dat", (IList<ESearchSelection>)array);

        Modul1.Suchfeld[0] = fraHntSearchFields1.eSearchSelection1;
        Modul1.Suchfeld[1] = fraHntSearchFields1.eSearchSelection2;
        Modul1.Suchfeld[2] = fraHntSearchFields1.eSearchSelection3;

        bool flag = false;
        if (array[0] != Modul1.Suchfeld[0])
        {
            flag = true;
        }
        if (array[1] != Modul1.Suchfeld[1])
        {
            flag = true;
        }
        if (array[2] != Modul1.Suchfeld[2])
        {
            flag = true;
        }
        if (flag)
        {
            while (Interaction.MsgBox("Zum Erstellen des neuen Suchbegriffs ist eine Reorganisation erforderlich!\nJetzt reorganisieren?", title: "", mb: MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                if (Interaction.MsgBox("Änderung der Suchkriterien verwerfen?", title: "", mb: MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    continue;
                }
                Modul1.reorga = false;
                Modul1.Suchfeld[0] = array[0];
                Modul1.Suchfeld[1] = array[1];
                Modul1.Suchfeld[2] = array[2];
                fraHntSearchFields1.Show();
                return;
            }
            Modul1.Persistence.PutEnumsMand("suchfeld.dat", Modul1.Suchfeld);
            Modul1.reorga = true;
        }
    }

    private void Panel1_Paint(object sender, PaintEventArgs e)
    {
    }
}
