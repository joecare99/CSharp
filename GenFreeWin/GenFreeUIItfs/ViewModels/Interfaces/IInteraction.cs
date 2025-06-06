﻿using Microsoft.VisualBasic;
using System.Windows.Forms;

namespace GenFree.ViewModels.Interfaces;

public interface IInteraction
{
    void Beep();
    object? Choose(double v1,params object?[] v2);
    string? InputBox(string v,string title ="",string sDefault="");
    DialogResult MsgBox(string prompt, string title = "", MessageBoxButtons mb = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None);
    int Shell(string v, int winStyle = 1);
}