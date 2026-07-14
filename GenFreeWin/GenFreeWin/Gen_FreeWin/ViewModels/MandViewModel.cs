using BaseLib.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using GenFreeWin.Views;
using GenFree;
using GenFree.Data;
using GenFree.Interfaces.Sys;
using GenFree.Interfaces.VB;
using GenFree.ViewModels.Interfaces;
using GenFreeWin.Views;
using Microsoft.VisualBasic;
using MVVM.ViewModel;
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace GenFreeWin.ViewModels;

public partial class MandViewModel : BaseViewModelCT, IMandViewModel
{
    private string Mandantname;
    IModul1 Modul1 => _Modul1.Instance;
    [Obsolete]
    IProjectData ProjectData => Modul1.ProjectData;
    IInteraction Interaction;

    [Obsolete]
    IVBInformation Information => Modul1.Information;
    [Obsolete]
    IVBConversions Conversion => Modul1.Conversions;
    [Obsolete]
    IStrings Strings => Modul1.Strings;
    [DebuggerNonUserCode]

    IContainerControl IMandViewModel.View { get; set; }

    Mand View => (Mand)((IMandViewModel)this).View;

    [ObservableProperty]
    public partial bool Frame1_Visible { get; set; }

    IList List1_Items => View.List1.Items;

    public void Form_Load(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_02ef, IL_030d, IL_0470
        int try0001_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        num = 1;
        FileSystem.FileClose(99);
        goto IL_Start;
        num = 43;
        if (Information.Err().Number == 3044)
        {
            goto end_IL_0001_3;
        }
        if (Information.Err().Number == 3024)
        {
            Modul1.Verz = View.Laufwerk1.Text.Left(1) + ":" + Modul1.GenFreeDir;
            ProjectData.ClearProjectError();
            if (num2 == 0)
            {
                throw ProjectData.CreateProjectError(-2146828268);
            }
            return;
        }
        if (Information.Err().Number == 3043)
        {
            {
                num = 53;
                string nDrive = Modul1.Verz1.Left(1) + ":\\";
                var Modul1_Typ = new DriveInfo(nDrive).DriveType;
                if (Modul1_Typ == DriveType.CDRom)
                {
                    _ = Interaction.MsgBox("F114");
                }
            }
            _ = Interaction.MsgBox("F115");
        }
        if (Interaction.MsgBox(Conversion.ErrorToString(), title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OKCancel) == DialogResult.Cancel)
        {
            ProjectData.EndApp();
        }
        Debugger.Break();
        ProjectData.ClearProjectError();
        if (num2 == 0)
        {
            throw ProjectData.CreateProjectError(-2146828268);
        }
    IL_Start:
        num = 2;
        View.WindowState = Menue.Default.WindowState;
        View.BackColor = Modul1.HintFarb;
        if (Modul1.FontSize > 0f)
        {
            View.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.Label15.Font = new Font("Arial", Modul1.FontSize, FontStyle.Bold);
            View.Label16.Font = new Font("Arial", Modul1.FontSize, FontStyle.Bold);
            View.Label17.Font = new Font("Arial", Modul1.FontSize, FontStyle.Bold);
        }
        //Todo: in Modul1 verschieben
        var ai = Modul1.Persistence.ReadIntsMand("maspos.dat", 2);
        View.Left = ai[0];
        View.Top = ai[1];

        View.Text = Modul1.AppName;
        View.Label15.Width = View.Width;
        View.Label16.Width = View.Width;
        View.Label17.Width = View.Width;
        View.Label15.Text = Modul1.VersionT;
        View.Label16.Text = Modul1.Version1;
        View.Label17.Text = Modul1.Version;
        FileSystem.FileClose(6);
        if (!Directory.Exists(Modul1.Verz))
        {
            Mandantname = Modul1.GenFreeDir + "\\Testdat1\\Gen_plusdaten.mdb";
        }
        else
        {
            // Todo: allgemeiner machen
            Mandantname = Modul1.Verz + "Gen_plusdaten.mdb";
        }

        if (Modul1.Typ == DriveType.CDRom)
        {
            DataModul.MandDB = DataModul.OpenDatabase(Mandantname.ToUpper(), true, true, "");
        }
        else
        {
            DataModul.MandDB?.Close();
            DataModul.MandDB = DataModul.OpenDatabase(Mandantname, false, false, "");
        }
        manda();
        return;

    end_IL_0001_2:
        return;
    end_IL_0001_3:
        return;
    }

    private void manda()
    {
        //Discarded unreachable code: IL_03d2, IL_03f3
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
                            goto IL_Start;
                        case 1249:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_03f7;
                                    default:
                                        goto end_IL_0001;
                                }
                                goto IL_03d4;
                            }
                        IL_03d4:
                            num = 53;
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_03f7;
                        IL_03f7:
                            num4 = unchecked(num2 + 1);
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 2:
                                    goto IL_Start;
                                case 53:
                                    goto IL_03d4;
                                default:
                                    goto end_IL_0001;
                                case 52:
                                case 54:
                                    goto end_IL_0001_2;
                            }
                            goto default;
                        IL_Start:
                            num = 2;
                            // Todo: move to Modul1
                            if (Modul1.Verz.Length < 14)
                            {
                                Modul1.Verz = "";
                            }
                            string drive;
                            if (Modul1.Verz == "")
                            {
                                Modul1.Verz = Modul1.Persistence.ReadStringInit("gen-verz.ini");
                                Modul1.Verz1 = Modul1.Verz.Left(15);
                                if (Modul1.Verz.Trim() == "")
                                {
                                    Modul1.Verz = FileSystem.CurDir().Left(3) + "Gen_Pluswin\\";
                                    if (Modul1.System.VerSpecial == 1)
                                    {
                                        Modul1.Verz = FileSystem.CurDir().Left(3) + "Gen_Pluswin_Sond\\";
                                    }
                                }
                                if (Modul1.System.VerSpecial == 1)
                                {
                                    Modul1.Verz1 = Modul1.Verz.Left(20);
                                }
                                drive = Modul1.Verz.Left(2);
                            }
                            Modul1.Verz1 = Modul1.Verz.Left(15);
                            if (Modul1.System.VerSpecial == 1)
                            {
                                Modul1.Verz1 = Modul1.Verz.Left(20);
                            }
                            View.BezMAND.Text = Modul1.Verz;
                            FileSystem.FileClose(); //??
                            List1_Items.Clear();
                            drive = Modul1.Verz.Left(2);
                            View.Laufwerk1.Text = drive;

                            foreach (var mands in Modul1.EnumerateMandants(Path.GetPathRoot(Modul1.Verz1)))
                            {
                                List1_Items.Add(mands);
                            }

                            if (Modul1.System.xDemo)
                            {
                                if (List1_Items.Count > 1)
                                {
                                    _ = Interaction.MsgBox(Modul1.Message_sDemoVerNotPossibl);
                                    View.cmdNewMandant.Visible = false;
                                }
                            }
                            View.Label2.Text = List1_Items[0].ToString().Left(Modul1.Verz1.Length);
                            goto end_IL_0001_2;

                        end_IL_0001:
                            break;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0001_dispatch = 1249;
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

    public void List1_Click(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_03aa, IL_058f
        int try0001_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int number = default;
        int lErl = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;

                int num4;
                string nDrive;
                switch (try0001_dispatch)
                {
                    default:
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        goto IL_Start;
                    case 1745:
                        {
                            num2 = num;
                            switch (num3 <= -2 ? 1 : num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_0593;
                                default:
                                    goto end_IL_0001;
                            }
                            goto IL_03ac;
                        }
                    IL_0593:
                        num4 = num2 + 1;
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 2:
                                goto IL_Start;
                            case 37:
                            case 64:
                                goto IL_039c;
                            case 39:
                                goto IL_03ac;
                            case 51:
                                goto IL_0430;
                            case 57:
                                goto IL_0489;
                            case 61:
                                goto IL_04c6;
                            case 67:
                                goto IL_0536;
                            case 70:
                            case 71:
                                goto IL_0553;
                            case 72:
                                goto end_IL_0001_3;
                            default:
                                goto end_IL_0001;
                            case 38:
                            case 40:
                            case 44:
                            case 45:
                            case 48:
                            case 49:
                            case 50:
                            case 55:
                            case 56:
                            case 59:
                            case 60:
                            case 65:
                            case 68:
                            case 69:
                            case 73:
                            case 74:
                            case 75:
                                goto end_IL_0001_2;
                        }
                        goto default;
                    IL_Start:
                        num = 2;
                        View.cmdDeleteMandant.Enabled = false;
                        if ($"{Modul1.Verz1}{View.List1.SelectedItem}\\".ToUpper() != View.BezMAND.Text.ToUpper())
                        {
                            View.cmdDeleteMandant.Enabled = true;
                        }
                        View.Bez1.Text = "0";
                        View.Bez2.Text = "0";
                        nDrive = Modul1.Verz1.Left(1) + ":\\";
                        var Modul1_Typ = new DriveInfo(Modul1.Verz1.Left(1)).DriveType;
                        if (Modul1_Typ == DriveType.Network)//cdrom
                        {
                            if (!Module2.IsDriveReady(Modul1.Verz1.Left(1) + ":", (Modul1.Verz1 + View.List1.SelectedItem + "\\Gen_plusdaten.mdb").AsString(), bCheckWriteAccess: true))
                            {
                                Modul1_Typ = DriveType.CDRom;
                            }
                        }
                        // todo: allgemeiner machen
                        Mandantname = Path.Combine(Modul1.Verz1, View.List1.SelectedItem.AsString(), "Gen_plusdaten.mdb");
                        if (Modul1.Verz1.Right(1) != "\\")
                        {
                            Modul1.Verz1 += "\\";
                        }
                        Application.DoEvents();
                        //=============================
                        var (PCnt, FCnt) = DataModul.PeekMandant(Mandantname, Modul1.Typ == DriveType.CDRom, File.SetAttributes);
                        View.Bez1.Text = PCnt.AsString();
                        View.Bez2.Text = FCnt.AsString();
                        goto IL_039c;

                    IL_039c:
                        num = 37;
                        lErl = 12;
                        goto end_IL_0001_2;

                    IL_03ac:
                        num = 39;
                        number = Information.Err().Number;
                        if (number == 5)
                        {
                            _ = Interaction.MsgBox(Conversion.ErrorToString(), title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OK);
                            goto end_IL_0001_2;
                        }
                        if (number == 3021)
                        {
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_0593;
                        }
                        else
                        {
                            goto IL_0430;
                        }
                    IL_0430:
                        num = 51;
                        if (number == 3024)
                        {
                            _ = Interaction.MsgBox($"Keine {Modul1.AppName}-Datei");
                            View.Bez1.Text = "-1";
                            View.Bez2.Text = "-1";
                            goto end_IL_0001_2;
                        }
                        goto IL_0489;
                    IL_0489:
                        num = 57;
                        if (number == 3343)
                        {
                            _ = Interaction.MsgBox(Conversion.ErrorToString(), title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OK);
                            goto end_IL_0001_2;
                        }
                        goto IL_04c6;
                    IL_04c6:
                        num = 61;
                        if (number is 3044 or 3356)
                        {
                            _ = Interaction.MsgBox(Conversion.ErrorToString(), title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OK);
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            num2 = 0;
                            goto IL_039c;
                        }
                        if (number == 53)
                        {
                            goto IL_0536;
                        }
                        else
                        {
                            goto IL_0553;
                        }
                    IL_0536:
                        num = 67;
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        goto IL_0593;

                    IL_0553:
                        num = 71;
                        _ = Interaction.MsgBox(Conversion.ErrorToString(), title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OK);
                        break;
                    end_IL_0001_3:
                        break;
                }
                num = 72;
                _ = Interaction.MsgBox("F117");
                break;
            end_IL_0001:
                ;
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 1745;
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

    public void List1_DoubleClick(object eventSender, EventArgs eventArgs)
    {
        DataModul.CitationData.Clear();
        View.cmdDeleteMandant.Enabled = false;
        View.BezMAND.Text = Path.Combine(Modul1.Verz1, View.List1.SelectedItem.AsString(), "Gen_plusdaten.mdb");

        //        View.BezMAND.Text = lblState.Text.ToUpper() + List1.Text.ToUpper() + "\\";
        Modul1.Verz = Path.GetDirectoryName(View.BezMAND.Text);
        View.Befehl2.PerformClick();
    }

    public void Command1_Click(object eventSender, EventArgs eventArgs)
    {
        //Discarded unreachable code: IL_04a4, IL_0537
        int try0001_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int number = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;

                checked
                {
                    var M1_Iter = 0;
                    int num4;
                    Type typeFromHandle;
                    object[] array;
                    ListBox list;
                    object[] array2;
                    bool[] array3;
                    object obj;
                    string DateiName;
                    switch (try0001_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_Start;
                        case 1669:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 2:
                                    case 3:
                                        break;
                                    case 1:
                                        goto IL_053b;
                                    default:
                                        goto end_IL_0001;
                                }
                                goto IL_04a6;
                            }
                        IL_04e7:
                            num = 73;
                            if (number != 53)
                            {
                                break;
                            }
                            goto IL_04f8;
                        IL_04f8:
                            num = 74;
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_053b;
                        IL_053b:
                            num4 = unchecked(num2 + 1);
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 2:
                                    goto IL_Start;
                                case 10:
                                case 15:
                                case 16:
                                    goto IL_0130;
                                case 70:
                                    goto IL_04cb;
                                case 73:
                                    goto IL_04e7;
                                case 74:
                                    goto IL_04f8;
                                case 67:
                                case 71:
                                case 72:
                                case 75:
                                case 76:
                                case 77:
                                    goto end_IL_0001_3;
                                default:
                                    goto end_IL_0001;
                                case 7:
                                case 14:
                                case 19:
                                case 65:
                                case 78:
                                    goto end_IL_0001_2;
                            }
                            goto default;
                        IL_04cb:
                            num = 70;
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_053b;
                        IL_Start:
                            num = 2;
                            DataModul.MandDB.Close();
                            DataModul.TempDB.Close();
                            DataModul.DOSB.Close();
                            DataModul.DSB.Close();
                            if (View.edtNewMandant.Text == "" & Modul1.Verz != "")
                            {
                                //exit
                                goto end_IL_0001_2;
                            }
                            string text = View.edtNewMandant.Text.ToUpper();
                            switch (text)
                            {
                                case "INIT":
                                case "TEMP":
                                case "LIST":
                                case "TEMPOSB":
                                case "HILFE":
                                case "INTERAHN":
                                case "TESTDAT1":
                                    break;
                                default:
                                    goto IL_0130;
                            }
                            _ = Interaction.MsgBox("Der Mandantenname ist unzulässig");
                            goto end_IL_0001_2;

                        IL_0130:
                            num = 16;
                            int count = List1_Items.Count;
                            M1_Iter = 0;
                            while (M1_Iter <= count)
                            {
                                typeFromHandle = typeof(Strings);
                                array = new object[1];
                                list = View.List1;
                                array[0] = list.SelectedItem;
                                array2 = array;
                                array3 = new bool[1] { true };
                                obj = list.SelectedItem.AsString().ToUpper();
                                if (array3[0])
                                {
                                    list.SelectedItem = array2[0];
                                }
                                if (obj.AsString().TrimEnd() == View.edtNewMandant.Text.ToUpper().Trim())
                                {
                                    _ = Interaction.MsgBox("Genealogie existiert schon");
                                    //return
                                    goto end_IL_0001_2;
                                }
                                M1_Iter++;

                            }
                            ProjectData.ClearProjectError();
                            num3 = 3;
                            Modul1.Verz1 = Modul1.Verz.Left(15);
                            FileSystem.MkDir(Modul1.Verz1 + View.edtNewMandant.Text.Trim());
                            Modul1.Verz = Modul1.Verz1 + View.edtNewMandant.Text.Trim() + "\\";
                            DateiName = Modul1.GenFreeDir + "\\Gen_plusdaten3.mdb";
                            string source;
                            if (File.Exists(DateiName))
                            {
                                source = Modul1.GenFreeDir + "\\Gen_plusdaten3.mdb";
                                //=================
                            }
                            else
                            {
                                _ = Interaction.MsgBox("UTF-8 nicht möglich");
                                source = Modul1.InitDir + "Gen_plusdaten1.mdb";
                            }
                            string destination = Modul1.Verz + "Gen_plusdaten.mdb";
                            FileSystem.FileCopy(source, destination);

                            FileSystem.Kill(Modul1.Verz + "Such.mdb");
                            FileSystem.Kill(Modul1.Verz + "Ort1.mdb");
                            FileSystem.Kill(Modul1.Verz + "Tempo.mdb");

                            source = File.Exists(DateiName = Modul1.GenFreeDir + "\\Such2.mdb") ? DateiName : Modul1.GenFreeDir + "\\Such.mdb";
                            destination = Modul1.Verz + "Such.mdb";
                            FileSystem.FileCopy(source, destination);

                            source = File.Exists(DateiName = Modul1.GenFreeDir + "\\Ort2.mdb") ? DateiName : Modul1.GenFreeDir + "\\Ort1.mdb";
                            destination = Modul1.Verz + "Ort1.mdb";
                            FileSystem.FileCopy(source, destination);

                            source = File.Exists(DateiName = Modul1.GenFreeDir + "\\Tempo2.mdb") ? DateiName : Modul1.GenFreeDir + "\\Tempo.mdb";
                            destination = Modul1.Verz + "Tempo.mdb";
                            FileSystem.FileCopy(source, destination);

                            FileSystem.FileClose();
                            View.Close();
                            View.Befehl2.PerformClick();
                            goto end_IL_0001_2;
                            //=================
                        IL_04a6:
                            num = 66;
                            number = Information.Err().Number;
                            if (number == 3420)
                            {
                                goto IL_04cb;
                            }
                            else
                            {
                                goto IL_04e7;
                            }

                            //=================
                        end_IL_0001_3:
                            break;
                    }
                    num = 77;
                    _ = Interaction.MsgBox(Conversion.ErrorToString(), title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OK);
                    break;
                }
            end_IL_0001:
                ;
            }
            catch (Exception obj2) when (obj2 is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj2);
                try0001_dispatch = 1669;
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

    private void _Command1_1_Click(object sender, EventArgs e)
    {
        Frame1_Visible = false;
    }

    public void CmdNewMandant_Click(object sender, EventArgs e)
    {
        // ??  
        View.Label9.Text = View.List1.Text.Left(Modul1.Verz1.Length);
        Frame1_Visible = true;
        _ = View.edtNewMandant.Focus();
    }

    public void Befehl2_Click(object sender, EventArgs e)
    {

        //Modul1.Verz =
        //View.BezMAND.Text = Path.Combine(View.Laufwerk1.SelectedItem.ToString().Left(2) + $"\\{Modul1.AppName}\\", View.List1.SelectedItem.AsString());

        if (Modul1.Verz.Length <= 15)
        {
            Modul1.Verz = Modul1.Verz1;
        }
        View.BezMAND.Text = Modul1.Verz;

        FileSystem.FileClose(99);
        string nDrive = FileSystem.CurDir().Left(1) + ":\\";
        if (new DriveInfo(nDrive).DriveType != DriveType.CDRom)
        {
            Modul1.Persistence.WriteStringInit("gen-verz.ini", Modul1.Verz);
            Modul1.Verz1 = Modul1.Verz.Left(15);
        }
        nDrive = Modul1.Verz.Left(1) + ":\\";
        var Modul1_Typ = new DriveInfo(nDrive).DriveType;
        if (Modul1_Typ == DriveType.Network && !Module2.IsDriveReady(Modul1.Verz1.Left(1) + ":", (Modul1.Verz1 + View.List1.SelectedItem + ("\\Gen_plusdaten.mdb")).AsString(), bCheckWriteAccess: true))
        {
            Modul1_Typ = DriveType.CDRom;
        }
        FileSystem.FileClose();
        string text = "";

        if (File.Exists(nDrive = Modul1.Verz + "Wappen.bmp")
            || File.Exists(nDrive = Modul1.Verz + "Wappen.TIF")
            || File.Exists(nDrive = Modul1.Verz + "Wappen.GIF")
            || File.Exists(nDrive = Modul1.Verz + "Wappen.JPG")
            || File.Exists(nDrive = Modul1.Verz1 + "Wappen.bmp")
            || File.Exists(nDrive = Modul1.Verz1 + "Wappen.TIF")
            || File.Exists(nDrive = Modul1.Verz1 + "Wappen.GIF")
            || File.Exists(nDrive = Modul1.Verz1 + "Wappen.JPG"))
            text = nDrive;

        if (text.Trim() != "")
        {
            Bitmap oBitmap;
            if (Modul1.Typ != DriveType.CDRom)
            {
                FileStream fileStream = new FileStream(text, FileMode.Open);
                oBitmap = new Bitmap(fileStream);
                fileStream.Close();
            }
            else
            {
                oBitmap = new Bitmap(text);
            }
            PictureBox pictureBox = Menue.Default.pbxCodeOfArms;
            pictureBox.Image = Modul1.AutoSizeImage(oBitmap, pictureBox.ClientRectangle.Width, pictureBox.ClientRectangle.Height);
        }
        FileSystem.FileClose(99);
        View.Close();
    }

    public void CmdDeleteMandant_Click(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_05d2
        int try0001_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        int lErl = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;

                checked
                {
                    int num6;
                    switch (try0001_dispatch)
                    {
                        default:
                            num = 1;
                            goto IL_Start;
                        case 2214:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 2:
                                    case 3:
                                        if (Information.Err().Number == 53)
                                        {
                                            ProjectData.ClearProjectError();
                                            if (num2 == 0)
                                            {
                                                throw ProjectData.CreateProjectError(-2146828268);
                                            }
                                            num6 = unchecked(num2 + 1);
                                        }
                                        else if (Information.Err().Number == 55)
                                        {
                                            _ = Interaction.MsgBox(Conversion.ErrorToString() + "\n\nVersuchen Sie es nach dem nächsten Programmstart erneut.");
                                            goto end_IL_0001_2;
                                        }
                                        else if (Information.Err().Number == 75)
                                        {
                                            _ = Interaction.MsgBox(Conversion.ErrorToString() + $"\n\nIn diesem Mandanten befinden sich andere Daten (nicht {Modul1.AppName}).\n Löschen aus Sicherheitsgründen nicht möglich!");
                                            goto end_IL_0001_2;
                                        }
                                        else if (Information.Err().Number == 3420)
                                        {
                                            ProjectData.ClearProjectError();
                                            if (num2 == 0)
                                            {
                                                throw ProjectData.CreateProjectError(-2146828268);
                                            }
                                            num6 = unchecked(num2 + 1);
                                        }
                                        else
                                        {
                                            if (Interaction.MsgBox(Conversion.ErrorToString(), title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                                            {
                                                ProjectData.EndApp();
                                            }
                                            Debugger.Break();
                                            ProjectData.ClearProjectError();
                                            if (num2 == 0)
                                            {
                                                throw ProjectData.CreateProjectError(-2146828268);
                                            }
                                            num6 = num2;
                                        }
                                        break;
                                    case 1:
                                        num6 = unchecked(num2 + 1);
                                        break;
                                    default:
                                        goto end_IL_0001;
                                }
                            }
                            goto IL_0710;
                        IL_0710:
                            num2 = 0;
                            switch (num6)
                            {
                                case 1:
                                    break;
                                case 2:
                                    goto IL_Start;

                                default:
                                    goto end_IL_0001;
                                case 3:
                                case 18:
                                case 22:
                                case 26:
                                case 30:
                                case 76:
                                case 83:
                                case 87:
                                case 98:
                                    goto end_IL_0001_2;
                            }
                            goto default;

                        IL_Start:
                            num = 2;
                            if (Modul1.Typ == DriveType.CDRom)
                            {
                                _ = Interaction.MsgBox(Modul1.Message_sNoChangesOnCD, title: "", icon: MessageBoxIcon.Information);
                                goto end_IL_0001_2;
                            }
                            View.ListBox1.Items.Clear();

                            FileSystem.FileClose(99);
                            Modul1.Verz = View.BezMAND.Text;
                            Modul1.Persistence.WriteStringInit(Modul1.Verz, "gen-verz.ini");
                            string text2 = ((Modul1.Verz1) + (List1_Items[View.List1.SelectedIndex])).AsString();
                            FileSystem.FileClose(99);
                            Modul1.Verz = Modul1.Persistence.ReadStringInit("gen-verz.ini");

                            if (text2.ToUpper() + "\\" == Modul1.Verz.ToUpper() | text2.ToUpper() + "\\" == View.BezMAND.Text.ToUpper())
                            {
                                _ = Interaction.MsgBox("Sie können nicht den aktuellen Mandanten löschen");
                                goto end_IL_0001_2;
                            }

                            short num7 = (short)Interaction.MsgBox($"Mandanten {text2} wirklich löschen?", title: "", mb: MessageBoxButtons.YesNo);
                            if (num7 > 6)
                            {
                                goto end_IL_0001_2;
                            }
                            num7 = (short)Interaction.MsgBox($"Alle Daten des Mandanten {text2} gehen unwiderruflich verloren", title: "Warnung", mb: MessageBoxButtons.OKCancel);
                            if (num7 > 1)
                            {
                                goto end_IL_0001_2;
                            }
                            num7 = (short)Interaction.MsgBox($"Mandanten {text2} wirklich löschen?", title: "Letzte Warnung", mb: MessageBoxButtons.YesNo);
                            if (num7 > 6)
                            {
                                goto end_IL_0001_2;
                            }
                            //====================================================================================================
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            DataModul.MandDB.Close();
                            DataModul.TempDB.Close();
                            DataModul.DOSB.Close();
                            DataModul.DSB.Close();
                            View.ListBox1.Items.Clear();
                            string[] array = Modul1.F_GetAllFiles(text2, 2);
                            int num8 = array.Length - 1;
                            int num4 = 0;
                            while (num4 <= num8)
                            {
                                _ = View.ListBox1.Items.Add(array[num4]);
                                num4++;
                            }
                            //====================================================================================================
                            ProjectData.ClearProjectError();

                            num3 = 3;
                            int num11 = View.ListBox1.Items.Count - 1;
                            int num5 = 0;
                            while (num5 <= num11)
                            {
                                FileSystem.Kill(((View.ListBox1.Items[num5]) + ("\\*.*")).AsString());
                                FileSystem.RmDir(View.ListBox1.Items[num5].AsString());
                                num5++;
                            }
                            FileSystem.Kill(text2 + "\\*.*");
                            _ = Modul1.RemoveWriteProtection(text2);
                            FileSystem.RmDir(text2);
                            lErl = 2;
                            Modul1.Verz1 = Modul1.Verz.Left(15);
                            View.Bez1.Text = "";
                            View.Bez2.Text = "";
                            if (Modul1.System.VerSpecial == 1)
                            {
                                Modul1.Verz1 = Modul1.Verz.Left(20);
                            }

                            View.BezMAND.Text = Modul1.Verz;
                            List1_Items.Clear();
                            foreach (var item in Modul1.EnumerateMandants(Modul1.Verz1))
                            {
                                List1_Items.Add(item);
                            }
                            View.cmdDeleteMandant.Enabled = false;
                            goto end_IL_0001_2;
                        end_IL_0001:
                            break;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 2214;
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

    public void Text1_KeyPress(object sender, KeyPressEventArgs e)
    {
        short num = checked((short)Strings.Asc(e.KeyChar));
        int num2 = Strings.Asc(e.KeyChar);
        if (num2 != 8
                && num2 != 32 // Space
                && (num2 < 48 || num2 > 57) // 0-9
                && (num2 < 65 || num2 > 90) // A-Z
                && num2 != 95 // _
                && (num2 < 97 || num2 > 122) // a-z
                && num2 != 223 // ß 
            || 1 == 0) // reserve for future use
        {
            e.Handled = true;
        }
        if (num == 13) // Enter
        {
            View.Command1.PerformClick();
        }
    }

    public void Laufwerk1_SelectedIndexChanged(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_0393
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
                            goto IL_Start;
                        case 1195:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_03e3;
                                    default:
                                        goto end_IL_0001;
                                }
                                goto IL_0395;
                            }

                        IL_Resume:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 2:
                                    goto IL_Start;
                                case 42:
                                    goto IL_0395;
                                default:
                                    goto end_IL_0001;
                                case 5:
                                case 11:
                                case 41:
                                case 46:
                                    goto end_IL_0001_2;
                            }
                            goto default;

                        IL_Start:
                            num = 2;
                            Modul1.cMandDrive = new DriveInfo(View.Laufwerk1.Text);
                            if (!Modul1.cMandDrive.IsReady)
                            {
                                _ = Interaction.MsgBox($"Laufwerk {View.Laufwerk1.Text.ToUpper()} ist nicht bereit", title: "Bedienungsfehler", mb: MessageBoxButtons.OK);
                                // return
                                goto end_IL_0001_2;
                            }
                            if (!Directory.Exists(Modul1.GenFreeDir))
                            {
                                _ = Interaction.MsgBox($"Auf Laufwerk {View.Laufwerk1.Text.ToUpper()} befinden sich keine {Modul1.AppName}-Dateien", title: "", mb: MessageBoxButtons.OK);
                                View.Laufwerk1.Text = Modul1.Verz.Left(2);
                                // return
                                goto end_IL_0001_2;
                            }
                            Modul1.Verz1 = Modul1.GenFreeDir;
                            View.BezMAND.Text = Modul1.Verz;
                            List1_Items.Clear();
                            foreach (var item in Modul1.EnumerateMandants(Modul1.cMandDrive.Name))
                            {
                                List1_Items.Add(item);
                            }

                            if (Modul1.System.xDemo)
                            {
                                if (List1_Items.Count > 1)
                                {
                                    _ = Interaction.MsgBox(Modul1.Message_sDemoVerNotPossibl);
                                    View.cmdNewMandant.Visible = false;
                                }
                            }
                            if (Modul1.Verz1.Right(1) != "\\")
                            {
                                Modul1.Verz1 += "\\";
                            }
                            View.Label2.Text = View.List1.Text.Left(Modul1.Verz1.Length);
                            goto end_IL_0001_2;
                            //== Error Handling ==

                        IL_0395:
                            num = 42;
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
                            goto IL_Resume;

                        IL_03e3:
                            num4 = unchecked(num2 + 1);
                            goto IL_Resume;

                        end_IL_0001:
                            break;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0001_dispatch = 1195;
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
