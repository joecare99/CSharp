using BaseLib.Helper;
using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualBasic;
using System;
using System.Windows.Forms;

namespace Gen_FreeWin.ViewModels;

public partial class NamenSuchViewModel
{
    [Obsolete]
    public void Check2_CheckStateChanged(object eventSender, EventArgs eventArgs)
    {
        if (eventSender == UiForm.chbFamOnly)
        {
            Male2_Checked = false;
            Female2_Checked = false;
            Male_Checked = false;
            Females_Checked = false;
        }

        if (eventSender != UiForm.chbSelection)
        {
            if (FamOnly_Checked)
            {
                Male2_Visible = false;
                Female2_Visible = false;

                Male_Visible = true;
                Females_Visible = true;
            }
            else
            {
                Male_Visible = false;
                Females_Visible = false;

                Male2_Visible = true;
                Female2_Visible = true;
            }

            _ = ComboBox1.Focus();
        }
    }

    [Obsolete]
    public void Check2_MouseDown(object eventSender, MouseEventArgs eventArgs)
    {
        ;
    }

    private void chbOmitSpouse_CheckStateChanged(object eventSender, EventArgs eventArgs)
    {
        Male_Checked = false;
        Females_Checked = false;
        FamOnly_Checked = false;
        if (OmitSpouse_Checked)
        {
            Label3_Text = "Name,Vorname                        Datum JJJJ  Personennr.";
            Male_Visible = false;
            Females_Visible = false;
            FamOnly_Visible = false;
        }
        else
        {
            Label3_Text = "Name,Vorname                        Datum JJJJ  Personennr. Heirat       Partner";
            Male2_Visible = true;
            Female2_Visible = true;
            FamOnly_Visible = true;
        }

        if (ComboBox1.Text != "")
        {
            StartSearch();
        }
    }

    private void Combo1_KeyPress(object eventSender, KeyPressEventArgs eventArgs)
    {
        short num = checked((short)Strings.Asc(eventArgs.KeyChar));
        _ = Interaction.MsgBox(num.ToString());
        if (num == 13)
        {
            StartSearch();
        }

        eventArgs.KeyChar = Strings.Chr(num);
        if (num != 0)
        {
        }
    }

    [Obsolete]
    private void Command1_Click(object eventSender, EventArgs eventArgs)
    {
        var Index = UiForm.Command1.GetIndex((Button)eventSender);

        Modul1.UbgT1 = "";
        switch (Index)
        {
            case 0:
                break;
            case 1:
                PersonSheet();
                break;
            case 2:
                FamilySheet();
                break;
            case 3:
                StartSearch();
                break;
            case 5:
            case 6:
                Command1_Section56(Index);
                break;
            case 7:
                PrintList();
                break;
            default:
                UiForm.Cursor = Cursors.Default;
                break;
        }
    }
}
