using BaseLib.Models.Interfaces;
using GenFree;
using GenFree.Interfaces.Sys;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Views;

namespace GenFreeWin.Views;

[DesignerGenerated]
public partial class Ortsver : Form
{
    private static List<WeakReference> __ENCList = new();

    private IOrtsVerViewModel _viewModel;

    public Keys ModifierKeys => ModifierKeys;
    public Ortsver(IOrtsVerViewModel viewModel)
    {
        _viewModel = viewModel;
        _viewModel.View = this;
        Load += Form_Load;
        Load += Form_Load;
        lock (__ENCList)
        {
            __ENCList.Add(new WeakReference(this));
        }
        InitializeComponent();
        CommandBindingAttribute.Commit(this, _viewModel);
    }

    IModul1 Modul1 => _Modul1.Instance;
    private void Form_Load(object sender, EventArgs e)
    {
        var WinPath = Environment.GetEnvironmentVariable("Windir");

        RTB1.AddContextMenu();
        if (Modul1.FontSize > 0f)
        {
            Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            Font = new Font("Courier New", Modul1.FontSize, FontStyle.Regular);
            Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
        }
        var aiPos = Modul1.Persistence.ReadIntsProg("maspos.dat", 2);
        Left = aiPos[0];
        Top = aiPos[1];
        Text = $"{Modul1.AppName} Ortsverwaltung für Mandant {Modul1.Mandant}";
        BackColor = Modul1.HintFarb;
        Modul1.Persistence.ReadEnumInit<FormWindowState>("Windowstate", out var WiS);
        WindowState = WiS;

    }

    private void TextBox1_KeyUp(object s, KeyEventArgs e) => _viewModel.TextBox1_KeyUp(s, e);
    private void Textbox1_KeyPress(object s, KeyPressEventArgs e)
    {
        short num = checked((short)Strings.Asc(e.KeyChar));
        if ((ModifierKeys & Keys.Alt) != 0)
        {
            return;
        }

        string name = ((TextBox)s).Name;
        if (name == edtPlace.Name)
        {
            if (num == 13)
            {
                _ = edtSuburb.Focus();
            }
        }
        else if (name == edtSuburb.Name)
        {
            if (num == 13)
            {
                _ = edtCounty.Focus();
            }
        }
        else if (name == edtCounty.Name)
        {
            if (num == 13)
            {
                _ = edtCountry.Focus();
            }
        }
        else if (name == edtCountry.Name)
        {
            if (num == 13)
            {
                _ = edtState.Focus();
            }
        }
        else if (name == edtState.Name)
        {
            if (num == 13)
            {
                _ = edtLocator.Focus();
            }
        }
        else if (name == edtLocator.Name)
        {
            if (num == 13)
            {
                _ = edtGOV.Focus();
            }
        }
        else if (name == edtGOV.Name)
        {
            if (num == 13)
            {
                _ = edtLat1.Focus();
            }
        }
        else if (name == edtLat1.Name)
        {
            if (num == 13)
            {
                _ = edtLat2.Focus();
            }
        }
        else if (name == edtLat2.Name)
        {
            if (num == 13)
            {
                _ = edtLat3.Focus();
            }
        }
        else if (name == edtLat3.Name)
        {
            if (num == 13)
            {
                _ = edtLong1.Focus();
            }
        }
        else if (name == edtLong1.Name)
        {
            if (num == 13)
            {
                _ = edtLong2.Focus();
            }
        }
        else if (name == edtLong2.Name)
        {
            if (num == 13)
            {
                _ = edtLong3.Focus();
            }
        }
        else if (name == edtLong3.Name)
        {
            Locber();
            if (num == 13)
            {
                _ = edtAdditional.Focus();
            }
        }
        else if (name == edtAdditional.Name)
        {
            if (num == 13)
            {
                _ = edtPolName.Focus();
            }
        }
        else if (name == edtPolName.Name)
        {
            if (num == 13)
            {
                _ = edtZIP.Focus();
            }
        }
        else if (name == edtZIP.Name)
        {
            if (num == 13)
            {
                _ = TextBox17.Focus();
            }
        }
        else if (name == TextBox17.Name && num == 13)
        {
            _ = TextBox18.Focus();
        }
    }
 

    private void TextBox13_TextChanged(object s, EventArgs e) => _viewModel.TextBox13_TextChanged(s, e);
    private void TextBox30_TextChanged(object s, EventArgs e) => _viewModel.TextBox30_TextChanged(s, e);
    private void TextBox31_TextChanged(object s, EventArgs e) => _viewModel.TextBox31_TextChanged(s, e);
    private void TextBox32_TextChanged(object s, EventArgs e) => _viewModel.TextBox32_TextChanged(s, e);

    private void ListBox1_DoubleClick(object s, EventArgs e) => _viewModel.ListBox1_DoubleClick(s, e);
    private void ListBox2_DoubleClick(object s, EventArgs e) => _viewModel.ListBox2_DoubleClick(s, e);
    private void ListBox3_DoubleClick(object s, EventArgs e) => _viewModel.ListBox3_DoubleClick(s, e);
    private void ListBox4_DoubleClick(object s, EventArgs e) => _viewModel.ListBox4_DoubleClick(s, e);

    private void Label27_TextChanged(object s, EventArgs e) => _viewModel.Label27_TextChanged(s, e);
    private void Label32_TextChanged(object s, EventArgs e) => _viewModel.Label32_TextChanged(s, e);

    private void Button24_Click(object s, EventArgs e) => _viewModel.Button24_Click(s, e);
    private void Button23_Click(object s, EventArgs e) => _viewModel.Button23_Click(s, e);
    private void Button22_Click(object s, EventArgs e) => _viewModel.Button22_Click(s, e);
    private void Button21_Click(object s, EventArgs e) => _viewModel.Button21_Click(s, e);
    private void Button20_Click(object s, EventArgs e) => _viewModel.Button20_Click(s, e);
    private void Button19_Click(object s, EventArgs e) => _viewModel.Button19_Click(s, e);
    private void Button18_Click(object s, EventArgs e) => _viewModel.Button18_Click(s, e);
    private void Button17_Click(object s, EventArgs e) => _viewModel.Button17_Click(s, e);
    private void Button16_Click(object s, EventArgs e) => _viewModel.Button16_Click(s, e);
    private void Button15_Click(object s, EventArgs e) => _viewModel.Button15_Click(s, e);
    private void Button14_Click(object s, EventArgs e) => _viewModel.Button14_Click(s, e);
    private void Button13_Click(object s, EventArgs e) => _viewModel.Button13_Click(s, e);
    private void Button12_Click(object s, EventArgs e) => _viewModel.Button12_Click(s, e);
    private void Button11_Click(object s, EventArgs e) => _viewModel.Button11_Click(s, e);
    private void Button10_Click(object s, EventArgs e) => _viewModel.Button10_Click(s, e);
    private void RTB1_KeyUp(object s, KeyEventArgs e) => _viewModel.RTB1_KeyUp(s, e);

}
