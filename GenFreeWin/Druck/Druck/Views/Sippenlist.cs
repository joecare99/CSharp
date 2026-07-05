using BaseLib.Helper;
using Druck.My;
using Druck.Views;
using GenFree;
using GenFree.Data;
using GenFree.Helper;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.Compatibility.VB6;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Druck.Views;

public partial class Sippenlist : Form
{


    [SpecialName]
    private StaticLocalInitFlag _0024STATIC_0024Berufe_00242001_0024Job_0024Init;

    [SpecialName]
    private string _0024STATIC_0024Berufe_00242001_0024Job1;

    [SpecialName]
    private StaticLocalInitFlag _0024STATIC_0024Berufe_00242001_0024Job1_0024Init;

    [SpecialName]
    private StaticLocalInitFlag _0024STATIC_0024Berufe_00242001_0024Datu1_0024Init;

    [SpecialName]
    private string _0024STATIC_0024Berufe_00242001_0024Datu1;

    [SpecialName]
    private byte _0024STATIC_0024Berufe_00242001_0024I1;

    [SpecialName]
    private string _0024STATIC_0024Berufe_00242001_0024Job;
    private EEventArt Modul1_Beruf;



    [DebuggerNonUserCode]
    public Sippenlist()
    {
        base.Load += Sippenlist_Load;
        _0024STATIC_0024Berufe_00242001_0024Job_0024Init = new StaticLocalInitFlag();
        _0024STATIC_0024Berufe_00242001_0024Job1_0024Init = new StaticLocalInitFlag();
        _0024STATIC_0024Berufe_00242001_0024Datu1_0024Init = new StaticLocalInitFlag();
        this.Command_Renamed = new ControlArray<System.Windows.Forms.Button>();
        this.Command2 = new ControlArray<System.Windows.Forms.Button>();
        this.Label1 = new ControlArray<System.Windows.Forms.Label>();
        this.RtB = new Microsoft.VisualBasic.Compatibility.VB6.RichTextBoxArray(this.components);
        InitializeComponent();
        this.Command_Renamed.SetIndex(this._Command_17, 17);
        this.Command_Renamed.SetIndex(this._Command_16, 16);
        this.Command_Renamed.SetIndex(this._Command_15, 15);
        this.Command_Renamed.SetIndex(this._Command_14, 14);
        this.Command2.SetIndex(this._Command2_3, 3);
        this.Command_Renamed.SetIndex(this._Command_13, 13);
        this.Command_Renamed.SetIndex(this._Command_12, 12);
        this.Command_Renamed.SetIndex(this._Command_8, 8);
        this.Command_Renamed.SetIndex(this._Command_9, 9);
        this.Command_Renamed.SetIndex(this._Command_10, 10);
        this.Command_Renamed.SetIndex(this._Command_11, 11);
        this.Command_Renamed.SetIndex(this._Command_7, 7);
        this.Command_Renamed.SetIndex(this._Command_6, 6);
        this.Command_Renamed.SetIndex(this._Command_5, 5);
        this.Command_Renamed.SetIndex(this._Command_4, 4);
        this.Command_Renamed.SetIndex(this._Command_3, 3);
        this.Command_Renamed.SetIndex(this._Command_2, 2);
        this.Command_Renamed.SetIndex(this._Command_0, 0);
        this.Command_Renamed.SetIndex(this._Command_1, 1);
        this.Label1.SetIndex(this._Label1_0, 0);
        this.Label1.SetIndex(this._Label1_1, 1);
        this.Label1.SetIndex(this._Label1_2, 2);
        this.Label1.SetIndex(this._Label1_3, 3);
        this.RtB.SetIndex(this._RtB_0, 0);
        this.RtB.SetIndex(this._RtB_1, 1);
        this.RtB.SetIndex(this._RtB_2, 2);
        Command_Renamed.AddClick(Command_Renamed_Click);

    }

    private void Command_Renamed_Click(object eventSender, EventArgs eventArgs)
    {
        //Discarded unreachable code: IL_1996, IL_1b64
        int index = Command_Renamed.GetIndex((Button)eventSender);
        string text = "";
        string text2 = "";
        string right = "";
        _Modul1.Instance.Schalt = 0;
        IList<int> aiFams;
        checked
        {
            switch (index)
            {
                case 0:
                    Close();
                    MyProject.Forms.Druck.Show();
                    break;
                case 1:
                    {
                        RtB[0].Font = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        RtB[1].Font = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        RtB[2].Font = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        List3.Items.Clear();
                        RtB[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                        RtB[0].Visible = true;
                        Frame1.Visible = false;
                        byte b2 = 0;
                        do
                        {
                            RtB[b2].Width = Width - 20;
                            RtB[b2].RightMargin = Width - 60;
                            RtB[b2].Height = Height - 130;
                            b2 = (byte)unchecked((uint)(b2 + 1));
                        }
                        while (unchecked(b2) <= 2u);
                        DataModul.DT_DescendentTable.Index = "GNR";
                        DataModul.DT_DescendentTable.MoveFirst();
                        string text6 = 0.AsString();
                        byte b4 = default;
                        while (!DataModul.DT_DescendentTable.EOF)
                        {
                            text6 += ">";
                            Label2.Text = text6;
                            Label2.Refresh();
                            if (text6.Length == 70)
                            {
                                text6 = "";
                            }
                            byte b3 = (byte)Math.Round(DataModul.DT_DescendentTable.Fields["gen"].Value.AsDouble());
                            if (unchecked(b3 > (uint)b4))
                            {
                                RtB[0].SelectedText = "\n" + Strings.Right("  " + b3.AsString().Trim(), 2) + ".Generation\n\n";
                                b4 = b3;
                            }
                            if (b3 > 2)
                            {
                                string key = DataModul.DT_DescendentTable.Fields["Nr"].AsString();
                                string text7 = DataModul.DT_DescendentTable.Fields["Nr"].AsString();
                                short num = 1;
                                do
                                {
                                    text7 = text7.Left(text7.Length - 1);
                                    if (text7.Right(1) == ".")
                                    {
                                        string text8 = text7;
                                        if (text8 == right)
                                        {
                                            break;
                                        }
                                        right = text8;
                                        DataModul.DT_DescendentTable.Index = "Nr";
                                        DataModul.DT_DescendentTable.Seek("=", text8);
                                        if (DataModul.DT_DescendentTable.NoMatch)
                                        {
                                            break;
                                        }
                                        _Modul1.Instance.PersInArb = DataModul.DT_DescendentTable.Fields["Pr"].AsInt();
                                        string text3 = " des ";
                                        byte b5 = 1;
                                        _Modul1.Instance.PerSatzLes(_Modul1.Instance.PersInArb);
                                        if (DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "F")
                                        {
                                            text3 = " der ";
                                            b5 = 2;
                                        }
                                        _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                        RtB[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Underline);
                                        if (_Modul1.Instance.Kont[1] != "")
                                        {
                                            _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[1] + " " + _Modul1.Instance.Kont[0];
                                        }
                                        if (_Modul1.Instance.Kont[2] != "")
                                        {
                                            _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[0] + " " + _Modul1.Instance.Kont[2];
                                        }
                                        RtB[0].SelectedText = "\nKinder" + text3 + _Modul1.Instance.Kont[3] + " " + _Modul1.Instance.Kont[0] + " (" + Operators.SubtractObject(DataModul.DT_DescendentTable.Fields["Gen"].Value, 1).AsString() + "- " + text8.Trim() + ") ";
                                        _Modul1.Instance.Eltq = text8.Trim();
                                        RtB[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                        RtB[0].SelectedText = "";
                                        if (b5 == 2)
                                        {
                                            List1.Items.Clear();
                                            _Modul1.Instance.eLKennz = ELinkKennz.lkMother;
                                            aiFams = DataModul.Link.GetPersonFams(_Modul1.Instance.PersInArb, _Modul1.Instance.eLKennz);
                                            string text9 = _Modul1.Instance.UbgT;
                                            byte b6 = 1;
                                            do
                                            {
                                                if (text9.Length > 0)
                                                {
                                                    _Modul1.Instance.FamInArb = (int)Math.Round(text9.Left(10).AsDouble());
                                                    text9 = Strings.Mid(text9, 11, text9.Length);
                                                    DataModul.DB_EventTable.Index = "ArtNr";
                                                    byte b7 = 0;
                                                    int num2 = 502;
                                                    do
                                                    {
                                                        DataModul.DB_EventTable.Seek("=", num2.AsString(), _Modul1.Instance.FamInArb.AsString(), "0");
                                                        if (!DataModul.DB_EventTable.NoMatch && Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                                        {
                                                            List1.Items.Add(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim() + "          " + _Modul1.Instance.FamInArb.AsString().Right(10));
                                                            b7 = 1;
                                                            break;
                                                        }
                                                        num2++;
                                                    }
                                                    while (num2 <= 505);
                                                    if (b7 == 0)
                                                    {
                                                        List1.Items.Add("        " + "          " + _Modul1.Instance.FamInArb.AsString().Right(10));
                                                    }
                                                }
                                                if (text9.Length < 10)
                                                {
                                                    break;
                                                }
                                                b6 = (byte)unchecked((uint)(b6 + 1));
                                            }
                                            while (unchecked(b6) <= 10u);
                                            int count = List1.Items.Count;
                                            for (int num2 = 0; num2 <= count; num2++)
                                            {
                                                _Modul1.Instance.FamInArb = (int)Math.Round(Conversion.Val(Strings.Mid(List1.Items[num2].AsString(), 9, 10)));
                                                _Modul1.Instance.Famles();
                                                if ((_Modul1.Instance.PersInArb = _Modul1.Instance.Family.Mann) > 0)
                                                {
                                                    RtB[0].SelectedText = " " + _Modul1.Instance.DTxt[7] + " ";
                                                    _Modul1.Instance.Schalt = 0;
                                                    _Modul1.Instance.Famdatles2();
                                                    if (_Modul1.Instance.Kont[2].Trim() != "")
                                                    {
                                                        RtB[0].SelectedText = _Modul1.Instance.Kont[2].Trim() + " ";
                                                    }
                                                    else if (_Modul1.Instance.Kont[3].Trim() != "")
                                                    {
                                                        RtB[0].SelectedText = _Modul1.Instance.Kont[3].Trim() + " ";
                                                    }
                                                    _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                                    RtB[0].SelectedText = "mit " + _Modul1.Instance.Kont[3] + " ";
                                                    if (_Modul1.Instance.Kont[1].Trim() != "")
                                                    {
                                                        _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[1].Trim() + " " + _Modul1.Instance.Kont[0].Trim();
                                                    }
                                                    if (_Modul1.Instance.Kont[2].Trim() != "")
                                                    {
                                                        _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[0].Trim() + " " + _Modul1.Instance.Kont[2].Trim();
                                                    }
                                                    RtB[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                                    RtB[0].SelectedText = _Modul1.Instance.Kont[0];
                                                    RtB[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                                    _Modul1.Instance.Ahnles(_Modul1.Instance.PersInArb, out var asAhnData);
                                                    if (_Modul1.Instance.Kont[13].Trim() != "")
                                                    {
                                                        RtB[0].SelectedText = " " + _Modul1.Instance.Kont[13];
                                                        List3.Items.Add("          " + _Modul1.Instance.Family.Mann.AsString().Right(10) + "          " + _Modul1.Instance.Family.Frau.AsString().Right(10));
                                                    }
                                                }
                                            }
                                        }
                                        List1.Items.Clear();
                                        if (b5 == 1)
                                        {
                                            List1.Items.Clear();
                                            _Modul1.Instance.eLKennz = ELinkKennz.lkFather;
                                            aiFams = DataModul.Link.GetPersonFams(_Modul1.Instance.PersInArb, _Modul1.Instance.eLKennz);
                                            string text9 = _Modul1.Instance.UbgT;
                                            byte b6 = 1;
                                            do
                                            {
                                                if (text9.Length > 0)
                                                {
                                                    _Modul1.Instance.FamInArb = (int)Math.Round(text9.Left(10).AsDouble());
                                                    text9 = Strings.Mid(text9, 11, text9.Length);
                                                    DataModul.DB_EventTable.Index = "ArtNr";
                                                    byte b7 = 0;
                                                    int num2 = 502;
                                                    do
                                                    {
                                                        DataModul.DB_EventTable.Seek("=", num2.AsString(), _Modul1.Instance.FamInArb.AsString(), "0");
                                                        if (!DataModul.DB_EventTable.NoMatch && Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                                        {
                                                            List1.Items.Add(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim() + "          " + _Modul1.Instance.FamInArb.AsString().Right(10));
                                                            b7 = 1;
                                                            break;
                                                        }
                                                        num2++;
                                                    }
                                                    while (num2 <= 505);
                                                    if (b7 == 0)
                                                    {
                                                        List1.Items.Add("        " + "          " + _Modul1.Instance.FamInArb.AsString().Right(10));
                                                    }
                                                }
                                                if (text9.Length < 10)
                                                {
                                                    break;
                                                }
                                                b6 = (byte)unchecked((uint)(b6 + 1));
                                            }
                                            while (unchecked(b6) <= 10u);
                                            int count = List1.Items.Count;
                                            for (int num2 = 0; num2 <= count; num2++)
                                            {
                                                _Modul1.Instance.FamInArb = (int)Math.Round(Conversion.Val(Strings.Mid(List1.Items[num2].AsString(), 9, 10)));
                                                _Modul1.Instance.Famles();
                                                if ((_Modul1.Instance.PersInArb = _Modul1.Instance.Family.Frau) > 0)
                                                {
                                                    RtB[0].SelectedText = " " + _Modul1.Instance.DTxt[7] + " ";
                                                    _Modul1.Instance.Schalt = 0;
                                                    _Modul1.Instance.Famdatles2();
                                                    if (_Modul1.Instance.Kont[2].Trim() != "")
                                                    {
                                                        RtB[0].SelectedText = _Modul1.Instance.Kont[2].Trim() + " ";
                                                    }
                                                    else if (_Modul1.Instance.Kont[3].Trim() != "")
                                                    {
                                                        RtB[0].SelectedText = _Modul1.Instance.Kont[3].Trim() + " ";
                                                    }
                                                    _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                                    RtB[0].SelectedText = "mit " + _Modul1.Instance.Kont[3] + " ";
                                                    if (_Modul1.Instance.Kont[1].Trim() != "")
                                                    {
                                                        _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[1].Trim() + " " + _Modul1.Instance.Kont[0].Trim();
                                                    }
                                                    if (_Modul1.Instance.Kont[2].Trim() != "")
                                                    {
                                                        _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[0].Trim() + " " + _Modul1.Instance.Kont[2].Trim();
                                                    }
                                                    RtB[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                                    RtB[0].SelectedText = _Modul1.Instance.Kont[0];
                                                    RtB[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                                    _Modul1.Instance.Ahnles(_Modul1.Instance.PersInArb, out var asAhnData);
                                                    if (_Modul1.Instance.Kont[13].Trim() != "")
                                                    {
                                                        RtB[0].SelectedText = " " + _Modul1.Instance.Kont[13];
                                                        List3.Items.Add("          " + _Modul1.Instance.Family.Mann.AsString().Right(10) + "          " + _Modul1.Instance.Family.Frau.AsString().Right(10));
                                                    }
                                                }
                                            }
                                        }
                                        List1.Items.Clear();
                                        if (b5 == 2)
                                        {
                                            List1.Items.Clear();
                                            _Modul1.Instance.eLKennz = ELinkKennz.lkMother;
                                            aiFams = DataModul.Link.GetPersonFams(_Modul1.Instance.PersInArb, _Modul1.Instance.eLKennz);
                                            string text9 = _Modul1.Instance.UbgT;
                                            byte b6 = 1;
                                            do
                                            {
                                                if (text9.Length > 0)
                                                {
                                                    _Modul1.Instance.FamInArb = (int)Math.Round(text9.Left(10).AsDouble());
                                                    text9 = Strings.Mid(text9, 11, text9.Length);
                                                    DataModul.DB_EventTable.Index = "ArtNr";
                                                    byte b7 = 0;
                                                    int num2 = 502;
                                                    do
                                                    {
                                                        DataModul.DB_EventTable.Seek("=", num2.AsString(), _Modul1.Instance.FamInArb.AsString(), "0");
                                                        if (!DataModul.DB_EventTable.NoMatch && Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                                        {
                                                            List1.Items.Add(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim() + "          " + _Modul1.Instance.FamInArb.AsString().Right(10));
                                                            b7 = 1;
                                                            break;
                                                        }
                                                        num2++;
                                                    }
                                                    while (num2 <= 505);
                                                    if (b7 == 0)
                                                    {
                                                        List1.Items.Add("        " + "          " + _Modul1.Instance.FamInArb.AsString().Right(10));
                                                    }
                                                }
                                                if (text9.Length < 10)
                                                {
                                                    break;
                                                }
                                                b6 = (byte)unchecked((uint)(b6 + 1));
                                            }
                                            while (unchecked(b6) <= 10u);
                                            int count = List1.Items.Count;
                                            for (int num2 = 0; num2 <= count; num2++)
                                            {
                                                _Modul1.Instance.FamInArb = (int)Math.Round(Conversion.Val(Strings.Mid(List1.Items[num2].AsString(), 9, 10)));
                                                _Modul1.Instance.Famles();
                                                if ((_Modul1.Instance.PersInArb = _Modul1.Instance.Family.Mann) > 0)
                                                {
                                                    RtB[0].SelectedText = " " + _Modul1.Instance.DTxt[7] + " ";
                                                    _Modul1.Instance.Schalt = 0;
                                                    _Modul1.Instance.Famdatles2();
                                                    if (_Modul1.Instance.Kont[2].Trim() != "")
                                                    {
                                                        RtB[0].SelectedText = _Modul1.Instance.Kont[2].Trim() + " ";
                                                    }
                                                    else if (_Modul1.Instance.Kont[3].Trim() != "")
                                                    {
                                                        RtB[0].SelectedText = _Modul1.Instance.Kont[3].Trim() + " ";
                                                    }
                                                    _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                                    RtB[0].SelectedText = "mit " + _Modul1.Instance.Kont[3] + " ";
                                                    if (_Modul1.Instance.Kont[1].Trim() != "")
                                                    {
                                                        _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[1].Trim() + " " + _Modul1.Instance.Kont[0].Trim();
                                                    }
                                                    if (_Modul1.Instance.Kont[2].Trim() != "")
                                                    {
                                                        _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[0].Trim() + " " + _Modul1.Instance.Kont[2].Trim();
                                                    }
                                                    RtB[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                                    RtB[0].SelectedText = _Modul1.Instance.Kont[0];
                                                    RtB[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                                    _Modul1.Instance.Ahnles(_Modul1.Instance.PersInArb, out var asAhnData);
                                                    if (_Modul1.Instance.Kont[13].Trim() != "")
                                                    {
                                                        RtB[0].SelectedText = " " + _Modul1.Instance.Kont[13];
                                                        List3.Items.Add("          " + _Modul1.Instance.Family.Mann.AsString().Right(10) + "          " + _Modul1.Instance.Family.Frau.AsString().Right(10));
                                                    }
                                                }
                                            }
                                        }
                                        List1.Items.Clear();
                                        if (b5 == 1)
                                        {
                                            List1.Items.Clear();
                                            _Modul1.Instance.eLKennz = ELinkKennz.lkFather;
                                            aiFams = DataModul.Link.GetPersonFams(_Modul1.Instance.PersInArb, _Modul1.Instance.eLKennz);
                                            string text9 = _Modul1.Instance.UbgT;
                                            byte b6 = 1;
                                            do
                                            {
                                                if (text9.Length > 0)
                                                {
                                                    _Modul1.Instance.FamInArb = (int)Math.Round(text9.Left(10).AsDouble());
                                                    text9 = Strings.Mid(text9, 11, text9.Length);
                                                    DataModul.DB_EventTable.Index = "ArtNr";
                                                    byte b7 = 0;
                                                    int num2 = 502;
                                                    do
                                                    {
                                                        DataModul.DB_EventTable.Seek("=", num2.AsString(), _Modul1.Instance.FamInArb.AsString(), "0");
                                                        if (!DataModul.DB_EventTable.NoMatch && Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                                        {
                                                            List1.Items.Add(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim() + "          " + _Modul1.Instance.FamInArb.AsString().Right(10));
                                                            b7 = 1;
                                                            break;
                                                        }
                                                        num2++;
                                                    }
                                                    while (num2 <= 505);
                                                    if (b7 == 0)
                                                    {
                                                        List1.Items.Add("        " + "          " + _Modul1.Instance.FamInArb.AsString().Right(10));
                                                    }
                                                }
                                                if (text9.Length < 10)
                                                {
                                                    break;
                                                }
                                                b6 = (byte)unchecked((uint)(b6 + 1));
                                            }
                                            while (unchecked(b6) <= 10u);
                                            int count = List1.Items.Count;
                                            for (int num2 = 0; num2 <= count; num2++)
                                            {
                                                _Modul1.Instance.FamInArb = (int)Math.Round(Conversion.Val(Strings.Mid(List1.Items[num2].AsString(), 9, 10)));
                                                _Modul1.Instance.Famles();
                                                if ((_Modul1.Instance.PersInArb = _Modul1.Instance.Family.Frau) > 0)
                                                {
                                                    RtB[0].SelectedText = " " + _Modul1.Instance.DTxt[7] + " ";
                                                    _Modul1.Instance.Schalt = 0;
                                                    _Modul1.Instance.Famdatles2();
                                                    if (_Modul1.Instance.Kont[2].Trim() != "")
                                                    {
                                                        RtB[0].SelectedText = _Modul1.Instance.Kont[2].Trim() + " ";
                                                    }
                                                    else if (_Modul1.Instance.Kont[3].Trim() != "")
                                                    {
                                                        RtB[0].SelectedText = _Modul1.Instance.Kont[3].Trim() + " ";
                                                    }
                                                    _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                                    RtB[0].SelectedText = "mit " + _Modul1.Instance.Kont[3] + " ";
                                                    if (_Modul1.Instance.Kont[1].Trim() != "")
                                                    {
                                                        _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[1].Trim() + " " + _Modul1.Instance.Kont[0].Trim();
                                                    }
                                                    if (_Modul1.Instance.Kont[2].Trim() != "")
                                                    {
                                                        _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[0].Trim() + " " + _Modul1.Instance.Kont[2].Trim();
                                                    }
                                                    RtB[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                                    RtB[0].SelectedText = _Modul1.Instance.Kont[0];
                                                    RtB[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                                    _Modul1.Instance.Ahnles(_Modul1.Instance.PersInArb, out var asAhnData);
                                                    if (_Modul1.Instance.Kont[13].Trim() != "")
                                                    {
                                                        RtB[0].SelectedText = " " + _Modul1.Instance.Kont[13];
                                                        List3.Items.Add("          " + _Modul1.Instance.Family.Mann.AsString().Right(10) + "          " + _Modul1.Instance.Family.Frau.AsString().Right(10));
                                                    }
                                                }
                                            }
                                        }
                                        List1.Items.Clear();
                                        if (b5 == 1)
                                        {
                                            List1.Items.Clear();
                                            _Modul1.Instance.eLKennz = ELinkKennz.lkFather;
                                            aiFams = DataModul.Link.GetPersonFams(_Modul1.Instance.PersInArb, _Modul1.Instance.eLKennz);
                                            string text9 = _Modul1.Instance.UbgT;
                                            byte b6 = 1;
                                            do
                                            {
                                                if (text9.Length > 0)
                                                {
                                                    _Modul1.Instance.FamInArb = (int)Math.Round(text9.Left(10).AsDouble());
                                                    text9 = Strings.Mid(text9, 11, text9.Length);
                                                    DataModul.DB_EventTable.Index = "ArtNr";
                                                    byte b7 = 0;
                                                    int num2 = 502;
                                                    do
                                                    {
                                                        DataModul.DB_EventTable.Seek("=", num2.AsString(), _Modul1.Instance.FamInArb.AsString(), "0");
                                                        if (!DataModul.DB_EventTable.NoMatch && Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                                        {
                                                            List1.Items.Add(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim() + "          " + _Modul1.Instance.FamInArb.AsString().Right(10));
                                                            b7 = 1;
                                                            break;
                                                        }
                                                        num2++;
                                                    }
                                                    while (num2 <= 505);
                                                    if (b7 == 0)
                                                    {
                                                        List1.Items.Add("        " + "          " + _Modul1.Instance.FamInArb.AsString().Right(10));
                                                    }
                                                }
                                                if (text9.Length < 10)
                                                {
                                                    break;
                                                }
                                                b6 = (byte)unchecked((uint)(b6 + 1));
                                            }
                                            while (unchecked(b6) <= 10u);
                                            int count = List1.Items.Count;
                                            for (int num2 = 0; num2 <= count; num2++)
                                            {
                                                _Modul1.Instance.FamInArb = (int)Math.Round(Conversion.Val(Strings.Mid(List1.Items[num2].AsString(), 9, 10)));
                                                _Modul1.Instance.Famles();
                                                if ((_Modul1.Instance.PersInArb = _Modul1.Instance.Family.Frau) > 0)
                                                {
                                                    RtB[0].SelectedText = " " + _Modul1.Instance.DTxt[7] + " ";
                                                    _Modul1.Instance.Schalt = 0;
                                                    _Modul1.Instance.Famdatles2();
                                                    if (_Modul1.Instance.Kont[2].Trim() != "")
                                                    {
                                                        RtB[0].SelectedText = _Modul1.Instance.Kont[2].Trim() + " ";
                                                    }
                                                    else if (_Modul1.Instance.Kont[3].Trim() != "")
                                                    {
                                                        RtB[0].SelectedText = _Modul1.Instance.Kont[3].Trim() + " ";
                                                    }
                                                    _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                                    RtB[0].SelectedText = "mit " + _Modul1.Instance.Kont[3] + " ";
                                                    if (_Modul1.Instance.Kont[1].Trim() != "")
                                                    {
                                                        _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[1].Trim() + " " + _Modul1.Instance.Kont[0].Trim();
                                                    }
                                                    if (_Modul1.Instance.Kont[2].Trim() != "")
                                                    {
                                                        _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[0].Trim() + " " + _Modul1.Instance.Kont[2].Trim();
                                                    }
                                                    RtB[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                                    RtB[0].SelectedText = _Modul1.Instance.Kont[0];
                                                    RtB[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                                    _Modul1.Instance.Ahnles(_Modul1.Instance.PersInArb, out var asAhnData);
                                                    if (_Modul1.Instance.Kont[13].Trim() != "")
                                                    {
                                                        RtB[0].SelectedText = " " + _Modul1.Instance.Kont[13];
                                                        List3.Items.Add("          " + _Modul1.Instance.Family.Mann.AsString().Right(10) + "          " + _Modul1.Instance.Family.Frau.AsString().Right(10));
                                                    }
                                                }
                                            }
                                        }
                                        List1.Items.Clear();
                                        if (b5 == 1)
                                        {
                                            List1.Items.Clear();
                                            _Modul1.Instance.eLKennz = ELinkKennz.lkFather;
                                            aiFams = DataModul.Link.GetPersonFams(_Modul1.Instance.PersInArb, _Modul1.Instance.eLKennz);
                                            string text9 = _Modul1.Instance.UbgT;
                                            byte b6 = 1;
                                            do
                                            {
                                                if (text9.Length > 0)
                                                {
                                                    _Modul1.Instance.FamInArb = (int)Math.Round(text9.Left(10).AsDouble());
                                                    text9 = Strings.Mid(text9, 11, text9.Length);
                                                    DataModul.DB_EventTable.Index = "ArtNr";
                                                    byte b7 = 0;
                                                    int num2 = 502;
                                                    do
                                                    {
                                                        DataModul.DB_EventTable.Seek("=", num2.AsString(), _Modul1.Instance.FamInArb.AsString(), "0");
                                                        if (!DataModul.DB_EventTable.NoMatch && Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                                        {
                                                            List1.Items.Add(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim() + "          " + _Modul1.Instance.FamInArb.AsString().Right(10));
                                                            b7 = 1;
                                                            break;
                                                        }
                                                        num2++;
                                                    }
                                                    while (num2 <= 505);
                                                    if (b7 == 0)
                                                    {
                                                        List1.Items.Add("        " + "          " + _Modul1.Instance.FamInArb.AsString().Right(10));
                                                    }
                                                }
                                                if (text9.Length < 10)
                                                {
                                                    break;
                                                }
                                                b6 = (byte)unchecked((uint)(b6 + 1));
                                            }
                                            while (unchecked(b6) <= 10u);
                                            int count = List1.Items.Count;
                                            for (int num2 = 0; num2 <= count; num2++)
                                            {
                                                _Modul1.Instance.FamInArb = (int)Math.Round(Conversion.Val(Strings.Mid(List1.Items[num2].AsString(), 9, 10)));
                                                _Modul1.Instance.Famles();
                                                if ((_Modul1.Instance.PersInArb = _Modul1.Instance.Family.Frau) > 0)
                                                {
                                                    RtB[0].SelectedText = " " + _Modul1.Instance.DTxt[7] + " ";
                                                    _Modul1.Instance.Schalt = 0;
                                                    _Modul1.Instance.Famdatles2();
                                                    if (_Modul1.Instance.Kont[2].Trim() != "")
                                                    {
                                                        RtB[0].SelectedText = _Modul1.Instance.Kont[2].Trim() + " ";
                                                    }
                                                    else if (_Modul1.Instance.Kont[3].Trim() != "")
                                                    {
                                                        RtB[0].SelectedText = _Modul1.Instance.Kont[3].Trim() + " ";
                                                    }
                                                    _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                                    RtB[0].SelectedText = "mit " + _Modul1.Instance.Kont[3] + " ";
                                                    if (_Modul1.Instance.Kont[1].Trim() != "")
                                                    {
                                                        _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[1].Trim() + " " + _Modul1.Instance.Kont[0].Trim();
                                                    }
                                                    if (_Modul1.Instance.Kont[2].Trim() != "")
                                                    {
                                                        _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[0].Trim() + " " + _Modul1.Instance.Kont[2].Trim();
                                                    }
                                                    RtB[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                                    RtB[0].SelectedText = _Modul1.Instance.Kont[0];
                                                    RtB[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                                    _Modul1.Instance.Ahnles(_Modul1.Instance.PersInArb, out var asAhnData);
                                                    if (_Modul1.Instance.Kont[13].Trim() != "")
                                                    {
                                                        RtB[0].SelectedText = " " + _Modul1.Instance.Kont[13];
                                                        List3.Items.Add("          " + _Modul1.Instance.Family.Mann.AsString().Right(10) + "          " + _Modul1.Instance.Family.Frau.AsString().Right(10));
                                                    }
                                                }
                                            }
                                        }
                                        List1.Items.Clear();
                                        if (b5 == 1)
                                        {
                                            List1.Items.Clear();
                                            _Modul1.Instance.eLKennz = ELinkKennz.lkFather;
                                            aiFams = DataModul.Link.GetPersonFams(_Modul1.Instance.PersInArb, _Modul1.Instance.eLKennz);
                                            string text9 = _Modul1.Instance.UbgT;
                                            byte b6 = 1;
                                            do
                                            {
                                                if (text9.Length > 0)
                                                {
                                                    _Modul1.Instance.FamInArb = (int)Math.Round(text9.Left(10).AsDouble());
                                                    text9 = Strings.Mid(text9, 11, text9.Length);
                                                    DataModul.DB_EventTable.Index = "ArtNr";
                                                    byte b7 = 0;
                                                    int num2 = 502;
                                                    do
                                                    {
                                                        DataModul.DB_EventTable.Seek("=", num2.AsString(), _Modul1.Instance.FamInArb.AsString(), "0");
                                                        if (!DataModul.DB_EventTable.NoMatch && Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                                        {
                                                            List1.Items.Add(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim() + "          " + _Modul1.Instance.FamInArb.AsString().Right(10));
                                                            b7 = 1;
                                                            break;
                                                        }
                                                        num2++;
                                                    }
                                                    while (num2 <= 505);
                                                    if (b7 == 0)
                                                    {
                                                        List1.Items.Add("        " + "          " + _Modul1.Instance.FamInArb.AsString().Right(10));
                                                    }
                                                }
                                                if (text9.Length < 10)
                                                {
                                                    break;
                                                }
                                                b6 = (byte)unchecked((uint)(b6 + 1));
                                            }
                                            while (unchecked(b6) <= 10u);
                                            int count = List1.Items.Count;
                                            for (int num2 = 0; num2 <= count; num2++)
                                            {
                                                _Modul1.Instance.FamInArb = (int)Math.Round(Conversion.Val(Strings.Mid(List1.Items[num2].AsString(), 9, 10)));
                                                _Modul1.Instance.Famles();
                                                if ((_Modul1.Instance.PersInArb = _Modul1.Instance.Family.Frau) > 0)
                                                {
                                                    RtB[0].SelectedText = " " + _Modul1.Instance.DTxt[7] + " ";
                                                    _Modul1.Instance.Schalt = 0;
                                                    _Modul1.Instance.Famdatles2();
                                                    if (_Modul1.Instance.Kont[2].Trim() != "")
                                                    {
                                                        RtB[0].SelectedText = _Modul1.Instance.Kont[2].Trim() + " ";
                                                    }
                                                    else if (_Modul1.Instance.Kont[3].Trim() != "")
                                                    {
                                                        RtB[0].SelectedText = _Modul1.Instance.Kont[3].Trim() + " ";
                                                    }
                                                    _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                                    RtB[0].SelectedText = "mit " + _Modul1.Instance.Kont[3] + " ";
                                                    if (_Modul1.Instance.Kont[1].Trim() != "")
                                                    {
                                                        _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[1].Trim() + " " + _Modul1.Instance.Kont[0].Trim();
                                                    }
                                                    if (_Modul1.Instance.Kont[2].Trim() != "")
                                                    {
                                                        _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[0].Trim() + " " + _Modul1.Instance.Kont[2].Trim();
                                                    }
                                                    RtB[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                                    RtB[0].SelectedText = _Modul1.Instance.Kont[0];
                                                    RtB[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                                    _Modul1.Instance.Ahnles(_Modul1.Instance.PersInArb, out var asAhnData);
                                                    if (_Modul1.Instance.Kont[13].Trim() != "")
                                                    {
                                                        RtB[0].SelectedText = " " + _Modul1.Instance.Kont[13];
                                                        List3.Items.Add("          " + _Modul1.Instance.Family.Mann.AsString().Right(10) + "          " + _Modul1.Instance.Family.Frau.AsString().Right(10));
                                                    }
                                                }
                                            }
                                        }
                                        List1.Items.Clear();
                                        if (b5 == 1)
                                        {
                                            List1.Items.Clear();
                                            _Modul1.Instance.eLKennz = ELinkKennz.lkFather;
                                            aiFams = DataModul.Link.GetPersonFams(_Modul1.Instance.PersInArb, _Modul1.Instance.eLKennz);
                                            string text9 = _Modul1.Instance.UbgT;
                                            byte b6 = 1;
                                            do
                                            {
                                                if (text9.Length > 0)
                                                {
                                                    _Modul1.Instance.FamInArb = (int)Math.Round(text9.Left(10).AsDouble());
                                                    text9 = Strings.Mid(text9, 11, text9.Length);
                                                    DataModul.DB_EventTable.Index = "ArtNr";
                                                    byte b7 = 0;
                                                    int num2 = 502;
                                                    do
                                                    {
                                                        DataModul.DB_EventTable.Seek("=", num2.AsString(), _Modul1.Instance.FamInArb.AsString(), "0");
                                                        if (!DataModul.DB_EventTable.NoMatch && Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                                        {
                                                            List1.Items.Add(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim() + "          " + _Modul1.Instance.FamInArb.AsString().Right(10));
                                                            b7 = 1;
                                                            break;
                                                        }
                                                        num2++;
                                                    }
                                                    while (num2 <= 505);
                                                    if (b7 == 0)
                                                    {
                                                        List1.Items.Add("        " + "          " + _Modul1.Instance.FamInArb.AsString().Right(10));
                                                    }
                                                }
                                                if (text9.Length < 10)
                                                {
                                                    break;
                                                }
                                                b6 = (byte)unchecked((uint)(b6 + 1));
                                            }
                                            while (unchecked(b6) <= 10u);
                                            int count = List1.Items.Count;
                                            for (int num2 = 0; num2 <= count; num2++)
                                            {
                                                _Modul1.Instance.FamInArb = (int)Math.Round(Conversion.Val(Strings.Mid(List1.Items[num2].AsString(), 9, 10)));
                                                _Modul1.Instance.Famles();
                                                if ((_Modul1.Instance.PersInArb = _Modul1.Instance.Family.Frau) > 0)
                                                {
                                                    RtB[0].SelectedText = " " + _Modul1.Instance.DTxt[7] + " ";
                                                    _Modul1.Instance.Schalt = 0;
                                                    _Modul1.Instance.Famdatles2();
                                                    if (_Modul1.Instance.Kont[2].Trim() != "")
                                                    {
                                                        RtB[0].SelectedText = _Modul1.Instance.Kont[2].Trim() + " ";
                                                    }
                                                    else if (_Modul1.Instance.Kont[3].Trim() != "")
                                                    {
                                                        RtB[0].SelectedText = _Modul1.Instance.Kont[3].Trim() + " ";
                                                    }
                                                    _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                                    RtB[0].SelectedText = "mit " + _Modul1.Instance.Kont[3] + " ";
                                                    if (_Modul1.Instance.Kont[1].Trim() != "")
                                                    {
                                                        _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[1].Trim() + " " + _Modul1.Instance.Kont[0].Trim();
                                                    }
                                                    if (_Modul1.Instance.Kont[2].Trim() != "")
                                                    {
                                                        _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[0].Trim() + " " + _Modul1.Instance.Kont[2].Trim();
                                                    }
                                                    RtB[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                                    RtB[0].SelectedText = _Modul1.Instance.Kont[0];
                                                    RtB[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                                    _Modul1.Instance.Ahnles(_Modul1.Instance.PersInArb, out var asAhnData);
                                                    if (_Modul1.Instance.Kont[13].Trim() != "")
                                                    {
                                                        RtB[0].SelectedText = " " + _Modul1.Instance.Kont[13];
                                                        List3.Items.Add("          " + _Modul1.Instance.Family.Mann.AsString().Right(10) + "          " + _Modul1.Instance.Family.Frau.AsString().Right(10));
                                                    }
                                                }
                                            }
                                        }
                                        List1.Items.Clear();
                                        if (b5 == 1)
                                        {
                                            List1.Items.Clear();
                                            _Modul1.Instance.eLKennz = ELinkKennz.lkFather;
                                            aiFams = DataModul.Link.GetPersonFams(_Modul1.Instance.PersInArb, _Modul1.Instance.eLKennz);
                                            string text9 = _Modul1.Instance.UbgT;
                                            byte b6 = 1;
                                            do
                                            {
                                                if (text9.Length > 0)
                                                {
                                                    _Modul1.Instance.FamInArb = (int)Math.Round(text9.Left(10).AsDouble());
                                                    text9 = Strings.Mid(text9, 11, text9.Length);
                                                    DataModul.DB_EventTable.Index = "ArtNr";
                                                    byte b7 = 0;
                                                    int num2 = 502;
                                                    do
                                                    {
                                                        DataModul.DB_EventTable.Seek("=", num2.AsString(), _Modul1.Instance.FamInArb.AsString(), "0");
                                                        if (!DataModul.DB_EventTable.NoMatch && Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                                        {
                                                            List1.Items.Add(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim() + "          " + _Modul1.Instance.FamInArb.AsString().Right(10));
                                                            b7 = 1;
                                                            break;
                                                        }
                                                        num2++;
                                                    }
                                                    while (num2 <= 505);
                                                    if (b7 == 0)
                                                    {
                                                        List1.Items.Add("        " + "          " + _Modul1.Instance.FamInArb.AsString().Right(10));
                                                    }
                                                }
                                                if (text9.Length < 10)
                                                {
                                                    break;
                                                }
                                                b6 = (byte)unchecked((uint)(b6 + 1));
                                            }
                                            while (unchecked(b6) <= 10u);
                                            int count = List1.Items.Count;
                                            for (int num2 = 0; num2 <= count; num2++)
                                            {
                                                _Modul1.Instance.FamInArb = (int)Math.Round(Conversion.Val(Strings.Mid(List1.Items[num2].AsString(), 9, 10)));
                                                _Modul1.Instance.Famles();
                                                if ((_Modul1.Instance.PersInArb = _Modul1.Instance.Family.Frau) > 0)
                                                {
                                                    RtB[0].SelectedText = " " + _Modul1.Instance.DTxt[7] + " ";
                                                    _Modul1.Instance.Schalt = 0;
                                                    _Modul1.Instance.Famdatles2();
                                                    if (_Modul1.Instance.Kont[2].Trim() != "")
                                                    {
                                                        RtB[0].SelectedText = _Modul1.Instance.Kont[2].Trim() + " ";
                                                    }
                                                    else if (_Modul1.Instance.Kont[3].Trim() != "")
                                                    {
                                                        RtB[0].SelectedText = _Modul1.Instance.Kont[3].Trim() + " ";
                                                    }
                                                    _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                                    RtB[0].SelectedText = "mit " + _Modul1.Instance.Kont[3] + " ";
                                                    if (_Modul1.Instance.Kont[1].Trim() != "")
                                                    {
                                                        _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[1].Trim() + " " + _Modul1.Instance.Kont[0].Trim();
                                                    }
                                                    if (_Modul1.Instance.Kont[2].Trim() != "")
                                                    {
                                                        _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[0].Trim() + " " + _Modul1.Instance.Kont[2].Trim();
                                                    }
                                                    RtB[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                                    RtB[0].SelectedText = _Modul1.Instance.Kont[0];
                                                    RtB[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                                    _Modul1.Instance.Ahnles(_Modul1.Instance.PersInArb, out var asAhnData);
                                                    if (_Modul1.Instance.Kont[13].Trim() != "")
                                                    {
                                                        RtB[0].SelectedText = " " + _Modul1.Instance.Kont[13];
                                                        List3.Items.Add("          " + _Modul1.Instance.Family.Mann.AsString().Right(10) + "          " + _Modul1.Instance.Family.Frau.AsString().Right(10));
                                                    }
                                                }
                                            }
                                        }
                                        List1.Items.Clear();
                                        if (b5 == 2)
                                        {
                                            List1.Items.Clear();
                                            _Modul1.Instance.eLKennz = ELinkKennz.lkMother;
                                            aiFams = DataModul.Link.GetPersonFams(_Modul1.Instance.PersInArb, _Modul1.Instance.eLKennz);
                                            string text9 = _Modul1.Instance.UbgT;
                                            byte b6 = 1;
                                            do
                                            {
                                                if (text9.Length > 0)
                                                {
                                                    _Modul1.Instance.FamInArb = (int)Math.Round(text9.Left(10).AsDouble());
                                                    text9 = Strings.Mid(text9, 11, text9.Length);
                                                    DataModul.DB_EventTable.Index = "ArtNr";
                                                    byte b7 = 0;
                                                    int num2 = 502;
                                                    do
                                                    {
                                                        DataModul.DB_EventTable.Seek("=", num2.AsString(), _Modul1.Instance.FamInArb.AsString(), "0");
                                                        if (!DataModul.DB_EventTable.NoMatch && Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                                        {
                                                            List1.Items.Add(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim() + "          " + _Modul1.Instance.FamInArb.AsString().Right(10));
                                                            b7 = 1;
                                                            break;
                                                        }
                                                        num2++;
                                                    }
                                                    while (num2 <= 505);
                                                    if (b7 == 0)
                                                    {
                                                        List1.Items.Add("        " + "          " + _Modul1.Instance.FamInArb.AsString().Right(10));
                                                    }
                                                }
                                                if (text9.Length < 10)
                                                {
                                                    break;
                                                }
                                                b6 = (byte)unchecked((uint)(b6 + 1));
                                            }
                                            while (unchecked(b6) <= 10u);
                                            int count = List1.Items.Count;
                                            for (int num2 = 0; num2 <= count; num2++)
                                            {
                                                _Modul1.Instance.FamInArb = (int)Math.Round(Conversion.Val(Strings.Mid(List1.Items[num2].AsString(), 9, 10)));
                                                _Modul1.Instance.Famles();
                                                if ((_Modul1.Instance.PersInArb = _Modul1.Instance.Family.Frau) > 0)
                                                {
                                                    RtB[0].SelectedText = " " + _Modul1.Instance.DTxt[7] + " ";
                                                    _Modul1.Instance.Schalt = 0;
                                                    _Modul1.Instance.Famdatles2();
                                                    if (_Modul1.Instance.Kont[2].Trim() != "")
                                                    {
                                                        RtB[0].SelectedText = _Modul1.Instance.Kont[2].Trim() + " ";
                                                    }
                                                    else if (_Modul1.Instance.Kont[3].Trim() != "")
                                                    {
                                                        RtB[0].SelectedText = _Modul1.Instance.Kont[3].Trim() + " ";
                                                    }
                                                    _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                                    RtB[0].SelectedText = "mit " + _Modul1.Instance.Kont[3] + " ";
                                                    if (_Modul1.Instance.Kont[1].Trim() != "")
                                                    {
                                                        _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[1].Trim() + " " + _Modul1.Instance.Kont[0].Trim();
                                                    }
                                                    if (_Modul1.Instance.Kont[2].Trim() != "")
                                                    {
                                                        _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[0].Trim() + " " + _Modul1.Instance.Kont[2].Trim();
                                                    }
                                                    RtB[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                                    RtB[0].SelectedText = _Modul1.Instance.Kont[0];
                                                    RtB[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                                    _Modul1.Instance.Ahnles(_Modul1.Instance.PersInArb, out var asAhnData);
                                                    if (_Modul1.Instance.Kont[13].Trim() != "")
                                                    {
                                                        RtB[0].SelectedText = " " + _Modul1.Instance.Kont[13];
                                                        List3.Items.Add("          " + _Modul1.Instance.Family.Mann.AsString().Right(10) + "          " + _Modul1.Instance.Family.Frau.AsString().Right(10));
                                                    }
                                                }
                                            }
                                        }
                                        List1.Items.Clear();
                                        if (b5 == 1)
                                        {
                                            List1.Items.Clear();
                                            _Modul1.Instance.eLKennz = ELinkKennz.lkFather;
                                            aiFams = DataModul.Link.GetPersonFams(_Modul1.Instance.PersInArb, _Modul1.Instance.eLKennz);
                                            string text9 = _Modul1.Instance.UbgT;
                                            byte b6 = 1;
                                            do
                                            {
                                                if (text9.Length > 0)
                                                {
                                                    _Modul1.Instance.FamInArb = (int)Math.Round(text9.Left(10).AsDouble());
                                                    text9 = Strings.Mid(text9, 11, text9.Length);
                                                    DataModul.DB_EventTable.Index = "ArtNr";
                                                    byte b7 = 0;
                                                    int num2 = 502;
                                                    do
                                                    {
                                                        DataModul.DB_EventTable.Seek("=", num2.AsString(), _Modul1.Instance.FamInArb.AsString(), "0");
                                                        if (!DataModul.DB_EventTable.NoMatch && Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                                        {
                                                            List1.Items.Add(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim() + "          " + _Modul1.Instance.FamInArb.AsString().Right(10));
                                                            b7 = 1;
                                                            break;
                                                        }
                                                        num2++;
                                                    }
                                                    while (num2 <= 505);
                                                    if (b7 == 0)
                                                    {
                                                        List1.Items.Add("        " + "          " + _Modul1.Instance.FamInArb.AsString().Right(10));
                                                    }
                                                }
                                                if (text9.Length < 10)
                                                {
                                                    break;
                                                }
                                                b6 = (byte)unchecked((uint)(b6 + 1));
                                            }
                                            while (unchecked(b6) <= 10u);
                                            int count = List1.Items.Count;
                                            for (int num2 = 0; num2 <= count; num2++)
                                            {
                                                _Modul1.Instance.FamInArb = (int)Math.Round(Conversion.Val(Strings.Mid(List1.Items[num2].AsString(), 9, 10)));
                                                _Modul1.Instance.Famles();
                                                if ((_Modul1.Instance.PersInArb = _Modul1.Instance.Family.Frau) > 0)
                                                {
                                                    RtB[0].SelectedText = " " + _Modul1.Instance.DTxt[7] + " ";
                                                    _Modul1.Instance.Schalt = 0;
                                                    _Modul1.Instance.Famdatles2();
                                                    if (_Modul1.Instance.Kont[2].Trim() != "")
                                                    {
                                                        RtB[0].SelectedText = _Modul1.Instance.Kont[2].Trim() + " ";
                                                    }
                                                    else if (_Modul1.Instance.Kont[3].Trim() != "")
                                                    {
                                                        RtB[0].SelectedText = _Modul1.Instance.Kont[3].Trim() + " ";
                                                    }
                                                    _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                                    RtB[0].SelectedText = "mit " + _Modul1.Instance.Kont[3] + " ";
                                                    if (_Modul1.Instance.Kont[1].Trim() != "")
                                                    {
                                                        _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[1].Trim() + " " + _Modul1.Instance.Kont[0].Trim();
                                                    }
                                                    if (_Modul1.Instance.Kont[2].Trim() != "")
                                                    {
                                                        _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[0].Trim() + " " + _Modul1.Instance.Kont[2].Trim();
                                                    }
                                                    RtB[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                                    RtB[0].SelectedText = _Modul1.Instance.Kont[0];
                                                    RtB[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                                    _Modul1.Instance.Ahnles(_Modul1.Instance.PersInArb, out var asAhnData);
                                                    if (_Modul1.Instance.Kont[13].Trim() != "")
                                                    {
                                                        RtB[0].SelectedText = " " + _Modul1.Instance.Kont[13];
                                                        List3.Items.Add("          " + _Modul1.Instance.Family.Mann.AsString().Right(10) + "          " + _Modul1.Instance.Family.Frau.AsString().Right(10));
                                                    }
                                                }
                                            }
                                        }
                                        List1.Items.Clear();
                                        if (b5 == 1)
                                        {
                                            List1.Items.Clear();
                                            _Modul1.Instance.eLKennz = ELinkKennz.lkFather;
                                            aiFams = DataModul.Link.GetPersonFams(_Modul1.Instance.PersInArb, _Modul1.Instance.eLKennz);
                                            string text9 = _Modul1.Instance.UbgT;
                                            byte b6 = 1;
                                            do
                                            {
                                                if (text9.Length > 0)
                                                {
                                                    _Modul1.Instance.FamInArb = (int)Math.Round(text9.Left(10).AsDouble());
                                                    text9 = Strings.Mid(text9, 11, text9.Length);
                                                    DataModul.DB_EventTable.Index = "ArtNr";
                                                    byte b7 = 0;
                                                    int num2 = 502;
                                                    do
                                                    {
                                                        DataModul.DB_EventTable.Seek("=", num2.AsString(), _Modul1.Instance.FamInArb.AsString(), "0");
                                                        if (!DataModul.DB_EventTable.NoMatch && Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                                        {
                                                            List1.Items.Add(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim() + "          " + _Modul1.Instance.FamInArb.AsString().Right(10));
                                                            b7 = 1;
                                                            break;
                                                        }
                                                        num2++;
                                                    }
                                                    while (num2 <= 505);
                                                    if (b7 == 0)
                                                    {
                                                        List1.Items.Add("        " + "          " + _Modul1.Instance.FamInArb.AsString().Right(10));
                                                    }
                                                }
                                                if (text9.Length < 10)
                                                {
                                                    break;
                                                }
                                                b6 = (byte)unchecked((uint)(b6 + 1));
                                            }
                                            while (unchecked(b6) <= 10u);
                                            int count = List1.Items.Count;
                                            for (int num2 = 0; num2 <= count; num2++)
                                            {
                                                _Modul1.Instance.FamInArb = (int)Math.Round(Conversion.Val(Strings.Mid(List1.Items[num2].AsString(), 9, 10)));
                                                _Modul1.Instance.Famles();
                                                if ((_Modul1.Instance.PersInArb = _Modul1.Instance.Family.Frau) > 0)
                                                {
                                                    RtB[0].SelectedText = " " + _Modul1.Instance.DTxt[7] + " ";
                                                    _Modul1.Instance.Schalt = 0;
                                                    _Modul1.Instance.Famdatles2();
                                                    if (_Modul1.Instance.Kont[2].Trim() != "")
                                                    {
                                                        RtB[0].SelectedText = _Modul1.Instance.Kont[2].Trim() + " ";
                                                    }
                                                    else if (_Modul1.Instance.Kont[3].Trim() != "")
                                                    {
                                                        RtB[0].SelectedText = _Modul1.Instance.Kont[3].Trim() + " ";
                                                    }
                                                    _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                                    RtB[0].SelectedText = "mit " + _Modul1.Instance.Kont[3] + " ";
                                                    if (_Modul1.Instance.Kont[1].Trim() != "")
                                                    {
                                                        _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[1].Trim() + " " + _Modul1.Instance.Kont[0].Trim();
                                                    }
                                                    if (_Modul1.Instance.Kont[2].Trim() != "")
                                                    {
                                                        _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[0].Trim() + " " + _Modul1.Instance.Kont[2].Trim();
                                                    }
                                                    RtB[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                                    RtB[0].SelectedText = _Modul1.Instance.Kont[0];
                                                    RtB[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                                    _Modul1.Instance.Ahnles(_Modul1.Instance.PersInArb, out var asAhnData);
                                                    if (_Modul1.Instance.Kont[13].Trim() != "")
                                                    {
                                                        RtB[0].SelectedText = " " + _Modul1.Instance.Kont[13];
                                                        List3.Items.Add("          " + _Modul1.Instance.Family.Mann.AsString().Right(10) + "          " + _Modul1.Instance.Family.Frau.AsString().Right(10));
                                                    }
                                                }
                                            }
                                        }
                                        List1.Items.Clear();
                                        if (b5 == 1)
                                        {
                                            List1.Items.Clear();
                                            _Modul1.Instance.eLKennz = ELinkKennz.lkFather;
                                            aiFams = DataModul.Link.GetPersonFams(_Modul1.Instance.PersInArb, _Modul1.Instance.eLKennz);
                                            string text9 = _Modul1.Instance.UbgT;
                                            byte b6 = 1;
                                            do
                                            {
                                                if (text9.Length > 0)
                                                {
                                                    _Modul1.Instance.FamInArb = (int)Math.Round(text9.Left(10).AsDouble());
                                                    text9 = Strings.Mid(text9, 11, text9.Length);
                                                    DataModul.DB_EventTable.Index = "ArtNr";
                                                    byte b7 = 0;
                                                    int num2 = 502;
                                                    do
                                                    {
                                                        DataModul.DB_EventTable.Seek("=", num2.AsString(), _Modul1.Instance.FamInArb.AsString(), "0");
                                                        if (!DataModul.DB_EventTable.NoMatch && Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                                        {
                                                            List1.Items.Add(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim() + "          " + _Modul1.Instance.FamInArb.AsString().Right(10));
                                                            b7 = 1;
                                                            break;
                                                        }
                                                        num2++;
                                                    }
                                                    while (num2 <= 505);
                                                    if (b7 == 0)
                                                    {
                                                        List1.Items.Add("        " + "          " + _Modul1.Instance.FamInArb.AsString().Right(10));
                                                    }
                                                }
                                                if (text9.Length < 10)
                                                {
                                                    break;
                                                }
                                                b6 = (byte)unchecked((uint)(b6 + 1));
                                            }
                                            while (unchecked(b6) <= 10u);
                                            int count = List1.Items.Count;
                                            for (int num2 = 0; num2 <= count; num2++)
                                            {
                                                _Modul1.Instance.FamInArb = (int)Math.Round(Conversion.Val(Strings.Mid(List1.Items[num2].AsString(), 9, 10)));
                                                _Modul1.Instance.Famles();
                                                if ((_Modul1.Instance.PersInArb = _Modul1.Instance.Family.Frau) > 0)
                                                {
                                                    RtB[0].SelectedText = " " + _Modul1.Instance.DTxt[7] + " ";
                                                    _Modul1.Instance.Schalt = 0;
                                                    _Modul1.Instance.Famdatles2();
                                                    if (_Modul1.Instance.Kont[2].Trim() != "")
                                                    {
                                                        RtB[0].SelectedText = _Modul1.Instance.Kont[2].Trim() + " ";
                                                    }
                                                    else if (_Modul1.Instance.Kont[3].Trim() != "")
                                                    {
                                                        RtB[0].SelectedText = _Modul1.Instance.Kont[3].Trim() + " ";
                                                    }
                                                    _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                                    RtB[0].SelectedText = "mit " + _Modul1.Instance.Kont[3] + " ";
                                                    if (_Modul1.Instance.Kont[1].Trim() != "")
                                                    {
                                                        _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[1].Trim() + " " + _Modul1.Instance.Kont[0].Trim();
                                                    }
                                                    if (_Modul1.Instance.Kont[2].Trim() != "")
                                                    {
                                                        _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[0].Trim() + " " + _Modul1.Instance.Kont[2].Trim();
                                                    }
                                                    RtB[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                                    RtB[0].SelectedText = _Modul1.Instance.Kont[0];
                                                    RtB[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                                    _Modul1.Instance.Ahnles(_Modul1.Instance.PersInArb, out var asAhnData);
                                                    if (_Modul1.Instance.Kont[13].Trim() != "")
                                                    {
                                                        RtB[0].SelectedText = " " + _Modul1.Instance.Kont[13];
                                                        List3.Items.Add("          " + _Modul1.Instance.Family.Mann.AsString().Right(10) + "          " + _Modul1.Instance.Family.Frau.AsString().Right(10));
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                while (false);
                            }
                        }
                    }
                    break;
            }
        }
    }

    private void Sippenlist_Load(object eventSender, EventArgs eventArgs)
    {
        int try0000_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        string source = default;
        string destination = default;
        string name = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                int num4;
                Hinter hinter;
                short Listart;
                EEventArt Art;
                bool neb; switch (try0000_dispatch)
                {
                    default:
                        num = 1;
                        _Modul1.Instance.DAus[101] = _Modul1.Instance.Font1;
                        goto IL_0010;
                    case 2072:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 3:
                                    break;
                                case 2:
                                    goto IL_05c9;
                                case 1:
                                    goto IL_066a;
                                default:
                                    goto end_IL_0000;
                            }
                            goto IL_0565;
                        }
                    end_IL_0000:
                        break;
                    IL_0010:
                        num = 2;
                        _Modul1.Instance.DAus[102] = "10";
                        _Modul1.Instance.Dateienopen();
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        BackColor = _Modul1.Instance.HintFarb;
                        FileSystem.MkDir(_Modul1.Instance.Verz + "Sippe");
                        hinter = MyProject.Forms.Hinter;
                        hinter.Att(_Modul1.Instance.Verz + "Sippe");
                        FileSystem.Kill(_Modul1.Instance.Verz + "Sippe\\*.*");
                        _Modul1.Instance.Verz1 = _Modul1.Instance.Verz.Left(15);
                        source = _Modul1.Instance.GenFreeDir + "INIT\\GedAUS.mdb";
                        destination = _Modul1.Instance.Verz + "Sippe\\GEDAUS.mdb";
                        FileSystem.FileCopy(source, destination);
                        name = _Modul1.Instance.Verz + "Sippe\\GEDAUS.mdb";
                        DataModul.NB = UpgradeSupport.DAODBEngine_definst.OpenDatabase(name, false, false, "");
                        DataModul.NB_PersonTable = DataModul.NB.OpenRecordset(dbTables.Personen1, RecordsetTypeEnum.dbOpenTable);
                        DataModul.NB_FamilyTable = DataModul.NB.OpenRecordset(dbTables.Familie1, RecordsetTypeEnum.dbOpenTable);
                        ProjectData.ClearProjectError();
                        num3 = 3;
                        _Modul1.Instance.Feg = (short)_Modul1.Instance.Persistence.ReadIntInit("state");
                        _Modul1.Instance.Fs = _Modul1.Instance.Feg switch
                        {
                            0 => 7.8f,
                            1 => 8.7f,
                            2 => 9.5f,
                            3 => 10.3f,
                            4 => 11f,
                            5 => 11.7f,
                            6 => 12.4f,
                            7 => 13.2f,
                            8 => 14.9f,
                            9 => 16.5f,
                            _ => _Modul1.Instance.Fs,
                        };
                        goto IL_02fd;
                    IL_02fd: // <========== 12
                        num = 58;
                        Font = new Font("Arial", _Modul1.Instance.Fs, FontStyle.Regular);
                        DataModul.DT_DescendentTable.Index = "NR";
                        DataModul.DT_DescendentTable.MoveFirst();
                        _Modul1.Instance.PersInArb = DataModul.DT_DescendentTable.Fields["Pr"].AsInt();
                        _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                        if (_Modul1.Instance.Kont[1].Trim() != "")
                        {
                            _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[1] + " " + _Modul1.Instance.Kont[0];
                        }
                        if (_Modul1.Instance.Kont[2].Trim() != "")
                        {
                            _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[0] + " " + _Modul1.Instance.Kont[2];
                        }
                        Label1[0].Text = _Modul1.Instance.Kont[3] + " " + _Modul1.Instance.Kont[0];
                        Listart = 0;
                        Art = default;
                        neb = false;
                        _Modul1.Instance.Datles3(Listart, default, Art, ref neb);
                        if (_Modul1.Instance.Kont[11] != "")
                        {
                            _Modul1.Instance.Kont[11] = "Geb.: " + _Modul1.Instance.Kont[11];
                        }
                        if (_Modul1.Instance.Kont[11] == "")
                        {
                            _Modul1.Instance.Kont[11] = "Get.: " + _Modul1.Instance.Kont[12];
                        }
                        if (_Modul1.Instance.Kont[13] != "")
                        {
                            _Modul1.Instance.Kont[13] = "Gest.: " + _Modul1.Instance.Kont[13];
                        }
                        if (_Modul1.Instance.Kont[13] == "")
                        {
                            _Modul1.Instance.Kont[13] = "Begr.: " + _Modul1.Instance.Kont[14];
                        }
                        Label1[1].Text = _Modul1.Instance.Kont[11];
                        Label1[2].Text = _Modul1.Instance.Kont[13];
                        goto IL_0565;
                    IL_0565: // <========== 3
                        num = 85;
                        if (Information.Err().Number != 3021)
                        {
                        }
                        else
                        {

                            Label1[3].Text = "Keine Berechnung vorhanden";
                            Label1[0].Text = "Sie müssen erst die Nachfahren berechnen.";
                            Command_Renamed[1].Enabled = false;
                        }
                        goto end_IL_0000_2;
                    IL_05c9:
                        num = 91;
                        if (Information.Err().Number == 75)
                        {
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_066a;
                        }
                        if (Information.Err().Number == 53)
                        {
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_066a;
                        }
                        if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                        {
                            ProjectData.EndApp();
                        }
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        num4 = num2;
                        goto IL_066e;
                    IL_066a: // <========== 3
                        num4 = num2 + 1;
                        goto IL_066e;
                    IL_066e:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 26:
                            case 30:
                            case 33:
                            case 36:
                            case 39:
                            case 42:
                            case 45:
                            case 48:
                            case 51:
                            case 54:
                            case 57:
                            case 58:
                                goto IL_02fd;
                            case 85:
                                goto IL_0565;
                            case 89:
                            case 90:
                            case 103:
                                goto end_IL_0000_2;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 2072;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0000_2: // <========== 3
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    public void Personlesen()
    {
        _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
        string M_Namen = _Modul1.Instance.Kont[0];
        _Modul1.Instance.Ind1 = (DataModul.DT_DescendentTable.Fields["gen"].AsString() + "- " + DataModul.DT_DescendentTable.Fields["Nr"].AsString());
        Namenindex();
        if (_Modul1.Instance.Kont[1] != "")
        {
            _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[1] + " " + _Modul1.Instance.Kont[0];
        }
        if (_Modul1.Instance.Kont[2] != "")
        {
            _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[0] + " " + _Modul1.Instance.Kont[2];
        }
        RtB[0].SelectedText = _Modul1.Instance.Kont[3] + " ";
        RtB[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
        RtB[0].SelectedText = _Modul1.Instance.Kont[0];
        RtB[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
        if (_Modul1.Instance.Kont[4].Trim() != "")
        {
            RtB[0].SelectedText = " (" + _Modul1.Instance.Kont[4] + ")";
        }
        short Listart = 0;
        bool neb = false;
        _Modul1.Instance.Datles3(Listart, default, default, ref neb);
        M_Namen = "";
        if (_Modul1.Instance.Kont[11] != "")
        {
            _Modul1.Instance.Kont[11] = " " + _Modul1.Instance.DTxt[1] + " " + _Modul1.Instance.Kont[11];
        }
        if (_Modul1.Instance.Kont[12] != "" && _Modul1.Instance.Kont[11] == "")
        {
            _Modul1.Instance.Kont[11] = " " + _Modul1.Instance.DTxt[2] + " " + _Modul1.Instance.Kont[12];
        }
        if (_Modul1.Instance.Kont[13] != "")
        {
            _Modul1.Instance.Kont[13] = _Modul1.Instance.DTxt[3] + " " + _Modul1.Instance.Kont[13];
        }
        if (_Modul1.Instance.Kont[14] != "" && _Modul1.Instance.Kont[13] == "")
        {
            _Modul1.Instance.Kont[13] = _Modul1.Instance.DTxt[4] + " " + _Modul1.Instance.Kont[14];
        }
        RtB[0].SelectedText = _Modul1.Instance.Kont[11] + " " + _Modul1.Instance.Kont[13].Replace("  ", " ");
        _Modul1.Instance.Ubg = 300;
        Berufe();
        _Modul1.Instance.Ubg = 301;
        Berufe();
    }

    public void Namenindex()
    {
        if (_Modul1.Instance.Kont[0] != "" && _Modul1.Instance.Kont[0] != "NN" && _Modul1.Instance.Kont[0] != "N.N.")
        {
            DataModul.DSB_NamIdxTable.AddNew();
            if (_Modul1.Instance.Kont[3] == "")
            {
                DataModul.DSB_NamIdxTable.Fields["Name"].Value = "?";
            }
            else
            {
                DataModul.DSB_NamIdxTable.Fields["Name"].Value = _Modul1.Instance.Kont[99];
            }
            DataModul.DSB_NamIdxTable.Fields["Name1"].Value = _Modul1.Instance.Kont[0];
            DataModul.DSB_NamIdxTable.Fields["Ind"].Value = _Modul1.Instance.Ind1;
            M_Namen = _Modul1.Instance.Kont[0];
            DataModul.DSB_NamIdxTable.Update();
        }
    }

    public void Berufe()
    {
        Monitor.Enter(_0024STATIC_0024Berufe_00242001_0024Job_0024Init);
        try
        {
            if (_0024STATIC_0024Berufe_00242001_0024Job_0024Init.State == 0)
            {
                _0024STATIC_0024Berufe_00242001_0024Job_0024Init.State = 2;
                _0024STATIC_0024Berufe_00242001_0024Job = "";
            }
            else if (_0024STATIC_0024Berufe_00242001_0024Job_0024Init.State == 2)
            {
                throw new IncompleteInitialization();
            }
        }
        finally
        {
            _0024STATIC_0024Berufe_00242001_0024Job_0024Init.State = 1;
            Monitor.Exit(_0024STATIC_0024Berufe_00242001_0024Job_0024Init);
        }
        Monitor.Enter(_0024STATIC_0024Berufe_00242001_0024Job1_0024Init);
        try
        {
            if (_0024STATIC_0024Berufe_00242001_0024Job1_0024Init.State == 0)
            {
                _0024STATIC_0024Berufe_00242001_0024Job1_0024Init.State = 2;
                _0024STATIC_0024Berufe_00242001_0024Job1 = "";
            }
            else if (_0024STATIC_0024Berufe_00242001_0024Job1_0024Init.State == 2)
            {
                throw new IncompleteInitialization();
            }
        }
        finally
        {
            _0024STATIC_0024Berufe_00242001_0024Job1_0024Init.State = 1;
            Monitor.Exit(_0024STATIC_0024Berufe_00242001_0024Job1_0024Init);
        }
        Monitor.Enter(_0024STATIC_0024Berufe_00242001_0024Datu1_0024Init);
        try
        {
            if (_0024STATIC_0024Berufe_00242001_0024Datu1_0024Init.State == 0)
            {
                _0024STATIC_0024Berufe_00242001_0024Datu1_0024Init.State = 2;
                _0024STATIC_0024Berufe_00242001_0024Datu1 = "";
            }
            else if (_0024STATIC_0024Berufe_00242001_0024Datu1_0024Init.State == 2)
            {
                throw new IncompleteInitialization();
            }
        }
        finally
        {
            _0024STATIC_0024Berufe_00242001_0024Datu1_0024Init.State = 1;
            Monitor.Exit(_0024STATIC_0024Berufe_00242001_0024Datu1_0024Init);
        }
        List1.Items.Clear();
        checked
        {
            Modul1_Beruf = (EEventArt)_Modul1.Instance.Ubg;
            DataModul.DB_EventTable.Index = "Besu";
            DataModul.DB_EventTable.Seek("=", Modul1_Beruf.AsString(), _Modul1.Instance.PersInArb.AsString());
            if (DataModul.DB_EventTable.NoMatch)
            {
                DataModul.DB_EventTable.Index = "ArtNr";
                return;
            }
            _0024STATIC_0024Berufe_00242001_0024Job1 = "";
            _0024STATIC_0024Berufe_00242001_0024I1 = 1;
            while (!DataModul.DB_EventTable.EOF)
            {
                if (!Operators.ConditionalCompareObjectLess(DataModul.DB_EventTable.Fields[EventFields.LfNr].Value, 1, TextCompare: false))
                {
                    int M1_J = 0;
                    do
                    {
                        _Modul1.Instance.Kont[M1_J] = "";
                        M1_J = (byte)unchecked((uint)(M1_J + 1));
                    }
                    while (unchecked(M1_J) <= 15u);
                    _Modul1.Instance.Ubg = _0024STATIC_0024Berufe_00242001_0024I1;
                    _Modul1.Instance.sDatu = "";
                    if (Conversions.ToBoolean(DataModul.DB_EventTable.NoMatch
                        | (DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != _Modul1.Instance.PersInArb)
                        | (DataModul.DB_EventTable.Fields[EventFields.Art].AsEnum<EEventArt>() != Modul1_Beruf)))
                    {
                        DataModul.DB_EventTable.Index = "ArtNr";
                        break;
                    }
                    _Modul1.Instance.UbgT = "";
                    _Modul1.Instance.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                    string ds = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                    if (_Modul1.Instance.sDatu.AsDouble() > 0.0)
                    {
                        _Modul1.Instance.Datwand1(ref _Modul1.Instance.sDatu, ds);
                        if (DataModul.DB_EventTable.Fields[EventFields.DatumB].AsInt() > 0)
                        {
                            _0024STATIC_0024Berufe_00242001_0024Datu1 = "Von " + _Modul1.Instance.sDatu;
                        }
                        else
                        {
                            _0024STATIC_0024Berufe_00242001_0024Datu1 = " " + _Modul1.Instance.sDatu;
                        }
                    }
                    else
                    {
                        _0024STATIC_0024Berufe_00242001_0024Datu1 = "";
                    }
                    ds = "";
                    ds = DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString();
                    _Modul1.Instance.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumB].AsString();
                    if (_Modul1.Instance.sDatu.AsDouble() > 0.0)
                    {
                        _Modul1.Instance.Datwand1(ref _Modul1.Instance.sDatu, ds);
                        if ((_Modul1.Instance.sDatu != "") & (ds.Trim() == ""))
                        {
                            _Modul1.Instance.sDatu = " bis " + _Modul1.Instance.sDatu;
                        }
                    }
                    else
                    {
                        _Modul1.Instance.sDatu = "";
                    }
                    _Modul1.Instance.sDatu = _0024STATIC_0024Berufe_00242001_0024Datu1 + _Modul1.Instance.sDatu;
                    if (DataModul.DB_EventTable.Fields[EventFields.Ort].Value.AsDouble() > 0.0)
                    {
                        int ortNr = (int)Math.Round(DataModul.DB_EventTable.Fields[EventFields.Ort].Value.AsDouble());
                        _Modul1.Instance.UbgT = _Modul1.Instance.ortles(ortNr, 1);
                        _Modul1.Instance.Kont[1] = " " + _Modul1.Instance.UbgT.Trim();
                        _Modul1.Instance.UbgT = "";
                    }
                    if (DataModul.DB_EventTable.Fields[EventFields.KBem].Value.AsDouble() > 0.0)
                    {
                        int AAA = DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt();
                        string LD = "";
                        _Modul1.Instance.Kont[0] = DataModul.TextLese1(AAA);
                        if (_Modul1.Instance.Kont[0] != "")
                        {
                            _Modul1.Instance.Kont[8] = " " + _Modul1.Instance.Kont[0].Trim() + " ";
                        }
                    }
                    if (DataModul.DB_EventTable.Fields[EventFields.Platz].Value.AsDouble() > 0.0)
                    {
                        int AAA = DataModul.DB_EventTable.Fields[EventFields.Platz].AsInt();
                        string LD = "";
                        _Modul1.Instance.Kont[0] = DataModul.TextLese1(AAA);
                        if (_Modul1.Instance.Kont[0] != "")
                        {
                            _Modul1.Instance.Kont[9] = " " + _Modul1.Instance.Kont[0].Trim();
                        }
                    }
                    if (_Modul1.Instance.Aus[2] == "Y" && (DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString() != " "))
                    {
                        _Modul1.Instance.Kont[3] = DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim();
                    }
                    if (_Modul1.Instance.Aus[3] == "Y" && (DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString() != " "))
                    {
                        _Modul1.Instance.Kont[4] = DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim();
                    }
                    if ((_Modul1.Instance.Kont[3] != "") | (_Modul1.Instance.Kont[4] != ""))
                    {
                        _Modul1.Instance.Kont[2] = " {" + _Modul1.Instance.Kont[3].Trim();
                        if (_Modul1.Instance.Kont[3] == "")
                        {
                            _Modul1.Instance.Kont[2] = " {" + _Modul1.Instance.Kont[4].Trim();
                        }
                        else
                        {
                            _Modul1.Instance.Kont[2] = _Modul1.Instance.Kont[2] + " " + _Modul1.Instance.Kont[4].Trim();
                        }
                        _Modul1.Instance.Kont[2] = _Modul1.Instance.Kont[2].Trim() + "} ";
                    }
                    if (DataModul.DB_EventTable.Fields[EventFields.Reg].AsString() != " ")
                    {
                        _0024STATIC_0024Berufe_00242001_0024Job1 = _Modul1.Instance.sDatu + _Modul1.Instance.Kont[9] + _Modul1.Instance.Kont[8] + _Modul1.Instance.Kont[2] + _Modul1.Instance.Kont[1];
                        _0024STATIC_0024Berufe_00242001_0024Job1 = Strings.Trim(_0024STATIC_0024Berufe_00242001_0024Job1.Replace("  ", " ")) + ".";
                    }
                    else
                    {
                        List1.Items.Add(_Modul1.Instance.sDatu + _Modul1.Instance.Kont[8] + _Modul1.Instance.Kont[9] + _Modul1.Instance.Kont[2] + _Modul1.Instance.Kont[1] + ".");
                    }
                }
                DataModul.DB_EventTable.MoveNext();
                _0024STATIC_0024Berufe_00242001_0024I1 = (byte)unchecked((uint)(_0024STATIC_0024Berufe_00242001_0024I1 + 1));
                if (unchecked(_0024STATIC_0024Berufe_00242001_0024I1) > 70u)
                {
                    break;
                }
            }
            switch (Modul1_Beruf)
            {
                case EEventArt.eA_300:
                    if (List1.Items.Count == 0)
                    {
                        return;
                    }
                    if (List1.Items.Count == 1)
                    {
                        RtB[0].SelectedText = " Beruf: ";
                    }
                    if (List1.Items.Count > 1)
                    {
                        RtB[0].SelectedText = " Berufe: ";
                    }
                    break;
                case EEventArt.eA_301:
                    if (List1.Items.Count == 0)
                    {
                        return;
                    }
                    if (List1.Items.Count == 1)
                    {
                        RtB[0].SelectedText = " " + _Modul1.Instance.IText[70] + " ";
                    }
                    if (List1.Items.Count > 1)
                    {
                        RtB[0].SelectedText = " " + _Modul1.Instance.IText[70] + " ";
                    }
                    break;
                case EEventArt.eA_302:
                    if (List1.Items.Count == 0)
                    {
                        return;
                    }
                    if (List1.Items.Count == 1)
                    {
                        RtB[0].SelectedText = " " + _Modul1.Instance.IText[8] + " ";
                    }
                    if (List1.Items.Count > 1)
                    {
                        RtB[0].SelectedText = " " + _Modul1.Instance.IText[8] + " ";
                    }
                    break;
            }
            RtB[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
            _0024STATIC_0024Berufe_00242001_0024Job = "";
            byte b = (byte)(List1.Items.Count - 1);
            _0024STATIC_0024Berufe_00242001_0024I1 = 0;
            while (unchecked(_0024STATIC_0024Berufe_00242001_0024I1 <= (uint)b))
            {
                if ((Operators.CompareString(List1.Items[_0024STATIC_0024Berufe_00242001_0024I1].AsString(), List1.Items[unchecked(_0024STATIC_0024Berufe_00242001_0024I1) + 1].AsString(), TextCompare: false) != 0) & (List1.Items[_0024STATIC_0024Berufe_00242001_0024I1].AsString() != _0024STATIC_0024Berufe_00242001_0024Job1))
                {
                    _0024STATIC_0024Berufe_00242001_0024Job += List1.Items[_0024STATIC_0024Berufe_00242001_0024I1].AsString().Replace("  ", " ");
                }
                _0024STATIC_0024Berufe_00242001_0024I1 = (byte)unchecked((uint)(_0024STATIC_0024Berufe_00242001_0024I1 + 1));
            }
            _0024STATIC_0024Berufe_00242001_0024Job = _0024STATIC_0024Berufe_00242001_0024Job1.Trim() + " " + _0024STATIC_0024Berufe_00242001_0024Job;
            if (_0024STATIC_0024Berufe_00242001_0024Job.Trim() != "")
            {
                RtB[0].SelectedText = _0024STATIC_0024Berufe_00242001_0024Job.Trim();
            }
        }
    }

    private void _Command_1_Click(object sender, EventArgs e)
    {
    }
}
