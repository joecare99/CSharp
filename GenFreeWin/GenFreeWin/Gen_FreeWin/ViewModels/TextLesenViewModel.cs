using BaseLib.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gen_FreeWin.Views;
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
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Gen_FreeWin.ViewModels;

public partial class TextLesenViewModel : ObservableObject, ITextLesenViewModel
{
    Form ITextLesenViewModel.View { get; set; }
    [Obsolete]
    Textlesen View => (Textlesen)((ITextLesenViewModel)this).View;
    [ObservableProperty]
    public partial bool Frame1_Visible { get; set; }

    IModul1 Modul1 => _Modul1.Instance;
    IInteraction Interaction;
    IVBInformation Information => Modul1.Information;
    IProjectData ProjectData => Modul1.ProjectData;
    IVBConversions Conversions => Modul1.Conversions;
    IVBConversions Conversion => Modul1.Conversions;
    IStrings Strings => Modul1.Strings;

    IList List1_Items => View.List1.Items;
    IList List4_Items => View.List4.Items;
    IList Liste1_Items => View.Liste1.Items;
    IList List2_Items => View.List2.Items;

    [ObservableProperty]
    public partial string Text1_Text { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string Text2_Text { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string Text3_Text { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Text4_Text { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool Check1_Checked { get; set; } = false;
    [ObservableProperty]
    public partial bool Check3_Checked { get; set; } = false;


    [ObservableProperty]
    public partial bool ChangeSex_Visibility { get; set; } = false;

    private int Nr;

    private int AltPer;

    private int iNeuOrt;

    private int iAltOrt;

    private int A1;

    private float Abbr;

    private short Value;

    private ETextKennz Txknz;

    public (string sText, ETextKennz eTKnz) tTextBez
    {
        get => (View.Bezeichnung6.Text, View.Bezeichnung6.Tag.AsEnum<ETextKennz>());
        set => View.Bezeichnung6.Text = $"{Liste1_Items.Count} {value.sText,100} {(char)(View.Bezeichnung6.Tag = value.eTKnz)}";
    }

    private (string, ETextKennz) M_Bezeichnu;
    private bool Modul1_Glob = true;

    /// <summary>
    /// Befs the click.
    /// </summary>
    /// <iTxtNr name="eventSender">The event sender.</iTxtNr>
    /// <iTxtNr name="eventArgs">The <see cref="EventArgs" /> instance containing the event data.</iTxtNr>
    public void Bef_Click(object eventSender, EventArgs eventArgs)
    {
        //Discarded unreachable code: IL_4d28, IL_6713, IL_6e34, IL_7572, IL_7598
        int try0001_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        string text = default;
        string prompt = default;
        string str = default;
        string text2 = default;
        int num10 = default;
        int num12 = default;
        int num18 = default;
        int num19 = default;
        int num23 = default;
        int num25 = default;
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
                    int Ortnr;
                    short Schalt;
                    int num20;
                    int num24;
                    int num26;
                    int num14;
                    string B;
                    int num15;
                    ETextKennz kennz;
                    switch (try0001_dispatch)
                    {
                        default:
                            num = 1;
                            goto IL_0017;
                        case 35221:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 3:
                                        goto IL_6715;
                                    case 1:
                                        goto IL_76cf;
                                    default:
                                        goto end_IL_0001;
                                }
                                int number = Information.Err().Number;
                                if (number == 25)
                                {
                                    Value = (short)Interaction.MsgBox("Bitte den Drucker bereit machen.", title: "", mb: MessageBoxButtons.OKCancel);
                                    if (Value == 2)
                                    {
                                        num15 = 0;
                                        goto end_IL_0001_2;
                                    }
                                    else
                                    {
                                        ProjectData.ClearProjectError();
                                        if (num2 == 0)
                                        {
                                            throw ProjectData.CreateProjectError(-2146828268);
                                        }
                                    }
                                    goto IL_76cb;
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
                                        goto IL_76cf;
                                    }
                                    else
                                    {
                                        if (number == 63)
                                        {
                                            num15 = 0;
                                            goto end_IL_0001_2;
                                        }
                                        else
                                        {
                                            if (number == 3022)
                                            {
                                                kennz = Modul1.eTKennz;
                                                switch (kennz)
                                                {
                                                    case ETextKennz.A_:
                                                    case ETextKennz.B_:
                                                    case ETextKennz.C_:
                                                    case ETextKennz.D_:
                                                    case ETextKennz.P_:
                                                    case ETextKennz.tkName:
                                                    case ETextKennz.V_:
                                                    case ETextKennz.F_:
                                                    case ETextKennz.U_:
                                                        Handle_Person_TextErr(str, text2);
                                                        ProjectData.ClearProjectError();
                                                        if (num2 == 0)
                                                        {
                                                            throw ProjectData.CreateProjectError(-2146828268);
                                                        }
                                                        num2 = 0;
                                                        goto IL_0017;
                                                    default:
                                                        break;
                                                }
                                                switch (kennz)
                                                {
                                                    case ETextKennz.H_:
                                                    case ETextKennz.I_:
                                                    case ETextKennz.J_:
                                                    case ETextKennz.K_:
                                                    case ETextKennz.L_:
                                                        if (Modul1.eTKennz == ETextKennz.H_)
                                                        {
                                                            //Ind = ETextKennz.O_;
                                                        }
                                                        else if (Modul1.eTKennz == ETextKennz.I_)
                                                        {
                                                            //Ind = "OT";
                                                        }
                                                        else if (Modul1.eTKennz == ETextKennz.J_)
                                                        {
                                                            //Ind = ETextKennz.K_;
                                                        }
                                                        else if (Modul1.eTKennz == ETextKennz.K_)
                                                        {
                                                            //Ind = ETextKennz.L_;
                                                        }
                                                        else if (Modul1.eTKennz == ETextKennz.L_)
                                                        {
                                                            //Ind = ETextKennz.S_;
                                                        }
                                                        DataModul.DB_TexteTable.Delete();
                                                        DataModul.DB_TexteTable.Index = nameof(TexteIndex.ITexte);
                                                        DataModul.DB_TexteTable.Seek("=", str.Trim(), (char)Modul1.eTKennz);
                                                        num10 = DataModul.DB_TexteTable.Fields[TexteFields.TxNr].AsInt();
                                                        ProjectData.ClearProjectError();
                                                        if (num2 == 0)
                                                        {
                                                            throw ProjectData.CreateProjectError(-2146828268);
                                                        }
                                                        goto IL_76cf;
                                                    case ETextKennz.M_:
                                                    case ETextKennz.G_:
                                                    case ETextKennz.E_:
                                                    case ETextKennz.Q_:
                                                    case ETextKennz.W_:
                                                        DataModul.DB_TexteTable.Index = nameof(TexteIndex.ITexte);
                                                        DataModul.DB_TexteTable.Seek("=", str.Trim(), Modul1.eTKennz);
                                                        Nr = View._Bezeichnung1_0.Tag.AsInt();
                                                        DataModul.Event.UpdateAllSetVal(EventIndex.KText, EventFields.KBem, Nr, DataModul.DB_TexteTable.Fields[TexteFields.TxNr].AsInt());
                                                        DataModul.DB_TexteTable.Seek("=", text2, Modul1.eTKennz);
                                                        if (!DataModul.DB_TexteTable.NoMatch)
                                                        {
                                                            DataModul.DB_TexteTable.Delete();
                                                        }
                                                        ProjectData.ClearProjectError();
                                                        if (num2 == 0)
                                                        {
                                                            throw ProjectData.CreateProjectError(-2146828268);
                                                        }
                                                        goto IL_76cf;
                                                    default:
                                                        break;
                                                }
                                                if (kennz == ETextKennz.tk3_)
                                                {
                                                    DataModul.DB_TexteTable.Index = nameof(TexteIndex.ITexte);
                                                    DataModul.DB_TexteTable.Seek("=", str.Trim(), Modul1.eTKennz);
                                                    Nr = View._Bezeichnung1_0.Tag.AsInt();
                                                    DataModul.Event.UpdateAllSetVal(EventIndex.CText, EventFields.Causal, Nr, DataModul.DB_TexteTable.Fields[TexteFields.TxNr].AsInt());
                                                    DataModul.DB_TexteTable.Seek("=", text2, Modul1.eTKennz);
                                                    if (!DataModul.DB_TexteTable.NoMatch)
                                                    {
                                                        DataModul.DB_TexteTable.Delete();
                                                    }
                                                    ProjectData.ClearProjectError();
                                                    if (num2 == 0)
                                                    {
                                                        throw ProjectData.CreateProjectError(-2146828268);
                                                    }
                                                    goto IL_76cf;
                                                }
                                                else if (kennz == ETextKennz.O_)
                                                {
                                                    DataModul.DB_TexteTable.Index = nameof(TexteIndex.ITexte);
                                                    DataModul.DB_TexteTable.Seek("=", str.Trim(), Modul1.eTKennz);
                                                    Nr = View._Bezeichnung1_0.Tag.AsInt();
                                                    DataModul.Event.UpdateAllSetVal(EventIndex.PText, EventFields.Platz, Nr, DataModul.DB_TexteTable.Fields[TexteFields.TxNr].AsInt());
                                                    DataModul.DB_TexteTable.Seek("=", text2, Modul1.eTKennz);
                                                    if (!DataModul.DB_TexteTable.NoMatch)
                                                    {
                                                        DataModul.DB_TexteTable.Delete();
                                                    }
                                                    ProjectData.ClearProjectError();
                                                    if (num2 == 0)
                                                    {
                                                        throw ProjectData.CreateProjectError(-2146828268);
                                                    }
                                                    goto IL_76cf;
                                                }
                                                else if (kennz == ETextKennz.T_)
                                                {
                                                    DataModul.DB_TexteTable.Index = nameof(TexteIndex.ITexte);
                                                    DataModul.DB_TexteTable.Seek("=", str.Trim(), Modul1.eTKennz);
                                                    Nr = View._Bezeichnung1_0.Tag.AsInt();
                                                    DataModul.Event.UpdateAllSetVal(EventIndex.NText, EventFields.ArtText, Nr, DataModul.DB_TexteTable.Fields[TexteFields.TxNr].AsInt());
                                                    DataModul.DB_TexteTable.Seek("=", text2, Modul1.eTKennz);
                                                    if (!DataModul.DB_TexteTable.NoMatch)
                                                    {
                                                        DataModul.DB_TexteTable.Delete();
                                                    }
                                                    ProjectData.ClearProjectError();
                                                    if (num2 == 0)
                                                    {
                                                        throw ProjectData.CreateProjectError(-2146828268);
                                                    }
                                                    goto IL_76cf;
                                                }
                                                else if (kennz == ETextKennz.tk7_)
                                                {
                                                    DataModul.DB_TexteTable.Index = nameof(TexteIndex.ITexte);
                                                    DataModul.DB_TexteTable.Seek("=", str.Trim(), Modul1.eTKennz);
                                                    num10 = DataModul.DB_TexteTable.Fields[TexteFields.TxNr].AsInt();
                                                    DataModul.DB_PersonTable.Index = nameof(PersonIndex.reli);
                                                    Nr = View._Bezeichnung1_0.Tag.AsInt();
                                                    DataModul.DB_PersonTable.Seek("=", Nr);
                                                    while (!DataModul.DB_PersonTable.EOF
                                    && !DataModul.DB_PersonTable.NoMatch
                                    && !(DataModul.DB_PersonTable.Fields[PersonFields.religi].AsInt() != Nr))
                                                    {
                                                        DataModul.DB_PersonTable.Edit();
                                                        DataModul.DB_PersonTable.Fields[PersonFields.Konv].Value = str.Trim();
                                                        DataModul.DB_PersonTable.Fields[PersonFields.religi].Value = num10;
                                                        DataModul.DB_PersonTable.Update();
                                                        DataModul.DB_PersonTable.MoveNext();
                                                    }
                                                    goto IL_65db;
                                                }
                                                else
                                                {
                                                    _ = Interaction.MsgBox("Stop", title: "11", mb: MessageBoxButtons.OK);
                                                }
                                                goto IL_65db;
                                            }
                                            else
                                            {
                                                if (number == 3021)
                                                {
                                                    ProjectData.ClearProjectError();
                                                    if (num2 == 0)
                                                    {
                                                        throw ProjectData.CreateProjectError(-2146828268);
                                                    }
                                                    goto IL_76cf;
                                                }
                                                else
                                                {
                                                    if (number == 3027)
                                                    {
                                                        _ = Interaction.MsgBox(Modul1.Message_sNoChangesOnCD, title: "", icon: MessageBoxIcon.Information);
                                                        goto end_IL_0001_2;
                                                    }
                                                    else
                                                    {
                                                        if (number != 3167)
                                                        {
                                                            if (number == 3376)
                                                            {
                                                                ProjectData.ClearProjectError();
                                                                if (num2 == 0)
                                                                {
                                                                    throw ProjectData.CreateProjectError(-2146828268);
                                                                }
                                                                goto IL_76cf;
                                                            }
                                                            else
                                                            {
                                                                Modul1.UbgT = "Texte Bef_Click";
                                                                _ = Interaction.MsgBox(Conversion.ErrorToString() + Information.Err().Number.AsString());
                                                                _ = Interaction.MsgBox("F119");
                                                                Debugger.Break();
                                                                ProjectData.ClearProjectError();
                                                                if (num2 == 0)
                                                                {
                                                                    throw ProjectData.CreateProjectError(-2146828268);
                                                                }
                                                            }
                                                            goto IL_76cb;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                goto IL_65db;
                            }
                        end_IL_0001:
                            break;
                        IL_0017:
                            ProjectData.ClearProjectError();
                            num15 = View.Bef.GetIndex((Button)eventSender);
                            num3 = 2;
                            View.Command3.Visible = false;
                            switch (num15)
                            {
                                case 0:
                                    Bef0_Click();
                                    break;
                                case 1:
                                    Bef1_Click(eventSender, eventArgs);
                                    break;

                                case 2:
                                    Bef2_Click(eventSender, eventArgs);
                                    break;
                                case 3:
                                    Bef3_Click(eventSender, eventArgs);
                                    break;
                                case 4:
                                    Bez4_Click();
                                    break;
                                default:
                                    break;
                            }
                            goto end_IL_0001_2;
                            goto end_IL_0001_2;

                        IL_65db: // <========== 4
                            num = 1015;
                            View._Bezeichnung1_0.Text = "";
                            View._Bezeichnung1_0.Tag = 0;
                            View.Bezeichnung2.Text = "";
                            View.Bezeichnung2.Tag = false;
                            str = "";
                            Liste1_Items.Clear();
                            Liste1_Items.Clear();
                            Txknz = tTextBez.eTKnz;
                            if (View.Text2.Text != "")
                            {
                                Modul1.UbgT = View.Text2.Text.ToUpper();
                                Modul1.STextles("TL5", Txknz, Modul1.UbgT, Liste1_Items);
                                if (List1_Items.Count > 0)
                                    tTextBez = M_Bezeichnu;

                            }
                            else
                            {
                                B = "";
                                Modul1.KTextlesTL5(Txknz, Liste1_Items, M_Bezeichnu);
                            }
                            goto end_IL_0001_2;
                        IL_6715:
                            num = 1029;
                            if (Information.Err().Number == 3022)
                            {
                                var num5 = 0;
                                var num6 = 0;
                                int num7 = 0;
                                int num8 = 0;
                                int num9 = 0;
                                num5 = DataModul.DB_PlaceTable.Fields[PlaceFields.Ort].AsInt();
                                num6 = DataModul.DB_PlaceTable.Fields[PlaceFields.Ortsteil].AsInt();
                                num7 = DataModul.DB_PlaceTable.Fields[PlaceFields.Kreis].AsInt();
                                num8 = DataModul.DB_PlaceTable.Fields[PlaceFields.Land].AsInt();
                                num9 = DataModul.DB_PlaceTable.Fields[PlaceFields.Staat].AsInt();
                                iAltOrt = DataModul.DB_PlaceTable.Fields[PlaceFields.OrtNr].AsInt();
                                if (Strings.Trim(DataModul.DB_PlaceTable.Fields[PlaceFields.Bem].AsString()) != "")
                                {
                                    _ = Interaction.MsgBox("Stop", title: "12", mb: MessageBoxButtons.OK);
                                }
                                DataModul.DB_PlaceTable.Index = nameof(PlaceIndex.Orte);
                                DataModul.DB_PlaceTable.Seek("=", num5, num6, num7, num8, num9);
                                string title;
                                if (DataModul.DB_PlaceTable.NoMatch)
                                {
                                    title = 1.AsString();
                                }
                                else if (DataModul.DB_PlaceTable.Fields[PlaceFields.Ort].AsInt() != num5)
                                {
                                    title = 2.AsString();
                                }
                                else if (DataModul.DB_PlaceTable.Fields[PlaceFields.Ortsteil].AsInt() != num6)
                                {
                                    title = 3.AsString();
                                }
                                else if (DataModul.DB_PlaceTable.Fields[PlaceFields.Kreis].AsInt() != num7)
                                {
                                    title = 4.AsString();
                                }
                                else if (DataModul.DB_PlaceTable.Fields[PlaceFields.Land].AsInt() != num8)
                                {
                                    title = 5.AsString();
                                }
                                else if (DataModul.DB_PlaceTable.Fields[PlaceFields.Staat].AsInt() != num9)
                                {
                                    title = 6.AsString();
                                }
                                else
                                {
                                    iNeuOrt = DataModul.DB_PlaceTable.Fields[PlaceFields.OrtNr].AsInt();
                                    DataModul.Event.UpdateAllSetVal(EventIndex.EOrt, EventFields.Ort, iAltOrt, iNeuOrt);
                                    if (DataModul.Place.Delete(iAltOrt))
                                    {
                                        DataModul.DOSB_OrtSTable.Index = "OrtNr";
                                        DataModul.DOSB_OrtSTable.Seek("=", iAltOrt);
                                        if (!DataModul.DOSB_OrtSTable.NoMatch)
                                        {
                                            DataModul.DOSB_OrtSTable.Delete();
                                        }
                                    }
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    num2 = 0;
                                    lErl = 23;
                                    (PlaceIndex eIndex, PlaceFields eIndexField) = Modul1.eTKennz switch
                                    {
                                        ETextKennz.H_ => (PlaceIndex.O, PlaceFields.Ort),
                                        ETextKennz.I_ => (PlaceIndex.OT, PlaceFields.Ortsteil),
                                        ETextKennz.J_ => (PlaceIndex.K, PlaceFields.Kreis),
                                        ETextKennz.K_ => (PlaceIndex.L, PlaceFields.Land),
                                        ETextKennz.L_ => (PlaceIndex.L, PlaceFields.Staat),
                                        _ => (default, default),
                                    };
                                    Place_UpdateAllSetValAct(eIndex, eIndexField, Nr, num10, (cPl) =>
                                    {
                                        _ = Liste1_Items.Add(new ListItem(cPl.ID.AsString(), cPl.ID));
                                        return true;
                                    });
                                    Indber();
                                    View._Bezeichnung1_0.Text = "";
                                    View._Bezeichnung1_0.Tag = 0;
                                    View.Bezeichnung2.Text = "";
                                    View.Bezeichnung2.Tag = false;
                                    str = "";
                                    Liste1_Items.Clear();
                                    Txknz = View.Bezeichnung6.Tag.AsEnum<ETextKennz>();
                                    if (View.Text2.Text != "")
                                    {
                                        Modul1.UbgT = View.Text2.Text.ToUpper();
                                        Modul1.STextles("TL5", Txknz, Modul1.UbgT, ocItems: MainProject.Forms.Ereignis.ListBox2.Items);
                                        if (List1_Items.Count > 0)
                                            tTextBez = M_Bezeichnu;

                                    }
                                    else
                                    {
                                        B = "";
                                        Modul1.KTextlesTL5(Txknz, Liste1_Items, M_Bezeichnu);
                                    }
                                    lErl = 46;
                                    goto end_IL_0001_2;
                                }
                                _ = Interaction.MsgBox("Ortfehler1", title: title, mb: MessageBoxButtons.OK);
                                DataModul.DB_PlaceTable.Index = nameof(PlaceIndex.Orte);
                                DataModul.DB_PlaceTable.Seek("=", num5, num6, num7, num8, num9);
                                _ = Interaction.MsgBox(DataModul.DB_PlaceTable.Fields[PlaceFields.OrtNr].AsString());
                                iAltOrt = DataModul.DB_PlaceTable.Fields[PlaceFields.OrtNr].AsInt();
                                _ = Interaction.MsgBox(prompt);
                                _ = Interaction.MsgBox("");
                                ProjectData.EndApp();
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                            }
                            else
                            {
                                if (Information.Err().Number == 3167)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_76cf;
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
                                }
                            }
                            goto IL_76cb;
                        IL_6e36: // <========== 5
                                 // <========== 5
                                 // <========== 5
                            num = 1109;
                            View.ListBox1.Items.Clear();
                            View.Frame1.Text = "Verwendungsliste " + text;
                            ETextKennz kennz2 = Modul1.eTKennz;
                            switch (kennz2)
                            {
                                case ETextKennz.I_:
                                    DataModul.DB_PlaceTable.Index = nameof(PlaceIndex.OT);
                                    DataModul.DB_PlaceTable.Seek("=", Modul1.Ubg);
                                    while (!DataModul.DB_PlaceTable.EOF && !DataModul.DB_PlaceTable.NoMatch && !(DataModul.DB_PlaceTable.Fields[PlaceFields.Ortsteil].AsInt() != Modul1.Ubg))
                                    {
                                        _ = View.ListBox1.Items.Add(DataModul.DB_PlaceTable.Fields[PlaceFields.OrtNr].AsInt().AsString());
                                        DataModul.DB_PlaceTable.MoveNext();
                                    }
                                    break;
                                case ETextKennz.J_:
                                    DataModul.DB_PlaceTable.Index = nameof(PlaceIndex.K);
                                    DataModul.DB_PlaceTable.Seek("=", Modul1.Ubg);
                                    while (!DataModul.DB_PlaceTable.EOF && !DataModul.DB_PlaceTable.NoMatch && !(DataModul.DB_PlaceTable.Fields[PlaceFields.Kreis].AsInt() != Modul1.Ubg))
                                    {
                                        _ = View.ListBox1.Items.Add(DataModul.DB_PlaceTable.Fields[PlaceFields.OrtNr].AsInt().AsString());
                                        DataModul.DB_PlaceTable.MoveNext();
                                    }
                                    break;
                                case ETextKennz.K_:
                                    DataModul.DB_PlaceTable.Index = nameof(PlaceIndex.L);
                                    DataModul.DB_PlaceTable.Seek("=", Modul1.Ubg);
                                    while (!DataModul.DB_PlaceTable.EOF && !(DataModul.DB_PlaceTable.Fields[PlaceFields.Land].AsInt() != Modul1.Ubg))
                                    {
                                        _ = View.ListBox1.Items.Add(DataModul.DB_PlaceTable.Fields[PlaceFields.OrtNr].AsInt().AsString());
                                        DataModul.DB_PlaceTable.MoveNext();
                                    }
                                    break;
                                case ETextKennz.L_:
                                    DataModul.DB_PlaceTable.Index = nameof(PlaceIndex.S);
                                    DataModul.DB_PlaceTable.Seek("=", Modul1.Ubg);
                                    while (!DataModul.DB_PlaceTable.EOF && !(DataModul.DB_PlaceTable.Fields[PlaceFields.Staat].AsInt() != Modul1.Ubg))
                                    {
                                        _ = View.ListBox1.Items.Add(DataModul.DB_PlaceTable.Fields[PlaceFields.OrtNr].AsInt().AsString());
                                        DataModul.DB_PlaceTable.MoveNext();
                                    }
                                    break;
                                default:
                                    break;
                            }
                            List1_Items.Clear();
                            int num16 = View.ListBox1.Items.Count - 1;
                            num12 = 0;
                            while (num12 <= num16)
                            {
                                Ortnr = View.ListBox1.Items[num12].ItemData<int>();
                                if (DataModul.Place.ReadData(Ortnr, out var cPlace))
                                    Modul1.UbgT = DataModul.Place.FullName(cPlace);
                                _ = List1_Items.Add(new ListItem(Modul1.UbgT, Ortnr));
                                num12++;
                            }
                            View.Label4.Text = "Einträge" + List1_Items.Count.AsString();
                            View.btnShowAsocPeople.Visible = true;
                            View.ListBox1.Items.Clear();
                            goto end_IL_0001_2;

                        IL_76cb: // <========== 3
                            num4 = num2;
                            goto IL_76d3;
                        IL_76cf: // <========== 10
                            num4 = unchecked(num2 + 1);
                            goto IL_76d3;
                        IL_76d3:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 792:
                                case 799:
                                case 802:
                                case 803:
                                case 804:
                                case 807:
                                case 808:
                                case 812:
                                case 815:
                                case 842:
                                case 864:
                                case 865:
                                case 889:
                                case 890:
                                case 914:
                                case 915:
                                case 939:
                                case 940:
                                case 964:
                                case 965:
                                case 977:
                                case 988:
                                case 991:
                                case 992:
                                case 995:
                                case 996:
                                case 1000:
                                case 1002:
                                case 1005:
                                case 1006:
                                case 1013:
                                case 1014:
                                case 1015:
                                    goto IL_65db;
                                case 433:
                                case 437:
                                case 441:
                                case 445:
                                case 1108:
                                case 1109:
                                    goto IL_6e36;
                                case 1186:
                                    num = 1186;
                                    _ = Interaction.MsgBox(Information.Err().Number.AsString());
                                    goto end_IL_0001_2;
                                case 6:
                                case 101:
                                case 102:
                                case 126:
                                case 310:
                                case 311:
                                case 312:
                                case 316:
                                case 320:
                                case 331:
                                case 337:
                                case 782:
                                case 786:
                                case 787:
                                case 788:
                                case 789:
                                case 790:
                                case 798:
                                case 811:
                                case 999:
                                case 1024:
                                case 1027:
                                case 1028:
                                case 1185:
                                case 1187:
                                case 1197:
                                    goto end_IL_0001_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 35221;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001_2: // <========== 15
                       // <========== 17
                       // <========== 17
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    private int Bez4_Click()
    {
        checked
        {
            int num;
            num = 339;
            string sLeitname2 = View.Text3.Text.Replace("ß", "ssss");

            View.btnChangeSexToF.Visible = false;
            View.btnChangeSexToM.Visible = false;
            View.btnShowAsocPeople.Visible = false;
            View.RichTextBox1.Visible = false;

            View.Frame1.Top = View._Label1_2.Top + View._Label1_2.Height;
            View.Frame1.Left = 0;
            View.Frame1.Height = View.Height - (View._Label1_2.Top + View._Label1_2.Height);
            View.Frame1.Width = View.Width;
            Frame1_Visible = true;

            View.List1.Visible = true;
            View.Liste1.Visible = true;
            View.Frame1.BackColor = View.BackColor;

            Modul1.Ubg = View.Label5.Tag.AsInt();
            Nr = Modul1.Ubg;
            AltPer = 0;
            if (!View._Check2_1.Visible)
            {
                View._Check2_1.CheckState = CheckState.Unchecked;
            }
            View._Command1_0.Text = Modul1.IText[EUserText.tNMBack];
            Nr = View._Bezeichnung1_0.Tag.AsInt();
            Modul1.eTKennz = tTextBez.eTKnz;
            List1_Items.Clear();
            ETextKennz kennz3;
            if (Nr > 0 | Nr == 0 & Modul1.eTKennz == ETextKennz.Z_)
            {
                string str = "";
                string text = Modul1.eTKennz switch
                {
                    ETextKennz.tkName when View._Check2_1.Checked => Modul1.IText[EUserText.tName] + " Leitname >" + sLeitname2.Trim() + "<",
                    ETextKennz.tkName => Modul1.IText[EUserText.tName] + " >" + str.Trim() + "<",
                    ETextKennz.A_ => "Namenspräfix >" + str.Trim() + "<",
                    ETextKennz.B_ when View._Check2_1.Checked => Modul1.IText[EUserText.t184] + " Leitname >" + sLeitname2.Trim() + "<",
                    ETextKennz.B_ => "Namenssuffix >" + str.Trim() + "<",
                    ETextKennz.C_ => Modul1.IText[EUserText.tAlias] + " >" + str.Trim() + "<",
                    ETextKennz.D_ => Modul1.IText[EUserText.t126] + " >" + str.Trim() + "<",
                    ETextKennz.V_ when View._Check2_1.Checked => Modul1.IText[EUserText.t182] + " Leitname >" + sLeitname2.Trim() + "<",
                    ETextKennz.V_ => Modul1.IText[EUserText.t182] + " >" + str.Trim() + "<",
                    ETextKennz.E_ when View._Check2_1.Checked => Modul1.IText[EUserText.tOccupation] + " Leitname >" + sLeitname2.Trim() + "<",
                    ETextKennz.E_ => Modul1.IText[EUserText.tOccupation] + " >" + str.Trim() + "<",
                    ETextKennz.G_ => Modul1.IText[EUserText.t70] + " >" + str.Trim() + "<",
                    ETextKennz.M_ => Modul1.IText[EUserText.t186] + " >" + str.Trim() + "<",
                    ETextKennz.Q_ => Modul1.IText[EUserText.t75] + " >" + str.Trim() + "<",
                    ETextKennz.tk7_ => "Konfession >" + str.Trim() + "<",
                    ETextKennz.tk2_ => "Prädikat >" + str.Trim() + "<",
                    ETextKennz.O_ => Modul1.IText[EUserText.t194] + " >" + str.Trim() + "<",
                    ETextKennz.U_ => "Status  >" + str.Trim() + "<",
                    ETextKennz.T_ => "Ereignisname  >" + str.Trim() + "<",
                    ETextKennz.Z_ => "Religionen  >" + str.Trim() + "<",
                    ETextKennz.H_ => "Ort >" + str.Trim() + "<",
                    ETextKennz.I_ => "Ortsteil >" + str.Trim() + "<",
                    ETextKennz.J_ => "Kreise >" + str.Trim() + "<",
                    ETextKennz.K_ => "Länder >" + str.Trim() + "<",
                    ETextKennz.L_ => "Staaten >" + str.Trim() + "<",
                    ETextKennz.tk5_ => "Hausnummer >" + str.Trim() + "<",
                    _ => ""
                };
                View.Frame1.Text = "Verwendungsliste " + text;
                kennz3 = Modul1.eTKennz;
                int nr = Nr;
                int num12;
                int num18 = 0;
                switch (kennz3)
                {
                    case ETextKennz.A_:
                    case ETextKennz.B_:
                    case ETextKennz.C_:
                    case ETextKennz.D_:
                    case ETextKennz.F_:
                    case ETextKennz.tkName:
                    case ETextKennz.V_:
                    case ETextKennz.U_:
                    case ETextKennz.tk2_:
                        if (View._Check2_0.Checked)
                        {
                            DataModul.DB_NameTable.Index = nameof(NameIndex.PNamen);
                            DataModul.DB_NameTable.MoveFirst();
                            while (!DataModul.DB_NameTable.EOF
                                && !DataModul.DB_NameTable.NoMatch
                                && DataModul.DB_NameTable.Fields[NameFields.Kennz].AsEnum<ETextKennz>() == Modul1.eTKennz
                                && DataModul.DB_NameTable.Fields[NameFields.Text].AsInt() == nr)
                            {
                                num18++;
                                _ = View.Sortbox1.Items.Add(Strings.Right("                 " + DataModul.DB_NameTable.Fields[NameFields.PersNr].AsInt().AsString(), 10));
                                //alttnr = DataModul.DB_NameTable.Fields[NameFields.PersNr].AsInt();
                                DataModul.DB_NameTable.MoveNext();
                            }
                        }
                        else
                        {
                            if (View._Check2_1.Checked)
                            {
                                List2_Items.Clear();
                                DataModul.DB_TexteTable.Index = nameof(TexteIndex.ITexte);
                                DataModul.DB_TexteTable.Seek("=", sLeitname2.Trim(), Modul1.eTKennz);
                                if (!DataModul.DB_TexteTable.NoMatch)
                                {
                                    while (!DataModul.DB_TexteTable.EOF && !(DataModul.DB_TexteTable.Fields[TexteFields.Kennz].AsEnum<ETextKennz>() != Modul1.eTKennz) && !(DataModul.DB_TexteTable.Fields[TexteFields.Txt].AsString() != sLeitname2.Trim()))
                                    {
                                        _ = List2_Items.Add(DataModul.DB_TexteTable.Fields[TexteFields.TxNr].AsInt());
                                        DataModul.DB_TexteTable.MoveNext();
                                    }
                                }
                                DataModul.DB_TexteTable.Index = nameof(TexteIndex.LTexte);
                                DataModul.DB_TexteTable.Seek("=", sLeitname2, Modul1.eTKennz);
                                while (!DataModul.DB_TexteTable.EOF && !DataModul.DB_TexteTable.NoMatch && !(DataModul.DB_TexteTable.Fields[TexteFields.Kennz].AsEnum<ETextKennz>() != Modul1.eTKennz) && !(DataModul.DB_TexteTable.Fields[TexteFields.Kennz].AsEnum<ETextKennz>() != Modul1.eTKennz) && !(DataModul.DB_TexteTable.Fields[TexteFields.Leitname].AsString() != sLeitname2.Trim()))
                                {
                                    _ = List2_Items.Add(DataModul.DB_TexteTable.Fields[TexteFields.TxNr].AsInt());
                                    DataModul.DB_TexteTable.MoveNext();
                                }
                                int num19 = List2_Items.Count - 1;
                                num12 = 0;
                                while (num12 <= num19)
                                {
                                    nr = List2_Items[num12].AsInt();
                                    DataModul.DB_NameTable.Index = nameof(NameIndex.TxNr);
                                    DataModul.DB_NameTable.Seek("=", nr);
                                    while (!DataModul.DB_NameTable.EOF
                                        && !DataModul.DB_NameTable.NoMatch
                                        && DataModul.DB_NameTable.Fields[NameFields.Text].AsInt() == nr)
                                    {
                                        _ = View.Sortbox1.Items.Add(Strings.Right("               " + DataModul.DB_NameTable.Fields[NameFields.PersNr].AsInt().AsString(), 10));
                                        DataModul.DB_NameTable.MoveNext();
                                    }
                                    num12++;
                                }
                            }
                            else
                            {
                                DataModul.DB_NameTable.Index = nameof(NameIndex.TxNr);
                                DataModul.DB_NameTable.Seek("=", nr);
                                if (!DataModul.DB_NameTable.EOF
                                    && !DataModul.DB_NameTable.NoMatch
                                    && DataModul.DB_NameTable.Fields[NameFields.Text].AsInt() == nr)
                                {
                                    _ = View.Sortbox1.Items.Add(Strings.Right("               " + DataModul.DB_NameTable.Fields[NameFields.PersNr].AsInt().AsString(), 10));
                                    DataModul.DB_NameTable.MoveNext();
                                }
                            }
                        }
                        break;
                    case ETextKennz.E_:
                    case ETextKennz.G_:
                    case ETextKennz.M_:
                    case ETextKennz.Q_:
                    case ETextKennz.W_:
                        if (View._Check2_1.CheckState == CheckState.Unchecked)
                        {
                            DataModul.Event.ForEachDo(EventIndex.KText, EventFields.KBem, nr, (cEv) =>
                            {
                                num18++;
                                View.Label2.Text = num18.AsString() + " Einträge";
                                View.Label2.Refresh();
                                return 0 < View.Sortbox1.Items.Add(new ListItem($"{cEv.iPerFamNr,10}{cEv.eArt.AsInt(),10}", (cEv.iPerFamNr, cEv.eArt)));
                            }
                            );
                        }
                        else
                        {
                            if (View._Check2_1.Checked)
                            {
                                List2_Items.Clear();
                                DataModul.DB_TexteTable.Index = nameof(TexteIndex.ITexte);
                                DataModul.DB_TexteTable.Seek("=", sLeitname2.Trim(), Modul1.eTKennz);
                                if (!DataModul.DB_TexteTable.NoMatch)
                                {
                                    while (!DataModul.DB_TexteTable.EOF && !(DataModul.DB_TexteTable.Fields[TexteFields.Kennz].AsEnum<ETextKennz>() != Modul1.eTKennz) && !(DataModul.DB_TexteTable.Fields[TexteFields.Txt].AsString() != sLeitname2.Trim()))
                                    {
                                        _ = List2_Items.Add(DataModul.DB_TexteTable.Fields[TexteFields.TxNr].AsInt());
                                        DataModul.DB_TexteTable.MoveNext();
                                    }
                                }
                                DataModul.DB_TexteTable.Index = nameof(TexteIndex.LTexte);
                                DataModul.DB_TexteTable.Seek("=", sLeitname2, Modul1.eTKennz);
                                while (!DataModul.DB_TexteTable.EOF && !DataModul.DB_TexteTable.NoMatch && !(DataModul.DB_TexteTable.Fields[TexteFields.Kennz].AsEnum<ETextKennz>() != Modul1.eTKennz) && !(DataModul.DB_TexteTable.Fields[TexteFields.Leitname].AsString() != sLeitname2.Trim()))
                                {
                                    _ = List2_Items.Add(DataModul.DB_TexteTable.Fields[TexteFields.TxNr].AsInt());
                                    DataModul.DB_TexteTable.MoveNext();
                                }
                                int num21 = List2_Items.Count - 1;
                                num12 = 0;
                                while (num12 <= num21)
                                {
                                    nr = List2_Items[num12].AsInt();
                                    DataModul.Event.ForEachDo(EventIndex.KText, EventFields.KBem, nr, (cEv) =>
                                    {
                                        num18++;
                                        View.Label2.Text = num18.AsString() + " Einträge";
                                        View.Label2.Refresh();
                                        return 0 < View.Sortbox1.Items.Add(new ListItem($"{cEv.iPerFamNr,10}{cEv.eArt.AsInt(),10}", (cEv.iPerFamNr, cEv.eArt)));
                                    }
                                    );
                                    num12++;
                                }
                            }
                        }
                        break;
                    case ETextKennz.O_:
                        DataModul.Event.ForEachDo(EventIndex.PText, EventFields.Platz, nr, (cEv) =>
                                0 < View.Sortbox1.Items.Add(new ListItem($"{cEv.iPerFamNr,10}{cEv.eArt.AsInt(),10}", (cEv.iPerFamNr, cEv.eArt))));
                        break;
                    case ETextKennz.tk3_:
                        DataModul.Event.ForEachDo(EventIndex.CText, EventFields.Causal, nr, (cEv) =>
                                0 < View.Sortbox1.Items.Add(new ListItem($"{cEv.iPerFamNr,10}{cEv.eArt.AsInt(),10}", (cEv.iPerFamNr, cEv.eArt))));
                        break;
                    case ETextKennz.tk5_:
                        DataModul.Event.ForEachDo(EventIndex.HaNu, EventFields.Hausnr, nr, (cEv) =>
                                0 < View.Sortbox1.Items.Add(new ListItem($"{cEv.iPerFamNr,10}{cEv.eArt.AsInt(),10}", (cEv.iPerFamNr, cEv.eArt))));
                        break;
                    case ETextKennz.T_:
                        DataModul.Event.ForEachDo(EventIndex.NText, EventFields.ArtText, nr, (cEv) =>
                                0 < View.Sortbox1.Items.Add(new ListItem($"{cEv.iPerFamNr,10}{cEv.eArt.AsInt(),10}", (cEv.iPerFamNr, cEv.eArt))));
                        break;
                    case ETextKennz.H_:
                        List4_Items.Clear();
                        DataModul.DB_PlaceTable.Index = nameof(PlaceIndex.O);
                        DataModul.DB_PlaceTable.Seek("=", nr);
                        while (!DataModul.DB_PlaceTable.EOF
                                && !DataModul.DB_PlaceTable.NoMatch
                                && !(DataModul.DB_PlaceTable.Fields[PlaceFields.Ort].AsInt() > nr))
                        {
                            _ = List4_Items.Add(DataModul.DB_PlaceTable.Fields[PlaceFields.OrtNr].AsInt());
                            DataModul.DB_PlaceTable.MoveNext();
                        }
                        int num23 = List4_Items.Count - 1;
                        num12 = 0;
                        while (num12 <= num23)
                        {
                            nr = List4_Items[num12].ItemData<int>();
                            DataModul.Event.ForEachDo(EventIndex.EOrt, EventFields.Ort, nr, (cEv) =>
                            {
                                _ = View.Sortbox1.Items.Add(new ListItem($"{cEv.iPerFamNr,10}{cEv.eArt.AsInt(),10}", (cEv.iPerFamNr, cEv.eArt)));
                                if (View.Sortbox1.Items.Count <= 32500)
                                    return true;
                                Abbr = 2f;
                                return false;
                            }
                            );
                            num12++;
                        }
                        break;
                    case ETextKennz.I_:
                        List4_Items.Clear();
                        DataModul.DB_PlaceTable.Index = nameof(PlaceIndex.OT);
                        DataModul.DB_PlaceTable.Seek("=", Modul1.Ubg);
                        while (!DataModul.DB_PlaceTable.EOF
                            && !DataModul.DB_PlaceTable.NoMatch
                            && !(DataModul.DB_PlaceTable.Fields[PlaceFields.Ortsteil].AsInt() > Modul1.Ubg))
                        {
                            _ = List4_Items.Add(DataModul.DB_PlaceTable.Fields[PlaceFields.OrtNr].AsInt());
                            DataModul.DB_PlaceTable.MoveNext();
                        }
                        View.Sortbox1.Items.Clear();
                        int num25 = List4_Items.Count - 1;
                        num12 = 0;
                        while (num12 <= num25)
                        {
                            nr = List4_Items[num12].AsInt();
                            DataModul.Event.ForEachDo(EventIndex.EOrt, EventFields.Ort, nr, (cEv) =>
                            {
                                _ = View.Sortbox1.Items.Add(new ListItem($"{cEv.iPerFamNr,10}{cEv.eArt.AsInt(),10}", (cEv.iPerFamNr, cEv.eArt)));
                                if (View.Sortbox1.Items.Count <= 32500)
                                    return true;
                                Abbr = 2f;
                                return false;
                            }
                            );
                            num12++;
                        }
                        break;
                    case ETextKennz.tk7_:
                        DataModul.DB_PersonTable.Index = nameof(PersonIndex.reli);
                        DataModul.DB_PersonTable.Seek("=", nr);
                        while (!DataModul.DB_PersonTable.EOF
                                && !DataModul.DB_PersonTable.NoMatch
                                && !(DataModul.DB_PersonTable.Fields[PersonFields.religi].AsInt() != nr))
                        {
                            num18++;
                            _ = View.Sortbox1.Items.Add(Strings.Right("                 " + DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt().AsString(), 10));
                            //alttnr = DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt();
                            DataModul.DB_PersonTable.MoveNext();
                        }
                        break;
                    case ETextKennz.Z_:
                        _ = Interaction.MsgBox("Nicht möglich");
                        Frame1_Visible = false;
                        break;
                    default:
                        break;
                }
            }
            listPerles();
            return num;
        }
    }

    private void Handle_Person_TextErr(string str, string text2)
    {
        IRecordset dB_TexteTable = DataModul.DB_TexteTable;

        dB_TexteTable.Index = nameof(TexteIndex.ITexte);
        dB_TexteTable.Seek("=", str.Trim(), Modul1.eTKennz);
        Nr = View._Bezeichnung1_0.Tag.AsInt();
        DataModul.Names.UpdateAllSetVal(NameIndex.TxNr, NameFields.Text, Nr, dB_TexteTable.Fields[TexteFields.TxNr].AsInt());
        View._Bezeichnung1_0.Text = Modul1.IText[EUserText.t197] + dB_TexteTable.Fields[TexteFields.TxNr].AsInt().AsString();
        View._Bezeichnung1_0.Tag = dB_TexteTable.Fields[TexteFields.TxNr].AsInt();
        dB_TexteTable.Seek("=", text2, Modul1.eTKennz);
        if (!dB_TexteTable.NoMatch)
        {
            dB_TexteTable.Delete();
        }
    }

    public void Bef1_Click(object eventSender, EventArgs eventArgs)
    {
        string sNewText = View.Text1.Text.Replace("ß", "ssss");
        string sLeitname = View.Text3.Text.Replace("ß", "ssss");
        string sRTNotes = View.RTB.Text;

        View.List3.Visible = false;
        int param = View._Bezeichnung1_0.Tag.AsInt();
        if (sNewText.Trim() == "")
        {
            // User guidance (tooltip)
            _ = Interaction.MsgBox("Leere Texte können nicht gespeichert werden.");
            return;
        }
        Modul1.eTKennz = tTextBez.eTKnz;
        if (!DatasModul_Texte_Exists(param)
            || (DataModul.DB_TexteTable.Fields[TexteFields.Txt].AsString() == sNewText.Trim()
                && DataModul.DB_TexteTable.Fields[TexteFields.Leitname].AsString() == sLeitname.Trim()
                && DataModul.DB_TexteTable.Fields[TexteFields.Bem].AsString() == sRTNotes.Trim()))
        {
            return;
        }
        View.RTB.Text = "";
        View.Text3.Text = "";
        string sOldText = DataModul.DB_TexteTable.Fields[TexteFields.Txt].AsString();

        DataModul_Texte_EditRaw(sNewText, sLeitname, sRTNotes);

        ETextKennz kennz4 = Modul1.eTKennz;
        switch (kennz4)
        {
            case ETextKennz.tkName:
            case ETextKennz.V_:
            case ETextKennz.F_:
                HandleSaveName(sNewText, param, sOldText);
                break;
            case ETextKennz.H_:
            case ETextKennz.I_:
            case ETextKennz.J_:
            case ETextKennz.K_:
            case ETextKennz.L_:
                HandleSavePlace(param);
                break;
            default:
                break;
        }

        View._Bezeichnung1_0.Text = "";
        View._Bezeichnung1_0.Tag = 0;
        View.Bezeichnung2.Text = "";
        View.Bezeichnung2.Tag = false;
        //            Update_View ?
        sNewText = "";
        Liste1_Items.Clear();
        Txknz = View.Bezeichnung6.Tag.AsEnum<ETextKennz>();
        if (View.Text2.Text != "")
        {
            var ubgT = View.Text2.Text.ToUpper();
            Modul1.STextles("TL5", Txknz, sNewText.Trim(), ocItems: MainProject.Forms.Ereignis.ListBox2.Items);
            if (List1_Items.Count > 0)
                tTextBez = M_Bezeichnu;

        }
        else
        {
            Modul1.KTextlesTL5(Txknz, List1_Items, M_Bezeichnu);
        }
    }

    private void HandleSavePlace(int num10)
    {
        Liste1_Items.Clear();
        ProjectData.ClearProjectError();
        if (num10 > 0)
        {
            Nr = View._Bezeichnung1_0.Tag.AsInt();
            (PlaceIndex eIndex, PlaceFields eIndexField) = Modul1.eTKennz switch
            {
                ETextKennz.H_ => (PlaceIndex.O, PlaceFields.Ort),
                ETextKennz.I_ => (PlaceIndex.OT, PlaceFields.Ortsteil),
                ETextKennz.J_ => (PlaceIndex.K, PlaceFields.Kreis),
                ETextKennz.K_ => (PlaceIndex.L, PlaceFields.Land),
                ETextKennz.L_ => (PlaceIndex.L, PlaceFields.Staat),
                _ => (default, default),
            };
            Place_UpdateAllSetValAct(eIndex, eIndexField, Nr, num10, (cPl) =>
            {
                _ = Liste1_Items.Add(new ListItem(cPl.ID.AsString(), cPl.ID));
                return true;
            });
            Indber();
        }
    }

    private void HandleSaveName(string sNewText, int param, string sOldText)
    {
        DataModul.DB_NameTable.Index = nameof(NameIndex.TxNr);
        DataModul.DB_NameTable.Seek("=", param);
        while (!DataModul.DB_NameTable.EOF
                    && !DataModul.DB_NameTable.NoMatch
                    && !(DataModul.DB_NameTable.Fields[NameFields.Text].AsInt() !=
                    param.AsDouble()))
        {
            DataModul.DSB_SearchTable.Index = "Nummer";
            DataModul.DSB_SearchTable.Seek("=", DataModul.DB_NameTable.Fields[NameFields.PersNr]);
            if (!DataModul.DSB_SearchTable.NoMatch)
            {
                string text4 = DataModul.DSB_SearchTable.Fields["Name"].AsString();
                if (text4 != "")
                {
                    if (Modul1.eTKennz == ETextKennz.tkName)
                    {
                        A1 = Strings.InStr(text4, ",");
                        text4 = A1 > 0 ? sNewText.Trim().Trim() + Strings.Mid(text4, A1, text4.Length) : sNewText.Trim().Trim();
                    }
                    else
                    {
                        A1 = Strings.InStr(text4, sOldText);
                        if (A1 > 0)
                        {
                            text4 = text4.Left(A1 - 1) + sNewText.Trim() + Strings.Mid(text4, A1 + sOldText.Length, text4.Length);
                        }
                    }
                    DataModul.DSB_SearchTable.Edit();
                    DataModul.DSB_SearchTable.Fields["Name"].Value = text4.Left(50).Trim();
                    if (null != DataModul.DB_TexteTable.Fields[TexteFields.Leitname].Value)
                    {
                        DataModul.DSB_SearchTable.Fields["Leit"].Value = Strings.Trim(DataModul.DB_TexteTable.Fields[TexteFields.Leitname].AsString()) == "" ? text4.Left(50).Trim()
                                        : (object)Strings.Trim(
                                            Strings.Left(DataModul.DB_TexteTable.Fields[TexteFields.Leitname].AsString() + "," + Modul1.Person.Givennames.Trim(), 50));
                    }
                    DataModul.DSB_SearchTable.Update();
                }
            }
            DataModul.DB_NameTable.MoveNext();
        }
    }

    private static void DataModul_Texte_EditRaw(string str, string sLeitname, string sRTNotes)
    {
        DataModul.DB_TexteTable.Edit();
        DataModul.DB_TexteTable.Fields[TexteFields.Leitname].Value = sLeitname.Trim() == "" ? "" : (object)sLeitname.Trim();
        DataModul.DB_TexteTable.Fields[TexteFields.Bem].Value = sRTNotes.Trim() == "" ? " " : sRTNotes.Trim();
        DataModul.DB_TexteTable.Fields[TexteFields.Txt].Value = str;
        DataModul.DB_TexteTable.Update();
    }

    private static bool DatasModul_Texte_Exists(int iTxtNr)
    {
        DataModul.DB_TexteTable.Index = nameof(TexteIndex.TxNr1);
        DataModul.DB_TexteTable.Seek("=", iTxtNr);
        return !DataModul.DB_TexteTable.NoMatch;
    }

    private static void Place_UpdateAllSetValAct(PlaceIndex eIndex, PlaceFields eIndexField, int iIndexVal, int iNewVal, Func<IPlaceData, bool> func)
    {
        IRecordset dB_PlaceTable = DataModul.DB_PlaceTable;
        dB_PlaceTable.Index = $"{eIndex}";
        dB_PlaceTable.Seek("=", iIndexVal);
        while (!dB_PlaceTable.EOF
                && !dB_PlaceTable.NoMatch
                && !(dB_PlaceTable.Fields[$"{eIndexField}"].AsInt() != iIndexVal))
        {
            if (func(new CPlaceData(dB_PlaceTable)))
            {
                dB_PlaceTable.Edit();
                dB_PlaceTable.Fields[$"{eIndexField}"].Value = iNewVal;
                dB_PlaceTable.Update();
                dB_PlaceTable.MoveNext();
            }
            else
                break;
        }
    }

    public void Bef3_Click(object eventSender, EventArgs eventArgs)
    {
        if (View.Bezeichnung2.Text != "")
        {
            if (!View.Bezeichnung2.Tag.AsBool())
            {
                DataModul.DB_TexteTable.Index = nameof(TexteIndex.TxNr1);
                DataModul.DB_TexteTable.Seek("=", Modul1.Ubg);
                if (!DataModul.DB_TexteTable.NoMatch)
                {
                    DataModul.DB_TexteTable.Delete();
                }
                View._Bezeichnung1_0.Text = "";
                View.Bezeichnung2.Text = "";
                View.Bezeichnung2.Tag = false;
                Clear_Text1_Liste1();
            }
            else
            {
                _ = Interaction.MsgBox(Modul1.IText[EUserText.t178]);
            }
        }

    }

    public void Bef2_Click(object eventSender, EventArgs eventArgs)
    {
        View.Close();
        Menue.Default.Show();
    }

    private void Bef0_Click()
    {
        Modul1.Ubg = View.Label5.Tag.AsInt();
        Nr = Modul1.Ubg;
        Frame1_Visible = true;
        View.List1.Visible = false;
        View._Command1_0.Text = Modul1.IText[EUserText.tNMBack];
        string text = Modul1.eTKennz switch
        {
            ETextKennz.tkName => "Textliste Namen\n",
            ETextKennz.A_ => "Textliste Namenzusatz vor dem Namen\n",
            ETextKennz.B_ => "Textliste Namenzusatz hinter dem Namen\n",
            ETextKennz.C_ => "Textliste Aliasnamen\n",
            ETextKennz.D_ => "Textliste Sippennamen\n",
            ETextKennz.F_ => "Textliste Vornamen weiblich\n",
            ETextKennz.V_ => "Textliste Vornamen männlich\n",
            ETextKennz.E_ => "Textliste Berufe\n",
            ETextKennz.G_ => "Textliste Titel\n",
            ETextKennz.M_ => "Textliste Kurzbemerkungen\n",
            ETextKennz.Q_ => "Textliste Strassen\n",
            ETextKennz.O_ => "Textliste " + Modul1.IText[EUserText.t194] + "\n",
            ETextKennz.U_ => "Textliste Status\n",
            ETextKennz.T_ => "Textliste Ereignisname\n",
            ETextKennz.Z_ => "Textliste Religionen\n",
            ETextKennz.H_ => "Textliste Orte\n",
            ETextKennz.J_ => "Textliste Kreise\n",
            ETextKennz.K_ => "Textliste Länder\n",
            ETextKennz.L_ => "Textliste Staaten\n",
            _ => "\n",
        };
        View.Frame1.Top = 75;
        View.Frame1.Left = 0;
        View.Frame1.Height = View.Height - 131;
        View.Frame1.Width = View.Width - 10;
        View.Frame1.BackColor = View.BackColor;
        Frame1_Visible = true;
        View.btnShowAsocPeople.Visible = false;
        View.RichTextBox1.Text = "";
        View.RichTextBox1.Visible = true;
        View.RichTextBox1.SelectionFont = new Font("Arial", 11.01f, FontStyle.Bold);
        View.RichTextBox1.SelectedText = text;
        View.RichTextBox1.SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
        if (Liste1_Items.Count > 0)
        {
            int num11 = Liste1_Items.Count - 1;
            int num12 = 0;
            while (num12 <= num11)
            {
                View.Liste1.SelectedIndex = num12;
                View.RichTextBox1.SelectedText = "  " + View.Liste1.Text.Left(240).Trim();
                DataModul.DB_TexteTable.Index = nameof(TexteIndex.TxNr1);
                DataModul.DB_TexteTable.Seek("=", Strings.Mid(View.Liste1.Text, 240, 10).Trim());
                if (!DataModul.DB_TexteTable.NoMatch)
                {
                    if (Strings.Trim(DataModul.DB_TexteTable.Fields[TexteFields.Bem].AsString()) != "")
                    {
                        View.RichTextBox1.SelectedText = ((" Bemerkung:  ") + (DataModul.DB_TexteTable.Fields[TexteFields.Bem].Value)).AsString();
                    }
                    if (null != DataModul.DB_TexteTable.Fields[TexteFields.Leitname].Value)
                    {
                        if (DataModul.DB_TexteTable.Fields[TexteFields.Leitname].AsString() != "")
                        {
                            View.RichTextBox1.SelectedText = ((" Leitname: ") + (DataModul.DB_TexteTable.Fields[TexteFields.Leitname].Value)).AsString();
                        }
                    }
                }
                View.RichTextBox1.SelectedText = "\n";
                num12++;
            }
        }

    }

    public void btnMoveNameToAlias_Click(object eventSender, EventArgs eventArgs)
    {
        int num = View._Bezeichnung1_0.Tag.AsInt();
        string text = "";
        DataModul.DB_TexteTable.Index = nameof(TexteIndex.TxNr1);
        if (num > 0)
        {
            DataModul.DB_TexteTable.Seek("=", num.AsString());
            if (!DataModul.DB_TexteTable.NoMatch)
            {
                DataModul.DB_TexteTable.Edit();
                text = DataModul.DB_TexteTable.Fields[TexteFields.Txt].AsString();
            }
            string ubgT = text;
            ETextKennz eTKennz = ETextKennz.C_;
            int iSatz = default;
            if (text.Trim() != "")
            {
                iSatz = DataModul_Texte_SaveText(text, eTKennz);

                Liste1_Items.Clear();
                DataModul.DB_NameTable.Index = nameof(NameIndex.TxNr);
                DataModul.DB_NameTable.Seek("=", num);
                while (!DataModul.DB_NameTable.EOF)
                {
                    if (!DataModul.DB_NameTable.NoMatch)
                    {
                        if (!(DataModul.DB_NameTable.Fields[NameFields.Text].AsInt() == num))
                        {
                            break;
                        }
                        _ = Liste1_Items.Add(DataModul.DB_NameTable.Fields[NameFields.PersNr].AsInt());
                    }
                    DataModul.DB_NameTable.MoveNext();
                }
            }
            DataModul.DB_NameTable.Index = nameof(NameIndex.NamKenn);

            short num4 = 0;
            while (num4 <= Liste1_Items.Count - 1)
            {
                View.Liste1.SelectedIndex = num4;
                Modul1.PersInArb = Liste1_Items[num4].ItemData<int>();
                DataModul.DB_NameTable.Seek("=", Modul1.PersInArb, eTKennz);
                if (DataModul.DB_NameTable.NoMatch)
                {
                    DataModul.DB_NameTable.AddNew();
                    DataModul.DB_NameTable.Fields[NameFields.PersNr].Value = Modul1.PersInArb;
                    DataModul.DB_NameTable.Fields[NameFields.Kennz].Value = eTKennz;
                    DataModul.DB_NameTable.Fields[NameFields.Text].Value = iSatz;
                    DataModul.DB_NameTable.Fields[NameFields.LfNr].Value = 0;
                    DataModul.DB_NameTable.Fields[NameFields.Ruf].Value = 0;
                    DataModul.DB_NameTable.Update();
                }
                else
                {
                    DataModul.DB_TexteTable.Index = nameof(TexteIndex.TxNr1);
                    DataModul.DB_TexteTable.Seek("=", DataModul.DB_NameTable.Fields[NameFields.Text]);
                    if (!DataModul.DB_TexteTable.NoMatch)
                    {
                        ubgT = ((text) + ((("; ") + (DataModul.DB_TexteTable.Fields[TexteFields.Txt].Value)))).AsString();
                    }
                    DataModul.DB_TexteTable.Index = nameof(TexteIndex.ITexte);
                    DataModul.DB_TexteTable.Seek("=", text, eTKennz);
                    int num7;
                    if (!DataModul.DB_TexteTable.NoMatch)
                    {
                        num7 = DataModul.DB_TexteTable.Fields[TexteFields.TxNr].AsInt();
                    }
                    else
                    {
                        DataModul.DB_TexteTable.Index = nameof(TexteIndex.TxNr1);
                        DataModul.DB_TexteTable.MoveLast();
                        num7 = DataModul.DB_TexteTable.Fields[TexteFields.TxNr].AsInt() + 1;
                        DataModul.DB_TexteTable.AddNew();
                        DataModul.DB_TexteTable.Fields[TexteFields.Kennz].Value = eTKennz;
                        DataModul.DB_TexteTable.Fields[TexteFields.Txt].Value = text;
                        DataModul.DB_TexteTable.Fields[TexteFields.Bem].Value = " ";
                        DataModul.DB_TexteTable.Fields[TexteFields.TxNr].Value = num7;
                        DataModul.DB_TexteTable.Update();
                    }
                    DataModul.DB_NameTable.Edit();
                    DataModul.DB_NameTable.Fields[NameFields.Text].Value = num7;
                    DataModul.DB_NameTable.Update();
                }
                num4 = (short)unchecked(num4 + 1);
            }
        }
        View._Bef_1.PerformClick();

    }

    private int DataModul_Texte_SaveText(string text, ETextKennz eTKennz)
    {
        int iSatz;
        DataModul.DB_TexteTable.Index = nameof(TexteIndex.ITexte);
        DataModul.DB_TexteTable.Seek("=", text, eTKennz);
        if (!DataModul.DB_TexteTable.NoMatch)
        {
            iSatz = DataModul.DB_TexteTable.Fields[TexteFields.TxNr].AsInt();
        }
        else
        {
            DataModul.DB_TexteTable.Index = nameof(TexteIndex.TxNr1);
            DataModul.DB_TexteTable.MoveLast();
            iSatz = Conversions.ToInteger(((DataModul.DB_TexteTable.Fields[TexteFields.TxNr].AsInt()) + (1)));
            DataModul.DB_TexteTable.AddNew();
            DataModul.DB_TexteTable.Fields[TexteFields.Kennz].Value = eTKennz;
            DataModul.DB_TexteTable.Fields[TexteFields.Txt].Value = text;
            DataModul.DB_TexteTable.Fields[TexteFields.Bem].Value = " ";
            DataModul.DB_TexteTable.Fields[TexteFields.TxNr].Value = iSatz;
            DataModul.DB_TexteTable.Update();
        }

        return iSatz;
    }

    public void Bez_Click(object eventSender, EventArgs eventArgs)
    {
        //Discarded unreachable code: IL_0b30
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
                string B;
                int index;
                switch (try0001_dispatch)
                {
                    default:
                        num = 1;
                        index = View.Bez.GetIndex((Label)eventSender);
                        goto IL_0016;
                    case 3820:
                        {
                            num2 = num;
                            switch (num3 <= -2 ? 1 : num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_0bf2;
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
                                goto IL_0bf2;
                            }
                            else
                            {
                                Modul1.UbgT = "Texte Bez_klick";
                                _ = Interaction.MsgBox("Fehler " + Information.Err().Number.AsString() + " in " + Modul1.UbgT);
                                _ = Interaction.MsgBox("F120");
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                                goto IL_0bf6;
                            }
                        }
                    end_IL_0001:
                        break;
                    IL_0016:
                        num = 2;
                        View.Command3.Visible = false;
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        View.btnMoveNameToAlias.Visible = false;
                        View._Bef_4.Visible = true;
                        View_HideText3_Label1_1();
                        View.Text3.Text = "";
                        View._Check2_0.Visible = false;
                        (string, ETextKennz) bezeichnu = ("", ETextKennz.tkNone);
                        ETextKennz tKennz = ETextKennz.tkNone;
                        switch (index)
                        {
                            case 0:
                                tKennz = ETextKennz.O_;
                                bezeichnu = (Modul1.IText[EUserText.t194] + new string(' ', 100), tKennz);
                                break;
                            case 1:
                                tKennz = ETextKennz.tkName;
                                View.btnMoveNameToAlias.Visible = true;
                                bezeichnu = (Modul1.IText[EUserText.t180] + new string(' ', 100), tKennz);
                                View_ShowText3_Label1_1();
                                break;
                            case 2:
                                tKennz = ETextKennz.F_;
                                bezeichnu = (Modul1.IText[EUserText.t181] + new string(' ', 100), tKennz);
                                View_ShowText3_Label1_1();
                                break;
                            case 3:
                                tKennz = ETextKennz.V_;
                                bezeichnu = (Modul1.IText[EUserText.t182] + new string(' ', 100), tKennz);
                                View_ShowText3_Label1_1();
                                break;
                            case 4:
                                tKennz = ETextKennz.B_;
                                bezeichnu = (Modul1.IText[EUserText.t195] + new string(' ', 100), tKennz);
                                View_ShowText3_Label1_1();
                                break;
                            case 5:
                                tKennz = ETextKennz.C_;
                                bezeichnu = (Modul1.IText[EUserText.tAlias] + new string(' ', 100), tKennz);
                                break;
                            case 6:
                                tKennz = ETextKennz.E_;
                                bezeichnu = (Modul1.IText[EUserText.tTitle] + new string(' ', 100), tKennz);
                                View_ShowText3_Label1_1();
                                break;
                            case 7:
                                tKennz = ETextKennz.G_;
                                bezeichnu = (Modul1.IText[EUserText.t70] + new string(' ', 100), tKennz);
                                View_ShowText3_Label1_1();
                                break;
                            case 8:
                                tKennz = ETextKennz.Q_;
                                bezeichnu = (Modul1.IText[EUserText.t75] + new string(' ', 100), tKennz);
                                break;
                            case 9:
                                tKennz = ETextKennz.M_;
                                bezeichnu = (Modul1.IText[EUserText.t186] + new string(' ', 100), tKennz);
                                break;
                            case 10:
                                tKennz = ETextKennz.H_;
                                bezeichnu = (Modul1.IText[EUserText.t99_Places] + new string(' ', 100), tKennz);
                                break;
                            case 11:
                                tKennz = ETextKennz.J_;
                                bezeichnu = (Modul1.IText[EUserText.t189] + new string(' ', 100), tKennz);
                                break;
                            case 12:
                                tKennz = ETextKennz.K_;
                                bezeichnu = (Modul1.IText[EUserText.t190] + new string(' ', 100), tKennz);
                                break;
                            case 13:
                                tKennz = ETextKennz.L_;
                                bezeichnu = (Modul1.IText[EUserText.t191] + new string(' ', 100), tKennz);
                                break;
                            case 14:
                                tKennz = ETextKennz.I_;
                                bezeichnu = (Modul1.IText[EUserText.t188] + new string(' ', 100), tKennz);
                                break;
                            case 15:
                                tKennz = ETextKennz.D_;
                                bezeichnu = ($"{Modul1.IText[EUserText.t126],100}", tKennz);
                                View._Check2_0.Visible = true;
                                break;
                            case 16:
                                tKennz = ETextKennz.A_;
                                bezeichnu = ($"{Modul1.IText[EUserText.t196],100}", tKennz);
                                ;
                                View._Check2_0.Visible = true;
                                break;
                            case 17:
                                tKennz = ETextKennz.T_;
                                bezeichnu = ($"{"Ereignisbezeichnungen",100}", tKennz);
                                break;
                            case 18:
                                tKennz = ETextKennz.U_;
                                View._Check2_0.Visible = true;
                                bezeichnu = ($"{"Statusbezeichnungen",100}", tKennz);
                                break;
                            case 19:
                                tKennz = ETextKennz.Z_;
                                View.Cursor = Cursors.WaitCursor;
                                View.Label6.Visible = true;
                                Application.DoEvents();
                                View._Check2_0.Visible = true;
                                bezeichnu = ($"{"Religionsbezeichnungen",100}", tKennz);
                                if (DataModul.DT_RelgionTable.RecordCount == 0)
                                {
                                    DataModul.DB_TexteTable.Index = nameof(TexteIndex.KText);
                                    DataModul.DB_TexteTable.Seek("=", tKennz);
                                    while (!DataModul.DB_TexteTable.EOF && !DataModul.DB_TexteTable.NoMatch
                                        && !(DataModul.DB_TexteTable.Fields[TexteFields.Kennz].AsEnum<ETextKennz>() != tKennz))
                                    {
                                        DataModul.DB_TexteTable.Delete();
                                        DataModul.DB_TexteTable.MoveNext();
                                    }
                                    if (DataModul.DB_PersonTable.RecordCount == 0)
                                    {
                                        goto end_IL_0001_2;
                                    }
                                    DataModul.DB_PersonTable.MoveFirst();
                                    int recordCount = DataModul.DB_PersonTable.RecordCount;
                                    int num5 = 1;
                                    while (num5 <= recordCount)
                                    {
                                        Modul1.UbgT = DataModul.DB_PersonTable.Fields[PersonFields.Konv].AsString();
                                        if (Modul1.UbgT != "")
                                        {
                                            Modul1.eTKennz = ETextKennz.Z_;
                                            Modul1.Ubg = Modul1.TextSpeich(Modul1.UbgT, Modul1.UbgT1, Modul1.eTKennz, Modul1.PersInArb, Modul1.LfNR);
                                        }
                                        else
                                        {
                                            Modul1.Ubg = 0;
                                        }
                                        DataModul.DT_RelgionTable.AddNew();
                                        DataModul.DT_RelgionTable.Fields["PerNr"].Value = DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt();
                                        DataModul.DT_RelgionTable.Fields["TextNr"].Value = Modul1.Ubg;
                                        DataModul.DT_RelgionTable.Update();
                                        DataModul.DB_PersonTable.MoveNext();
                                        num5 = checked(num5 + 1);
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                        tTextBez = bezeichnu;
                        View.Cursor = Cursors.Arrow;
                        View.Label6.Visible = false;
                        Application.DoEvents();
                        M_Bezeichnu = bezeichnu;
                        View._Bezeichnung1_0.Text = "";
                        View._Bezeichnung1_0.Tag = 0;
                        View.Bezeichnung2.Text = "";
                        View.Bezeichnung2.Tag = false;
                        Update_View();
                        goto end_IL_0001_2;

                    IL_0bf2:
                        num4 = num2 + 1;
                        goto IL_0bf6;
                    IL_0bf6:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;

                            case 130:
                            case 175:
                            case 176:
                            case 177:
                            case 186:
                                goto end_IL_0001_2;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0001_dispatch = 3820;
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
    public void Command1_Click(object eventSender, EventArgs eventArgs)
    {
        checked
        {
            switch (View.Command1.GetIndex((Button)eventSender))
            {
                case 0:
                    View.btnShowAsocPeople.Enabled = true;
                    Frame1_Visible = false;
                    break;
                case 1:
                    FileSystem.FileClose(99);
                    if (View.List1.Visible)
                    {
                        View.RichTextBox1.Text = "";
                        int num = List1_Items.Count - 1;
                        int M1_Iter = 0;
                        while (true)
                        {
                            int i = M1_Iter;
                            int num2 = num;
                            if (i > num2)
                            {
                                break;
                            }
                            View.List1.SelectedIndex = M1_Iter;
                            View.RichTextBox1.SelectionFont = new Font("Courier New", 10.01f, FontStyle.Regular);
                            View.RichTextBox1.SelectedText = View.List1.Text.Left(160) + "\n";
                            M1_Iter++;
                        }
                        FileSystem.FileClose(99);
                    }
                    if (Modul1.Typ == DriveType.CDRom)
                    {
                        View.RichTextBox1.SaveFile(Modul1.TempPath + "\\Text2.RTF", RichTextBoxStreamType.RichText);
                        View.RichTextBox1.LoadFile(Modul1.TempPath + "\\Text2.RTF", RichTextBoxStreamType.RichText);
                    }
                    else
                    {
                        View.RichTextBox1.SaveFile(Modul1.TempPath + "\\Text2.RTF", RichTextBoxStreamType.RichText);
                        View.RichTextBox1.LoadFile(Modul1.TempPath + "\\Text2.RTF", RichTextBoxStreamType.RichText);
                        View.RichTextBox1.SaveFile(Modul1.GenFreeDir + "\\Temp\\Text2.RTF", RichTextBoxStreamType.RichText);
                        View.RichTextBox1.LoadFile(Modul1.GenFreeDir + "\\Temp\\Text2.RTF", RichTextBoxStreamType.RichText);
                    }
                    Modul1.Ausdruck("\\Text2.RTF");
                    View.RichTextBox1.SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
                    break;
            }
        }
    }

    public void Command3_Click(object eventSender, EventArgs eventArgs)
    {
        //Discarded unreachable code: IL_0bcb
        int try0001_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        int num5 = default;
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
                    int i;
                    int num6;
                    switch (try0001_dispatch)
                    {
                        default:
                            num = 1;
                            Modul1.eTKennz = tTextBez.eTKnz;
                            goto IL_001a;
                        case 3711:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_0c81;
                                    default:
                                        goto end_IL_0001;
                                }
                                int number = Information.Err().Number;
                                if (number == 94)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_0c81;
                                }
                                else
                                {
                                    Modul1.UbgT = "Texte Liste1_Dblklick";
                                    _ = Interaction.MsgBox("Fehler " + Information.Err().Number.AsString() + " in " + Modul1.UbgT);
                                    _ = Interaction.MsgBox("F121");
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    num4 = num2;
                                    goto IL_0c85;
                                }
                            }
                        end_IL_0001:
                            break;
                        IL_001a:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            num5 = Liste1_Items.Count - 1;
                            M1_Iter = 0;
                            while (M1_Iter <= num5)
                            {
                                View.Liste1.SelectedIndex = M1_Iter;
                                Modul1.Ubg = View.Liste1.SelectedItem.ItemData<int>();
                                DataModul.DB_TexteTable.Index = nameof(TexteIndex.TxNr1);
                                DataModul.DB_TexteTable.Seek("=", Modul1.Ubg);
                                ETextKennz text = tTextBez.eTKnz;
                                switch (text)
                                {
                                    case ETextKennz.tkName:
                                    case ETextKennz.F_:
                                    case ETextKennz.V_:
                                    case ETextKennz.A_:
                                    case ETextKennz.B_:
                                    case ETextKennz.C_:
                                    case ETextKennz.D_:
                                    case ETextKennz.U_:
                                    case ETextKennz.tk2_:
                                        DataModul.DB_NameTable.Index = nameof(NameIndex.TxNr);
                                        DataModul.DB_NameTable.Seek("=", Modul1.Ubg);
                                        if (DataModul.DB_NameTable.NoMatch)
                                        {
                                            DataModul.DB_OFBTable.Index = "IndNum";
                                            DataModul.DB_OFBTable.Seek("=", Modul1.Ubg);
                                            if (DataModul.DB_OFBTable.NoMatch)
                                            {
                                                DataModul.DB_TexteTable.Delete();
                                            }
                                        }
                                        break;
                                    case ETextKennz.H_:
                                        DataModul.DB_PlaceTable.Index = nameof(PlaceIndex.O);
                                        DataModul.DB_PlaceTable.Seek("=", Modul1.Ubg);
                                        if (DataModul.DB_PlaceTable.NoMatch)
                                        {
                                            DataModul.DB_TexteTable.Delete();
                                        }
                                        break;
                                    case ETextKennz.I_:
                                        DataModul.DB_PlaceTable.Index = nameof(PlaceIndex.OT);
                                        DataModul.DB_PlaceTable.Seek("=", Modul1.Ubg);
                                        if (DataModul.DB_PlaceTable.NoMatch)
                                        {
                                            DataModul.DB_TexteTable.Delete();
                                        }
                                        break;
                                    case ETextKennz.J_:
                                        DataModul.DB_PlaceTable.Index = nameof(PlaceIndex.K);
                                        DataModul.DB_PlaceTable.Seek("=", Modul1.Ubg);
                                        if (DataModul.DB_PlaceTable.NoMatch)
                                        {
                                            DataModul.DB_TexteTable.Delete();
                                        }
                                        break;
                                    case ETextKennz.K_:
                                        DataModul.DB_PlaceTable.Index = nameof(PlaceIndex.L);
                                        DataModul.DB_PlaceTable.Seek("=", Modul1.Ubg);
                                        if (DataModul.DB_PlaceTable.NoMatch)
                                        {
                                            DataModul.DB_TexteTable.Delete();
                                        }
                                        break;
                                    case ETextKennz.L_:
                                        DataModul.DB_PlaceTable.Index = nameof(PlaceIndex.S);
                                        DataModul.DB_PlaceTable.Seek("=", Modul1.Ubg);
                                        if (DataModul.DB_PlaceTable.NoMatch)
                                        {
                                            DataModul.DB_TexteTable.Delete();
                                        }
                                        break;
                                    case ETextKennz.E_:
                                    case ETextKennz.M_:
                                    case ETextKennz.G_:
                                    case ETextKennz.Q_:
                                    case ETextKennz.W_:
                                        if (DataModul.Event.Seek(EventIndex.KText, Modul1.Ubg) == null)
                                        {
                                            DataModul.DB_OFBTable.Index = "IndNum";
                                            DataModul.DB_OFBTable.Seek("=", Modul1.Ubg);
                                            if (DataModul.DB_OFBTable.NoMatch)
                                            {
                                                DataModul.DB_TexteTable.Delete();
                                            }
                                        }
                                        break;
                                    case ETextKennz.O_:
                                    case ETextKennz.T_:
                                    case ETextKennz.tk3_:
                                    case ETextKennz.tk5_:
                                        lErl = 3;
                                        (EventIndex eIndex, EventFields eIndexFields) = text switch
                                        {
                                            ETextKennz.T_ => (EventIndex.NText, EventFields.ArtText),
                                            ETextKennz.tk3_ => (EventIndex.CText, EventFields.Causal),
                                            ETextKennz.tk5_ => (EventIndex.HaNu, EventFields.Hausnr),
                                            _ => (EventIndex.PText, EventFields.Platz),
                                        };
                                        DataModul.Event.UpdateClearPred(eIndex, eIndexFields, Modul1.Ubg, (cEv) => !(text == ETextKennz.T_
                                            && (cEv.eArt == EEventArt.eA_105
                                              || cEv.eArt == EEventArt.eA_603)));
                                        DataModul.DB_TexteTable.Delete();
                                        break;
                                    case ETextKennz.tk7_:
                                        DataModul.DB_PersonTable.Index = nameof(PersonIndex.reli);
                                        DataModul.DB_PersonTable.Seek("=", Modul1.Ubg);
                                        if (DataModul.DB_PersonTable.NoMatch)
                                        {
                                            DataModul.DB_TexteTable.Delete();
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                M1_Iter++;
                            }
                            Liste1_Items.Clear();
                            View.Command3.Enabled = false;
                            goto end_IL_0001_2;

                        IL_0c81:
                            num4 = unchecked(num2 + 1);
                            goto IL_0c85;
                        IL_0c85:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 102:
                                    num = 102;
                                    Liste1_Items.Clear();
                                    goto end_IL_0001_2;
                                case 103:
                                case 108:
                                case 110:
                                case 114:
                                case 115:
                                case 121:
                                case 122:
                                case 123:
                                    goto end_IL_0001_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 3711;
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
    public void Form_Load(object eventSender, EventArgs eventArgs)
    {
        View_HideButtons();
        View.RichTextBox1.SelectionFont = new Font("Arial", 11.01f, FontStyle.Regular);
        View.Text1.Font = new Font("Arial", 11.01f, FontStyle.Regular);
        View.Text4.Font = new Font("Arial", 11.01f, FontStyle.Regular);
        View.Liste1.Font = new Font("Courier New", 11.01f, FontStyle.Regular);
        View.List1.Font = new Font("Courier New", 11.01f, FontStyle.Regular);
        View.List2.Font = new Font("Courier New", 11.01f, FontStyle.Regular);
        View.List3.Font = new Font("Courier New", 11.01f, FontStyle.Regular);
        View.List4.Font = new Font("Courier New", 11.01f, FontStyle.Regular);
        if (Modul1.FontSize > 0f)
        {
            View.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.Bezeichnung3.Font = new Font("Arial", Modul1.FontSize, FontStyle.Bold);
            View._Label1_2.Font = new Font("Arial", Modul1.FontSize, FontStyle.Bold);
            View._Label1_3.Font = new Font("Arial", Modul1.FontSize, FontStyle.Bold);
            View._Label1_4.Font = new Font("Arial", Modul1.FontSize, FontStyle.Bold);
            View.Text4.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.RichTextBox1.SelectionFont = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.Text1.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.Text4.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.RTB.Font = new Font("Arial", Modul1.FontSize, FontStyle.Bold);
            View.Liste1.Font = new Font("Courier New", Modul1.FontSize, FontStyle.Regular);
            View.List1.Font = new Font("Courier New", Modul1.FontSize, FontStyle.Regular);
            View.List2.Font = new Font("Courier New", Modul1.FontSize, FontStyle.Regular);
            View.List3.Font = new Font("Courier New", Modul1.FontSize, FontStyle.Regular);
            View.List4.Font = new Font("Courier New", Modul1.FontSize, FontStyle.Regular);
        }
        byte b = 3;
        View.Label3.Text = Modul1.IText[EUserText.t421];
        View.Label7.Text = Modul1.IText[EUserText.t180];
        checked
        {
            View.Label8.Left = View.Label7.Left + View.Label7.Width + unchecked(b);
            View.Label8.Text = Modul1.IText[EUserText.t181];
            View.Label9.Left = View.Label8.Left + View.Label8.Width + unchecked(b);
            View.Label9.Text = Modul1.IText[EUserText.t182];
            View.Label10.Left = View.Label9.Left + View.Label9.Width + unchecked(b);
            View.Label10.Text = Modul1.IText[EUserText.t413];
            View.Label11.Left = View.Label10.Left + View.Label10.Width + unchecked(b);
            View.Label11.Text = Modul1.IText[EUserText.t414];
            View.Label12.Left = View.Label11.Left + View.Label11.Width + unchecked(b);
            View.Label12.Text = Modul1.IText[EUserText.tAlias];
            View.Label13.Left = View.Label12.Left + View.Label12.Width + unchecked(b);
            View.Label13.Text = Modul1.IText[EUserText.t126];
            View.Label14.Left = View.Label13.Left + View.Label13.Width + unchecked(b);
            View.Label14.Text = Modul1.IText[EUserText.tTitle];
            View.Label15.Left = View.Label14.Left + View.Label14.Width + unchecked(b);
            View.Label15.Text = Modul1.IText[EUserText.t70];
            View.Label16.Left = View.Label15.Left + View.Label15.Width + unchecked(b);
            View.Label16.Text = Modul1.IText[EUserText.t186];
            View.Label17.Left = View.Label16.Left + View.Label16.Width + unchecked(b);
            View.Label17.Text = Modul1.IText[EUserText.t397];
            View.Label18.Text = Modul1.IText[EUserText.t416];
            View.Label19.Left = View.Label18.Left + View.Label18.Width + unchecked(b);
            View.Label19.Text = Modul1.IText[EUserText.t99_Places];
            View.Label20.Left = View.Label19.Left + View.Label19.Width + unchecked(b);
            View.Label20.Text = Modul1.IText[EUserText.t188];
            View.Label21.Left = View.Label20.Left + View.Label20.Width + unchecked(b);
            View.Label21.Text = Modul1.IText[EUserText.t189];
            View.Label22.Left = View.Label21.Left + View.Label21.Width + unchecked(b);
            View.Label22.Text = Modul1.IText[EUserText.t190];
            View.Label23.Left = View.Label22.Left + View.Label22.Width + unchecked(b);
            View.Label23.Text = Modul1.IText[EUserText.t191];
            View.Label24.Left = View.Label23.Left + View.Label23.Width + unchecked(b);
            View.Label24.Text = Modul1.IText[EUserText.t75];
            View.Label29.Left = View.Label24.Left + View.Label24.Width + unchecked(b);
            View.Label29.Text = Modul1.IText[EUserText.t417];
            View.Label25.Left = View.Label29.Left + View.Label29.Width + unchecked(b);
            View.Label25.Text = Modul1.IText[EUserText.t194];
            View.Label26.Left = View.Label25.Left + View.Label25.Width + unchecked(b);
            View.Label26.Text = Modul1.IText[EUserText.t415];
            View.Label27.Left = View.Label26.Left + View.Label26.Width + unchecked(b);
            View.Label27.Text = Modul1.IText[EUserText.t418];
            View.Label28.Left = View.Label27.Left + View.Label27.Width + unchecked(b);
            View.Label28.Text = Modul1.IText[EUserText.t419];
            View._Label1_0.Text = Modul1.IText[EUserText.tText];
            View._Label1_1.Text = Modul1.IText[EUserText.t424];
            View.btnShowAsocPeople.Text = Modul1.IText[EUserText.t432];
            View.Button2.Text = Modul1.IText[EUserText.t426];
            View.btnMoveToChurchCemet.Text = Modul1.IText[EUserText.t427];
            View.btnMoveToEntityAnot.Text = Modul1.IText[EUserText.t434];
            View.btnMoveToLowerDateAnot.Text = Modul1.IText[EUserText.t433];
            View._Bezeichnung1_1.Text = Modul1.IText[EUserText.t422];
            View.Bezeichnung3.Text = Modul1.IText[EUserText.t193] + ":";
            View._Bef_0.Text = Modul1.IText[EUserText.t429];
            View.Command3.Text = Modul1.IText[EUserText.t428];
            View.Check1.Text = Modul1.IText[EUserText.t423];
            View._Check2_0.Text = Modul1.IText[EUserText.t321];
            View._Check2_1.Text = Modul1.IText[EUserText.t321];
            View.Check3.Text = Modul1.IText[EUserText.t420];
            var Pos = Modul1.Persistence.ReadIntsProg("maspos.dat", 2);
            View.Left = Pos[0];
            View.Top = Pos[1];
            View.Text = "Gen_Plus Textverwaltung für Mandant: " + Modul1.Verz;
            View.WindowState = Menue.Default.WindowState;
            View.BackColor = Modul1.HintFarb;
            View._Label1_4.Text = Modul1.VersionT;
            View._Label1_3.Text = Modul1.Version1;
            View._Label1_2.Text = Modul1.Version;
            if (Modul1.System.VerSpecial == 1)
            {
                View._Label1_2.Text = "Eingeschränkte Sonderversion";
            }
            View.btnMoveNameToAlias.Text = Modul1.IText[EUserText.t179];
            View._Bef_1.Text = Modul1.IText[EUserText.tNMSave];
            View._Bef_4.Text = "&Verwendung";
            View._Bef_3.Text = "&" + Modul1.IText[EUserText.tDelete];
            View._Bef_3.Enabled = false;
            View._Bef_2.Text = Modul1.IText[EUserText.t158];
            View.Bezeichnung5.Text = Modul1.IText[EUserText.t192];
            View.Bezeichnung3.Text = Modul1.IText[EUserText.t193];
            View_HideText3_Label1_1();
            View.Label2.BackColor = View.BackColor;
            View.Bezeichnung2.BackColor = View.BackColor;
            if (Modul1_Glob)
            {
                View.btnReenter.Visible = true;
            }
            if (Modul1.Typ == DriveType.CDRom)
            {
                View.btnMoveNameToAlias.Enabled = false;
                View._Bef_1.Enabled = false;
                View.Command3.Enabled = false;
            }
        }
    }

    private void View_HideButtons()
    {
        View.btnDeleteEntry.Visible = false;
        View.btnMoveToDateAnot.Visible = false;
        View.btnMoveToEntityAnot.Visible = false;
        View.btnMoveToLowerDateAnot.Visible = false;
        View.btnMoveToCause.Visible = false;
        View.btnMoveToChurchCemet.Visible = false;
    }

    public void Form_Closing(object eventSender, FormClosingEventArgs eventArgs)
    {
        bool cancel = default;
        int num2 = default;
        try
        {
            CloseReason closeReason = eventArgs.CloseReason;
            ProjectData.ClearProjectError();
            if (closeReason != 0)
            {
                return;
            }
            DataModul.MandDB.Close();
            DataModul.TempDB.Close();
            DataModul.DOSB.Close();
            DataModul.DSB.Close();
            ProjectData.EndApp();
        }
        catch (Exception obj)
        {
            ProjectData.SetProjectError(obj);
            if (Information.Err().Number == 91)
            {
                ProjectData.ClearProjectError();
                if (num2 == 0)
                {
                    throw ProjectData.CreateProjectError(-2146828268);
                }
            }
            else if (Information.Err().Number == 3420)
            {
                ProjectData.ClearProjectError();
                if (num2 == 0)
                {
                    throw ProjectData.CreateProjectError(-2146828268);
                }
            }
            eventArgs.Cancel = cancel;
        }
    }

    public void List1_DoubleClick(object eventSender, EventArgs eventArgs)
    {
        if (List1_Items.Count == 0 | View.List1.SelectedIndex < 0)
        {
            return;
        }
        checked
        {
            if (List1_Items.ItemData<int>(View.List1.SelectedIndex) != 0)
            {
                Modul1.Ubg = List1_Items.ItemData<int>(View.List1.SelectedIndex);
                Modul1.Schalt = (byte)List1_Items.ItemData(View.List1.SelectedIndex);
                MainProject.Forms.Ortsver.Button10.Text = Modul1.IText[EUserText.tNMBack];
                MainProject.Forms.Ortsver.Show();
                return;
            }
            if (View.List1.Text.Left(1) == "F")
            {
                Modul1.FamInArb = Strings.Mid(View.List1.Text, 2, 10).AsInt();
                Familie.Default.Show(Modul1.FamInArb);
                return;
            }
            Modul1.PersInArb = (int)Math.Round(View.List1.Text.Right(10).AsDouble());
            Personen.Default.lblSearch2.Text = "";
            Modul1.Aend = 0f;
            Personen.Default.Close();
            if ((double)(int)Math.Round(View.List1.Text.Right(10).AsDouble()) != Personen.Default.PersonNr)
            {
                Modul1.PersInArb = (int)Math.Round(View.List1.Text.Right(10).AsDouble());
            }
            Personen.Default.Show(Modul1.PersInArb, EUserText.tNMBack);
        }
    }

    public void List3_DoubleClick(object eventSender, EventArgs eventArgs)
    {
        View.Text3.Text = View.List3.Text;
        View.List3.Visible = false;
    }

    public void Liste1_DoubleClick(object eventSender, EventArgs eventArgs)
    {
        //Discarded unreachable code: IL_16e6
        int try0001_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        string Artt = default;
        int lErl = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                int num4;
                string text2;
                IEventData cEvent;
                switch (try0001_dispatch)
                {
                    default:
                        num = 1;
                        View.btnDeleteEntry.Visible = false;
                        goto IL_0011;
                    case 7076:
                        {
                            num2 = num;
                            switch (num3 <= -2 ? 1 : num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_17f6;
                                default:
                                    goto end_IL_0001;
                            }
                            int number = Information.Err().Number;
                            if (number == 94)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_17f6;
                            }
                            else
                            {
                                Modul1.UbgT = "Texte Liste1_Dblklick";
                                if (Interaction.MsgBox(Conversion.ErrorToString(), title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                _ = Interaction.MsgBox("Fehler " + Information.Err().Number.AsString() + " in " + Modul1.UbgT);
                                _ = Interaction.MsgBox("F121");
                                Debugger.Break();
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                                goto IL_17fa;
                            }
                        }
                    end_IL_0001:
                        break;
                    IL_0011:
                        num = 2;
                        View.List3.Visible = false;

                        View.btnDeleteEntry.Visible = false;
                        View._Check2_1.Visible = false;

                        View.Text3.Text = "";
                        var eTKennz = tTextBez.eTKnz;
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        View._Bezeichnung1_0.Text = Modul1.IText[EUserText.t197] + Strings.Mid(View.Liste1.Text, 241, View.Liste1.Text.Length);
                        View._Bezeichnung1_0.Tag = View.Liste1.SelectedItem.ItemData<int>();
                        Modul1.Ubg = View.Liste1.SelectedItem.ItemData<int>();
                        View.Label5.Text = Modul1.Ubg.AsString();
                        View.Label5.Tag = Modul1.Ubg;
                        (string Modul1_V, string Modul1_LD) = DataModul.TextLese2(Modul1.Ubg);
                        View.Text1.Text = Modul1_V;
                        View.Text3.Text = Modul1_LD;
                        View.RTB.Text = DataModul.DB_TexteTable.Fields[TexteFields.Bem].AsString();
                        View.Bezeichnung2.Text = "";
                        View.Bezeichnung2.Tag = false;
                        View._Bef_4.Enabled = true;
                        switch (eTKennz)
                        {
                            case ETextKennz.tkName:
                            case ETextKennz.F_:
                            case ETextKennz.V_:
                            case ETextKennz.A_:
                            case ETextKennz.B_:
                            case ETextKennz.C_:
                            case ETextKennz.D_:
                            case ETextKennz.U_:
                            case ETextKennz.tk2_:
                                DataModul.DB_NameTable.Index = nameof(NameIndex.TxNr);
                                DataModul.DB_NameTable.Seek("=", Modul1.Ubg);
                                if (!DataModul.DB_NameTable.NoMatch)
                                {
                                    View.Bezeichnung2.Text = Modul1.IText[EUserText.t176] + ": " + Modul1.IText[EUserText.tYes];
                                    View.Bezeichnung2.Tag = true;
                                    text2 = Modul1.IText[EUserText.t177] + DataModul.DB_NameTable.Fields[NameFields.PersNr].AsInt().AsString();
                                }
                                else
                                {
                                    DataModul.DB_OFBTable.Index = "IndNum";
                                    DataModul.DB_OFBTable.Seek("=", Modul1.Ubg);
                                    if (!DataModul.DB_OFBTable.NoMatch)
                                    {
                                        View.Bezeichnung2.Text = $"{Modul1.IText[EUserText.t176]}: {Modul1.IText[EUserText.tYes]}, in den OFB-Feldern; Auflistung nicht möglich";
                                        View.Bezeichnung2.Tag = true;
                                        View._Bef_4.Enabled = false;
                                        goto end_IL_0001_2;
                                    }
                                    else
                                    {
                                        View.Bezeichnung2.Text = $"{Modul1.IText[EUserText.t176]}: {Modul1.IText[EUserText.tNo]}";
                                        View.Bezeichnung2.Tag = false;
                                    }
                                }
                                break;
                            case ETextKennz.H_:
                                DataModul.DB_PlaceTable.Index = nameof(PlaceIndex.O);
                                DataModul.DB_PlaceTable.Seek("=", Modul1.Ubg);
                                if (!DataModul.DB_PlaceTable.NoMatch)
                                {
                                    View.Bezeichnung2.Text = Modul1.IText[EUserText.t176] + ": " + Modul1.IText[EUserText.tYes] + ", bei Ort:" + DataModul.DB_PlaceTable.Fields[PlaceFields.OrtNr].AsString();
                                    View.Bezeichnung2.Tag = true;
                                }
                                else
                                {
                                    View.Bezeichnung2.Text = Modul1.IText[EUserText.t176] + ": " + Modul1.IText[EUserText.tNo];
                                    View.Bezeichnung2.Tag = false;
                                }
                                break;
                            case ETextKennz.I_:
                                DataModul.DB_PlaceTable.Index = nameof(PlaceIndex.OT);
                                DataModul.DB_PlaceTable.Seek("=", Modul1.Ubg);
                                if (!DataModul.DB_PlaceTable.NoMatch)
                                {
                                    View.Bezeichnung2.Text = Modul1.IText[EUserText.t176] + ": " + Modul1.IText[EUserText.tYes];
                                    View.Bezeichnung2.Tag = true;
                                }
                                else
                                {
                                    View.Bezeichnung2.Text = Modul1.IText[EUserText.t176] + ": " + Modul1.IText[EUserText.tNo];
                                    View.Bezeichnung2.Tag = false;
                                }
                                break;
                            case ETextKennz.J_:
                                DataModul.DB_PlaceTable.Index = nameof(PlaceIndex.K);
                                DataModul.DB_PlaceTable.Seek("=", Modul1.Ubg);
                                if (!DataModul.DB_PlaceTable.NoMatch)
                                {
                                    View.Bezeichnung2.Text = Modul1.IText[EUserText.t176] + ": " + Modul1.IText[EUserText.tYes];
                                    View.Bezeichnung2.Tag = true;
                                }
                                else
                                {
                                    View.Bezeichnung2.Text = Modul1.IText[EUserText.t176] + ": " + Modul1.IText[EUserText.tNo];
                                    View.Bezeichnung2.Tag = false;
                                }
                                break;
                            case ETextKennz.K_:
                                DataModul.DB_PlaceTable.Index = nameof(PlaceIndex.L);
                                DataModul.DB_PlaceTable.Seek("=", Modul1.Ubg);
                                if (!DataModul.DB_PlaceTable.NoMatch)
                                {
                                    View.Bezeichnung2.Text = Modul1.IText[EUserText.t176] + ": " + Modul1.IText[EUserText.tYes];
                                    View.Bezeichnung2.Tag = true;
                                }
                                else
                                {
                                    View.Bezeichnung2.Text = Modul1.IText[EUserText.t176] + ": " + Modul1.IText[EUserText.tNo];
                                    View.Bezeichnung2.Tag = false;
                                }
                                break;
                            case ETextKennz.L_:
                                DataModul.DB_PlaceTable.Index = nameof(PlaceIndex.S);
                                DataModul.DB_PlaceTable.Seek("=", Modul1.Ubg);
                                if (!DataModul.DB_PlaceTable.NoMatch)
                                {
                                    View.Bezeichnung2.Text = Modul1.IText[EUserText.t176] + ": " + Modul1.IText[EUserText.tYes];
                                    View.Bezeichnung2.Tag = true;
                                }
                                else
                                {
                                    View.Bezeichnung2.Text = Modul1.IText[EUserText.t176] + ": " + Modul1.IText[EUserText.tNo];
                                    View.Bezeichnung2.Tag = false;
                                }
                                break;
                            case ETextKennz.O_:
                                View.btnMoveToDateAnot.Visible = true;
                                View.btnMoveToLowerDateAnot.Visible = true;
                                if (!DataModul.Event.ReadData(EventIndex.PText, Modul1.Ubg, out cEvent))
                                {
                                    Artt = Artsch(cEvent);
                                    View.Bezeichnung2.Text = Modul1.IText[EUserText.t176] + ": " + Modul1.IText[EUserText.tYes];
                                    View.Bezeichnung2.Tag = true;
                                }
                                else
                                {
                                    View.Bezeichnung2.Text = Modul1.IText[EUserText.t176] + ": " + Modul1.IText[EUserText.tNo];
                                    View.Bezeichnung2.Tag = false;
                                }
                                break;
                            case ETextKennz.E_:
                            case ETextKennz.M_:
                            case ETextKennz.G_:
                            case ETextKennz.Q_:
                            case ETextKennz.W_:
                                if (!DataModul.Event.ReadData(EventIndex.KText, Modul1.Ubg, out cEvent))
                                {
                                    Artt = Artsch(cEvent);
                                    View.Bezeichnung2.Text = Modul1.IText[EUserText.t176] + ": " + Modul1.IText[EUserText.tYes];
                                    View.Bezeichnung2.Tag = true;
                                }
                                else
                                {
                                    DataModul.DB_OFBTable.Index = "IndNum";
                                    DataModul.DB_OFBTable.Seek("=", Modul1.Ubg);
                                    if (!DataModul.DB_OFBTable.NoMatch)
                                    {
                                        View.Bezeichnung2.Text = $"{Modul1.IText[EUserText.t176]}: {Modul1.IText[EUserText.tYes]}, in den OFB-Feldern; Auflistung nicht möglich";
                                        View.Bezeichnung2.Tag = true;
                                        View._Bef_4.Enabled = false;
                                        goto end_IL_0001_2;
                                    }
                                    else
                                    {
                                        View.Bezeichnung2.Text = $"{Modul1.IText[EUserText.t176]}: {Modul1.IText[EUserText.tNo]}";
                                        View.Bezeichnung2.Tag = false;
                                    }
                                }
                                break;
                            case ETextKennz.Z_:
                                DataModul.DT_RelgionTable.Index = "T";
                                DataModul.DT_RelgionTable.Seek("=", Modul1.Ubg);
                                if (!DataModul.DT_RelgionTable.NoMatch)
                                {
                                    View.Bezeichnung2.Text = Modul1.IText[EUserText.t176] + ": " + Modul1.IText[EUserText.tYes];
                                    View.Bezeichnung2.Tag = true;
                                    text2 = Modul1.IText[EUserText.t177] + DataModul.DT_RelgionTable.Fields["PerNr"].AsInt().AsString() + " " + Modul1.IText[EUserText.t177] + DataModul.DT_RelgionTable.Fields["PerNr"].AsInt().AsString();
                                }
                                else
                                {
                                    View.Bezeichnung2.Text = Modul1.IText[EUserText.t176] + ": " + Modul1.IText[EUserText.tNo];
                                    View.Bezeichnung2.Tag = false;
                                }
                                break;
                            case ETextKennz.T_:
                            case ETextKennz.tk3_:
                                lErl = 3;
                                (EventIndex eIndex, EventFields eIndexField) = eTKennz switch
                                {
                                    ETextKennz.T_ => (EventIndex.NText, EventFields.ArtText),
                                    ETextKennz.tk3_ => (EventIndex.CText, EventFields.Causal),
                                    _ => (EventIndex.PText, EventFields.Platz)
                                };

                                DataModul.Event.UpdateClearPred(eIndex, eIndexField, Modul1.Ubg, (cEv) => eIndex != EventIndex.NText
                                || (cEv.eArt != EEventArt.eA_105
                                && cEv.eArt != EEventArt.eA_603));
                                if (!DataModul.Event.ReadData(eIndex, Modul1.Ubg, out cEvent))
                                {
                                    Artt = Artsch(cEvent);
                                    View.Bezeichnung2.Text = Modul1.IText[EUserText.t176] + ": " + Modul1.IText[EUserText.tYes];
                                    View.Bezeichnung2.Tag = true;
                                }
                                else
                                {
                                    View.Bezeichnung2.Text = Modul1.IText[EUserText.t176] + ": " + Modul1.IText[EUserText.tNo];
                                    View.Bezeichnung2.Tag = false;
                                }
                                break;
                            case ETextKennz.tk5_:
                                if (DataModul.Event.Seek(EventIndex.HaNu, Modul1.Ubg) != null)
                                {
                                    View.Bezeichnung2.Text = Modul1.IText[EUserText.t176] + ": " + Modul1.IText[EUserText.tYes];
                                    View.Bezeichnung2.Tag = true;
                                }
                                else
                                {
                                    View.Bezeichnung2.Text = Modul1.IText[EUserText.t176] + ": " + Modul1.IText[EUserText.tNo];
                                    View.Bezeichnung2.Tag = false;
                                }
                                break;
                            case ETextKennz.tk7_:
                                DataModul.DB_PersonTable.Index = nameof(PersonIndex.reli);
                                DataModul.DB_PersonTable.Seek("=", Modul1.Ubg);
                                if (!DataModul.DB_PersonTable.NoMatch)
                                {
                                    View.Bezeichnung2.Text = Modul1.IText[EUserText.t176] + ": " + Modul1.IText[EUserText.tYes];
                                    View.Bezeichnung2.Tag = true;
                                }
                                else
                                {
                                    View.Bezeichnung2.Text = Modul1.IText[EUserText.t176] + ": " + Modul1.IText[EUserText.tNo];
                                    View.Bezeichnung2.Tag = false;
                                }
                                break;
                            default:
                                break;
                        }
                        _ = View.Text1.Focus();
                        if (View.Text3.Text.Trim() != "")
                        {
                            View._Check2_1.Visible = true;
                        }
                        if (!View.Bezeichnung2.Tag.AsBool())
                        {
                            goto end_IL_0001_2;
                        }
                        switch (eTKennz)
                        {
                            case ETextKennz.E_:
                            case ETextKennz.G_:
                            case ETextKennz.O_:
                                View.btnMoveToEntityAnot.Visible = true;
                                break;
                            case ETextKennz.M_:
                                View_ShowAllButtons();
                                break;

                            default:
                                View_HideButtons();
                                break;
                        }
                        goto end_IL_0001_2;
                    IL_17f6:
                        num4 = num2 + 1;
                        goto IL_17fa;
                    IL_17fa:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 35:
                            case 118:
                            case 190:
                            case 194:
                            case 202:
                            case 210:
                            case 211:
                            case 212:
                            case 214:
                            case 218:
                            case 219:
                            case 229:
                            case 230:
                            case 231:
                                goto end_IL_0001_2;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 7076;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001_2: // <========== 6
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    private void View_ShowAllButtons()
    {
        View.btnDeleteEntry.Visible = true;
        View.btnMoveToDateAnot.Visible = true;
        View.btnMoveToCause.Visible = true;
        View.btnMoveToChurchCemet.Visible = true;
        View.btnMoveToEntityAnot.Visible = true;
        View.btnMoveToLowerDateAnot.Visible = true;
    }

    public void Text1_TextChanged(object eventSender, EventArgs eventArgs)
    {
        if (tTextBez.eTKnz == ETextKennz.Z_ && View.Text1.Text.Trim().Length > DataModul.DB_PersonTable.Fields[PersonFields.Konv].Size)
        {
            string text = "Der Text für die Religion darf bei diesem Mandanten max. " + DataModul.DB_PersonTable.Fields[PersonFields.Konv].Size.AsString() + " Zeichen lang sein\n";
            text += "In einem neuen Mandanten kann die Länge bis zu 240 Zeichen betragen!";
            _ = Interaction.MsgBox(text, title: "", icon: MessageBoxIcon.Error);
        }
    }

    public void Text3_KeyUp(object eventSender, KeyEventArgs eventArgs)
    {
        checked
        {
            short num = (short)eventArgs.KeyCode;
            short num2 = (short)unchecked((int)eventArgs.KeyData / 65536);
            if (num == 13)
            {
                View.List3.Visible = false;
                return;
            }
            float num3 = 0f;
            if (View.Text3.Text.Trim() == "")
            {
                return;
            }
            View.List3.Visible = true;
            View.List3.Items.Clear();
            DataModul.DB_TexteTable.Index = nameof(TexteIndex.LTexte);
            DataModul.DB_TexteTable.Seek(">=", View.Text3.Text, Modul1.eTKennz);
            string right = "";
            while (!DataModul.DB_TexteTable.EOF && !DataModul.DB_TexteTable.NoMatch)
            {
                if (DataModul.DB_TexteTable.Fields[TexteFields.Leitname].AsString() != right
                    && DataModul.DB_TexteTable.Fields[TexteFields.Kennz].AsEnum<ETextKennz>() == Modul1.eTKennz)
                {
                    num3 += 1f;
                    _ = View.List3.Items.Add(Strings.Replace(DataModul.DB_TexteTable.Fields[TexteFields.Leitname].AsString(), "ssss", "ß"));
                    right = DataModul.DB_TexteTable.Fields[TexteFields.Leitname].AsString();
                }
                DataModul.DB_TexteTable.MoveNext();
                if (num3 >= 50f)
                {
                    break;
                }
            }
        }
    }

    public void Listrein()
    {
    }

    public static string Artsch(IEventData cEv) => cEv.eArt switch
    {
        EEventArt.eA_Birth => " Geburt",
        EEventArt.eA_Baptism => " Taufe",
        EEventArt.eA_Death => " Tod",
        EEventArt.eA_Burial => " Begraben",
        EEventArt.eA_201 => " Taufe HLT",
        EEventArt.eA_202 => " Endowment HLT",
        EEventArt.eA_203 => " Siegel Kind an Eltern HLT",
        EEventArt.eA_300 => " Beruf",
        EEventArt.eA_301 => " Titel",
        EEventArt.eA_302 => " Wohnort",
        EEventArt.eA_303 => " Civil data",
        EEventArt.eA_Marriage => " Heirat",
        EEventArt.eA_MarrReligious => " Kirchl. Heirat",
        EEventArt.eA_504 => " Scheidung",
        EEventArt.eA_510 => " Siegel Frau an Mann HLT",
        _ => " Unbekannt",
    };

    public void listPerles()
    {
        //Discarded unreachable code: IL_0429, IL_079c
        int try0001_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        string text = default;
        int lErl = default;
        int num6 = default;
        string text2 = default;
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
                        case 2405:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_07cb;
                                    default:
                                        goto end_IL_0001;
                                }
                                Debugger.Break();
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                                goto IL_07cf;
                            }
                        end_IL_0001:
                            break;
                        IL_0009:
                            num = 2;
                            num6 = View.Sortbox1.Items.Count - 1;
                            M1_Iter = 0;
                            goto IL_06d8;
                        IL_00d6: // <========== 4
                            num = 17;
                            lErl = 3;
                            Modul1.PersInArb = (int)Math.Round(View.Sortbox1.Text.Left(10).AsDouble());
                            if (AltPer != Modul1.PersInArb)
                            {
                                AltPer = Modul1.PersInArb;
                                if (Modul1.PersInArb > 0)
                                {
                                    var pt = DataModul.Person.Seek(Modul1.PersInArb);
                                    Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                    var sPrefix = Modul1.Person.Prefix != "" ? Modul1.Person.Prefix + " " : "";
                                    var sSuffix = Modul1.Person.Suffix != "" ? Modul1.Person.Suffix + " " : "";
                                    var sGivennames = Modul1.Person.Givennames.Replace("\"", "");
                                    text = " " + Modul1.Person.SurName + " " + sPrefix + sGivennames + sSuffix + Modul1.Person.Alias + Modul1.Person.Clan;
                                    text = View.Sortbox1.Text.Length > 10
                                        ? text + new string(' ', 240).Left(63) + "           " + Modul1.PersInArb.AsString().Right(10)
                                        : text + new string(' ', 240).Left(63) + "           " + Modul1.PersInArb.AsString().Right(10);
                                    _ = List1_Items.Add(text);
                                    text = "";
                                }
                            }
                            goto IL_06c0;
                        IL_06c0: // <========== 4
                            num = 83;
                            lErl = 5;
                            M1_Iter++;
                            goto IL_06d8;
                        IL_06d8:
                            if (M1_Iter <= num6)
                            {
                                View.Sortbox1.SelectedIndex = M1_Iter;
                                if (View.Sortbox1.Items[M1_Iter].AsString().Length > 10)
                                {
                                    if (!(Strings.Mid(View.Sortbox1.Text, 11, 10).AsDouble() > 499.0))
                                    {
                                        goto IL_00d6;
                                    }
                                    else
                                    {
                                        lErl = 4;
                                        Modul1.FamInArb = (int)Math.Round(View.Sortbox1.Text.Left(10).AsDouble());
                                        DataModul.Link.ReadFamily(Modul1.FamInArb, Modul1.Family);
                                        text = "";
                                        if (Modul1.Family.Mann > 0)
                                        {
                                            Modul1.PersInArb = Modul1.Family.Mann;
                                            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                            var sPrefix = Modul1.Person.Prefix != "" ? Modul1.Person.Prefix + " " : "";
                                            text = "F" + text.TrimEnd() + View.Sortbox1.Text.Left(10) + " " + Modul1.Person.Givennames + " " + sPrefix + Modul1.Person.SurName + " / ";
                                        }
                                        else
                                        {
                                            text = "F" + text.TrimEnd() + View.Sortbox1.Text.Left(10) + " " + Modul1.IText[EUserText.tUnknown] + " / ";
                                        }
                                        if (Modul1.Family.Frau > 0)
                                        {
                                            Modul1.PersInArb = Modul1.Family.Frau;
                                            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                            var sPrefix = Modul1.Person.Prefix != "" ? Modul1.Person.Prefix + " " : "";
                                            text = text.TrimEnd() + Modul1.Person.Givennames + " " + sPrefix + Modul1.Person.SurName;
                                        }
                                        else
                                        {
                                            text = text.TrimEnd() + Modul1.IText[EUserText.tUnknown];
                                        }
                                        _ = List1_Items.Add(text);
                                        text = "";
                                    }
                                    goto IL_06c0;
                                }
                                goto IL_00d6;
                            }
                            else
                            {
                                View.Sortbox1.Items.Clear();
                                text2 = "Einträge:\n" + List1_Items.Count.AsString();
                                if (Abbr == 1f)
                                {
                                    text2 += "\nTeilmenge!! In der gewählten Einstellung können nicht alle Einträge dargestellt werden.";
                                }
                                if (Abbr == 2f)
                                {
                                    text2 += "\nTeilmenge!! In der gewählten Einstellung können nicht alle Einträge dargestellt werden.";
                                    text2 += "\nEine vollständige Anzeige erhalten Sie in der Personenmaske unter Recherche";
                                }
                                View.Label4.Text = text2;
                                goto end_IL_0001_2;
                            }
                        IL_07cb:
                            num4 = unchecked(num2 + 1);
                            goto IL_07cf;
                        IL_07cf:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 8:
                                case 11:
                                case 12:
                                case 15:
                                case 16:
                                    num = 16;
                                    _ = Interaction.MsgBox(View.Liste1.Text.Left(19));
                                    goto IL_00d6;
                                case 10:
                                case 14:
                                case 17:
                                    goto IL_00d6;

                                case 52:
                                case 53:
                                case 54:
                                case 83:
                                    goto IL_06c0;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 2405;
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

    public void Indber()
    {
        int num = Liste1_Items.Count - 1;
        int num2 = 0;
        int Ortnr;
        while (num <= num2
            && (Ortnr = Liste1_Items[num2].ItemData<int>()) != 0
            && DataModul.Place.ReadData(Ortnr, out var cPlace))
        {
            Modul1.UbgT = DataModul.Place.FullName(cPlace);

            OrtSTable_SetData(Ortnr, DataModul.Place.FullName(cPlace, true, false));
            num2++;
        }
    }

    private static void OrtSTable_SetData(int iOrtNr, string sPLaceName)
    {
        IRecordset ortSTable = DataModul.DOSB_OrtSTable;
        ortSTable.Index = "OrtNr";
        ortSTable.Seek("=", iOrtNr);
        if (ortSTable.NoMatch)
        {
            ortSTable.AddNew();
        }
        else
        {
            ortSTable.Edit();
        }
        ortSTable.Fields["Name"].Value = sPLaceName;
        ortSTable.Update();
    }

    public void btnShowAsocPeople_Click(object sender, EventArgs e)
    {
        View.btnShowAsocPeople.Enabled = false;
        List4_Items.Clear();
        checked
        {
            int num = List1_Items.Count - 1;
            int num2 = 0;
            while (true)
            {
                int num3 = num2;
                int num4 = num;
                if (num3 > num4)
                {
                    break;
                }
                View.List1.SelectedIndex = num2;
                _ = List4_Items.Add(List1_Items.ItemData(View.List1.SelectedIndex));
                num2++;
            }
            List1_Items.Clear();
            int num5 = List4_Items.Count - 1;
            int M1_Iter = 0;
            while (true)
            {
                int i = M1_Iter;
                int num4 = num5;
                if (i > num4)
                {
                    break;
                }
                Nr = View.List4.Items[M1_Iter].ItemData<int>();
                DataModul.Event.ForEachDo(EventIndex.EOrt, EventFields.Ort, Nr, (cEv) =>
                {
                    _ = View.Sortbox1.Items.Add(new ListItem($"{cEv.iPerFamNr,10}{cEv.eArt.AsInt(),10}", (cEv.iPerFamNr, cEv.eArt)));
                    if (View.Sortbox1.Items.Count <= 32500)
                        return true;
                    Abbr = 2f;
                    return false;
                });
                M1_Iter++;
            }
            listPerles();
        }
    }

    public void Bezeichnung2_TextChanged(object sender, EventArgs e)
    {
        View._Bef_3.Enabled = !View.Bezeichnung2.Tag.AsBool();
    }

    public void Text4_KeyUp(object sender, KeyEventArgs e)
    {
        View.Text2.Text = "";
    }

    public void Text2_KeyUp(object sender, KeyEventArgs e)
    {
        Text4_Text = "";
    }

    public void btnReenter_Click(object sender, EventArgs e)
    {
        var Modul1_UbgT = View.Text1.Text;
        if (Modul1.UbgT == "")
        {
            return;
        }
        int iSatz = DataModul.Texte_Schreib(Modul1_UbgT, Modul1.UbgT1, ETextKennz.Q_);
        if (!View.Bezeichnung2.Tag.AsBool())
        {
            return;
        }
        int ubg = iSatz;
        DataModul.Event.UpdateAllSetValPred(EventIndex.EOrt, EventFields.Ort, Nr, EventFields.KBem, ubg, (cEv) => cEv.eArt == EEventArt.eA_302, 0x3DD4);
        View.Text1.Text = "";
    }

    public void Label7_Click(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_0da4
        int try0001_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        (string, ETextKennz) bezeichnu = default;
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
                    string text;
                    string B;
                    switch (try0001_dispatch)
                    {
                        default:
                            num = 1;
                            View.btnMoveToDateAnot.Visible = false;
                            goto IL_0011;
                        case 4485:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_0e73;
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
                                    goto IL_0e73;
                                }
                                else
                                {
                                    Modul1.UbgT = "Texte Bez_klick";
                                    _ = Interaction.MsgBox("Fehler " + Information.Err().Number.AsString() + " in " + Modul1.UbgT);
                                    _ = Interaction.MsgBox("F120");
                                    Debugger.Break();
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    num4 = num2;
                                    goto IL_0e77;
                                }
                            }
                        end_IL_0001:
                            break;
                        IL_0011:
                            num = 2;
                            View_HideButtons();
                            View.Command3.Visible = false;
                            View.btnMoveNameToAlias.Visible = false;
                            View_HideText3_Label1_1();
                            View._Check2_0.Visible = false;

                            ProjectData.ClearProjectError();

                            num3 = 2;
                            View._Bef_4.Visible = true;
                            View.Text3.Text = "";
                            bezeichnu = ("", ETextKennz.tkNone);
                            Txknz = ETextKennz.tkNone;
                            View.Bezeichnung6.Text = "";
                            string name = ((Label)sender).Name;
                            if (name == nameof(View.Label7))
                            {
                                Txknz = ETextKennz.tkName;
                                View.btnMoveNameToAlias.Visible = true;
                                bezeichnu = (Modul1.IText[EUserText.t180] + new string(' ', 100), Txknz);
                                View_ShowText3_Label1_1();
                            }
                            else if (name == nameof(View.Label8))
                            {
                                Txknz = ETextKennz.F_;
                                bezeichnu = (Modul1.IText[EUserText.t181] + new string(' ', 100), Txknz);
                                View_ShowText3_Label1_1();
                            }
                            else if (name == nameof(View.Label9))
                            {
                                Txknz = ETextKennz.V_;
                                bezeichnu = (Modul1.IText[EUserText.t182] + new string(' ', 100), Txknz);
                                View_ShowText3_Label1_1();
                            }
                            else if (name == nameof(View.Label10))
                            {
                                Txknz = ETextKennz.A_;
                                bezeichnu = ("Zusatz vor dem Namen" + new string(' ', 100), Txknz);
                                View._Check2_0.Visible = true;
                            }
                            else if (name == nameof(View.Label11))
                            {
                                Txknz = ETextKennz.B_;
                                bezeichnu = (Modul1.IText[EUserText.t195] + new string(' ', 100), Txknz);
                                View_ShowText3_Label1_1();
                            }
                            else if (name == nameof(View.Label12))
                            {
                                Txknz = ETextKennz.C_;
                                bezeichnu = (Modul1.IText[EUserText.tAlias] + new string(' ', 100), Txknz);
                            }
                            else if (name == nameof(View.Label13))
                            {
                                Txknz = ETextKennz.D_;
                                bezeichnu = (Modul1.IText[EUserText.t126] + new string(' ', 100), Txknz);
                                View._Check2_0.Visible = true;
                            }
                            else if (name == nameof(View.Label14))
                            {
                                Txknz = ETextKennz.E_;
                                bezeichnu = (Modul1.IText[EUserText.tTitle] + new string(' ', 100), Txknz);
                                View_ShowText3_Label1_1();
                            }
                            else if (name == nameof(View.Label15))
                            {
                                Txknz = ETextKennz.G_;
                                bezeichnu = (Modul1.IText[EUserText.t70] + new string(' ', 100), Txknz);
                                View_ShowText3_Label1_1();
                            }
                            else if (name == nameof(View.Label16))
                            {
                                Txknz = ETextKennz.M_;
                                FileSystem.FileClose(99);
                                FileSystem.FileOpen(99, Path.Combine(Modul1.GenFreeDir, "Adresse"), OpenMode.Append);
                                FileSystem.FileClose(99);
                                FileSystem.FileOpen(99, Path.Combine(Modul1.GenFreeDir, "Adresse"), OpenMode.Input);
                                text = "";
                                M1_Iter = 0;
                                goto IL_066f;
                            }
                            else if (name == nameof(View.Label17))
                            {
                                Txknz = ETextKennz.tk2_;
                                bezeichnu = ("Prädikat" + new string(' ', 100), Txknz);
                            }
                            else if (name == nameof(View.Label18))
                            {
                                Txknz = ETextKennz.T_;
                                bezeichnu = ("Ereignisbezeichnungen" + new string(' ', 100), Txknz);
                            }
                            else if (name == nameof(View.Label19))
                            {
                                Txknz = ETextKennz.H_;
                                bezeichnu = (Modul1.IText[EUserText.t99_Places] + new string(' ', 100), Txknz);
                            }
                            else if (name == nameof(View.Label20))
                            {
                                Txknz = ETextKennz.I_;
                                bezeichnu = (Modul1.IText[EUserText.t188] + new string(' ', 100), Txknz);
                            }
                            else if (name == nameof(View.Label21))
                            {
                                Txknz = ETextKennz.J_;
                                bezeichnu = (Modul1.IText[EUserText.t189] + new string(' ', 100), Txknz);
                            }
                            else if (name == nameof(View.Label22))
                            {
                                Txknz = ETextKennz.K_;
                                bezeichnu = (Modul1.IText[EUserText.t190] + new string(' ', 100), Txknz);
                            }
                            else if (name == nameof(View.Label23))
                            {
                                Txknz = ETextKennz.L_;
                                bezeichnu = (Modul1.IText[EUserText.t191] + new string(' ', 100), Txknz);
                            }
                            else if (name == nameof(View.Label24))
                            {
                                Txknz = ETextKennz.Q_;
                                bezeichnu = (Modul1.IText[EUserText.t75] + new string(' ', 100), Txknz);
                            }
                            else if (name == nameof(View.Label25))
                            {
                                Txknz = ETextKennz.O_;
                                bezeichnu = (Modul1.IText[EUserText.t194] + new string(' ', 100), Txknz);
                            }
                            else if (name == nameof(View.Label26))
                            {
                                Txknz = ETextKennz.U_;
                                View._Check2_0.Visible = true;
                                bezeichnu = ("Statusbezeichnungen" + new string(' ', 100), Txknz);
                            }
                            else if (name == nameof(View.Label27))
                            {
                                View_ShowText3_Label1_1();
                                Txknz = ETextKennz.tk7_;
                                bezeichnu = ("Konfessionen" + new string(' ', 100), Txknz);
                            }
                            else if (name == nameof(View.Label28))
                            {
                                View_ShowText3_Label1_1();
                                Txknz = ETextKennz.tk3_;
                                bezeichnu = ("Todesursache" + new string(' ', 100), Txknz);
                            }
                            else if (name == nameof(View.Label29))
                            {
                                View_HideText3_Label1_1();
                                Txknz = ETextKennz.tk5_;
                                bezeichnu = ("Hausnummer" + new string(' ', 100), Txknz);
                            }
                            goto IL_0b89;
                        IL_066f: // <========== 3
                            num = 85;
                            if (!FileSystem.EOF(99))
                            {
                                text = FileSystem.LineInput(99);
                                M1_Iter++;
                                if (M1_Iter != 9)
                                {
                                    goto IL_066f;
                                }
                                else
                                {
                                    goto IL_0683;
                                }
                            }
                            else
                            {
                                goto IL_0683;
                            }
                        IL_0683: // <========== 3
                            num = 92;
                            if (M1_Iter != 9)
                            {
                            }
                            Txknz = ETextKennz.M_;
                            bezeichnu = (Modul1.IText[EUserText.t186] + new string(' ', 100), Txknz);
                            goto IL_0b89;
                        IL_0b89: // <========== 3
                            num = 155;
                            View.Cursor = Cursors.Arrow;
                            View.Label6.Visible = false;
                            Application.DoEvents();
                            M_Bezeichnu = bezeichnu;
                            View._Bezeichnung1_0.Text = "";
                            View._Bezeichnung1_0.Tag = 0;
                            View.Bezeichnung2.Text = "";
                            Update_View();
                            goto end_IL_0001_2;
                        IL_0e73:
                            num4 = unchecked(num2 + 1);
                            goto IL_0e77;
                        IL_0e77:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 84:
                                case 85:
                                case 90:
                                case 91:
                                    goto IL_066f;
                                case 89:
                                case 92:
                                    goto IL_0683;
                                case 23:
                                case 31:
                                case 37:
                                case 43:
                                case 48:
                                case 54:
                                case 58:
                                case 63:
                                case 69:
                                case 75:
                                case 95:
                                case 99:
                                case 103:
                                case 107:
                                case 111:
                                case 115:
                                case 119:
                                case 123:
                                case 127:
                                case 131:
                                case 136:
                                case 142:
                                case 148:
                                case 154:
                                case 155:
                                    goto IL_0b89;
                                case 180:
                                case 181:
                                case 182:
                                case 192:
                                    goto end_IL_0001_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0001_dispatch = 4485;
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

    private void View_HideText3_Label1_1()
    {
        View.Text3.Visible = false;
        View._Label1_1.Visible = false;
    }

    private void View_ShowText3_Label1_1()
    {
        View.Text3.Visible = true;
        View._Label1_1.Visible = true;
    }

    public void btnMoveToCause_Click(object sender, EventArgs e)
    {
        DataModul.wrkDefault.Begin();
        int ubg = View.Label5.Tag.AsInt();
        if (DataModul.Event.ExistsPred(EventIndex.KText, EventFields.KBem, ubg,
            (cEv) => cEv.eArt != EEventArt.eA_Death))
        {
            _ = Interaction.MsgBox("Text wird nicht nur bei >Tod< verwendet.\n Verschieben nicht möglich");
            DataModul.wrkDefault.Rollback();
            return;
        }
        try
        {
            DataModul.Event.UpdateAllMvVal(EventIndex.KText, EventFields.KBem, ubg, EventFields.Causal);
            string ubgT = View.Text1.Text;
            if (ubgT.Trim() != "")
            {
                ETextKennz eTKennz = ETextKennz.tk3_;
                DataModul_Texte_SetKennz(ubg, eTKennz);
            }
            DataModul.wrkDefault.Commit();
        }
        catch (Exception _e)
        {
            Handle_Err(_e);
        }

        Update_View();
    end_IL_0001_2:
        return;

        void Handle_Err(Exception e)
        {
            string text = e.Message;

            if (true)
            {
                text = "Der Text ist unter Todesursachen schon vorhanden.\nÄndern Sie den Text unter Todesursachen z.B. durch vorsetzten eines >A<.";
                text += "\nVerschieben Sie dann diesen Text und machen die Änderung unter Todesursache wieder rückgängig.";
                DataModul.wrkDefault.Rollback();
                _ = Interaction.MsgBox(text, title: "Verschieben nicht möglich", mb: MessageBoxButtons.OK);
                Clear_Text1_Liste1();
            }
            else
            {
                DataModul.wrkDefault.Rollback();
                _ = Interaction.MsgBox(text, title: "Verschieben nicht möglich", mb: MessageBoxButtons.OK);
                Clear_Text1_Liste1();
            }

        }
    }

    private void Update_View()
    {
        Clear_Text1_Liste1();
        View.Command3.Visible = false;
        View.Liste1.Visible = true;
        if (View.Text2.Text != "")
        {
            Modul1.STextles("TL5", tTextBez.eTKnz, View.Text2.Text.ToUpper(), Liste1_Items);
            if (List1_Items.Count > 0)
                tTextBez = M_Bezeichnu;
        }
        else
        {
            Modul1.KTextlesTL5(tTextBez.eTKnz, List1_Items, M_Bezeichnu);
        }
        if (!View.Check1.Checked)
        {
            View.Text2.Text = "";
            View.Text4.Text = "";
        }
        if (Liste1_Items.Count > 0
            && Modul1.Typ != DriveType.CDRom)
        {
            View.Command3.Enabled = true;
            View.Command3.Visible = true;
        }
    }

    private static void DataModul_Texte_SetKennz(int ubg, ETextKennz eTKennz)
    {
        DataModul.DB_TexteTable.Index = nameof(TexteIndex.TxNr1);
        DataModul.DB_TexteTable.Seek("=", ubg);
        DataModul.DB_TexteTable.Edit();
        DataModul.DB_TexteTable.Fields[TexteFields.Kennz].Value = (char)eTKennz;
        DataModul.DB_TexteTable.Update();
    }

    [RelayCommand]
    private void ChangeSexToF()
    {
        foreach (var itms in List1_Items)
        {
            if (DataModul.Person.ReadData(itms.ItemData<int>(), out var personData))
            {
                personData.SetSex("F");
                DataModul.Person.SetData(personData.ID, personData);
            }
        }
        Debugger.Break();
        _ = Interaction.MsgBox(Nr.AsString());
        _ = Interaction.MsgBox("Fertig");
        View._Command1_0.PerformClick();

    }

    [RelayCommand]
    private void ChangeSexToM()
    {
        foreach (var itms in List1_Items)
        {
            if (DataModul.Person.ReadData(itms.ItemData<int>(), out var personData))
            {
                personData.SetSex("M");
                DataModul.Person.SetData(personData.ID, personData);
            }
        }
        Debugger.Break();
        _ = Interaction.MsgBox("Fertig");
        View._Command1_0.PerformClick();
    }

    [RelayCommand]
    private void MoveToChurchCemet()
    {
        //Discarded unreachable code: IL_0431, IL_049c, IL_04dc
        int try0001_dispatch = -1;
        int num2 = default;
        int num = default;
        string text = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;

                switch (try0001_dispatch)
                {
                    default:
                        ProjectData.ClearProjectError();
                        num2 = 2;
                        int iIndexVal = View.Label5.Tag.AsInt();
                        DataModul.wrkDefault.Begin();
                        DataModul.Event.UpdateAllMvVal(EventIndex.KText, EventFields.KBem, iIndexVal, EventFields.Platz);
                        if (View.Text1.Text.Trim() != "")
                        {
                            DataModul_Texte_SetKennz(iIndexVal, ETextKennz.O_);
                        }
                        DataModul.wrkDefault.Commit();
                        Update_View();
                        goto end_IL_0001;
                    case 1247:
                        num = -1;
                        switch (num2)
                        {
                            case 2:
                                if (Information.Err().Number == 3022)
                                {
                                    text = "Der Text ist unter Kirche/Friedhof schon vorhanden.\nÄndern Sie den Text unter Kirche/Friedho z.B. durch vorsetzten eines >A<.";
                                    text += "\nVerschieben Sie dann diesen Text und machen die Änderung unter Kirche/Friedhof wieder rückgängig.";
                                    DataModul.wrkDefault.Rollback();
                                    _ = Interaction.MsgBox(text, title: "Verschieben nicht möglich", mb: MessageBoxButtons.OK);
                                    Clear_Text1_Liste1();
                                }
                                else
                                {
                                    DataModul.wrkDefault.Rollback();
                                    _ = Interaction.MsgBox(text, title: "Verschieben nicht möglich", mb: MessageBoxButtons.OK);
                                    Clear_Text1_Liste1();
                                }
                                goto end_IL_0001;
                        }
                        break;
                }
                goto IL_0519;
            end_IL_0001:
                ;
            }
            catch (Exception obj) when (obj is not null && num2 != 0 && num == 0)
            {
                ProjectData.SetProjectError(obj);
                try0001_dispatch = 1247;
                continue;
            }
            break;
        IL_0519:
            throw ProjectData.CreateProjectError(-2146828237);
        }
        if (num != 0)
        {
            ProjectData.ClearProjectError();
        }
    }
    [RelayCommand]
    private void MoveToEntityAnot()
    {
        //Discarded unreachable code: IL_0544, IL_05af, IL_05ef
        int try0001_dispatch = -1;
        int num2 = default;
        string text = default;
        int num = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;

                switch (try0001_dispatch)
                {
                    default:
                        ProjectData.ClearProjectError();
                        num2 = 2;
                        DataModul.wrkDefault.Begin();
                        int iTextNr = View.Label5.Tag.AsInt();
                        switch (Modul1.eTKennz) //??
                        {
                            case ETextKennz.E_:
                            case ETextKennz.G_:
                            case ETextKennz.M_:
                            case ETextKennz.O_:
                                {
                                    Event_UpdateAllAct(EventIndex.KText, EventFields.KBem, iTextNr, (cEv) =>
                                    {
                                        if (cEv.eArt < EEventArt.eA_500)
                                            Person_UpdateTextAppend(cEv.iPerFamNr, View.Text1.Text);
                                        else
                                            Family_UpdateTextAppend(cEv.iPerFamNr, View.Text1.Text);
                                    });
                                    text = "Fertig\nMit OK werden die Änderungen übernommen,";
                                    text += "\nmit Abbrechen werden die Änderungen verworfen und Ihre Datei bleibt unverändert ";
                                    byte b = checked((byte)Interaction.MsgBox(text, title: "Texte verschieben", mb: MessageBoxButtons.OKCancel));
                                    if (b == 1)
                                    {
                                        DataModul.wrkDefault.Commit();
                                    }
                                    else
                                    {
                                        DataModul.wrkDefault.Rollback();
                                    }
                                    break;
                                }
                        }
                        goto end_IL_0001;
                    case 1522:
                        num = -1;
                        switch (num2)
                        {
                            case 2:
                                if (Information.Err().Number == 3022)
                                {
                                    text = "Der Text ist unter Todesursachen schon vorhanden.\nÄndern Sie den Text unter Todesursachen z.B. durch vorsetzten eines >A<.";
                                    text += "\nVerschieben Sie dann diesen Text und machen die Änderung unter Todesursache wieder rückgängig.";
                                    DataModul.wrkDefault.Rollback();
                                    _ = Interaction.MsgBox(text, title: "Verschieben nicht möglich", mb: MessageBoxButtons.OK);
                                    Clear_Text1_Liste1();
                                }
                                else
                                {
                                    DataModul.wrkDefault.Rollback();
                                    _ = Interaction.MsgBox(text, title: "Verschieben nicht möglich", mb: MessageBoxButtons.OK);
                                    Clear_Text1_Liste1();
                                }
                                goto end_IL_0001;
                        }
                        break;
                }
            }
            catch (Exception obj) when (obj is not null && num2 != 0 && num == 0)
            {
                ProjectData.SetProjectError(obj);
                try0001_dispatch = 1522;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001:
            break;
        }
        if (num != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    private static void Event_UpdateAllAct(EventIndex eIndex, EventFields eIndexField, int iIndexVal, Action<IEventData> action)
    {
        IRecordset dB_EventTable = DataModul.Event.Seek(eIndex, iIndexVal);
        while (dB_EventTable?.NoMatch == false
            && !dB_EventTable.EOF
            && dB_EventTable.Fields[$"{eIndexField}"].AsInt() == iIndexVal)
        {
            action(new CEventData(dB_EventTable));

            dB_EventTable.Edit();
            dB_EventTable.Fields[$"{eIndexField}"].Value = 0;
            dB_EventTable.Update();

            dB_EventTable.MoveNext();
        }
    }

    private static void Family_UpdateTextAppend(int iFamNr, string sNewText)
    {
        IRecordset dB_FamilyTable = DataModul.Family.Seek(iFamNr);
        dB_FamilyTable.Index = nameof(FamilyIndex.Fam);
        dB_FamilyTable.Seek("=", iFamNr);
        dB_FamilyTable.Edit();
        IField field = dB_FamilyTable.Fields[FamilyFields.Bem1];
        field.Value = field.AsString().Trim() == ""
            ? sNewText
            : (object)(field.Value + " " + sNewText);
        dB_FamilyTable.Update();
    }

    private static void Person_UpdateTextAppend(int iPersNr, string sNewText)
    {
        IRecordset dB_PersonTable = DataModul.Person.Seek(iPersNr);
        dB_PersonTable.Edit();
        IField field = dB_PersonTable.Fields[PersonFields.Bem1];
        field.Value = field.AsString().Trim() == ""
            ? sNewText
            : (object)(field.Value + " " + sNewText);
        dB_PersonTable.Update();
    }

    public void btnMoveToLowerDateAnot_Click(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_047e, IL_04e9, IL_0529
        int try0001_dispatch = -1;
        int num2 = default;
        string text = default;
        int num = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;

                switch (try0001_dispatch)
                {
                    default:
                        {
                            ProjectData.ClearProjectError();
                            num2 = 2;
                            DataModul.wrkDefault.Begin();
                            int ubg = View.Label5.Tag.AsInt();
                            if (Txknz == ETextKennz.O_)
                            {
                                DataModul.Event.UpdateAllMvAppend(EventIndex.PText, EventFields.Platz, ubg, EventFields.Bem2, View.Text1.Text);
                            }
                            else
                            {
                                DataModul.Event.UpdateAllMvAppend(EventIndex.KText, EventFields.KBem, ubg, EventFields.Bem2, View.Text1.Text);
                            }
                            text = "Fertig\nMit OK werden die Änderungen übernommen,";
                            text += "\nmit Abbrechen werden die Änderungen verworfen und Ihre Datei bleibt unverändert ";
                            byte b = checked((byte)Interaction.MsgBox(text, title: "Texte verschieben", mb: MessageBoxButtons.OKCancel));
                            if (b == 1)
                            {
                                DataModul.wrkDefault.Commit();
                            }
                            else
                            {
                                DataModul.wrkDefault.Rollback();
                            }
                            goto end_IL_0001;
                        }
                    case 1324:
                        num = -1;
                        switch (num2)
                        {
                            case 2:
                                text = HandleDB_Err(text);
                                goto end_IL_0001;
                        }
                        break;
                }
                goto IL_0564;
            end_IL_0001:
                ;
            }
            catch (Exception obj) when (obj is not null && num2 != 0 && num == 0)
            {
                ProjectData.SetProjectError(obj);
                try0001_dispatch = 1324;
                continue;
            }
            break;
        IL_0564:
            throw ProjectData.CreateProjectError(-2146828237);
        }
        if (num != 0)
        {
            ProjectData.ClearProjectError();
        }

        string HandleDB_Err(string text)
        {
            if (Information.Err().Number == 3022)
            {
                text = "Der Text ist unter Todesursachen schon vorhanden.\nÄndern Sie den Text unter Todesursachen z.B. durch vorsetzten eines >A<.";
                text += "\nVerschieben Sie dann diesen Text und machen die Änderung unter Todesursache wieder rückgängig.";
                DataModul.wrkDefault.Rollback();
                _ = Interaction.MsgBox(text, title: "Verschieben nicht möglich", mb: MessageBoxButtons.OK);
                Clear_Text1_Liste1();
            }
            else
            {
                DataModul.wrkDefault.Rollback();
                _ = Interaction.MsgBox(text, title: "Verschieben nicht möglich", mb: MessageBoxButtons.OK);
                Clear_Text1_Liste1();
            }

            return text;
        }
    }

    private void Clear_Text1_Liste1()
    {
        View.Text1.Text = "";
        Liste1_Items.Clear();
    }

    public void btnDeleteEntry_Click(object sender, EventArgs e)
    {
        if (Interaction.MsgBox("Text wirklich bei alle Persoen / Familien löschen?", title: "", mb: MessageBoxButtons.YesNo) != DialogResult.Yes)
        {
            return;
        }
        DataModul.Event.ClearAllRemText(EventIndex.KText, EventFields.KBem, View.Label5.Tag.AsInt());
        View.btnDeleteEntry.Visible = false;
    }

    public void btnMoveToDateAnot_Click(object sender, EventArgs e)
    {
        var adr = Modul1.Persistence.ReadStringMLProg("Adresse", 3);
        ProjectData.ClearProjectError();

        DataModul.wrkDefault.Begin();
        if (Txknz == ETextKennz.O_)
        {
            DataModul.Event.UpdateAllMvAppend(EventIndex.PText, EventFields.Platz, View.Label5.Tag.AsInt(), EventFields.Bem1, View.Text1.Text);
        }
        else
        {
            DataModul.Event.UpdateAllMvAppend(EventIndex.KText, EventFields.KBem, View.Label5.Tag.AsInt(), EventFields.Bem1, View.Text1.Text);
        }

        var text = "Fertig\nMit OK werden die Änderungen übernommen,";
        text += "\nmit Abbrechen werden die Änderungen verworfen und Ihre Datei bleibt unverändert ";
        byte b = (byte)Interaction.MsgBox(text, title: "Texte verschieben", mb: MessageBoxButtons.OKCancel);
        if (b == 1)
        {
            DataModul.wrkDefault.Commit();
        }
        else
        {
            DataModul.wrkDefault.Rollback();
        }
        goto end_IL_0001;
        /*
         * num = -1;
                                    switch (iSatz)
                                    {
                                        case 2:
                                            if (Information.Err().Number == 3022)
                                            {
                                                text = "Der Text ist unter Todesursachen schon vorhanden.\nÄndern Sie den Text unter Todesursachen z.B. durch vorsetzten eines >A<.";
                                                text += "\nVerschieben Sie dann diesen Text und machen die Änderung unter Todesursache wieder rückgängig.";
                                                DataModul.wrkDefault.Rollback();
                                                _ = Interaction.MsgBox(text, mb: MessageBoxButtons.OK, title: "Verschieben nicht möglich");
                                                Text1.Text = "";
                                                Liste1.List6_Items.Clear();
                                            }
                                            else
                                            {
                                                DataModul.wrkDefault.Rollback();
                                                _ = Interaction.MsgBox(text, mb: MessageBoxButtons.OK, title: "Verschieben nicht möglich");
                                                Text1.Text = "";
                                                Liste1.List6_Items.Clear();
                                            }
                                            goto end_IL_0001;
                                    }
                                    break;
                            }
                            goto IL_0617;
                        }*/
    end_IL_0001:
        ;
    }

}
