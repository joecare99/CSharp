using BaseLib.Helper;
using GenFree;
using GenFree.Data;
using GenFree.Interfaces;
using GenFree.Interfaces.DB;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Gen_FreeWin.ViewModels;

public partial class NamenSuchViewModel
{
    public void EPerles(short Index, int FamSP1, int Persp1, bool Opt10, IDocument document)
    {
        //Discarded unreachable code: IL_485c
        int try0001_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int lErl = default;
        while (true)
        {
            try
            {
                ;
                checked
                {
                    int num4;
                    int persInArb2;
                    int persInArb3;
                    string InText;
                    int Ortnr;
                    short Art;
                    float Idned;
                    var person = new CPersonData();
                    switch (try0001_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0009;
                        case 21374:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_48b4;
                                    default:
                                        goto end_IL_0001;
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
                                goto IL_48b8;
                            }
                        end_IL_0001:
                            break;
                        IL_0009:
                            num = 2;
                            int M1_PersInArb = Modul1.PersInArb;
                            var M1_Kont0 = Modul1.Person.SurName;
                            var M1_Givennames = Modul1.Person.Givennames;
                            var M1_Kont4 = Modul1.Person.Alias;
                            var M1_Kont5 = Modul1.Person.Clan;
                            var M1_Kont6 = Modul1.Person.Stat;
                            var M1_Prae = Modul1.Person.Prae;
                            var M1_Kont11 = Modul1.Person.Birthday;
                            var M1_Kont12 = Modul1.Person.Baptised;
                            var M1_Kont13 = Modul1.Person.Death;
                            var M1_Kont14 = Modul1.Person.Burial;
                            string M1_Kont16 = "";
                            string M1_Kont17 = "";
                            string M1_Kont21 = "";
                            string M1_Kont22 = "";
                            string M1_Kont25 = "";
                            string M1_Kont31 = "";
                            string M1_Kont32 = "";
                            string M1_Kont41 = "";
                            string M1_Kont42 = "";

                            int M1_Ubg;
                            string M1_UbgT;
                            int persInArb = M1_PersInArb;
                            var M1_DTxt = Modul1.DTxt;

                            Font font_Arial_11_Bold = new("Arial", 11.01f, FontStyle.Bold);
                            Font font_Arial_11_Reg = new("Arial", 11.01f, FontStyle.Regular);

                            var dB_PersonTable = DataModul.DB_PersonTable;

                            Modul1.Person_ReadNames(M1_PersInArb, person);
                            int Pers_iReligi = dB_PersonTable.Fields[PersonFields.religi].AsInt();

                            if (M1_Prae.Trim() != "")
                            {
                                M1_Prae = M1_Prae.Trim() + " ";
                            }

                            if (Option[EOutCfg.o10_EmitIDs])
                            {
                                document.AppendText($"<{M1_PersInArb}> ");
                            }
                            document.AppendText(M1_Prae);
                            document.AppendText(M1_Givennames.TrimEnd() + " ");
                            _ = Namerw(Modul1.Person.SurName, Modul1.Person.Prefix, Modul1.Person.Suffix);
                            string selectedText = M1_Kont0.TrimEnd();
                            document.SetFont(font_Arial_11_Bold);
                            document.AppendText(selectedText);
                            document.SetFont(font_Arial_11_Reg);
                            if (M1_Kont5 != "")
                            {
                                document.AppendText($" Sippe {M1_Kont5.TrimEnd()}");
                            }
                            document.SetFont(new Font("Arial", 11.01f, FontStyle.Italic));
                            if (M1_Kont4 != "")
                            {
                                document.AppendText($" ({M1_Kont4.TrimEnd()})");
                            }
                            else
                            {
                                document.AppendText("");
                            }
                            document.SetFont(font_Arial_11_Reg);

                            if (Pers_iReligi > 0)
                            {
                                M1_Ubg = Pers_iReligi.AsInt();
                                M1_UbgT = DataModul.TextLese1(M1_Ubg);
                                document.AppendText(", " + M1_UbgT);
                            }
                            Bildaus("P");
                            if (M1_Kont6.Trim() != "")
                            {
                                document.AppendText(" " + M1_Kont6.TrimEnd() + " ");
                            }
                            document.SetFont(font_Arial_11_Reg);
                            AppendRelatives(Modul1.PersInArb, document);
                            Datles1(Modul1.PersInArb, Modul1.Person);
                            document.AppendText(M1_Kont25);
                            Retweg3(Document);
                            document.SetHangingIndent(40);
                            document.AppendText("\n");
                            document.SetFont(font_Arial_11_Reg);
                            document.SetIndent(0);
                            Datles1(Modul1.PersInArb, Modul1.Person);
                            string QuText;
                            if (M1_Kont11.Trim() != ""
                                || M1_Kont16.Length > 0
                                || M1_Kont21.Length > 0
                                || M1_Kont31.Trim() != ""
                                || M1_Kont41.Trim() != "")
                            {
                                document.SetIndent(20);
                                document.AppendText(M1_DTxt[1] + " " + M1_Kont11.Trim() + ".");
                                if (Option[EOutCfg.o02]
                                    && M1_Kont16.Trim() != "")
                                {
                                    QuText = M1_Kont16;
                                    QuText = Zeiweg(QuText, xStrip: !Option[EOutCfg.o07_KeepFormat]);
                                    document.AppendText(" " + QuText);
                                    QuText = "";
                                }
                                if (Option[EOutCfg.o03]
                                    && M1_Kont21.Trim() != "")
                                {
                                    QuText = M1_Kont21;
                                    QuText = Zeiweg(QuText, xStrip: !Option[EOutCfg.o07_KeepFormat]);
                                    document.AppendText(" " + QuText);
                                    QuText = "";
                                }
                                if (M1_Kont31.Trim() != "")
                                {
                                    QuText = M1_Kont31;
                                    QuText = Zeiweg(QuText, xStrip: !Option[EOutCfg.o07_KeepFormat]);
                                    document.AppendText(" " + QuText);
                                    QuText = "";
                                }
                                if (document.AppendTextIfNd("."))
                                {
                                    document.AppendText(" ");
                                }
                                if (M1_Kont41 != "")
                                {
                                    document.AppendText(" " + M1_Kont41);
                                }
                            }
                            if (M1_Kont12.Trim() != ""
                                || M1_Kont17.Length > 0
                                || M1_Kont22.Length > 0
                                || M1_Kont32.Trim() != ""
                                || M1_Kont42.Trim() != "")
                            {
                                document.AppendText("\n" + M1_DTxt[2] + " " + M1_Kont12.Trim() + ".");
                                if (Option[EOutCfg.o02]
                                    && M1_Kont17.Length > 0)
                                {
                                    QuText = M1_Kont17;
                                    QuText = Zeiweg(QuText, xStrip: !Option[EOutCfg.o07_KeepFormat]);
                                    document.AppendText(" " + QuText);
                                    QuText = "";
                                }
                                if (Option[EOutCfg.o03] && M1_Kont22.Length > 0)
                                {
                                    QuText = M1_Kont22;
                                    QuText = Zeiweg(QuText, xStrip: !Option[EOutCfg.o07_KeepFormat]);
                                    document.AppendText(" " + QuText);
                                    QuText = "";
                                }
                                if (M1_Kont32.Trim() != "")
                                {
                                    QuText = M1_Kont32;
                                    QuText = Zeiweg(QuText, xStrip: !Option[EOutCfg.o07_KeepFormat]);
                                    document.AppendText(" " + QuText);
                                    QuText = "";
                                }
                                _ = document.AppendTextIfNd(".");
                                if (M1_Kont42 != "")
                                {
                                    document.AppendText(" " + M1_Kont42);
                                }
                            }
                            int num11;
                            if (Index == 1 | Index == 2)
                            {
                                if (Option[EOutCfg.o30])
                                {
                                    persInArb2 = M1_PersInArb;
                                    int num5 = 0;

                                    foreach (var Link in DataModul.Link.ReadAllFams(persInArb2, ELinkKennz.lkGodparent))
                                    {
                                        Modul1.Person_ReadNames(Link.iPersNr, Modul1.Person);
                                        var sFullname = Namerw(Modul1.Person.SurName, Modul1.Person.Prefix, Modul1.Person.Suffix);
                                        document.SetFont(font_Arial_11_Reg);
                                        document.SetIndent(20);
                                        if (num5 == 0)
                                        {
                                            document.AppendText("\n");
                                            document.SetFont(font_Arial_11_Reg);
                                            document.AppendText(" " + Modul1.IText[EUserText.tGodparents].Replace("&", "") + ": " + (M1_Prae.Trim() + " " + M1_Givennames.Trim()).Trim() + " " + sFullname.Trim());
                                            num5 = 1;
                                        }
                                        else
                                        {
                                            document.AppendText((M1_Prae.Trim() + " " + M1_Givennames.Trim()).Trim() + " " + sFullname.Trim());
                                        }
                                        if (Option[EOutCfg.o31_GodpWithoutData])
                                        {
                                            Datles1(Link.iPersNr, Modul1.Person);
                                            if (M1_Kont11 == "")
                                            {
                                                M1_Kont11 = M1_Kont12;
                                            }
                                            if (M1_Kont11.Trim() != "")
                                            {
                                                document.AppendText(" * " + M1_Kont11.Trim());
                                            }
                                            if (M1_Kont13 == "")
                                            {
                                                M1_Kont13 = M1_Kont14;
                                            }
                                            if (M1_Kont14.Trim() != "" & M1_Kont11.Trim() != "")
                                            {
                                                document.AppendText(",");
                                            }
                                            if (M1_Kont14.Trim() != "")
                                            {
                                                document.AppendText(M1_DTxt[3] + " " + M1_Kont13.Trim());
                                            }
                                            document.AppendText("; ");
                                        }
                                        else
                                        {
                                            document.AppendText("; ");
                                        }
                                    }
                                    M1_PersInArb = persInArb;
                                    var dB_PersonTable2 = DataModul.Person.Seek(M1_PersInArb);
                                    string Pers_sBem2 = dB_PersonTable2.Fields[PersonFields.Bem2].AsString();

                                    if (null != Pers_sBem2 && Pers_sBem2.Trim() != "")
                                    {
                                        if (num5 == 0)
                                        {
                                            document.AppendText(" " + Modul1.IText[EUserText.tGodparents].Replace("&", "") + ": ");
                                        }
                                        document.AppendText(Pers_sBem2.Trim());
                                    }
                                    M1_PersInArb = persInArb;
                                }
                                _ = document.AppendTextIfNd(".");
                                if (Option[EOutCfg.o40])
                                {
                                    Sterdat(0);
                                }
                                _ = TrimEnd(document);
                                _ = document.TrimEnd(";");
                                _ = document.AppendTextIfNd(".");
                                var liList5 = new List<(DateTime, int)>();
                                if (Option[EOutCfg.o38])
                                {
                                    liList5.Clear();
                                    PersNr = M1_PersInArb.AsInt();

                                    foreach (var link in DataModul.Link.ReadAllPers(M1_PersInArb, ELinkKennz.lkGodparent))
                                    {
                                        M1_PersInArb = link.iFamNr;

                                        Datu = DataModul.Event.GetDate(EEventArt.eA_Baptism, M1_PersInArb, out _);
                                        if (Datu == default)
                                        {
                                            Datu = DataModul.Event.GetDate(EEventArt.eA_Birth, M1_PersInArb, out _);
                                        }

                                        liList5.Add((Datu, M1_PersInArb));
                                    }

                                    short num6 = (short)(liList5.Count - 1);
                                    short num7 = 0;
                                    while (num7 <= num6)
                                    {
                                        M1_PersInArb = liList5[num7].Item2;
                                        Modul1.Person_ReadNames(M1_PersInArb, Modul1.Person);
                                        _ = Namerw(Modul1.Person.SurName, Modul1.Person.Prefix, Modul1.Person.Suffix);
                                        Datu = liList5[num7].Item1;

                                        document.AppendText("\n" + Modul1.IText[EUserText.t348] + ": " + Datu + " bei " + (M1_Prae.Trim() + " " + M1_Givennames.Trim()).Trim() + " " + M1_Kont0.Trim());
                                        M1_PersInArb = PersNr;
                                        num7 = (short)unchecked(num7 + 1);
                                    }
                                }
                                M1_PersInArb = persInArb;
                                if (Option[EOutCfg.o37])
                                {
                                    Zeuge_Bei(Modul1.PersInArb);
                                }
                                M1_PersInArb = persInArb;
                                persInArb3 = M1_PersInArb;
                                foreach (var link in DataModul.Link.ReadAllFams(M1_PersInArb, ELinkKennz.lk9))
                                {
                                    M1_PersInArb = link.iPersNr;
                                    Modul1.Person_ReadNames(M1_PersInArb, Modul1.Person);
                                    M1_Kont0 = Namerw(Modul1.Person.SurName, Modul1.Person.Prefix, Modul1.Person.Suffix);
                                    document.AppendText("\n");
                                    document.AppendText("Verbundene Person: ");
                                    document.AppendText((M1_Prae.Trim() + " " + M1_Givennames.Trim()).Trim() + " " + M1_Kont0.Trim());
                                    Datles1(Modul1.PersInArb, Modul1.Person);
                                    if (M1_Kont11 == "")
                                    {
                                        M1_Kont11 = M1_Kont12;
                                    }
                                    if (M1_Kont11.Trim() != "")
                                    {
                                        document.AppendText(" * " + M1_Kont11.Trim() + ".");
                                    }
                                    if (M1_Kont13 == "")
                                    {
                                        M1_Kont13 = M1_Kont14;
                                    }
                                    if (M1_Kont14.Trim() != "")
                                    {
                                        document.AppendText(M1_DTxt[3] + " " + M1_Kont13.Trim() + ".");
                                    }
                                    M1_PersInArb = persInArb;
                                }
                                M1_PersInArb = persInArb;
                                foreach (var link in DataModul.Link.ReadAllPers(M1_PersInArb, ELinkKennz.lk9))
                                {
                                    M1_PersInArb = link.iFamNr;
                                    Modul1.Person_ReadNames(M1_PersInArb, Modul1.Person);
                                    M1_Kont0 = Namerw(Modul1.Person.SurName, Modul1.Person.Prefix, Modul1.Person.Suffix);
                                    document.AppendText("\nVerbunden mit " + (M1_Prae.Trim() + " " + M1_Givennames.Trim()).Trim() + " " + M1_Kont0.Trim());
                                    M1_PersInArb = persInArb;
                                }
                                Modul1.eLKennz = ELinkKennz.lkWitnOfEngage;
                                var aiFam = DataModul.Link.GetPersonFams(M1_PersInArb, Modul1.eLKennz);

                                liList5.Clear();
                                int num10 = aiFam.Count;
                                num11 = 1;
                                while (num11 <= num10)
                                {
                                    Modul1.FamInArb = aiFam[num11 - 1];
                                    Datu = DataModul.Event.GetDate(EEventArt.eA_MarrReligious, aiFam[num11 - 1]);
                                    liList5.Add((Datu, Modul1.FamInArb));
                                    num11++;
                                }
                                InText = "Verlobungszeuge";
                                Art = 501;
                                Zeugenaus(ref InText, ref Art, liList5);
                                M1_PersInArb = persInArb;
                                aiFam.Clear();
                                Modul1.eLKennz = ELinkKennz.lkMarrWitness;
                                aiFam = DataModul.Link.GetPersonFams(M1_PersInArb, Modul1.eLKennz);
                                liList5.Clear();
                                int num14 = aiFam.Count;
                                num11 = 1;
                                while (num11 <= num14)
                                {
                                    Modul1.FamInArb = aiFam[num11 - 1];
                                    Datu = DataModul.Event.GetDate(EEventArt.eA_MarrReligious, aiFam[num11 - 1]);
                                    liList5.Add((Datu, Modul1.FamInArb));
                                    num11++;
                                }
                                InText = "Trauzeuge";
                                Art = 502;
                                Zeugenaus(ref InText, ref Art, liList5);
                                M1_PersInArb = persInArb;
                                aiFam.Clear();
                                Modul1.eLKennz = ELinkKennz.lkWitnOfMarr;
                                aiFam = DataModul.Link.GetPersonFams(M1_PersInArb, Modul1.eLKennz);
                                liList5.Clear();
                                int num16 = aiFam.Count;
                                num11 = 1;
                                while (num11 <= num16)
                                {
                                    Modul1.FamInArb = aiFam[num11 - 1];
                                    Datu = DataModul.Event.GetDate(EEventArt.eA_MarrReligious, aiFam[num11 - 1]);
                                    liList5.Add((Datu, Modul1.FamInArb));
                                    num11++;
                                }
                                InText = "kirchl. Trauzeuge";
                                Art = 503;
                                Zeugenaus(ref InText, ref Art, liList5);
                                M1_PersInArb = persInArb;
                            }
                            if (Option[EOutCfg.o43] | false)
                            {
                                Berufe(EEventArt.eA_300, Document);
                                Berufe(EEventArt.eA_301, Document);
                                Berufe(EEventArt.eA_302, Document);
                                _ = document.AppendTextIfNd();
                                Berufe(EEventArt.eA_105, Document);
                                _ = document.AppendTextIfNd();
                                document.SetFont(font_Arial_11_Reg);
                                Berufe(EEventArt.eA_106, Document);
                            }
                            _ = document.AppendTextIfNd();
                            document.SetFont(font_Arial_11_Reg);
                            if (Option[EOutCfg.o40])
                            {
                                Sterdat(1);
                            }
                            _ = document.AppendTextIfNd();

                            string Pers_sBem1 = dB_PersonTable.Fields[PersonFields.Bem1].AsString();

                            if (Option[EOutCfg.o01_Person] && Pers_sBem1.Trim() != "")
                            {
                                QuText = Pers_sBem1.Trim();
                                QuText = Zeiweg(QuText, xStrip: !Option[EOutCfg.o07_KeepFormat]);
                                document.AppendText("{" + QuText + "}.\n");
                                QuText = "";
                            }
                            Quellen();
                            Persp1 = M1_PersInArb;
                            Modul1.eLKennz = ELinkKennz.lkFather;
                            if (dB_PersonTable.Fields[PersonFields.Sex].AsString() == "F")
                            {
                                Modul1.eLKennz = ELinkKennz.lkMother;
                            }
                            if (Index == 2)
                            {
                                Weitehen(FamSP1, this.Document);
                            }
                            M1_PersInArb = persInArb;
                            Modul1.Family.Mann = 0;
                            Modul1.Family.Frau = 0;
                            M1_Ubg = Adoeltsuch(Modul1.PersInArb);
                            Adoelt(M1_Ubg, Document);
                            M1_PersInArb = persInArb;
                            Modul1.Family.Mann = 0;
                            Modul1.Family.Frau = 0;
                            M1_Ubg = Modul1.Eltsuch(M1_PersInArb);
                            if (M1_Ubg > 0)
                            {
                                do
                                {
                                    document.AppendText("\n");
                                    document.SetIndent(40);
                                    ID = 200;
                                    document.SetFont(font_Arial_11_Bold);
                                    document.AppendText(Modul1.IText[EUserText.t82] + "\n");
                                    document.SetFont(font_Arial_11_Reg);
                                    Modul1.FamInArb = M1_Ubg;
                                    if (!DataModul.Link.GetFamPerson(Modul1.FamInArb, ELinkKennz.lkFather, out M1_PersInArb))
                                    {
                                        Modul1.Family.Mann = M1_PersInArb;

                                        Modul1.Person_ReadNames(M1_PersInArb, person);
                                        string FullName = Namerw(person.SurName, person.Prefix, person.Suffix);
                                        if (Option[EOutCfg.o10_EmitIDs])
                                        {
                                            document.AppendText("<" + M1_PersInArb.AsString() + "> " + M1_Prae.Trim() + " " + M1_Givennames.Trim());
                                        }
                                        else
                                        {
                                            document.AppendText(M1_Prae.Trim() + " " + M1_Givennames.Trim());
                                        }
                                        document.SetFont(font_Arial_11_Bold);
                                        document.AppendText(" " + FullName);
                                        document.SetFont(font_Arial_11_Reg);
                                        if (M1_Kont4 != "")
                                        {
                                            document.SetFont(new Font("Arial", 11.01f, FontStyle.Italic));
                                            document.AppendText(" (" + M1_Kont4.TrimEnd() + ")");
                                        }
                                        else
                                        {
                                            document.AppendText("");
                                        }
                                        document.SetFont(font_Arial_11_Reg);
                                        var pt = DataModul.Person.Seek(M1_PersInArb);
                                        if (pt.Fields[PersonFields.religi].AsInt() > 0)
                                        {
                                            M1_Ubg = pt.Fields[PersonFields.religi].AsInt();
                                            M1_UbgT = DataModul.TextLese1(M1_Ubg);
                                            document.AppendText(", " + M1_UbgT);
                                        }
                                        Bildaus("P");
                                        AppendRelatives(Modul1.PersInArb, document);
                                    }
                                    else
                                    {
                                        document.AppendText(Modul1.IText[EUserText.tSpouseUnknown] + "\n");
                                        M1_PersInArb = 0;
                                    }
                                    document.SetFont(font_Arial_11_Reg);
                                    BemSch = 0;
                                    Datu = default;
                                    if (M1_PersInArb > 0)
                                    {
                                        Idned = 0f;
                                        Datr1(ref Idned, Modul1.PersInArb);
                                    }
                                    num11 = 1;
                                    while (num11 <= 6)
                                    {
                                        Modul1.Kont1[num11] = "";
                                        num11++;
                                    }
                                    EEventArt eArt;
                                    if ((!DataModul.Event.ReadData((eArt = EEventArt.eA_Marriage, Modul1.FamInArb, 0), out var cEv)
                                            || cEv.dDatumV == default && cEv.dDatumB == default)
                                        && (!DataModul.Event.Exists((eArt = EEventArt.eA_MarrReligious, Modul1.FamInArb, 0))
                                            || cEv.dDatumV == default && cEv.dDatumB == default))
                                    {
                                        break;
                                    }
                                    Modul1_priv = cEv.iPrivacy;
                                    if (!(Modul1_priv > privaus))
                                    {
                                        if (cEv.dDatumV != default)
                                        {
                                            Datu = cEv.dDatumV;
                                            var sDatu = Datwand1(Datu, cEv.sDatumV_S);
                                            Modul1.Kont1[1] = sDatu;
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
                                        M1_UbgT = "";
                                        if (cEv.iOrt > 0)
                                        {
                                            Ortnr = cEv.iOrt;
                                            Modul1.UbgT = Place_ReadData(Ortnr, 1, 0, Option[EOutCfg.o35], cEv.sZusatz);
                                            if (Strings.Trim(cEv.sOrt_S) != "")
                                            {
                                                M1_UbgT = M1_UbgT.TrimEnd() + " " + Strings.Trim(cEv.sOrt_S);
                                            }
                                        }
                                        num11 = 1;
                                        while (num11 <= 6)
                                        {
                                            if (Modul1.Kont1[num11] == "0")
                                            {
                                                Modul1.Kont1[num11] = "";
                                            }
                                            num11++;
                                        }
                                        document.SetIndent(50);
                                        if (Option[EOutCfg.o14])
                                        {
                                            document.SetIndent(70);
                                        }
                                        _ = document.AppendTextIfNd(" ");

                                        string text2 = "";
                                        if (cEv.eArt == EEventArt.eA_Marriage)
                                        {
                                            text2 = M1_DTxt[7];
                                        }
                                        if (cEv.eArt == EEventArt.eA_MarrReligious)
                                        {
                                            text2 = M1_DTxt[8];
                                        }
                                        _ = document.AppendTextIfNd();
                                        if (Option[EOutCfg.o34] && "" != cEv.sReg && cEv.sReg.Trim() != "")
                                        {
                                            M1_UbgT = M1_UbgT + " (Urk.-Nr.: " + cEv.sReg.Trim() + ") ";
                                        }
                                        if (Option[EOutCfg.o10_EmitIDs])
                                        {
                                            document.AppendText(text2 + " [" + Modul1.FamInArb.AsString() + "] " + Modul1.Kont1[1] + " " + Modul1.Kont1[2] + Modul1.Kont1[3] + Modul1.Kont1[5] + Modul1.Kont1[6] + " " + M1_UbgT + " mit\n");
                                            M1_UbgT = "";
                                        }
                                        else
                                        {
                                            document.AppendText(text2 + " " + Modul1.Kont1[1] + " " + Modul1.Kont1[2] + Modul1.Kont1[3] + Modul1.Kont1[5] + Modul1.Kont1[6] + " " + M1_UbgT + " mit\n");
                                            M1_UbgT = "";
                                        }
                                    }
                                }
                                while (false);
                            }
                            else
                            {
                                Modul1.FamInArb = 0;
                                goto end_IL_0001_2;
                            }
                            lErl = 33;
                            document.SetIndent(40);
                            _ = document.AppendTextIfNd();
                            document.SetFont(font_Arial_11_Reg);
                            if (DataModul.Link.GetFamPerson(Modul1.FamInArb, ELinkKennz.lkMother, out M1_PersInArb))
                            {
                                Modul1.Family.Frau = M1_PersInArb;

                                Modul1.Person_ReadNames(M1_PersInArb, person);
                                var FullName = Namerw(person.SurName, person.Prefix, person.Suffix);
                                if (Option[EOutCfg.o10_EmitIDs])
                                {
                                    document.AppendText("<" + M1_PersInArb.AsString() + "> " + M1_Prae.Trim() + " " + M1_Givennames.Trim());
                                }
                                else
                                {
                                    document.AppendText(M1_Prae.Trim() + " " + M1_Givennames.Trim());
                                }
                                document.SetFont(font_Arial_11_Bold);
                                document.AppendText(" " + FullName);
                                document.SetFont(font_Arial_11_Reg);
                                document.SetFont(new Font("Arial", 11.01f, FontStyle.Italic));
                                if (M1_Kont4 != "")
                                {
                                    document.AppendText(" (" + M1_Kont4.TrimEnd() + ")");
                                }
                                else
                                {
                                    document.AppendText("");
                                }
                                document.SetFont(font_Arial_11_Reg);
                                var pt = DataModul.Person.Seek(M1_PersInArb);
                                if (pt.Fields[PersonFields.religi].AsInt() > 0)
                                {
                                    M1_Ubg = pt.Fields[PersonFields.religi].AsInt().AsInt();
                                    M1_UbgT = DataModul.TextLese1(M1_Ubg);
                                    document.AppendText(", " + M1_UbgT);
                                }
                                Bildaus("P");
                                AppendRelatives(Modul1.PersInArb, document);
                                Idned = 0f;
                                Datr1(ref Idned, Modul1.PersInArb);
                                _ = document.AppendTextIfNd();
                                document.SetFont(font_Arial_11_Reg);
                            }
                            else
                            {
                                document.AppendText(Modul1.IText[EUserText.tSpouseUnknown] + "\n");
                                document.SetFont(font_Arial_11_Reg);
                            }
                            goto end_IL_0001_2;
                        IL_48b4:
                            num4 = unchecked(num2 + 1);
                            goto IL_48b8;
                        IL_48b8:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 669:
                                case 674:
                                case 681:
                                case 686:
                                    goto end_IL_0001_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 21374;
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

    public void AppendRelatives(int persInArb, IDocument edtAnzeige)
    {
        bool xAppendAnc = Option[EOutCfg.o11];
        bool xAppendDesc = Option[EOutCfg.o12];
        if (xAppendAnc || xAppendDesc)
        {
            Font fArial11Underl = new("Arial", 11.01f, FontStyle.Underline);
            Font fArial11Reg = new("Arial", 11.01f, FontStyle.Regular);
            Ahnles(persInArb, out _, out string Modul1_Kont11, out string Modul1_Kont13, out _);
            if (xAppendAnc && Modul1_Kont11.Trim() != "")
            {
                edtAnzeige.SetFont(fArial11Underl);
                edtAnzeige.AppendText(" ");
                edtAnzeige.AppendText("Ahnen-Nr.: " + Modul1_Kont11.Trim() + ".");
                edtAnzeige.SetFont(fArial11Reg);
            }
            if (xAppendDesc && Modul1_Kont13.Trim() != "")
            {
                edtAnzeige.AppendText(" ");
                edtAnzeige.SetFont(fArial11Underl);
                edtAnzeige.AppendText(Modul1_Kont13);
                edtAnzeige.SetFont(fArial11Reg);
            }
            edtAnzeige.SetFont(fArial11Reg);
        }
    }

    public void Kindhei(int persInArb1)
    {
        string sPerSex;
        Modul1.eLKennz = (sPerSex = DataModul.Person.GetSex(persInArb1)) == "F" ? ELinkKennz.lkMother : ELinkKennz.lkFather;

        var aiFam = Modul1.Link_Famsuch(persInArb1, Modul1.eLKennz);
        var liList5 = new List<(DateTime, int)>();
        liList5.Clear();
        checked
        {
            int num = aiFam.Count;
            int M1_Iter = 1;
            while (M1_Iter <= num)
            {
                Modul1.FamInArb = aiFam[M1_Iter - 1];
                EEventArt num3 = EEventArt.eA_500;
                while (num3 <= EEventArt.eA_505)
                {
                    if ((Datu = DataModul.Event.GetDate(num3, Modul1.FamInArb)) != default && num3 != EEventArt.eA_504)
                    {
                        break;
                    }
                    num3++;
                }
                if (Datu.AsInt() == 0.0)
                {
                    Datu = DataModul.Event.GetDate(EEventArt.eA_601, Modul1.FamInArb);
                }
                liList5.Add((Datu, Modul1.FamInArb));
                M1_Iter++;
            }

            IDocument edtAnzeige = Document;
            if (Option[EOutCfg.o14])
            {
                _ = edtAnzeige.AppendTextIfNd();
                edtAnzeige.SetIndent(90);
            }
            edtAnzeige.SetIndent(0);
            _ = edtAnzeige.AppendTextIfNd(" ");
            if (liList5.Count == 1)
            {
                edtAnzeige.AppendText("Verbindung:");
            }
            if (liList5.Count > 1)
            {
                edtAnzeige.AppendText("Verbindungen:");
            }
            short num6 = (short)(liList5.Count - 1);
            short num7 = 0;
            while (num7 <= num6)
            {
                Modul1.FamInArb = liList5[num7].Item2;
                Heidat(out Scheid, Modul1.FamInArb, Document);
                LPweg();
                Bildaus("F");
                if (Option[EOutCfg.o14])
                {
                    edtAnzeige.AppendText("\n");
                    edtAnzeige.SetIndent(30);
                    edtAnzeige.AppendText("mit ");
                }
                else
                {
                    edtAnzeige.AppendText(" mit ");
                }
                Modul1.eLKennz = sPerSex == "F" ? ELinkKennz.lkFather : ELinkKennz.lkMother;
                if (DataModul.Link.GetFamPerson(Modul1.FamInArb, Modul1.eLKennz, out var persInArb2))
                {
                    Modul1.Person_ReadNames(persInArb2, Modul1.Person);
                    Modul1.Person.SetFullSurname(Namerw(Modul1.Person.SurName, Modul1.Person.Prefix, Modul1.Person.Suffix));
                    if (Option[EOutCfg.o10_EmitIDs])
                    {
                        edtAnzeige.AppendText("<" + persInArb2.AsString() + "> " + Modul1.Person.Prae.Trim() + " " + Modul1.Person.Givennames.Trim());
                    }
                    else
                    {
                        edtAnzeige.AppendText(Modul1.Person.Prae.Trim() + " " + Modul1.Person.Givennames.Trim());
                    }
                    edtAnzeige.SetFont(new Font("Arial", 11.01f, FontStyle.Bold));
                    edtAnzeige.AppendText(" " + Modul1.Person.FullSurName);
                    edtAnzeige.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                    edtAnzeige.SetFont(new Font("Arial", 11.01f, FontStyle.Italic));
                    if (Modul1.Person.Alias != "")
                    {
                        edtAnzeige.AppendText(" (" + Modul1.Person.Alias.TrimEnd() + ") ");
                    }
                    else
                    {
                        edtAnzeige.AppendText("");
                    }
                    edtAnzeige.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                    var pt = DataModul.Person.Seek(persInArb2);
                    if (pt.Fields[PersonFields.religi].AsInt() > 0)
                    {
                        var ubg = pt.Fields[PersonFields.religi].AsInt();
                        _ = DataModul.Textlese(ubg, out var ubgT, out _);
                        edtAnzeige.AppendText(", " + ubgT);
                    }
                    Bildaus("P");
                    AppendRelatives(persInArb2, edtAnzeige);
                    ID = 600;
                    float Idned = 0f;
                    Datr1(ref Idned, persInArb2);
                }
                else
                {
                    edtAnzeige.AppendText(Modul1.IText[EUserText.tSpouseUnknown] + "\n");
                }
                num7++;
            }
            edtAnzeige.SetIndent(0);
        }
    }
}
