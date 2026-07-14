using BaseLib.Helper;
using GenFreeWin.Views;
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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace GenFreeWin.ViewModels;

public partial class RechTextViewModel : BaseViewModelCT, IRechTextViewModel
{
    IContainerControl IRechTextViewModel.View { get; set; }
    RechText View => (RechText)((IRechTextViewModel)this).View;

    IModul1 Modul1 => _Modul1.Instance;
    IInteraction Interaction => Menue.Default;
    IVBConversions Conversion => Modul1.Conversions;
    IVBInformation Information => Modul1.Information;
    IStrings Strings => Modul1.Strings;
    IProjectData ProjectData => Modul1.ProjectData;

    public int FamPerSchalt { get; private set; }
    public int PersInArb { get; private set; }
    public int FamInArb { get; private set; }

    private int Nr;

    private int I;

    private int Lz;

    private float Z;

    private float W;


    private object Modul1_LiText;
    private string Modul1_Persex;
    private (string, ETextKennz) Modul1_Bezeichnu;

    public void Bef_Click(object eventSender, EventArgs eventArgs)
    {
        //Discarded unreachable code: IL_4230, IL_449b, IL_4a4e
        int try0001_dispatch = -1;
        int num = default;
        short num5 = default;
        int num2 = default;
        int num3 = default;
        int lErl = default;
        int num6 = default;
        int num10 = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num4;
                    ListBox L;
                    int i;
                    int i4;
                    int num7;
                    switch (try0001_dispatch)
                    {
                        default:
                            num = 1;
                            num5 = (short)View.ABef.GetIndex((Button)eventSender);
                            goto IL_0016;
                        case 23928:
                            {
                                num2 = num;
                                switch ((num3 <= -2) ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_5016;
                                    default:
                                        goto end_IL_0001;
                                }

                                int number = Information.Err().Number;
                                switch (number)
                                {
                                    case 25:
                                        var Modul1_Value = Interaction.MsgBox("Bitte den Drucker bereit machen.", title: "", mb: MessageBoxButtons.OKCancel);
                                        if (Modul1_Value == DialogResult.Cancel)
                                        {
                                            num5 = 0;
                                            goto end_IL_0001_2;
                                        }
                                        else
                                        {
                                            ProjectData.ClearProjectError();
                                            if (num2 == 0)
                                            {
                                                throw ProjectData.CreateProjectError(-2146828268);
                                            }
                                            goto IL_5012;
                                        }

                                    case 55:
                                        ProjectData.ClearProjectError();
                                        if (num2 == 0)
                                        {
                                            throw ProjectData.CreateProjectError(-2146828268);
                                        }
                                        goto IL_5016;
                                    case 63:
                                        num5 = 0;
                                        goto end_IL_0001_2;
                                    case 91:
                                        ProjectData.ClearProjectError();
                                        if (num2 == 0)
                                        {
                                            throw ProjectData.CreateProjectError(-2146828268);
                                        }
                                        goto IL_5016;
                                    case 3021:
                                        ProjectData.ClearProjectError();
                                        if (num2 == 0)
                                        {
                                            throw ProjectData.CreateProjectError(-2146828268);
                                        }
                                        goto IL_5016;
                                    case 3022:
                                        ProjectData.ClearProjectError();
                                        if (num2 == 0)
                                        {
                                            throw ProjectData.CreateProjectError(-2146828268);
                                        }
                                        goto IL_5016;
                                    case 3167:
                                        Debugger.Break();
                                        goto end_IL_0001_2;
                                    case 3420:
                                        ProjectData.ClearProjectError();
                                        if (num2 == 0)
                                        {
                                            throw ProjectData.CreateProjectError(-2146828268);
                                        }
                                        goto IL_5016;
                                    default:
                                        if (Interaction.MsgBox(Conversion.ErrorToString(), mb: MessageBoxButtons.OKCancel, title: Information.Err().Number.AsString()) == DialogResult.Cancel)
                                        {
                                            ProjectData.EndApp();
                                        }
                                        ProjectData.ClearProjectError();
                                        if (num2 == 0)
                                        {
                                            throw ProjectData.CreateProjectError(-2146828268);
                                        }
                                        goto IL_5012;
                                }
                            }
                        end_IL_0001_3:
                            break;
                        IL_0016:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            View.List4.Visible = true;
                            View.Liste1.Visible = false;
                            DataModul.WB_FrauTable.MoveFirst();
                            goto IL_00b0;
                        IL_00b0: // <========== 3
                            num = 8;
                            while (!DataModul.WB_FrauTable.EOF)
                            {
                                DataModul.WB_FrauTable.Edit();
                                DataModul.WB_FrauTable.Fields["Nr"].Value = 0;
                                DataModul.WB_FrauTable.Update();
                                DataModul.WB_FrauTable.MoveNext();
                            }
                            View.List1.Items.Clear();
                            switch (num5)
                            {
                                case 0:
                                    View.List4.Items.Clear();
                                    View.List1.Items.Clear();
                                    View.List2.Items.Clear();
                                    View.List3.Items.Clear();
                                    View.Text2.Text = "";
                                    View.Bezeichnung4.Text = "";
                                    View.Bezeichnung4.Tag = 0;
                                    string source = Modul1.InitDir + "NUMTEMP.mdb";
                                    string destination = Modul1.TempPath + "\\NumTemp.mdb";
                                    DataModul.CreateWBDatabase(destination, source);
                                    goto end_IL_0001_2;
                                case 1:
                                    goto IL_02a6;
                                case 2:
                                    MainProject.Forms.Textlesen.Close();
                                    View.Close();
                                    MainProject.Forms.Namensuch.Close();
                                    DataModul.WB_FrauTable.Close();
                                    PersInArb = 0;
                                    FamInArb = 0;
                                    goto end_IL_0001_2;
                                case 3:
                                    _ = View.List4.Items.Count == 0 ? View.List4.Items.Add("Suche " + View.Text1.Text) : View.List4.Items.Add("oder " + View.Text1.Text);
                                    View.Cursor = Cursors.WaitCursor;
                                    Nr = (int)Math.Round(Strings.Mid(View.Bezeichnung1.Text, Modul1.IText[EUserText.t197].Length + 1, View.Bezeichnung1.Text.Length).AsDouble());
                                    Modul1.eNKennz = View.Label2.Tag.AsEnum<ETextKennz>();
                                    ETextKennz kennz = Modul1.eNKennz;
                                    switch (kennz)
                                    {
                                        case ETextKennz.A_:
                                        case ETextKennz.B_:
                                        case ETextKennz.C_:
                                        case ETextKennz.D_:
                                        case ETextKennz.F_:
                                        case ETextKennz.tkName:
                                        case ETextKennz.V_:
                                        case ETextKennz.U_:
                                            DataModul.DB_NameTable.Index = nameof(NameIndex.TxNr);
                                            DataModul.DB_NameTable.Seek("=", Nr);
                                            while (!DataModul.DB_NameTable.EOF
                                                && !DataModul.DB_NameTable.NoMatch
                                                && DataModul.DB_NameTable.Fields[NameFields.Text].AsInt() == Nr && !(DataModul.DB_NameTable.Fields[NameFields.Kennz].AsEnum<ETextKennz>() != Modul1.eNKennz))
                                            {
                                                Modul1.PersInArb = DataModul.DB_NameTable.Fields[NameFields.PersNr].AsInt();
                                                DataModul.WB_FrauTable.AddNew();
                                                DataModul.WB_FrauTable.Fields["LfNr"].Value = Modul1.PersInArb;
                                                DataModul.WB_FrauTable.Fields["Nr"].Value = 0;
                                                DataModul.WB_FrauTable.Update();
                                                lErl = 4;
                                                DataModul.DB_NameTable.MoveNext();
                                            }
                                            goto IL_3cb9;
                                        case ETextKennz.E_:
                                        case ETextKennz.G_:
                                        case ETextKennz.M_:
                                        case ETextKennz.Q_:
                                            View.List1.Items.Clear();
                                            DataModul.DB_EventTable.Index = nameof(EventIndex.KText);
                                            DataModul.DB_EventTable.Seek("=", Nr);
                                            while (!DataModul.DB_EventTable.EOF
                                                && !DataModul.DB_EventTable.NoMatch
                                                && !(DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt() != Nr))
                                            {
                                                if ((DataModul.DB_EventTable.Fields[EventFields.Art].AsInt() < 499))
                                                {
                                                    DataModul.WB_FrauTable.AddNew();
                                                    DataModul.WB_FrauTable.Fields["LfNr"].Value = DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt();
                                                    DataModul.WB_FrauTable.Fields["Nr"].Value = 0;
                                                    DataModul.WB_FrauTable.Update();
                                                }
                                                else
                                                {
                                                    _ = View.List1.Items.Add(Strings.Right("               " + DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt().AsString(), 10) + Strings.Right("          " + DataModul.DB_EventTable.Fields[EventFields.Art].AsInt().AsString(), 10));
                                                }
                                                DataModul.DB_EventTable.MoveNext();
                                            }
                                            goto IL_3cb9;
                                        case ETextKennz.tk5_:
                                            View.List1.Items.Clear();
                                            DataModul.DB_EventTable.Index = nameof(EventIndex.HaNu);
                                            DataModul.DB_EventTable.Seek("=", Nr);
                                            while (!DataModul.DB_EventTable.EOF
                                                && !DataModul.DB_EventTable.NoMatch
                                                && !(DataModul.DB_EventTable.Fields[EventFields.Hausnr].AsInt() != Nr))
                                            {
                                                if ((DataModul.DB_EventTable.Fields[EventFields.Art].AsInt() < 499))
                                                {
                                                    DataModul.WB_FrauTable.AddNew();
                                                    DataModul.WB_FrauTable.Fields["LfNr"].Value = DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt();
                                                    DataModul.WB_FrauTable.Fields["Nr"].Value = 0;
                                                    DataModul.WB_FrauTable.Update();
                                                }
                                                else
                                                {
                                                    _ = View.List1.Items.Add(Strings.Right("               " + DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt().AsString(), 10) + Strings.Right("          " + DataModul.DB_EventTable.Fields[EventFields.Art].AsInt().AsString(), 10));
                                                }
                                                DataModul.DB_EventTable.MoveNext();
                                            }
                                            goto IL_3cb9;
                                        case ETextKennz.O_:
                                            DataModul.DB_EventTable.Index = nameof(EventIndex.PText);
                                            DataModul.DB_EventTable.Seek("=", Nr);
                                            while (!DataModul.DB_EventTable.EOF && !DataModul.DB_EventTable.NoMatch && !(DataModul.DB_EventTable.Fields[EventFields.Platz].AsInt() != Nr))
                                            {
                                                if ((DataModul.DB_EventTable.Fields[EventFields.Art].AsInt() < 499))
                                                {
                                                    DataModul.WB_FrauTable.AddNew();
                                                    DataModul.WB_FrauTable.Fields["LfNr"].Value = DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt();
                                                    DataModul.WB_FrauTable.Fields["Nr"].Value = 0;
                                                    DataModul.WB_FrauTable.Update();
                                                }
                                                else
                                                {
                                                    _ = View.List1.Items.Add(Strings.Right("               " + DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt().AsString(), 10) + Strings.Right("          " + DataModul.DB_EventTable.Fields[EventFields.Art].AsInt().AsString(), 10));
                                                }
                                                DataModul.DB_EventTable.MoveNext();
                                            }
                                            goto IL_3cb9;
                                        case ETextKennz.H_:
                                            DataModul.DB_PlaceTable.Index = nameof(PlaceIndex.O);
                                            View.List2.Items.Clear();
                                            DataModul.DB_PlaceTable.Seek("=", Nr);
                                            while (!DataModul.DB_PlaceTable.EOF)
                                            {
                                                if (!DataModul.DB_PlaceTable.NoMatch)
                                                {
                                                    if (DataModul.DB_PlaceTable.Fields[PlaceFields.Ort].AsInt() == Nr)
                                                    {
                                                        _ = View.List2.Items.Add(DataModul.DB_PlaceTable.Fields[PlaceFields.OrtNr].Value);
                                                    }
                                                    else
                                                        break;
                                                }
                                                else
                                                {
                                                    _ = Interaction.MsgBox("Ortsname " + View.Text1.Text.Left(240).Trim() + " wird nicht verwendet");
                                                }
                                                DataModul.DB_PlaceTable.MoveNext();
                                            }
                                            goto IL_449c;
                                        case ETextKennz.I_:
                                            DataModul.DB_PlaceTable.Index = nameof(PlaceIndex.OT);
                                            View.List2.Items.Clear();
                                            DataModul.DB_PlaceTable.Seek("=", Nr);
                                            while (!DataModul.DB_PlaceTable.EOF)
                                            {
                                                if (!DataModul.DB_PlaceTable.NoMatch)
                                                {
                                                    if (DataModul.DB_PlaceTable.Fields[PlaceFields.Ortsteil].AsInt() == Nr)
                                                    {
                                                        _ = View.List2.Items.Add(DataModul.DB_PlaceTable.Fields[PlaceFields.OrtNr].Value);
                                                    }
                                                    else
                                                        break;
                                                }
                                                else
                                                {
                                                    _ = Interaction.MsgBox("Text " + View.Text1.Text.Left(240).Trim() + " wird nicht verwendet");
                                                }
                                                DataModul.DB_PlaceTable.MoveNext();
                                            }
                                            goto IL_449c;
                                        case ETextKennz.J_:
                                            DataModul.DB_PlaceTable.Index = nameof(PlaceIndex.K);
                                            View.List2.Items.Clear();
                                            DataModul.DB_PlaceTable.Seek("=", Nr);
                                            while (!DataModul.DB_PlaceTable.EOF)
                                            {
                                                if (!DataModul.DB_PlaceTable.NoMatch)
                                                {
                                                    if (DataModul.DB_PlaceTable.Fields[PlaceFields.Kreis].AsInt() == Nr)
                                                    {
                                                        _ = View.List2.Items.Add(DataModul.DB_PlaceTable.Fields[PlaceFields.OrtNr].Value);
                                                    }
                                                    else
                                                        break;
                                                }
                                                else
                                                {
                                                    _ = Interaction.MsgBox("Text " + View.Text1.Text.Left(240).Trim() + " wird nicht verwendet");
                                                }
                                                DataModul.DB_PlaceTable.MoveNext();
                                            }
                                            goto IL_449c;
                                        case ETextKennz.K_:
                                            DataModul.DB_PlaceTable.Index = nameof(PlaceIndex.L);
                                            View.List2.Items.Clear();
                                            DataModul.DB_PlaceTable.Seek("=", Nr);
                                            while (!DataModul.DB_PlaceTable.EOF)
                                            {
                                                if (!DataModul.DB_PlaceTable.NoMatch)
                                                {
                                                    if (DataModul.DB_PlaceTable.Fields[PlaceFields.Land].AsInt() == Nr)
                                                    {
                                                        _ = View.List2.Items.Add(DataModul.DB_PlaceTable.Fields[PlaceFields.OrtNr].Value);
                                                    }
                                                    else
                                                        break;
                                                }
                                                else
                                                {
                                                    _ = Interaction.MsgBox("Text " + View.Text1.Text.Left(240).Trim() + " wird nicht verwendet");
                                                }
                                                DataModul.DB_PlaceTable.MoveNext();
                                            }
                                            goto IL_449c;
                                        case ETextKennz.L_:
                                            DataModul.DB_PlaceTable.Index = nameof(PlaceIndex.S);
                                            View.List2.Items.Clear();
                                            DataModul.DB_PlaceTable.Seek("=", Nr);
                                            while (!DataModul.DB_PlaceTable.EOF)
                                            {
                                                if (!DataModul.DB_PlaceTable.NoMatch)
                                                {
                                                    if (DataModul.DB_PlaceTable.Fields[PlaceFields.Staat].AsInt() == Nr)
                                                    {
                                                        _ = View.List2.Items.Add(DataModul.DB_PlaceTable.Fields[PlaceFields.OrtNr].Value);
                                                    }
                                                    else
                                                        break;
                                                }
                                                else
                                                {
                                                    _ = Interaction.MsgBox("Text " + View.Text1.Text.Left(240).Trim() + " wird nicht verwendet");
                                                }
                                                DataModul.DB_PlaceTable.MoveNext();
                                            }
                                            goto IL_449c;
                                        case ETextKennz.Z_:
                                            DataModul.DT_RelgionTable.Index = "T";
                                            DataModul.DT_RelgionTable.Seek("=", Nr);
                                            while (Lz != 32700
                                                && !DataModul.DT_RelgionTable.EOF
                                                && !DataModul.DT_RelgionTable.NoMatch
                                                && !(DataModul.DT_RelgionTable.Fields["textnr"].AsInt() != Nr))
                                            {
                                                Lz++;
                                                DataModul.WB_FrauTable.AddNew();
                                                DataModul.WB_FrauTable.Fields["LfNr"].Value = DataModul.DT_RelgionTable.Fields["PerNr"].AsInt();
                                                DataModul.WB_FrauTable.Fields["Nr"].Value = 0;
                                                DataModul.WB_FrauTable.Update();
                                                DataModul.DT_RelgionTable.MoveNext();
                                            }
                                            goto IL_3cb9;
                                        case ETextKennz.T_:
                                            DataModul_Event_ForEachIx(EventIndex.NText, EventFields.ArtText, Nr, Handle_EntityEvent);
                                            goto IL_3cb9;
                                        case ETextKennz.tk7_:
                                            DataModul_Person_ForEachIx(PersonIndex.reli, PersonFields.religi, Nr, DataModul.WB_Frau.Commit);
                                            goto IL_3cb9;
                                        default:
                                            _ = Interaction.MsgBox("Nicht möglich");
                                            break;
                                    }
                                    goto end_IL_0001_2;
                                case 4:
                                    L = View.List3;
                                    Modul1.Listbox3Clip(L.Items, 1);

                                    goto end_IL_0001_2;
                                case 5:
                                    MainProject.Forms.Textlesen.Close();
                                    break;
                                default:
                                    break;
                            }
                            goto IL_3edb;

                        IL_02a6:
                            num = 41;
                            View.Cursor = Cursors.WaitCursor;
                            _ = View.List4.Items.Count == 0 ? View.List4.Items.Add("Suche " + View.Text1.Text) : View.List4.Items.Add("und " + View.Text1.Text);
                            Nr = (int)Math.Round(Strings.Mid(View.Bezeichnung1.Text, Modul1.IText[EUserText.t197].Length + 1, View.Bezeichnung1.Text.Length).AsDouble());
                            Modul1.eNKennz = View.Label2.Tag.AsEnum<ETextKennz>();
                            ETextKennz eTKennz;
                            if (Nr > 0)
                            {
                                eTKennz = Modul1.eNKennz;
                                switch (eTKennz)
                                {
                                    case ETextKennz.A_:
                                    case ETextKennz.B_:
                                    case ETextKennz.C_:
                                    case ETextKennz.D_:
                                    case ETextKennz.F_:
                                    case ETextKennz.tkName:
                                    case ETextKennz.V_:
                                    case ETextKennz.U_:

                                        DataModul.DB_NameTable.Index = nameof(NameIndex.TxNr);
                                        DataModul.DB_NameTable.Seek("=", Nr);
                                        while (!DataModul.DB_NameTable.EOF
                                            && !DataModul.DB_NameTable.NoMatch
                                            && !(DataModul.DB_NameTable.Fields[NameFields.Text].AsInt() != Nr))
                                        {
                                            DataModul.WB_FrauTable.Index = "LfNR";
                                            DataModul.WB_FrauTable.Seek("=", DataModul.DB_NameTable.Fields[NameFields.PersNr]);
                                            if (!DataModul.WB_FrauTable.NoMatch)
                                            {
                                                DataModul.WB_FrauTable.Edit();
                                                DataModul.WB_FrauTable.Fields["NR"].Value = 1;
                                                DataModul.WB_FrauTable.Update();
                                            }
                                            DataModul.DB_NameTable.MoveNext();
                                        }
                                        break;
                                    case ETextKennz.E_:
                                    case ETextKennz.G_:
                                    case ETextKennz.M_:
                                    case ETextKennz.Q_:
                                        DataModul.DB_EventTable.Index = nameof(EventIndex.KText);
                                        DataModul.DB_EventTable.Seek("=", Nr);
                                        while (!DataModul.DB_EventTable.EOF
                                            && !DataModul.DB_EventTable.NoMatch)
                                        {
                                            if (DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt() != Nr)
                                                break;

                                            if ((DataModul.DB_EventTable.Fields[EventFields.Art].AsInt() < 499))
                                            {
                                                DataModul.WB_FrauTable.Index = "LfNR";
                                                DataModul.WB_FrauTable.Seek("=", DataModul.DB_EventTable.Fields[EventFields.PerFamNr]);
                                                if (!DataModul.WB_FrauTable.NoMatch)
                                                {
                                                    DataModul.WB_FrauTable.Edit();
                                                    DataModul.WB_FrauTable.Fields["NR"].Value = 1;
                                                    DataModul.WB_FrauTable.Update();
                                                }
                                            }
                                            else
                                            {
                                                _ = View.List1.Items.Add(Strings.Right("               " + DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt().AsString(), 10) + Strings.Right("          " + DataModul.DB_EventTable.Fields[EventFields.Art].AsInt().AsString(), 10));
                                            }

                                            DataModul.DB_EventTable.MoveNext();
                                        }
                                        break;
                                    case ETextKennz.tk5_:
                                        DataModul.DB_EventTable.Index = nameof(EventIndex.HaNu);
                                        DataModul.DB_EventTable.Seek("=", Nr);
                                        while (!DataModul.DB_EventTable.EOF
                                            && !DataModul.DB_EventTable.NoMatch)
                                        {
                                            if (DataModul.DB_EventTable.Fields[EventFields.Hausnr].AsInt() != Nr)
                                                break;

                                            if ((DataModul.DB_EventTable.Fields[EventFields.Art].AsInt() < 499))
                                            {
                                                DataModul.WB_FrauTable.Index = "LfNR";
                                                DataModul.WB_FrauTable.Seek("=", DataModul.DB_EventTable.Fields[EventFields.PerFamNr]);
                                                if (!DataModul.WB_FrauTable.NoMatch)
                                                {
                                                    DataModul.WB_FrauTable.Edit();
                                                    DataModul.WB_FrauTable.Fields["NR"].Value = 1;
                                                    DataModul.WB_FrauTable.Update();
                                                }
                                            }
                                            else
                                            {
                                                _ = View.List1.Items.Add(Strings.Right("               " + DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt().AsString(), 10) + Strings.Right("          " + DataModul.DB_EventTable.Fields[EventFields.Art].AsInt().AsString(), 10));
                                            }
                                            DataModul.DB_EventTable.MoveNext();
                                        }
                                        break;
                                    case ETextKennz.O_:
                                        DataModul.DB_EventTable.Index = nameof(EventIndex.PText);
                                        DataModul.DB_EventTable.Seek("=", Nr);
                                        while (!DataModul.DB_EventTable.EOF
                                            && !DataModul.DB_EventTable.NoMatch
                                            && !(DataModul.DB_EventTable.Fields[EventFields.Platz].AsInt() != Nr))
                                        {
                                            if ((DataModul.DB_EventTable.Fields[EventFields.Art].AsInt() < 499))
                                            {
                                                DataModul.WB_FrauTable.Index = "LfNR";
                                                DataModul.WB_FrauTable.Seek("=", DataModul.DB_EventTable.Fields[EventFields.PerFamNr]);
                                                if (!DataModul.WB_FrauTable.NoMatch)
                                                {
                                                    DataModul.WB_FrauTable.Edit();
                                                    DataModul.WB_FrauTable.Fields["NR"].Value = 1;
                                                    DataModul.WB_FrauTable.Update();
                                                }
                                            }
                                            else
                                            {
                                                _ = View.List1.Items.Add(Strings.Right("               " + DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt().AsString(), 10) + Strings.Right("          " + DataModul.DB_EventTable.Fields[EventFields.Art].AsInt().AsString(), 10));
                                            }
                                            DataModul.DB_EventTable.MoveNext();
                                        }
                                        break;
                                    case ETextKennz.H_:
                                        DataModul.DB_PlaceTable.Index = nameof(PlaceIndex.O);
                                        View.List2.Items.Clear();
                                        DataModul.DB_PlaceTable.Seek("=", Nr);
                                        while (!DataModul.DB_PlaceTable.EOF
                                            && !DataModul.DB_PlaceTable.NoMatch
                                            && !(DataModul.DB_PlaceTable.Fields[PlaceFields.Ort].AsInt() != Nr))
                                        {
                                            _ = View.List2.Items.Add(DataModul.DB_PlaceTable.Fields[PlaceFields.OrtNr].Value);
                                            DataModul.DB_PlaceTable.MoveNext();
                                        }
                                        goto IL_4a4f;
                                    //break;
                                    case ETextKennz.I_:
                                        DataModul.DB_PlaceTable.Index = nameof(PlaceIndex.OT);
                                        View.List2.Items.Clear();
                                        DataModul.DB_PlaceTable.Seek("=", Nr);
                                        while (!DataModul.DB_PlaceTable.EOF
                                            && !DataModul.DB_PlaceTable.NoMatch
                                            && !(DataModul.DB_PlaceTable.Fields[PlaceFields.Ortsteil].AsInt() != Nr))
                                        {
                                            _ = View.List2.Items.Add(DataModul.DB_PlaceTable.Fields[PlaceFields.OrtNr].Value);
                                            DataModul.DB_PlaceTable.MoveNext();
                                        }
                                        goto IL_4a4f;
                                    //  break;
                                    case ETextKennz.J_:
                                        DataModul.DB_PlaceTable.Index = nameof(PlaceIndex.K);
                                        View.List2.Items.Clear();
                                        DataModul.DB_PlaceTable.Seek("=", Nr);
                                        while (!DataModul.DB_PlaceTable.EOF
                                            && !DataModul.DB_PlaceTable.NoMatch
                                            && !(DataModul.DB_PlaceTable.Fields[PlaceFields.Kreis].AsInt() != Nr))
                                        {
                                            _ = View.List2.Items.Add(DataModul.DB_PlaceTable.Fields[PlaceFields.OrtNr].Value);
                                            DataModul.DB_PlaceTable.MoveNext();
                                        }
                                        goto IL_4a4f;
                                    // break;
                                    case ETextKennz.L_:
                                        DataModul.DB_PlaceTable.Index = nameof(PlaceIndex.S);
                                        View.List2.Items.Clear();
                                        DataModul.DB_PlaceTable.Seek("=", Nr);
                                        while (!DataModul.DB_PlaceTable.EOF
                                            && !DataModul.DB_PlaceTable.NoMatch
                                            && !(DataModul.DB_PlaceTable.Fields[PlaceFields.Staat].AsInt() != Nr))
                                        {
                                            _ = View.List2.Items.Add(DataModul.DB_PlaceTable.Fields[PlaceFields.OrtNr].Value);
                                            DataModul.DB_PlaceTable.MoveNext();
                                        }
                                        goto IL_4a4f;
                                    //   break;
                                    case ETextKennz.K_:
                                        DataModul.DB_PlaceTable.Index = nameof(PlaceIndex.L);
                                        View.List2.Items.Clear();
                                        DataModul.DB_PlaceTable.Seek("=", Nr);
                                        while (!DataModul.DB_PlaceTable.EOF && !DataModul.DB_PlaceTable.NoMatch && !(DataModul.DB_PlaceTable.Fields[PlaceFields.Land].AsInt() != Nr))
                                        {
                                            _ = View.List2.Items.Add(DataModul.DB_PlaceTable.Fields[PlaceFields.OrtNr].Value);
                                            DataModul.DB_PlaceTable.MoveNext();
                                        }
                                        goto IL_4a4f;
                                    //  break;
                                    case ETextKennz.T_:
                                        DataModul_Event_ForEachIx(EventIndex.NText, EventFields.ArtText, Nr, Handle_EntityEvent);
                                        break;
                                    case ETextKennz.Z_:
                                        DataModul_Religion_ForEachIx();
                                        break;
                                    case ETextKennz.tk7_:
                                        DataModul_Person_ForEachIx(PersonIndex.reli, PersonFields.religi, Nr, DataModul.WB_Frau.Commit);
                                        break;
                                    default:
                                        _ = Interaction.MsgBox("Nicht möglich");
                                        goto end_IL_0001_2;
                                        // break;
                                }
                            }
                            int num12 = View.List1.Items.Count - 1;
                            I = 0;
                            while (I <= num12)
                            {
                                var (iFam, eArt) = View.List1.Items[I].ItemData<(int, EEventArt)>();
                                if (eArt > EEventArt.eA_499)
                                {
                                    DataModul.Link.ReadFamily(iFam, Modul1.Family);
                                    DataModul.WB_Frau.SetParentTo1(Modul1.Family);
                                }
                                I++;
                            }
                            View.List1.Items.Clear();
                            DataModul.WB_Frau.DeleteEmpty();
                            DataModul.WB_Frau.ClearNr();
                            View.Bezeichnung4.Text = DataModul.WB_FrauTable.RecordCount.AsString() + " Einträge gefunden.";
                            View.Bezeichnung4.Tag = DataModul.WB_FrauTable.RecordCount;
                            View.Bezeichnung1.Text = "";
                            View.Text1.Text = "";
                            View._Bef_1.Enabled = false;
                            View._Bef_3.Enabled = false;
                            View._Bef_0.Enabled = true;
                            goto IL_3edb;

                        IL_3cb9: // <========== 16
                            num = 629;
                            int num9 = View.List1.Items.Count - 1;
                            I = 0;
                            while (I <= num9)
                            {
                                View.List1.SelectedIndex = I;
                                if (View.List1.Text.Length > 10)
                                {
                                    if (Strings.Mid(View.List1.Text, 11, 10).AsDouble() > 499.0)
                                    {
                                        View.List1.SelectedIndex = I;
                                        DataModul.Link.ReadFamily(View.List1.Text.Left(10).AsInt(), Modul1.Family);
                                        DataModul.WB_Frau.AddParent(Modul1.Family);
                                    }
                                }
                                I++;
                            }
                            if (DataModul.WB_FrauTable.RecordCount <= 0)
                            {
                                lErl = 5;
                                View.Liste1.Items.Clear();
                                goto end_IL_0001_2;
                            }
                            else
                            {
                                View.Bezeichnung4.Text = DataModul.WB_FrauTable.RecordCount.AsString() + " Einträge gefunden.";
                                View.Bezeichnung4.Tag = DataModul.WB_FrauTable.RecordCount;
                                View.Bezeichnung6.Text = "Suchverlauf";
                                View.Bezeichnung1.Text = "";
                                View.Text1.Text = "";
                                View._Bef_1.Enabled = false;
                                View._Bef_3.Enabled = false;
                                View._Bef_0.Enabled = true;
                                goto IL_3edb;
                            }
                        IL_3edb: // <========== 7
                            num = 658;
                            lErl = 66;
                            View.List3.Items.Clear();
                            if (DataModul.WB_FrauTable.RecordCount == 0)
                            {
                                _ = View.List3.Items.Add("Kein Eintrag gefunden");
                                View._Bef_1.Enabled = false;
                                View._Bef_3.Enabled = false;
                                View._Bef_0.Enabled = true;
                                View.Cursor = Cursors.Arrow;
                                goto end_IL_0001_2;
                            }
                            DataModul.WB_Frau.ForAll(DataModul_SearchTab_GetNameDate);
                            DataModul.WB_FrauTable.Index = "Name";
                            DataModul.WB_FrauTable.MoveFirst();
                            while (!DataModul.WB_FrauTable.EOF)
                            {
                                if (0 != DataModul.WB_FrauTable.Fields["LfNr"].AsInt())
                                {
                                    Modul1.PersInArb = DataModul.WB_FrauTable.Fields["LfNr"].AsInt();
                                    Zeigfam(Modul1.PersInArb);
                                }
                                DataModul.WB_FrauTable.MoveNext();
                            }
                            View.Cursor = Cursors.Arrow;

                            goto end_IL_0001_2;
                        IL_449c: // <========== 11
                            num = 742;
                            lErl = 6;
                            View.List1.Items.Clear();
                            num6 = View.List2.Items.Count - 1;
                            I = 0;
                            while (I <= num6)
                            {
                                View.List2.SelectedIndex = I;
                                Nr = View.List2.Text.AsInt();

                                DataModul.DB_EventTable.Index = nameof(EventIndex.EOrt);
                                DataModul.DB_EventTable.Seek("=", Nr);
                                while (!DataModul.DB_EventTable.EOF
                                    && !DataModul.DB_EventTable.NoMatch)
                                {
                                    EEventArt eArt = DataModul.DB_EventTable.Fields[EventFields.Art].AsEnum<EEventArt>();

                                    int iOrt = DataModul.DB_EventTable.Fields[EventFields.Ort].AsInt();
                                    int iPerFamNr = DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt();
                                    if (eArt < EEventArt.eA_100)
                                    {
                                        DataModul.DB_EventTable.Delete();
                                    }
                                    else if (!(iOrt != Nr))
                                    {
                                        if (eArt < EEventArt.eA_499)
                                        {
                                            if (0 != iPerFamNr)
                                            {
                                                DataModul.WB_Frau.AddRow(iPerFamNr);
                                            }
                                        }
                                        else
                                        {
                                            _ = View.List1.Items.Add(Strings.Right("               " + DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt().AsString(), 10) + Strings.Right("          " + eArt.AsString(), 10));
                                        }
                                    }
                                    else
                                        break;
                                    lErl = 3;
                                    DataModul.DB_EventTable.MoveNext();
                                }
                                I++;
                            }
                            View.Bezeichnung4.Text = "Keine Einträge gefunden.";
                            View.Bezeichnung4.Tag = 0;
                            int num8 = View.List1.Items.Count - 1;
                            I = 0;
                            while (I <= num8)
                            {
                                View.List1.SelectedIndex = I;
                                if (View.List1.Text.Length > 10)
                                {
                                    if (Strings.Mid(View.List1.Text, 11, 10).AsDouble() > 499.0)
                                    {
                                        View.List1.SelectedIndex = I;
                                        DataModul.Link.ReadFamily(View.List1.Text.Left(10).AsInt(), Modul1.Family);
                                        DataModul.WB_Frau.AddParent(Modul1.Family);
                                    }
                                }
                                I++;
                            }
                            if (DataModul.WB_FrauTable.RecordCount > 0)
                            {
                                View.Bezeichnung4.Text = DataModul.WB_FrauTable.RecordCount.AsString() + " Einträge gefunden.";
                                View.Bezeichnung4.Tag = DataModul.WB_FrauTable.RecordCount;
                                View.Bezeichnung1.Text = "";
                                View.Text1.Text = "";
                                View._Bef_1.Enabled = false;
                                View._Bef_3.Enabled = false;
                                View._Bef_0.Enabled = true;
                                goto IL_3edb;
                            }
                            else
                            {
                                View.Text1.Text = "";
                                View._Bef_1.Enabled = false;
                                View._Bef_3.Enabled = false;
                                break;
                            }
                        IL_4a4f: // <========== 11
                            num = 799;
                            lErl = 7;
                            View.List1.Items.Clear();
                            num10 = View.List2.Items.Count - 1;
                            I = 0;
                            while (I <= num10)
                            {
                                View.List2.SelectedIndex = I;
                                Nr = View.List2.Text.AsInt();
                                foreach (var cEv in DataModul.Event.ReadAll(EventIndex.EOrt, Nr))
                                {
                                    Handle_EntityEventA(cEv.eArt, cEv.iPerFamNr, DataModul.WB_Frau.AddRow);
                                }
                                I++;
                            }
                            int num11 = View.List1.Items.Count - 1;
                            I = 0;
                            while (I <= num11)
                            {
                                var (iFam, eArt) = View.List1.Items[I].ItemData<(int, EEventArt)>();
                                if (eArt > EEventArt.eA_499)
                                {
                                    DataModul.Link.ReadFamily(iFam, Modul1.Family);
                                    DataModul.WB_Frau.SetParentTo1(Modul1.Family);
                                }
                                I++;
                            }
                            DataModul.WB_Frau.DeleteEmpty();

                            View.Bezeichnung4.Text = DataModul.WB_Frau.Count.AsString() + " Einträge gefunden.";
                            View.Bezeichnung4.Tag = DataModul.WB_FrauTable.RecordCount;
                            View.Bezeichnung1.Text = "";
                            View.Text1.Text = "";
                            View._Bef_1.Enabled = false;
                            View._Bef_3.Enabled = false;
                            View._Bef_0.Enabled = true;

                            goto IL_3edb;

                        IL_5012:
                        IL_5016: // <========== 6
                            num4 = unchecked(num2 + 1);
                            goto IL_501a;
                        IL_501a:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 7:
                                case 8:
                                case 13:
                                    goto IL_00b0;
                                case 365:
                                case 384:
                                case 390:
                                case 399:
                                case 413:
                                case 422:
                                case 436:
                                case 444:
                                case 458:
                                case 477:
                                case 496:
                                case 515:
                                case 534:
                                case 553:
                                case 562:
                                case 573:
                                case 577:
                                case 585:
                                case 599:
                                case 607:
                                case 624:
                                case 628:
                                case 629:
                                    goto IL_3cb9;
                                case 16:
                                case 39:
                                case 343:
                                case 344:
                                case 353:
                                case 645:
                                case 650:
                                case 654:
                                case 657:
                                case 658:
                                case 792:
                                case 851:
                                    goto IL_3edb;
                                case 467:
                                case 476:
                                case 486:
                                case 495:
                                case 505:
                                case 514:
                                case 524:
                                case 533:
                                case 543:
                                case 552:
                                case 742:
                                    goto IL_449c;
                                case 797:
                                    goto end_IL_0001_3;
                                case 157:
                                case 163:
                                case 173:
                                case 179:
                                case 189:
                                case 195:
                                case 205:
                                case 211:
                                case 246:
                                case 255:
                                case 799:
                                    goto IL_4a4f;
                                case 38:
                                case 307:
                                case 352:
                                case 627:
                                case 649:
                                case 653:
                                case 666:
                                case 692:
                                case 694:
                                case 700:
                                case 701:
                                case 704:
                                case 705:
                                case 706:
                                case 709:
                                case 710:
                                case 713:
                                case 714:
                                case 717:
                                case 718:
                                case 721:
                                case 722:
                                case 725:
                                case 726:
                                case 729:
                                case 732:
                                case 733:
                                case 739:
                                case 740:
                                case 741:
                                case 798:
                                case 852:
                                    goto end_IL_0001_2;
                            }
                            goto default;
                    }
                    num = 797;
                    View._Bef_0.Enabled = true;
                    break;
                }
            end_IL_0001:
                ;
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 23928;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001_2: // <========== 12
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    private void DataModul_Religion_ForEachIx()
    {
        IRecordset dT_RelgionTable = DataModul.DT_RelgionTable;
        dT_RelgionTable.Index = ReligionIndex.T.AsFld();
        dT_RelgionTable.Seek("=", Nr);
        Lz = 0;
        while (Lz < 32700
            && !dT_RelgionTable.EOF
            && !dT_RelgionTable.NoMatch
            && !(dT_RelgionTable.Fields[ReligionFields.TextNr].AsInt() != Nr))
        {
            int iPerNr = dT_RelgionTable.Fields[ReligionFields.PerNr].AsInt();
            if (DataModul.WB_Frau.Update(iPerNr))
                Lz++;

            dT_RelgionTable.MoveNext();
        }
    }


    private static (string, string) DataModul_SearchTab_GetNameDate(int iPerFamNr)
    {
        IRecordset dSB_SearchTable = DataModul.DSB_SearchTable;
        dSB_SearchTable.Index = "Nummer";
        dSB_SearchTable.Seek("=", iPerFamNr);
        var sName = dSB_SearchTable.Fields["Name"].AsString();
        var sDatum = dSB_SearchTable.Fields["Datum"].AsString();
        return (sName, sDatum);
    }

    private void DataModul_Event_ForEachIx(EventIndex nText, Enum artText, int nr, Action<EEventArt, int> Handle_EntityEvent)
    {
        IRecordset dB_EventTable = DataModul.DB_EventTable;
        dB_EventTable.Index = nText.AsFld();
        dB_EventTable.Seek("=", nr);
        while (!dB_EventTable.EOF
            && !dB_EventTable.NoMatch
            && dB_EventTable.Fields[artText].AsInt() == Nr)
        {
            EEventArt eArt = dB_EventTable.Fields[EventFields.Art].AsEnum<EEventArt>();
            int iPerFamNr = dB_EventTable.Fields[EventFields.PerFamNr].AsInt();
            Handle_EntityEvent(eArt, iPerFamNr);
            dB_EventTable.MoveNext();
        }
    }

    private void DataModul_Person_ForEachIx(PersonIndex Idx, PersonFields ixF, int Nr, Action<int> DataModul_WB_Frau_Commit)
    {
        IRecordset dB_PersonTable = DataModul.DB_PersonTable;
        dB_PersonTable.Index = Idx.AsFld();
        dB_PersonTable.Seek("=", Nr);
        while (!dB_PersonTable.EOF
            && !dB_PersonTable.NoMatch
            && !(dB_PersonTable.Fields[ixF].AsInt() != Nr))
        {
            int iPers = dB_PersonTable.Fields[PersonFields.PersNr].AsInt();

            DataModul_WB_Frau_Commit(iPers);
            dB_PersonTable.MoveNext();
        }
    }


    private void Handle_EntityEvent(EEventArt eArt, int iPerFamNr)
        => Handle_EntityEventA(eArt, iPerFamNr, DataModul.WB_Frau.AddRow);
    private void Handle_EntityEventA(EEventArt eArt, int iPerFamNr, Action<int> DataModul_WB_Frau_AddRow)
    {
        if (eArt < EEventArt.eA_499)
        {
            DataModul_WB_Frau_AddRow(iPerFamNr);
        }
        else
        {
            _ = View.List1.Items.Add(new ListItem<(int, EEventArt)>(iPerFamNr.AsString().PadLeft(10, ' ') + eArt.AsString().PadLeft(10, ' '), (iPerFamNr, eArt)));
        }
    }
    public void Command1_Click(object eventSender, EventArgs eventArgs)
    {
        //Discarded unreachable code: IL_065a, IL_06b5
        int try0001_dispatch = -1;
        int num = default;
        short index = default;
        int num2 = default;
        int num3 = default;
        int persInArb = default;
        int num4 = default;
        int num9 = default;
        int num5;
        int i;
        int i3;
        int num7;
        switch (try0001_dispatch)
        {
            default:
                num = 1;
                index = (short)View.ACommand1.GetIndex((Button)eventSender);
                goto IL_0017;
            case 2488:
                {
                    num2 = num;
                    switch ((num3 <= -2) ? 1 : num3)
                    {
                        case 3:
                            break;
                        case 2:
                            goto IL_06b7;
                        case 1:
                            goto IL_07be;
                        default:
                            goto end_IL_0001;
                    }
                    if (View.List3.Items.Count > 0)
                    {
                        View.List3.SelectedIndex = 0;
                    }
                    View.Bezeichnung4.Text = View.List3.Items.Count.AsString();
                    View.Bezeichnung4.Tag = View.List3.Items.Count;
                    goto end_IL_0001_2;
                }
            end_IL_0001:
                break;
            IL_0017:
                num = 2;
                int num6;
                if (index == 2)
                {
                    if (Modul1.Typ == DriveType.CDRom)
                    {
                        _ = Interaction.MsgBox("Auf der CD nicht möglich!", title: "", icon: MessageBoxIcon.Information);
                    }
                    else
                    {
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        string source = Modul1.InitDir + "GedAUS.mdb";
                        string destination = Modul1.Verz + "Rech\\GEDAUS.mdb";
                        DataModul.PrepRechData(destination, source);
                        num6 = View.List3.Items.Count - 1;
                        I = 0;
                        while (I <= num6
                            && I <= View.List3.Items.Count)
                        {
                            View.List3.SelectedIndex = I;
                            var recordset = DataModul.NB_FrauTable;
                            recordset.AddNew();
                            recordset.Fields["Nr"].Value = Strings.Mid(View.List3.Text, 48, 10).AsInt();
                            recordset.Update();
                            I++;
                        }
                    }
                    goto end_IL_0001_2;
                }
                else
                {
                    View.ListBox1.Items.Clear();
                    ProjectData.ClearProjectError();
                    num3 = 3;
                    int num8 = View.List3.Items.Count - 1;
                    I = 0;
                    while (I <= num8)
                    {
                        View.List3.SelectedIndex = I;
                        _ = View.ListBox1.Items.Add(View.List3.Text);
                        I++;
                    }
                    num4 = 0;
                    View.ListBox1.Refresh();
                    View.List3.Items.Clear();
                    num9 = View.ListBox1.Items.Count - 1;
                    I = 0;
                    goto IL_061b;
                }
            IL_061b:
                i3 = I;
                num7 = num9;
                if (I <= num9
                    && I <= View.ListBox1.Items.Count)
                {
                    View.ListBox1.SelectedIndex = I;
                    //Todo : value from Listbox1
                    Modul1.PersInArb = (int)Math.Round(Strings.Mid(View.ListBox1.Text, 48, 10).AsDouble());
                    var pt = DataModul.Person.Seek(Modul1.PersInArb);
                    object value = pt.Fields[PersonFields.Sex].Value;
                    if (value == "m")
                    {
                        Debugger.Break();
                    }
                    else
                    {
                        if (!(value == "M"))
                        {
                            if (value == "f")
                            {
                                Debugger.Break();
                            }
                            else if (!(value == "F"))
                            {
                                Debugger.Break();
                            }
                        }
                    }
                    switch (index)
                    {
                        case 0:
                            if (DataModul.Person.GetSex(persInArb) != "M")
                            {
                                if (persInArb != Modul1.PersInArb)
                                {
                                    num4++;
                                }
                                _ = View.List3.Items.Add(View.ListBox1.Text);
                            }
                            break;
                        case 1:
                            if (DataModul.Person.GetSex(persInArb) != "F")
                            {
                                if (persInArb != Modul1.PersInArb)
                                {
                                    num4++;
                                }
                                _ = View.List3.Items.Add(View.ListBox1.Text);
                            }
                            break;
                        default:
                            break;
                    }
                    persInArb = Modul1.PersInArb;
                    I++;
                    goto IL_061b;
                }
                else
                {
                    goto IL_062c;
                }
            IL_062c: // <========== 3
                num = 92;
                View.List3.SelectedIndex = 0;
                View.Bezeichnung4.Text = num4.AsString();
                View.Bezeichnung4.Tag = num4;
                goto end_IL_0001_2;
            IL_06b7:
                num = 100;
                int number = Information.Err().Number;
                if (number is 53 or 91)
                {
                    ProjectData.ClearProjectError();
                    if (num2 == 0)
                    {
                        throw ProjectData.CreateProjectError(-2146828268);
                    }
                    goto IL_07be;
                }
                else
                {
                    if (number == 75)
                    {
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        goto IL_07be;
                    }
                    else
                    {
                        if (number == 3022)
                        {
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_07be;
                        }
                        else
                        {
                            if (Interaction.MsgBox(Conversion.ErrorToString(), mb: MessageBoxButtons.OKCancel, title: Information.Err().Number.AsString()) == DialogResult.Cancel)
                            {
                                ProjectData.EndApp();
                            }
                            Debugger.Break();
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            num5 = num2;
                            goto IL_07c2;
                        }
                    }
                }
            IL_07be: // <========== 4
                num5 = unchecked(num2 + 1);
                goto IL_07c2;
            IL_07c2:
                num2 = 0;
                switch (num5)
                {
                    case 1:
                        break;
                    case 68:
                    case 78:
                    case 79:
                    case 88:
                    case 89:
                    case 90:
                    case 46:
                    case 92:
                        goto IL_062c;
                    case 5:
                    case 26:
                    case 33:
                    case 94:
                    case 99:
                    case 122:
                        goto end_IL_0001_2;
                }
                goto default;
        }
    end_IL_0001_2: // <========== 6
        return;
    }
    public void RechText_Load(object eventSender, EventArgs eventArgs)
    {
        //Discarded unreachable code: IL_03a5
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
                string destination;
                switch (try0001_dispatch)
                {
                    default:
                        num = 1;
                        destination = "";
                        goto IL_000a;
                    case 1383:
                        {
                            num2 = num;
                            switch ((num3 <= -2) ? 1 : num3)
                            {
                                case 2:
                                case 3:
                                    break;
                                case 1:
                                    goto IL_0461;
                                default:
                                    goto end_IL_0001;
                            }
                            if (Information.Err().Number == 3420)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_0461;
                            }
                            else
                            {
                                if (Information.Err().Number == 91)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_0461;
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
                                    goto IL_0465;
                                }
                            }
                        }
                    end_IL_0001:
                        break;
                    IL_000a:
                        num = 2;
                        string source = "";
                        MainProject.Forms.Namensuch.Hide();
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        Modul1.Persistence.ReadEnumInit<FormWindowState>("Windowstate", out var WiS);
                        View.WindowState = WiS;
                        var aiPos = Modul1.Persistence.ReadIntsProg("maspos.dat", 2);
                        View.Left = aiPos[0];
                        View.Top = aiPos[1];
                        DataModul.WB_FrauTable.Close();
                        source = Modul1.InitDir + "NUMTEMP.mdb";
                        destination = Modul1.TempPath + "\\NumTemp.mdb";
                        DataModul.CreateWBDatabase(destination, source);
                        if (Modul1.FontSize > 0f)
                        {
                            View.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                            View.List3.Font = new Font("Courier new", Modul1.FontSize, FontStyle.Regular);
                            View.Text1.Font = new Font("Courier new", Modul1.FontSize, FontStyle.Regular);
                        }
                        View.BackColor = Modul1.HintFarb;
                        View._Bef_2.Text = "Suche Abbrechen";
                        goto end_IL_0001_2;
                    IL_0461: // <========== 3
                        num4 = num2 + 1;
                        goto IL_0465;
                    IL_0465:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 40:
                                ProjectData.ClearProjectError();
                                num3 = 3;
                                Modul1.Verz = Modul1.Persistence.ReadStringInit("GEN-verz.ini");
                                Modul1.Verz1 = Modul1.Verz.Left(15);
                                string text = Modul1.Verz.Left(2);
                                Modul1.Dateienopen();
                                goto end_IL_0001_2;
                            case 39:
                            case 48:
                            case 61:
                                goto end_IL_0001_2;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0001_dispatch = 1383;
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

    public void RechText_FormClosing(object eventSender, FormClosingEventArgs eventArgs)
    {
        //Discarded unreachable code: IL_00db, IL_0138
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
                CloseReason closeReason;
                switch (try0001_dispatch)
                {
                    default:
                        num = 1;
                        cancel = eventArgs.Cancel;
                        goto IL_000b;
                    case 423:
                        {
                            num2 = num;
                            switch ((num3 <= -2) ? 1 : num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_013b;
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
                            }
                            else
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                            }
                            goto IL_013b;
                        }
                    end_IL_0001:
                        break;
                    IL_000b:
                        num = 2;
                        closeReason = eventArgs.CloseReason;
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        if (View.CheckBox1.CheckState != 0)
                        {
                            goto end_IL_0001_2;
                        }
                        View.List4.Items.Clear();
                        View.List1.Items.Clear();
                        View.List2.Items.Clear();
                        View.List3.Items.Clear();
                        View.Text2.Text = "";
                        View.Bezeichnung4.Text = "";
                        View.Bezeichnung4.Tag = 0;
                        DataModul.WB_FrauTable.Close();
                        DataModul.WB.Close();
                        goto end_IL_0001_2;
                    IL_013b:
                        num4 = num2 + 1;
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 21:
                            case 22:
                                num = 22;
                                eventArgs.Cancel = cancel;
                                goto end_IL_0001_2;
                            case 13:
                            case 14:
                            case 15:
                            case 23:
                                goto end_IL_0001_2;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0001_dispatch = 423;
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
    public void List3_DoubleClick(object eventSender, EventArgs eventArgs)
    {
        if (View.List3.Text.Length > 135)
        {
            Modul1.FamInArb = View.List3.Text.Right(10).AsInt();
        }
        checked
        {
            Modul1.PersInArb = (int)Math.Round(Strings.Mid(View.List3.Text, 48, 10).AsDouble());
            if (View.List3.Text.Length < 100)
            {
                Modul1.PersInArb = (int)Math.Round(Strings.Mid(View.List3.Text, 48, 10).AsDouble());
                FamPerSchalt = 1;
            }
            if (View.CheckBox1.CheckState == CheckState.Unchecked)
            {
                View.Close();
            }
            else
            {
                View.Hide();
            }
        }
    }

    public void Liste1_DoubleClick(object eventSender, EventArgs eventArgs)
    {
        if (View.Bezeichnung4.Tag.AsInt() > 0)
        {
            View._Bef_1.Enabled = true;
        }
        View._Bef_3.Enabled = true;
        ProjectData.ClearProjectError();
        View.Text1.Text = View.Liste1.Text.Left(240);
        View.Bezeichnung1.Text = Modul1.IText[EUserText.t197] + Strings.Mid(View.Liste1.Text, 241, View.Liste1.Text.Length);
        Modul1.Ubg = checked((int)Math.Round(Strings.Mid(View.Liste1.Text, 240, View.Liste1.Text.Length).AsDouble()));
    }

    public void Zeigfam(int persInArb)
    {
        //Discarded unreachable code: IL_0c6c
        int try0001_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        string text = default;
        float num5 = default;
        string text2 = default;
        int num7 = default;
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
                    switch (try0001_dispatch)
                    {
                        default:
                            num = 1;
                            goto IL_000a;
                        case 4131:
                            {
                                num2 = num;
                                switch ((num3 <= -2) ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_0d91;
                                    default:
                                        goto end_IL_0001;
                                }

                                int number = Information.Err().Number;
                                if (number == 3021)
                                {
                                    _ = View.List3.Items.Add("------------ Ende der Liste-----------");
                                    goto end_IL_0001_2;
                                }
                                else
                                {
                                    if (number == 94)
                                    {
                                        DataModul.DSB_SearchTable.Edit();
                                        DataModul.DSB_SearchTable.Fields["iKenn"].Value = "9";
                                        DataModul.DSB_SearchTable.Update();
                                        ProjectData.ClearProjectError();
                                        if (num2 == 0)
                                        {
                                            throw ProjectData.CreateProjectError(-2146828268);
                                        }
                                        num4 = num2;
                                        goto IL_0d95;
                                    }
                                    else
                                    {
                                        if (number == 3421)
                                        {
                                            goto end_IL_0001_2;
                                        }
                                        if (number != 3167)
                                        {
                                            goto end_IL_0001_2;
                                        }
                                        ProjectData.ClearProjectError();
                                        if (num2 == 0)
                                        {
                                            throw ProjectData.CreateProjectError(-2146828268);
                                        }
                                        goto IL_0d91;
                                    }
                                }
                            }
                        end_IL_0001:
                            break;
                        IL_000a:
                            num = 2;
                            text2 = "";
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            Modul1.Person_ReadNames(persInArb, Modul1.Person);
                            if (Modul1.Person.Prefix.Trim() != "")
                            {
                                Modul1.Person.SetFullSurname(Modul1.Person.Prefix.Trim() + " " + Modul1.Person.SurName);
                            }
                            text = Strings.RTrim(Modul1.Person.FullSurName.Left(25) + "," + Modul1.Person.Givennames);
                            EEventArt eArt = EEventArt.eA_Birth;
                            if (eArt <= EEventArt.eA_Burial)
                            {
                                var Datu = DataModul.Event.GetDate(eArt, persInArb);
                                if (Datu != default)
                                {
                                    var Prefix = "";
                                    Prefix = eArt switch
                                    {
                                        EEventArt.eA_Birth => Modul1.sBirthMark,
                                        EEventArt.eA_Baptism => Modul1.sBaptismMark,
                                        EEventArt.eA_Death => Modul1.sDeathMark,
                                        EEventArt.eA_Burial => Modul1.sBurialMark,
                                        _ => "",
                                    };
                                    text2 = $"{Prefix} {Datu.Year}";
                                    break;
                                }
                                eArt++;
                            }
                            if (text2.Trim() == "")
                            {
                                text2 = "      ";
                            }
                            text2 = !DataModul.Link.ExistE(persInArb, ELinkKennz.lkChild) ? "K " + text2 : "  " + text2;
                            text = text + new string(' ', 40).Left(38) + text2 + "              " + persInArb.AsString().Right(10);
                            Modul1_Persex = DataModul.Person.GetSex(persInArb);
                            var aiFam = Modul1.Ehesuch(persInArb, Modul1_Persex);
                            if (aiFam.Count != 0)
                            {
                                ELinkKennz eLKenn = Modul1_Persex == "F" ? ELinkKennz.lkFather : ELinkKennz.lkMother;
                                foreach (var iFam in aiFam)
                                {
                                    Modul1_LiText = Modul1.Famzeig(iFam, eLKenn);
                                    _ = View.List3.Items.Add(new ListItem(text + Modul1_LiText + new string(' ', 10) + iFam.AsString().Right(10), iFam));
                                    Modul1_LiText = "";
                                }
                            }
                            else
                            {
                                _ = View.List3.Items.Add(new ListItem(text, persInArb));
                            }
                            goto end_IL_0001_2;

                        IL_0d91:
                            num4 = unchecked(num2 + 1);
                            goto IL_0d95;
                        IL_0d95:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;


                                case 99:
                                    num = 99;
                                    I = 1;
                                    Modul1.Person_ReadNames(persInArb, Modul1.Person);
                                    if (Modul1.Person.Prefix.Trim() != "")
                                    {
                                        Modul1.Person.SetFullSurname(Modul1.Person.Prefix.Trim() + " " + Modul1.Person.SurName);
                                    }
                                    text = Modul1.Person.FullSurName.TrimEnd() + "," + Modul1.Person.Givennames;
                                    Modul1_Persex = DataModul.Person.GetSex(persInArb);
                                    string Persex = Modul1_Persex;
                                    _ = Modul1.Ehesuch(personNr: persInArb, Persex: Persex);
                                    Modul1.eLKennz = Modul1_Persex == "F" ? ELinkKennz.lkFather : ELinkKennz.lkMother;
                                    if (Modul1.UbgT != "")
                                    {
                                        num5 = (float)(Modul1.UbgT.Length / 10.0);
                                        Z = 1f;
                                        break;
                                    }
                                    W = Strings.InStr(text, ",");
                                    if (W > 25f)
                                    {
                                        text = text.Left(25) + Strings.Mid(text, (int)Math.Round(W), text.Length);
                                    }
                                    _ = View.List1.Items.Add(text + new string(' ', 40).Left(40) + text2 + "           " + DataModul.DSB_SearchTable.Fields["Nummer"].AsString().Right(10) + Modul1_LiText);
                                    num7++;
                                    Modul1_LiText = "";
                                    goto case 124;

                                case 114:
                                    num = 114;
                                    while (Z <= num5)
                                    {
                                        int Fam = (int)Math.Round(Modul1.UbgT.Left(10).AsDouble());
                                        Modul1.UbgT = Strings.Mid(Modul1.UbgT, 11, Modul1.UbgT.Length);
                                        W = Strings.InStr(text, ",");
                                        if (W > 25f)
                                        {
                                            text = text.Left(25) + Strings.Mid(text, (int)Math.Round(W), text.Length);
                                        }
                                        _ = View.List1.Items.Add(new ListItem(text + new string(' ', 40).Left(40) + text2 + "              " + DataModul.DSB_SearchTable.Fields["Nummer"].AsString().Right(10) + Modul1_LiText, Fam));
                                        num7++;
                                        Modul1_LiText = "";
                                        Z += 1f;
                                    }
                                    goto case 124;


                                case 124:
                                case 133:
                                case 134:
                                    num = 134;
                                    lErl = 2;
                                    DataModul.DSB_SearchTable.MoveNext();
                                    if (num7 < Modul1.Aus[13].AsInt())
                                        goto case 137;
                                    else
                                    {
                                        goto end_IL_0001_2;
                                    }
                                case 67:
                                case 68:
                                case 74:
                                case 98:
                                case 137:
                                case 140:
                                case 142:
                                case 146:
                                case 152:
                                case 153:
                                case 155:
                                case 158:
                                case 159:
                                case 160:
                                    goto end_IL_0001_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 4131;
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

    public void Label5_Click(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_0a44
        int try0001_dispatch = -1;
        int num = default;
        string text2 = default;
        int num2 = default;
        int num3 = default;
        string text = default;
        ETextKennz tKennz = default;
        string Fld = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                int num4;
                string B;
                switch (try0001_dispatch)
                {
                    default:
                        num = 1;
                        text2 = "";
                        goto IL_000a;
                    case 3315:
                        {
                            num2 = num;
                            switch ((num3 <= -2) ? 1 : num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_0aad;
                                default:
                                    goto end_IL_0001;
                            }
                            Modul1.UbgT = "Texte Bez_klick";
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
                            goto IL_0ab1;
                        }
                    end_IL_0001:
                        break;
                    IL_000a:
                        num = 2;
                        tKennz = ETextKennz.tkNone;
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        View.Liste1.Visible = true;
                        View.List4.Visible = false;
                        View._Bef_3.Visible = true;
                        string name = ((Label)sender).Name;
                        if (name == nameof(View.Label5))
                        {
                            tKennz = ETextKennz.tkName;
                            text2 = Modul1.IText[EUserText.t180] + new string(' ', 100) + "N";
                        }
                        else if (name == nameof(View.Label6))
                        {
                            tKennz = ETextKennz.F_;
                            text2 = Modul1.IText[EUserText.t181] + new string(' ', 100) + "F";
                        }
                        else if (name == nameof(View.Label7))
                        {
                            tKennz = ETextKennz.V_;
                            text2 = Modul1.IText[EUserText.t182] + new string(' ', 100) + "V";
                        }
                        else if (name == nameof(View.Label8))
                        {
                            tKennz = ETextKennz.A_;
                            text2 = Modul1.IText[EUserText.t196] + new string(' ', 100) + "A";
                        }
                        else if (name == nameof(View.Label9))
                        {
                            tKennz = ETextKennz.B_;
                            text2 = Modul1.IText[EUserText.t195] + new string(' ', 100) + "B";
                        }
                        else if (name == nameof(View.Label10))
                        {
                            tKennz = ETextKennz.C_;
                            text2 = Modul1.IText[EUserText.tAlias] + new string(' ', 100) + "C";
                        }
                        else if (name == nameof(View.Label11))
                        {
                            tKennz = ETextKennz.D_;
                            text2 = Modul1.IText[EUserText.t126] + new string(' ', 100) + "D";
                        }
                        else if (name == nameof(View.Label12))
                        {
                            tKennz = ETextKennz.E_;
                            text2 = Modul1.IText[EUserText.tTitle] + new string(' ', 100) + "E";
                        }
                        else if (name == nameof(View.Label13))
                        {
                            tKennz = ETextKennz.G_;
                            text2 = Modul1.IText[EUserText.t70] + new string(' ', 100) + "G";
                        }
                        else if (name == nameof(View.Label14))
                        {
                            tKennz = ETextKennz.M_;
                            text2 = Modul1.IText[EUserText.t186] + new string(' ', 100) + "M";
                        }
                        else if (name == nameof(View.Label15))
                        {
                            tKennz = ETextKennz.T_;
                            text2 = "Ereignisbezeichnungen" + new string(' ', 100) + "T";
                        }
                        else if (name == nameof(View.Label16))
                        {
                            tKennz = ETextKennz.H_;
                            text2 = Modul1.IText[EUserText.t99_Places] + new string(' ', 100) + "H";
                        }
                        else if (name == nameof(View.Label17))
                        {
                            tKennz = ETextKennz.I_;
                            text2 = Modul1.IText[EUserText.t188] + new string(' ', 100) + "I";
                        }
                        else if (name == nameof(View.Label18))
                        {
                            tKennz = ETextKennz.J_;
                            text2 = Modul1.IText[EUserText.t189] + new string(' ', 100) + "J";
                        }
                        else if (name == nameof(View.Label19))
                        {
                            tKennz = ETextKennz.K_;
                            text2 = Modul1.IText[EUserText.t190] + new string(' ', 100) + "K";
                        }
                        else if (name == nameof(View.Label20))
                        {
                            tKennz = ETextKennz.L_;
                            text2 = Modul1.IText[EUserText.t191] + new string(' ', 100) + "L";
                        }
                        else if (name == nameof(View.Label21))
                        {
                            tKennz = ETextKennz.Q_;
                            text2 = Modul1.IText[EUserText.t75] + new string(' ', 100) + "Q";
                        }
                        else if (name == nameof(View.Label22))
                        {
                            tKennz = ETextKennz.O_;
                            text2 = Modul1.IText[EUserText.t194] + new string(' ', 100) + "O";
                        }
                        else if (name == nameof(View.Label23))
                        {
                            tKennz = ETextKennz.U_;
                            text2 = "Status" + new string(' ', 100) + "U";
                        }
                        else if (name == nameof(View.Label24))
                        {
                            tKennz = ETextKennz.tk7_;
                            text2 = Modul1.IText[EUserText.t75] + new string(' ', 100) + "7";
                        }
                        else if (name == nameof(View.Label25))
                        {
                            tKennz = ETextKennz.tk5_;
                            text2 = "Hausnummern" + new string(' ', 100) + "5";
                        }

                        View.Bezeichnung1.Text = "";
                        View.Text1.Text = "";
                        View.Liste1.Items.Clear();
                        View.Label2.Text = $"{(char)tKennz}";
                        View.Label2.Tag = tKennz;
                        DataModul.DB_TexteTable.Index = nameof(TexteIndex.KText);
                        DataModul.DB_TexteTable.Seek("=", tKennz);
                        if (View.Text2.Text != "")
                        {
                            Modul1.UbgT = View.Text2.Text.ToUpper();
                            Modul1.STextles("Rechtext.Liste1", tKennz, Modul1.UbgT, View.Liste1.Items);
                            goto end_IL_0001_2;
                        }
                        else
                        {
                            var B_ = ("", tKennz);
                            Modul1.KTextles("Rechtext.Liste1", tKennz, View.Liste1.Items, B_);
                            text = 0.AsString();
                        }
                        goto IL_09d0;
                    IL_08d0: // <========== 3
                        num = 122;
                        Modul1_LiText = Strings.Left(Strings.Replace(Strings.LTrim(DataModul.DB_TexteTable.Fields[TexteFields.Txt].AsString()), "ssss", "ß") + new string(' ', 240), 240) + Strings.LTrim(DataModul.DB_TexteTable.Fields[TexteFields.TxNr].AsString());
                        _ = View.Liste1.Items.Add(Modul1_LiText);
                        DataModul.DB_TexteTable.MoveNext();
                        text += ">";
                        if (text.Length == 30)
                        {
                            text = "";
                        }
                        View.Bezeichnung6.Text = text;
                        View.Bezeichnung6.Refresh();
                        goto IL_09d0;
                    IL_09d0: // <========== 3
                        num = 108;
                        if (!DataModul.DB_TexteTable.EOF && !DataModul.DB_TexteTable.NoMatch
                            && !(DataModul.DB_TexteTable.Fields[TexteFields.Kennz].AsEnum<ETextKennz>() != tKennz))
                        {
                            Fld = DataModul.DB_TexteTable.Fields[TexteFields.Txt].AsString();
                            Fld = Modul1.Umlaute(Fld);
                            if (View.Text2.Text.ToUpper() == ""
                                || Fld.ToUpper().Left(Modul1.UbgT.Length).CompareTo(Modul1.UbgT) <= 0)
                            {
                                goto IL_08d0;
                            }
                        }
                        goto IL_09ea;
                    IL_09ea: // <========== 5
                        num = 132;
                        text2 = View.Liste1.Items.Count.AsString() + " " + text2;
                        View.Bezeichnung6.Text = text2;
                        View.Bezeichnung6.Refresh();
                        goto end_IL_0001_2;
                    IL_0aad:
                        num4 = num2 + 1;
                        goto IL_0ab1;
                    IL_0ab1:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 120:
                            case 122:
                                goto IL_08d0;
                            case 107:
                            case 108:
                            case 131:
                                goto IL_09d0;
                            case 110:
                            case 113:
                            case 119:
                            case 132:
                                goto IL_09ea;
                            case 103:
                            case 135:
                            case 141:
                                goto end_IL_0001_2;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0001_dispatch = 3315;
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
    public void _Bef_3_Click(object sender, EventArgs e)
    {
    }
}
