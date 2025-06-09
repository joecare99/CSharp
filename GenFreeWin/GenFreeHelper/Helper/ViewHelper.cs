using Gen_FreeWin;
using GenFree.Interfaces.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GenFree.Helper;

public static class ViewHelper
{
    public static void SetCommandBtn(this Button button, bool xCond, string sBtnHeader,IApplUserTexts IText)
    {
        button.Text = sBtnHeader + ":" + IText[EUserText.tNo];
        button.BackColor = Color.FromArgb(0xE0E0E0);
        if (xCond)
        {
            button.Visible = true;
            button.Text = sBtnHeader + ":" + IText[EUserText.tYes];
            button.BackColor = Color.FromArgb(0xC0FFFF);
        }
    }

    public static void SetLabelTxt(this System.Windows.Forms.Label label, bool xCond, string sLblHeader, Color backColor, IApplUserTexts IText)
    {
        label.Text = sLblHeader + ":" + IText[EUserText.tNo];
        label.BackColor = backColor;
        if (xCond)
        {
            label.Visible = true;
            label.Text = sLblHeader + ":" + IText[EUserText.tYes];
            label.BackColor = Color.FromArgb(0xC0FFFF);
            label.Refresh();
        }
    }

}
