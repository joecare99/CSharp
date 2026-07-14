using BaseLib.Helper;
using GenFree;
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces.Data;
using GenFree.Interfaces.Sys;
using GenFree.Interfaces.VB;
using GenFree.ViewModels.Interfaces;
using GenFreeWin.Views;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Gen_FreeWin.Views;

public partial class FrmEreignis : Form
{
    [Obsolete]
    IProjectData ProjectData;
    IInteraction Interaction => Menue.Default;
    [Obsolete]
    IVBInformation Information;
    [Obsolete]
    IOperators Operators;
    [Obsolete]
    IVBConversions Conversions;
    [Obsolete]
    IStrings Strings;
    [Obsolete]
    IStrings StringType;

    IModul1 Modul1 => _Modul1.Instance;

    private string Topsch;
    private int Ortnr;

    private EEventArt eEventArt { get; set; }

    private float Schaltsp;

    private float A;

    private int KBem;

    public byte schalt5;
    private Keys Modul1_EingCode;
    private EEventArt Modul1_ErArt;
    private int Modul1_PerfamNr;
    private string Modul1_Event_LetztOrtspei;
    private (string, ETextKennz) Modul1_Bezeichnu;
    private List<string> Modul1_MyList = new();
    private IList<string> Modul1_oResult;
    private object Modul1_priv;

    public List<int> Modul1_Per1 { get; } = new();

    public FrmEreignis()
    {
        FormClosed += Ereignis_FormClosed;
        Load += Ereignis_Load;
        lock (__ENCList)
        {
            __ENCList.Add(new WeakReference(this));
        }
        schalt5 = 0;
        InitializeComponent();
    }


    public new DialogResult ShowDialog()
    {
        return ShowDialog(null);
    }

    private void Ereignis_FormClosed(object sender, FormClosedEventArgs e)
    {
        CheckBox2.Visible = false;
        if (eEventArt > EEventArt.eA_499)
        {
            if (Familie.Default.Visible)
            {
                Modul1.FamInArb = Familie.Default.iFamNr;
                short Rich;
                Familie.Default.Show(Modul1.FamInArb);
            }
        }
        else
        {
            Modul1.PersInArb = 0;
            Modul1.PersInArb = Personen.Default.PersonNr;
            if (Modul1.PersInArb == 0)
            {
                _ = Interaction.MsgBox(Modul1.PersInArb.AsString(), title: "2", mb: MessageBoxButtons.OK);
            }
        }
    }

    private void Ereignis_Load(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_3bf6
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
                    int num4;
                    string Wort;
                    short Schalt;
                    int AAA;
                    int farb = 0;
                    int I = default;
                    switch (try0001_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0009;
                        case 18292:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 3:
                                        break;
                                    case 2:
                                    case 4:
                                        goto IL_3c38;
                                    case 1:
                                        goto IL_3d1a;
                                    default:
                                        goto end_IL_0001;
                                }
                                if (Information.Err().Number == 62)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    num2 = 0;
                                    goto IL_0009;
                                }
                                goto IL_3c38;
                            }
                        end_IL_0001:
                            break;
                        IL_0009:
                            num = 2;
                            //      Modul1.AddContextMenu(RichTextBox1);
                            //      Modul1.AddContextMenu(RichTextBox2);
                            TextBox3.Visible = true;
                            TextBox9.Visible = true;
                            TextBox10.Visible = true;
                            Label15.Visible = true;
                            TextBox16.Visible = true;
                            Label20.Visible = true;
                            if (eEventArt == EEventArt.eA_Death)
                            {
                                CheckBox2.Visible = true;
                                Label21.Visible = true;
                                TextBox17.Visible = true;
                                TextBox19.Visible = true;
                            }
                            else
                            {
                                Label21.Visible = false;
                                TextBox17.Visible = false;
                                TextBox19.Visible = false;
                            }
                            Box1.Visible = eEventArt != EEventArt.eA_601;
                            if (eEventArt == EEventArt.eA_302
                                | eEventArt == EEventArt.eA_602)
                            {
                                Label22.Visible = true;
                                TextBox18.Visible = true;
                            }
                            else
                            {
                                Label22.Visible = false;
                                TextBox18.Visible = false;
                            }
                            TextBox16.Text = "";
                            ProjectData.ClearProjectError();
                            num3 = 3;
                            Modul1_EingCode = default;
                            if (Modul1.FontSize > 0f)
                            {
                                Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                ListBox1.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                ListBox2.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                ListBox3.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                ListBox4.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                Frame1.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                Button12.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                Label9.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                            }
                            if (Modul1.Typ != DriveType.CDRom)
                            {
                                var aiList = Modul1.Persistence.ReadStringsMand("OW.dat", 7);
                                foreach (var item in aiList)
                                {
                                    ListBox3.Items.Add(item);
                                }
                            }
                            Button12.Visible = true;
                            ProjectData.ClearProjectError();
                            num3 = 4;
                            TextBox8.Visible = true;
                            if (Modul1.Typ != DriveType.CDRom)
                            {
                                var afarb = Modul1.Persistence.ReadFarbenInit("Farb.dat", 3);
                                Modul1.Farb = afarb[2];

                                BackColor = Modul1.Farb;
                                Box1.BackColor = BackColor;
                                FileSystem.FileClose(99);
                                FileSystem.FileOpen(99, Modul1.Verz + "\\ErPos.dat", OpenMode.Append);
                                FileSystem.FileClose(99);
                                FileSystem.FileOpen(99, Modul1.Verz + "\\ErPos.dat", OpenMode.Input);
                                Top = (int)Math.Round(FileSystem.LineInput(99).AsDouble());
                                FileSystem.FileClose(99);
                            }
                            if (Top <= 0)
                            {
                                Top = 290;
                            }
                            if (Top > 350)
                            {
                                Top = 290;
                            }
                            if (eEventArt != EEventArt.eA_603 & eEventArt != EEventArt.eA_105)
                            {
                                Label1.Visible = false;
                                TextBox1.Visible = false;
                            }
                            TextBox6.Visible = true;
                            Label6.Visible = true;
                            Label13.Text = "";
                            Label3.Text = Modul1.IText[EUserText.t285];
                            Label12.Text = Modul1.IText[EUserText.t286];
                            Label4.Visible = true;
                            Label6.Text = Modul1.IText[EUserText.tChurchCemetrEtc];
                            Label2.Text = Modul1.IText[EUserText.tDate];
                            Label4.Text = Modul1.IText[EUserText.tText];
                            Label5.Text = Modul1.IText[EUserText.tPlace];
                            Label14.Text = Modul1.IText[EUserText.t283];
                            Frame1.Text = Modul1.IText[EUserText.t293];
                            Label16.Text = Modul1.IText[EUserText.t294];
                            Label9.Text = Modul1.IText[EUserText.t295];
                            Box1.Text = Modul1.IText[EUserText.t290];
                            Label14.Text = Modul1.IText[EUserText.t283];
                            Button1.Text = Modul1.IText[EUserText.t244] + ": " + Modul1.IText[EUserText.tNo];
                            Button1.BackColor = Button13.BackColor;
                            Button6.Text = Modul1.IText[EUserText.tDelete];
                            Button8.Text = Modul1.IText[EUserText.t284];
                            Button11.Text = Modul1.IText[EUserText.t291];
                            Button12.Text = Modul1.IText[EUserText.t44];
                            Button13.Text = Modul1.IText[EUserText.tNMNewPlace];
                            Button14.Text = Modul1.IText[EUserText.t292];
                            CheckBox1.Text = Modul1.IText[EUserText.t282];
                            RadioButton1.Text = Modul1.IText[EUserText.t279];
                            RadioButton2.Text = Modul1.IText[EUserText.t280];
                            RadioButton3.Text = Modul1.IText[EUserText.t281];
                            Button9.Text = Modul1.IText[EUserText.t296];
                            Button10.Text = Modul1.IText[EUserText.t297];
                            Button4.Text = Modul1.IText[EUserText.tNMCancel];
                            Button15.Text = Modul1.IText[EUserText.t299];
                            if (eEventArt is EEventArt.eA_Birth or EEventArt.eA_Baptism or
                                EEventArt.eA_Death or EEventArt.eA_Burial
                                or EEventArt.eA_500 or EEventArt.eA_501 or EEventArt.eA_Marriage or
                                EEventArt.eA_MarrReligious or EEventArt.eA_504 or EEventArt.eA_505)
                            {
                                CheckBox1.Visible = true;
                                Check1.Visible = false;
                            }
                            else
                            {
                                CheckBox1.Visible = false;
                            }
                            if (eEventArt > EEventArt.eA_499 & eEventArt < EEventArt.eA_602)
                            {
                                Modul1.Nr = Familie.Default.iFamNr;
                                Modul1.LfNR = 0;
                            }
                            else if (eEventArt == EEventArt.eA_603)
                            {
                                Modul1.Nr = Familie.Default.iFamNr;
                                TextBox1.Visible = true;
                                Label1.Visible = true;
                            }
                            else if (eEventArt == EEventArt.eA_602)
                            {
                                Modul1.Nr = Familie.Default.iFamNr;
                            }
                            else if (Modul1.Typ != DriveType.CDRom)
                            {
                                Modul1.Nr = Personen.Default.PersonNr;
                                DataModul.DB_PersonTable.Index = nameof(PersonIndex.PerNr);
                                DataModul.DB_PersonTable.Seek("=", Modul1.PersInArb.AsString());
                                if (DataModul.DB_PersonTable.NoMatch)
                                {
                                    _ = Interaction.MsgBox("Stop", mb: MessageBoxButtons.OK, title: "5");
                                }
                                DataModul.DB_PersonTable.Edit();
                                DataModul.DB_PersonTable.Fields[PersonFields.Sex].Value = Personen.Default.edtSex.Text.Trim().ToUpper();
                                Wort = Personen.Default.edtReligion.Text.Trim();
                                int Satz;
                                Satz = DataModul.Texte_Schreib(Wort, Modul1.UbgT1, ETextKennz.tk7_);
                                DataModul.DB_PersonTable.Fields[PersonFields.religi].Value = Satz;
                                if (Personen.Default.edtNotes.Text.TrimEnd() == "")
                                {
                                    Personen.Default.edtNotes.Text = " ";
                                }
                                I = 1;
                                while (I <= 20
                                    && Strings.Asc(Personen.Default.edtNotes.Text.Right(1)) < 14)
                                {
                                    Personen.Default.edtNotes.Text = Personen.Default.edtNotes.Text.Left(Personen.Default.edtNotes.Text.Length - 1);
                                    I += 1;
                                }
                                DataModul.DB_PersonTable.Fields[PersonFields.Bem1].Value = Personen.Default.edtNotes.Text;
                                DataModul.DB_PersonTable.Fields[PersonFields.Pruefen].Value = "0";
                                DataModul.DB_PersonTable.Update();
                            }
                            else
                            {
                                Modul1.Nr = Personen.Default.PersonNr;
                            }
                            if (Modul1.System.VerSpecial == 0 && Modul1.Typ != DriveType.CDRom)
                            {
                                Modul1.Datcheck((int)eEventArt);
                                if (Modul1.Aend == 5f)
                                {
                                    Modul1.ErSchalt = 10;
                                    DataModul.Event.AppendRaw((eEventArt, Modul1.Nr, Modul1.LfNR));
                                }
                            }
                            lErl = 344;
                            Button2.Text = Modul1.IText[EUserText.t263] + ": " + Modul1.IText[EUserText.tNo];
                            Button2.BackColor = Button13.BackColor;
                            Button3.Text = Modul1.IText[EUserText.tNMCancel];
                            Button5.Text = "&" + Modul1.IText[EUserText.t113];
                            Button8.Visible = false;
                            if (eEventArt != EEventArt.eA_Birth)
                            {
                                if (DataModul.Event.GetDate(EEventArt.eA_Birth, Modul1.Nr) == default)
                                {
                                    Button8.Visible = true;
                                }

                            }
                            if (eEventArt > EEventArt.eA_499)
                            {
                                Button8.Visible = true;
                            }
                            if (eEventArt == EEventArt.eA_105)
                            {
                                TextBox1.Visible = true;
                                Label1.Visible = true;
                            }
                            if (DataModul.Event.ReadData(eEventArt, Modul1.Nr, out var cEvent, Modul1.LfNR))
                            {
                                //Recordset recordset = default;
                                int priv = cEvent.iPrivacy;
                                if (priv == 0)
                                {
                                    RadioButton3.Checked = true;
                                }
                                else if (priv == 1)
                                {
                                    RadioButton2.Checked = true;
                                }
                                else if (priv == 2)
                                {
                                    RadioButton1.Checked = true;
                                }

                                if (cEvent.iArtText > 0)
                                {
                                    TextBox1.Text = cEvent.sArtText;
                                }
                                if (cEvent.iDatumText > 0)
                                {
                                    TextBox16.Text = cEvent.sDatumText;
                                    TextBox3.Visible = false;
                                    TextBox9.Visible = false;
                                    TextBox10.Visible = false;
                                    Label15.Visible = false;
                                }
                                if (cEvent.sDatumV_S != "")
                                {
                                    TextBox16.Text = "";
                                    TextBox16.Visible = false;
                                    Label20.Visible = false;
                                }
                                if (cEvent.dDatumB != default)
                                {
                                    TextBox16.Text = "";
                                    TextBox16.Visible = false;
                                    Label20.Visible = false;
                                }
                                if (cEvent.sDatumB_S != "")
                                {
                                    TextBox16.Text = "";
                                    TextBox16.Visible = false;
                                    Label20.Visible = false;
                                }
                                TextBox2.Text = cEvent.dDatumV.AsString();
                                TextBox2.Tag = cEvent.dDatumV;
                                TextBox3.Text = cEvent.sDatumV_S;
                                TextBox9.Text = cEvent.dDatumB.AsString();
                                TextBox9.Tag = cEvent.dDatumB;
                                TextBox10.Text = cEvent.sDatumB_S;
                                if (eEventArt == EEventArt.eA_Death)
                                {
                                    if (cEvent.xIsDead)
                                    {
                                        CheckBox2.CheckState = CheckState.Checked;
                                    }
                                }
                                if (eEventArt == EEventArt.eA_302 | eEventArt == EEventArt.eA_602)
                                {
                                    if (cEvent.iHausNr > 0)
                                    {
                                        TextBox18.Text = cEvent.sHausNr;
                                        TextBox18.Enabled = true;
                                    }
                                    else
                                        TextBox18.Enabled = false;
                                }
                                TextBox5.Tag = 0;
                                TextBox5.Text = "";
                                if (cEvent.sZusatz != "")
                                {
                                    TextBox7.Text = cEvent.sZusatz;
                                }
                                if (cEvent.iOrt > 0 && DataModul.Place.ReadData(cEvent.iOrt, out var cPlace))
                                {
                                    TextBox5.Tag = cEvent.iOrt;
                                    TextBox5.Text = DataModul.Place.FullName(cPlace);
                                }
                                TextBox11.Text = cEvent.sOrt_S;
                                if (cEvent.sPlatz != "")
                                {
                                    TextBox6.Text = cEvent.sPlatz;
                                }
                                TextBox8.Text = cEvent.sReg;
                                if (TextBox8.Text.Trim() == "")
                                {
                                    if (cEvent.dDatumV != default)
                                    {
                                        Button12.Visible = true;
                                        Label13.Text = cEvent.dDatumV.Year + "/";
                                    }
                                }
                                if (cEvent.iKBem > 0)
                                {
                                    TextBox4.Text = cEvent.sKBem;
                                    if (TextBox4.Text.Length > 2)
                                    {
                                        TextBox18.Enabled = true;
                                    }
                                }
                                if (eEventArt == EEventArt.eA_Death)
                                {
                                    if (0 != cEvent.iCausal)
                                    {
                                        TextBox17.Text = cEvent.sCausal;
                                        if (0 != cEvent.iAn)
                                        {
                                            TextBox19.Text = cEvent.sAn;
                                        }
                                    }
                                    if (TextBox4.Text != "" & TextBox17.Text == "")
                                    {
                                        Button16.Visible = true;
                                    }
                                }
                                if (cEvent.sBem[1] != "")
                                {
                                    RichTextBox1.Text = cEvent.sBem[1];
                                }
                                if (cEvent.sBem[2] != "")
                                {
                                    RichTextBox2.Text = cEvent.sBem[2];
                                }
                                if (cEvent.sBem[4] != "")
                                {
                                    Button2.Text = "&Zeugen: Ja";
                                    Button2.BackColor = ColorTranslator.FromOle(65535);
                                }
                                Modul1_PerfamNr = eEventArt > EEventArt.eA_499 ? Familie.Default.iFamNr : Personen.Default.PersonNr;
                                DataModul.DB_WitnessTable.Index = nameof(WitnessIndex.ZeugSu);
                                DataModul.DB_WitnessTable.Seek("=", Modul1_PerfamNr, "10", eEventArt, Modul1.LfNR);
                                if (!DataModul.DB_WitnessTable.NoMatch)
                                {
                                    Button2.Text = "&Zeugen: Ja";
                                    Button2.BackColor = ColorTranslator.FromOle(65535);
                                }
                                if (cEvent.sBem[3] != "")
                                {
                                    Button1.Text = "&Quellen: Ja";
                                    Button1.BackColor = ColorTranslator.FromOle(65535);
                                }
                                DataModul.DB_SourceLinkTable.Index = "Tab22";
                                DataModul.DB_SourceLinkTable.Seek("=", 3, Modul1.Nr, eEventArt, Modul1.LfNR);
                                if (!DataModul.DB_SourceLinkTable.NoMatch)
                                {
                                    Button1.Text = "&Quellen: Ja";
                                    Button1.BackColor = ColorTranslator.FromOle(65535);
                                }
                                if (cEvent.iPlatz > 0)
                                {
                                    TextBox6.Text = cEvent.sPlatz;
                                }
                            }
                            CheckBox1.Checked = cEvent.sVChr != "0";

                            TextBox2.Text = "00000000" + TextBox2.Text.Trim().Right(8);
                            TextBox2.Text = TextBox2.Text.AsInt() > 0.0
                                ? Strings.Mid(TextBox2.Text, 7, 2) + "." + Strings.Mid(TextBox2.Text, 5, 2) + "." + TextBox2.Text.Left(4)
                                : "";
                            if (0 < cEvent.iGrabNr)
                            {
                                edtGrabNr.Text = cEvent.sGrabNr;
                            }
                            TextBox9.Text = "00000000" + TextBox9.Text.Trim().Right(8);
                            TextBox9.Text = TextBox9.Text.AsInt() > 0.0
                                ? Strings.Mid(TextBox9.Text, 7, 2) + "." + Strings.Mid(TextBox9.Text, 5, 2) + "." + TextBox9.Text.Left(4)
                                : "";
                            Show();
                            _ = TextBox2.Focus();
                            Label23.Visible = false;
                            Button17.Visible = false;
                            edtGrabNr.Visible = false;
                            switch (eEventArt)
                            {
                                case EEventArt.eA_Birth:
                                    Topsch = "101 " + Modul1.IText[EUserText.t264];
                                    Label10.Text = Strings.Mid(Topsch, 4, Topsch.Length);
                                    break;
                                case EEventArt.eA_Baptism:
                                    Topsch = "102 Taufe";
                                    Label10.Text = Strings.Mid(Topsch, 4, Topsch.Length);
                                    break;
                                case EEventArt.eA_Death:
                                    Topsch = "103 Tod";
                                    Label10.Text = Strings.Mid(Topsch, 4, Topsch.Length);
                                    break;
                                case EEventArt.eA_Burial:
                                    Topsch = "104 Begraben";
                                    Label10.Text = Strings.Mid(Topsch, 4, Topsch.Length);
                                    if (Modul1.System.xJudenfriedhofVersion)
                                    {
                                        Label23.Visible = true;
                                        if (edtGrabNr.Text.Trim() != "")
                                        {
                                            Button17.Visible = true;
                                        }
                                        edtGrabNr.Visible = true;
                                    }
                                    break;
                                case EEventArt.eA_105:
                                    _ = TextBox1.Focus();
                                    Check1.Text = "Haupteintrag";
                                    Topsch = "105 Sonst. Datum Kommunion/Konfirmation/etc.";
                                    Label10.Text = Strings.Mid(Topsch, 4, Topsch.Length);
                                    break;
                                case EEventArt.eA_106:
                                    Topsch = "106 Heimatort";
                                    Label10.Text = Strings.Mid(Topsch, 4, Topsch.Length);
                                    break;
                                case EEventArt.eA_107:
                                    Topsch = "107 " + Modul1.IText[EUserText.tResidence];
                                    Label10.Text = Strings.Mid(Topsch, 4, Topsch.Length);
                                    break;
                                case EEventArt.eA_201:
                                    Topsch = "201 Taufe/Baptism LDS";
                                    Label10.Text = Strings.Mid(Topsch, 4, Topsch.Length);
                                    break;
                                case EEventArt.eA_202:
                                    Topsch = "202 Begabung/Endowment LDS";
                                    Label10.Text = Strings.Mid(Topsch, 4, Topsch.Length);
                                    break;
                                case EEventArt.eA_203:
                                    Topsch = "203 Siegel Kind an Eltern LDS";
                                    Label10.Text = Strings.Mid(Topsch, 4, Topsch.Length);
                                    break;
                                case EEventArt.eA_300:
                                    Button12.Visible = false;
                                    TextBox8.Visible = false;
                                    Check1.Visible = true;
                                    Label4.Text = Modul1.IText[EUserText.t70];
                                    Label4.Text = Modul1.IText[EUserText.tOccupation];
                                    Label6.Text = Modul1.IText[EUserText.t71];
                                    Topsch = "300 " + Modul1.IText[EUserText.tOccupation];
                                    Label10.Text = Strings.Mid(Topsch, 4, Topsch.Length);
                                    _ = Modul1.Aus[26].AsBool() ? TextBox4.Focus() : TextBox2.Focus();
                                    Check1.Text = Modul1.IText[EUserText.t74];
                                    if (cEvent.sReg.Trim() == "1")
                                    {
                                        Check1.CheckState = CheckState.Checked;
                                    }
                                    break;
                                case EEventArt.eA_301:
                                    Button12.Visible = false;
                                    TextBox8.Visible = false;
                                    Check1.Visible = true;
                                    Label4.Text = Modul1.IText[EUserText.t70];
                                    Label6.Text = Modul1.IText[EUserText.t71];
                                    Topsch = "301 " + Modul1.IText[EUserText.t70];
                                    Label10.Text = Strings.Mid(Topsch, 4, Topsch.Length);
                                    _ = Modul1.Aus[26].AsBool() ? TextBox4.Focus() : TextBox2.Focus();
                                    Check1.Text = Modul1.IText[EUserText.t77];
                                    if (cEvent.sReg.Trim() == "1")
                                    {
                                        Check1.CheckState = CheckState.Checked;
                                    }
                                    break;
                                case EEventArt.eA_302:
                                    Button12.Visible = false;
                                    TextBox8.Visible = false;
                                    Check1.Visible = true;
                                    Label4.Text = Modul1.IText[EUserText.t70];
                                    Label4.Text = Modul1.IText[EUserText.t75];
                                    Topsch = "302 Wohnort";
                                    Label10.Text = Strings.Mid(Topsch, 4, Topsch.Length);
                                    if (Modul1.Aus[26] == "")
                                    {
                                        Modul1.Aus[26] = true.AsString();
                                    }
                                    _ = Modul1.Aus[26].AsBool() ? TextBox5.Focus() : TextBox2.Focus();
                                    Check1.Text = Modul1.IText[EUserText.t76];
                                    if (cEvent.sReg.Trim() == "1")
                                    {
                                        Check1.CheckState = CheckState.Checked;
                                    }
                                    break;
                                case EEventArt.eA_500:
                                    Topsch = "500 " + Modul1.IText[EUserText.tProclamation];
                                    Label10.Text = Strings.Mid(Topsch, 4, Topsch.Length);
                                    break;
                                case EEventArt.eA_501:
                                    Topsch = "501 " + Modul1.IText[EUserText.tEngagement];
                                    Label10.Text = Strings.Mid(Topsch, 4, Topsch.Length);
                                    break;
                                case EEventArt.eA_Marriage:
                                    Topsch = "502 " + Modul1.IText[EUserText.tMarriage];
                                    Label10.Text = Strings.Mid(Topsch, 4, Topsch.Length);
                                    break;
                                case EEventArt.eA_MarrReligious:
                                    Topsch = "503 " + Modul1.IText[EUserText.tMarriageRelig];
                                    Label10.Text = Strings.Mid(Topsch, 4, Topsch.Length);
                                    break;
                                case EEventArt.eA_504:
                                    Topsch = "504 " + Modul1.IText[EUserText.tDivorce];
                                    Label10.Text = Strings.Mid(Topsch, 4, Topsch.Length);
                                    break;
                                case EEventArt.eA_505:
                                    Topsch = "505 " + Modul1.IText[EUserText.tPartnership];
                                    Label10.Text = Strings.Mid(Topsch, 4, Topsch.Length);
                                    break;
                                case EEventArt.eA_506:
                                    Topsch = "506 Außereheliche Verbindung";
                                    Label10.Text = Strings.Mid(Topsch, 4, Topsch.Length);
                                    break;
                                case EEventArt.eA_507:
                                    Topsch = "507 Dimissiorale";
                                    Label10.Text = Strings.Mid(Topsch, 4, Topsch.Length);
                                    break;
                                case EEventArt.eA_510:
                                    Topsch = "510 Siegel Frau an Mann HLT";
                                    Label10.Text = Strings.Mid(Topsch, 4, Topsch.Length);
                                    break;
                                case EEventArt.eA_601:
                                    TextBox9.Visible = false;
                                    TextBox1.Visible = false;
                                    TextBox3.Visible = false;
                                    TextBox4.Visible = false;
                                    TextBox8.Visible = false;
                                    TextBox6.Visible = false;
                                    TextBox7.Visible = false;
                                    Button12.Visible = false;
                                    Label10.Visible = true;
                                    TextBox10.Visible = false;
                                    Button1.Visible = false;
                                    Button2.Visible = false;
                                    Button8.Visible = false;
                                    Label14.Visible = false;
                                    TextBox11.Visible = false;
                                    Label15.Visible = false;
                                    RichTextBox1.Visible = false;
                                    RichTextBox2.Visible = false;
                                    if (Familie.Default.lstChildren.Items.Count > 0)
                                    {
                                        Button11.Visible = true;
                                    }
                                    Label6.Visible = false;
                                    Label4.Visible = false;
                                    Label3.Visible = false;
                                    Label12.Visible = false;
                                    Label6.Visible = false;
                                    Topsch = "601 Fiktives Heiratsdatum, nur zur Sortierung im Ortsfamilienbuch";
                                    Label10.Text = Strings.Mid(Topsch, 4, Topsch.Length);
                                    break;
                                case EEventArt.eA_602:
                                    Button12.Visible = false;
                                    Check1.Visible = true;
                                    TextBox8.Visible = false;
                                    Check1.Visible = true;
                                    Label4.Text = Modul1.IText[EUserText.t75];
                                    Check1.Text = Modul1.IText[EUserText.t76];
                                    if (cEvent.sReg.Trim() == "1")
                                    {
                                        Check1.CheckState = CheckState.Checked;
                                    }
                                    Topsch = "602 Wohnort der Familie";
                                    Label10.Text = Strings.Mid(Topsch, 4, Topsch.Length);
                                    if (Modul1.Aus[26] == "")
                                    {
                                        Modul1.Aus[26] = true.AsString();
                                    }
                                    _ = Modul1.Aus[26].AsBool() ? TextBox5.Focus() : TextBox2.Focus();
                                    break;
                                case EEventArt.eA_603:
                                    Topsch = "603 Sonstiges Datum der Familie";
                                    Label10.Text = Strings.Mid(Topsch, 4, Topsch.Length);
                                    _ = TextBox1.Focus();
                                    break;
                                default:
                                    _ = TextBox1.Focus();
                                    break;
                            }
                            Label10.Tag = Topsch.Left(4);
                            ListBox2.Visible = false;
                            ListBox1.Visible = false;
                            ListBox4.Visible = false;
                            Modul1.UbgT = "";
                            goto end_IL_0001_2;
                        IL_3c38: // <========== 3
                            num = 640;
                            int number = Information.Err().Number;
                            if (number is 94 or 62)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_3d1a;
                            }
                            else
                            {
                                if (number == 3021)
                                {
                                    goto end_IL_0001_2;
                                }
                                if (Interaction.MsgBox(Conversion.ErrorToString(), mb: MessageBoxButtons.OKCancel, title: Information.Err().Number.AsString()) == DialogResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                _ = Interaction.MsgBox("F110");
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                                goto IL_3d1e;
                            }
                        IL_3d1a:
                            num4 = unchecked(num2 + 1);
                            goto IL_3d1e;
                        IL_3d1e:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 639:
                                case 640:
                                    goto IL_3c38;
                                case 635:
                                case 641:
                                case 645:
                                case 646:
                                case 648:
                                case 649:
                                case 656:
                                case 657:
                                case 658:
                                    goto end_IL_0001_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 18292;
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


    private void Button3_Click(object sender, EventArgs e)
    {
        Schaltsp = Modul1.ErSchalt;
        checked
        {
            Modul1.PFSatz = eEventArt > EEventArt.eA_499 ? Familie.Default.iFamNr : Personen.Default.PersonNr;
            if (LeerPruef(eEventArt) | Schaltsp == 10f)
            {
                Modul1.PFSatz = eEventArt > EEventArt.eA_499 ? Familie.Default.iFamNr : Personen.Default.PersonNr;
                DataModul.Event.Delete(key: (eEventArt, Modul1.PFSatz, Modul1.LfNR));
                Close();
            }
            else
            {
                if (Modul1.ErSchalt == 2)
                {
                    DataModul.Event.Delete(key: (eEventArt, Modul1.PFSatz, Modul1.LfNR));
                }
                Modul1.ErSchalt = 0;
            }
            Check1.Visible = false;
            ListBox1.Visible = false;
            TextBox1.Text = "";
            TextBox2.Text = "";
            TextBox3.Text = "";
            TextBox4.Text = "";
            TextBox5.Tag = "";
            TextBox5.Text = "";
            TextBox5.Text = "";
            TextBox6.Text = "";
            TextBox7.Text = "";
            TextBox8.Text = "";
            TextBox9.Text = "";
            TextBox10.Text = "";
            TextBox11.Text = "";
            TextBox17.Text = "";
            RichTextBox1.Text = "";
            RichTextBox2.Text = "";
            KBem = 0;
            Modul1.Aend = 0f;
            Close();
        }
    }

    private void Button6_Click(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_017d, IL_0185
        var Modul1_Value = Interaction.MsgBox("Diesen Datumseintrag unwiderruflich löschen?", icon: MessageBoxIcon.Exclamation, mb: MessageBoxButtons.YesNo, title: "ACHTUNG !!");
        if (Modul1_Value != DialogResult.Yes)
        {
            return;
        }
        while (true)
        {
            if (!DataModul_SourceLink_Delete())
            {
                continue;
            }
            if (!DataModul_Witness(GetModul1_PerfamNr(), GetEEventArt1()))
            {
                break;
            }
        }
        Modul1.PFSatz = eEventArt > EEventArt.eA_499 ? Familie.Default.iFamNr : Personen.Default.PersonNr;
        DataModul.Event.Delete(key: (eEventArt, Modul1.PFSatz, Modul1.LfNR));
        Check1.Visible = false;
        ListBox1.Visible = false;
        TextBox1.Text = "";
        TextBox2.Text = "";
        TextBox3.Text = "";
        TextBox4.Text = "";
        TextBox5.Tag = "";
        TextBox5.Text = "";
        TextBox5.Text = "";
        TextBox6.Text = "";
        TextBox7.Text = "";
        TextBox8.Text = "";
        TextBox9.Text = "";
        TextBox10.Text = "";
        TextBox11.Text = "";
        RichTextBox1.Text = "";
        RichTextBox2.Text = "";
        checked
        {
            if (eEventArt > EEventArt.eA_499)
            {
                Modul1.FamInArb = Familie.Default.iFamNr;
                short Rich;
                Familie.Default.Fameinlesen(Modul1.FamInArb, out Rich);
            }
            else
            {
                Modul1.PersInArb = Personen.Default.PersonNr;
            }
            Close();
        }
    }

    private int GetModul1_PerfamNr()
    {
        return Modul1_PerfamNr;
    }

    private EEventArt GetEEventArt1()
    {
        return eEventArt;
    }

    private bool DataModul_Witness(int modul1_PerfamNr, EEventArt eEventArt1)
    {
        GenFree.Interfaces.DB.IRecordset dB_WitnessTable = DataModul.DB_WitnessTable;
        dB_WitnessTable.Index = nameof(WitnessIndex.ZeugSu);
        dB_WitnessTable.Seek("=", modul1_PerfamNr, 10, eEventArt1, Modul1.LfNR);
        if (!dB_WitnessTable.NoMatch
            && dB_WitnessTable.Fields[WitnessFields.FamNr].AsInt() == modul1_PerfamNr
             && dB_WitnessTable.Fields[WitnessFields.Kennz].AsInt() == 10
              && dB_WitnessTable.Fields[WitnessFields.Art].AsEnum<EEventArt>() == eEventArt1 && dB_WitnessTable.Fields[WitnessFields.LfNr].AsInt() == Modul1.LfNR)
        {
            dB_WitnessTable.Delete();
            return true;
        }
        return false;
    }

    private bool DataModul_SourceLink_Delete()
    {
        DataModul.DB_SourceLinkTable.Index = "Tab22";
        DataModul.DB_SourceLinkTable.Seek("=", 3, Modul1.Nr, eEventArt, Modul1.LfNR);
        if (!DataModul.DB_SourceLinkTable.NoMatch
            && DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._1].AsInt() == 3
            && DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._2].AsInt() == Modul1.Nr
            && DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Art].AsEnum<EEventArt>() == eEventArt
            && DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.LfNr].AsInt() == Modul1.LfNR)
        {
            DataModul.DB_SourceLinkTable.Delete();
            return true;
        }
        return false;

    }

    private void Button1_Click(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_06fb
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
                            goto IL_0008;
                        case 2137:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_0749;
                                    default:
                                        goto end_IL_0001;
                                }
                                if (Interaction.MsgBox(Conversion.ErrorToString(), mb: MessageBoxButtons.OKCancel, title: Information.Err().Number.AsString()) == DialogResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                                goto IL_074c;
                            }
                        end_IL_0001:
                            break;
                        IL_0008:
                            num = 2;
                            Quellen frmQuellen = MainProject.Forms.Quellen;
                            Modul1.Qkenn = 3;
                            //eEventArt = eEventArt;
                            Modul1.Nr = eEventArt < EEventArt.eA_499 ? Modul1.PersInArb : Familie.Default.iFamNr;

                            frmQuellen.SourceLink_RefreshListEvt(Modul1.Nr, eEventArt, Modul1.LfNR);
                            frmQuellen.Button3.Enabled = frmQuellen.ComboBox1.Items.Count > 0;

                            var dB_EventTable = DataModul.Event.Seek((eEventArt, Modul1.Nr, Modul1.LfNR));
                            if (null != dB_EventTable.Fields[EventFields.Bem3].Value)
                            {
                                if (dB_EventTable.Fields[EventFields.Bem3].AsString().Length > 1)
                                {
                                    frmQuellen.RTB.Text = dB_EventTable.Fields[EventFields.Bem3].AsString();
                                }
                                Button1.Text = "&Quellen: Ja";
                                Button1.BackColor = ColorTranslator.FromOle(65535);
                            }
                            else
                            {
                                frmQuellen.RTB.Text = "";
                            }
                            frmQuellen.Top = Top + Label2.Top;
                            frmQuellen.Left = (int)Math.Round(Left + Label2.Width / 2.0);
                            _ = frmQuellen.ShowDialog((int)eEventArt);
                            goto end_IL_0001_2;
                        IL_0749:
                            num4 = unchecked(num2 + 1);
                            goto IL_074c;
                        IL_074c:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0001_dispatch = 2137;
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
    private void Button12_Click(object sender, EventArgs e)
    {
        if (TextBox8.Text.Trim() == "")
        {
            TextBox8.Text = Label13.Text;
            _ = TextBox8.Focus();
            byte b = (byte)TextBox8.Text.Length;
            TextBox8.SelectionStart = unchecked(b) + 1;
        }

    }

    private void Button5_Click(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_0ae0
        int try0001_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        bool Ortf = default;
        int lErl = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    short Rich;
                    int num4;
                    MainProject.MyForms forms;
                    Form Formtocheck;
                    short num5;
                    switch (try0001_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0009;
                        case 3511:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_0b7d;
                                    default:
                                        goto end_IL_0001;
                                }
                                if (Information.Err().Number == 13)
                                {
                                    _ = Interaction.MsgBox("Im Datumsfeld dürfen keine Buchstaben stehen.", title: Information.Err().Number.AsString(), icon: MessageBoxIcon.Exclamation);
                                    goto end_IL_0001_2;
                                }
                                else
                                {
                                    if (Interaction.MsgBox(Conversion.ErrorToString(), mb: MessageBoxButtons.OKCancel, title: Information.Err().Number.AsString()) == DialogResult.Cancel)
                                    {
                                        ProjectData.EndApp();
                                    }
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    num4 = num2;
                                    goto IL_0b81;
                                }
                            }
                        end_IL_0001:
                            break;
                        IL_0009:
                            num = 2;
                            _ = Button5.Focus();
                            if (TextBox5.Text != "" & TextBox5.Tag.AsInt() == 0.0)
                            {
                                _ = Interaction.MsgBox("Ort kann so nicht gespeichert werden. Neueingabe nur über >neuer Ort<");
                                goto end_IL_0001_2;
                            }
                            if (TextBox2.Text != "")
                            {
                                var array = TextBox2.Text.Split('.');
                                if (array[0].AsDouble() < 0.0 | array[0].AsDouble() > 31.0)
                                {
                                    _ = Interaction.MsgBox("Die Tagesangabe \"" + array[0] + "\" ist falsch und kann so nicht gespeichert werden");
                                    goto end_IL_0001_2;
                                }
                                else if (array[1].AsDouble() < 0.0 | array[1].AsDouble() > 12.0)
                                {
                                    _ = Interaction.MsgBox("Die Monatsangabe \"" + array[1] + "\" ist falsch und kann so nicht gespeichert werden");
                                    goto end_IL_0001_2;
                                }

                            }
                            if (TextBox9.Text != "")
                            {
                                var array2 = TextBox9.Text.Split('.');
                                if (array2[0].AsDouble() < 0.0 | array2[0].AsDouble() > 31.0)
                                {
                                    _ = Interaction.MsgBox("Die Tagesangabe \"" + array2[0] + "\" ist falsch und kann so nicht gespeichert werden");
                                    goto end_IL_0001_2;
                                }
                                else if (array2[1].AsDouble() < 0.0 | array2[1].AsDouble() > 12.0)
                                {
                                    _ = Interaction.MsgBox("Die Monatsangabe \"" + array2[1] + "\" ist falsch und kann so nicht gespeichert werden");
                                    goto end_IL_0001_2;
                                }

                            }
                            if (TextBox2.Text == "")
                            {
                                TextBox3.Text = "";
                            }
                            if (TextBox9.Text == "")
                            {
                                TextBox10.Text = "";
                            }
                            if (TextBox9.Text != "" & TextBox2.Text != "")
                            {
                                if (TextBox3.Text.Trim() != "" & TextBox3.Text.ToUpper() != "Z".ToUpper())
                                {
                                    if (eEventArt == EEventArt.eA_Birth
                                        || eEventArt == EEventArt.eA_Baptism
                                        || eEventArt == EEventArt.eA_Death
                                        || eEventArt == EEventArt.eA_Burial
                                        || eEventArt == EEventArt.eA_500
                                        || eEventArt == EEventArt.eA_501
                                        || eEventArt == EEventArt.eA_Marriage
                                        || eEventArt == EEventArt.eA_MarrReligious
                                        || eEventArt == EEventArt.eA_504
                                        || eEventArt == EEventArt.eA_505
                                        || eEventArt == EEventArt.eA_506
                                        || eEventArt == EEventArt.eA_507)
                                    {
                                        _ = Interaction.MsgBox("Bei Belegung beider Datumsfelder ist der Zusatz \"" + TextBox3.Text + "\" nicht möglich");
                                        TextBox3.Text = "z";
                                        TextBox10.Text = "a";
                                    }
                                    else if (TextBox3.Text.Trim() != "" & TextBox3.Text.ToUpper() != "B".ToUpper())
                                    {
                                        _ = Interaction.MsgBox("Bei Belegung beider Datumsfelder ist der Zusatz \"" + TextBox3.Text + "\" nicht möglich");
                                        TextBox3.Text = "";
                                        TextBox10.Text = "b";
                                    }

                                }
                            }
                            Pruefaend();
                            Ortf = false;
                            if (Modul1.Typ != DriveType.CDRom)
                            {
                                Erspeich(Ortf);
                            }
                            if (Ortf)
                            {
                                goto end_IL_0001_2;
                            }
                            lErl = 2;
                            Modul1.Aend = 0f;
                            TextBox1.Text = "";
                            TextBox2.Text = "";
                            TextBox3.Text = "";
                            TextBox4.Text = "";
                            TextBox5.Tag = "";
                            TextBox5.Text = "";
                            TextBox6.Text = "";
                            TextBox7.Text = "";
                            TextBox8.Text = "";
                            TextBox9.Text = "";
                            TextBox10.Text = "";
                            TextBox11.Text = "";
                            TextBox17.Text = "";
                            TextBox18.Text = "";
                            TextBox19.Text = "";
                            edtGrabNr.Text = "";
                            RichTextBox1.Text = "";
                            RichTextBox2.Text = "";
                            KBem = 0;
                            Modul1_MyList.Clear();
                            int num7 = ListBox3.Items.Count - 1;
                            int num6 = 0;
                            while (num6 <= num7)
                            {
                                ListBox3.SelectedIndex = num6;
                                Modul1_MyList.Add(ListBox3.Text);
                                num6++;
                            }
                            ListBox3.Items.Clear();
                            Modul1_oResult = Modul1.DeleteDoublicates(Modul1_MyList);
                            ListBox3.Items.AddRange(Modul1_oResult.ToArray());
                            ListBox3.Visible = true;
                            if (Modul1.Typ != DriveType.CDRom)
                            {
                                Persistence_WriteStringsMand("OW.dat", 6, ListBox3.Items);
                            }
                            Close();
                            if (eEventArt > EEventArt.eA_499)
                            {
                                Modul1.FamInArb = Familie.Default.iFamNr;
                                Rich = 3;
                                Familie.Default.Fameinlesen(Modul1.FamInArb, out Rich);
                            }
                            else
                            {
                                Modul1.PersInArb = 0;
                                forms = MainProject.Forms;
                                Formtocheck = forms.Personen;
                                num5 = Modul1.IsFormloaded(Formtocheck);
                                forms.Personen = (Personen)Formtocheck;
                                if (num5 == -1)
                                {
                                    Modul1.PersInArb = Personen.Default.PersonNr;
                                    if (Modul1.PersInArb == 0)
                                    {
                                        _ = Interaction.MsgBox(Modul1.PersInArb.AsString(), title: "3", mb: MessageBoxButtons.OK);
                                    }
                                }
                            }
                            if (Modul1.Typ != DriveType.CDRom)
                            {
                                Persistence_WriteIntMand("ErPos.dat", Top);
                            }
                            goto end_IL_0001_2;
                        IL_0b7d:
                            num4 = unchecked(num2 + 1);
                            goto IL_0b81;
                        IL_0b81:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 5:
                                case 11:
                                case 15:
                                case 22:
                                case 26:
                                case 60:
                                case 129:
                                case 132:
                                case 138:
                                    goto end_IL_0001_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 3511;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001_2: // <========== 9
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    private void Persistence_WriteIntMand(string sSection, int iVal)
    {
        checked
        {
            FileSystem.FileClose(99);
            FileSystem.FileOpen(99, Modul1.Verz + "\\" + sSection, OpenMode.Output);
            FileSystem.PrintLine(99, iVal);
            FileSystem.FileClose(99);
        }
    }

    private void Persistence_WriteStringsMand(string sSection, int iCnt, ListBox.ObjectCollection items)
    {
        FileSystem.FileClose(99);
        FileSystem.FileOpen(99, Modul1.Verz + "\\" + sSection, OpenMode.Output);
        var I = 0;
        var num12 = 0;
        while (I <= items.Count - 1
            && num12 < iCnt)
        {

            if (items[I].AsString().Trim() != "")
            {
                FileSystem.PrintLine(99, items[I].AsString());
                num12++;
            }
            I++;
        }
        FileSystem.FileClose(99);
    }

    public bool LeerPruef(EEventArt eArt)
    {
        short erSchalt = 0;
        if (Rahmen.Default.Visible
            || TextBox1.Text.Trim() != "" || TextBox2.Text.Trim() != "" || TextBox3.Text.Trim() != ""
            || TextBox4.Text.Trim() != "" || TextBox5.Text.Trim() != "" || TextBox6.Text.Trim() != ""
            || TextBox7.Text.Trim() != "" || TextBox8.Text.Trim() != "" || TextBox9.Text.Trim() != ""
            || TextBox10.Text.Trim() != "" || TextBox11.Text.Trim() != "" || TextBox16.Text.Trim() != ""
            || TextBox17.Text.Trim() != "" || TextBox19.Text.Trim() != "" || edtGrabNr.Text.Trim() != ""
            || eEventArt == EEventArt.eA_Death)
        {
            erSchalt = 1;
        }
        if (RichTextBox1.Text.Trim() != "" || RichTextBox2.Text.Trim() != "")
        {
            erSchalt = 1;
        }
        if (DataModul.Event.GetValue(Modul1_PerfamNr, eArt, EventFields.Bem4, (f) => f.AsString()).Trim() != "")
        {
            erSchalt = 1;
        }
        if (DataModul.Event.GetValue(Modul1_PerfamNr, eArt, EventFields.Bem4, (f) => f.AsString()).Trim() != "")
        {
            erSchalt = 1;
        }
        return (Modul1.ErSchalt = erSchalt) == 0;
    }

    private void ListBox4_DoubleClick(object sender, EventArgs e)
    {
        int Ortnr = ListBox4.SelectedItem.ItemData<int>();
        SelectPlace(Ortnr);
    }
    private void ListBox1_DoubleClick(object sender, EventArgs e)
    {
        int Ortnr = ListBox1.SelectedItem.ItemData<int>();
        SelectPlace(Ortnr);
    }

    private void SelectPlace(int Ortnr)
    {
        var asOData = DataModul.Place.ReadData(Ortnr, out var cPlace) ? DataModul.Place.FullName(cPlace, true, true) : "";
        TextBox5.Text = asOData;
        Modul1_Event_LetztOrtspei = asOData + new string(' ', 50).Left(50) + Ortnr.AsString();
        TextBox5.Tag = Ortnr;
        ListBox2.Visible = false;
        ListBox1.Visible = false;
        ListBox4.Visible = false;
        _ = TextBox11.Focus();
    }


    private void ListBox2_DoubleClick(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_030a
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
                    case 1150:
                        {
                            num2 = num;
                            switch (num3 <= -2 ? 1 : num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_0386;
                                default:
                                    goto end_IL_0001;
                            }
                            if (Information.Err().Number == 5)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_0386;
                            }
                            else
                            {
                                if (Interaction.MsgBox(Conversion.ErrorToString(), mb: MessageBoxButtons.OKCancel, title: Information.Err().Number.AsString()) == DialogResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                                goto IL_0389;
                            }
                        }
                    end_IL_0001:
                        break;
                    IL_0008:
                        num = 2;
                        var ubg = Modul1.Ubg;
                        Modul1.UbgT = "";
                        if (ubg == 0)
                        {
                            TextBox1.Text = ListBox2.Text.Left(240);
                            ListBox2.Visible = false;
                            _ = TextBox2.Focus();
                        }
                        if (ubg == 5)
                        {
                            TextBox4.Text = ListBox2.Text.Left(240);
                            ListBox2.Visible = false;
                            if (TextBox18.Visible)
                            {
                                if (TextBox4.Text.Length > 99)
                                {
                                    TextBox18.Enabled = true;
                                    _ = TextBox18.Focus();
                                }
                                else
                                {
                                    _ = TextBox5.Focus();
                                }
                            }
                            else
                            {
                                _ = TextBox5.Focus();
                            }
                        }
                        else if (ubg == 6)
                        {
                            TextBox5.Text = ListBox2.Text.Left(50);
                            Modul1_Event_LetztOrtspei = ListBox2.Text;
                            TextBox5.Tag = Strings.Mid(ListBox2.Text, 51, 10).AsDouble().AsString();
                            ListBox2.Visible = false;
                            ListBox1.Visible = false;
                            ListBox4.Visible = false;
                            _ = TextBox6.Focus();
                        }
                        else if (ubg == 9)
                        {
                            TextBox6.Text = ListBox2.Text.Left(240);
                            ListBox2.Visible = false;
                        }
                        else if (ubg == 58)
                        {
                            TextBox16.Text = ListBox2.Text.Left(240);
                            ListBox2.Visible = false;
                            _ = TextBox4.Focus();
                        }
                        else if (ubg == 17)
                        {
                            TextBox17.Text = ListBox2.Text.Left(240);
                            ListBox2.Visible = false;
                        }
                        ubg = 0;
                        Modul1.UbgT = "";
                        Modul1_EingCode = 0;
                        goto end_IL_0001_2;
                    IL_0386:
                        num4 = num2 + 1;
                        goto IL_0389;
                    IL_0389:
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
                try0001_dispatch = 1150;
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
    private void TextBox5_GotFocus(object sender, EventArgs e)
    {
        if (!ListBox3.Visible)
        {
            Modul1.UbgT = "";
        }
        ListBox3.Visible = true;
        Button13.Visible = true;
        if (Modul1_Event_LetztOrtspei != "")
        {
            Label8.Visible = true;
            Button14.Visible = true;
            Label8.Text = Modul1_Event_LetztOrtspei;
        }
    }

    private void TextBox1_GotFocus(object sender, EventArgs e)
    {
        ListBox3.Visible = false;
        Button14.Visible = false;
        Label8.Visible = false;
        Button13.Visible = false;
        if (TextBox3.Focused)
        {
            Label11.Text = Modul1.IText[EUserText.t288];
        }
        else
        {
            Label11.Text = TextBox10.Focused ? Modul1.IText[EUserText.t289] : "";
        }
    }

    private void TextBox1_KeyDown(object sender, KeyEventArgs e)
    {
        //Discarded unreachable code: IL_05b8, IL_07bb, IL_07c4, IL_08d9
        schalt5 = 1;
        short num;
        short num2;
        short num3 = default;
        checked
        {
            num = (short)e.KeyCode;
            num2 = (short)unchecked((int)e.KeyData / 65536);
            Modul1_EingCode = 0;
            switch (((TextBox)sender).Name)
            {
                case "edtPredicate":
                    num3 = 0;
                    break;
                case "edtSuburb":
                    num3 = 1;
                    break;
                case "edtCounty":
                    num3 = 2;
                    TextBox16.Text = "";
                    TextBox16.Visible = false;
                    Label20.Visible = false;
                    break;
                case "edtCountry":
                    num3 = 5;
                    break;
                case "edtNameprefix":
                    num3 = 6;
                    break;
                case "edtNameSuffix":
                    num3 = 9;
                    break;
                case "edtGOV":
                    num3 = 3;
                    TextBox16.Text = "";
                    TextBox16.Visible = false;
                    Label20.Visible = false;
                    break;
                case "edtLat2":
                    TextBox16.Text = "";
                    TextBox16.Visible = false;
                    Label20.Visible = false;
                    break;
            }
            string left = (TextBox3.Text + TextBox9.Text + TextBox10.Text).Trim();
            if (left == "")
            {
                TextBox16.Visible = true;
                Label20.Visible = true;
            }
            if (Modul1.Trans == 0)
            {
                Modul1.Trans = 1;
            }
        }
        if (num3 is 1 or 3)
        {
            if (num2 > 0)
            {
                return;
            }
            switch (num)
            {
                case 18:
                case 48:
                case 49:
                case 50:
                case 51:
                case 52:
                case 53:
                case 54:
                case 55:
                case 56:
                case 57:
                case 96:
                case 97:
                case 98:
                case 99:
                case 100:
                case 101:
                case 102:
                case 103:
                case 104:
                case 105:
                case 144:
                case 145:
                case 190:
                    return;
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
                    break;
                case 13:
                    {
                        string text = ((TextBox)sender).Text;
                        float num4 = Strings.InStr(text, ".");
                        if (!(num4 == 2f || num4 == 4f) && !(num4 > 0f && text.Length < 10) && !(text.Length > 0 && text.Length < 4))
                        {
                            if (text.Length < 8)
                            {
                                text = text.PadRight(8, '0');
                            }
                            text = text.Replace(".", "");
                            if (!(text.Left(2).AsDouble() > 31.0) && !(Strings.Mid(text, 3, 2).AsDouble() > 12.0))
                            {
                                if (TextBox8.Text.Trim() == "")
                                {
                                    Button12.Visible = true;
                                    if (num3 == 1 && text.AsInt() > 0.0)
                                    {
                                        Label13.Text = text.Right(4) + "/";
                                    }
                                }
                                return;
                            }
                        }
                        goto default;
                    }
                case 8:
                case 37:
                case 38:
                case 39:
                case 40:
                case 46:
                    return;
                default:
                    _ = Interaction.MsgBox("Eingabefehler Datum\nEingabeformat TTMMJJJJ oder TT.MM.JJJJ");
                    return;
            }
            checked
            {
                ((TextBox)sender).Text = ((TextBox)sender).Text != "" ? Strings.Trim(((TextBox)sender).Text) + " " + Modul1.Te[num - 113] : Modul1.Te[num - 113];
                if (num3 == 1)
                {
                    _ = TextBox3.Focus();
                }
                if (num3 == 3)
                {
                    _ = TextBox10.Focus();
                }
            }
        }
        if (num >= 113 && num <= 123 && num3 == 6 || num2 != 0)
        {
            return;
        }
        checked
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
                    ((TextBox)sender).Text = ((TextBox)sender).Text != "" ? Strings.Trim(((TextBox)sender).Text) + " " + Modul1.Te[num - 113] : Modul1.Te[num - 113];
                    ((TextBox)sender).SelectionStart = Strings.Len(((TextBox)sender).Text);
                    break;
            }
        }
    }

    private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
    {
        //Discarded unreachable code: IL_018b, IL_01ae, IL_0390, IL_0432, IL_0462, IL_0521, IL_0559, IL_05a7
        Keys num = Modul1_EingCode = (Keys)e.KeyChar;
        short num2 = default;
        switch (((TextBox)sender).Name)
        {
            case nameof(TextBox1):
                num2 = 0;
                Modul1.UbgT = "";
                Modul1.UbgT = TextBox1.Text.TrimStart().ToUpper();
                break;
            case nameof(TextBox2):
                num2 = 1;
                break;
            case nameof(TextBox3):
                num2 = 2;
                break;
            case nameof(TextBox4):
                num2 = 5;
                Modul1.UbgT = "";
                Modul1.UbgT = TextBox4.Text.TrimStart().ToUpper();
                break;
            case nameof(TextBox5):
                num2 = 6;
                Modul1.UbgT = "";
                Modul1.UbgT = TextBox5.Text.TrimStart().ToUpper();
                break;
            case nameof(TextBox6):
                num2 = 9;
                Modul1.UbgT = "";
                Modul1.UbgT = TextBox6.Text.TrimStart().ToUpper();
                break;
            case nameof(TextBox7):
                return;
            case nameof(TextBox8):
                return;
            case nameof(TextBox9):
                num2 = 3;
                break;
            case nameof(TextBox10):
                num2 = 4;
                break;
            case nameof(TextBox16):
                num2 = 58;
                Modul1.UbgT = "";
                Modul1.UbgT = TextBox16.Text.TrimStart().ToUpper();
                break;
            case nameof(TextBox17):
                num2 = 17;
                Modul1.UbgT = "";
                Modul1.UbgT = TextBox17.Text.TrimStart().ToUpper();
                break;
            case nameof(TextBox18):
                num2 = 18;
                Modul1.UbgT = "";
                Modul1.UbgT = TextBox18.Text.TrimStart().ToUpper();
                break;
        }
        short num3 = num2;
        if (num3 == 2)
        {
            short num4 = (short)num;
            if (num4 is 8 or 90 or 82 or 85 or 63 or 78 or 86 or 118 or 110 or 117 or 114 or 122)
            {
                if (TextBox2.Text.Trim() != "")
                {
                    TextBox3.Text = "";
                    goto IL_0471;
                }
                num = 0;
            }
            else if (num4 != 13)
            {
                num = 0;
            }
        }
        else if (num3 == 4)
        {
            short num5 = (short)num;
            if (num5 is 8 or 65 or 66 or 97 or 98)
            {
                if (TextBox9.Text.Trim() != "")
                {
                    TextBox10.Text = "";
                    goto IL_0471;
                }
                num = 0;
            }
            else if (num5 != 13)
            {
                num = 0;
            }
        }
        else if (num3 != 10)
        {
            if (num3 != 58)
            {
            }
            goto IL_0471;
        }
        else
        {
            goto IL_0aae;
        }
    IL_0471:
        Modul1_EingCode = num;
        if (num == Keys.Escape)
        {
            num = 0;
        }
        if (Modul1_EingCode != (Keys)127)
        {
            if (num2 == 6)
            {
            }
            switch (Modul1_EingCode)
            {
                case Keys.Back:
                    if (Modul1.UbgT.Length > 0)
                    {
                        Modul1.UbgT = Modul1.UbgT.Left(checked(Modul1.UbgT.Length - 1));
                    }
                    goto IL_060e;
                case Keys.Enter:
                    if (0 - (TextBox11.Visible ? 1 : 0) == 0)
                    {
                        Modul1_EingCode = default;
                        _ = Button5.Focus();
                    }
                    break;
                case (Keys)18:
                    _ = Interaction.MsgBox("F40");
                    goto default;
                default:
                    {
                        if (Modul1_EingCode == Keys.Enter)
                        {
                            _ = Interaction.MsgBox("F111");
                            Debugger.Break();
                            _ = TextBox11.Focus();
                            break;
                        }
                        Modul1.UbgT = Modul1.UbgT.Length == 0
                            ? Strings.Chr((short)Modul1_EingCode).AsString().ToUpper()
                            : Modul1.UbgT.TrimStart() + Strings.Chr((short)Modul1_EingCode).AsString().ToUpper();
                        goto IL_060e;
                    }
                IL_060e:
                    ListBox2.Visible = false;
                    ListBox1.Visible = false;
                    ListBox4.Visible = false;
                    if (!(Modul1.UbgT != " " | Modul1.UbgT != ""))
                    {
                        break;
                    }
                    if (Modul1.UbgT.Length > 0)
                    {
                        if (num2 == 6)
                        {
                            ListBox2.Visible = true;
                        }
                        if (Strings.Asc(Modul1.UbgT.Left(1)) == 127)
                        {
                            break;
                        }
                        if (Modul1.UbgT.Length == 0)
                        {
                            ListBox2.Visible = false;
                            ListBox1.Visible = false;
                            ListBox4.Visible = false;
                            break;
                        }
                        ListBox2.Items.Clear();
                        ListBox1.Items.Clear();
                        ListBox4.Items.Clear();
                        switch (num2)
                        {
                            case 6:
                                {
                                    DataModul.DOSB_OrtSTable.Index = "Ortsu";
                                    DataModul.DOSB_OrtSTable.Seek(">=", Modul1.UbgT);
                                    float num6 = 0f;
                                    while (!DataModul.DOSB_OrtSTable.EOF && !DataModul.DOSB_OrtSTable.NoMatch)
                                    {
                                        num6 += 1f;
                                        _ = ListBox2.Items.Add(Strings.Left(((DataModul.DOSB_OrtSTable.Fields["Name"].Value) + (new string(' ', 50))).AsString(), 50) + Strings.Right("          " + DataModul.DOSB_OrtSTable.Fields["Nr"].AsString().TrimEnd(), 10));
                                        DataModul.DOSB_OrtSTable.MoveNext();
                                        if (num6 == 200f)
                                        {
                                            break;
                                        }
                                    }
                                    ListBox2.Visible = true;
                                    _ = TextBox5.Focus();
                                    Modul1.STextles("Ortsteil", ETextKennz.I_, Modul1.UbgT, ocItems: ListBox1.Items);
                                    Modul1.STextles("Ausland", ETextKennz.S_, Modul1.UbgT, ocItems: ListBox4.Items);
                                    ListBox1.Visible = ListBox1.Items.Count > 0;
                                    ListBox4.Visible = ListBox4.Items.Count > 0;
                                    break;
                                }
                            case 5:
                                {
                                    if (eEventArt == EEventArt.eA_300)
                                    {
                                        Modul1.eTKennz = ETextKennz.E_;
                                    }
                                    else if (eEventArt == EEventArt.eA_301)
                                    {
                                        Modul1.eTKennz = ETextKennz.G_;
                                    }
                                    else
                                    {
                                        Modul1.eTKennz = eEventArt is EEventArt.eA_302 or EEventArt.eA_602 ? ETextKennz.Q_ : ETextKennz.M_;
                                    }
                                    if (Modul1.UbgT.Trim() != "")
                                    {
                                        Modul1.STextles("Ereignis.Liste2", Modul1.eTKennz, Modul1.UbgT, ocItems: ListBox2.Items);
                                    }
                                    ListBox2.Visible = true;
                                    break;
                                }
                            case 9:
                                {
                                    Modul1.STextles("Ereignis.Liste2", ETextKennz.O_, Modul1.UbgT, ocItems: ListBox2.Items);
                                    ListBox2.Visible = true;
                                    break;
                                }
                            case 0:
                                {
                                    Modul1.STextles("Ereignis.Liste2", ETextKennz.T_, Modul1.UbgT, ocItems: ListBox2.Items);
                                    ListBox2.Visible = true;
                                    break;
                                }
                            case 58:
                                {
                                    Modul1.STextles("Ereignis.Liste2", ETextKennz.tk1_, Modul1.UbgT, ocItems: ListBox2.Items);
                                    ListBox2.Visible = true;
                                    break;
                                }
                            case 17:
                                {
                                    Modul1.STextles("Ereignis.Liste2", ETextKennz.tk3_, Modul1.UbgT, ocItems: ListBox2.Items);
                                    ListBox2.Visible = true;
                                    break;
                                }
                        }
                    }
                    else
                    {
                        ListBox2.Items.Clear();
                    }
                    break;
            }
        }
        goto IL_0aae;
    IL_0aae:
        e.KeyChar = Strings.Chr((short)num);
        if (num == 0)
        {
            e.Handled = true;
        }
    }

    private void TextBox2_KeyDown(object sender, KeyEventArgs e)
    {
        schalt5 = 1;
    }

    private void TextBox11_KeyPress(object sender, KeyPressEventArgs e)
    {
        int num = Strings.Asc(e.KeyChar);
        if (num != 63 && num != 13 && num != 8 || 1 == 0)
        {
            e.Handled = true;
        }
    }

    private void TextBox1_KeyUp1(object sender, KeyEventArgs e)
    {
        //Discarded unreachable code: IL_119a
        int try0001_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        string DDatum = default;
        short num5 = default;
        short num7 = default;
        int lErl = default;
        string HT = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num4;
                    short num6;
                    string[] array;
                    short num9;
                    short num10;
                    short num11;
                    switch (try0001_dispatch)
                    {
                        default:
                            num = 1;

                            goto IL_001c;
                        case 5839:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_1229;
                                    default:
                                        goto end_IL_0001_2;
                                }
                                if (Information.Err().Number == 5)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_1229;
                                }
                                else
                                {
                                    if (Interaction.MsgBox(Conversion.ErrorToString(), mb: MessageBoxButtons.OKCancel, title: Information.Err().Number.AsString()) == DialogResult.Cancel)
                                    {
                                        ProjectData.EndApp();
                                    }
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    num4 = num2;
                                    goto IL_122d;
                                }
                            }
                        end_IL_0001_2:
                            break;
                        IL_001c:
                            if (schalt5 != 1)
                            {
                                goto end_IL_0001;
                            }
                            num = 4;
                            num5 = (short)e.KeyCode;
                            num6 = (short)unchecked((int)e.KeyData / 65536);
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            array = new string[12];
                            string name = ((TextBox)sender).Name;
                            if (name == nameof(TextBox1))
                            {
                                num7 = 0;
                            }
                            else if (name == nameof(TextBox2))
                            {
                                num7 = 1;
                            }
                            else if (name == nameof(TextBox3))
                            {
                                num7 = 2;
                            }
                            else if (name == nameof(TextBox4))
                            {
                                num7 = 5;
                            }
                            else if (name == nameof(TextBox5))
                            {
                                num7 = 6;
                            }
                            else if (name == nameof(TextBox7))
                            {
                                num7 = 9;
                            }
                            else if (name == nameof(TextBox7))
                            {
                            }
                            else if (name == nameof(TextBox8))
                            {
                                num7 = 18;
                            }
                            else if (name == nameof(TextBox9))
                            {
                                num7 = 3;
                            }
                            else if (name == nameof(TextBox10))
                            {
                                num7 = 4;
                            }
                            else if (name == nameof(TextBox11))
                            {
                                num7 = 7;
                            }
                            else if (name == nameof(TextBox18))
                            {
                                num7 = 18;
                            }
                            else if (name == nameof(TextBox16))
                            {
                                num7 = 58;
                                if (Modul1_EingCode == Keys.Enter)
                                {
                                    ListBox2.Visible = false;
                                }
                            }
                            else if (name == nameof(edtGrabNr))
                            {
                                num7 = 18;
                            }



                            switch (num7)
                            {
                                case 1:
                                case 2:
                                case 3:
                                case 4:
                                    break;
                                default:
                                    goto IL_02ef;
                            }
                            {
                                goto IL_02be;
                            }
                        IL_02be:
                            num = 59;
                            if (num5 == 40 & num5 != (short)Modul1_EingCode)
                            {
                                _ = TextBox5.Focus();
                            }
                            goto IL_035b;
                        IL_02ef:
                            num = 64;
                            if (num7 < 10)
                            {
                                if (num5 == 40 & num5 != (short)Modul1_EingCode)
                                {
                                    _ = TextBox18.Enabled ? TextBox18.Focus() : TextBox5.Focus();
                                }
                            }
                            goto IL_035b;
                        IL_035b: // <========== 3
                            num = 75;
                            DDatum = "";
                            if (unchecked(num7 == 6 && num5 == 46) & num5 != (short)Modul1_EingCode)
                            {
                                ListBox2.Visible = false;
                                ListBox1.Visible = false;
                                ListBox4.Visible = false;
                            }
                            if (unchecked((short)(0u - (num7 == 6 ? 1u : 0u)) & num5 & (short)(0u - (Modul1_EingCode == 0 ? 1u : 0u))) != 0)
                            {
                                if (num5 == 112)
                                {
                                    DDatum = ListBox3.Items[0].ToString();
                                    TextBox5.Text = DDatum;
                                    TextBox5.Tag = DDatum.Right(10);
                                }
                                else if (num5 == 113)
                                {
                                    DDatum = ListBox3.Items[1].ToString();
                                    TextBox5.Text = DDatum;
                                    TextBox5.Tag = DDatum.Right(10);
                                }
                                else if (num5 == 114)
                                {
                                    DDatum = ListBox3.Items[2].ToString();
                                    TextBox5.Text = DDatum;
                                    TextBox5.Tag = DDatum.Right(10);
                                }
                                else if (num5 == 115)
                                {
                                    DDatum = ListBox3.Items[3].ToString();
                                    TextBox5.Text = DDatum;
                                    TextBox5.Tag = DDatum.Right(10);
                                }
                                else if (num5 == 116)
                                {
                                    DDatum = ListBox3.Items[4].ToString();
                                    TextBox5.Text = DDatum;
                                    TextBox5.Tag = DDatum.Right(10);
                                }
                                else if (num5 == 117)
                                {
                                    DDatum = ListBox3.Items[5].ToString();
                                    TextBox5.Text = DDatum;
                                    TextBox5.Tag = DDatum.Right(10);
                                }
                                if (DDatum.Trim() != "")
                                {
                                    Modul1_Event_LetztOrtspei = DDatum;
                                }
                                DDatum = "";
                                if (TextBox5.Text.Trim() != "")
                                {
                                    ListBox3.Visible = false;
                                    if (TextBox11.Visible)
                                    {
                                        _ = TextBox11.Focus();
                                    }
                                    ListBox1.Visible = false;
                                    ListBox2.Visible = false;
                                    goto end_IL_0001;
                                }
                            }
                            if (num5 == 13)
                            {
                                Modul1.UbgT = "";
                                switch (num7)
                                {
                                    case 0:
                                        break;
                                    case 1:
                                    case 3:
                                        goto IL_0805;
                                    case 2:
                                    case 4:
                                        goto IL_0caf;
                                    case 5:
                                        goto IL_0ea8;
                                    case 6:
                                        goto IL_0f41;
                                    case 7:
                                        goto IL_10ae;
                                    case 8:
                                        goto IL_10c7;
                                    case 9:
                                        goto IL_10f3;
                                    case 58:
                                        goto IL_1148;
                                    case 17:
                                        goto IL_115e;
                                    default:
                                        goto IL_118a;
                                }
                                {
                                    goto IL_07d8;
                                }
                            }
                            else
                            {
                                goto IL_118a;
                            }
                        IL_07d8:
                            num = 132;
                            ListBox2.Visible = false;
                            _ = TextBox2.Focus();
                            goto IL_118a;
                        IL_0805:
                            num = 136;
                            float num8;
                            if (Strings.Len(Strings.RTrim(Strings.LTrim(((TextBox)sender).Text))) > 0 && Strings.Len(Strings.RTrim(Strings.LTrim(((TextBox)sender).Text))) < 4)
                            {
                                Interaction.Beep();
                                goto end_IL_0001;
                            }
                            else
                            {
                                num8 = Strings.InStr(((TextBox)sender).Text, ".");
                                if (num8 > 0f || Strings.Len(((TextBox)sender).Text) == 0)
                                {
                                    DDatum = Strings.Mid(((TextBox)sender).Text, 7, 4) + Strings.Mid(((TextBox)sender).Text, 4, 2) + Strings.Mid(((TextBox)sender).Text, 1, 2);
                                }
                                else
                                {
                                    Modul1.sDatu = ((TextBox)sender).Text;
                                    if (Modul1.sDatu.Length == 3)
                                    {
                                        Modul1.sDatu = "0" + Modul1.sDatu;
                                    }
                                    if (Modul1.sDatu.Length > 4 && Modul1.sDatu.Length < 8)
                                    {
                                        _ = Interaction.MsgBox("Eingabefehler ");
                                        goto end_IL_0001;
                                    }
                                    else
                                    {
                                        string text2 = Modul1.sDatu.Right(4);
                                        Modul1.sDatu = Modul1.sDatu.Left(Modul1.sDatu.Length - 4);
                                        string text3 = Modul1.sDatu.Right(2);
                                        Modul1.sDatu = Modul1.sDatu.Length > 2 ? Modul1.sDatu.Left(Modul1.sDatu.Length - 2) : "";

                                        string datu = Modul1.sDatu;
                                        Modul1.sDatu = "";
                                        num9 = (short)datu.AsInt();
                                        num10 = (short)text3.AsInt();
                                        num11 = (short)text2.AsInt();
                                        DDatum = text2 + text3 + datu;
                                        string text4 = text2 + "/";
                                        if (eEventArt != EEventArt.eA_300
                                            && eEventArt != EEventArt.eA_301
                                            && eEventArt != EEventArt.eA_302
                                            && eEventArt != EEventArt.eA_602 || 1 == 0)
                                        {
                                            if (DDatum.AsInt() > 1000.0)
                                            {
                                                Label13.Text = text4;
                                                Label13.Visible = true;
                                                Button12.Visible = true;
                                            }
                                        }
                                        HT = "";
                                        HT = DDatum.Date2DotDateStr2();
                                        ((TextBox)sender).Text = HT;
                                    }
                                }
                                if (num7 == 1)
                                {
                                    if (DDatum.Length == 4)
                                    {
                                        DDatum += "0000";
                                    }
                                    if (unchecked(0 - (TextBox3.Visible ? 1 : 0)) == 0)
                                    {
                                        _ = TextBox4.Focus();
                                        goto IL_0c95;
                                    }
                                    else
                                    {
                                        _ = TextBox3.Focus();
                                    }
                                }
                                else
                                {
                                    _ = TextBox10.Focus();
                                    Label11.Text = "Zeitraum des Datums  A= und (>And<) B=Bis";
                                }
                                DDatum = "";
                                if (((TextBox)sender).Text != "")
                                {
                                    Button8.Enabled = true;
                                }
                            }
                            goto IL_0c95;
                        IL_0c95: // <========== 3
                            num = 203;
                            lErl = 1;
                            num5 = 0;
                            goto IL_118a;
                        IL_0caf:
                            num = 207;
                            num8 = Strings.InStr(((TextBox)sender).Text, "\0");
                            if (num8 > 0f)
                            {
                                ((TextBox)sender).Text = Strings.Mid(((TextBox)sender).Text, 1, (int)Math.Round(num8 - 1f)) + " " + Strings.Mid(((TextBox)sender).Text, (int)Math.Round(num8 + 1f), Strings.Len(((TextBox)sender).Text));
                            }

                            string text = Strings.UCase(Strings.LTrim(Strings.RTrim(((TextBox)sender).Text)));
                            switch (text)
                            {
                                case "V":
                                case "N":
                                case "U":
                                case "R":
                                case "?":
                                case "Z":
                                case "A":
                                case "B":
                                case null:
                                case "":
                                    break;
                                default:
                                    goto IL_118a;
                            }
                            if (num7 == 2)
                            {
                                _ = TextBox9.Focus();
                                Label11.Text = "Datum bis";
                            }
                            else
                            {
                                _ = TextBox4.Focus();
                                Label11.Text = "Kurzbemerkung; Text";
                            }
                            goto end_IL_0001;
                        IL_0ea8:
                            num = 228;
                            ListBox2.Visible = false;
                            if (TextBox18.Visible & TextBox4.Text.Trim() != "")
                            {
                                TextBox18.Enabled = true;
                                _ = TextBox18.Focus();
                            }
                            else
                            {
                                _ = TextBox5.Focus();
                            }
                            goto IL_118a;
                        IL_0f41:
                            num = 238;
                            if (num5 == 13)
                            {
                                if (ListBox2.Visible)
                                {
                                    ListBox2.SelectedIndex = 0;
                                    TextBox5.Text = ListBox2.Text.Left(50);
                                    Modul1_Event_LetztOrtspei = ListBox2.Text;
                                    ListBox2.Visible = false;
                                    ListBox1.Visible = false;
                                    ListBox4.Visible = false;
                                    _ = TextBox11.Focus();
                                    TextBox5.Tag = Strings.Mid(ListBox2.Text, 51, 10).AsDouble().AsString();
                                }
                            }
                            if (unchecked(0 - (TextBox11.Visible ? 1 : 0)) == 0)
                            {
                                _ = TextBox4.Focus();
                                goto end_IL_0001;
                            }
                            else
                            {
                                _ = TextBox11.Focus();
                                Label11.Text = "Sicherheit des Ortes ?=Vermutet S=sicher";
                            }
                            goto IL_118a;
                        IL_10ae:
                            num = 258;
                            _ = TextBox6.Focus();
                            goto IL_118a;
                        IL_10c7:
                            num = 261;
                            _ = TextBox6.Focus();
                            _ = RichTextBox1.Focus();
                            goto IL_118a;
                        IL_10f3:
                            num = 265;
                            if (num5 == 13)
                            {
                                ListBox2.Visible = false;
                            }
                            if (eEventArt == EEventArt.eA_Death)
                            {
                                _ = TextBox17.Focus();
                            }
                            goto IL_118a;
                        IL_1148:
                            num = 273;
                            _ = TextBox4.Focus();
                            goto IL_118a;
                        IL_115e:
                            num = 276;
                            if (num5 == 13)
                            {
                                ListBox2.Visible = false;
                            }
                            goto IL_118a;
                        IL_118a: // <========== 13
                            num = 283;
                            num5 = 0;
                            goto end_IL_0001;
                        IL_1229:
                            num4 = unchecked(num2 + 1);
                            goto IL_122d;
                        IL_122d:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 56:
                                case 61:
                                case 62:
                                case 68:
                                case 71:
                                case 72:
                                case 73:
                                case 74:
                                case 75:
                                    goto IL_035b;
                                case 191:
                                case 202:
                                case 203:
                                    goto IL_0c95;
                                case 129:
                                case 134:
                                case 205:
                                case 212:
                                case 219:
                                case 224:
                                case 225:
                                case 226:
                                case 232:
                                case 235:
                                case 236:
                                case 256:
                                case 259:
                                case 263:
                                case 270:
                                case 271:
                                case 274:
                                case 278:
                                case 279:
                                case 280:
                                case 281:
                                case 282:
                                case 283:
                                    goto IL_118a;
                                case 2:
                                case 123:
                                case 138:
                                case 151:
                                case 218:
                                case 223:
                                case 252:
                                case 284:
                                case 293:
                                    goto end_IL_0001;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 5839;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001: // <========== 8
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }
    private void ListBox3_DoubleClick(object sender, EventArgs e)
    {
        TextBox5.Text = ListBox3.Text.Left(240);
        Modul1_Event_LetztOrtspei = ListBox3.Text;
        TextBox5.Tag = ListBox3.Text.Right(10);
        Button14.Visible = false;
        Label8.Visible = false;
        Button13.Visible = false;
        ListBox3.Visible = false;
        ListBox2.Visible = false;
        ListBox1.Visible = false;
        ListBox4.Visible = false;
    }

    private void Button9_Click(object sender, EventArgs e)
    {
        var sDat_Death = TextBox2.Text;
        if (sDat_Death.Right(4).AsDouble() == 0.0)
        {
            return;
        }
        if (sDat_Death.Left(2) == "  ")
        {
            StringType.MidStmtStr(ref sDat_Death, 1, 2, "28");
        }
        if (Strings.Mid(sDat_Death, 4, 2) == "  ")
        {
            StringType.MidStmtStr(ref sDat_Death, 4, 2, "12");
        }
        if (sDat_Death.Left(2) == "00")
        {
            StringType.MidStmtStr(ref sDat_Death, 1, 2, "28");
        }
        if (Strings.Mid(sDat_Death, 4, 2) == "00")
        {
            StringType.MidStmtStr(ref sDat_Death, 4, 2, "12");
        }
        if (TextBox12.Text.AsInt() > 0.0)
        {
            sDat_Death = sDat_Death.AsDate().AddYears(-TextBox12.Text.AsInt()).AsString();
        }
        if (TextBox13.Text.AsInt() > 0.0)
        {
            sDat_Death = sDat_Death.AsDate().AddMonths(-TextBox13.Text.AsInt()).AsString();
        }
        if (TextBox14.Text.AsInt() > 0.0)
        {
            sDat_Death = sDat_Death.AsDate().AddDays(-7 * TextBox13.Text.AsInt()).AsString();
        }
        if (TextBox15.Text.AsInt() > 0.0)
        {
            sDat_Death = sDat_Death.AsDate().AddDays(-TextBox13.Text.AsInt()).AsString();
        }
        if (TextBox15.Text.AsInt() == 0.0)
        {
            StringType.MidStmtStr(ref sDat_Death, 1, 2, "00");
        }
        if (TextBox14.Text.AsInt() == 0.0 && TextBox13.Text.AsInt() == 0.0 && TextBox15.Text.AsInt() == 0.0)
        {
            StringType.MidStmtStr(ref sDat_Death, 1, 2, "00");
            StringType.MidStmtStr(ref sDat_Death, 4, 2, "00");
        }
        string datum = sDat_Death;
        Label7.Text = "err. Geburtsdatum: " + datum;
        Label17.Text = datum;
        if (eEventArt <= EEventArt.eA_499)
        {
            Button10.Enabled = Label7.Text.Trim() != "";
        }
    }

    private void Button8_Click(object sender, EventArgs e)
    {
        if (eEventArt > EEventArt.eA_499)
        {
            Button10.Enabled = false;
        }
        Frame1.Visible = true;
        _ = TextBox12.Focus();
        TextBox12.Text = "";
        TextBox13.Text = "";
        TextBox14.Text = "";
        TextBox15.Text = "";
    }

    private void Button4_Click(object sender, EventArgs e)
    {
        TextBox12.Text = "";
        TextBox13.Text = "";
        TextBox14.Text = "";
        TextBox15.Text = "";
        Frame1.Visible = false;
    }

    private void Button2_Click(object sender, EventArgs e)
    {

        Modul1_ErArt = eEventArt;
        Modul1_Per1.Clear();
        short lfNR = default;
        while (Rahmen.Default.eResult == ERahmenResult.eRR_Removed)
        {
            if (Rahmen.Default.DataHolder.iPerfam > 0)
            {
                _ = Interaction.MsgBox("Einem Zeugen können während der Eingabe keine Zeugen zugefügt werden");
                return;
            }

            Modul1_PerfamNr = eEventArt > EEventArt.eA_499 ? Familie.Default.iFamNr : Personen.Default.PersonNr;

            DataModul_Witness_ForAllZeug(Modul1_PerfamNr, eEventArt, Modul1.LfNR, (cWitness) => Modul1_Per1.Add(cWitness.iPers));

            Erspeich(false);
            Rahmen.Default.Close();
            Rahmen.Default.Top = Top;
            lfNR = Modul1.LfNR;
            Rahmen.Default.ShowDialog((int)eEventArt, Modul1_Per1, EUserText.t301);
        }
        if (Rahmen.Default.eResult == ERahmenResult.eRR_OK)
        {
            ProjectData.ClearProjectError();

            Modul1.LfNR = lfNR;
            string Event_sBem4 = default;
            if ((Event_sBem4 = DataModul.Event.GetValue(Modul1_PerfamNr, Modul1_ErArt, EventFields.Bem4, (f) => f.AsString())) != ""
                && Event_sBem4.Length > 0
                && Event_sBem4.Trim() != "")
            {
                Button2.Text = "&Zeugen: Ja";
                Button2.BackColor = ColorTranslator.FromOle(65535);
            }

            if (DataModul.Witness.ExistZeug(Modul1_PerfamNr, eEventArt, Modul1.LfNR))
            {
                Button2.Text = "&Zeugen: Ja";
                Button2.BackColor = ColorTranslator.FromOle(65535);

                DataModul_Witness_ForAllZeug(Modul1_PerfamNr, eEventArt, Modul1.LfNR, (c) => Modul1_Per1.Add(c.iPers));

                Rahmen.Default.Close();
                if (Rahmen.Default.eResult != ERahmenResult.eRR_OK)
                {
                    Rahmen.Default.Top = Top;
                    Rahmen.Default.ShowDialog(Modul1.Ubg, Modul1_Per1, EUserText.t301);
                    ProjectData.ClearProjectError();
                }
            }
            else
            {
                Rahmen.Default.Close();
                if (Rahmen.Default.eResult != ERahmenResult.eRR_OK)
                {
                    Rahmen.Default.Top = Top;
                    Rahmen.Default.ShowDialog((int)eEventArt, Modul1_Per1, EUserText.t301);
                    ProjectData.ClearProjectError();
                }

            }
        }
    }

    private void DataModul_Witness_ForAllZeug(int modul1_PerfamNr, EEventArt eEventArt1, short lfNR, Action<IWitnessData> action)
    {
        if (DataModul.Witness.ExistZeug(modul1_PerfamNr, eEventArt, lfNR))
        {
            foreach (var cWitness in DataModul.Witness.ReadAllZeug(modul1_PerfamNr, eEventArt))
            {
                action(cWitness);
            }
        }
    }

    private void TextBox2_LostFocus(object sender, EventArgs e)
    {
        if (Strings.Len(Strings.RTrim(Strings.LTrim(((TextBox)sender).Text))) > 0 && Strings.Len(Strings.RTrim(Strings.LTrim(((TextBox)sender).Text))) < 4)
        {
            Interaction.Beep();
            return;
        }
        float num = Strings.InStr(((TextBox)sender).Text, ".");
        if (num > 0f || Strings.Len(((TextBox)sender).Text) == 0)
        {
            string text = Strings.Mid(((TextBox)sender).Text, 7, 4) + Strings.Mid(((TextBox)sender).Text, 4, 2) + Strings.Mid(((TextBox)sender).Text, 1, 2);
            return;
        }
        Modul1.sDatu = ((TextBox)sender).Text;
        if (Modul1.sDatu.Length == 3)
        {
            Modul1.sDatu = "0" + Modul1.sDatu;
        }
        if (Modul1.sDatu.Length > 4 && Modul1.sDatu.Length < 8)
        {
            _ = Interaction.MsgBox("Eingabefehler ");
            return;
        }
        string text2 = Modul1.sDatu.Right(4);
        checked
        {
            Modul1.sDatu = Modul1.sDatu.Left(Modul1.sDatu.Length - 4);
            string text3 = Modul1.sDatu.Right(2);
            Modul1.sDatu = Modul1.sDatu.Length > 2 ? Modul1.sDatu.Left(Modul1.sDatu.Length - 2) : "";
            string datu = Modul1.sDatu;
            Modul1.sDatu = "";
            short num2 = (short)datu.AsInt();
            short num3 = (short)text3.AsInt();
            short num4 = (short)text2.AsInt();
            string text = text2 + text3 + datu;
            string text4 = text2 + "/";
            if ((eEventArt != EEventArt.eA_300
                && eEventArt != EEventArt.eA_301
                && eEventArt != EEventArt.eA_302
                && eEventArt != EEventArt.eA_601
                && eEventArt != EEventArt.eA_602
                || 1 == 0)
                && text.AsInt() > 1000.0)
            {
                Label13.Text = text4;
                Label13.Visible = true;
                Button12.Visible = true;
            }
            ((TextBox)sender).Text = text.Date2DotDateStr2();
        }
    }

    private void Button13_Click(object sender, EventArgs e)
    {
        MainProject.Forms.Ortsver.Close();
        MainProject.Forms.Ortsver.Show();
        MainProject.Forms.Ortsver.Button11.PerformClick();
        MainProject.Forms.Ortsver.Button10.Visible = false;
        MainProject.Forms.Ortsver.Button12.Visible = false;
        MainProject.Forms.Ortsver.Button19.Visible = true;
        MainProject.Forms.Ortsver.edtPlace.Text = TextBox5.Text.Left(1).ToUpper() + Strings.Mid(TextBox5.Text, 2, TextBox5.Text.Trim().Length);
        MainProject.Forms.Ortsver.edtPlace.SelectionStart = MainProject.Forms.Ortsver.edtPlace.Text.Length;
    }

    private void Button14_Click_1(object sender, EventArgs e)
    {
        Label8.Visible = false;
        TextBox5.Text = Modul1_Event_LetztOrtspei.Left(50);
        TextBox5.Tag = Modul1_Event_LetztOrtspei.Right(10);
        Button14.Visible = false;
        Label8.Visible = false;
        Button13.Visible = false;
        ListBox2.Visible = false;
        ListBox1.Visible = false;
        ListBox4.Visible = false;
        _ = TextBox11.Focus();
    }

    private void Button10_Click(object sender, EventArgs e)
    {
        if (!DataModul.Event.Exists(EEventArt.eA_Birth, Modul1.PersInArb, 0))
        {
            DataModul.Event.AppendRaw((EEventArt.eA_Birth, Modul1.PersInArb, 0));
        }
        var sDat_Birth = Label7.Text.Right(10).Trim(); //??
        checked
        {
            if (sDat_Birth.Length > 0)
            {
                var A = sDat_Birth.IndexOf('.');
                string text = sDat_Birth.Left(A - 1).Trim().PadLeft(2, '0');
                sDat_Birth = sDat_Birth.Substring(A);
                A = sDat_Birth.IndexOf('.');
                string text2 = sDat_Birth.Left(A - 1).Trim().PadLeft(2, '0');
                sDat_Birth = sDat_Birth.Substring(A);
                string text3 = sDat_Birth.Left(4).PadLeft(4, '0');
                sDat_Birth = text3 + text2 + text;
            }
            DataModul.Event.ReadData(EEventArt.eA_Birth, Modul1.PersInArb, out var cEvent, 0);
            cEvent.SetPropValue(EEventProp.dDatumV, sDat_Birth.AsInt());
            cEvent.SetPropValue(EEventProp.sDatumV_S, "r");
            cEvent.Update();
            Frame1.Visible = false;
        }
    }

    private void Button11_Click(object sender, EventArgs e)
    {
        Familie.Default.lstChildren.SelectedIndex = 0;
        TextBox2.Text = Strings.Mid(Familie.Default.lstChildren.Text, 4, 10);
        TextBox2.Tag = (Familie.Default.lstChildren.Items[0] as ListItem).ItemData.AsInt();
        Button5.PerformClick();
    }

    public void Erspeich(bool Ortf)
    {
        //Discarded unreachable code: IL_1db7
        Ortf = false;
        int try0001_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        int num4 = default;
        int num5 = default;
        int num7 = default;
        int ubg = default;
        int num10 = default;
        int num11 = default;
        int lErl = default;

        int num6;
        MainProject.MyForms forms;
        Form Formtocheck;
        short num8;
        string left;
        float i;
        float num12;
        int I = default;

        if (Modul1.Typ == DriveType.CDRom)
        {
            Modul1.Aend = 0f;
            Modul1.Trans = 0;
            Close();
            return;
        }
        int iNum = (short)Math.Round(Topsch.Left(3).AsDouble());
        iNum = 0;
        LeerPruef(Modul1_ErArt);
        if (Modul1.ErSchalt == 0)
        {
            if (eEventArt > EEventArt.eA_499)
            {
                Modul1.PFSatz = Familie.Default.iFamNr;
                Modul1.Aendf = true;
            }
            else
            {
                Modul1.PFSatz = Personen.Default.PersonNr;
            }
            DataModul.Event.Delete((eEventArt, Modul1.PFSatz, Modul1.LfNR));
            Close();
            return;
        }
        else
        {
            if (eEventArt > EEventArt.eA_499)
            {
                Modul1.PFSatz = Familie.Default.iFamNr;
            }
            else
            {
                forms = MainProject.Forms;
                Formtocheck = forms.Personen;
                num8 = Modul1.IsFormloaded(Formtocheck);
                forms.Personen = (Personen)Formtocheck;
                Modul1.PFSatz = num8 == -1 ? Personen.Default.PersonNr : Modul1.PersInArbsp;
            }
            ProjectData.ClearProjectError();
            num3 = 2;
            if (!DataModul.Event.Exists((eEventArt, Modul1.PFSatz, Modul1.LfNR)))
            {
                DataModul.Event.AppendRaw((eEventArt, Modul1.PFSatz, Modul1.LfNR));
            }
            DataModul.Event.ReadData(eEventArt, Modul1.PFSatz, out var cEvent, Modul1.LfNR);
            if (TextBox4.Text.Trim().Length > 0)
            {
                Modul1.UbgT = TextBox4.Text.Trim();
                Modul1.eTKennz = eEventArt switch
                {
                    EEventArt.eA_300 => ETextKennz.E_,
                    EEventArt.eA_301 => ETextKennz.G_,
                    EEventArt.eA_302 => ETextKennz.Q_,
                    EEventArt.eA_602 => ETextKennz.Q_,
                    EEventArt.eA_603 => ETextKennz.M_,
                    _ => ETextKennz.M_,
                };
                iNum = Modul1.TextSpeich(Modul1.UbgT, Modul1.UbgT1, Modul1.eTKennz, Modul1.PersInArb, Modul1.LfNR);
                cEvent.SetPropValue(EEventProp.iKBem, iNum);
            }
            else
                cEvent.SetPropValue(EEventProp.iKBem, 0);

            if (TextBox1.Text.Trim().Length > 0)
            {
                Modul1.UbgT = TextBox1.Text.Trim();
                Modul1.eTKennz = ETextKennz.T_;
                iNum = Modul1.TextSpeich(Modul1.UbgT, Modul1.UbgT1, Modul1.eTKennz, Modul1.PersInArb, Modul1.LfNR);
                cEvent.SetPropValue(EEventProp.iArtText, iNum);
            }
            else
                cEvent.SetPropValue(EEventProp.iArtText, 0);
            iNum = 0;
            if (TextBox16.Text.Trim().Length > 0)
            {
                Modul1.UbgT = TextBox16.Text.Trim();
                Modul1.eTKennz = ETextKennz.tk1_;
                iNum = Modul1.TextSpeich(Modul1.UbgT, Modul1.UbgT1, Modul1.eTKennz, Modul1.PersInArb, Modul1.LfNR);
                cEvent.SetPropValue(EEventProp.iDatumText, iNum);
            }
            else
                cEvent.SetPropValue(EEventProp.iDatumText, 0);

            if (TextBox6.Text.Trim().Length > 0)
            {
                Modul1.UbgT = TextBox6.Text.Trim();
                Modul1.eTKennz = ETextKennz.O_;
                iNum = Modul1.TextSpeich(Modul1.UbgT, Modul1.UbgT1, Modul1.eTKennz, Modul1.PersInArb, Modul1.LfNR);
                cEvent.SetPropValue(EEventProp.iPlatz, iNum);
            }
            else
                cEvent.SetPropValue(EEventProp.iPlatz, 0);

            if (TextBox17.Text.Trim().Length > 0)
            {
                Modul1.UbgT = TextBox17.Text.Trim();
                Modul1.eTKennz = ETextKennz.tk3_;
                iNum = Modul1.TextSpeich(Modul1.UbgT, Modul1.UbgT1, Modul1.eTKennz, Modul1.PersInArb, Modul1.LfNR);
                cEvent.SetPropValue(EEventProp.iCausal, iNum);

                if (TextBox19.Text.Trim().Length > 0)
                {
                    Modul1.UbgT = TextBox19.Text.Trim();
                    Modul1.eTKennz = ETextKennz.tk4_;
                    num5 = 0;
                    iNum = Modul1.TextSpeich(Modul1.UbgT, Modul1.UbgT1, Modul1.eTKennz, Modul1.PersInArb, Modul1.LfNR);
                    cEvent.SetPropValue(EEventProp.iAn, iNum);
                }
                else
                    cEvent.SetPropValue(EEventProp.iAn, 0);

            }
            else
                cEvent.SetPropValue(EEventProp.iCausal, 0);

            if (edtGrabNr.Text.Trim().Length > 0)
            {
                Modul1.UbgT = edtGrabNr.Text.Trim();
                Modul1.eTKennz = ETextKennz.tk6_;
                iNum = Modul1.TextSpeich(Modul1.UbgT, Modul1.UbgT1, Modul1.eTKennz, Modul1.PersInArb, Modul1.LfNR);
                cEvent.SetPropValue(EEventProp.iGrabNr, iNum);
            }
            else
                cEvent.SetPropValue(EEventProp.iGrabNr, 0);

            if (TextBox18.Text.Trim().Length > 0)
            {
                Modul1.UbgT = TextBox18.Text.Trim();
                Modul1.eTKennz = ETextKennz.tk5_;
                iNum = Modul1.TextSpeich(Modul1.UbgT, Modul1.UbgT1, Modul1.eTKennz, Modul1.PersInArb, Modul1.LfNR);
                cEvent.SetPropValue(EEventProp.iHausNr, iNum);
            }
            else
                cEvent.SetPropValue(EEventProp.iHausNr, 0);

            Label18.Text = "";
            Label18.Tag = 0;
            Label19.Tag = 0;
            Label19.Text = "";
            if (TextBox5.Text.Trim() == "")
            {
                TextBox5.Tag = "";
            }
            if (TextBox2.Text.Trim() != "")
            {
                Label18.Text = Strings.Mid(TextBox2.Text, 7, 4) + Strings.Mid(TextBox2.Text, 4, 2) + TextBox2.Text.Left(2);
                Label18.Text = (Label18.Text.Trim() + "0000").Left(8);
                Label18.Tag = Label18.Text.AsInt();
            }
            else
            {
                Label18.Text = "";
                Label18.Tag = 0;
            }
            if (TextBox9.Text.Trim() != "")
            {
                Label19.Text = Strings.Mid(TextBox9.Text, 7, 4) + Strings.Mid(TextBox9.Text, 4, 2) + TextBox9.Text.Left(2);
                Label19.Text = (Label19.Text.Trim() + "0000").Left(8);
                Label19.Tag = Label19.Text.AsInt();
            }
            else
            {
                Label19.Text = "";
                Label19.Tag = 0;
            }
            if (Label18.Text != "" && Label19.Text != "")
            {
                if (Label18.Tag.AsInt() > Label19.Tag.AsInt())
                {
                    _ = Interaction.MsgBox(" Eingabefehler, Endedatum muß größer sein wie Anfangsdatum!");
                    Ortf = true;
                    return;
                }
            }
            cEvent.SetPropValue(EEventProp.dDatumV, Label18.Tag.AsDate());
            cEvent.SetPropValue(EEventProp.sDatumV_S, TextBox3.Text.Trim().Left(1));
            cEvent.SetPropValue(EEventProp.dDatumB, Label19.Tag.AsDate());

            if (TextBox10.Text.TrimStart().TrimEnd() != "")
            {
                cEvent.SetPropValue(EEventProp.sDatumB_S, TextBox10.Text.Trim().Left(1));
            }
            else if (cEvent.sDatumV_S.ToUpper() == "Z" && cEvent.dDatumB != default)
            {
                cEvent.SetPropValue(EEventProp.sDatumB_S, "a");
            }
            else
                cEvent.SetPropValue(EEventProp.sDatumB_S, "");

            TextBox11.Text = TextBox11.Text.Trim();
            if (TextBox11.Text == "")
            {
                TextBox11.Text = " ";
            }
            cEvent.SetPropValue(EEventProp.sOrt_S, TextBox11.Text.Trim().Left(1));

            TextBox8.Text = TextBox8.Text.Trim();
            if (TextBox8.Text == "")
            {
                TextBox8.Text = TextBox8.Text.Trim() + " ";
            }
            if (Check1.Visible)
            {
                cEvent.SetPropValue(EEventProp.sReg, Check1.Checked ? "1" : (object)" ");
            }
            else
                cEvent.SetPropValue(EEventProp.sReg, TextBox8.Text);

            if (CheckBox2.Visible)
            {
                if (cEvent.dDatumV != default
                    || cEvent.dDatumB != default)
                {
                    cEvent.SetPropValue(EEventProp.xIsDead, false);
                }
                else
                    cEvent.SetPropValue(EEventProp.xIsDead, CheckBox2.Checked);
            }
            Check1.Visible = false;

            if (RichTextBox1.Text.Trim() != "")
            {
                I = 1;
                while (I <= 20
                    && RichTextBox1.Text.Trim() != ""
                    && Strings.Asc(RichTextBox1.Text.Right(1)) < 14)
                {
                    RichTextBox1.Text = RichTextBox1.Text.Left(RichTextBox1.Text.Length - 1);
                    I++;
                }
            }

            if (RichTextBox1.Text == "")
            {
                RichTextBox1.Text = " ";
            }

            Ortnr = TextBox5.Tag.AsInt();
            if (Ortnr > 0)
            {
                if (Label8.Text.Trim() != "")
                {
                    string item = TextBox5.Text + new string(' ', 240).Left(240) + Strings.Right(((new string(' ', 10)) + (TextBox5.Tag)).AsString(), 10);
                    ListBox3.Items.Insert(0, item);
                    item = "";
                }
            }
            if (cEvent.iOrt != Ortnr)
            {
                Modul1.Trans = 1;
            }
            lErl = 3;
            if (RichTextBox2.Text.Trim() != "")
            {
                I = 1;
                while (I <= 20
                    && RichTextBox2.Text.Trim() != ""
                    && Strings.Asc(RichTextBox2.Text.Right(1)) < 14)
                {
                    RichTextBox2.Text = RichTextBox2.Text.Left(RichTextBox2.Text.Length - 1);
                    I++;
                }
            }
            if (RichTextBox2.Text == "")
                RichTextBox2.Text = " ";

            cEvent.SetPropValue(EEventProp.iOrt, Ortnr);
            cEvent.sBem[1] = RichTextBox1.Text;
            cEvent.sBem[2] = RichTextBox2.Text;
            if (cEvent.sBem[2] == "")
            {
                cEvent.sBem[2] = " ";
            }
            cEvent.SetPropValue(EEventProp.iPlatz, num10);
            cEvent.SetPropValue(EEventProp.iCausal, num11);
            cEvent.SetPropValue(EEventProp.iAn, num5);
            cEvent.SetPropValue(EEventProp.iDatumText, num4);
            cEvent.SetPropValue(EEventProp.sZusatz, TextBox7.Text);
            cEvent.SetPropValue(EEventProp.sVChr, CheckBox1.CheckState);
            cEvent.SetPropValue(EEventProp.iHausNr, num7);
            cEvent.SetPropValue(EEventProp.iGrabNr, ubg);
            cEvent.SetPropValue(EEventProp.iPrivacy, Modul1_priv);
            cEvent.Update();
        }
    }
    private void RichTextBox1_KeyDown(object sender, KeyEventArgs e)
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
                        RichTextBox1.SelectedText = Modul1.Te[num - 113];
                        break;
                }
            }
        }
    }

    private void RichTextBox1_KeyUp(object sender, KeyEventArgs e)
    {
        checked
        {
            short num = (short)e.KeyCode;
            short num2 = (short)unchecked((int)e.KeyData / 65536);
            Modul1.Aend = 1f;
        }
    }

    private void RichTextBox2_KeyDown(object sender, KeyEventArgs e)
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
                        RichTextBox2.SelectedText = Modul1.Te[num - 113];
                        break;
                }
            }
        }
    }

    private void RichTextBox2_KeyUp(object sender, KeyEventArgs e)
    {
        checked
        {
            short num = (short)e.KeyCode;
            short num2 = (short)unchecked((int)e.KeyData / 65536);
            Modul1.Aend = 1f;
        }
    }

    private void TextBox12_KeyPress(object sender, KeyPressEventArgs e)
    {
        int num = Strings.Asc(e.KeyChar);
        if ((num < 48 || num > 57) && num != 8 && num != 13 || 1 == 0)
        {
            e.Handled = true;
        }
    }

    private void TextBox12_TextChanged(object sender, EventArgs e)
    {
        Label7.Text = "";
        Button9.Enabled = TextBox12.Text.Trim() + TextBox13.Text.Trim() + TextBox14.Text.Trim() + TextBox15.Text.Trim() != "";
    }

    private void Label3_Click(object sender, EventArgs e)
    {
        Button12.Visible = false;
        checked
        {
            Modul1.Posi[1] = (short)RichTextBox1.Top;
            Modul1.Posi[2] = (short)RichTextBox1.Height;
            Modul1.Posi[3] = (short)RichTextBox2.Top;
            Modul1.Posi[4] = (short)RichTextBox2.Height;
            RichTextBox1.Top = 25;
            RichTextBox1.Height = Button7.Top - 40;
            RichTextBox2.Visible = false;
            Button7.Visible = true;
        }
    }

    private void Button7_Click(object sender, EventArgs e)
    {
        RichTextBox1.Top = Modul1.Posi[1];
        RichTextBox1.Height = Modul1.Posi[2];
        RichTextBox2.Top = Modul1.Posi[3];
        RichTextBox2.Height = Modul1.Posi[4];
        RichTextBox1.Visible = true;
        RichTextBox2.Visible = true;
        Button12.Visible = true;
        Button7.Visible = false;
    }

    private void Label12_Click(object sender, EventArgs e)
    {
        checked
        {
            Modul1.Posi[1] = (short)RichTextBox1.Top;
            Modul1.Posi[2] = (short)RichTextBox1.Height;
            Modul1.Posi[3] = (short)RichTextBox2.Top;
            Modul1.Posi[4] = (short)RichTextBox2.Height;
            Button12.Visible = false;
            RichTextBox2.Top = 25;
            RichTextBox2.Height = Button7.Top - 40;
            RichTextBox1.Visible = false;
            Button7.Visible = true;
        }
    }

    private void Button15_Click(object sender, EventArgs e)
    {
        Clipboard.SetText(Label7.Text.Right(10));
    }

    public void Pruefaend()
    {
        Modul1.FAendmerk = false;
        Modul1.PAendmerk = false;
        int Modul1_priv = 0;
        if (RadioButton3.Checked)
        {
            Modul1_priv = 0;
        }
        else if (RadioButton2.Checked)
        {
            Modul1_priv = 1;
        }
        else if (RadioButton1.Checked)
        {
            Modul1_priv = 2;
        }
        Modul1.PFSatz = eEventArt > EEventArt.eA_499 ? Familie.Default.iFamNr : Personen.Default.PersonNr;
        if (DataModul.Event.ReadData((eEventArt, Modul1.PFSatz, Modul1.LfNR), out var cEvent)
            && ((cEvent.eArt == EEventArt.eA_105 || cEvent.eArt == EEventArt.eA_603)
                && (cEvent.iArtText > 0 || TextBox1.Text != "")
                && TextBox1.Text != cEvent.sArtText
                || eEventArt == EEventArt.eA_Death && CheckBox2.Checked
                || cEvent.dDatumV != TextBox2.Tag.AsDate()
                || cEvent.sDatumV_S.Trim() != TextBox3.Text
                || (cEvent.iKBem > 0 || TextBox4.Text != "") && TextBox4.Text != cEvent.sKBem
                || (cEvent.iCausal > 0 || TextBox17.Text != "") && TextBox17.Text != cEvent.sCausal
                || (cEvent.iAn > 0 || TextBox19.Text != "") && TextBox19.Text != cEvent.sAn
                || cEvent.iOrt != TextBox5.Tag.AsInt()
                || (cEvent.dDatumB != TextBox9.Tag.AsDate())
                || (cEvent.iPlatz > 0 || TextBox6.Text != "") && TextBox6.Text != cEvent.sPlatz
                || cEvent.sZusatz != TextBox7.Text
                || (cEvent.iGrabNr > 0 || edtGrabNr.Text != "") && edtGrabNr.Text != cEvent.sGrabNr
                || cEvent.sReg != TextBox8.Text
                || cEvent.sDatumB_S != TextBox10.Text
                || cEvent.sOrt_S != TextBox11.Text
                || cEvent.sBem[1].Trim() != RichTextBox1.Text
                || cEvent.sBem[2].Trim() != RichTextBox2.Text
                || cEvent.iPrivacy != Modul1_priv))
        {
            if (eEventArt > EEventArt.eA_499)
            {
                Modul1.FAendmerk = true;
            }
            else
            {
                Modul1.PAendmerk = true;
            }

        }
        else
        {
            Close();

        }
    }

    private void TextBox17_KeyUp(object sender, KeyEventArgs e)
    {
        short num = checked((short)e.KeyCode);
        if (num == 13)
        {
            _ = TextBox19.Focus();
        }
    }

    private void Button16_Click(object sender, EventArgs e)
    {
        TextBox17.Text = TextBox4.Text;
        TextBox4.Text = "";
    }

    private void Button17_Click(object sender, EventArgs e)
    {
        string[] array = edtGrabNr.Text.Split('^');
        _ = Process.Start("http://www.verdener-familienforscher.de/verden/judenfriedhof/index.php?id=GP&ia=" + array[1] + "&ib=" + array[0]);
    }

    public DialogResult ShowEventDialog(EEventArt iEvent)
    {
        eEventArt = iEvent;
        return ShowDialog();
    }

}
