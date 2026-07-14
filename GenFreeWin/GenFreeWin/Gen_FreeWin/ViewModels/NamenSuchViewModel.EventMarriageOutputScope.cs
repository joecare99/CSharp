using BaseLib.Helper;
using GenFree;
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces;
using GenFree.Interfaces.DB;
using Microsoft.VisualBasic;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Gen_FreeWin.ViewModels;

public partial class NamenSuchViewModel
{
    public void Berufe(EEventArt eArt, IDocument rtbAnz)
    {
        int try0001_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        int lErl = default;
        while (true)
        {
            try
            {
                checked
                {
                    int num7;
                    string QuText;
                    switch (try0001_dispatch)
                    {
                        default:
                            num = 1;
                            QuText = "";
                            goto IL_000a;
                        case 14030:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_2f90;
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
                                    goto IL_2f90;
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
                                num7 = num2;
                                goto IL_2f94;
                            }
                        end_IL_0001:
                            break;
                        IL_000a:
                            num = 2;
                            int privaus1 = privaus;
                            int persInArb1 = Modul1.PersInArb;
                            var evtList = List3_Items;

                            string QuText2 = "";
                            Modul1.UbgT1 = "";
                            ProjectData.ClearProjectError();

                            num3 = 2;
                            string sEvent = "";
                            evtList.Clear();
                            EEventArt eEventArt = eArt;
                            int iFamPers = persInArb1;
                            if (eEventArt > EEventArt.eA_601)
                            {
                                iFamPers = Modul1.FamInArb;
                            }

                            foreach (var evt in DataModul.Event.ReadEventsBeSu(iFamPers, eEventArt))
                            {
                                if (evt.iPrivacy <= privaus1 && evt.iLfNr >= 1)
                                {
                                    string sEventDate = evt.dDatumV != default ? evt.dDatumV.AsString() : "";
                                    sEventDate += 0 != evt.iDatumText ? evt.sDatumText.FrameIfNEoW(" (", ")") : "";
                                    string sEventNote = evt.iKBem > 0 ? " " + evt.sKBem.Trim() + " " : " ";
                                    sEventNote += evt.iHausNr > 0 ? evt.sHausNr.Trim() + " " : "";

                                    sEvent = sEventDate + sEventNote + new string(' ', 240).Left(240) + evt.iLfNr.AsString();
                                    if (eEventArt == EEventArt.eA_105 && Option[EOutCfg.o47])
                                    {
                                        sEvent = evt.sArtText + sEvent;
                                    }

                                    evtList.Add(new ListItem<(EEventArt, int, short)>(sEvent, (evt.eArt, evt.iPerFamNr, (short)evt.iLfNr)));
                                }
                            }
                            lErl = 13;

                            if (evtList.Count <= 0)
                            {
                                goto end_IL_0001_2;
                            }
                            var headingDefinition = DocumentComposer.ComposeBerufeHeading(
                                eEventArt,
                                evtList.Count,
                                Modul1.IText[EUserText.tOccupation],
                                Modul1.IText[EUserText.tTitle],
                                Modul1.IText[EUserText.t70],
                                Modul1.IText[EUserText.t444],
                                Modul1.IText[EUserText.t445]);

                            if (headingDefinition != null)
                            {
                                DocumentOutputService.RenderHeading(rtbAnz, headingDefinition);
                                if (headingDefinition.ResetContextUbgT1)
                                {
                                    Modul1.UbgT1 = "";
                                }
                            }

                            short num11 = (short)(evtList.Count - 1);
                            short num10 = 0;
                            while (num10 <= num11)
                            {
                                var lfNr = evtList[num10].ItemData<(int iArt, int iPers, int iLfNr)>().iLfNr;
                                var xBreak = !DataModul.Event.ReadData(((EEventArt)Beruf, iFamPers, (short)lfNr), out var evt);
                                if (xBreak || evt.iLfNr != lfNr)
                                {
                                    _ = Interaction.MsgBox("7: FehlerStop");
                                }
                                var num4 = 0;
                                while (num4++ <= 15)
                                {
                                    Modul1.Kont1[num4] = "";
                                }
                                Datu = default;
                                if (evt.iPerFamNr != iFamPers)
                                {
                                    break;
                                }
                                var sDatu = "";
                                if (evt.dDatumV != default)
                                {
                                    Datu = evt.dDatumV;
                                    sDatu = Datwand1(Datu, evt.sDatumV_S);
                                    Modul1.Kont1[1] = Datu.AsString();
                                }
                                if (evt.dDatumB != default)
                                {
                                    Datu = evt.dDatumB;
                                    sDatu = Datwand1(Datu, evt.sDatumB_S);
                                    if (sDatu.Left(2) == "am")
                                    {
                                        sDatu = Strings.Mid(sDatu, 4, sDatu.Length);
                                    }
                                    if (sDatu != "" && evt.sDatumB_S.Trim() == "")
                                    {
                                        sDatu = " bis " + sDatu;
                                    }
                                    if (Modul1.Kont1[1].Left(2) == "am")
                                    {
                                        Modul1.Kont1[1] = Strings.Mid(Modul1.Kont1[1], 4, Modul1.Kont1[1].Length);
                                    }
                                    if (evt.sDatumV_S.Trim() == "")
                                    {
                                        if (Modul1.Kont1[1].Trim() != "")
                                        {
                                            Modul1.Kont1[1] = Modul1.Kont1[1].Length < 10 ? "von " + Modul1.Kont1[1] : "vom " + Modul1.Kont1[1];
                                        }
                                        Modul1.Kont1[3] = " " + sDatu;
                                    }
                                    if (Modul1.Kont1[3] == "" && sDatu.Trim() != "")
                                    {
                                        Modul1.Kont1[3] = " " + sDatu;
                                    }
                                }
                                Modul1.UbgT = "";
                                if (0 != evt.iDatumText)
                                {
                                    Modul1.Kont1[1] += evt.sDatumText.FrameIfNEoW(" (", ")");
                                }
                                if (eEventArt == EEventArt.eA_105 && Option[EOutCfg.o34] && "" != evt.sReg && evt.sReg.Trim() != "")
                                {
                                    Modul1.Kont1[10] = " (Urk.-Nr.: " + evt.sReg.Trim() + ") ";
                                }
                                if (evt.iKBem > 0)
                                {
                                    Modul1.Kont1[7] = evt.sKBem.FrameIfNEoW(" ");
                                }
                                if (0 != evt.iHausNr)
                                {
                                    Modul1.Kont1[7] += evt.sHausNr.Trim() + " ";
                                }

                                if (eEventArt == EEventArt.eA_603)
                                {
                                    Modul1.Kont[0] = evt.sArtText;
                                }
                                else if (eEventArt == EEventArt.eA_105)
                                {
                                    Anz_AppendNewlineIfNeeded(rtbAnz);
                                    Modul1.Kont[0] = 0 != evt.iArtText ? evt.sArtText : "";
                                    rtbAnz.SetFont(new Font("Arial", 11.01f, FontStyle.Bold));
                                    rtbAnz.AppendText(Modul1.Kont[0].Trim() + ": ");
                                    rtbAnz.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                                }
                                if (eEventArt == EEventArt.eA_106)
                                {
                                    Anz_AppendNewlineIfNeeded(rtbAnz);
                                    rtbAnz.SetFont(new Font("Arial", 11.01f, FontStyle.Bold));
                                    rtbAnz.AppendText("Heimatort/recht: ");
                                    rtbAnz.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                                }
                                if (eEventArt == EEventArt.eA_603)
                                {
                                    Anz_AppendNewlineIfNeeded(rtbAnz);
                                    rtbAnz.SetFont(new Font("Arial", 11.01f, FontStyle.Bold));
                                    rtbAnz.AppendText(Modul1.Person.SurName.Trim() + ": ");
                                    rtbAnz.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                                }
                                if (evt.iOrt > 0)
                                {
                                    Modul1.UbgT = Place_ReadData(evt.iOrt, 0, 0, Option[EOutCfg.o35], evt.sZusatz);
                                    if (evt.sOrt_S != "")
                                    {
                                        Modul1.UbgT = Modul1.UbgT.TrimEnd() + " " + evt.sOrt_S.Trim();
                                    }
                                    Modul1.Kont1[5] = " " + Modul1.UbgT.Trim();
                                    Modul1.UbgT = "";
                                }
                                Modul1.Kont1[4] = evt.sDatumB_S;
                                if (evt.iPlatz > 0)
                                {
                                    Modul1.Kont1[6] = evt.sPlatz.FrameIfNEoW(" ");
                                }
                                Modul1.Kont1[4] = "";
                                if (Option[EOutCfg.o02] && evt.sBem[1] != " ")
                                {
                                    Modul1.Kont1[2] = evt.sBem[1].Trim().FrameIfNEoW("{", "}");
                                }
                                if (Option[EOutCfg.o03] && evt.sBem[2] != " ")
                                {
                                    Modul1.Kont1[4] = evt.sBem[2].Trim().FrameIfNEoW("{", "}");
                                }
                                if (Modul1.Kont1[2].Trim() != "" || Modul1.Kont1[4].Trim() != "")
                                {
                                    QuText = " " + Modul1.Kont1[2].Trim() + " " + Modul1.Kont1[4].Trim();
                                    QuText = Zeiweg(QuText, xStrip: !Option[EOutCfg.o07_KeepFormat]);
                                }
                                sEvent = Jobdreh(sEvent, ereiRf: EreiRf);
                                sEvent = sEvent + " " + QuText;
                                QuText = "";
                                if (Option[EOutCfg.o39])
                                {
                                    DataModul.DB_SourceLinkTable.Index = "Tab22";
                                    DataModul.DB_SourceLinkTable.Seek("=", 3, iFamPers, eEventArt, lfNr);
                                    QuText2 = "";
                                    while (!DataModul.DB_SourceLinkTable.EOF && !DataModul.DB_SourceLinkTable.NoMatch && 0 != DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Art].AsInt())
                                    {
                                        string sSourceLink_3 = DataModul.DB_SourceLinkTable.Fields[3].AsString();
                                        var iSourceLink_0 = DataModul.DB_SourceLinkTable.Fields[0].AsInt();
                                        var iSourceLink_1 = DataModul.DB_SourceLinkTable.Fields[1].AsInt();
                                        string sSourceLink_Aus = DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].AsString();
                                        string sSourceLink_Orig = DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].AsString();
                                        EEventArt eSourceLink_EArt = DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Art].AsEnum<EEventArt>();
                                        int iSourceLink_LfNr = DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.LfNr].AsInt();
                                        string sSourceLink_Kom = DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Kom].AsString();

                                        if (iSourceLink_0 != 3 |
                                            iSourceLink_1 > iFamPers |
                                            iSourceLink_1 > persInArb1 |
                                            eSourceLink_EArt != eEventArt |
                                            iSourceLink_LfNr != lfNr)
                                        {
                                            break;
                                        }

                                        DataModul.DB_QuTable.Index = "NR";
                                        DataModul.DB_QuTable.Seek("=", sSourceLink_3);
                                        if (!DataModul.DB_QuTable.NoMatch)
                                        {
                                            string sQuTable_2 = DataModul.DB_QuTable.Fields[QuFields._2].AsString();

                                            if (QuText2 == "")
                                            {
                                                QuText2 = ". " + Modul1.IText[EUserText.t450] + " ";
                                            }
                                            QuText2 = QuText2.Trim().Length > 10 ? (QuText2 + "; " + sQuTable_2) : (QuText2 + sQuTable_2);
                                            if (sSourceLink_3.AsString().Trim() != "")
                                            {
                                                QuText2 = sSourceLink_Aus == ""
                                                    ? QuText2 + " " + Modul1.IText[EUserText.t449] + " " + sSourceLink_3.Trim()
                                                    : QuText2 + ", " + sSourceLink_Aus.Trim() + " " + sSourceLink_3.Trim();
                                            }
                                            if ("" != sSourceLink_Orig)
                                            {
                                                if (sSourceLink_Orig != "")
                                                {
                                                    QuText2 = (QuText2 + " >" + sSourceLink_Orig + "<").AsString();
                                                }
                                                QuText2 = Zeiweg(QuText2, xStrip: !Option[EOutCfg.o07_KeepFormat]);
                                            }
                                            if ("" != sSourceLink_Kom)
                                            {
                                                if (sSourceLink_Kom != "")
                                                {
                                                    QuText2 = (QuText2 + " ==" + sSourceLink_Kom + "==").AsString();
                                                }
                                                QuText2 = Zeiweg(QuText2, xStrip: !Option[EOutCfg.o07_KeepFormat]);
                                            }
                                        }
                                        DataModul.DB_SourceLinkTable.MoveNext();
                                    }
                                    if ("" != evt.sBem[3])
                                    {
                                        if (evt.sBem[3].Trim() != "")
                                        {
                                            Modul1.Kont1[9] = evt.sBem[3].Trim();
                                        }
                                        if (Modul1.Kont1[9].Trim() != "")
                                        {
                                            if (QuText2 == "")
                                            {
                                                QuText2 = ". " + Modul1.IText[EUserText.t450] + " " + Modul1.Kont1[9].Trim();
                                            }
                                            else
                                            {
                                                QuText2 = QuText2 + "; " + Modul1.Kont1[9].Trim();
                                                Modul1.Kont1[9] = "";
                                            }
                                            if (QuText2.Trim() != "")
                                            {
                                                QuText2 = Zeiweg(QuText2, xStrip: !Option[EOutCfg.o07_KeepFormat]);
                                                QuText2 = QuText2.Trim();
                                                if (QuText2.Right(1) == ";")
                                                {
                                                    QuText2 = Strings.Trim(QuText2.Left(QuText2.Trim().Length - 1));
                                                }
                                                QuText2 = QuText2.Trim() + ".";
                                            }
                                        }
                                    }
                                }
                                if (eEventArt <= EEventArt.eA_499)
                                {
                                    int persInArb = persInArb1;
                                    int num15 = 1;
                                    while (num15 <= 100)
                                    {
                                        KontSP[num15] = Modul1.Kont[num15];
                                        Modul1.Kont[num15] = "";
                                        num15 += 1;
                                    }

                                    EEventArt num17 = eEventArt;
                                    if (Option[EOutCfg.o32])
                                    {
                                        Zeugsu(num17);
                                        Modul1.UbgT1 = DataModul.Event.GetValue(persInArb, num17, EventFields.Bem4, f => f.AsString());
                                    }
                                    persInArb1 = persInArb;
                                    num15 = 1;
                                    while (num15 <= 100)
                                    {
                                        Modul1.Kont[num15] = KontSP[num15];
                                        KontSP[num15] = "";
                                        num15 += 1;
                                    }
                                    persInArb1 = persInArb;
                                    if (Modul1.UbgT1.Trim() != "")
                                    {
                                        Modul1.UbgT1 = " Zeugen: " + Modul1.UbgT1.Trim() + ".";
                                    }
                                }
                                lErl = 10;
                                if (Modul1.Kont1[10].Trim() != "")
                                {
                                    Modul1.Kont1[10] = " " + Modul1.Kont1[10].Trim() + " ";
                                }
                                sEvent = sEvent.Trim() + Modul1.Kont1[10] + QuText2 + Modul1.UbgT1;
                                QuText2 = "";
                                Modul1.UbgT1 = "";
                                if (sEvent.Trim().Right(1) != ".")
                                {
                                    sEvent = sEvent.Trim() + ". ";
                                }
                                if (eEventArt == EEventArt.eA_105 || num10 == 0)
                                {
                                    AppendTextIfNE(rtbAnz, sEvent);
                                }
                                else if (eEventArt != EEventArt.eA_603)
                                {
                                    if (sEvent.Trim() != "")
                                    {
                                        rtbAnz.AppendText("\n" + sEvent);
                                    }
                                }
                                else if (sEvent.Trim() != "")
                                {
                                    rtbAnz.AppendText(sEvent);
                                }

                                lErl = 22;
                                num10 = (short)unchecked(num10 + 1);
                            }

                            eEventArt = 0;
                            goto end_IL_0001_2;
                        IL_2f90:
                            num7 = unchecked(num2 + 1);
                            goto IL_2f94;
                        IL_2f94:
                            num2 = 0;
                            switch (num7)
                            {
                                case 1:
                                    break;
                                case 17:
                                case 86:
                                case 102:
                                case 115:
                                case 131:
                                case 160:
                                case 450:
                                case 459:
                                    goto end_IL_0001_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 14030;
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

        void Anz_AppendNewlineIfNeeded(IDocument rtbAnz)
        {
            _ = rtbAnz.AppendTextIfNd("\n");
        }

        static void AppendTextIfNE(IDocument rtbAnz, string sText)
        {
            if (!string.IsNullOrWhiteSpace(sText))
            {
                rtbAnz.AppendText(sText);
            }
        }
    }

    public void Heidat(out bool scheid, int famInArb, IDocument richTextBox)
    {
        int try0001_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int lErl = default;
        int num4 = default;
        int num6 = default;
        EEventArt eArt = default;
        string text = default;
        string QuText = default;
        scheid = false;
        while (true)
        {
            try
            {
                checked
                {
                    int num5;
                    switch (try0001_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0009;
                        case 8948:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_1e26;
                                    default:
                                        goto end_IL_0001;
                                }
                                if (Information.Err().Number == 94)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_1e26;
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
                                num5 = num2;
                                goto IL_1e2a;
                            }
                        end_IL_0001:
                            break;
                        IL_0009:
                            num = 2;
                            richTextBox.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                            if (Option[EOutCfg.o10_EmitIDs])
                            {
                                richTextBox.AppendText("[" + famInArb.AsString() + "]");
                            }
                            DataModul.DB_FamilyTable.Index = nameof(FamilyIndex.Fam);
                            DataModul.DB_FamilyTable.Seek("=", famInArb);
                            if (DataModul.DB_FamilyTable.Fields[FamilyFields.Aeb].AsInt() == -1)
                            {
                                richTextBox.AppendText("Aussereheliche Verbindung");
                            }
                            richTextBox.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                            num6 = 0;
                            goto IL_0159;
                        IL_0159:
                            num = 13;
                            eArt = num6 switch
                            {
                                0 => EEventArt.eA_501,
                                1 => EEventArt.eA_500,
                                2 => EEventArt.eA_507,
                                3 => EEventArt.eA_505,
                                4 => EEventArt.eA_Marriage,
                                5 => EEventArt.eA_MarrReligious,
                                6 => EEventArt.eA_504,
                                7 => EEventArt.eA_506,
                                _ => eArt,
                            };
                            Datu = default;
                            short num10 = 1;
                            while (num10 <= 15)
                            {
                                Modul1.Kont1[num10] = "";
                                num10++;
                            }
                            if (DataModul.Event.ReadData(eArt, famInArb, out var cEv) && !(cEv.iPrivacy > privaus))
                            {
                                Datu = cEv.dDatumV;
                                if (Datu.AsInt() > 0.0)
                                {
                                    _ = Datwand1(Datu, cEv.sDatumV_S);
                                    Modul1.Kont1[1] = Datu.AsString();
                                    if (cEv.sVChr != "0")
                                    {
                                        Modul1.Kont1[1] = Modul1.Kont1[1] + " VChr";
                                    }
                                }
                                else
                                {
                                    Modul1.Kont1[1] = "";
                                }
                                if (cEv.dDatumB != default)
                                {
                                    Datu = cEv.dDatumB;
                                    var sDatu = Datwand1(Datu, cEv.sDatumB_S);
                                    if (Modul1.Kont1[1] != "")
                                    {
                                        sDatu = " / " + sDatu;
                                    }
                                    Modul1.Kont1[3] = sDatu;
                                }
                                Modul1.UbgT = "";
                                if (cEv.iOrt > 0)
                                {
                                    Modul1.UbgT = Place_ReadData(cEv.iOrt, 0, 0, Option[EOutCfg.o35], cEv.sZusatz);
                                    if (cEv.sOrt_S.Trim() != "")
                                    {
                                        Modul1.UbgT = Modul1.UbgT.TrimEnd() + " " + Strings.Trim(cEv.sOrt_S);
                                    }
                                    Modul1.Kont1[5] = Modul1.UbgT;
                                }
                                if (0 != cEv.iDatumText)
                                {
                                    Modul1.Kont1[14] = " " + cEv.sDatumText + " ";
                                }
                                if (cEv.iKBem > 0)
                                {
                                    Modul1.Kont1[7] = cEv.sKBem.Trim();
                                }
                                if (0 != cEv.iHausNr)
                                {
                                    Modul1.Kont1[7] = Modul1.Kont1[7] + " " + cEv.sHausNr.Trim() + " ";
                                }
                                if (cEv.iPlatz > 0)
                                {
                                    Modul1.Kont1[6] = cEv.sPlatz.Trim();
                                }

                                bool flag = false;
                                DataModul.DB_SourceLinkTable.Index = "Tab22";
                                DataModul.DB_SourceLinkTable.Seek("=", 3, famInArb, cEv.eArt, 0);
                                flag |= !DataModul.DB_SourceLinkTable.NoMatch;
                                flag |= "" != cEv.sBem[3];
                                flag |= Option[EOutCfg.o05] && cEv.sBem[1].Trim() != "";
                                flag |= Option[EOutCfg.o06] && cEv.sBem[2].Trim() != "";
                                flag |= Modul1.Kont1[1].Trim() != ""
                                    || Modul1.Kont1[2].Trim() != ""
                                    || Modul1.Kont1[3].Trim() != ""
                                    || Modul1.Kont1[5].Trim() != ""
                                    || Modul1.Kont1[6].Trim() != ""
                                    || Modul1.Kont1[7].Trim() != ""
                                    || Modul1.UbgT.Trim() != "";
                                if (flag)
                                {
                                    text = eArt switch
                                    {
                                        EEventArt.eA_500 => Modul1.DTxt[5],
                                        EEventArt.eA_501 => Modul1.DTxt[6],
                                        EEventArt.eA_Marriage => Modul1.DTxt[7],
                                        EEventArt.eA_MarrReligious => Modul1.DTxt[8],
                                        EEventArt.eA_504 => Modul1.DTxt[9],
                                        EEventArt.eA_505 => Modul1.DTxt[10],
                                        EEventArt.eA_507 => Modul1.DTxt[15],
                                        _ => string.Empty,
                                    };
                                    var heidatEventDefinition = DocumentComposer.ComposeHeidatEvent(eArt, Option[EOutCfg.o14], text);
                                    scheid |= heidatEventDefinition.IsDivorceEvent;
                                    Modul1.Job = Jobdreh(Modul1.Job, ereiRf: EreiRf);
                                    if (Option[EOutCfg.o34] && "" != cEv.sReg.Trim())
                                    {
                                        Modul1.Job = Modul1.Job + " (Urk.-Nr.: " + Strings.Trim(cEv.sReg) + ") ";
                                    }
                                    DocumentOutputService.RenderHeidatEventPrefix(richTextBox, heidatEventDefinition);
                                    richTextBox.AppendText(Modul1.Job);
                                    Modul1.Job = "";
                                    LfNR = 0;
                                    Modul1.Kont1[20] = "";
                                    if (Strings.RTrim(cEv.sBem[1]) != "" && Option[EOutCfg.o05])
                                    {
                                        richTextBox.AppendText(" {" + Strings.RTrim(cEv.sBem[1]) + "}");
                                    }
                                    if (Strings.RTrim(cEv.sBem[2]) != "" && Option[EOutCfg.o06])
                                    {
                                        richTextBox.AppendText(" {" + Strings.RTrim(cEv.sBem[2]) + "}");
                                    }
                                    if (Option[EOutCfg.o39])
                                    {
                                        QuText = "";
                                        DataModul.DB_SourceLinkTable.Index = "Tab22";
                                        DataModul.DB_SourceLinkTable.Seek("=", 3, famInArb, cEv.eArt, 0);
                                        if (!DataModul.DB_SourceLinkTable.NoMatch)
                                        {
                                            _ = richTextBox.AppendTextIfNd(".");
                                            richTextBox.AppendText(Modul1.IText[EUserText.t450] + " ");
                                            while (!DataModul.DB_SourceLinkTable.EOF
                                                && !DataModul.DB_SourceLinkTable.NoMatch
                                                && 0 != DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Art].AsInt()
                                                && DataModul.DB_SourceLinkTable.Fields[0].AsInt() == 3
                                                && DataModul.DB_SourceLinkTable.Fields[1].AsInt() <= famInArb
                                                && DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Art].AsEnum<EEventArt>() == cEv.eArt
                                                && DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.LfNr].AsInt() == 0)
                                            {
                                                DataModul.DB_QuTable.Index = "Nr";
                                                DataModul.DB_QuTable.Seek("=", DataModul.DB_SourceLinkTable.Fields[2]);
                                                QuText = QuText != ""
                                                    ? (QuText + "; " + DataModul.DB_QuTable.Fields[QuFields._2].Value).AsString()
                                                    : DataModul.DB_QuTable.Fields[QuFields._2].AsString();
                                                if (DataModul.DB_SourceLinkTable.Fields[3].AsString().Trim() != "")
                                                {
                                                    QuText = null == DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].Value
                                                        ? QuText + ", " + Modul1.IText[EUserText.t449] + " " + DataModul.DB_SourceLinkTable.Fields[3].AsString().Trim()
                                                        : QuText + ", " + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].AsString().Trim() + " " + DataModul.DB_SourceLinkTable.Fields[3].AsString().Trim();
                                                }
                                                if (null != DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].Value && DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].AsString() != "")
                                                {
                                                    QuText = string.Concat(QuText, " >" + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].AsString() + "<");
                                                }
                                                if (null != DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Kom].Value && DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Kom].AsString() != "")
                                                {
                                                    QuText = string.Concat(QuText, " ==" + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Kom].AsString() + "==");
                                                }
                                                QuText = Zeiweg(QuText, xStrip: !Option[EOutCfg.o07_KeepFormat]);
                                                DataModul.DB_SourceLinkTable.MoveNext();
                                            }
                                        }
                                        if ("" != cEv.sBem[3])
                                        {
                                            if (Strings.Trim(cEv.sBem[3]) != "")
                                            {
                                                Modul1.UbgT = Strings.RTrim(cEv.sBem[3]);
                                                Modul1.UbgT = Zeiweg(Modul1.UbgT, !Option[EOutCfg.o07_KeepFormat]);
                                                QuText = QuText == "" ? ". " + Modul1.IText[EUserText.t450] + " " + Modul1.UbgT.TrimEnd() : QuText.Trim() + "; " + Modul1.UbgT.TrimEnd();
                                            }
                                            richTextBox.AppendText(QuText);
                                            QuText = "";
                                            _ = richTextBox.AppendTextIfNd(".");
                                        }
                                    }
                                    goto IL_19be;
                                }

                                goto IL_19be;
                            }

                            goto IL_1d11;
                        IL_19be:
                            num = 259;
                            if (Option[EOutCfg.o32])
                            {
                                Zeugsu(eArt);
                                Modul1.Kont1[20] = DataModul.Event.GetValue(famInArb, eArt, EventFields.Bem4, f => f.AsString());
                                if (Modul1.Kont1[20].Trim() != "")
                                {
                                    var heidatEventDefinition = DocumentComposer.ComposeHeidatEvent(eArt, emitIndentedOutput: false, eventText: string.Empty);
                                    _ = richTextBox.AppendTextIfNd(".");
                                    richTextBox.AppendText(" " + heidatEventDefinition.WitnessLabel + ": " + Modul1.Kont1[20].Trim());
                                    _ = richTextBox.AppendTextIfNd(".");
                                }
                            }
                            goto IL_1d11;
                        IL_1d11:
                            num = 288;
                            lErl = 22;
                            num6++;
                            if (num6 <= 7)
                            {
                                goto IL_0159;
                            }

                            if (num4 == 1)
                            {
                                richTextBox.AppendText(" mit\n");
                            }
                            richTextBox.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                            goto end_IL_0001_2;
                        IL_1e26:
                            num5 = unchecked(num2 + 1);
                            goto IL_1e2a;
                        IL_1e2a:
                            num2 = 0;
                            switch (num5)
                            {
                                case 1:
                                    break;
                                case 13:
                                    goto IL_0159;
                                case 255:
                                case 256:
                                case 257:
                                case 258:
                                case 259:
                                    goto IL_19be;
                                case 44:
                                case 53:
                                case 285:
                                case 286:
                                case 287:
                                case 288:
                                    goto IL_1d11;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 8948;
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
