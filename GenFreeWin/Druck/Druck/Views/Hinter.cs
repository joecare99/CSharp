using BaseLib.Helper;
using Druck.My;
using GenFree;
using GenFree.Helper;
using GenFree.Interfaces.Sys;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.Compatibility.VB6;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Druck.Views;

[DesignerGenerated]
public partial class Hinter : Form
{

    [DebuggerNonUserCode]
    public Hinter()
    {
        base.Load += Hinter_Load;
        this.Command1 = new ControlArray<System.Windows.Forms.Button>();
        this.Label1 = new ControlArray<System.Windows.Forms.Label>();
        this.Label2 = new ControlArray<System.Windows.Forms.Label>();
        this.Text1 = new Microsoft.VisualBasic.Compatibility.VB6.TextBoxArray(this.components);
        InitializeComponent();
        Command1.AddClick(Command1_Click);
        this.Text1.SetIndex(this._Text1_14, 14);
        this.Text1.SetIndex(this._Text1_13, 13);
        this.Text1.SetIndex(this._Text1_12, 12);
        this.Text1.SetIndex(this._Text1_0, 0);
        this.Text1.SetIndex(this._Text1_1, 1);
        this.Text1.SetIndex(this._Text1_2, 2);
        this.Text1.SetIndex(this._Text1_3, 3);
        this.Text1.SetIndex(this._Text1_4, 4);
        this.Text1.SetIndex(this._Text1_5, 5);
        this.Text1.SetIndex(this._Text1_6, 6);
        this.Text1.SetIndex(this._Text1_7, 7);
        this.Text1.SetIndex(this._Text1_8, 8);
        this.Text1.SetIndex(this._Text1_9, 9);
        this.Text1.SetIndex(this._Text1_10, 10);
        this.Text1.SetIndex(this._Text1_11, 11);
        this.Command1.SetIndex(this._Command1_1, 1);
        this.Command1.SetIndex(this._Command1_0, 0);
        this.Label2.SetIndex(this._Label2_14, 14);
        this.Label2.SetIndex(this._Label2_13, 13);
        this.Label2.SetIndex(this._Label2_12, 12);
        this.Label2.SetIndex(this._Label2_0, 0);
        this.Label2.SetIndex(this._Label2_1, 1);
        this.Label2.SetIndex(this._Label2_2, 2);
        this.Label2.SetIndex(this._Label2_3, 3);
        this.Label2.SetIndex(this._Label2_4, 4);
        this.Label2.SetIndex(this._Label2_5, 5);
        this.Label2.SetIndex(this._Label2_6, 6);
        this.Label2.SetIndex(this._Label2_7, 7);
        this.Label2.SetIndex(this._Label2_8, 8);
        this.Label2.SetIndex(this._Label2_10, 10);
        this.Label2.SetIndex(this._Label2_9, 9);
        this.Label2.SetIndex(this._Label2_11, 11);
        this.Label1.SetIndex(this._Label1_2, 2);
        this.Label1.SetIndex(this._Label1_1, 1);
        this.Label1.SetIndex(this._Label1_0, 0);
    }


    private void Command1_Click(object eventSender, EventArgs eventArgs)
    {
        int index = Command1.GetIndex((Button)eventSender);
        Visible = false;
        checked
        {
            switch (index)
            {
                case 0:
                    {
                        FileSystem.FileClose(99);
                        FileSystem.FileOpen(99, _Modul1.Instance.GenFreeDir + "init\\DruckTexte.dat", OpenMode.Output);
                        byte b = 1;
                        do
                        {
                            _Modul1.Instance.DTxt[b] = Text1[(short)(unchecked(b) - 1)].Text;
                            FileSystem.PrintLine(99, _Modul1.Instance.DTxt[b]);
                            b = (byte)unchecked((uint)(b + 1));
                        }
                        while (unchecked(b) <= 15u);
                        FileSystem.PrintLine(99, TextBox4.Text);
                        FileSystem.PrintLine(99, TextBox5.Text);
                        FileSystem.FileClose(99);
                        FileSystem.FileOpen(99, _Modul1.Instance.GenFreeDir + "init\\DruckTexte.dat", OpenMode.Input);
                        b = 0;
                        while (!FileSystem.EOF(99))
                        {
                            b = (byte)(unchecked(b) + 1);
                            _Modul1.Instance.DTxt[b] = FileSystem.LineInput(99);
                        }
                        Module2.Drucktexte();
                        Frame1.Visible = false;
                        FileSystem.FileClose(99);
                        MyProject.Forms.Druck.Visible = true;
                        break;
                    }
                case 1:
                    Frame1.Visible = false;
                    MyProject.Forms.Druck.Visible = true;
                    break;
            }
        }
    }

    private void Hinter_Load(object eventSender, EventArgs eventArgs)
    {
        object[] array = new object[11];
        Label1[0].Text = _Modul1.Instance.VersionT;
        Label1[1].Text = "(c) 1994-2018 Gisbert Berwe 49082 Osnabrück Friedrich-Holthaus-Str. 18 Tel.: 0541-80 00 79 00";
        Label1[2].Text = "Version 24.09.04 Stand 25.11.2018";
        WindowState = _Modul1.Instance.Persistence.ReadEnumInit<FormWindowState>("Windowstate");
        var Cols = _Modul1.Instance.Persistence.ReadFarbenInit("Farb.dat", 2);
        _Modul1.Instance.HintFarb = ColorTranslator.FromOle(0x808080);
        _Modul1.Instance.Feld1Farb = ColorTranslator.FromOle(0xFFFFFF);
        _Modul1.Instance.HintFarb = Cols[1];
        _Modul1.Instance.Feld1Farb = Cols[2];
        BackColor = _Modul1.Instance.HintFarb;
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
        Font = new Font("Arial", _Modul1.Instance.Fs, FontStyle.Regular);
        FileSystem.FileOpen(99, _Modul1.Instance.GenFreeDir + "init\\Textegerm.dat", OpenMode.Input);
        checked
        {
            var num = 0;
            while (!FileSystem.EOF(99))
            {
                num = (short)(num + 1);
                _Modul1.Instance.IText[num] = FileSystem.LineInput(99);
            }
            FileSystem.FileClose(99);
            num = 0;
            FileSystem.FileOpen(99, _Modul1.Instance.GenFreeDir + "init\\DruckTexte.dat", OpenMode.Input);
            while (!FileSystem.EOF(99))
            {
                num = (short)(num + 1);
                _Modul1.Instance.DTxt[num] = FileSystem.LineInput(99);
            }
            Module2.Drucktexte();
            MyProject.Forms.Druck.Show();
        }
    }

    public void Att(string Fad)
    {
        //Discarded unreachable code: IL_00dd
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        object CounterResult = default;
        object LoopForResult = default;
        string pathName = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                int num4;
                switch (try0000_dispatch)
                {
                    default:
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        goto IL_0008;
                    case 295:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_00e1;
                                default:
                                    goto end_IL_0000;
                            }
                            goto IL_00ac;
                        }
                    IL_00ac:
                        num = 11;
                        if (Information.Err().Number != 53)
                        {
                            goto end_IL_0000_2;
                        }
                        goto IL_00be;
                    IL_00be:
                        num = 12;
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        goto IL_00e1;
                    IL_0097:
                        num = 9;
                        if (!ObjectFlowControl.ForLoopControl.ForNextCheckObj(CounterResult, LoopForResult, ref CounterResult))
                        {
                            goto end_IL_0000_2;
                        }
                        goto IL_003a;
                    IL_00e1:
                        num4 = num2 + 1;
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 2:
                                goto IL_0008;
                            case 3:
                                goto IL_0018;
                            case 4:
                                goto IL_003a;
                            case 5:
                                goto IL_0064;
                            case 7:
                            case 8:
                                goto IL_008d;
                            case 9:
                                goto IL_0097;
                            case 11:
                                goto IL_00ac;
                            case 12:
                                goto IL_00be;
                            default:
                                goto end_IL_0000;
                            case 6:
                            case 10:
                            case 13:
                                goto end_IL_0000_2;
                        }
                        goto default;
                    IL_0008:
                        num = 2;
                        // Todo: File1.Path = Fad;
                        goto IL_0018;
                    IL_0018:
                        num = 3;
                        if (!ObjectFlowControl.ForLoopControl.ForLoopInitObj(CounterResult, 0, 100, 1, ref LoopForResult, ref CounterResult))
                        {
                            goto end_IL_0000_2;
                        }
                        goto IL_003a;
                    IL_003a:
                        num = 4;
                        // Todo: pathName = File1.Path + "\\" + File1.Items.Count;
                        goto IL_0064;
                    IL_0064:
                        num = 5;
                        if (Operators.CompareString(Strings.Trim(File1.Items.Count.ToString()), "", TextCompare: false) == 0)
                        {
                            goto end_IL_0000_2;
                        }
                        goto IL_008d;
                    IL_008d:
                        num = 8;
                        FileSystem.SetAttr(pathName, FileAttribute.Normal);
                        goto IL_0097;
                    end_IL_0000:
                        break;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 295;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0000_2:
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    private void _Command1_0_Click(object sender, EventArgs e)
    {
    }
}
