using BaseLib.Helper;
using Gen_FreeWin.Main;
using GenFree;
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces.Sys;
using GenFree.Interfaces.VB;
using GenFree.ViewModels.Interfaces;
using GenFreeWin.Views;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Gen_FreeWin.Views;

internal partial class Ahnen : Form
{
    private static readonly List<WeakReference> __ENCList = new();

    private IContainer components;
    IModul1 Modul1 => _Modul1.Instance;
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

    public ToolTip ToolTip1;

    /*
     [AccessedThroughProperty(nameof(btnCancel4))]
     private ControlArray<Button> _Command1;

     [AccessedThroughProperty(nameof(Frame2))]
     private ControlArray<GroupBox> _Frame2;

     [AccessedThroughProperty(nameof(lblEnterLicence))]
     private ControlArray<Label> Label1;

     [AccessedThroughProperty(nameof(lblState))]
     private ControlArray<Label> _Label2;

     [AccessedThroughProperty(nameof(Option1))]
     private ControlArray<RadioButton> _Option1;
 */
    private string _ahnNr;
    private int _startgen;
    private int _maxGen;
    private int _genMax;
    private string _a1;
    private string _nachnr;
    private string Datum;
    private string Fami;
    private short AI;
    private string Modul1_Persex;
    private int Gen;
    private short D;
    private string Spitz;
    private int Perprüf;
    private int Z;
    private string Modul1_Gen1;
    private int Modul1_Nr1;


    /*
    public virtual ControlArray<Button> btnCancel4
    {
        [DebuggerNonUserCode]
        get =>  _Command1;
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerNonUserCode]
        set
        {
            EventHandler obj = Command1_1_Click;
            if (_Command1 != null)
            {
                _Command1.RemoveClick(obj);
            }
            _Command1 = value;
            if (_Command1 != null)
            {
                _Command1.AddClick(obj);
            }
        }
    }

    public ControlArray<GroupBox> Frame2;
    public ControlArray<Label> lblEnterLicence;
    public ControlArray<Label> lblState;
    public ControlArray<RadioButton> Option1;    */
    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams createParams = base.CreateParams;
            createParams.ClassStyle |= 512;
            return createParams;
        }
    }

    [DebuggerNonUserCode]
    public Ahnen()
    {
        Load += Ahnen_Load;
        FormClosing += Ahnen_FormClosing;
        lock (__ENCList)
        {
            __ENCList.Add(new WeakReference(this));
        }
        InitializeComponent();
    }

    [DebuggerNonUserCode]
    protected override void Dispose(bool Disposing)
    {
        if (Disposing && components != null)
        {
            components.Dispose();
        }
        base.Dispose(Disposing);
    }

    private void Command1_Click(object eventSender, EventArgs eventArgs)
    {
        //Discarded unreachable code: IL_1f66, IL_21c0
        int try0001_dispatch = -1;
        int num = default;
        short index = default;
        int num2 = default;
        int num3 = default;
        int number = default;
        string persex = default;
        short num5 = default;
        int num7 = default;
        int num8 = default;
        string persex2 = default;
        short num11 = default;
        int num12 = default;
        int lErl = default;
        int num14 = default;
        int num15 = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;

                checked
                {
                    int num4 = 0;
                    switch (try0001_dispatch)
                    {
                        default:
                            num = 1;
                            index = short.Parse($"{((Button)eventSender).Tag}");
                            goto IL_Start;
                        case 10229:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 1:
                                        num4 = unchecked(num2 + 1);
                                        goto IL_Restart;
                                    case 2:
                                    case 3:
                                        goto IL_L359;
                                    case 4:
                                    case 5:
                                        goto IL_L353;
                                    case 6:
                                        break;
                                    default:
                                        goto end_IL_0001;
                                }
                                goto IL_L321;
                            }

                        IL_Restart:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 2:
                                    goto IL_Start;
                                case 47:
                                case 48:
                                    goto IL_L048;
                                default:
                                    goto end_IL_0001;
                                case 4:
                                case 35:
                                case 42:
                                case 45:
                                case 46:
                                case 76:
                                case 316:
                                case 319:
                                case 320:
                                case 339:
                                case 354:
                                case 362:
                                case 365:
                                    goto end_IL_0001_2;
                            }
                            goto default;

                        IL_Start:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            switch (index)
                            {
                                case 0:
                                    break;
                                case 1:
                                    goto IL_L048;
                                case 2:
                                    Close();
                                    goto end_IL_0001_2;
                                default:
                                    goto end_IL_0001_2;
                            }
                            _Label1_0.Visible = true;
                            _Label1_3.Text = "";
                            _Label1_3.Visible = true;
                            DataModul.DT_AncesterTable.Close();
                            DataModul.DT_KindAhnTable.Close();
                            DataModul.TempDB.TryExecute("DROP Table Ahnen1");
                            DataModul.TempDB.TryExecute("DROP Table Ahnew");
                            var ancCtx = PrepareAncesterCalculation();
                            if (ancCtx == null)
                            {
                                goto end_IL_0001_2;
                            }
                            goto end_IL_0001_2;

                        IL_L048:
                            num = 48;
                            _Label2_0.Text = "";
                            _Label2_1.Text = "";
                            _Label2_2.Text = "";
                            _Label1_0.Visible = true;
                            _Label1_3.Visible = true;
                            DataModul.DT_DescendentTable.Close();
                            DataModul.TempDB.TryExecute("DROP Table Nachk");
                            var descCtx = PrepareDescendentCalculation();
                            num3 = 3;
                            if (descCtx == null)
                            {
                                goto end_IL_0001_2;
                            }

                            Modul1_Persex = DataModul.Person.GetSex(Modul1.PersInArb);
                            Gen = 1;
                            DataModul.DT_DescendentTable.AddNew();
                            DataModul.DT_DescendentTable.Fields["Nr"].Value = "  " + Gen.AsString().Right(2);
                            DataModul.DT_DescendentTable.Fields["Gen"].Value = "  " + Gen.AsString().Right(2);
                            DataModul.DT_DescendentTable.Fields["Pr"].Value = Modul1.PersInArb;
                            DataModul.DT_DescendentTable.Update();
                            persex = Modul1_Persex;
                            if (persex == "M")
                            {
                                Modul1.eLKennz = ELinkKennz.lkFather;
                            }
                            else if (persex == "F")
                            {
                                Modul1.eLKennz = ELinkKennz.lkMother;
                            }
                            var aiFams = Modul1.Link_Famsuch(Modul1.PersInArb, Modul1.eLKennz);
                            if (aiFams.Count > 1)
                            {
                                Mehrehezeig(aiFams, List1);
                            }
                            CollectDescendentChildren(aiFams);
                            Gen++;
                            PersistDescendentGeneration();
                            goto IL_L178;
                        IL_L178:
                            num = 178;
                            while (DataModul.NB_OrtindTable.RecordCount > 0)
                            {
                                DataModul.NB_OrtindTable.MoveFirst();
                                Gen = (short)Math.Round(Conversion.Val(DataModul.NB_OrtindTable.Fields["Ort"].AsString().Left(2)) + 1.0);
                                if (!(Gen > Modul1_Gen1.AsInt()))
                                {
                                    _nachnr = DataModul.NB_OrtindTable.Fields["Name"].AsString();
                                    Modul1.PersInArb = DataModul.NB_OrtindTable.Fields["OrtNr"].AsInt();
                                    DataModul.NB_OrtindTable.Delete();
                                    Modul1_Persex = DataModul.Person.GetSex(Modul1.PersInArb);
                                    persex2 = Modul1_Persex;
                                    if (persex2 == "M")
                                    {
                                        Modul1.eLKennz = ELinkKennz.lkFather;
                                    }
                                    else if (persex2 == "F")
                                    {
                                        Modul1.eLKennz = ELinkKennz.lkMother;
                                    }
                                    aiFams = Modul1.Link_Famsuch(Modul1.PersInArb, Modul1.eLKennz);
                                    if (aiFams.Count > 1)
                                    {
                                        Mehrehezeig(aiFams, List1);
                                    }
                                    List1.Items.Clear();
                                    Fami = Modul1.UbgT;
                                    num11 = (short)Math.Round(Fami.Length / 10.0);
                                    AI = 1;
                                    while (AI <= num11)
                                    {
                                        Modul1.FamInArb = (int)Math.Round(Fami.Left(10).AsDouble());
                                        Fami = Strings.Mid(Fami, 11, Fami.Length);
                                        DataModul.Link.ReadFamily(Modul1.FamInArb, Modul1.Family);
                                        num8 = 1;
                                        Datum = "";
                                        while (Modul1.Family.Kind[num8] != 0 && (num8 <= 99))
                                        {
                                            if (Modul1.Family.Kinder[num8].aTxt != "A")
                                            {
                                                Modul1.PersInArb = Modul1.Family.Kind[num8];
                                                var Datu = DataModul.Event.GetPersonBirthOrBapt(Modul1.PersInArb);
                                                if (Datu != default)
                                                {
                                                    Datum = Strings.Right("00000000" + Datu.AsInt().AsString(), 8);
                                                }
                                                else
                                                    Datum = new string(' ', 8);
                                                _ = List1.Items.Add(Datum + "  " + AI.AsString().Right(2) + Modul1.PersInArb.AsString());
                                            }
                                            num8++;
                                        }
                                        AI++;
                                    }
                                    PersistDescendentGenerationWithKia();
                                }
                            }
                            lErl = 44;
                            _Label1_3.Text = "";
                            _Label1_3.Visible = false;
                            _Label1_0.Visible = false;
                            num14 = 0;
                            num15 = unchecked(0 - (num15 == 0 ? 1 : 0));
                            DataModul.DT_DescendentTable.Index = "GNR";
                            DataModul.DT_DescendentTable.MoveFirst();
                            while (!DataModul.DT_DescendentTable.EOF)
                            {
                                if (num15 != DataModul.DT_DescendentTable.Fields["gen"].AsInt())
                                {
                                    num15 = DataModul.DT_DescendentTable.Fields["gen"].AsInt();
                                    num8 = 0;
                                }
                                num8++;
                                DataModul.DT_DescendentTable.Edit();
                                DataModul.DT_DescendentTable.Fields["LfNr"].Value = num8;
                                DataModul.DT_DescendentTable.Update();
                                DataModul.DT_DescendentTable.MoveNext();
                                num14++;
                            }
                            DataModul.DT_DescendentTable.Index = "NR";
                            DataModul.DT_DescendentTable.MoveFirst();
                            Modul1.PersInArb = DataModul.DT_DescendentTable.Fields["Pr"].AsInt();
                            _Label2_0.Text = "Nachfahren berechnet für:   Personenziffer " + Modul1.PersInArb.AsString();
                            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                            if (Modul1.Person.Prefix.Trim() != "")
                            {
                                Modul1.Person.SetFullSurname(Modul1.Person.Prefix.Trim() + " " + Modul1.Person.SurName.Trim());
                            }
                            if (Modul1.Person.Suffix.Trim() != "")
                            {
                                Modul1.Person.SetFullSurname(Modul1.Person.SurName.Trim() + " " + Modul1.Person.Suffix.Trim());
                            }
                            _Label2_1.Text = Modul1.Person.Givennames + " " + Modul1.Person.FullSurName;
                            DataModul.DT_DescendentTable.Index = "GNr";
                            DataModul.DT_DescendentTable.MoveLast();
                            _Label2_2.Text = "  " + DataModul.DT_DescendentTable.RecordCount.AsString() + " Personen in" + DataModul.DT_DescendentTable.Fields["Gen"].AsInt().AsString() + " Generationen";
                            _ = Interaction.MsgBox("Berechnung fertig");
                            Close();
                            Menue.Default.Show();
                            goto end_IL_0001_2;
                        //==========================
                        IL_L321: // Standard-Fehlerbehandlung
                            num = 321;
                            _ = Interaction.MsgBox(Information.Err().Number.AsString());
                            number = Information.Err().Number;
                            if (number == 6)
                            {
                                _ = Interaction.MsgBox("5");
                                Debugger.Break();
                                Modul1.Schalt = 2;
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = unchecked(num2 + 1);
                                goto IL_Restart;
                            }
                            else if (number == 55)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = unchecked(num2 + 1);
                                goto IL_Restart;
                            }
                            else if (number == 3021)
                            {
                                _ = Interaction.MsgBox("Berechnet wurden" + _genMax.AsString() + " Generationen", mb: MessageBoxButtons.OK, title: "Fertig");
                                Close();
                                goto end_IL_0001_2;
                            }
                            else if (number == 3022)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num2 = 0;
                                goto IL_L178;
                            }
                            Modul1.UbgT = "IsFamLes, Error =" + Information.Err().Number.AsString();
                            _ = Interaction.MsgBox(Conversion.ErrorToString(), mb: MessageBoxButtons.OK, title: Modul1.UbgT);
                            _ = Interaction.MsgBox("F124");
                            Debugger.Break();
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            num4 = num2;
                            goto IL_Restart;

                        //====================================================================================================
                        IL_L353:
                            num = 353;
                            if (Information.Err().Number == 3021)
                            {
                                goto end_IL_0001_2;
                            }
                            _ = Interaction.MsgBox(Information.Err().Number.AsString());
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            num4 = unchecked(num2 + 1);
                            goto IL_Restart;
                        //====================================================================================================

                        IL_L359:
                            num = 359;
                            if (Information.Err().Number == 3021)
                            {
                                _ = Interaction.MsgBox("Person hat keine Nachkommen!");
                                Close();
                                goto end_IL_0001_2;
                            }
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_Restart;

                        end_IL_0001:
                            break;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 10229;
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

    private void Command2_Click(object eventSender, EventArgs eventArgs)
    {
        //Discarded unreachable code: IL_168e, IL_23cf
        int try0001_dispatch = -1;
        int num = default;
        string value = default;
        int num2 = default;
        int num3 = default;
        int number = default;
        int lErl = default;
        int nr = default;
        double num5 = default;
        string prompt = default;
        int recordCount = default;
        int recordCount2 = default;
        int recordCount3 = default;
        int recordCount4 = default;
        int recordCount5 = default;
        int recordCount6 = default;
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
                            num = 1;
                            value = "";
                            goto IL_Start;
                        case 11557:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_25f7;
                                    default:
                                        goto end_IL_0001;
                                }
                                number = Information.Err().Number;
                                if (number == 6)
                                {
                                    _ = Interaction.MsgBox(Conversion.ErrorToString(), mb: MessageBoxButtons.OK, title: Information.Err().Number.AsString());
                                    Debugger.Break();
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_25f3;
                                }
                                else
                                {
                                    if (number == 55)
                                    {
                                        ProjectData.ClearProjectError();
                                        if (num2 == 0)
                                        {
                                            throw ProjectData.CreateProjectError(-2146828268);
                                        }
                                        goto IL_25f7;
                                    }
                                    else
                                    {
                                        if (number == 3021)
                                        {
                                            _ = Interaction.MsgBox("Berechnt wurden" + _genMax.AsString() + " Generationen", mb: MessageBoxButtons.OK, title: "Fertig");
                                            Close();
                                            goto end_IL_0001_2;
                                        }
                                        if (number == 3022)
                                        {
                                            if (D != 1)
                                            {
                                                goto end_IL_0001_2;
                                            }
                                            ProjectData.ClearProjectError();
                                            if (num2 == 0)
                                            {
                                                throw ProjectData.CreateProjectError(-2146828268);
                                            }
                                            num2 = 0;
                                            goto IL_0896;
                                        }
                                        Modul1.UbgT = "IsFamLes, Error =" + Information.Err().Number.AsString();
                                        _ = Interaction.MsgBox(Conversion.ErrorToString(), mb: MessageBoxButtons.OK, title: Modul1.UbgT);
                                        _ = Interaction.MsgBox("F125");
                                        Debugger.Break();
                                        ProjectData.ClearProjectError();
                                        if (num2 == 0)
                                        {
                                            throw ProjectData.CreateProjectError(-2146828268);
                                        }
                                        goto IL_25f3;
                                    }
                                }
                            }
                        end_IL_0001:
                            break;
                        IL_Start:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            Frame1.Visible = false;
                            Modul1.FamInArb = 0;
                            if (Modul1_Nr1 > 1)
                            {
                                var sPerSex = DataModul.Person.GetSex(Modul1.PersInArb);
                                if (Conversions.ToBoolean(Modul1_Nr1 / 2.0 == Conversion.Int(Modul1_Nr1 / 2.0)
                                    && sPerSex == "F"))
                                {
                                    _ = Interaction.MsgBox("Person ist weiblich!\nWeibliche Personen können keine grade Ahnenziffer bekommen.");
                                    goto end_IL_0001_2;
                                }
                                if (Conversions.ToBoolean(Modul1_Nr1 / 2.0 != Conversion.Int(Modul1_Nr1 / 2.0)
                                    && sPerSex == "M"))
                                {
                                    _ = Interaction.MsgBox("Person ist männlich!\nMännliche Personen können keine ungrade Ahnenziffer größer 1 bekommen.");
                                    goto end_IL_0001_2;
                                }
                            }
                            if (Modul1_Nr1 == 1)
                            {
                                Gen = 0;
                            }
                            else if (Modul1_Nr1 > 1 && Modul1_Nr1 < 4)
                            {
                                Gen = 1;
                            }
                            else if (Modul1_Nr1 > 3 && Modul1_Nr1 < 8)
                            {
                                Gen = 2;
                            }
                            else if (Modul1_Nr1 > 7 && Modul1_Nr1 < 16)
                            {
                                Gen = 3;
                            }
                            else if (Modul1_Nr1 > 15 && Modul1_Nr1 < 32)
                            {
                                Gen = 4;
                            }
                            else if (Modul1_Nr1 > 31 && Modul1_Nr1 < 64)
                            {
                                Gen = 5;
                            }
                            else if (Modul1_Nr1 > 63 && Modul1_Nr1 < 128)
                            {
                                Gen = 6;
                            }
                            else if (Modul1_Nr1 > 127 && Modul1_Nr1 < 256)
                            {
                                Gen = 7;
                            }
                            else if (Modul1_Nr1 > 255 && Modul1_Nr1 < 512)
                            {
                                Gen = 8;
                            }
                            else if (Modul1_Nr1 > 511 && Modul1_Nr1 < 1024)
                            {
                                Gen = 9;
                            }
                            else if (Modul1_Nr1 > 1023 && Modul1_Nr1 < 2048)
                            {
                                Gen = 10;
                            }
                            else if (Modul1_Nr1 > 2047 && Modul1_Nr1 < 4096)
                            {
                                Gen = 11;
                            }
                            else if (Modul1_Nr1 > 4095 && Modul1_Nr1 < 8192)
                            {
                                Gen = 12;
                            }
                            else if (Modul1_Nr1 > 8191 && Modul1_Nr1 < 16384)
                            {
                                Gen = 13;
                            }
                            else if (Modul1_Nr1 > 16383 && Modul1_Nr1 < 32768)
                            {
                                Gen = 14;
                            }
                            else if (Modul1_Nr1 > 32767 && Modul1_Nr1 < 65536)
                            {
                                Gen = 15;
                            }
                            else if (Modul1_Nr1 > 65535 && Modul1_Nr1 < 131072)
                            {
                                Gen = 16;
                            }
                            else if (Modul1_Nr1 > 131071 && Modul1_Nr1 < 262144)
                            {
                                Gen = 17;
                            }
                            else if (Modul1_Nr1 > 262143 && Modul1_Nr1 < 524288)
                            {
                                Gen = 18;
                            }
                            else if (Modul1_Nr1 > 524287 && Modul1_Nr1 < 1048576)
                            {
                                Gen = 19;
                            }
                            else if (Modul1_Nr1 > 1048575 && Modul1_Nr1 < 2097152)
                            {
                                Gen = 20;
                            }
                            else if (Modul1_Nr1 > 2097151 && Modul1_Nr1 < 4194304)
                            {
                                Gen = 21;
                            }
                            else if (Modul1_Nr1 > 4194303 && Modul1_Nr1 < 8388608)
                            {
                                Gen = 22;
                            }
                            else if (Modul1_Nr1 > 8388607 && Modul1_Nr1 < 16777216)
                            {
                                Gen = 23;
                            }
                            else if (Modul1_Nr1 > 16777215 && Modul1_Nr1 < 33554432)
                            {
                                Gen = 24;
                            }
                            else if (Modul1_Nr1 > 33554431 && Modul1_Nr1 < 67108864)
                            {
                                Gen = 25;
                            }
                            else if (Modul1_Nr1 > 67108863 && Modul1_Nr1 < 134217728)
                            {
                                Gen = 26;
                            }
                            else if (Modul1_Nr1 > 134217727 && Modul1_Nr1 < 268435456)
                            {
                                Gen = 27;
                            }
                            else if (Modul1_Nr1 > 268435455 && Modul1_Nr1 < 536870912)
                            {
                                Gen = 28;
                            }
                            else if (Modul1_Nr1 > 536870911 && Modul1_Nr1 < 1073741824)
                            {
                                Gen = 29;
                            }
                            else if (Modul1_Nr1 > 1073741823)
                            {
                                _ = Interaction.MsgBox("Startziffer zu groß, Folgegenerationen können nicht berechnet werden");
                                goto end_IL_0001_2;
                            }
                            if (_Option1_1.Checked)
                            {
                                Gen++;
                            }
                            _startgen = Gen;
                            nr = Modul1_Nr1;
                            Z = nr - 1;
                            Modul1_Gen1 = Modul1_Gen1.AsInt().AsString();
                            if (Modul1_Gen1.AsDouble() == 0.0)
                            {
                                Modul1_Gen1 = 93.AsString();
                            }
                            Modul1_Gen1 = (Modul1_Gen1.AsDouble() + (Gen - 1)).AsString();
                            num5 = (double)Modul1_Nr1;
                            _Label1_3.Text = Modul1.PersInArb.AsString();
                            M1_Iter = Modul1_Nr1;
                            number = 0;
                            goto IL_0896;
                        IL_0896:
                            num = 124;
                            while (true)
                            {
                                if (number != 3022)
                                {
                                    lErl = 5;
                                    Perprüf = Modul1.PersInArb;
                                    Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                    Application.DoEvents();
                                    Modul1.Kont[0] = Modul1.Person.SurName.ToUpper();
                                    if (Modul1.Person.SurName.Trim() == "")
                                    {
                                        Modul1.Kont[0] = "NN";
                                    }
                                    Modul1.Kont[0] = Modul1.Umlaute_UCase(Modul1.Kont[0]);
                                    DataModul.DT_AncesterTable.Index = "PerNr";
                                    DataModul.DT_AncesterTable.Seek("=", Modul1.PersInArb);
                                    if (_maxGen < Gen)
                                    {
                                        _maxGen = Gen;
                                    }
                                    Modul1.Schalt = 1;
                                    Modul1.eLKennz = ELinkKennz.lkChild;
                                    Modul1.Ubg = famsuch1(Modul1.PersInArb, Modul1.eLKennz);
                                    Spitz = Modul1.Ubg == 0 ? "1" : "0";
                                    if (DataModul.DT_AncesterTable.NoMatch)
                                    {
                                        DataModul.DT_AncesterTable.AddNew();
                                        if (Modul1.PersInArb == 0)
                                        {
                                            goto end_IL_0001_2;
                                        }
                                        DataModul.DT_AncesterTable.Fields["PerNr"].Value = Modul1.PersInArb;
                                        DataModul.DT_AncesterTable.Fields["Gen"].Value = Gen;
                                        DataModul.DT_AncesterTable.Fields["Ahn"].Value = new string(' ', 40) + num5.AsString().Right(40);
                                        DataModul.DT_AncesterTable.Fields["Weiter"].Value = "0";
                                        DataModul.DT_AncesterTable.Fields["Ehe"].Value = Modul1.FamInArb;
                                        DataModul.DT_AncesterTable.Fields["Name"].Value = Modul1.Kont[0];
                                        DataModul.DT_AncesterTable.Fields["aiSpitz"].Value = Spitz;
                                        DataModul.DT_AncesterTable.Update();
                                    }
                                    else if (DataModul.DT_AncesterTable.Fields["Ahn"].AsInt() != 0)
                                    {
                                        DataModul.DT_AncesterTable.Edit();
                                        DataModul.DT_AncesterTable.Fields["Weiter"].Value = "1";
                                        DataModul.DT_AncesterTable.Update();
                                        DataModul.DT_AncesterTable.AddNew();
                                        DataModul.DT_AncesterTable.Fields["PerNr"].Value = Modul1.PersInArb;
                                        DataModul.DT_AncesterTable.Fields["Gen"].Value = Gen;
                                        DataModul.DT_AncesterTable.Fields["Ahn"].Value = new string(' ', 40) + num5.AsString().Right(40);
                                        if (Strings.InStr(DataModul.DT_AncesterTable.Fields["Ahn"].AsString(), "E") != 0)
                                        {
                                            Debugger.Break();
                                        }
                                        DataModul.DT_AncesterTable.Fields["Weiter"].Value = "1";
                                        if (Modul1.FamInArb == 0)
                                        {
                                            _ = Interaction.MsgBox("Stop", mb: MessageBoxButtons.OK, title: "13");
                                        }
                                        DataModul.DT_AncesterTable.Fields["Ehe"].Value = Modul1.FamInArb;
                                        DataModul.DT_AncesterTable.Fields["Name"].Value = Modul1.Person.SurName;
                                        DataModul.DT_AncesterTable.Fields["aiSpitz"].Value = Spitz;
                                        D = 1;
                                        DataModul.DT_AncesterTable.Update();
                                        D = 0;
                                        //====================================================================================================
                                    }
                                    else
                                    {
                                        DataModul.DT_AncesterTable.Edit();
                                        DataModul.DT_AncesterTable.Fields["Ahn"].Value = new string(' ', 40) + num5.AsString().Right(40);
                                        DataModul.DT_AncesterTable.Fields["Gen"].Value = Gen;
                                        if (Modul1.FamInArb == 0)
                                        {
                                            _ = Interaction.MsgBox("Stop", mb: MessageBoxButtons.OK, title: "14");
                                        }
                                        DataModul.DT_AncesterTable.Fields["Ehe"].Value = Modul1.FamInArb;
                                        DataModul.DT_AncesterTable.Fields["Name"].Value = Modul1.Person.SurName;
                                        DataModul.DT_AncesterTable.Fields["aiSpitz"].Value = Spitz;
                                        DataModul.DT_AncesterTable.Update();
                                    }
                                }
                                DataModul.DT_KindAhnTable.AddNew();
                                if (Modul1.PersInArb == 0)
                                {
                                    goto end_IL_0001_2;
                                }
                                DataModul.DT_KindAhnTable.Fields["PerNr"].Value = Modul1.PersInArb;
                                DataModul.DT_KindAhnTable.Fields["Ahn"].Value = new string(' ', 40) + num5.AsString().Right(40);
                                DataModul.DT_KindAhnTable.Update();
                                _Label1_3.Text = Modul1.PersInArb.AsString();
                                Modul1.Schalt = 1;
                                Modul1.eLKennz = ELinkKennz.lkChild;
                                Modul1.Ubg = famsuch1(Modul1.PersInArb, Modul1.eLKennz);
                                Modul1.FamInArb = Modul1.Ubg;
                                Gen++;
                                if (!(Gen == 135 || Gen > Modul1_Gen1.AsDouble() + 1.0))
                                {
                                    if (Modul1.Ubg != 0)
                                    {
                                        Modul1.Family.Mann = 0;
                                        Modul1.Family.Frau = 0;
                                        Modul1.Ubg = 0;
                                        Perprüf = Modul1.PersInArb;
                                        DataModul.Link.ReadFamily(Modul1.FamInArb, Modul1.Family);
                                        if (!(Modul1.Family.Mann == 0 && Modul1.Family.Frau == 0))
                                        {
                                            Modul1.PersInArb = Modul1.Family.Mann;
                                            if (Modul1.Family.Mann == 0)
                                            {
                                                if (Modul1.Schalt < 2)
                                                {
                                                    num5 *= 2d;
                                                }
                                                if (Modul1.Schalt < 2)
                                                {
                                                    value = (num5 + 1).AsString();
                                                }
                                                else
                                                    Modul1.Frauen_Renamed.SetKek1(-1);
                                            }
                                            if (Modul1.PersInArb > 0)
                                            {
                                                if (Perprüf == Modul1.PersInArb)
                                                {
                                                    prompt = "Person" + Perprüf.AsString() + " ist sein eigener Vorfahre. Diesen Fehler müssen Sie erst berichtigen.";
                                                    _ = Interaction.MsgBox(prompt, title: "Dateifehler", icon: MessageBoxIcon.Exclamation);
                                                    Close();
                                                    Menue.Default.Show();
                                                    goto end_IL_0001_2;
                                                }
                                                if (Perprüf == Modul1.Family.Frau)
                                                {
                                                    Debugger.Break();
                                                }
                                                if (Modul1.Schalt < 2)
                                                {
                                                    num5 *= 2d;
                                                }
                                                if (Modul1.Schalt < 2)
                                                {
                                                    value = (num5 + 1d).AsString();
                                                }
                                                else
                                                    Modul1.Frauen_Renamed.SetKek1(-1);
                                            }
                                            if (Modul1.Family.Frau > 0)
                                            {
                                                DataModul.NB_FrauTable.Index = "PerNR";
                                                DataModul.NB_FrauTable.Seek("=", Modul1.Family.Frau);
                                                if (DataModul.NB_FrauTable.NoMatch)
                                                {
                                                    DataModul.NB_FrauTable.AddNew();
                                                    DataModul.NB_FrauTable.Fields["Nr"].Value = Modul1.Family.Frau;
                                                    DataModul.NB_FrauTable.Fields["Kek"].Value = value;
                                                    DataModul.NB_FrauTable.Fields["Gen"].Value = Gen;
                                                    DataModul.NB_FrauTable.Fields["LfNr"].Value = Modul1_Nr1;
                                                    Modul1_Nr1++;
                                                    DataModul.NB_FrauTable.Fields["Alt"].Value = Modul1.FamInArb.AsString();
                                                    DataModul.NB_FrauTable.Update();
                                                }
                                                else
                                                {
                                                    DataModul.NB_FrauTable.AddNew();
                                                    DataModul.NB_FrauTable.Fields["Nr"].Value = Modul1.Family.Frau;
                                                    DataModul.NB_FrauTable.Fields["Kek"].Value = value;
                                                    DataModul.NB_FrauTable.Fields["Gen"].Value = Gen;
                                                    DataModul.NB_FrauTable.Fields["LfNr"].Value = Modul1_Nr1;
                                                    Modul1_Nr1++;
                                                    DataModul.NB_FrauTable.Fields["Alt"].Value = Modul1.FamInArb.AsString();
                                                    DataModul.NB_FrauTable.Update();
                                                }
                                            }
                                            Modul1.Family.Frau = 0;
                                            if (Modul1.Family.Mann > 0)
                                            {
                                                continue;
                                            }
                                            if (Modul1.PersInArb != 0)
                                            {
                                                M1_Iter++;
                                                continue;
                                            }
                                        }
                                    }
                                }
                                lErl = 6;
                                Z++;
                                DataModul.NB_FrauTable.Index = "LfNr";
                                DataModul.NB_FrauTable.Seek("=", Z);
                                if (!DataModul.NB_FrauTable.NoMatch)
                                {
                                    Gen = DataModul.NB_FrauTable.Fields["Gen"].AsInt().AsInt();
                                    Modul1.PersInArb = DataModul.NB_FrauTable.Fields["Nr"].AsInt();
                                    Modul1.FamInArb = DataModul.NB_FrauTable.Fields["Alt"].AsInt();
                                    num5 = DataModul.NB_FrauTable.Fields["Kek"].AsDouble();
                                    if (Modul1.PersInArb > 0)
                                    {
                                        continue;
                                    }
                                }
                                break;
                            }
                            if (Modul1.Suchfeld[0] == ESearchSelection.e5)
                            {
                                DataModul.DB_PersonTable.Index = nameof(PersonIndex.PerNr);
                                DataModul.DB_PersonTable.MoveFirst();
                                recordCount = DataModul.DB_PersonTable.RecordCount;
                                M1_Iter = 1;
                                while (M1_Iter <= recordCount)
                                {
                                    DataModul.DB_PersonTable.Edit();
                                    DataModul.DB_PersonTable.Fields[PersonFields.Such4].Value = "";
                                    DataModul.DB_PersonTable.Update();
                                    DataModul.DB_PersonTable.MoveNext();
                                    M1_Iter++;
                                }
                                DataModul.DT_AncesterTable.Index = "Ahnen";
                                DataModul.DT_AncesterTable.MoveFirst();
                                recordCount2 = DataModul.DT_AncesterTable.RecordCount;
                                M1_Iter = 1;
                                while (M1_Iter <= recordCount2)
                                {
                                    if (DataModul.DT_AncesterTable.NoMatch)
                                    {
                                        Debugger.Break();
                                    }
                                    if (DataModul.DT_AncesterTable.EOF)
                                    {
                                        Debugger.Break();
                                    }
                                    _ahnNr = DataModul.DT_AncesterTable.Fields["Ahn"].AsString();
                                    Modul1.PersInArb = DataModul.DT_AncesterTable.Fields["PernR"].AsInt();
                                    DataModul.DB_PersonTable.Seek("=", Modul1.PersInArb);
                                    DataModul.DB_PersonTable.Edit();
                                    DataModul.DB_PersonTable.Fields[PersonFields.Such4].Value = "00000000000" + _ahnNr.AsString().Trim().Right(10);
                                    DataModul.DB_PersonTable.Update();
                                    DataModul.DT_AncesterTable.MoveNext();
                                    M1_Iter++;
                                }
                            }
                            if (Modul1.Suchfeld[1] == ESearchSelection.e5)
                            {
                                DataModul.DB_PersonTable.Index = nameof(PersonIndex.PerNr);
                                DataModul.DB_PersonTable.MoveFirst();
                                recordCount3 = DataModul.DB_PersonTable.RecordCount;
                                M1_Iter = 1;
                                while (M1_Iter <= recordCount3)
                                {
                                    DataModul.DB_PersonTable.Edit();
                                    DataModul.DB_PersonTable.Fields[PersonFields.Such5].Value = "";
                                    DataModul.DB_PersonTable.Update();
                                    DataModul.DB_PersonTable.MoveNext();
                                    M1_Iter++;
                                }
                                DataModul.DT_AncesterTable.Index = "Ahnen";
                                DataModul.DT_AncesterTable.MoveFirst();
                                recordCount4 = DataModul.DT_AncesterTable.RecordCount;
                                M1_Iter = 1;
                                while (M1_Iter <= recordCount4)
                                {
                                    if (DataModul.DT_AncesterTable.NoMatch)
                                    {
                                        Debugger.Break();
                                    }
                                    if (DataModul.DT_AncesterTable.EOF)
                                    {
                                        Debugger.Break();
                                    }
                                    _ahnNr = DataModul.DT_AncesterTable.Fields["Ahn"].AsString();
                                    Modul1.PersInArb = DataModul.DT_AncesterTable.Fields["PernR"].AsInt();
                                    DataModul.DB_PersonTable.Seek("=", Modul1.PersInArb);
                                    DataModul.DB_PersonTable.Edit();
                                    DataModul.DB_PersonTable.Fields[PersonFields.Such5].Value = "00000000000" + _ahnNr.AsString().Trim().Right(10);
                                    DataModul.DB_PersonTable.Update();
                                    DataModul.DT_AncesterTable.MoveNext();
                                    M1_Iter++;
                                }
                            }
                            if (Modul1.Suchfeld[2] == ESearchSelection.e5)
                            {
                                DataModul.DB_PersonTable.Index = nameof(PersonIndex.PerNr);
                                DataModul.DB_PersonTable.MoveFirst();
                                recordCount5 = DataModul.DB_PersonTable.RecordCount;
                                M1_Iter = 1;
                                while (M1_Iter <= recordCount5)
                                {
                                    DataModul.DB_PersonTable.Edit();
                                    DataModul.DB_PersonTable.Fields[PersonFields.Such6].Value = "";
                                    DataModul.DB_PersonTable.Update();
                                    DataModul.DB_PersonTable.MoveNext();
                                    M1_Iter++;
                                }
                                DataModul.DT_AncesterTable.Index = "Ahnen";
                                DataModul.DT_AncesterTable.MoveFirst();
                                recordCount6 = DataModul.DT_AncesterTable.RecordCount;
                                M1_Iter = 1;
                                while (M1_Iter <= recordCount6)
                                {
                                    if (DataModul.DT_AncesterTable.NoMatch)
                                    {
                                        Debugger.Break();
                                    }
                                    if (DataModul.DT_AncesterTable.EOF)
                                    {
                                        Debugger.Break();
                                    }
                                    _ahnNr = DataModul.DT_AncesterTable.Fields["Ahn"].AsString();
                                    Modul1.PersInArb = DataModul.DT_AncesterTable.Fields["PernR"].AsInt();
                                    DataModul.DB_PersonTable.Seek("=", Modul1.PersInArb);
                                    DataModul.DB_PersonTable.Edit();
                                    DataModul.DB_PersonTable.Fields[PersonFields.Such6].Value = "00000000000" + _ahnNr.AsString().Trim().Right(10);
                                    DataModul.DB_PersonTable.Update();
                                    DataModul.DT_AncesterTable.MoveNext();
                                    M1_Iter++;
                                }
                            }
                            _Label2_9.Text = Modul1.IText[EUserText.t412];
                            DataModul.DT_AncesterTable.Index = "Ahnen";
                            if (DataModul.DT_AncesterTable.RecordCount != 0)
                            {
                                DataModul.DT_AncesterTable.MoveFirst();
                                if (!DataModul.DT_AncesterTable.NoMatch)
                                {
                                    _Label2_9.Text = "Ahnen berechnet für:";
                                    DataModul.DT_AncesterTable.MoveFirst();
                                    if (!DataModul.DT_AncesterTable.EOF)
                                    {
                                        if (!DataModul.DT_AncesterTable.NoMatch)
                                        {
                                            _ahnNr = DataModul.DT_AncesterTable.Fields["Ahn"].AsString();
                                            DataModul.DT_AncesterTable.MoveFirst();
                                            Modul1.PersInArb = DataModul.DT_AncesterTable.Fields["PerNr"].AsInt();
                                            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                            _Label2_9.Text = "Ahnen berechnet für:   Personenziffer " + Modul1.PersInArb.AsString();
                                            var sPrefix = Modul1.Person.Prefix != "" ? Modul1.Person.Prefix + " " : "";
                                            _Label2_8.Text = Modul1.Person.Givennames + " " + sPrefix + Modul1.Person.SurName;
                                            DataModul.DT_AncesterTable.Index = "Gen";
                                            DataModul.DT_AncesterTable.MoveLast();
                                            _Label2_7.Text = DataModul.DT_AncesterTable.RecordCount.AsString() + " Personen in" + DataModul.DT_AncesterTable.Fields["Gen"].AsString() + " Generationen für Ahnenziffer " + _ahnNr;
                                            _Label1_0.Visible = false;
                                            _Label1_3.Visible = false;
                                        }
                                    }
                                }
                            }
                            lErl = 1;
                            _ = Interaction.MsgBox("Berechnung fertig", mb: MessageBoxButtons.OK, title: "Generationen von " + _startgen.AsString() + " bis" + _maxGen.AsString());
                            Close();
                            Menue.Default.Show();
                            goto end_IL_0001_2;
                        IL_25f3:
                            num4 = num2;
                            goto IL_25fb;
                        IL_25f7: // <========== 3
                            num4 = unchecked(num2 + 1);
                            goto IL_25fb;
                        IL_25fb:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 124:
                                case 283:
                                case 289:
                                case 300:
                                    goto IL_0896;
                                case 427:
                                case 428:
                                    num = 428;
                                    Modul1.Schalt = 2;
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_25f7;
                                case 9:
                                case 13:
                                case 108:
                                case 148:
                                case 197:
                                case 240:
                                case 419:
                                case 421:
                                case 430:
                                case 431:
                                case 434:
                                case 435:
                                case 439:
                                case 440:
                                case 445:
                                case 446:
                                case 453:
                                case 454:
                                case 455:
                                    goto end_IL_0001_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 11557;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001_2: // <========== 10
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }
    private void Command3_Click(object eventSender, EventArgs eventArgs)
    {
        ProjectData.EndApp();
    }

    private void Ahnen_Load(object eventSender, EventArgs eventArgs)
    {
        //Discarded unreachable code: IL_09b5
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
                    switch (try0001_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = -2;
                            goto IL_0009;
                        case 2899:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 1:
                                        break;
                                    default:
                                        goto end_IL_0001;
                                }
                                int num4 = unchecked(num2 + 1);
                                num2 = 0;
                                switch (num4)
                                {
                                    case 1:
                                        break;
                                    case 71:
                                    case 75:
                                    case 80:
                                    case 83:
                                    case 97:
                                        goto end_IL_0001_2;
                                    case 98:
                                        goto end_IL_0001_3;
                                }
                                goto default;
                            }
                        end_IL_0001_2:
                            break;
                        IL_0009:
                            num = 2;
                            if (Modul1.FontSize > 0f)
                            {
                                Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                _Label1_0.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                _Label1_3.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                _Label1_8.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                _Label1_9.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                _Label2_2.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                _Command1_0.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                _Command1_1.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                _Command1_2.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                _Label2_7.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                _Label2_8.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                _Label2_9.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                Command3.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                                Label3.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                            }
                            Bezeichnung1.Text = Modul1.IText[EUserText.t91].Replace("&", "");
                            var aiPos = Modul1.Persistence.ReadIntsProg("maspos.dat", 2);
                            Left = aiPos[0];
                            Top = aiPos[1];
                            Label3.Text = "Mandant: " + Modul1.Verz;
                            BackColor = Modul1.HintFarb;
                            Modul1.Persistence.ReadEnumInit<FormWindowState>("Windowstate", out var WiS);
                            WindowState = WiS;
                            Label1_10.Text = Modul1.VersionT;
                            _Label1_9.Text = Modul1.Version1;
                            _Label1_8.Text = Modul1.Version;
                            if (Modul1.System.VerSpecial == 1)
                            {
                                _Label1_8.Text = "Eingeschränkte Sonderversion";
                            }
                            Label1_10.Width = Width;
                            _Label1_9.Width = Width;
                            _Label1_8.Width = Width;
                            _Command1_0.Text = Modul1.IText[EUserText.t241];
                            _Command1_1.Text = Modul1.IText[EUserText.t238];
                            _Command1_2.Text = Modul1.IText[EUserText.t158];
                            Command3.Text = Modul1.IText[EUserText.t392];
                            _Label2_0.Text = Modul1.IText[EUserText.t412];
                            if (DataModul.DT_DescendentTable.RecordCount != 0)
                            {
                                DataModul.DT_DescendentTable.Index = "NR";
                                DataModul.DT_DescendentTable.MoveFirst();
                                Modul1.PersInArb = DataModul.DT_DescendentTable.Fields["Pr"].AsInt();
                                _Label2_0.Text = "Nachfahren berechnet für:   Personenziffer " + Modul1.PersInArb.AsString();
                                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                if (Modul1.Person.Prefix.Trim() != "")
                                {
                                    Modul1.Person.SetFullSurname(Modul1.Person.Prefix.Trim() + " " + Modul1.Person.SurName.Trim());
                                }
                                if (Modul1.Person.Suffix.Trim() != "")
                                {
                                    Modul1.Person.SetFullSurname(Modul1.Person.SurName.Trim() + " " + Modul1.Person.Suffix.Trim());
                                }
                                _Label2_1.Text = Modul1.Person.Givennames + " " + Modul1.Person.FullSurName;
                                DataModul.DT_DescendentTable.Index = "GNr";
                                DataModul.DT_DescendentTable.MoveLast();
                                _Label2_2.Text = $"  {(DataModul.DT_DescendentTable.RecordCount - 1).AsString()} Personen in{DataModul.DT_DescendentTable.Fields["Gen"].AsString()} Generationen";
                            }
                            lErl = 2;
                            _Label2_9.Text = Modul1.IText[EUserText.t412];
                            DataModul.DT_AncesterTable.Index = "Ahnen";
                            if (DataModul.DT_AncesterTable.RecordCount == 0)
                            {
                                break;
                            }
                            DataModul.DT_AncesterTable.MoveFirst();
                            if (DataModul.DT_AncesterTable.NoMatch)
                            {
                                break;
                            }
                            _Label2_9.Text = "Ahnen berechnet für:";
                            DataModul.DT_AncesterTable.MoveFirst();
                            if (DataModul.DT_AncesterTable.EOF)
                            {
                                break;
                            }
                            if (DataModul.DT_AncesterTable.NoMatch)
                            {
                                break;
                            }
                            _ahnNr = DataModul.DT_AncesterTable.Fields["Ahn"].AsString();
                            DataModul.DT_AncesterTable.MoveFirst();
                            Modul1.PersInArb = DataModul.DT_AncesterTable.Fields["PerNr"].AsInt();
                            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                            _Label2_9.Text = "Ahnen berechnet für:   Personenziffer " + Modul1.PersInArb.AsString();
                            var sPrefix = Modul1.Person.Prefix != "" ? Modul1.Person.Prefix + " " : "";
                            _Label2_8.Text = Modul1.Person.Givennames + " " + sPrefix + Modul1.Person.SurName;
                            DataModul.DT_AncesterTable.Index = "Gen";
                            DataModul.DT_AncesterTable.MoveLast();
                            _Label2_7.Text = $"{(DataModul.DT_AncesterTable.RecordCount - 1).AsString()} Personen in{DataModul.DT_AncesterTable.Fields["Gen"].AsString()} Generationen für Ahnenziffer {_ahnNr}";
                            break;
                            //====================================================================================================
                    }
                    num = 97;
                    lErl = 1;
                    break;
                }
            end_IL_0001:
                ;
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 2899;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001_3:
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }
    public int famsuch1(int persInArb, ELinkKennz eLKennz)
    {
        List<int> aiFam = new List<int>();
        foreach (var link in DataModul.Link.ReadAllPers(persInArb, eLKennz))
        {
            aiFam.Add(link.iFamNr);
        }
        if (eLKennz == ELinkKennz.lkChild && aiFam.Count > 1)
        {
            string text = "Person " + persInArb.AsString() + " ist in den Familien " + Fami + " als Kind eingebunden. Eine Person kann aber nur in einer Familie als Kind sein.";
            text += "\nBitte diesen Fehler zuerst korrigieren.";
            _ = Interaction.MsgBox(text);
        }
        if (aiFam.Count == 1)
            return aiFam[0];
        else
            return 0;
    }

    private void Ahnen_FormClosing(object eventSender, FormClosingEventArgs eventArgs)
    {
        //Discarded unreachable code: IL_007d, IL_00f5
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
                    case 352:
                        {
                            num2 = num;
                            switch (num3 <= -2 ? 1 : num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_00f8;
                                default:
                                    goto end_IL_0001;
                            }
                            goto IL_007f;
                        }

                    IL_007f:
                        num = 13;
                        if (Information.Err().Number == 91)
                        {
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                        }
                        else
                        {
                            if (Information.Err().Number != 3420)
                            {
                                break;
                            }
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                        }
                        goto IL_00f8;

                    IL_00f8:
                        num4 = num2 + 1;
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 2:
                                goto IL_000b;

                            case 13:
                                goto IL_007f;
                            case 15:
                            case 19:
                            case 21:
                                goto end_IL_0001_3;
                            default:
                                goto end_IL_0001;
                            case 10:
                            case 11:
                            case 12:
                            case 22:
                                goto end_IL_0001_2;
                        }
                        goto default;
                    IL_000b:
                        num = 2;
                        CloseReason closeReason = eventArgs.CloseReason;
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        if (closeReason != 0)
                        {
                        }
                        else
                        {

                            DataModul.MandDB.Close();
                            DataModul.TempDB.Close();
                            DataModul.DOSB.Close();
                            DataModul.DSB.Close();
                            FileSystem.Kill("\\Init\\Bild"); //??
                            ProjectData.EndApp();
                        }
                        goto end_IL_0001_2;

                    end_IL_0001_3:
                        break;
                }
                num = 21;
                eventArgs.Cancel = cancel;
                break;
            end_IL_0001:
                ;
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0001_dispatch = 352;
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
    public void Mehrehezeig(IList<int> aiFam, ListBox List1)
    {
        List<(string, int)> items = new();
        items.Clear();
        DateTime dt;
        foreach (var num8 in aiFam)
        {
            Modul1.sDatu = "    ";
            var num9 = 500;
            while (num9 <= 507)
            {
                if ((dt = DataModul.Event.GetDate((EEventArt)num9, num8)) != default)
                {
                    Modul1.sDatu = Strings.Right("00000000" + Strings.Trim(dt.AsString()), 8);
                    Modul1.sDatu = Modul1.sDatu.Left(4);
                    break;
                }
                num9++;
            }
            if (Modul1.sDatu.Trim() == ""
                && (dt = DataModul.Event.GetDate(EEventArt.eA_601, num8)) != default)
            {
                Modul1.sDatu = Strings.Right("00000000" + Strings.Trim(dt.AsString()), 8);
                Modul1.sDatu = Modul1.sDatu.Left(4);
            }
            items.Add((Modul1.sDatu, num8));
        }
        var num7 = items.Count - 1;
        var M1_Iter = 0;
        while (M1_Iter <= num7)
        {
            List1.Items.Add(new ListItem<(string, int)>($"{items[M1_Iter].Item1} {items[M1_Iter].Item2,10}", items[M1_Iter]));
            M1_Iter++;
        }

    }

    // Neue Hilfsklassen zur Entkopplung der Datenlogik von der UI

    private sealed class DescendentCalculationContext
    {
        public int StartPersonId { get; set; }
        public int MaxGenerations { get; set; }
        public int StartGeneration { get; set; }
    }

    private sealed class AncesterCalculationContext
    {
        public int StartPersonId { get; set; }
        public int StartNumber { get; set; }
        public int StartGeneration { get; set; }
        public int MaxGeneration { get; set; }
    }

    private DescendentCalculationContext PrepareDescendentCalculation()
    {
        Modul1.Dateienopen();
        Gen = 0;
        Modul1.Schalt = 0;
        _a1 = "Nummer der gesuchten Person\rLeer,0 oder Abbrechen wechselt in die Suche nach Namen";
        Modul1.PersInArb = (int)Math.Round(
            Interaction.InputBox(_a1,
                "Auswahl der Person deren Nachkommen berechnet werden sollen.").AsDouble());

        if (Modul1.PersInArb == 0)
        {
            Modul1.PersInArb = SelectPersonByNameDialog(Modul1.PersInArb);
        }
        if (Modul1.PersInArb == 0)
        {
            return null;
        }

        Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
        Bezeichnung1.Text = "Nachfahrenberechnung für " + Modul1.Person.Givennames + " " + Modul1.Person.SurName;
        Modul1_Gen1 = Interaction.InputBox("Wieviel Generationen maximal", "Leer = alle Nachkommen").AsDouble().AsString();
        if (Modul1_Gen1.AsInt() == 0.0)
        {
            Modul1_Gen1 = "100";
        }
        Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);

        return new DescendentCalculationContext
        {
            StartPersonId = Modul1.PersInArb,
            MaxGenerations = Modul1_Gen1.AsInt(),
            StartGeneration = 1
        };
    }

    private AncesterCalculationContext PrepareAncesterCalculation()
    {
        Modul1.Dateienopen();
        Gen = 0;
        Modul1.Schalt = 0;
        _a1 = "Nummer der gesuchten Person\rLeer,0 oder Abbrechen wechselt in die Suche nach Namen";
        string personSelectionInput = Interaction.InputBox(_a1,
            "Auswahl der Person deren Ahnen berechnet werden sollen.");
        Modul1.PersInArb = int.TryParse(personSelectionInput, out int selectedPersonId)
            ? selectedPersonId
            : 0;

        if (Modul1.PersInArb == 0)
        {
            Modul1.PersInArb = SelectPersonByNameDialog(Modul1.PersInArb);
        }
        if (Modul1.PersInArb == 0)
        {
            return null;
        }

        Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
        Bezeichnung1.Text = "Ahnenberechnung für " + Modul1.Person.Givennames + " " + Modul1.Person.SurName;
        Modul1_Gen1 = Interaction.InputBox("Wieviel Generationen maximal", "max. 93 Generationen", "93");
        string startNumberInput = Interaction.InputBox("Start mit Ahnenziffer:", "", "1");
        Modul1_Nr1 = int.TryParse(startNumberInput, out int startNumber)
            ? startNumber
            : 0;
        if (Modul1_Nr1 == 0)
        {
            return null;
        }

        Frame1.Visible = true;

        return new AncesterCalculationContext
        {
            StartPersonId = Modul1.PersInArb,
            StartNumber = Modul1_Nr1,
            StartGeneration = Gen,
            MaxGeneration = Modul1_Gen1.AsInt()
        };
    }

    private int SelectPersonByNameDialog(int currentPersonNr)
    {
        Modul1.Suchfam = 0;
        Modul1.SuchPer = 0;

        var dlg = MainProject.Forms.Namensuch;

        dlg.Show();
        if (dlg.List1.SelectedIndex > 10)
        {
            dlg.List1.TopIndex = dlg.List1.SelectedIndex - 5;
        }

        dlg.ComboBox1.Text = "";
        _ = dlg.ComboBox1.Focus();
        dlg.ComboBox1.SelectionStart = dlg.ComboBox1.Text.Length;
        dlg.Visible = false;
        _ = dlg.ShowDialog(currentPersonNr);

        return Modul1.SuchPer;
    }

    // Neue Hilfsmethode: sammelt Kinder der Startperson in List1
    private void CollectDescendentChildren(IList<int> aiFams)
    {
        List1.Items.Clear();
        short famCount = (short)aiFams.Count;
        AI = 1;
        while (AI <= famCount)
        {
            Modul1.FamInArb = aiFams[AI - 1];
            DataModul.Link.ReadFamily(Modul1.FamInArb, Modul1.Family);
            int i = 1;
            while (Modul1.Family.Kind[i] != 0 && i <= 99)
            {
                Modul1.PersInArb = Modul1.Family.Kind[i];
                var dat = DataModul.Event.GetPersonBirthOrBapt(Modul1.PersInArb);
                if (dat != default)
                {
                    Datum = Strings.Right("00000000" + dat.AsInt().AsString(), 8);
                }
                else
                {
                    Datum = new string(' ', 8);
                }
                _ = List1.Items.Add(Datum + Modul1.PersInArb.AsString());
                i++;
            }
            AI++;
        }
    }

    // Neue Hilfsmethode: persistiert eine Nachfahren-Generation (ohne kia)
    private void PersistDescendentGeneration()
    {
        int count = List1.Items.Count - 1;
        int i = 0;
        while (i <= count)
        {
            Modul1.PersInArb = (int)Math.Round(Conversion.Val(Strings.Mid(List1.Items[i].AsString(), 10, 10)));
            var rs = DataModul.DT_DescendentTable;
            rs.Index = "PerNr";
            rs.Seek("=", Modul1.PersInArb);

            if (rs.NoMatch)
            {
                rs.AddNew();
                if (Gen < 0)
                {
                    _ = Interaction.MsgBox("Stop", mb: MessageBoxButtons.OK, title: "16");
                }
                rs.Fields["Gen"].Value = "  " + Gen.AsString().Right(2);
                rs.Fields["Nr"].Value = _nachnr + "  " + (i + 1).AsString().Right(2) + ".";
                if (Gen > _genMax)
                {
                    _genMax = Gen;
                }
                rs.Fields["Pr"].Value = Strings.Mid(List1.Items[i].AsString(), 10, 30);
                ProjectData.ClearProjectError();
                rs.Update();
            }
            else if (rs.Fields["Nr"].AsString() != _nachnr + "  " + (i + 1).AsString().Right(2) + ".")
            {
                if (rs.Fields["Gen"].AsInt() >= Gen)
                {
                    rs.Edit();
                    rs.Fields["Gen"].Value = "  " + Gen.AsString().Right(2);
                    rs.Fields["Nr"].Value = _nachnr + "  " + (i + 1).AsString().Right(2) + ".";
                    rs.Update();
                }
            }

            DataModul.NB_OrtindTable.AddNew();
            DataModul.NB_OrtindTable.Fields["Ort"].Value = "  " + Gen.AsString().Right(2);
            DataModul.NB_OrtindTable.Fields["Name"].Value = "  " + (i + 1).AsString().Right(2) + ".";
            DataModul.NB_OrtindTable.Fields["OrtNr"].Value = Strings.Mid(List1.Items[i].AsString(), 10, 30);
            DataModul.NB_OrtindTable.Fields["Ind"].Value = "=";
            DataModul.NB_OrtindTable.Update();

            i++;
        }
    }

    // Neue Hilfsmethode: persistiert Generation mit kia-Feld (Block in IL_L178)
    private void PersistDescendentGenerationWithKia()
    {
        int count = List1.Items.Count - 1;
        int i = 0;
        while (i <= count)
        {
            Modul1.PersInArb = (int)Math.Round(Conversion.Val(Strings.Mid(List1.Items[i].AsString(), 12, 10)));
            DataModul.DT_DescendentTable.Index = "PerNr";
            DataModul.DT_DescendentTable.Seek("=", Modul1.PersInArb);
            if (DataModul.DT_DescendentTable.NoMatch)
            {
                if (Gen < 0)
                {
                    _ = Interaction.MsgBox("Stop", mb: MessageBoxButtons.OK, title: "18");
                }
                if (Gen > _genMax)
                {
                    _genMax = Gen;
                }
                DataModul.DT_DescendentTable.AddNew();
                DataModul.DT_DescendentTable.Fields["Gen"].Value = "  " + Gen.AsString().Right(2);
                DataModul.DT_DescendentTable.Fields["Nr"].Value = _nachnr + "  " + (i + 1).AsString().Right(2) + ".";
                DataModul.DT_DescendentTable.Fields["Pr"].Value = Strings.Mid(List1.Items[i].AsString(), 12, 30);
                DataModul.DT_DescendentTable.Fields["kia"].Value = Strings.Mid(List1.Items[i].AsString(), 10, 2);
                DataModul.DT_DescendentTable.Update();
                ProjectData.ClearProjectError();
            }
            else if (Operators.ConditionalCompareObjectGreater(DataModul.DT_DescendentTable.Fields["Nr"].AsInt(), _nachnr + "  " + (i + 1).AsString().Right(2) + ".", TextCompare: false))
            {
                if (DataModul.DT_DescendentTable.Fields["Gen"].AsInt() >= Gen)
                {
                    DataModul.DT_DescendentTable.Edit();
                    DataModul.DT_DescendentTable.Fields["Gen"].Value = "  " + Gen.AsString().Right(2);
                    DataModul.DT_DescendentTable.Fields["Nr"].Value = _nachnr + "  " + (i + 1).AsString().Right(2) + ".";
                    DataModul.DT_DescendentTable.Update();
                }
            }
            DataModul.NB_OrtindTable.AddNew();
            DataModul.NB_OrtindTable.Fields["Name"].Value = _nachnr + "  " + (i + 1).AsString().Right(2) + ".";
            DataModul.NB_OrtindTable.Fields["OrtNr"].Value = Strings.Mid(List1.Items[i].AsString(), 12, 30);
            DataModul.NB_OrtindTable.Fields["Ort"].Value = "  " + Gen.AsString().Right(2);
            DataModul.NB_OrtindTable.Fields["Ind"].Value = "=";
            DataModul.NB_OrtindTable.Update();
            i++;
        }
    }

    private int M1_Iter; // Iterator counter for ancester processing

    private void ProcessAncesterStep(ref double num5, ref string value, ref int number)
    {
        Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
        Application.DoEvents();
        Modul1.Kont[0] = Modul1.Person.SurName.ToUpper();
        if (Modul1.Person.SurName.Trim() == "")
        {
            Modul1.Kont[0] = "NN";
        }
        Modul1.Kont[0] = Modul1.Umlaute_UCase(Modul1.Kont[0]);
        DataModul.DT_AncesterTable.Index = "PerNr";
        DataModul.DT_AncesterTable.Seek("=", Modul1.PersInArb);
        if (_maxGen < Gen)
        {
            _maxGen = Gen;
        }
        Modul1.Schalt = 1;
        Modul1.eLKennz = ELinkKennz.lkChild;
        Modul1.Ubg = famsuch1(Modul1.PersInArb, Modul1.eLKennz);
        Spitz = Modul1.Ubg == 0 ? "1" : "0";
        if (DataModul.DT_AncesterTable.NoMatch)
        {
            DataModul.DT_AncesterTable.AddNew();
            if (Modul1.PersInArb == 0)
            {
                return;
            }
            DataModul.DT_AncesterTable.Fields["PerNr"].Value = Modul1.PersInArb;
            DataModul.DT_AncesterTable.Fields["Gen"].Value = Gen;
            DataModul.DT_AncesterTable.Fields["Ahn"].Value = new string(' ', 40) + num5.AsString().Right(40);
            DataModul.DT_AncesterTable.Fields["Weiter"].Value = "0";
            DataModul.DT_AncesterTable.Fields["Ehe"].Value = Modul1.FamInArb;
            DataModul.DT_AncesterTable.Fields["Name"].Value = Modul1.Kont[0];
            DataModul.DT_AncesterTable.Fields["aiSpitz"].Value = Spitz;
            DataModul.DT_AncesterTable.Update();
        }
        else if (DataModul.DT_AncesterTable.Fields["Ahn"].AsInt() != 0)
        {
            DataModul.DT_AncesterTable.Edit();
            DataModul.DT_AncesterTable.Fields["Weiter"].Value = "1";
            DataModul.DT_AncesterTable.Update();
            DataModul.DT_AncesterTable.AddNew();
            DataModul.DT_AncesterTable.Fields["PerNr"].Value = Modul1.PersInArb;
            DataModul.DT_AncesterTable.Fields["Gen"].Value = Gen;
            DataModul.DT_AncesterTable.Fields["Ahn"].Value = new string(' ', 40) + num5.AsString().Right(40);
            if (Strings.InStr(DataModul.DT_AncesterTable.Fields["Ahn"].AsString(), "E") != 0)
            {
                Debugger.Break();
            }
            DataModul.DT_AncesterTable.Fields["Weiter"].Value = "1";
            if (Modul1.FamInArb == 0)
            {
                _ = Interaction.MsgBox("Stop", mb: MessageBoxButtons.OK, title: "13");
            }
            DataModul.DT_AncesterTable.Fields["Ehe"].Value = Modul1.FamInArb;
            DataModul.DT_AncesterTable.Fields["Name"].Value = Modul1.Person.SurName;
            DataModul.DT_AncesterTable.Fields["aiSpitz"].Value = Spitz;
            D = 1;
            DataModul.DT_AncesterTable.Update();
            D = 0;
            //====================================================================================================
        }
        else
        {
            DataModul.DT_AncesterTable.Edit();
            DataModul.DT_AncesterTable.Fields["Ahn"].Value = new string(' ', 40) + num5.AsString().Right(40);
            DataModul.DT_AncesterTable.Fields["Gen"].Value = Gen;
            if (Modul1.FamInArb == 0)
            {
                _ = Interaction.MsgBox("Stop", mb: MessageBoxButtons.OK, title: "14");
            }
            DataModul.DT_AncesterTable.Fields["Ehe"].Value = Modul1.FamInArb;
            DataModul.DT_AncesterTable.Fields["Name"].Value = Modul1.Person.SurName;
            DataModul.DT_AncesterTable.Fields["aiSpitz"].Value = Spitz;
            DataModul.DT_AncesterTable.Update();
        }

        DataModul.DT_KindAhnTable.AddNew();
        if (Modul1.PersInArb == 0)
        {
            return;
        }
        DataModul.DT_KindAhnTable.Fields["PerNr"].Value = Modul1.PersInArb;
        DataModul.DT_KindAhnTable.Fields["Ahn"].Value = new string(' ', 40) + num5.AsString().Right(40);
        DataModul.DT_KindAhnTable.Update();
        _Label1_3.Text = Modul1.PersInArb.AsString();
        Modul1.Schalt = 1;
        Modul1.eLKennz = ELinkKennz.lkChild;
        Modul1.Ubg = famsuch1(Modul1.PersInArb, Modul1.eLKennz);
        Modul1.FamInArb = Modul1.Ubg;
        Gen++;
        if (!(Gen == 135 || Gen > Modul1_Gen1.AsDouble() + 1.0))
        {
            if (Modul1.Ubg != 0)
            {
                Modul1.Family.Mann = 0;
                Modul1.Family.Frau = 0;
                Modul1.Ubg = 0;
                Perprüf = Modul1.PersInArb;
                DataModul.Link.ReadFamily(Modul1.FamInArb, Modul1.Family);
                if (!(Modul1.Family.Mann == 0 && Modul1.Family.Frau == 0))
                {
                    Modul1.PersInArb = Modul1.Family.Mann;
                    if (Modul1.Family.Mann == 0)
                    {
                        if (Modul1.Schalt < 2)
                        {
                            num5 *= 2d;
                        }
                        if (Modul1.Schalt < 2)
                        {
                            value = (num5 + 1).AsString();
                        }
                        else
                            Modul1.Frauen_Renamed.SetKek1(-1);
                    }
                    if (Modul1.PersInArb > 0)
                    {
                        if (Perprüf == Modul1.PersInArb)
                        {
                            var prompt = "Person" + Perprüf.AsString() + " ist sein eigener Vorfahre. Diesen Fehler müssen Sie erst berichtigen.";
                            _ = Interaction.MsgBox(prompt, title: "Dateifehler", icon: MessageBoxIcon.Exclamation);
                            Close();
                            Menue.Default.Show();
                            return;
                        }
                        if (Perprüf == Modul1.Family.Frau)
                        {
                            Debugger.Break();
                        }
                        if (Modul1.Schalt < 2)
                        {
                            num5 *= 2d;
                        }
                        if (Modul1.Schalt < 2)
                        {
                            value = (num5 + 1d).AsString();
                        }
                        else
                            Modul1.Frauen_Renamed.SetKek1(-1);
                    }
                    if (Modul1.Family.Frau > 0)
                    {
                        DataModul.NB_FrauTable.Index = "PerNR";
                        DataModul.NB_FrauTable.Seek("=", Modul1.Family.Frau);
                        if (DataModul.NB_FrauTable.NoMatch)
                        {
                            DataModul.NB_FrauTable.AddNew();
                            DataModul.NB_FrauTable.Fields["Nr"].Value = Modul1.Family.Frau;
                            DataModul.NB_FrauTable.Fields["Kek"].Value = value;
                            DataModul.NB_FrauTable.Fields["Gen"].Value = Gen;
                            DataModul.NB_FrauTable.Fields["LfNr"].Value = Modul1_Nr1;
                            Modul1_Nr1++;
                            DataModul.NB_FrauTable.Fields["Alt"].Value = Modul1.FamInArb.AsString();
                            DataModul.NB_FrauTable.Update();
                        }
                        else
                        {
                            DataModul.NB_FrauTable.AddNew();
                            DataModul.NB_FrauTable.Fields["Nr"].Value = Modul1.Family.Frau;
                            DataModul.NB_FrauTable.Fields["Kek"].Value = value;
                            DataModul.NB_FrauTable.Fields["Gen"].Value = Gen;
                            DataModul.NB_FrauTable.Fields["LfNr"].Value = Modul1_Nr1;
                            Modul1_Nr1++;
                            DataModul.NB_FrauTable.Fields["Alt"].Value = Modul1.FamInArb.AsString();
                            DataModul.NB_FrauTable.Update();
                        }
                    }
                    Modul1.Family.Frau = 0;
                    if (Modul1.Family.Mann <= 0)
                    {
                        if (Modul1.PersInArb != 0)
                        {
                            M1_Iter++;
                        }
                    }
                }
            }
        }
        return ;
    }

}